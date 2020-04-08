using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ApiModels.OrderApiModels
{
    public class GetOrderApiModel
    {
        public string lsh { get; set; }
        public string time { get; set; }
        public string user { get; set; }
        public string ch { get; set; }
    }
}
