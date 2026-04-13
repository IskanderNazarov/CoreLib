// File: Assets/Core/Scripts/Editor/DataSaver_Editor.cs
using System;
using System.Collections;
using System.IO;
using Core._Services._Saving;
using UnityEngine;

namespace _Services._Saving {
    public class DataSaver_Editor : IDataSaver {
        private readonly string _saveFolder;

        public DataSaver_Editor() {
            // create directory in project folder
            _saveFolder = Path.Combine(Directory.GetCurrentDirectory(), "Saves");
            if (!Directory.Exists(_saveFolder)) {
                Directory.CreateDirectory(_saveFolder);
            }
        }

        public IEnumerator Load(string key, Action<string> onLoaded) {
            var path = GetSavePath(key);
            var json = string.Empty;

            if (File.Exists(path)) {
                json = File.ReadAllText(path);
            }

            yield return null; // simulate async
            onLoaded?.Invoke(json);
        }

        public void Save(string key, string json) {
            var path = GetSavePath(key);
            File.WriteAllText(path, json);
            // Debug.Log($"// local save updated at: {path}");
        }

        public void Delete(string key) {
            var path = GetSavePath(key);
            if (File.Exists(path)) {
                File.Delete(path);
            }
        }

        private string GetSavePath(string key) {
            return Path.Combine(_saveFolder, $"{key}.json");
        }
    }
}