using System;
using System.Collections.Generic;

namespace Solutions.Models
{
    public class Route
    {
        public Route(Position startPosition, Package package)
        {
            Id = Guid.NewGuid().ToString();
            StartPosition = startPosition;
            package.AssignedRouteId = Id;
            listPackagesToDeliver = new List<Package>() { package };
            Update();
        }

        public string Id { get; private set; } 

        public List<Package> listPackagesToDeliver { get; set; } = new List<Package>();

        public Position StartPosition { get; private set; }

        public double Size { get; private set; }
        public Cargo Cargo { get; private set; }

        public Vehicle AssignedVehicle { get; set; }

        private void Update()
        {
            double distance = 0f;
            Cargo = new Cargo();           

            for (int i = 0; i < listPackagesToDeliver.Count; i ++)
            {
                Package currentPackage = listPackagesToDeliver[i];

                Cargo += currentPackage.Cargo;

                if (i == 0) continue;

                Package lastPackage = listPackagesToDeliver[i - 1];
                distance += currentPackage.DeliverPosition.Distance(lastPackage.DeliverPosition);
            }

            distance += listPackagesToDeliver[0].DeliverPosition.Distance(StartPosition);
            distance += StartPosition.Distance(listPackagesToDeliver[listPackagesToDeliver.Count - 1].DeliverPosition);

            Size = distance;
        }

        public void Merge(Route route)
        {
            route.listPackagesToDeliver.ForEach(x => x.AssignedRouteId = Id);
            listPackagesToDeliver.AddRange(route.listPackagesToDeliver);
            Update();
        }

        public void RemoveAssignedVehicle()
        {
            if (AssignedVehicle != null)
            {
                AssignedVehicle.GetCurrentTravel().RemoveRoute(this);
                AssignedVehicle = null;
            }
        }
    }
}
