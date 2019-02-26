using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Models
{
    public class DemmandModel
    {
        public float Weight { get { return Volume * 1000f; } }
        public float Volume { get; set; }
    }
}
