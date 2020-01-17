using System;
using System.Text.RegularExpressions;

namespace eidng8.SpaceFlight.Managers
{
    /// <summary>
    ///     Prefab type identifiers. Currently I haven't found out a way to
    ///     bridge
    ///     between prefab resources and entities. In order to spawn any
    ///     cached
    ///     prefab loaded in sub-scenes, I have to manually link up the
    ///     two. This
    ///     enumeration is the link I've came up with. Every prefab has
    ///     exactly one
    ///     corresponding member in this enumeration.
    ///     The
    ///     <see cref="eidng8.SpaceFlight.Components.PrefabComponent" />
    ///     has
    ///     a field of this type. Which is assigned by
    ///     <see cref="eidng8.SpaceFlight.Authoring.PrefabEntityAuthoring" />
    ///     during
    ///     conversion. Which in turn is manually chosen in the inspector.
    /// </summary>
    public enum PrefabTypes
    {
        Crosair = 1
    }

    public static class PrefabTypesExtension
    {
        public static PrefabTypes ToPrefabTypes(this string name) {
            Regex regex = new Regex("[^A-Za-z0-9]+");
            string s = regex.Replace(name, "_");
            return (PrefabTypes)Enum.Parse(typeof(PrefabTypes), s);
        }
    }
}
