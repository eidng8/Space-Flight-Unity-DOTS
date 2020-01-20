using System;
using eidng8.SpaceFlight.Managers;
using Unity.Entities;

namespace eidng8.SpaceFlight.Components
{
    /// <summary>
    ///     Bridge between prefab (converted) entity and prefab resources.
    ///     There
    ///     can be exactly one type to one prefab entity. Types are defined
    ///     by
    ///     <see cref="PrefabTypes" />. ALl comparison are done with
    ///     <see cref="type" /> only.
    /// </summary>
    public struct PrefabComponent
        : IComponentData,
            IComparable<PrefabComponent>,
            IEquatable<PrefabComponent>
    {
        /// <summary>
        ///     The integer representation of <see cref="PrefabTypes" />.
        /// </summary>
        public readonly int type;

        /// <summary>
        ///     The corresponding prefab (converted) entity.
        /// </summary>
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
