using eidng8.SpaceFlight.Configurable.Ship;
using eidng8.SpaceFlight.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Space
{
    public class UI : MonoBehaviour
    {
        public Text quantityLabel;

        public void Spawn()
        {
            var cfg = Resources.Load<ShipConfig>(
                GameManager.DataFilePath("Ships/Crosair"));
            PrefabCacheManager.Instantiate(
                PrefabTypes.Crosair,
                int.Parse(quantityLabel.text),
                cfg
            );
        }

        public void QuantityChanged(float value)
        {
            quantityLabel.text = $"{(int) value}";
        }
    }
}