using Unity.Entities;

namespace eidng8.SpaceFlight.Systems.Jobs
{
    public abstract class JobSystemWithCommandBuffer : JobComponentSystem
    {
        protected EndSimulationEntityCommandBufferSystem mCmd;

        protected override void OnCreate() {
            this.mCmd = this.World
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            base.OnCreate();
        }

        protected virtual EntityCommandBuffer.Concurrent CreateCommandBuffer() {
            return this.mCmd.CreateCommandBuffer().ToConcurrent();
        }
    }
}
