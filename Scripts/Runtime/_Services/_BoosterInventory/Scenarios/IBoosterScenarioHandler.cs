using System;

namespace _Services._BoosterInventory.Scenarios {
    /// <summary>
    /// Strategy interface for booster usage scenarios.
    /// New scenarios can be added without modifying game logic or other scenarios.
    /// </summary>
    public interface IBoosterScenarioHandler<TBoosterId> where TBoosterId : Enum {
        BoosterScenarioType ScenarioType { get; }

        /// <summary>
        /// Handles click when booster count == 0.
        /// </summary>
        void HandleEmptyBoosterClick(BoosterClickContext<TBoosterId> context);
    }
}
