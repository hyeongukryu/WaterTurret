using System;

using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

using WaterTurret.Module.Views;
using WaterTurret.Module.Services;
using WaterTurret.Common;
using WaterTurret.Module.ViewModels;

namespace WaterTurret.Module
{
    public class ModuleInit : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public ModuleInit(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        #region IModule Members

        public void Initialize()
        {
            // Register the PlantService concrete type with the container.
            // The PlantService's lifetime is controlled by the container so
            // a single instance is created and shared across the application.
            // Change this to swap in another data service implementation.
            _container.RegisterType<ITurretService, TurretService>(new ContainerControlledLifetimeManager());
            

            // ISerialService

            _container.RegisterType<ISerialService, SerialService>(new ContainerControlledLifetimeManager());
            // _container.RegisterType<ISerialService, SerialServiceNull>(new ContainerControlledLifetimeManager());

            _container.RegisterType<IPlantService, PlantService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IConfigService, ConfigService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IImageProcessorService, ImageProcessorService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ICameraService, CameraService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IDeviceService, DeviceService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IPhysicsService, PhysicsService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ICoreService, CoreService>(new ContainerControlledLifetimeManager());

            // Display the View in the Shell. Uses Prism's 'View
            // Discovery' mechanism to automatically display the view
            // in the specified named region.
            // _regionManager.RegisterViewWithRegion("TopLeftRegion", () => _container.Resolve<View1>());

            // 기본
            _regionManager.RegisterViewWithRegion(RegionNames.LeftRegion, () => _container.Resolve<SerialStatusView>());
            _regionManager.RegisterViewWithRegion(RegionNames.RibbonRegion, () => _container.Resolve<InputConfigRibbonTabView>());
            _regionManager.RegisterViewWithRegion(RegionNames.RibbonRegion, () => _container.Resolve<ViewConfigRibbonTabView>());
            
            // 대기 모드 진입
            _regionManager.RegisterViewWithRegion(RegionNames.RibbonRegion, () => _container.Resolve<StandbyRibbonTabView>());
            // _regionManager.RegisterViewWithRegion(RegionNames.WorkspaceRegion, () => _container.Resolve<StandbyWorkspaceView>());
            

            // Register the view types with the container so that it can create them
            // during navigation. Note that the container will attempt to create the
            // view as a named object so the views have to be registered as such.
            _container.RegisterType<object, StandbyWorkspaceView>("StandbyWorkspaceView");
            _container.RegisterType<object, StandbyRibbonTabView>("StandbyRibbonTabView");

            _container.RegisterType<object, WateringWorkspaceView>("WateringWorkspaceView");

            _container.RegisterType<object, CleaningWorkspaceView>("CleaningWorkspaceView");

            _container.RegisterType<object, ManualWorkspaceView>("ManualWorkspaceView");
            _container.RegisterType<object, ManualRibbonTabView>("ManualRibbonTabView");
            
        }

        #endregion
    }
}
