using Unity.Entities;

namespace eidng8.SpaceFlight.Components.Configurable
{
    public struct EnergyComponent : IComponentData
    {
        public float capacitor;
        public float recharge;
    }
}
