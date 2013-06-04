using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WaterTurret.Module.Models;

namespace WaterTurret.Module.Services
{
    public class ConfigService : IConfigService
    {
        private CleaningConfig _cleaningConfig;
        public CleaningConfig CleaningConfig
        {
            get
            {
                if (_cleaningConfig == null)
                {
                    _cleaningConfig = new CleaningConfig();
                }

                return _cleaningConfig;
            }
            set
            {
                _cleaningConfig = value;
            }
        }

        private DeviceConfig _deviceConfig;
        public DeviceConfig DeviceConfig
        {
            get
            {
                if (_deviceConfig == null)
                {
                    _deviceConfig = new DeviceConfig();

                    _deviceConfig.SerialPortName = "COM3";
                    _deviceConfig.SerialBaudRate = 19200;
                    _deviceConfig.ReadTimeOut = 10000;
                    _deviceConfig.WriteTimeOut = 10000;

                    _deviceConfig.TiltNozzleMin = 600;
                    _deviceConfig.TiltNozzleMax = 2300;
                    _deviceConfig.TiltCameraMin = 1200;
                    _deviceConfig.TiltCameraMax = 2100;
                    
                    _deviceConfig.TiltNozzleMin = 900;
                    _deviceConfig.TiltCameraMin = 1240;
                    

                    _deviceConfig.TiltNozzleDefault = 1500;
                    _deviceConfig.TiltCameraDefault = 1500;

                    _deviceConfig.PanMax = 1500;
                    _deviceConfig.PanMin = -1500;

                    _deviceConfig.PanMax = 800;
                    _deviceConfig.PanMin = -950;

                    _deviceConfig.WaterOnDelay = new TimeSpan(0, 0, 0, 0, 600);
                    _deviceConfig.WaterOffDelay = new TimeSpan(0, 0, 0, 0, 300);

                    _deviceConfig.PixelWidth = 1280;
                    _deviceConfig.PixelHeight = 720;
                }

                return _deviceConfig;
            }
            set
            {
                _deviceConfig = value;
            }
        }

        private ImageProcessorConfig _imageProcessorConfig;
        public ImageProcessorConfig ImageProcessorConfig
        {
            get
            {
                if (_imageProcessorConfig == null)
                {
                    _imageProcessorConfig = new ImageProcessorConfig();

                    _imageProcessorConfig.BlobMinHeight = 50;
                    _imageProcessorConfig.BlobMinWidth = 50;

                    _imageProcessorConfig.AngleError1 = 45;
                    _imageProcessorConfig.AngleError1 = 40;
                    _imageProcessorConfig.AngleError2 = 75;
                    _imageProcessorConfig.AngleError2 = 60;
                    _imageProcessorConfig.LengthError = 1.3;

                    _imageProcessorConfig.BorderTestSize = 3;
                    _imageProcessorConfig.BorderTestDifference = 20;

                    _imageProcessorConfig.QuadrilateralTransformationWidth = 400;
                    _imageProcessorConfig.QuadrilateralTransformationHeight = 400;

                    _imageProcessorConfig.ColorTestWhite = 0.1;

                    _imageProcessorConfig.ChannelFilterBase = 160;

                    _imageProcessorConfig.TrackingGiveupTime = new TimeSpan(0, 0, 0, 0, 1000);

                    _imageProcessorConfig.Threshold = 40;

                    _imageProcessorConfig.MarkerSize = 6.3;

                }
                return _imageProcessorConfig;
            }
            set
            {
                _imageProcessorConfig = value;
            }
        }

        private WateringConfig _wateringConfig;
        public WateringConfig WateringConfig
        {
            get { return _wateringConfig; }
            set { _wateringConfig = value; }
        }


        private ViewConfig _viewConfig;
        public ViewConfig ViewConfig
        {
            get
            {
                if (_viewConfig == null)
                {
                    _viewConfig = new ViewConfig();

                    _viewConfig.BackgroundImage = ViewConfigBackgroundImage.CameraRaw;
                    _viewConfig.HudCameraAim = true;
                    _viewConfig.HudPump = true;
                    _viewConfig.HudPanAngle = true;
                    _viewConfig.HudTiltNozzleAngle = true;
                    _viewConfig.HudTiltCameraAngle = true;
                    _viewConfig.HudTimeNow = true;
                    _viewConfig.HudValve = true;

                    _viewConfig.MarkerOverlayBorder = true;
                    _viewConfig.MarkerOverlayDiagonal = true;
                    _viewConfig.MarkerOverlayGraphic = true;

                    _viewConfig.UseJoystick = false;
                    _viewConfig.UseKeyboard = true;
                }
                return _viewConfig;
            }
            set
            {
                _viewConfig = value;
            }
        }


    }
}
