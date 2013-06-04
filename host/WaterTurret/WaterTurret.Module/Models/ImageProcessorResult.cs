using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaterTurret.Module.Models
{
    public class ImageProcessorResult
    {
        public ImageProcessorResult()
        {
            Markers = new List<Marker>();
        }

        public List<Marker> Markers { get; private set; }
    }
}
