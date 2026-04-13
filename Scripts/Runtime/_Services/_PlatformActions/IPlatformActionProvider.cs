//using _Services._ScriptableObjects;

using _Services._ScriptableObjects;

namespace _Services._PlatformActions {
    public interface IPlatformActionProvider {
        SupportedPlatform GetCurrentPlatform();
        string GetISO();
        void CallGameReadyAPI();
        void SetMinimalDelayForInterstitial();
        bool IsRemoteConfigSupported();
    }
}