using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp4
{
    public class OrderInfo
    {
        public OrderInfo(int orderId, double weight, int cityDistrictId, DateTime deliveryTime)
        {
            OrderId = orderId;
            Weight = weight;
            CityDistrictId = cityDistrictId;
            DeliveryTime = deliveryTime;
        }
        public readonly int OrderId;
        public readonly double Weight;
        public readonly int CityDistrictId;
        public readonly DateTime DeliveryTime;

    }
}
