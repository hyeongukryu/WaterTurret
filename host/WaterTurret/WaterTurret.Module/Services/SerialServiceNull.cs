using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaterTurret.Module.Services
{
    public class SerialServiceNull : ISerialService
    {
        public void Open()
        {
        }

        public void Close()
        {
        }

        public void ReceivePacketSync(Models.DeviceMessage message)
        {
        }

        public void SendPacketSync(Models.DeviceMessage message)
        {
        }

        public void SendAndReceivePacketSync(Models.DeviceMessage message)
        {
        }
    }
}
