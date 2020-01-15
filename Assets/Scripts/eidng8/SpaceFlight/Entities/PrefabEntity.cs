using System.Collections.Generic;
using eidng8.SpaceFlight.Components;
using Unity.Entities;
using UnityEngine;

namespace eidng8.SpaceFlight.Entities
{
    public class PrefabEntity
        : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
    {
        public GameObject prefab;

        public void Convert(
            Entity entity,
            EntityManager dstManager,
            GameObjectConversionSystem conversionSystem
        ) {
            Debug.Log($"Convert {Time.realtimeSinceStartup}");
            Entity prefabEntity =
                conversionSystem.GetPrimaryEntity(this.prefab);
            MovableComponent data = new MovableComponent() {
                entity = prefabEntity
            };
            dstManager.AddComponentData(entity, data);
            Debug.Log($"Convert done {Time.realtimeSinceStartup}");
        }

        public void DeclareReferencedPrefabs(
            List<GameObject> referencedPrefabs
        ) {
            referencedPrefabs.Add(this.prefab);
        }
    }
}
