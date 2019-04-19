using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solutions.Models
{
    public class Enterprise : BaseRoutingObject
    {           
        public Position StartPos { get; set; }
        public List<Package> ListPackages { get; set; }
        public List<Vehicle> ListFleetVehicles { get; set; }
    }
}
