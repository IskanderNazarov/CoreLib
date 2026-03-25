// file: assets/_coregame/_scripts/boosters/boosterinventory.cs
// assembly: corelib.asmdef

using System;
using System.Collections.Generic;
using Core._Services._Saving;
using UnityEngine;
using Zenject;

namespace core.boosters {
    // the implementation is now generic
    public class BoosterInventory<TBoosterId> : IBoosterInventory<TBoosterId>, IInitializable
        where TBoosterId : Enum {
        private readonly IDataSaver _dataSaver;
        private Dictionary<TBoosterId, int> _caps;
        public event Action<TBoosterId, int> OnChanged;

        //[Inject] private IBoosterKeysProvider<TBoosterId> _keysProvider;
        private IBoosterKeysProvider<TBoosterId> _keysProvider;

        // note: zenject can inject dependencies into generic classes
        public BoosterInventory(IDataSaver dataSaver, /* Dictionary<TBoosterId, int> defaultValues,*/ Dictionary<TBoosterId, int> caps,
            IBoosterKeysProvider<TBoosterId> keysProvider) {
            _dataSaver = dataSaver;
            _caps = caps;
            _keysProvider = keysProvider;
        }

        public void Initialize() {
            //
        }

        public int GetCount(TBoosterId id) {
            return _dataSaver.GetDataInt(GetBoosterSaveKey(id));
        }

        // ... (вся остальная логика GetCap, SetCap, TryAdd, TryConsume) ...
        // ... она остается такой же, просто 'BoosterId' заменен на 'TBoosterId' ...

        public bool TryAdd(TBoosterId id, int amount, string source) {
            if (amount <= 0) return false;
            var current = GetCount(id);
            var cap = GetCap(id);
            var newValue = Mathf.Min(current + amount, cap);
            var delta = newValue - current;
            if (delta <= 0) return false;

            Save(id, newValue);
            OnChanged?.Invoke(id, delta);
            return true;
        }

        public bool TryConsume(TBoosterId id, int amount, string reason) {
            if (amount <= 0) return false;
            var current = GetCount(id);
            if (current < amount) return false;

            var newValue = current - amount;
            Save(id, newValue);
            OnChanged?.Invoke(id, -amount);
            return true;
        }

        public int GetCap(TBoosterId id) {
            return _caps.GetValueOrDefault(id, int.MaxValue);
        }

        public void SetCap(TBoosterId id, int cap) {
            _caps[id] = Mathf.Max(0, cap);
        }

        private void Save(TBoosterId id, int newValue) {
            _dataSaver.SetData(GetBoosterSaveKey(id), newValue.ToString());
        }

        private string GetBoosterSaveKey(TBoosterId boosterId) {
            // using the enum's name as the key
            //return $"booster_{boosterId}";
            return _keysProvider.GetSaveKey(boosterId);
        }
    }
}