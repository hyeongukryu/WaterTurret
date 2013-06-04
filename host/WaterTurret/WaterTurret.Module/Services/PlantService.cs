using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WaterTurret.Module.Models;

namespace WaterTurret.Module.Services
{
    class PlantService : IPlantService
    {
        Plants _model;

        public Plants GetModel()
        {
            if (_model == null)
            {
                _model = new Plants();

                _model.Add(new Plant
                {
                    Name = "율마",
                    MarkerColor = MarkerColor.Green,
                    WaterPeriod = new TimeSpan(10, 0, 0),
                    WaterTime = new TimeSpan(0, 0, 2),
                    LastWatered = DateTime.MinValue
                });
                _model.Add(new Plant
                {
                    Name = "아이비",
                    MarkerColor = MarkerColor.Red,
                    WaterPeriod = new TimeSpan(10, 0, 0),
                    WaterTime = new TimeSpan(0, 0, 7),
                    LastWatered = DateTime.MinValue
                });
                _model.Add(new Plant
                {
                    Name = "스파티필름",
                    MarkerColor = MarkerColor.Blue,
                    WaterPeriod = new TimeSpan(10, 0, 0),
                    WaterTime = new TimeSpan(0, 0, 5),
                    LastWatered = DateTime.MinValue
                });
            }

            return _model;
        }
    }
}
