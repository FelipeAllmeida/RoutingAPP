using Demo.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Models
{
    public class Travel
    {
        public Travel(Position startPosition)
        {
            StartPosition = startPosition;
        }

        public List<ClientModel> listClient { get; set; }

        public double TotalDistance { get; private set; }

        public VehicleModel AssignedVehicle { get; private set; }
        public Position StartPosition { get; private set; }
        public DemmandModel TotalDemmand { get; private set; }

        public bool AssignVehicle(VehicleModel vehicle)
        {
            if (vehicle.VolumeCapacity >= TotalDemmand.Volume)
            {
                if (vehicle.MaxKM >= TotalDistance + vehicle.CurrentKM)
                {
                    AssignedVehicle = vehicle;
                    AssignedVehicle.AddTraveledKM(TotalDistance);
                    return true;
                }
            }

            return false;
        }

        public void Update()
        {
            double distance = 0f;
            float totalVolume = 0f;

            for (int i = 0; i < listClient.Count; i ++)
            {
                ClientModel currentClient = listClient[i];

                totalVolume += currentClient.Cargo.Volume;

                if (i == 0) continue;

                ClientModel lastClient = listClient[i - 1];

                distance += MathUtils.Distance(currentClient.DeliverPosition, lastClient.DeliverPosition);
            }

            distance += MathUtils.Distance(listClient[0].DeliverPosition, StartPosition);
            distance += MathUtils.Distance(StartPosition, listClient[listClient.Count - 1].DeliverPosition);

            TotalDistance = distance;
            TotalDemmand = new DemmandModel() { Volume = totalVolume };
        }
    }
}
