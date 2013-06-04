using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaterTurret.Module.Models;
using AForge;

namespace WaterTurret.Module.Services
{
    public class PhysicsService : IPhysicsService
    {
        public short NozzleAngleToWidth(double radian)
        {
            var degree = RadianToDegree(radian);
            return (short)(1710.0 + degree * (-830.0) / 90.0);
        }

        public double CameraWidthToAngle(short width)
        {
            var degree = (width - 1500) / 10.0;
            return DegreeToRadian(degree);
        }

        public double PanStepToAngle(short step)
        {
            return (step / 3200.0) * Math.PI;
        }

        public short PanAngleToStep(double radian)
        {
            return (short)((radian / Math.PI) * 3200.0);
        }

        public short CameraAngleToWidth(double radian)
        {
            throw new NotImplementedException();
        }

        public double NozzleWidthToAngle(short width)
        {
            throw new NotImplementedException();
        }

        public double WaterAngle(double horizontalDistance, double height, bool first)
        {
            const double speed = 3.65;
            double low = double.MaxValue, result = 45;

            const int end = 10000;

            for (int i = 0; i < end; i++)
            {
                double angle = Math.PI / 2 / end * i;

                if (first == false)
                {
                    angle = Math.PI / 2 - angle;
                }
                
                // mks 단위 변환
                double oy = height * 0.01;
                double x = horizontalDistance * 0.01 - Math.Cos(angle) * 5.5 /* 노즐 */;

                double y = Math.Tan(angle) * x - (9.8 * x * x) / (2 * speed * speed * Math.Cos(angle) * Math.Cos(angle)) - (Math.Sin(angle) * 5.5) /* 노즐 */;
                double d = Math.Abs(oy - y);

                if (low > d)
                {
                    low = d;
                    result = angle;

                    if (low < 0.1)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        public dynamic ParallaxAngle(double horizontalDistance)
        {
            const double CameraAxis = 2.8, AxisNozzle = 8;
            // const double CameraNozzle = 10;

            const double CircleX = CameraAxis, CircleR = AxisNozzle;

            var PointY = horizontalDistance;

            double m, x;

            // 접선 방정식
            {
                var a = CircleX * CircleX - CircleR * CircleR;
                var b = CircleX * PointY;
                var c = PointY * PointY + -CircleR * CircleR;

                m = (-b + Math.Sqrt(b * b - a * c)) / a;
            }

            {
                // 접점
                var a = 1 + m * m;
                var b = m * PointY - CircleX;
                var c = CircleX * CircleX + PointY * PointY - CircleR * CircleR;

                // b^2 - ac = 0
                x = (-b /*- Math.Sqrt(b * b - a * c)*/) / a;
            }

            double y = Math.Sqrt(CircleR * CircleR - Math.Pow(x - CircleX, 2));
            
            var dx = x;
            var dy = PointY - y;
            double d = Math.Sqrt(dx * dx + dy * dy);

            double at = -Math.Atan(m);
            double angle = Math.PI / 2 - at;

            return new
            {
                Angle = angle,
                Distance = d
            };
        }

        public PhysicsResult Measure(Marker marker, short cameraWidth, short panStep, bool first)
        {
            var result = new PhysicsResult();

            double cameraAngle = CameraWidthToAngle(cameraWidth);
            
            marker.HorizontalDistance = Math.Cos(cameraAngle) * marker.EuclideanDistance;
            marker.Height = Math.Sin(cameraAngle) * marker.EuclideanDistance + 10;

            var pRes = ParallaxAngle(marker.HorizontalDistance);
            var theta = pRes.Angle;
            marker.HorizontalDistance = pRes.Distance;


            int k = 0;
            while (k-- > 0)
            {
                Console.Clear();
                Console.WriteLine(RadianToDegree(theta));
                Console.WriteLine(marker.HorizontalDistance);
                Console.WriteLine(marker.Height);
                System.Threading.Thread.Sleep(10);
            }

            // 카메라가 노즐보다 앞에 있음
            marker.HorizontalDistance += 9;

            result.EstimatedPanStep = (short)(panStep - PanAngleToStep(theta));
            result.EstimatedNozzleWidth = NozzleAngleToWidth(WaterAngle(marker.HorizontalDistance, marker.Height, first));
            
            return result;
        }

        public double DegreeToRadian(double degree)
        {
            return degree * Math.PI / 180;
        }

        public double RadianToDegree(double radian)
        {
            return radian / Math.PI * 180;
        }
    }
}
