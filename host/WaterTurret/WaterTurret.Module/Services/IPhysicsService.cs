using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaterTurret.Module.Models;

namespace WaterTurret.Module.Services
{
    public interface IPhysicsService
    {
        double DegreeToRadian(double degree);
        double RadianToDegree(double radian);

        short NozzleAngleToWidth(double radian);
        short CameraAngleToWidth(double radian);
        short PanAngleToStep(double radian);

        double NozzleWidthToAngle(short width);
        double CameraWidthToAngle(short width);
        double PanStepToAngle(short step);
        


        PhysicsResult Measure(Marker marker, short tiltCamera, short pan, bool first);
        
        dynamic ParallaxAngle(double euclideanDistance);
        double WaterAngle(double horizontalDistance, double height, bool first);
    }
}
