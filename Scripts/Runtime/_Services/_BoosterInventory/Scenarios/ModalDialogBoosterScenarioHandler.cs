using System;

namespace _Services._BoosterInventory.Scenarios {
    /// <summary>
    /// Scenario 2 (Modal Dialog):
    /// When booster count == 0, clicking the button opens BuyBoosterDialog.
    /// </summary>
    public class ModalDialogBoosterScenarioHandler<TBoosterId> : IBoosterScenarioHandler<TBoosterId> where TBoosterId : Enum {
        public BoosterScenarioType ScenarioType => BoosterScenarioType.ModalDialog;

        public void HandleEmptyBoosterClick(BoosterClickContext<TBoosterId> context) {
            context.RequestBuyDialog();
        }
    }
}
