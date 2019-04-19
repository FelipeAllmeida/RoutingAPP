using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solutions.Models
{
    public class Position
    {
        public Position(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X;
        public float Y;

        public double Distance(Position pos)
        {
            return Math.Sqrt(Math.Pow(X - pos.X, 2) + Math.Pow(Y - pos.Y, 2));
        }
    }
}
