using System.Collections.Generic;
using eidng8.SpaceFlight.Components;
using Unity.Entities;
using UnityEngine;

namespace eidng8.SpaceFlight.Authoring
{
    [RequiresEntityConversion]
    [AddComponentMenu("eidng8/Authoring/Prefab")]
    public class PrefabEntityAuthoring
        : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
    {
        public GameObject prefab;

        public void Convert(
            Entity entity,
            EntityManager dstManager,
            GameObjectConversionSystem conversionSystem
        ) {
            if (null == this.prefab) { return; }

            PrefabComponent data = new PrefabComponent() {
                prefab = conversionSystem.GetPrimaryEntity(this.prefab),
            };
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
