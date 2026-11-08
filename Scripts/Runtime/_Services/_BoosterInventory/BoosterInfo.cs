using System;
using _Services._Localization;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Services._BoosterInventory {
    [Serializable]
    public class BoosterInfo<TBoosterId> where TBoosterId : Enum {
        public TBoosterId BoosterId;
        [FormerlySerializedAs("Name")] [TranslationKey] public string NameKey;
        [FormerlySerializedAs("Description")] [TranslationKey] public string DescriptionKey;
        public Sprite Icon;

        [Tooltip("Starting amount of this booster for new players")]
        public int InitialCount = 0;

        [Tooltip("Maximum amount the player can hold. If <= 0, limit is infinite and cap is not shown on UI. If > 0, shown as {count}/{cap}")]
        public int Cap = 0;

        [Tooltip("Price of this booster in coins")]
        public int PriceInCoins;

        [Tooltip("How many boosters the player receives when buying for coins")]
        public int RewardForCoins = 1;

        [Tooltip("How many boosters the player receives for watching a rewarded ad")]
        public int RewardCount = 1;

        [FormerlySerializedAs("RequiredLevel")]
        [Tooltip("Level after which this booster unlocks (0-indexed)")]
        public int RequiredLevelIndex;

        public int AdRewardCount => RewardCount > 0 ? RewardCount : (RewardForCoins > 0 ? RewardForCoins : 1);
        public bool HasCap => Cap > 0;
    }
}
