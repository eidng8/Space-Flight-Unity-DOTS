using System.Collections.Generic;
using eidng8.SpaceFlight.Components;
using eidng8.SpaceFlight.Managers;
using Unity.Entities;
using UnityEngine;

namespace eidng8.SpaceFlight.Authoring
{
    /// <summary>
    ///     Converts prefab to entity.
    /// </summary>
    [RequiresEntityConversion]
    [AddComponentMenu("eidng8/Authoring/Prefab")]
    public class PrefabEntityAuthoring
        : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
    {
        /// <summary>
        ///     The prefab to be converted.
        /// </summary>
        [Tooltip("The prefab to be converted.")]
        public GameObject prefab;

        /// <summary>
        ///     This is used to bridge between prefab and entity. It is not
        ///     possible
        ///     to use string in <see cref="IComponentData" />. So we have to
        ///     make
        ///     use of something else. This property is casted to <c>int</c>
        ///     during
        ///     conversion, then set to <c>type</c> field of the underlying
        ///     <see cref="PrefabComponent" />.
        /// </summary>
        [Tooltip(
            "This is used to bridge between prefab and entity.\n"
            + "This property is casted to int during conversion,\n"
            + "then set to <c>type</c> field of the underlying PrefabComponent"
        )]
        public PrefabTypes type;

        public void Convert(
            Entity entity,
            EntityManager dstManager,
            GameObjectConversionSystem conversionSystem
        ) {
            if (null == this.prefab) { return; }

            PrefabComponent data = new PrefabComponent(
                this.type,
                conversionSystem.GetPrimaryEntity(this.prefab)
            );
            dstManager.AddComponentData(entity, data);
        }

        public void DeclareReferencedPrefabs(
            List<GameObject> referencedPrefabs
        ) {
            if (null == this.prefab) { return; }

            referencedPrefabs.Add(this.prefab);
        }
    }
}
