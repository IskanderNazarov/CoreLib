using System;
using System.Collections;
using Core._Purchasing;

namespace _Services._Purchasing {
    public class Purchaser_Editor : IPurchaser {
        public event Action<string, bool> OnPurchaseCompletedEvent;

        public IEnumerator Initialize(bool isSupported) {
            yield return null;
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