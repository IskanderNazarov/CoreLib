using System;
using _Data;
using _Services;
using _Services._AdsService;
using _Services._Localization;
using _Services._Purchasing;
using _Services._RemoteConfig;
using _Services._Saving;
using Cysharp.Threading.Tasks;
#if GAMEPUSH
using GamePush;
#endif
using Zenject;

public class NazCore {
    [Inject] private IAdsService _adsService;
    [Inject] private IPurchaser _purchaser;
    [Inject] private IRemoteConfig _remoteConfig;
    [Inject] private IDataSaver _dataSaver;
    [Inject] private Localizer _localizer;
    [Inject] private ProjectSettings _projectSettings;

    /// <summary>
    /// Поочередная инициализация всех сервисов ядра с поддержкой SaveManager.
    /// </summary>
    public async UniTask Initialize(
        IAsyncInitializable saveManager,
        IKeysStorage rcKeysStorage = null,
        bool iapSupported = false,
        Action<string, float> onServiceProgress = null) {
        onServiceProgress?.Invoke("SDK_Initialize started", 0f);
        // 1. Инициализация SDK (GamePush / Playgama)
        await InitSDK();
        onServiceProgress?.Invoke("SDK_Initialized", 0.2f);

        // 2. Инициализация AdsService
        _adsService.Initialize();
        onServiceProgress?.Invoke("AdsService_Initialized", 0.2f);

        // 3. Инициализация RemoteConfig
        if (rcKeysStorage != null) {
            await _remoteConfig.LoadConfigs(rcKeysStorage);
        }

        onServiceProgress?.Invoke("RemoteConfig_Initialized", 0.4f);

        // 4. Инициализация Purchaser
        await _purchaser.Initialize(iapSupported);
        onServiceProgress?.Invoke("Purchaser_Initialized", 0.6f);

        // 5. Инициализация Localizer
        _localizer.Initialize();
        onServiceProgress?.Invoke("Localizer_Initialized", 0.85f);

        // 6. Инициализация SaveManager (загрузка данных через IDataSaver)
        if (saveManager != null) {
            await saveManager.Initialize();
        }

        onServiceProgress?.Invoke("DataSaver_Initialized", 1);
    }

    /// <summary>
    /// Поочередная инициализация сервисов ядра с прямой загрузкой строки данных.
    /// </summary>
    public async UniTask Initialize(
        IKeysStorage rcKeysStorage = null,
        string saveKey = null,
        Action<string, string> onDataLoaded = null,
        bool iapSupported = false,
        Action<string> onServiceProgress = null) {
        // 1. Инициализация SDK (GamePush / Playgama)
        await InitSDK();
        onServiceProgress?.Invoke("SDK_Initialized");

        // 2. Инициализация AdsService
        _adsService.Initialize();
        onServiceProgress?.Invoke("AdsService_Initialized");

        // 3. Инициализация RemoteConfig
        if (rcKeysStorage != null) {
            await _remoteConfig.LoadConfigs(rcKeysStorage);
        }

        onServiceProgress?.Invoke("RemoteConfig_Initialized");

        // 4. Инициализация Purchaser
        await _purchaser.Initialize(iapSupported);
        onServiceProgress?.Invoke("Purchaser_Initialized");

        // 5. Инициализация Localizer
        _localizer.Initialize();
        onServiceProgress?.Invoke("Localizer_Initialized");

        // 6. Инициализация DataSaver
        if (!string.IsNullOrEmpty(saveKey)) {
            var loadedData = await _dataSaver.Load(saveKey);
            onDataLoaded?.Invoke(saveKey, loadedData);
        }

        onServiceProgress?.Invoke("DataSaver_Initialized");
    }

    private async UniTask InitSDK() {
#if GAMEPUSH
        if (_projectSettings.SDKType == SDK_Type.GamePush) {
            await UniTask.WaitUntil(() => GP_Init.isReady);
        }
#endif
        await UniTask.Yield(); // Just to avoid async warnings if empty
    }
}