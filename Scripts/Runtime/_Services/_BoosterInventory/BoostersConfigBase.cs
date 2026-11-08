using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Services._BoosterInventory {
    /// <summary>
    /// Base ScriptableObject for generic booster configurations with A/B scenario testing support.
    /// </summary>
    public abstract class BoostersConfigBase<TBoosterId> : ScriptableObject where TBoosterId : Enum {
        [Header("A/B Scenario Configuration")]
        [Tooltip("Choose which booster scenario is active for gameplay")]
        public BoosterScenarioType ActiveScenario = BoosterScenarioType.Classic;

        [Header("Booster Definitions")]
        public List<BoosterInfo<TBoosterId>> Boosters = new List<BoosterInfo<TBoosterId>>();

        public BoosterInfo<TBoosterId> GetBooster(TBoosterId id) {
            return Boosters != null ? Boosters.Find(b => EqualityComparer<TBoosterId>.Default.Equals(b.BoosterId, id)) : null;
        }

        public Dictionary<TBoosterId, int> GetCaps() {
            var caps = new Dictionary<TBoosterId, int>();
            if (Boosters != null) {
                foreach (var b in Boosters) {
                    caps[b.BoosterId] = b.Cap > 0 ? b.Cap : int.MaxValue;
                }
            }
            return caps;
        }

        public Dictionary<TBoosterId, int> GetInitialCounts() {
            var counts = new Dictionary<TBoosterId, int>();
            if (Boosters != null) {
                foreach (var b in Boosters) {
                    counts[b.BoosterId] = b.InitialCount;
                }
            }
            return counts;
        }

        protected virtual void OnValidate() {
            if (ActiveScenario == BoosterScenarioType.ModalDialog || ActiveScenario == BoosterScenarioType.InlineCoinBadge) {
                if (Boosters != null) {
                    foreach (var booster in Boosters) {
                        if (booster.PriceInCoins <= 0) {
                            Debug.LogWarning($"[{name}] Scenario '{ActiveScenario}' requires PriceInCoins > 0, but '{booster.BoosterId}' has PriceInCoins = {booster.PriceInCoins}.");
                        }
                    }
                }
            }
        }
    }
}
