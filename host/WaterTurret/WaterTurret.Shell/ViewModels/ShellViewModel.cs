using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using WaterTurret.Common;

namespace WaterTurret.Shell.ViewModels
{
    public class ShellViewModel : NotificationObject
    {
        public DelegateCommand StandbyCommand { get; private set; }
        public DelegateCommand WateringCommand { get; private set; }
        public DelegateCommand CleaningCommand { get; private set; }
        public DelegateCommand ManualCommand { get; private set; }

        private readonly IRegionManager _regionManager;

        public ShellViewModel(IRegionManager regionManager)
        {
            StandbyCommand = new DelegateCommand(Standby);
            WateringCommand = new DelegateCommand(Watering);
            CleaningCommand = new DelegateCommand(Cleaning);
            ManualCommand = new DelegateCommand(Manual);

            _regionManager = regionManager;

            Standby();
        }

        private void Standby()
        {
            _regionManager.RequestNavigate(RegionNames.RibbonRegion,
                new Uri("StandbyRibbonTabView", UriKind.Relative));

            _regionManager.RequestNavigate(RegionNames.WorkspaceRegion,
                new Uri("StandbyWorkspaceView", UriKind.Relative));
        }

        private void Watering()
        {
            _regionManager.RequestNavigate(RegionNames.RibbonRegion,
                new Uri("WateringRibbonTabView", UriKind.Relative));

            _regionManager.RequestNavigate(RegionNames.WorkspaceRegion,
                new Uri("WateringWorkspaceView", UriKind.Relative));
        }

        private void Cleaning()
        {
            _regionManager.RequestNavigate(RegionNames.RibbonRegion,
                new Uri("CleaningRibbonTabView", UriKind.Relative));

            _regionManager.RequestNavigate(RegionNames.WorkspaceRegion,
                new Uri("CleaningWorkspaceView", UriKind.Relative));
        }

        private void Manual()
        {
            _regionManager.RequestNavigate(RegionNames.RibbonRegion,
                new Uri("ManualRibbonTabView", UriKind.Relative));

            _regionManager.RequestNavigate(RegionNames.WorkspaceRegion,
                new Uri("ManualWorkspaceView", UriKind.Relative));
        }
    }
}
