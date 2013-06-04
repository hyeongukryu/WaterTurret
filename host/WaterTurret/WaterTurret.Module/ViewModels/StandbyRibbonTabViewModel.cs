using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

using WaterTurret.Module.Models;
using WaterTurret.Module.Services;
using WaterTurret.Common;
using AForge.Video.DirectShow;

namespace WaterTurret.Module.ViewModels
{
    public class StandbyRibbonTabViewModel : NavigationViewModel
    {
        public override string RegionName
        {
            get { return RegionNames.RibbonRegion; }
        }

        protected override void Selected()
        {
        }

        protected override void UnSelected()
        {
        }

        public DelegateCommand CameraSettingOpen { get; set; }

        private readonly ICameraService _cameraService;

        public StandbyRibbonTabViewModel(ICameraService cameraService)
        {
            _cameraService = cameraService;
            
            CameraSettingOpen = new DelegateCommand(() =>
                {
                    var device = _cameraService.Source as VideoCaptureDevice;
                    if (device != null)
                    {
                        device.DisplayPropertyPage(IntPtr.Zero);
                    }
                });
        }
    }
}
