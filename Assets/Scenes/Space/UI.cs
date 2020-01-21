using eidng8.SpaceFlight.Configurable.Ship;
using eidng8.SpaceFlight.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            var created = PrefabManager.Instantiate(
                PrefabTypes.Crosair,
                int.Parse(quantityLabel.text),
                cfg
            );

            if (!created) Debug.Log("Failed to create prefab");
        }

        public void QuantityChanged(float value)
        {
            quantityLabel.text = $"{(int) value}";
        }

        public void SwitchScene()
        {
            SceneManager.LoadScene("Scenes/Space/Space");
        }
    }
}