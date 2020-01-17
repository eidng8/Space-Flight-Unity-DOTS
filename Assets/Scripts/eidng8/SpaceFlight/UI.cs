using eidng8.SpaceFlight.Managers;
using eidng8.SpaceFlight.Systems;
using UnityEngine;

namespace eidng8.SpaceFlight
{
    public class UI : MonoBehaviour
    {
        public void Spawn() {
            SpawnSystem.SpawnPrefab(PrefabTypes.Crosair, 100);
        }
    }
}
