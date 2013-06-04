using System;
using System.Windows;
using System.Windows.Controls;

namespace WaterTurret.Module.Controls
{
    /// <summary>
    /// A simple control that provides a border around a region
    /// in order to display the region's name in a header.
    /// </summary>
    public sealed class RegionBorderControl : ContentControl
    {
        #region Dependency Property - RegionName

        public static readonly DependencyProperty RegionNameProperty =
            DependencyProperty.Register("RegionName", typeof(String), typeof(RegionBorderControl),
                                         new PropertyMetadata(string.Empty, OnRegionNameChanged));

        public string RegionName
        {
            get { return (string)this.GetValue(RegionBorderControl.RegionNameProperty); }
            set { this.SetValue(RegionBorderControl.RegionNameProperty, value); }
        }

        private static void OnRegionNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RegionBorderControl target = o as RegionBorderControl;
        }

        #endregion

        public RegionBorderControl()
        {
            // Set default style key for generic template to be applied.
            this.DefaultStyleKey = typeof(RegionBorderControl);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }
}
