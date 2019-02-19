using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Models
{
    public class EnterpriseModel : BaseClient
    {   
        public Dictionary<int, double> DictDistanceClients { get; set; }

        public List<EconomyModel> ListEconomy { get; set; }

        public List<Position> ListStartPoints { get; set; }
        public List<ClientModel> ListClients { get; set; }
        public List<VehicleModel> ListFleetVehicles { get; set; }
    }
}
