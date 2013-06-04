using System;
using System.Windows;

using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

using Microsoft.Windows.Controls.Ribbon;
using WaterTurret.Shell.Views;
using WaterTurret.Shell.Utility;

namespace WaterTurret.Shell
{
    public partial class Bootstrapper : UnityBootstrapper
    {
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            // Create the module catalog from a XAML file.
            return Microsoft.Practices.Prism.Modularity.ModuleCatalog.CreateFromXaml(
                        new Uri("/WaterTurret.Shell;component/ModuleCatalog.xaml", UriKind.Relative));
        }

        
        /// <summary>
        /// Configures the default region adapter mappings to use in the application, in order 
        /// to adapt UI controls defined in XAML to use a region and register it automatically.
        /// </summary>
        /// <returns>The RegionAdapterMappings instance containing all the mappings.</returns>
        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            // Call base method
            var mappings = base.ConfigureRegionAdapterMappings();
            if (mappings == null) return null;

            // Add custom mappings
            var ribbonRegionAdapter = ServiceLocator.Current.GetInstance<RibbonRegionAdapter>();
            mappings.RegisterMapping(typeof(Ribbon), ribbonRegionAdapter);

            // Set return value
            return mappings;
        }
        

        protected override DependencyObject CreateShell()
        {
            // Use the container to create an instance of the shell.
            ShellView view = Container.TryResolve<ShellView>();

            // Display the shell's root visual.
            view.Show();

            return view;
        }
    }
}
