using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using WaterTurret.Module.Models;

namespace WaterTurret.Module.Controls
{
    public class ModeGauge : Control
    {
        static ModeGauge()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModeGauge), new FrameworkPropertyMetadata(typeof(ModeGauge)));
        }
                
        public string ModeDescription
        {
            get { return (string)GetValue(ModeDescriptionProperty); }
            set { SetValue(ModeDescriptionProperty, value); }
        }
        
        // Using a DependencyProperty as the backing store for Mode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModeDescriptionProperty =
            DependencyProperty.Register("ModeDescription", typeof(string), typeof(ModeGauge), new UIPropertyMetadata("대기 모드"));
    }
}
