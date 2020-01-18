using Unity.Entities;
using Unity.Mathematics;

namespace eidng8.SpaceFlight.Components
{
    /// <summary>
    /// Anything that can move should has this component attached to it.
    /// </summary>
    public struct PropulsionComponent : IComponentData
    {
        /// <summary>
        /// Maximum force output on all three axis. The Z axis is divided into
        /// forward and backward.
        /// </summary>
        public float4 maxForces;

        /// <summary>
        /// Maximum torque output on all three axis.
        /// </summary>
        public float maxTorques;
    }
}
