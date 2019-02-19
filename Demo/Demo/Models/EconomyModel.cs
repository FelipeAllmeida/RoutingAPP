namespace Demo.Models
{
    public class EconomyModel
    {
        public int clientAId;
        public int clientBId;

        public double distanceClients;
        public double economy;

        public void Calculate(double distanceClientA, double distanceClientB)
        {
            economy = distanceClientA + distanceClientB - distanceClients;
        }
    }
}
