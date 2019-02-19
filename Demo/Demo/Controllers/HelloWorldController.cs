using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Demo.Controllers
{
    public class HelloWorldController : Controller
    {
        public IActionResult Index()
        {
            AlgoritimoDoLucao();

            return View();
        }

        public string Welcome()
        {
            return "This is the Welcome action method...";
        }

        private void AlgoritimoDoLucao()
        {
            var enterprises = InitializeMockedEnterpriseData();
            CreateDistanceBetweenPairs(enterprises);
            CalculateEconomies(enterprises);
        }

        #region Parte 1 - Distancia entre pares de clientes e distancia entre ponto de entrega/empresa
        private void CreateDistanceBetweenPairs(List<EnterpriseModel> listEnterprises)
        {
            for (int i = 0; i < listEnterprises.Count; i++)
            {
                var enterprise = listEnterprises[i];

                enterprise.DictDistanceClients = new Dictionary<int, double>();

                for (int j = 0; j < listEnterprises[i].ListClients.Count; j++)
                {
                    var client = listEnterprises[i].ListClients[j];

                    // Sqrt((x1 - x2)^2 + (y1 - y2)^2)

                    var distance = Math.Sqrt(
                                Math.Pow((client.DeliverPosition.X - enterprise.ListStartPoints[0].X), 2) +
                                Math.Pow((client.DeliverPosition.Y - enterprise.ListStartPoints[0].Y), 2));

                    enterprise.DictDistanceClients.Add(client.Id, distance);
                }

                // Iterate trough all list clients

                enterprise.ListEconomy = new List<EconomyModel>();

                for (int j = 0; j < enterprise.ListClients.Count; j++) // ID 5
                {
                    for (int k = 0; k < listEnterprises[i].ListClients.Count; k++) // ID 0
                    {
                        if (j == k) continue;

                        var clientA = enterprise.ListClients[j];
                        var clientB = enterprise.ListClients[k];

                        bool doesEconomyExist = enterprise.ListEconomy.Find(x => 
                            (x.clientAId == clientA.Id && x.clientBId == clientB.Id) ||
                            x.clientAId == clientB.Id && x.clientBId == clientA.Id) != null;

                        // Check if the same distance is not in the list of economy
                        if (doesEconomyExist == false)
                        {
                            // Calculate the distance Sqrt(deltaX^2 + deltaY^2);
                            var distanceClients = Math.Sqrt(
                                Math.Pow((clientA.DeliverPosition.X - clientB.DeliverPosition.X), 2) +
                                Math.Pow((clientA.DeliverPosition.Y - clientB.DeliverPosition.Y), 2));

                            EconomyModel economyModel = new EconomyModel();
                            economyModel.clientAId = clientA.Id;
                            economyModel.clientBId = clientB.Id;
                            economyModel.distanceClients = distanceClients;

                            enterprise.ListEconomy.Add(economyModel);
                        }
                    }
                }
            }
        }
        #endregion

        #region Parte 2 - Calcular as economias e ordenar DESC
        private void CalculateEconomies(List<EnterpriseModel> listEnterprises)
        {
            for (int i = 0; i < listEnterprises.Count; i ++)
            {
                var enterprise = listEnterprises[i];

                for(int j = 0; j < enterprise.ListEconomy.Count; j ++)
                {
                    var economy = enterprise.ListEconomy[j];
                    economy.Calculate(enterprise.DictDistanceClients[economy.clientAId], enterprise.DictDistanceClients[economy.clientBId]);
                }

                enterprise.ListEconomy = enterprise.ListEconomy.OrderByDescending(x => x.economy).ToList();
            }
        }
        #endregion

        #region Parte 3 - Routing 
        private void Routing(List<EnterpriseModel> listEnterprises)
        {
            for (int i = 0; i < listEnterprises.Count; i ++)
            {

            }
        }
        #endregion

        #region Mocked Data
        private List<EnterpriseModel> InitializeMockedEnterpriseData()
        {
            List<EnterpriseModel> listEnterprises = new List<EnterpriseModel>();

            for (int i = 0; i < 5; i++)
            {
                var enterpriseModel = new EnterpriseModel();
                enterpriseModel.Id = i;
                enterpriseModel.Name = $"Enterprise-{i}";
                enterpriseModel.ListStartPoints = new List<Position>();
                enterpriseModel.ListStartPoints.Add(CreateRandomPos());
                enterpriseModel.ListFleetVehicles = CreateFleetModel();
                enterpriseModel.ListClients = InitializeMockedClientData();

                listEnterprises.Add(enterpriseModel);
            }

            return listEnterprises;
        }

        private List<ClientModel> InitializeMockedClientData()
        {
            List<ClientModel> listClients = new List<ClientModel>();

            int randomClientNumber = new Random().Next(10, 100);

            for (int i = 0; i < randomClientNumber; i++)
            {
                ClientModel clientModel = new ClientModel();
                clientModel.Id = i;
                clientModel.Name = $"Client-{i}";
                clientModel.Cargo = CreateMockedDemmandData();
                clientModel.DeliverPosition = CreateRandomPos();

                listClients.Add(clientModel);
            }

            return listClients;
        }

        private List<VehicleModel> CreateFleetModel()
        {
            List<VehicleModel> listVehicle = new List<VehicleModel>();

            int vehicleRandomNumber = new Random().Next(3, 12);

            for (int i = 0; i < vehicleRandomNumber; i++)
            {
                VehicleModel vehicleModel = new VehicleModel();

                int volume = new Random().Next(5, 30);

                vehicleModel.VolumeCapacity = volume;
                vehicleModel.WeightCapacity = volume * 1000f;

                listVehicle.Add(vehicleModel);
            }

            return listVehicle;
        }

        private DemmandModel CreateMockedDemmandData()
        {
            DemmandModel demmand = new DemmandModel();

            int volume = new Random().Next(5, 20);

            demmand.Volume = volume;
            demmand.Weight = volume * 1000;// On the future this can be change to consider the object type

            return demmand;
        }
        #endregion

        private Position CreateRandomPos()
        {
            Random random = new Random();
            return new Position(random.Next((int)(2 * Math.Pow(10, 8)), (int)(3 * Math.Pow(10, 8))),
                    random.Next((int)(5 * Math.Pow(10, 8)), (int)(6 * Math.Pow(10, 8))));
        }

    }
}