// ---------------------------------------------------------------------------
// <copyright file="EventChannels.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

using System.IO;
using eidng8.SpaceFlight.Configurable.Ship;
using eidng8.SpaceFlight.Configurable.System;
using eidng8.SpaceFlight.Entities;
using Unity.Entities;
using UnityEngine;

namespace eidng8.SpaceFlight.Managers
{
    /// <summary>
    ///     The game manager is global to the whole game. It doesn't need
    ///     to be
    ///     added to scene.
    /// </summary>
    public static class GameManager
    {
        public static StartupConfig StartupConfig =>
            Resources.Load<StartupConfig>(
                GameManager.DataFilePath("Startup Config")
            );

        /// <summary>Resource path to the given file.</summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string DataFilePath(string file) {
            return Path.Combine("Data", file);
        }

        [RuntimeInitializeOnLoadMethod(
            RuntimeInitializeLoadType.BeforeSplashScreen
        )]
        public static void GlobalInit() {
            Debug.Log($"GlobalInit {Time.realtimeSinceStartup}");
            GameManager.SetupGameState();
        }

        /// <summary>Resource path to the give prefab file.</summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string PrefabFilePath(string file) {
            return Path.Combine("Prefabs", file);
        }

        public static GameObject CreateShip() {
            Debug.Log($"GameObject creating {Time.realtimeSinceStartup}");
            var cfg = Resources.Load<ShipConfig>(
                GameManager.DataFilePath("Ships/Crosair")
            );
            GameObject go = Object.Instantiate(new GameObject());
            var pe = go.AddComponent<PrefabEntity>();
            pe.prefab = cfg.prefab;
            go.AddComponent<ConvertToEntity>();
            Debug.Log($"GameObject created {Time.realtimeSinceStartup}");
            return go;
        }
        
        /// <summary>Path to the file in the persistent storage.</summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string SavedFilePath(string file) {
            return Path.Combine(Application.persistentDataPath, "Player");
        }

        /// <summary>Sets up the scene after the scene has been loaded.</summary>
        [RuntimeInitializeOnLoadMethod(
            RuntimeInitializeLoadType.AfterSceneLoad
        )]
        private static void LateSetupScene() {
            Debug.Log($"LateSetupScene {Time.realtimeSinceStartup}");
            GameManager.CreateShip();
        }

        /// <summary>
        ///     Fetches game state from persistent storage or network server.
        ///     Then
        ///     perform setup accordingly. This is run once while the game is
        ///     launched.
        /// </summary>
        private static void SetupGameState() { }

        /// <summary>Sets up the scene before before the scene were loaded.</summary>
        [RuntimeInitializeOnLoadMethod(
            RuntimeInitializeLoadType.BeforeSceneLoad
        )]
        private static void SetupScene() {
            Debug.Log($"SetupScene {Time.realtimeSinceStartup}");
        }
    }
}
