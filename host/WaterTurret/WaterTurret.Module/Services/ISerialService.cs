using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WaterTurret.Module.Models;

namespace WaterTurret.Module.Services
{
    public interface ISerialService
    {
        /// <summary>
        /// 통신을 시작합니다.
        /// </summary>
        void Open();
        /// <summary>
        /// 통신을 종료합니다.
        /// </summary>
        void Close();
        // TODO 종료 이벤트

        /// <summary>
        /// 보낸 메시지에 대한 응답을 동기적으로 받습니다.
        /// </summary>
        void ReceivePacketSync(DeviceMessage message);

        /// <summary>
        /// 요청하는 메시지를 동기적으로 보냅니다.
        /// </summary>
        void SendPacketSync(DeviceMessage message);

        /// <summary>
        /// 요청하는 메시지를 동기적으로 보내고 응답을 받습니다.
        /// </summary>
        void SendAndReceivePacketSync(DeviceMessage message);
    }
}
