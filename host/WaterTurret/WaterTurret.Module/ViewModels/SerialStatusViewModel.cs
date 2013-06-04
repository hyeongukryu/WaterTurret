using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaterTurret.Common;
using WaterTurret.Module.Services;

namespace WaterTurret.Module.ViewModels
{
    public class SerialStatusViewModel : NavigationViewModel
    {
        public override string RegionName
        {
            get { return RegionNames.LeftRegion; }
        }

        private readonly ISerialService _serialService;

        public SerialStatusViewModel(ISerialService serialService)
        {
            _serialService = serialService;
        }

        protected override void Selected()
        {
        }

        protected override void UnSelected()
        {
        }
    }
}
