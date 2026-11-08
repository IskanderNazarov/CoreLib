using System;

namespace _Services._BoosterInventory.Scenarios {
    /// <summary>
    /// Scenario 1 (Classic):
    /// When booster count == 0, clicking the button redirects directly to Market.
    /// </summary>
    public class ClassicBoosterScenarioHandler<TBoosterId> : IBoosterScenarioHandler<TBoosterId> where TBoosterId : Enum {
        public BoosterScenarioType ScenarioType => BoosterScenarioType.Classic;

        public void HandleEmptyBoosterClick(BoosterClickContext<TBoosterId> context) {
            context.RequestMarket();
        }
    }
}
