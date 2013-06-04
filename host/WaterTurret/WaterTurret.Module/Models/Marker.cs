using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaterTurret.Module.Models
{
    public class Marker
    {
        /// <summary>
        /// 화상 중심을 원점으로 하는 X 좌표입니다.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// 화상 중심을 원점으로 하는 Y 좌표입니다.
        /// </summary>
        public int Y { get; set; }

        public List<AForge.IntPoint> Points { get; set; }
        public System.Drawing.Bitmap Image { get; set; }
        public int FrameworkArea { get; set; }

        public MarkerColor Color { get; set; }
        public MarkerRotate Rotate { get; set; }

        public double Height { get; set; }
        public double EuclideanDistance { get; set; }
        public double HorizontalDistance  { get; set; }

        public double PanAngle { get; set; }
        public double TiltAngle { get; set; }

        public double TransX { get; set; }
        public double TransY { get; set; }
        public double TransZ { get; set; }
    }

    public enum MarkerColor
    {
        Red,
        Green,
        Blue,
        Black,
        White
    }

    public enum MarkerRotate
    {
        None = 0,
        Quad1 = 1,
        Quad2 = 2,
        Quad3 = 3,
        Quad4 = 4
    }
}
