using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

using WaterTurret.Common;
using WaterTurret.Module.Models;
using WaterTurret.Module.Services;

namespace WaterTurret.Module.ViewModels
{
    public class ManualRibbonTabViewModel : NavigationViewModel
    {
        public DelegateCommand PumpOnCommand { get; private set; }
        public DelegateCommand PumpOffCommand { get; private set; }
        public DelegateCommand ValveOnCommand { get; private set; }
        public DelegateCommand ValveOffCommand { get; private set; }
        public DelegateCommand WaterOnCommand { get; private set; }
        public DelegateCommand WaterOffCommand { get; private set; }

        public DelegateCommand CameraUpCommand { get; set; }
        public DelegateCommand CameraDownCommand { get; set; }
        public DelegateCommand CameraCenterCommand { get; set; }

        public DelegateCommand NozzleUpCommand { get; set; }
        public DelegateCommand NozzleDownCommand { get; set; }
        public DelegateCommand NozzleCenterCommand { get; set; }

        public DelegateCommand PanLeftCommand { get; set; }
        public DelegateCommand PanRightCommand { get; set; }
        public DelegateCommand PanCenterCommand { get; set; }

        bool _autoControl = false;
        public bool AutoControl
        {
            get { return _autoControl; }
            set
            {
                _autoControl = value;
                RaisePropertyChanged(() => AutoControl);
                RaisePropertyChanged(() => ManualControl);
            }
        }
        public bool ManualControl
        {
            get { return !_autoControl; }
        }

        public override string RegionName
        {
            get { return RegionNames.RibbonRegion; }
        }

        private readonly ITurretService _turretService;
        private readonly IDeviceService _deviceService;
        private readonly IConfigService _configService;

        public ManualRibbonTabViewModel(ITurretService turretService, IDeviceService deviceService, IConfigService configService)
        {
            _turretService = turretService;
            _deviceService = deviceService;
            _configService = configService;

            PumpOnCommand = new DelegateCommand(() => _deviceService.PumpOn());
            PumpOffCommand = new DelegateCommand(() => _deviceService.PumpOff());
            ValveOnCommand = new DelegateCommand(() => _deviceService.ValveOn());
            ValveOffCommand = new DelegateCommand(() => _deviceService.ValveOff());
            WaterOnCommand = new DelegateCommand(() => _deviceService.WaterOn());
            WaterOffCommand = new DelegateCommand(() => _deviceService.WaterOff());

            CameraUpCommand = new DelegateCommand(() =>_deviceService.TiltCameraAdd(10));
            CameraDownCommand = new DelegateCommand(() => _deviceService.TiltCameraAdd(-10));
            CameraCenterCommand = new DelegateCommand(() => _deviceService.TiltCameraSet(_configService.DeviceConfig.TiltCameraDefault));

            NozzleUpCommand = new DelegateCommand(() => _deviceService.TiltNozzleAdd(-10));
            NozzleDownCommand = new DelegateCommand(() => _deviceService.TiltNozzleAdd(10));
            NozzleCenterCommand = new DelegateCommand(() => _deviceService.TiltNozzleSet(_configService.DeviceConfig.TiltNozzleDefault));

            PanLeftCommand = new DelegateCommand(() => _deviceService.PanAdd(-50, 0));
            PanRightCommand = new DelegateCommand(() => _deviceService.PanAdd(50, 0));
            PanCenterCommand = new DelegateCommand(() => _deviceService.PanSet(0, 0));
        }

        public ITurretService TurretService
        {
            get { return _turretService; }
        }

        public IDeviceService DeviceService
        {
            get { return _deviceService; }
        }

        protected override void Selected()
        {
        }

        protected override void UnSelected()
        {
        }
    }
}
