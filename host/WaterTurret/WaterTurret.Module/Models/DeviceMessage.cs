using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaterTurret.Module.Models
{
    public class DeviceMessage
    {
        public short Sequence { get; set; }

        public short[] SendData { get; set; }
        public short[] ReceiveData { get; set; }
        
        public DeviceMessageSendType SendType { get; set; }
        public DeviceMessageReceiveType ReceiveType { get; set; }

        /// <summary>
        /// 순차적인 Sequence를 가지는 새로운 DeviceMessage를 반환합니다.
        /// </summary>
        public DeviceMessage()
        {
            Sequence = NextSequence();
        }
        

        /// <summary>
        /// 다음 Sequence 값을 구하고 해당 값을 반환합니다.
        /// </summary>
        /// <returns></returns>
        public static short NextSequence()
        {
            lock (SequenceLock)
            {
                if (++_lastSequence >= short.MaxValue)
                {
                    _lastSequence = 1;
                }

                return _lastSequence;
            }
        }
        private static short _lastSequence = 0;
        private static object SequenceLock = new object();
    }
    
    public enum DeviceMessageSendType
    {
        DeviceCheck = 10,
        Emergency = 11,

        PanGet = 20,
        PanSet = 21,

        TiltNozzleGet = 30,
        TiltNozzleSet = 31,

        TiltCameraGet = 40,
        TiltCameraSet = 41,

        PumpOn = 50,
        PumpOff = 51,

        ValveOn = 52,
        ValveOff = 53
    }

    public enum DeviceMessageReceiveType
    {
        Ok = 42,
        Fail = 100,
        Type = 101
    }
}
