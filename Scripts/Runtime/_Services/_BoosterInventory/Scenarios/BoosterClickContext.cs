using System;
using _Services._Currency;

namespace _Services._BoosterInventory.Scenarios {
    /// <summary>
    /// Context passed to scenario handlers when an empty booster is clicked.
    /// Provides all needed services and callback triggers.
    /// </summary>
    public class BoosterClickContext<TBoosterId> where TBoosterId : Enum {
        public TBoosterId BoosterId { get; }
        public BoosterInfo<TBoosterId> Info { get; }
        public ICoinsService CoinsService { get; }
        public IBoosterInventory<TBoosterId> Inventory { get; }

        private readonly Action<TBoosterId> _requestMarket;
        private readonly Action<TBoosterId, BoosterInfo<TBoosterId>> _requestBuyDialog;
        private readonly Action<TBoosterId, int> _notifyPurchased;

        public BoosterClickContext(
            TBoosterId boosterId,
            BoosterInfo<TBoosterId> info,
            ICoinsService coinsService,
            IBoosterInventory<TBoosterId> inventory,
            Action<TBoosterId> requestMarket,
            Action<TBoosterId, BoosterInfo<TBoosterId>> requestBuyDialog,
            Action<TBoosterId, int> notifyPurchased
        ) {
            BoosterId = boosterId;
            Info = info;
            CoinsService = coinsService;
            Inventory = inventory;
            _requestMarket = requestMarket;
            _requestBuyDialog = requestBuyDialog;
            _notifyPurchased = notifyPurchased;
        }

        public void RequestMarket() {
            _requestMarket?.Invoke(BoosterId);
        }

        public void RequestBuyDialog() {
            _requestBuyDialog?.Invoke(BoosterId, Info);
        }

        public void NotifyPurchased(int amount) {
            _notifyPurchased?.Invoke(BoosterId, amount);
        }
    }
}
