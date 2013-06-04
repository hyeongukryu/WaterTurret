using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing.Imaging;
using System.Threading;

using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using WaterTurret.Module.Helpers;

namespace WaterTurret.Module.Services
{
    public class CameraService : ICameraService
    {
        private IVideoSource _source;
        public IVideoSource Source
        {
            get
            {
                return _source;
            }
            set
            {
                if (_source != null)
                {
                    if (_source.IsRunning)
                    {
                        _source.Stop();
                    }
                    _source.NewFrame -= _source_NewFrame;
                }

                _source = value;

                if (_source != null)
                {
                    _source.NewFrame += _source_NewFrame;
                    _source.Start();
                }
            }
        }

        AutoResetEvent _newFrameEvent = new AutoResetEvent(false);

        Bitmap _currentFrame;

        void _source_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            var frame = eventArgs.Frame;
            _currentFrame = frame.CloneBitmap();
            _newFrameEvent.Set();
        }
        
        public Bitmap CurrentFrame
        {
            get
            {
                return _currentFrame;
            }
        }

        public void WaitNewFrame()
        {
            _newFrameEvent.WaitOne();
        }

        private readonly IConfigService _configService;
        public CameraService(IConfigService configService)
        {
            _configService = configService;

            var collection = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            // 가장 처음에 나오는 장치를 사용합니다.
            var _device = new VideoCaptureDevice(collection[0].MonikerString);
            _device.DesiredFrameSize = new Size
            {
                Width = _configService.DeviceConfig.PixelWidth,
                Height = _configService.DeviceConfig.PixelHeight
            };
            this.Source = _device;
        }
    }
}
