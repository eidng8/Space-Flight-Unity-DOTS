﻿// ---------------------------------------------------------------------------
// <copyright file="EventChannels.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

using System.IO;
using eidng8.SpaceFlight.Authoring;
using eidng8.SpaceFlight.Components;
using eidng8.SpaceFlight.Configurable.Ship;
using eidng8.SpaceFlight.Configurable.System;
using eidng8.SpaceFlight.Entities;
using eidng8.SpaceFlight.Systems.Jobs;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        public static void CreateShip(PrefabTypes type, int count = 1) {
            EntityManager em = World.Active.EntityManager;
            SpawnPrefab s = new SpawnPrefab() {
                type = (int)type,
                count = count,
            };
            EntityArchetype t = em.CreateArchetype(
                ComponentType.ReadWrite<SpawnPrefab>()
            );
            Entity e = em.CreateEntity(t);
            em.SetComponentData(e, s);
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
