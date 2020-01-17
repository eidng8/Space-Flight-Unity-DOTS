using System.Collections.Generic;
using eidng8.SpaceFlight.Components;
using eidng8.SpaceFlight.Components.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace eidng8.SpaceFlight.Systems.Jobs
{
    public class PrefabSpawningJob : JobComponentSystem
    {
        public const int QueueLength = 10;

        private static readonly List<PrefabComponent> Prefabs =
            new List<PrefabComponent>(PrefabSpawningJob.QueueLength);

        private EndSimulationEntityCommandBufferSystem _cmd;

        private JobHandle _lastJob;

        private NativeArray<PrefabComponent> _lastBatch;

        private bool _spawning;

        public static void Spawn(PrefabComponent prefab, int count = 1) {
            lock (PrefabSpawningJob.Prefabs) {
                for (int i = 0; i < count; i++) {
                    PrefabSpawningJob.Prefabs.Add(prefab);
                }
            }
        }

        public static void ResetQueue() {
            lock (PrefabSpawningJob.Prefabs) {
                PrefabSpawningJob.Prefabs.Clear();
                PrefabSpawningJob.Prefabs.Capacity =
                    PrefabSpawningJob.QueueLength;
            }
        }

        protected override void OnCreate() {
            this._cmd = this.World
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle dependsOn) {
            if (this._spawning) {
                if (this._lastJob.IsCompleted) {
                    this._lastBatch.Dispose();
                    this._spawning = false;
                } else {
                    return default;
                }
            }

            int count = PrefabSpawningJob.Prefabs.Count;
            if (count < 1) {
                return default;
            }

            lock (PrefabSpawningJob.Prefabs) {
                this._lastBatch = new NativeArray<PrefabComponent>(
                    PrefabSpawningJob.Prefabs.ToArray(),
                    Allocator.TempJob
                );
                PrefabSpawningJob.ResetQueue();
            }

            Job job = new Job() {
                cmd = this._cmd.CreateCommandBuffer().ToConcurrent(),
                prefabs = this._lastBatch,
            };
            this._lastJob = job.Schedule(count, 1, dependsOn);
            this._cmd.AddJobHandleForProducer(this._lastJob);

            this._spawning = true;

            return this._lastJob;
        }

        [BurstCompile]
        private struct Job : IJobParallelFor
        {
            [ReadOnly] public EntityCommandBuffer.Concurrent cmd;
            public NativeArray<PrefabComponent> prefabs;

            public void Execute(int index) {
                PrefabComponent prefab = this.prefabs[index];
                Entity e = this.cmd.Instantiate(index, prefab.prefab);
                this.cmd.AddComponent<JustSpawned>(index, e);
            }
        }
    }
}
