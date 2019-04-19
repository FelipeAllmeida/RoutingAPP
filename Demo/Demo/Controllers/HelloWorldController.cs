using Solutions.Models;
using Microsoft.AspNetCore.Mvc;
using Solutions.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Solutions.Controllers
{
    public class HelloWorldController : Controller
    {
        public IActionResult Index()
        {
            RoutingService routingService = new RoutingService();

            routingService.Route(CreateEnterpriseData());

            return View();
        }      

        public string Welcome()
        {
            return "This is the Welcome action method...";
        }

        
        private Enterprise CreateEnterpriseData()
        {
            Enterprise enterprise = new Enterprise();

            enterprise.Id = 0;
            enterprise.Name = $"Enterprise-{0}";
            enterprise.StartPos = CreateRandomPos();
            enterprise.ListFleetVehicles = CreateFleetModel();
            enterprise.ListPackages = CreateListClients();

            return enterprise;
        }

        private List<Vehicle> CreateFleetModel()
        {
            List<Vehicle> listVehicle = new List<Vehicle>();

            int vehicleRandomNumber = new Random().Next(4, 8);

            for (int i = 0; i < vehicleRandomNumber; i++)
            {
                Vehicle vehicleModel = new Vehicle();

                int volume = new Random().Next(20, 45);

                vehicleModel.MaxCargo = new Cargo()
                {
                    Volume = volume,
                    Weight = volume * 1000f
                };

                listVehicle.Add(vehicleModel);
            }

            return listVehicle.OrderBy(x => x.MaxCargo.Weight).ToList();
        }

        private List<Package> CreateListClients()
        {
            List<Package> listClients = new List<Package>();

            int randomClientNumber = new Random().Next(20, 30);

            for (int i = 0; i < randomClientNumber; i++)
            {
                Package packageModel = new Package();
                packageModel.Id = i;
                packageModel.Name = $"Client-{i}";
                packageModel.Cargo = CreateMockedDemmandData();
                packageModel.DeliverPosition = CreateRandomPos();

                listClients.Add(packageModel);
            }

            return listClients;
        }

        private Cargo CreateMockedDemmandData()
        {
            Cargo demmand = new Cargo();

            int volume = new Random().Next(3, 10);

            demmand.Volume = volume;

            return demmand;
        }

        private Position CreateRandomPos()
        {
            Random random = new Random();
            return new Position(random.Next((int)(2.5 * Math.Pow(10, 2)), (int)(3 * Math.Pow(10, 2))),
                    random.Next((int)(5.5 * Math.Pow(10, 2)), (int)(6 * Math.Pow(10, 2))));
        }

    }
}                   