using System;

namespace _Services._BoosterInventory {
    /// <summary>
    /// Immutable visual state passed to UI booster buttons.
    /// Completely decouples UI presentation from gameplay logic.
    /// </summary>
    public readonly struct BoosterVisualState<TBoosterId> where TBoosterId : Enum {
        public readonly TBoosterId BoosterId;
        public readonly int Count;
        public readonly int Cap;
        public readonly bool IsLocked;
        public readonly int RequiredLevel;
        public readonly string LockMessage;
        public readonly bool CanUseInGameplay;
        public readonly BoosterScenarioType Scenario;
        public readonly int PriceInCoins;
        public readonly bool HasCoinsToBuy;

        public bool HasCap => Cap > 0;

        public BoosterVisualState(
            TBoosterId boosterId,
            int count,
            int cap,
            bool isLocked,
            int requiredLevel,
            string lockMessage,
            bool canUseInGameplay,
            BoosterScenarioType scenario,
            int priceInCoins,
            bool hasCoinsToBuy
        ) {
            BoosterId = boosterId;
            Count = count;
            Cap = cap;
            IsLocked = isLocked;
            RequiredLevel = requiredLevel;
            LockMessage = lockMessage;
            CanUseInGameplay = canUseInGameplay;
            Scenario = scenario;
            PriceInCoins = priceInCoins;
            HasCoinsToBuy = hasCoinsToBuy;
        }
    }
}
