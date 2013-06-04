using System;
using System.Windows.Controls;

using WaterTurret.Module.ViewModels;

namespace WaterTurret.Module.Views
{
    public partial class View1 : UserControl
    {
        public View1(ViewModel1 viewModel)
        {
            InitializeComponent();

            // Set the ViewModel as this View's data context.
            this.DataContext = viewModel;
        }
    }
}
