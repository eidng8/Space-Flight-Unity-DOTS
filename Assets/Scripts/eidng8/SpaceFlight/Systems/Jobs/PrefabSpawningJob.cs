using System.Collections.Generic;
using eidng8.SpaceFlight.Components;
using eidng8.SpaceFlight.Managers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace eidng8.SpaceFlight.Systems.Jobs
{
    /// <summary>
    ///     The job scheduler for spawning prefab entities. This is for use
    ///     in conjunction with
    ///     <see cref="PrefabManager" />.
    /// </summary>
    public class PrefabSpawningJob : JobSystemWithCommandBuffer
    {
        public const int QueueLength = 10;

        public const int BatchSize = 4;

        /// <summary>
        ///     The list of prefabs to be instantiated, a.k.a. the queue.
        ///     There's a mutex on this list.
        /// </summary>
        private static readonly List<Request> Queue =
            new List<Request>(PrefabSpawningJob.QueueLength);

        private NativeArray<Request> _lastBatch;

        private JobHandle _lastJob;

        private bool _spawning;

        /// <summary>
        ///     Add a spawn request to queue. The queue is locked during this
        ///     call.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="count"></param>
        public static void Spawn(Request request, int count = 1) {
            lock (PrefabSpawningJob.Queue) {
                for (int i = 0; i < count; i++) {
                    PrefabSpawningJob.Queue.Add(request);
                }
            }
        }

        /// <summary>
        ///     Clear the spawn queue. The queue is locked during this call.
        /// </summary>
        public static void ResetQueue() {
            lock (PrefabSpawningJob.Queue) {
                PrefabSpawningJob.Queue.Clear();
                PrefabSpawningJob.Queue.Capacity =
                    PrefabSpawningJob.QueueLength;
            }
        }

        protected override JobHandle OnUpdate(JobHandle dependsOn) {
            // The dispatching of jobs is single threaded.
            // It is not necessary to consider racing condition while accessing
            // the `_spawning` flag.
            if (this._spawning) {
                if (this._lastJob.IsCompleted) {
                    this._lastBatch.Dispose();
                    this._spawning = false;
                } else {
                    return default;
                }
            }

            int count = PrefabSpawningJob.Queue.Count;
            if (count < 1) {
                return default;
            }

            // Acquire a mutex on the job queue first
            lock (PrefabSpawningJob.Queue) {
                this._lastBatch = new NativeArray<Request>(
                    PrefabSpawningJob.Queue.ToArray(),
                    Allocator.TempJob
                );
                // Everything has been moved to the native array.
                // We can clear the job queue now.
                PrefabSpawningJob.ResetQueue();
            }

            // Actually schedule a job
            Job job = new Job {
                cmd = this.CreateCommandBuffer(),
                queue = this._lastBatch
            };
            this._lastJob = job.Schedule(
                count,
                PrefabSpawningJob.BatchSize,
                dependsOn
            );
            this.mCmd.AddJobHandleForProducer(this._lastJob);

            // Set the flag so we won't be running scheduling until the current
            // job is done.
            this._spawning = true;

            return this._lastJob;
        }

        public struct Request
        {
            public Entity prefab;
            public bool hasConfig;
            public ConfigurableComponent config;
        }

        [BurstCompile]
        private struct Job : IJobParallelFor
        {
            [ReadOnly] public EntityCommandBuffer.Concurrent cmd;
            public NativeArray<Request> queue;

            public void Execute(int index) {
                Request request = this.queue[index];
                Entity e = this.cmd.Instantiate(index, request.prefab);

                // Add the tag component so others can work specifically
                // on newly created entities.
                // this.cmd.AddComponent<JustSpawned>(index, e);

                if (request.hasConfig) {
                    ConfigurableComponent cfg = request.config;
                    if (cfg.hasPropulsion) {
                        this.cmd.SetComponent(index, e, cfg.propulsion);
                    }

                    if (cfg.hasDefense) {
                        this.cmd.SetComponent(index, e, cfg.defense);
                    }

                    if (cfg.hasOffense) {
                        this.cmd.SetComponent(index, e, cfg.offense);
                    }

                    if (cfg.hasPower) {
                        this.cmd.SetComponent(index, e, cfg.power);
                    }
                }
            }
        }
    }
}
