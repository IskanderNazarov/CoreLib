using System.Collections.Generic;
using __CoreGameLib._Scripts._Services._Leaderboards;
using __CoreGameLib._Scripts._Services._Purchasing;
using __CoreGameLib._Scripts._Services._RemoteConfig;
using __CoreGameLib._Scripts._Services._Saving;
using _Data;
using _Infrastructure;
using _Infrastructure._Analytics;
using _Infrastructure.Services._Leaderboards;
using _Services._PlatformActions;
using _Services._Purchasing;
using _Services._Saving;
using Core._Purchasing;
using Core._Services;
using Core._Services._Saving;
using core.ads;
using core.purchasing;
using core.rating;
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
            if (_projectSettings.SDKType == SDK_Type.Playgama) {
                //_analyticsServices.Add(new PlaygamaAnalyticsService());
                InstallFor_Playgama();
            } else if (_projectSettings.SDKType == SDK_Type.GamePush) {
                _analyticsServices.Add(new GamePushAnalyticsService());
                InstallFor_GamePush();
            }
#endif


            Container.Bind<ProjectSettings>().FromScriptableObject(_projectSettings).AsSingle();
            Container.Bind<SoundManager>().FromNew().AsSingle().NonLazy();
            Container.Bind<RewardHandler>().FromNew().AsSingle().NonLazy();
            Container.Bind<IAnalyticsService>().To<CompositeAnalyticsService>().AsSingle().WithArguments(_analyticsServices.ToArray()).NonLazy();
        }

        private void InstallFor_Editor() {
            Container.Bind<IDataSaver>().To<DataSaver_Editor>().AsSingle();
            Container.BindInterfacesAndSelfTo<AdsService_Editor>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<Purchaser_Editor>().FromNew().AsSingle();

            Container.Bind<IRemoteConfig>().To<RemoteConfig_GP>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LeaderboardService_GP>().AsSingle().NonLazy();
            Container.Bind<IPlatformActionProvider>().To<PlatformActionProvider_GP>().AsSingle().NonLazy();
            Container.Bind<IRatingService>().To<RatingService_Editor>().AsSingle().NonLazy();
        }

        private void InstallFor_Playgama() {
            Container.Bind<IDataSaver>().To<DataSaver_PG>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<AdsService_PG>().FromNew().AsSingle();

            Container.BindInterfacesAndSelfTo<Purchaser_PG>().FromNew().AsSingle();
            Container.Bind<IRemoteConfig>().To<RemoteConfig_PG>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LeaderboardService_PG>().AsSingle().NonLazy();
            Container.Bind<IPlatformActionProvider>().To<PlatformActionProvider_PG>().AsSingle().NonLazy();
            Container.Bind<IRatingService>().To<RatingService_PG>().AsSingle().NonLazy();
        }

        private void InstallFor_GamePush() {
            Container.Bind<IDataSaver>().To<DataSaver_GP>().FromNew().AsSingle() /*.WithArguments(_projectSettings.PublicKeysFor_GP)*/.NonLazy();
            Container.BindInterfacesAndSelfTo<AdsService_GP>().FromNew().AsSingle().WithArguments(_projectSettings);

            Container.BindInterfacesAndSelfTo<Purchaser_GP>().FromNew().AsSingle();
            Container.Bind<IRemoteConfig>().To<RemoteConfig_GP>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LeaderboardService_GP>().AsSingle().NonLazy();
            Container.Bind<IPlatformActionProvider>().To<PlatformActionProvider_GP>().AsSingle().NonLazy();
            Container.Bind<IRatingService>().To<RatingService_GP>().AsSingle().NonLazy();
        }
    }
}
