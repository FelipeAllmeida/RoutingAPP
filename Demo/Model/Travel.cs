using Solutions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solutions.Models
{
    public class Travel
    {
        public Travel(Vehicle assignedVehicle, double maxTravelDistance, Cargo maxCargo)
        {
            TraveledDistance = 0;
            CurrentCargo = Cargo.Zero();

            AssignedVehicle = assignedVehicle;
            MaxTravelDistance = MaxTravelDistance;
            MaxCargo = maxCargo;
        }

        public double TraveledDistance { get; private set; }
        public Cargo CurrentCargo { get; private set; }

        public double MaxTravelDistance { get; private set; } = 1000;
        public Cargo MaxCargo { get; set; }

        public Vehicle AssignedVehicle { get; private set; } 

        public List<Route> listRoutes = new List<Route>();

        public bool AddRoute(Route route)
        {
            if (DoesSupportRoute(route.Size, route.Cargo))
            {
                TraveledDistance += route.Size;
                CurrentCargo += route.Cargo;
                route.AssignedVehicle = AssignedVehicle;
                listRoutes.Add(route);
                return true;
            }

            return false;
        }

        public bool RemoveRoute(Route route)
        {
            if (listRoutes.Remove(route))
            {
                TraveledDistance -= route.Size;
                CurrentCargo -= route.Cargo;
                return true;
            }
            return false;
        }

        public bool DoesSupportRoute(double routeSize, Cargo routeCargo)
        {
            return TraveledDistance + routeSize <= MaxTravelDistance
                && CurrentCargo.Weight + routeCargo.Weight <= MaxCargo.Weight
                && CurrentCargo.Volume + routeCargo.Volume <= MaxCargo.Volume;            
        }
    }
}
