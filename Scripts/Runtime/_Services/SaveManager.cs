// File: Assets/Core/Scripts/SaveManager.cs
using System;
using System.Collections;
using Core._Services._Saving;
using UnityEngine;
using Zenject;

namespace Core._Services {
    public class SaveManager<T> : ITickable, IDisposable where T : new() {
        protected readonly IDataSaver _dataSaver;
        protected readonly string _saveKey;
        
        public T Data { get; protected set; }
        
        private bool _isDirty;
        private float _timer;
        private const float AUTO_SAVE_DELAY = 5f; // reduced freq for web performance

        public SaveManager(IDataSaver dataSaver, string saveKey) {
            _dataSaver = dataSaver;
            _saveKey = saveKey;
            Data = new T();
        }

        public virtual IEnumerator Initialize() {
            var isDone = false;
            var json = string.Empty;

            // async load from sdk
            yield return _dataSaver.Load(_saveKey, result => {
                Debug.Log($"SaveManager__ json: {json}");
                json = result;
                isDone = true;
            });

            yield return new WaitUntil(() => isDone);

            if (!string.IsNullOrEmpty(json)) {
                try {
                    Data = JsonUtility.FromJson<T>(json);
                } catch (Exception e) {
                    Debug.LogError($"// error parsing save: {e.Message}");
                    Data = new T();
                }
            }
        }

        public void MarkDirty() => _isDirty = true;

        public void SaveImmediate() {
            if (!_isDirty) return;
            ForceSave();
        }

        private void ForceSave() {
            var json = JsonUtility.ToJson(Data);
            _dataSaver.Save(_saveKey, json);
            _isDirty = false;
            _timer = 0;
            // Debug.Log("// data saved to cloud");
        }

        public void Tick() {
            if (!_isDirty) return;

            _timer += Time.deltaTime;
            if (_timer >= AUTO_SAVE_DELAY) {
                ForceSave();
            }
        }

        public void Dispose() {
            SaveImmediate(); // final save on exit
        }
    }
}