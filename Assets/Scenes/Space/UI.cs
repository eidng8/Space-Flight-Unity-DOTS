using eidng8.SpaceFlight.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace eidng8.SpaceFlight
{
    public class UI : MonoBehaviour
    {
        public Text quantityLabel;

        public void Spawn()
        {
            PrefabCacheManager.Instantiate(
                PrefabTypes.Crosair,
                int.Parse(quantityLabel.text)
            );
        }

        public void QuantityChanged(float value)
        {
            quantityLabel.text = $"{(int) value}";
        }
    }
}