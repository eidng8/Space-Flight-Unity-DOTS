using eidng8.SpaceFlight.Managers;
using eidng8.SpaceFlight.Systems;
using UnityEngine;

namespace eidng8.SpaceFlight
{
    public class UI : MonoBehaviour
    {
        public void Spawn() {
            PrefabCacheManager.Instantiate(PrefabTypes.Crosair, 100);
        }
    }
}
