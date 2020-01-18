using Unity.Entities;
using Unity.Mathematics;

namespace eidng8.SpaceFlight.Components
{
    public struct DefenseComponent : IComponentData
    {
        public float armor;
        public float shield;
    }
}
