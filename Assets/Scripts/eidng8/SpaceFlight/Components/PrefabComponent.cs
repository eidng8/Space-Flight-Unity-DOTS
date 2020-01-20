using System;
using eidng8.SpaceFlight.Managers;
using Unity.Entities;

namespace eidng8.SpaceFlight.Components
{
    public struct PrefabComponent
        : IComponentData,
            IComparable<PrefabComponent>,
            IEquatable<PrefabComponent>
    {
        public readonly int type;
        public readonly Entity prefab;

        public PrefabComponent(PrefabTypes type, Entity prefab) {
            this.type = (int)type;
            this.prefab = prefab;
        }

        public int CompareTo(PrefabComponent other) {
            return this.type.CompareTo(other.type);
        }

        public bool Equals(PrefabComponent other) {
            return this.type == other.type;
        }

        public override bool Equals(object obj) {
            return obj is PrefabComponent other && this.Equals(other);
        }

        public override int GetHashCode() {
            return this.type;
        }
    }
}
