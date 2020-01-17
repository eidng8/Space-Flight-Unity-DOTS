using Unity.Entities;

namespace eidng8.SpaceFlight.Components
{
    public struct SpawnPrefab : IComponentData
    {
        public int type;
        public int count;
    }
}
