using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using WaterTurret.Module.Models;
using Microsoft.Practices.Prism.ViewModel;

namespace WaterTurret.Module.Services
{
    public class DeviceService : NotificationObject, IDeviceService
    {
        private readonly IConfigService _configService;
        private readonly ISerialService _serialService;

        public DeviceService(IConfigService configSerivce, ISerialService serialService)
        {
            _configService = configSerivce;
            _serialService = serialService;
        }

        static object _lock = new object();

        private DeviceMessage SendAndReadPacketSync(DeviceMessageSendType type, short[] data)
        {
            lock (_lock)
            {
                DeviceMessage message = new DeviceMessage();
                message.SendType = type;
                message.SendData = data;
                _serialService.SendAndReceivePacketSync(message);
                return message;
            }
        }

        private DeviceMessage SendAndReadPacketSync(DeviceMessageSendType type)
        {
            lock (_lock)
            {
                return SendAndReadPacketSync(type, null);
            }
        }

        public void Open()
        {
            _serialService.Open();
        }

        public void Close()
        {
            _serialService.Close();
        }

        public void DeviceCheck()
        {
            SendAndReadPacketSync(DeviceMessageSendType.DeviceCheck);
        }

        public void Emergency()
        {
            SendAndReadPacketSync(DeviceMessageSendType.Emergency);
        }

        #region Pump / Valve Status
        bool _isPumpOn = false;
        public bool IsPumpOn
        {
            get { return _isPumpOn; }
            private set
            {
                if (_isPumpOn != value)
                {
                    _isPumpOn = value;
                    RaisePropertyChanged(() => IsPumpOn);
                    RaisePropertyChanged(() => IsPumpOff);
                }
            }
        }
        public bool IsPumpOff
        {
            get { return !_isPumpOn; }
        }
        bool _isValveOn = false;
        public bool IsValveOn
        {
            get { return _isValveOn; }
            private set
            {
                if (_isValveOn != value)
                {
                    _isValveOn = value;
                    RaisePropertyChanged(() => IsValveOn);
                    RaisePropertyChanged(() => IsValveOff);
                }
            }
        }
        public bool IsValveOff
        {
            get { return !_isValveOn; }
        }
        #endregion

        public void PumpOn()
        {
            lock (_lock)
            {
                SendAndReadPacketSync(DeviceMessageSendType.PumpOn);
                IsPumpOn = true;
            }
        }

        public void PumpOff()
        {
            lock (_lock)
            {
                SendAndReadPacketSync(DeviceMessageSendType.PumpOff);
                IsPumpOn = false;
            }
        }

        public void ValveOn()
        {
            lock (_lock)
            {
                SendAndReadPacketSync(DeviceMessageSendType.ValveOn);
                IsValveOn = true;
            }
        }

        public void ValveOff()
        {
            lock (_lock)
            {
                SendAndReadPacketSync(DeviceMessageSendType.ValveOff);
                IsValveOn = false;
            }
        }
        public void WaterOn()
        {
            lock (_lock)
            {
                new Action(() =>
                {
                    lock (_lock)
                    {
                        PumpOn();
                        Thread.Sleep(_configService.DeviceConfig.WaterOnDelay);
                        ValveOn();
                    }
                }).BeginInvoke(null, null);
            }
        }

        public void WaterOff()
        {
            lock (_lock)
            {
                new Action(() =>
                {
                    lock (_lock)
                    {
                        ValveOff();
                        Thread.Sleep(_configService.DeviceConfig.WaterOffDelay);
                        PumpOff();
                    }
                }).BeginInvoke(null, null);
            }
        }

        private short SafeValue(short value, short min, short max)
        {
            if (min > value)
            {
                return min;
            }

            if (max < value)
            {
                return max;
            }

            return value;
        }

        public short PanGet()
        {
            var message = SendAndReadPacketSync(DeviceMessageSendType.PanGet);
            return message.ReceiveData.First();
        }

        bool _panRunning = false;

        public void PanSet(short absolute, short sleep)
        {
            lock (_lock)
            {
                _panRunning = true;
                new Action(() =>
                {
                    var value = SafeValue(absolute, _configService.DeviceConfig.PanMin, _configService.DeviceConfig.PanMax);
                    SendAndReadPacketSync(DeviceMessageSendType.PanSet, new[] { value, sleep });
                    _panRunning = false;
                }).BeginInvoke(null, null);
            }
        }

        public void PanAdd(short step, short sleep)
        {
            lock (_lock)
            {
                var pan = PanGet();
                PanSet((short)(pan + step), sleep);
            }
        }

        public short TiltNozzleGet()
        {
            var message = SendAndReadPacketSync(DeviceMessageSendType.TiltNozzleGet);
            return message.ReceiveData.First();
        }

        public void TiltNozzleSet(short width)
        {
            var value = SafeValue(width, _configService.DeviceConfig.TiltNozzleMin, _configService.DeviceConfig.TiltNozzleMax);
            SendAndReadPacketSync(DeviceMessageSendType.TiltNozzleSet, new[] { value });
        }

        public void TiltNozzleAdd(short width)
        {
            var tilt = TiltNozzleGet();
            TiltNozzleSet((short)(tilt + width));
        }

        public short TiltCameraGet()
        {
            var message = SendAndReadPacketSync(DeviceMessageSendType.TiltCameraGet);
            return message.ReceiveData.First();
        }

        public void TiltCameraSet(short width)
        {
            var value = SafeValue(width, _configService.DeviceConfig.TiltCameraMin, _configService.DeviceConfig.TiltCameraMax);
            SendAndReadPacketSync(DeviceMessageSendType.TiltCameraSet, new[] { value });
        }

        public void TiltCameraAdd(short width)
        {
            var tilt = TiltCameraGet();
            TiltCameraSet((short)(tilt + width));
        }


        public bool PanRunning()
        {
            return _panRunning;
        }
    }
}
