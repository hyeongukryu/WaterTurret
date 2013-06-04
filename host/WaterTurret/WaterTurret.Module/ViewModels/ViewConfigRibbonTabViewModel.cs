using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using WaterTurret.Common;
using WaterTurret.Module.Services;
using Microsoft.Practices.Prism.Commands;

using WaterTurret.Module.Models;

namespace WaterTurret.Module.ViewModels
{
    public class ViewConfigRibbonTabViewModel : NavigationViewModel
    {
        public DelegateCommand BackgroundImageCameraRawCommand { get; private set; }
        public DelegateCommand BackgroundImageEdgeCommand { get; private set; }
        public DelegateCommand BackgroundImageBinaryCommand { get; private set; }

        private readonly IConfigService _configService;

        public ViewConfig ViewConfig
        {
            get { return _configService.ViewConfig; }
        }

        public ViewConfigRibbonTabViewModel(IConfigService configService)
        {
            _configService = configService;
            

            BackgroundImageCameraRawCommand = new DelegateCommand(() =>
                ViewConfig.BackgroundImage = ViewConfigBackgroundImage.CameraRaw);

            BackgroundImageEdgeCommand = new DelegateCommand(() =>
                ViewConfig.BackgroundImage = ViewConfigBackgroundImage.Edge);

            BackgroundImageBinaryCommand = new DelegateCommand(() =>
                ViewConfig.BackgroundImage = ViewConfigBackgroundImage.Binary);
            
            BackgroundImageCameraRawCommand.Execute();
        }

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
    }
}
