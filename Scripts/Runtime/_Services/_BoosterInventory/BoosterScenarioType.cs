namespace _Services._BoosterInventory {
    /// <summary>
    /// Available booster usage scenarios for A/B testing and modular integration.
    /// </summary>
    public enum BoosterScenarioType {
        /// <summary>
        /// Scenario 1 (Classic): Booster displays count. When count == 0, shows [+] plus icon.
        /// Clicking with 0 opens the Market directly.
        /// </summary>
        Classic = 0,

        /// <summary>
        /// Scenario 2 (Modal Dialog): Booster displays count.
        /// When count == 0, shows [+] plus icon. Clicking opens BuyBoosterDialog (Buy for coins vs Rewarded ad).
        /// If coins are insufficient, redirects immediately to Market.
        /// </summary>
        ModalDialog = 1,

        /// <summary>
        /// Scenario 3 (Inline Coin Price Badge): Booster displays count.
        /// When count == 0, button displays a coin price badge directly on the button.
        /// Tapping with sufficient coins purchases the booster immediately in 1 click (no extra modal).
        /// If coins are insufficient, redirects to Market.
        /// </summary>
        InlineCoinBadge = 2
    }
}
