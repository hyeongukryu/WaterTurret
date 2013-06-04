using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using WaterTurret.Common;
using WaterTurret.Module.Services;
using WaterTurret.Module.Models;

namespace WaterTurret.Module.ViewModels
{
    public class InputConfigRibbonTabViewModel : NavigationViewModel
    {
        private readonly IConfigService _configService;
        public InputConfigRibbonTabViewModel(IConfigService configService)
        {
            _configService = configService;
        }

        public ViewConfig ViewConfig
        {
            get { return _configService.ViewConfig; }
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
