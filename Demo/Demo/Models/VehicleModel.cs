using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Models
{
    public class VehicleModel
    {
        public double CurrentKM { get; private set; } = 0d;
        public double MaxKM { get; private set; } = 150000d;
        public float WeightCapacity { get; set; }
        public float VolumeCapacity { get; set; }

        public void AddTraveledKM(double traveledKM)
        {
            CurrentKM += traveledKM;
        }
    }
}
