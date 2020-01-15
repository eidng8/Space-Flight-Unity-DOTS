using eidng8.SpaceFlight.Managers;
using UnityEngine;

namespace eidng8.SpaceFlight
{
    public class UI : MonoBehaviour
    {
        public void Spawn() {
            GameManager.CreateShip();
        }
    }
}
