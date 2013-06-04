using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Windows.Controls.Ribbon;

using System.Windows;
using System.Collections.Specialized;

namespace WaterTurret.Shell.Utility
{
    /// <summary>
    /// Enables use of a Ribbon control as a Prism region.
    /// </summary>
    /// <remarks> See Developer's Guide to Microsoft Prism (Ver. 4), p. 189.</remarks>
    public class RibbonRegionAdapter : RegionAdapterBase<Ribbon>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="behaviorFactory">Allows the registration of the default set of RegionBehaviors.</param>
        public RibbonRegionAdapter(IRegionBehaviorFactory behaviorFactory)
            : base(behaviorFactory)
        {
        }

        /// <summary>
        /// Adapts a WPF control to serve as a Prism IRegion. 
        /// </summary>
        /// <param name="region">The new region being used.</param>
        /// <param name="regionTarget">The WPF control to adapt.</param>
        protected override void Adapt(IRegion region, Ribbon regionTarget)
        {
            region.Views.CollectionChanged += (sender, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (FrameworkElement element in e.NewItems)
                        {
                            regionTarget.Items.Add(element);
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        foreach (UIElement elementLoopVariable in e.OldItems)
                        {
                            var element = elementLoopVariable;
                            if (regionTarget.Items.Contains(element))
                            {
                                regionTarget.Items.Remove(element);
                            }
                        }
                        break;
                }
            };
        }

        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }
    }
}
