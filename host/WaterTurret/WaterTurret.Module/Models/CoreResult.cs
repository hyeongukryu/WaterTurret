using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaterTurret.Module.Models
{
    public class CoreResult
    {
        public List<float> RotationVector { get; set; }
        public List<float> TranslationVector { get; set; }
        public double Distance { get; set; }
    }
}
