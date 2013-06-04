using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WaterTurret.Module.Models;

namespace WaterTurret.Module.Services
{
    public interface IConfigService
    {
        CleaningConfig CleaningConfig { get; set; }
        DeviceConfig DeviceConfig { get; set; }
        ImageProcessorConfig ImageProcessorConfig { get; set; }
        WateringConfig WateringConfig { get; set; }
        ViewConfig ViewConfig { get; set; }
    }
}
