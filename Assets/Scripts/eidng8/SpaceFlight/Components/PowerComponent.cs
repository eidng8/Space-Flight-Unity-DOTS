using Unity.Entities;
using Unity.Mathematics;

namespace eidng8.SpaceFlight.Components
{
    public struct PowerComponent : IComponentData
    {
        public float power;
        public float capacitor;
        public float recharge;
    }
}
