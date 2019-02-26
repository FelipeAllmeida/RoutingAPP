﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Models
{
    public class ClientModel : BaseRoutingObject
    {
        public Position DeliverPosition;
        public DemmandModel Cargo { get; set; }
    }
}
