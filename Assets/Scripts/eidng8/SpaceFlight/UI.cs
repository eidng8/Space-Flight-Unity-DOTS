using eidng8.SpaceFlight.Authoring;
using eidng8.SpaceFlight.Components;
using eidng8.SpaceFlight.Managers;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace eidng8.SpaceFlight
{
    public class UI : MonoBehaviour
    {
        public void Spawn() {
            GameManager.CreateShip(PrefabTypes.Crosair, 100);
        }
    }
}
