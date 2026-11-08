using System;
using System.Collections.Generic;
using _Services._AdsService;
using _Services._Analytics;
using _Services._BoosterInventory.Scenarios;
using _Services._Currency;
using UnityEngine;
using Zenject;

namespace _Services._BoosterInventory {
    /// <summary>
    /// Central, game-agnostic coordinator for boosters and A/B scenario execution.
    /// Manages clicks on booster buttons, scenarios (Classic, Modal, Inline Coin Badge),
    /// currency spending, ad rewards, and visual states.
    /// </summary>
    public class BoosterService<TBoosterId> : IInitializable where TBoosterId : Enum {
        private readonly Func<TBoosterId, BoosterInfo<TBoosterId>> _boosterInfoProvider;
        private readonly Func<BoosterScenarioType> _scenarioProvider;
        private readonly IBoosterInventory<TBoosterId> _inventory;
        private readonly ICoinsService _coinsService;
        private readonly IAdsService _adsService;
        private readonly IAnalyticsService _analyticsService;
        private readonly IBoosterVisualService<TBoosterId> _visualService;

        private readonly Dictionary<BoosterScenarioType, IBoosterScenarioHandler<TBoosterId>> _scenarioHandlers 
            = new Dictionary<BoosterScenarioType, IBoosterScenarioHandler<TBoosterId>>();

        public BoosterScenarioType ActiveScenario => _scenarioProvider != null ? _scenarioProvider.Invoke() : BoosterScenarioType.Classic;
        public IBoosterVisualService<TBoosterId> VisualService => _visualService;

        // --- Callbacks for Game / UI Layer ---
        public event Action<TBoosterId> OnBoosterUseRequested;
        public event Action<TBoosterId> OnMarketRequested;
        public event Action<TBoosterId, BoosterInfo<TBoosterId>, Action, Action> OnBuyDialogRequested;
        public event Action<TBoosterId, int> OnBoosterPurchased;
        public event Action<TBoosterId, int> OnBoosterAdRewarded;

        public BoosterService(
            Func<TBoosterId, BoosterInfo<TBoosterId>> boosterInfoProvider,
            Func<BoosterScenarioType> scenarioProvider,
            IBoosterInventory<TBoosterId> inventory,
            [InjectOptional] ICoinsService coinsService = null,
            [InjectOptional] IAdsService adsService = null,
            [InjectOptional] IAnalyticsService analyticsService = null,
            [InjectOptional] IBoosterVisualService<TBoosterId> visualService = null
        ) {
            _boosterInfoProvider = boosterInfoProvider;
            _scenarioProvider = scenarioProvider;
            _inventory = inventory;
            _coinsService = coinsService;
            _adsService = adsService;
            _analyticsService = analyticsService;
            _visualService = visualService;

            RegisterDefaultScenarios();
        }

        public void Initialize() {
            ValidateCurrencyAvailability();
        }

        private void RegisterDefaultScenarios() {
            RegisterScenario(new ClassicBoosterScenarioHandler<TBoosterId>());
            RegisterScenario(new ModalDialogBoosterScenarioHandler<TBoosterId>());
            RegisterScenario(new InlineCoinBadgeBoosterScenarioHandler<TBoosterId>());
        }

        public void RegisterScenario(IBoosterScenarioHandler<TBoosterId> handler) {
            if (handler != null) {
                _scenarioHandlers[handler.ScenarioType] = handler;
            }
        }

        private void ValidateCurrencyAvailability() {
            if (ActiveScenario == BoosterScenarioType.ModalDialog || ActiveScenario == BoosterScenarioType.InlineCoinBadge) {
                if (_coinsService == null || !_coinsService.IsAvailable) {
                    Debug.LogWarning($"[BoosterService] Developer Warning: Active scenario '{ActiveScenario}' requires Coins currency, " +
                                     "but ICoinsService is not available in the project! Coin purchases will fallback to Market.");
                }
            }
        }

        /// <summary>
        /// Handles click on a booster button.
        /// If count > 0: fires OnBoosterUseRequested.
        /// If count == 0: executes active A/B scenario handler.
        /// </summary>
        public void HandleBoosterClick(TBoosterId id) {
            int count = _inventory != null ? _inventory.GetCount(id) : 0;

            if (count > 0) {
                OnBoosterUseRequested?.Invoke(id);
                return;
            }

            var info = _boosterInfoProvider != null ? _boosterInfoProvider.Invoke(id) : null;
            var handler = GetActiveScenarioHandler();

            var context = new BoosterClickContext<TBoosterId>(
                id,
                info,
                _coinsService,
                _inventory,
                requestMarket: boosterId => OnMarketRequested?.Invoke(boosterId),
                requestBuyDialog: (boosterId, bInfo) => {
                    OnBuyDialogRequested?.Invoke(
                        boosterId,
                        bInfo,
                        () => ExecuteCoinPurchase(boosterId, bInfo),
                        () => ExecuteAdReward(boosterId, bInfo)
                    );
                },
                notifyPurchased: (bId, amount) => OnBoosterPurchased?.Invoke(bId, amount)
            );

            handler.HandleEmptyBoosterClick(context);
        }

        public void ExecuteCoinPurchase(TBoosterId id, BoosterInfo<TBoosterId> info, Action onComplete = null) {
            int price = info != null ? info.PriceInCoins : 0;
            int reward = info != null && info.RewardForCoins > 0 ? info.RewardForCoins : 1;

            if (_coinsService != null && _coinsService.IsAvailable && _coinsService.HasCoins(price)) {
                if (_coinsService.TryConsumeCoins(price, "buy_booster_dialog")) {
                    _inventory?.TryAdd(id, reward, "buy_booster_coins");
                    OnBoosterPurchased?.Invoke(id, reward);
                    onComplete?.Invoke();
                    return;
                }
            }

            // Insufficient coins or missing service -> redirect to Market
            OnMarketRequested?.Invoke(id);
        }

        public void ExecuteAdReward(TBoosterId id, BoosterInfo<TBoosterId> info, Action onComplete = null) {
            int reward = info != null && info.RewardCount > 0 ? info.RewardCount : 1;

            if (_adsService != null) {
                _adsService.ShowRewarded(
                    onRewardGranted: () => {
                        _inventory?.TryAdd(id, reward, "rewarded_ad");
                        OnBoosterAdRewarded?.Invoke(id, reward);
                        onComplete?.Invoke();
                    },
                    onAdClosed: null
                );
            } else {
                Debug.LogWarning("[BoosterService] IAdsService not available. Granting test reward.");
                _inventory?.TryAdd(id, reward, "test_reward");
                OnBoosterAdRewarded?.Invoke(id, reward);
                onComplete?.Invoke();
            }
        }

        public BoosterVisualState<TBoosterId> GetVisualState(
            TBoosterId id,
            bool canUseInGameplay,
            int currentLevel,
            string lockMessage = null
        ) {
            var info = _boosterInfoProvider != null ? _boosterInfoProvider.Invoke(id) : null;
            int count = _inventory != null ? _inventory.GetCount(id) : 0;
            int cap = info != null ? info.Cap : 0;
            int requiredLevel = info != null ? info.RequiredLevelIndex : 0;
            bool isLocked = requiredLevel > 0 && currentLevel <= requiredLevel;
            int price = info != null ? info.PriceInCoins : 0;
            bool hasCoins = _coinsService != null && _coinsService.HasCoins(price);

            return new BoosterVisualState<TBoosterId>(
                id,
                count,
                cap,
                isLocked,
                requiredLevel,
                lockMessage,
                canUseInGameplay,
                ActiveScenario,
                price,
                hasCoins
            );
        }

        private IBoosterScenarioHandler<TBoosterId> GetActiveScenarioHandler() {
            if (_scenarioHandlers.TryGetValue(ActiveScenario, out var handler)) {
                return handler;
            }
            return _scenarioHandlers[BoosterScenarioType.Classic];
        }
    }
}
