using Unity.Entities;
using Unity.Mathematics;

namespace eidng8.SpaceFlight.Components
{
    /// <summary>
    /// Anything that can move should has this component attached to it.
    /// </summary>
    public struct PowerComponent : IComponentData
    {
        public float power;
        public float capacitor;
        public float recharge;
    }
}
