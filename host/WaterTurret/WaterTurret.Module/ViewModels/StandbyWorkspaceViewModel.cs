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

namespace WaterTurret.Module.ViewModels
{
    public class StandbyWorkspaceViewModel : NavigationViewModel
    {
        public override string RegionName
        {
            get { return RegionNames.WorkspaceRegion; }
        }
        
        private readonly ITurretService _turretService;

        public StandbyWorkspaceViewModel(ITurretService turretService)
        {
            _turretService = turretService;
        }

        protected override void Selected()
        {
            _turretService.Mode = TurretServiceMode.Standby;
        }

        protected override void UnSelected()
        {
        }
    }
}
