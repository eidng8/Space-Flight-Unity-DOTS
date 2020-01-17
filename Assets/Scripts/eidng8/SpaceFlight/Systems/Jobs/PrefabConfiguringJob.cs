using eidng8.SpaceFlight.Components;
using eidng8.SpaceFlight.Components.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace eidng8.SpaceFlight.Systems.Jobs
{
    public class PrefabConfiguringJob : JobSystemWithCommandBuffer
    {
        protected override JobHandle OnUpdate(JobHandle dependsOn) {
            Job job = new Job() {cmd = this.GetCommandBuffer()};
            JobHandle h = job.Schedule(this, dependsOn);
            this.mCmd.AddJobHandleForProducer(h);
            return h;
        }

        [BurstCompile]
        private struct Job : IJobForEachWithEntity<PrefabComponent, JustSpawned>
        {
            [ReadOnly] public EntityCommandBuffer.Concurrent cmd;

            public void Execute(
                Entity entity,
                int index,
                [ReadOnly] ref PrefabComponent c0,
                [ReadOnly] ref JustSpawned _
            ) {
                // this.cmd.RemoveComponent<JustSpawned>(index, entity);
                // this.cmd.AddComponent<Alive>(index, entity);
            }
        }
    }
}
