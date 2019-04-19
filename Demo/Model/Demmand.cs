using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solutions.Models
{
    public class Cargo
    {
        public float Weight { get; set; }
        public float Volume { get; set; }

        public static Cargo Zero()
        {
            return new Cargo()
            {
                Weight = 0f,
                Volume = 0f
            };
        }

        public static Cargo operator +(Cargo cargo1, Cargo cargo2)
        {
            return new Cargo()
            {
                Weight = cargo1.Weight + cargo2.Weight,
                Volume = cargo1.Volume + cargo2.Volume
            };
        }

        public static Cargo operator -(Cargo cargo1, Cargo cargo2)
        {
            return new Cargo()
            {
                Weight = cargo1.Weight - cargo2.Weight,
                Volume = cargo1.Volume - cargo2.Volume
            };
        }

        public override string ToString()
        {
            return $"Volume: {Volume}, Weight: {Weight}";
        }
    }
}
