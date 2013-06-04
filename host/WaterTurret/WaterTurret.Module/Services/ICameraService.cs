using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;

namespace WaterTurret.Module.Services
{
    public interface ICameraService
    {
        IVideoSource Source { get; set; }
        Bitmap CurrentFrame { get; }
        void WaitNewFrame();
    }
}
