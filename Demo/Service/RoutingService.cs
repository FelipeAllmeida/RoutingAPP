using Solutions.Enum;
using Solutions.Models;
using Solutions.Routing.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solutions.Services
{
    public class RoutingService
    {
        public void Route(Enterprise enterprise)
        {
            bool routesNotAssigned = true;
            DateTime startTime = DateTime.Now;

            List<Package> listNotAttendedPackages = enterprise.ListPackages;

            do
            {
                var listRoutes = CreateRoutes(enterprise.StartPos, enterprise.ListFleetVehicles, listNotAttendedPackages);
                routesNotAssigned = listRoutes.Any(x => x.AssignedVehicle == null);
                if (routesNotAssigned)
                {
                    listNotAttendedPackages = new List<Package>();
                    for (int i = 0; i < listRoutes.Count; i ++)
                    {
                        if (listRoutes[i].AssignedVehicle == null)
                        {
                            listNotAttendedPackages.AddRange(listRoutes[i].listPackagesToDeliver);
                        }
                    }
                }
            }
            while (routesNotAssigned);

            DebugCreatedRoutes(enterprise, startTime);
        }

        public List<Route> CreateRoutes(Position startPosition, List<Vehicle> listFleetVehicles, List<Package> listPackages)
        {
            CreateVehicleTravels(listFleetVehicles);

            List<Economy> listEconomies = CreateListEconomies(startPosition, listPackages);
            List<Route> listRoutes = InitializeRoutes(startPosition, listPackages, listFleetVehicles);

            // Itera a lista de economias
            for (int i = 0; i < listEconomies.Count; i++)
            {
                var economy = listEconomies[i];

                // Verifica se o package A existe dentro da lista de rotas em uma das extremidades
                Route routeA = FindPackageOnRouteEdges(listRoutes, economy.packageAId);
                if (routeA == null) continue;

                // Verifica se o packagee B existe dentro da lista de rotas em uma das extremidades
                Route routeB = FindPackageOnRouteEdges(listRoutes, economy.packageBId);
                if (routeB == null) continue;

                // Verifica se o package A e o packagee B já estão na mesma rota
                // e se ambos estão nas extremidades, caso sim, vai para a próxima iteração
                if (routeA.Id == routeB.Id) continue;

                //Ambos os packagees encontrados, pega o totalDistance e o totalDemmand
                double newRouteSize = routeA.Size + routeB.Size - economy.value;
                Cargo newRouteCargo = routeA.Cargo + routeB.Cargo;

                //Procura menor veiculo que comporta a rota combinada, caso nenhum seja encontrado, pula para a próxima economia
                Vehicle vehicle = FindVehicleForRoute(listFleetVehicles, newRouteSize, newRouteCargo);

                // Todos os veiculos sobrecarregados para o dia de hoje, avança para próximo dia
                // - Salvar a economia da rota (soma da economia dos clientes)
                // - Quando sobrecarregar veiculos por distancia percorrida: 
                //      Pega as rotas sem veiculo alocado e passa para o segundo dia;  
                //      Executa CreateRoutes para a lista de clientes do segundo dia;

                if (vehicle != null)
                {
                    // Remove todos os veiculos alocados para as duas rotas
                    routeA.RemoveAssignedVehicle();
                    routeB.RemoveAssignedVehicle();

                    // Verifica a posição em que os pacotes estão na rota
                    RouteEdge packageAEdge = CheckPackagePosition(routeA, economy.packageAId);
                    RouteEdge packageBEdge = CheckPackagePosition(routeB, economy.packageBId);

                    // Se package A está na esquerda e packagee B também, inverte B e adiciona em A
                    if (packageAEdge == RouteEdge.LEFT && packageBEdge == RouteEdge.LEFT)
                    {
                        MergeRoutes(listRoutes, routeA, routeB, vehicle, true);
                    }
                    // Se package A está na esquerda e packagee B a direita, adiciona em A
                    else if (packageAEdge == RouteEdge.LEFT && packageBEdge == RouteEdge.RIGHT)
                    {
                        MergeRoutes(listRoutes, routeA, routeB, vehicle, false);
                    }
                    // Se package A está na direita e packagee B a esquerda, adiciona em B
                    else if (packageAEdge == RouteEdge.RIGHT && packageBEdge == RouteEdge.LEFT)
                    {
                        MergeRoutes(listRoutes, routeB, routeA, vehicle, false);
                    }
                    // Se package A está na direita e packagee B também, inverte A e adiciona em B
                    else if (packageAEdge == RouteEdge.RIGHT && packageBEdge == RouteEdge.RIGHT)
                    {
                        MergeRoutes(listRoutes, routeB, routeA, vehicle, true);
                    }
                }
            }

            return listRoutes;
        }

        #region Internal
        private void CreateVehicleTravels(List<Vehicle> listVehicles)
        {
            for(int i = 0; i < listVehicles.Count; i ++)
            {
                listVehicles[i].AddTravel();
            }
        }

        private List<Economy> CreateListEconomies(Position enterprisePos, List<Package> packages)
        {
            List<Economy> listEconomies = new List<Economy>();

            for (int i = 0; i < packages.Count; i ++)
            {
                Package clientA = packages[i];

                double distClientA = clientA.DeliverPosition.Distance(enterprisePos);

                for (int j = i; j < packages.Count; j ++)
                {
                    if (i == j) continue;

                    Package clientB = packages[j];

                    double distClientB = clientB.DeliverPosition.Distance(enterprisePos);
                    double distClientAB = clientA.DeliverPosition.Distance(clientB.DeliverPosition);

                    listEconomies.Add(new Economy()
                    {
                        packageAId = clientA.Id,
                        packageBId = clientB.Id,
                        value = distClientA + distClientB - distClientAB
                    });
                }
            }

            return listEconomies.OrderByDescending(x => x.value).ToList();
        }

        private List<Route> InitializeRoutes(Position enterprisePos, List<Package> packages, List<Vehicle> vehicles)
        {
            var listRoutes = packages.Select(x => new Route(enterprisePos, x)).ToList();
            AssignVehiclesToRoutes(listRoutes, vehicles);
            return listRoutes;
        }

        private void AssignVehiclesToRoutes(List<Route> routes, List<Vehicle> listVehicles)
        {
            for (int i = 0; i < routes.Count; i++)
            {
                AssignVehicleToRoute(routes[i], listVehicles);
            }
        }
        
        private bool AssignVehicleToRoute(Route route, List<Vehicle> listVehicles)
        {
            for (int i = 0; i < listVehicles.Count; i ++)
            {
                if (listVehicles[i].GetCurrentTravel().AddRoute(route))
                {
                    return true;
                }
            }

            return false;
        }

        private Vehicle FindVehicleForRoute(List<Vehicle> listVehicles, double routeSize, Cargo routeCargo)
        {
            for (int i = 0; i < listVehicles.Count; i ++)
            {
                if (listVehicles[i].GetCurrentTravel().DoesSupportRoute(routeSize, routeCargo))
                {
                    return listVehicles[i];
                }
            }

            return null;
        }

        private Route FindPackageOnRouteEdges(List<Route> listRoutes, int packageId)
        {
            // Itera a lista de rotas em busca dos packagees assigned para a mesma
            if (listRoutes.IsNullOrEmpty() == false)
            {
                for (int i = 0; i < listRoutes.Count; i ++)
                {
                    if (listRoutes[i].listPackagesToDeliver[0].Id == packageId
                    || listRoutes[i].listPackagesToDeliver[listRoutes[i].listPackagesToDeliver.Count - 1].Id == packageId) 
                    {
                        return listRoutes[i];
                    }
                }
            }

            return null;
        }

        private RouteEdge CheckPackagePosition(Route route, int packageId)
        {
            if (route.listPackagesToDeliver.IsNullOrEmpty()) return RouteEdge.NOT_ON_EDGE;
            if (route.listPackagesToDeliver[0].Id == packageId) return RouteEdge.LEFT;
            if (route.listPackagesToDeliver[route.listPackagesToDeliver.Count - 1].Id == packageId) return RouteEdge.RIGHT;

            return RouteEdge.NOT_ON_EDGE;
        }

        private void MergeRoutes(List<Route> listRoutes, Route route, Route routeToMerge, Vehicle vehicle, bool invertMerged)
        {
            listRoutes.Remove(routeToMerge);
            if (invertMerged)
                route.listPackagesToDeliver = route.listPackagesToDeliver.OrderByDescending(x => x.Id).ToList();
            route.Merge(routeToMerge);
            vehicle.GetCurrentTravel().AddRoute(route);
        }
        #endregion

        #region Test
        private void DebugCreatedRoutes(Enterprise enterprise, DateTime startExecutionTime)
        {
            Cargo totalClientDemmand = Cargo.Zero();
            //Cargo totalVehicleDemmand = Cargo.Zero();
            Cargo totalVehicleDemmandCapacity = Cargo.Zero();

            enterprise.ListPackages.ForEach(x => totalClientDemmand += x.Cargo);
            //enterprise.ListFleetVehicles.ForEach(x => totalVehicleDemmand += x.Demmand);
            enterprise.ListFleetVehicles.ForEach(x => totalVehicleDemmandCapacity += x.MaxCargo);

            System.Diagnostics.Debug.WriteLine("#############################################");
            System.Diagnostics.Debug.WriteLine($"Execution Time: {(DateTime.Now - startExecutionTime)}");

            System.Diagnostics.Debug.WriteLine($"{enterprise.ListPackages.Count} packages with volume of {totalClientDemmand.Volume}");

            System.Diagnostics.Debug.WriteLine($"{enterprise.ListFleetVehicles.Count} vehicles with cargo capacity {totalVehicleDemmandCapacity.ToString()}");

            System.Diagnostics.Debug.WriteLine("#############################################");

            System.Diagnostics.Debug.WriteLine($"Trucks:");
            for (int i = 0; i < enterprise.ListFleetVehicles.Count; i++)
            {
                var truck = enterprise.ListFleetVehicles[i];
                for (int j = 0; j < truck.listTravel.Count; j ++)
                {
                    var travel = truck.listTravel[j];
                    System.Diagnostics.Debug.WriteLine($"[{i.ToString("00")}] [Travel:{j}] TraveledDistance: {travel.TraveledDistance} | Cargo: {travel.CurrentCargo} | Routes: ");

                    for (int k = 0; k < travel.listRoutes.Count; k++)
                    {
                        var route = travel.listRoutes[k];
                        System.Diagnostics.Debug.WriteLine($"     -> Route {route.Id} | Clients: {route.listPackagesToDeliver.Count} | Size: {route.Size}");
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine("#############################################");
        }
        #endregion
    }
}
