using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Windows.Controls.Ribbon;
using WaterTurret.Module.ViewModels;

namespace WaterTurret.Module.Views
{
    /// <summary>
    /// ManualRibbonTabView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ManualRibbonTabView : RibbonTab
    {
        public ManualRibbonTabView(ManualRibbonTabViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
