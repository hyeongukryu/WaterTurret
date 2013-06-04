using System;
using System.Linq;
using System.ComponentModel;
using System.Windows.Data;

using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

using WaterTurret.Module.Models;
using WaterTurret.Module.Services;

namespace WaterTurret.Module.ViewModels
{
    /// <summary>
    /// ViewModel for View1.
    /// </summary>
    public class ViewModel1 : NotificationObject, INavigationAware
    {
        private readonly IPlantService _PlantService;
        private readonly IEventAggregator _eventAggregator;
        private readonly Plants _model;

        public ViewModel1(IPlantService PlantService, IEventAggregator eventAggregator)
        {
            _PlantService = PlantService;
            _eventAggregator = eventAggregator;

            // Get the data model from the data service.
            _model = _PlantService.GetModel();

            // Initialize the CollectionView for the underlying model
            // and track the current selection.
            PlantsCV = new ListCollectionView(_model);
            PlantsCV.CurrentChanged += new EventHandler(SelectedItemChanged);

            // Initialize this ViewModel's commands.
            Command1 = new DelegateCommand<string>(ExecuteCommand1, CanExecuteCommand1);
        }

        public Plant CurrentItem { get; private set; }

        #region Plants CollectionView

        public ICollectionView PlantsCV { get; private set; }

        private void SelectedItemChanged(object sender, EventArgs e)
        {
            // Update the command status.
            Command1.RaiseCanExecuteChanged();
        }
        #endregion

        #region Command1
        public DelegateCommand<string> Command1 { get; private set; }

        private void ExecuteCommand1(string commandParameter)
        {
            Plant item = PlantsCV.CurrentItem as Plant;
            if (item != null)
            {
                // Update the current item.
                CurrentItem = PlantsCV.CurrentItem as Plant;
                base.RaisePropertyChanged<Plant>(() => CurrentItem);
            }
        }

        private bool CanExecuteCommand1(string commandParameter)
        {
            // Command is only enabled when an item is selected.
            return PlantsCV.CurrentItem != null;
        }

        #endregion

        #region INavigationAware Members
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            // Called to see if this view can handle the navigation request.
            // If it can, this view is activated.
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            // Called when this view is deactivated as a result of navigation to another view.
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            // Called to initialize this view during navigation.

            // Retrieve any required paramaters from the navigation Uri.
            string id = navigationContext.Parameters["ID"];
            if (!string.IsNullOrEmpty(id))
            {
                Plant item = _PlantService.GetModel().FirstOrDefault(Plant => Plant.Name == id);
            }
        }
        #endregion
    }
}
