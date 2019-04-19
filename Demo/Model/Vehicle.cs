using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solutions.Models
{
    public class Vehicle
    {
        public List<Travel> listTravel = new List<Travel>();

        public double MaxTravelDistance { get; private set; } = 1000;
        public Cargo MaxCargo { get; set; }

        public void AddTravel()
        {
            listTravel.Add(new Travel(this, MaxTravelDistance, MaxCargo));
        }

        public Travel GetCurrentTravel()
        {
            return listTravel[listTravel.Count - 1];
        }

        //public double AllocatedKM
        //{
        //    get
        //    {
        //        double allocatedKm = 0;
                
        //        for (int i = 0; i < listRoutes.Count; i ++)
        //        {
        //            allocatedKm += listRoutes[i].Size;
        //        }

        //        return allocatedKm;
        //    }
        //}

        //public Cargo Demmand
        //{
        //    get
        //    {
        //        Cargo demmand = Cargo.Zero();

        //        for (int i = 0; i < listRoutes.Count; i++)
        //        {
        //            demmand += listRoutes[i].Cargo;
        //        }

        //        return demmand;
        //    }
        //}

        //public bool AddRoute(Route route)
        //{
        //    if (DoesSupportRoute(route.Size, route.Cargo))
        //    {
        //        route.AssignedVehicle = this;
        //        listRoutes.Add(route);
        //        return true;
        //    }
        //    return false;
        //}

        //public void RemoveRoute(Route route)
        //{
        //    listRoutes.Remove(route);
        //}

        //public bool DoesSupportRoute(double routeSize, Cargo routeDemmand)
        //{
        //    if (AllocatedKM + routeSize <= MaxTravelDistance
        //       && Demmand.Weight + routeDemmand.Weight <= MaxCargo.Weight
        //       && Demmand.Volume + routeDemmand.Volume <= MaxCargo.Volume)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
    }
}
