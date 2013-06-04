using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaterTurret.Module.Models;
using System.ComponentModel;

namespace WaterTurret.Module.Services
{
    public interface ITurretService : INotifyPropertyChanged
    {
        TurretServiceMode Mode { get; set; }

        bool Mono { get; set; }
        object ImageProcessorLock { get; set; }
        ImageProcessorResult ImageProcessorResult { get; }
        DateTime ImageProcessed { get; set; }

        void ImageProcess();
        DateTime ImageProcess(DateTime time);
    }
}
