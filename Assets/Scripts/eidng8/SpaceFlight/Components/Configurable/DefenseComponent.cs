using Unity.Entities;

namespace eidng8.SpaceFlight.Components.Configurable
{
    public struct DefenseComponent : IComponentData
    {
        public float armor;
        public float shield;
        public float shieldRecharge;
    }
}
