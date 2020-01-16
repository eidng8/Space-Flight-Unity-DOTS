using Unity.Entities;

namespace eidng8.SpaceFlight.Components
{
    public struct PrefabComponent : IComponentData
    {
        public Entity prefab;
    }
}
