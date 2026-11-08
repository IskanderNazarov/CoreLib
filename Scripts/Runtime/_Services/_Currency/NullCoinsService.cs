using UnityEngine;

namespace _Services._Currency {
    /// <summary>
    /// Fallback implementation when coin currency is not configured.
    /// Emits developer warnings whenever coin operations are attempted.
    /// </summary>
    public class NullCoinsService : ICoinsService {
        public bool IsAvailable => false;
        public int Coins => 0;

        public bool HasCoins(int amount) {
            WarnMissingCurrency();
            return false;
        }

        public void AddCoins(int amount, string source = "") {
            WarnMissingCurrency();
        }

        public bool TryConsumeCoins(int amount, string source = "") {
            WarnMissingCurrency();
            return false;
        }

        private static void WarnMissingCurrency() {
            Debug.LogWarning("[CoinsService] Developer Warning: Coins currency is not configured or available in this project!");
        }
    }
}
