using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaterTurret.Common;
using WaterTurret.Module.Services;

namespace WaterTurret.Module.ViewModels
{
    public class WateringWorkspaceViewModel : NavigationViewModel
    {
        private readonly ITurretService _turretService;
        public WateringWorkspaceViewModel(ITurretService turretService)
        {
            _turretService = turretService;
        }

        public override string RegionName
        {
            get { return RegionNames.WorkspaceRegion; }
        }
        
        protected override void Selected()
        {
            _turretService.Mode = Models.TurretServiceMode.Watering;
        }

        protected override void UnSelected()
        {
        }
    }
}
