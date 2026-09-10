using System;
using System.Collections;
using Cysharp.Threading.Tasks;

namespace _Services._Purchasing {
    public class Purchaser_Editor : IPurchaser {
        public event Action<string, bool> OnPurchaseCompletedEvent;

        public bool IsAvailable => _isSupported;

        private bool _isSupported;
        public UniTask Initialize(bool isSupported) {
            _isSupported = isSupported;
            return UniTask.CompletedTask;
        }

        public void BuyItem(string id) {
            OnPurchaseCompletedEvent?.Invoke(id, false);
        }

        public ProductInfo GetProdInfoByID(string id) {
            return new ProductInfo() {
                id = id
            };
        }

        public void ConsumePurchase(string id) {
        }
    }
}
