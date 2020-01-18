using eidng8.SpaceFlight.Managers;
using Unity.Entities;
using Unity.Mathematics;

namespace eidng8.SpaceFlight.Components
{
    public struct ConfigurableComponent : IComponentData
    {
        public PrefabTypes prefabType;
        public Entity prefab;

        public bool hasDefense;
        public DefenseComponent defense;

        public bool hasOffense;
        public OffenseComponent offense;

        public bool hasPower;
        public PowerComponent power;

        public bool hasPropulsion;
        public PropulsionComponent propulsion;
    }
}
