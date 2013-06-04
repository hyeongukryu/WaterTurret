using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace WaterTurret.Module.Services
{
    public interface IDeviceService : INotifyPropertyChanged
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
        /// 장치의 상태를 확인합니다. 문제가 있으면 예외가 throw 됩니다.
        /// </summary>
        void DeviceCheck();
        /// <summary>
        /// 장치 상태를 비상으로 전환합니다.
        /// </summary>
        void Emergency();

        /// <summary>
        /// 펌프 전원을 공급합니다.
        /// </summary>
        void PumpOn();
        /// <summary>
        /// 펌프 전원을 차단합니다.
        /// </summary>
        void PumpOff();

        /// <summary>
        /// 밸브 전원을 공급합니다.
        /// </summary>
        void ValveOn();
        /// <summary>
        /// 밸브 전원을 차단합니다.
        /// </summary>
        void ValveOff();

        /// <summary>
        /// 살수를 시작합니다. 지정된 지연 설정이 적용됩니다.
        /// </summary>
        void WaterOn();
        /// <summary>
        /// 살수를 중단합니다. 지정된 지연 설정이 적용됩니다.
        /// </summary>
        void WaterOff();

        /// <summary>
        /// 팬 스탭을 반환합니다.
        /// </summary>
        short PanGet();
        /// <summary>
        /// 팬 스탭을 직접 설정합니다.
        /// </summary>
        void PanSet(short absolute, short sleep);
        /// <summary>
        /// 팬 스탭에 지정된 스탭을 추가합니다.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="sleep"></param>
        void PanAdd(short step, short sleep);

        /// <summary>
        /// 노즐 틸트 폭을 가져옵니다.
        /// </summary>
        short TiltNozzleGet();
        /// <summary>
        /// 노즐 틸트 폭을 설정합니다.
        /// </summary>
        void TiltNozzleSet(short width);
        /// <summary>
        /// 노즐 틸트 폭에 지정된 폭을 추가합니다.
        /// </summary>
        /// <param name="width"></param>
        void TiltNozzleAdd(short width);

        /// <summary>
        /// 카메라 틸트 폭을 가져옵니다.
        /// </summary>
        short TiltCameraGet();
        /// <summary>
        /// 카메라 틸트 폭을 설정합니다.
        /// </summary>
        void TiltCameraSet(short width);
        /// <summary>
        /// 카메라 틸트 폭에 지정된 폭을 추가합니다.
        /// </summary>
        void TiltCameraAdd(short width);



        bool IsPumpOn { get; }
        bool IsPumpOff { get; }
        bool IsValveOn { get; }
        bool IsValveOff { get; }

        bool PanRunning();
    }
}
