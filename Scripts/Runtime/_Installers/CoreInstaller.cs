using System.Collections.Generic;
using _Data;
using _Infrastructure;
using _Services._Achievements;
using _Services._AdsService;
using _Services._Analytics;
using _Services._Leaderboards;
using _Services._Localization;
using _Services._PlatformActions;
using _Services._Purchasing;
using _Services._RemoteConfig;
using _Services._RewardsHandling;
using _Services._Saving;
using _Services._Social;
using _Services._SoundManagement;
using UnityEngine;
using Zenject;

namespace __CoreGameLib._Scripts._Installers {
    public class CoreServicesInstaller : MonoInstaller {
        [SerializeField] private ProjectSettings _projectSettings;
        private List<IAnalyticsService> _analyticsServices;

        public override void InstallBindings() {
            
            _analyticsServices = new List<IAnalyticsService>();
#if UNITY_EDITOR
            _analyticsServices.Add(new ConsoleAnalyticsService());
            InstallFor_Editor();
#else
#if PLAYGAMA
            if (_projectSettings.SDKType == SDK_Type.Playgama) {
                //_analyticsServices.Add(new PlaygamaAnalyticsService());
                InstallFor_Playgama();
            }
#endif
#if GAMEPUSH
            if (_projectSettings.SDKType == SDK_Type.GamePush) {
                _analyticsServices.Add(new GamePushAnalyticsService());
                InstallFor_GamePush();
            }
#endif
#endif


            Container.Bind<NazCore>().FromNew().AsSingle().NonLazy();
            Container.Bind<Localizer>().FromNew().AsSingle().NonLazy();
            Container.Bind<ProjectSettings>().FromScriptableObject(_projectSettings).AsSingle();
            Container.Bind<SoundManager>().FromNew().AsSingle().NonLazy();
            Container.Bind<RewardHandler>().FromNew().AsSingle().NonLazy();
            Container.Bind<IAnalyticsService>().To<CompositeAnalyticsService>().AsSingle().WithArguments(_analyticsServices.ToArray()).NonLazy();
        }

        private void InstallFor_Editor() {
            Container.Bind<IDataSaver>().To<DataSaver_Editor>().AsSingle();
            Container.BindInterfacesAndSelfTo<AdsService_Editor>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<Purchaser_Editor>().FromNew().AsSingle();

#if GAMEPUSH
            Container.Bind<IRemoteConfig>().To<RemoteConfig_GP>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LeaderboardService_GP>().AsSingle().NonLazy();
            Container.Bind<IPlatformActionProvider>().To<PlatformActionProvider_GP>().AsSingle().NonLazy();
#elif PLAYGAMA
            Container.Bind<IRemoteConfig>().To<RemoteConfig_PG>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LeaderboardService_PG>().AsSingle().NonLazy();
            Container.Bind<IPlatformActionProvider>().To<PlatformActionProvider_PG>().AsSingle().NonLazy();
#endif
            Container.Bind<IRatingService>().To<RatingService_Editor>().AsSingle().NonLazy();

            Container.Bind<IAchievementsService>().To<AchievementsService_Editor>().AsSingle().NonLazy();
        }

#if PLAYGAMA
        private void InstallFor_Playgama() {
            Container.Bind<IDataSaver>().To<DataSaver_PG>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<AdsService_PG>().FromNew().AsSingle();

            Container.BindInterfacesAndSelfTo<Purchaser_PG>().FromNew().AsSingle();
            Container.Bind<IRemoteConfig>().To<RemoteConfig_PG>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LeaderboardService_PG>().AsSingle().NonLazy();
            Container.Bind<IPlatformActionProvider>().To<PlatformActionProvider_PG>().AsSingle().NonLazy();
            Container.Bind<IRatingService>().To<RatingService_PG>().AsSingle().NonLazy();

            Container.Bind<IAchievementsService>().To<AchievementsService_PG>().AsSingle().NonLazy();
        }
#endif

#if GAMEPUSH
        private void InstallFor_GamePush() {
            Container.Bind<IDataSaver>().To<DataSaver_GP>().FromNew().AsSingle() /*.WithArguments(_projectSettings.PublicKeysFor_GP)*/.NonLazy();
            Container.BindInterfacesAndSelfTo<AdsService_GP>().FromNew().AsSingle().WithArguments(_projectSettings);

            Container.BindInterfacesAndSelfTo<Purchaser_GP>().FromNew().AsSingle();
            Container.Bind<IRemoteConfig>().To<RemoteConfig_GP>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LeaderboardService_GP>().AsSingle().NonLazy();
            Container.Bind<IPlatformActionProvider>().To<PlatformActionProvider_GP>().AsSingle().NonLazy();
            Container.Bind<IRatingService>().To<RatingService_GP>().AsSingle().NonLazy();

            Container.Bind<IAchievementsService>().To<AchievementsService_GP>().AsSingle().NonLazy();
        }
#endif
    }
}