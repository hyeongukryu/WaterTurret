using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaterTurret.Module.Models
{
    public class ViewConfig
    {
        public bool UseKeyboard { get; set; }
        public bool UseJoystick { get; set; }

        public ViewConfigBackgroundImage BackgroundImage { get; set; }

        public bool MarkerOverlayBorder { get; set; }
        public bool MarkerOverlayDiagonal { get; set; }
        public bool MarkerOverlayGraphic { get; set; }

        public bool HudTimeNow { get; set; }
        public bool HudCameraAim { get; set; }
        public bool HudTiltNozzleAngle { get; set; }
        public bool HudTiltCameraAngle { get; set; }
        public bool HudPanAngle { get; set; }
        public bool HudPump { get; set; }
        public bool HudValve { get; set; }
    }

    public enum ViewConfigBackgroundImage
    {
        CameraRaw,
        Edge,
        Binary
    }
}
