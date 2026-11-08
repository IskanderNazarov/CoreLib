using System;
using UnityEngine;

namespace _Services._BoosterInventory.Scenarios {
    /// <summary>
    /// Scenario 3 (Inline Coin Price Badge):
    /// When booster count == 0, button displays a coin price badge.
    /// Clicking with sufficient coins immediately consumes coins, adds boosters to inventory,
    /// and updates UI (does NOT auto-run booster).
    /// If coins are insufficient, redirects to Market.
    /// </summary>
    public class InlineCoinBadgeBoosterScenarioHandler<TBoosterId> : IBoosterScenarioHandler<TBoosterId> where TBoosterId : Enum {
        public BoosterScenarioType ScenarioType => BoosterScenarioType.InlineCoinBadge;

        public void HandleEmptyBoosterClick(BoosterClickContext<TBoosterId> context) {
            if (context.CoinsService == null || !context.CoinsService.IsAvailable) {
                Debug.LogWarning("[BoosterSystem] Developer Warning: Coins currency is not configured or available! Redirecting to Market.");
                context.RequestMarket();
                return;
            }

            int price = context.Info != null ? context.Info.PriceInCoins : 0;
            if (context.CoinsService.HasCoins(price)) {
                if (context.CoinsService.TryConsumeCoins(price, "booster_inline_buy")) {
                    int reward = context.Info != null && context.Info.RewardForCoins > 0 ? context.Info.RewardForCoins : 1;
                    context.Inventory?.TryAdd(context.BoosterId, reward, "coins_inline_buy");
                    context.NotifyPurchased(reward);
                    return;
                }
            }

            // Insufficient coins -> Open market
            context.RequestMarket();
        }
    }
}
