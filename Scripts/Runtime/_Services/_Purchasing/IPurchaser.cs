// file: assets/_coregame/_scripts/purchasing/purchaser.cs
// assembly: corelib.asmdef

using System;
using Cysharp.Threading.Tasks;
#if PLAYGAMA
#endif

namespace _Services._Purchasing {
    public interface IPurchaser {
        public event Action<string, bool> OnPurchaseCompletedEvent;
        public bool IsAvailable { get; }

        UniTask Initialize(bool isSupported);
        void BuyItem(string id);
        ProductInfo GetProdInfoByID(string id);
        
        void ConsumePurchase(string id);
    }
}