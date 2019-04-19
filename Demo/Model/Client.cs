using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solutions.Models
{
    public class Package : BaseRoutingObject
    {
        public string AssignedRouteId;
        public Position DeliverPosition;
        public Cargo Cargo;
    }
}
