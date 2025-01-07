using GR30323_Web.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR30323_Web.Domain.Models
{
    public class CartItem
    {
        public Car Car { get; set; }
        public int Quantity { get; set; }
    }
}
