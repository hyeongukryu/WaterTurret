using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaterTurret.Module.Models
{
    public class ImageProcessorConfig
    {
        public int BlobMinWidth { get; set; }
        public int BlobMinHeight { get; set; }

        public double AngleError1 { get; set; }
        public double AngleError2 { get; set; }
        public double LengthError { get; set; }

        public int BorderTestSize { get; set; }
        public double BorderTestDifference { get; set; }

        public int QuadrilateralTransformationWidth { get; set; }
        public int QuadrilateralTransformationHeight { get; set; }

        public double ColorTestWhite { get; set; }

        public int ChannelFilterBase { get; set; }

        public TimeSpan TrackingGiveupTime { get; set; }

        public int Threshold { get; set; }

        public double MarkerSize { get; set; }
    }
}
