using System.Collections.Generic;
using eidng8.SpaceFlight.Components;
using eidng8.SpaceFlight.Components.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

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

        protected virtual EntityCommandBuffer.Concurrent GetCommandBuffer() {
            return this.mCmd.CreateCommandBuffer().ToConcurrent();
        }
    }
}
