using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Threading.Tasks;
using System.Timers;

namespace WaterTurret.Module.Controls
{
    public class TimeGauge : Control
    {        
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TimeGauge));
        
        public TimeGauge()
        {
            DefaultStyleKey = typeof(TimeGauge);

            Initialized += new EventHandler(TimeGauge_Initialized);
        }

        void TimeGauge_Initialized(object sender, EventArgs e)
        {
            Timer timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Interval = 10;
            timer.Start();
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() => Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.f")), null);
        }
    }
}
