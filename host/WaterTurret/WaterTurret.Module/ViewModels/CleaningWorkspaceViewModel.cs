using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaterTurret.Common;
using WaterTurret.Module.Services;

namespace WaterTurret.Module.ViewModels
{
    public class CleaningWorkspaceViewModel : NavigationViewModel
    {
        public override string RegionName
        {
            get { return RegionNames.WorkspaceRegion; }
        }

        protected override void Selected()
        {
            _turretService.Mode = Models.TurretServiceMode.Cleaning;
        }

        protected override void UnSelected()
        {
        }

        private readonly ITurretService _turretService;
        public CleaningWorkspaceViewModel(ITurretService turretService)
        {
            _turretService = turretService;
        }
    }
}
