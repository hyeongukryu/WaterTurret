using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaterTurret.Module.Models
{
    public class Plant
    {
        /// <summary>
        /// 인간 친화적인 이름
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 대응되는 마커 색상
        /// </summary>
        public MarkerColor MarkerColor { get; set; }

        /// <summary>
        /// 마지막 관수 시각
        /// </summary>
        public DateTime LastWatered { get; set; }

        /// <summary>
        /// 관수 주기
        /// </summary>
        public TimeSpan WaterPeriod { get; set; }

        /// <summary>
        /// 관수 시간
        /// </summary>
        public TimeSpan WaterTime { get; set; }

        /// <summary>
        /// 지금 관수가 필요한지 검사합니다.
        /// </summary>
        public bool CheckWaterRequired()
        {
            return (DateTime.Now - LastWatered) > WaterPeriod;
        }

        /// <summary>
        /// 지정된 관수 시간을 모두 채우고 관수가 끝났을 때 호출합니다.
        /// </summary>
        public void WateredNow()
        {
            LastWatered = DateTime.Now;
        }
    }
}
