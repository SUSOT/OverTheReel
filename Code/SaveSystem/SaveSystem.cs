using System;
using System.Collections.Generic;
using System.IO;
using _01.Scripts.Manager;
using Newtonsoft.Json;
using UnityEngine;

namespace _01.Scripts.SaveSystem
{
    public static class SaveSystem
    {
        private const string SaveFileName = "save.json";
        private const string WebGlSaveKey = "fishing_game_save_json";

        private static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

        public static void Save(GameSaveData data)
        {
            GameSaveData saveData = data ?? new GameSaveData();
            SetDefaultData(saveData);

            string json = JsonConvert.SerializeObject(saveData);
#if UNITY_WEBGL && !UNITY_EDITOR
            PlayerPrefs.SetString(WebGlSaveKey, json);
            PlayerPrefs.Save();
#else
            string directory = Path.GetDirectoryName(SavePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            try
            {
                File.WriteAllText(SavePath, json);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"SaveSystem write failed: {exception.Message}");
            }
#endif
        }

        public static GameSaveData Load()
        {
            string json;

#if UNITY_WEBGL && !UNITY_EDITOR
            json = PlayerPrefs.GetString(WebGlSaveKey, string.Empty);
#else
            if (!File.Exists(SavePath))
            {
                return new GameSaveData();
            }

            try
            {
                json = File.ReadAllText(SavePath);
            }
            catch
            {
                return new GameSaveData();
            }
#endif

            if (string.IsNullOrWhiteSpace(json))
            {
                return new GameSaveData();
            }

            try
            {
                GameSaveData data = JsonConvert.DeserializeObject<GameSaveData>(json);
                if (data == null)
                {
                    return new GameSaveData();
                }

                SetDefaultData(data);
                return data;
            }
            catch
            {
                return new GameSaveData();
            }
        }

        private static void SetDefaultData(GameSaveData data)
        {
            data.fishMaxSizes ??= new Dictionary<string, int>();
            data.unlockedMaps ??= new List<Map>();
            data.uiPanelStates ??= new Dictionary<string, bool>();
            data.fishCaughtStates ??= new Dictionary<string, bool>();
            data.savedFishIDs ??= new List<string>();
        }
    }
}
