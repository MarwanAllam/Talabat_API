using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Entities.Order_Aggregate
{
    public class DeliveryMethod: BaseEntity
    {
       
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string DeliveryTime { get; set; }
        public decimal Cost { get; set; }

        public DeliveryMethod()
        {

        }
        public DeliveryMethod(string shortName, string descrption, string deliveryTime, decimal cost)
        {
            ShortName = shortName;
            Description = descrption;
            DeliveryTime = deliveryTime;
            Cost = cost;
        }

    }
}
