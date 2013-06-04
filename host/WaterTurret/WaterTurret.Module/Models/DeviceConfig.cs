using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace WaterTurret.Module.Models
{
    public class DeviceConfig : NotificationObject
    {
        public TimeSpan WaterOnDelay { get; set; }
        public TimeSpan WaterOffDelay { get; set; }

        public short TiltNozzleMin { get; set; }
        public short TiltNozzleMax { get; set; }
        public short TiltNozzleDefault { get; set; }

        public short TiltCameraMin { get; set; }
        public short TiltCameraMax { get; set; }
        public short TiltCameraDefault { get; set; }

        public short PanMin { get; set; }
        public short PanMax { get; set; }

        public string SerialPortName { get; set; }
        public int SerialBaudRate { get; set; }

        public int ReadTimeOut { get; set; }
        public int WriteTimeOut { get; set; }

        public int PixelWidth { get; set; }
        public int PixelHeight { get; set; }
    }
}
