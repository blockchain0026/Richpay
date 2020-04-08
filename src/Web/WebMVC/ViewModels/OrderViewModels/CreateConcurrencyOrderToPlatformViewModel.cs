using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;

namespace WebMVC.ViewModels.OrderViewModels
{
    public class CreateConcurrencyOrderToPlatformViewModel
    {
        public string ShopId { get; set; }
        public string ShopUserName { get; set; }
        public string ShopOrderId { get; set; }
        public int ShopOrderAmount { get; set; }
        public OrderGatewayType OrderGatewayType { get; set; }
    }
}
