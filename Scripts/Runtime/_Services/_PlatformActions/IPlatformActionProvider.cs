//using _Services._ScriptableObjects;

using _Data;

namespace _Services._PlatformActions {
    public interface IPlatformActionProvider {
        void Initialize();
        SupportedPlatform GetCurrentPlatform();
        string GetISO();
        void CallGameReadyAPI();
        void CallGameplayStart();
        void CallGameplayEnd();
        void SetMinimalDelayForInterstitial();
        bool IsRemoteConfigSupported();
        PlatformConfig PlatformConfig { get; }

    }
}