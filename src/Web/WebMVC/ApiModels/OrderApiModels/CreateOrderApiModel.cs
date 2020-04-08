using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ApiModels.OrderApiModels
{
    public class CreateOrderApiModel
    {
        public string lsh { get; set; }
        public int money { get; set; }
        public string user { get; set; }
        public string time { get; set; }
        public string type { get; set; }
        public string reurl { get; set; }
        public string okreurl { get; set; }
        public string ch { get; set; }
    }
}
