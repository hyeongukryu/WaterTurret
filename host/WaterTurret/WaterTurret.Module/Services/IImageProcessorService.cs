using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using WaterTurret.Module.Models;

namespace WaterTurret.Module.Services
{
    public interface IImageProcessorService : INotifyPropertyChanged
    {
        Bitmap BackgroundBitmap { get; }
        ImageProcessorResult Process(Bitmap bitmap, bool rgb);
    }
}
