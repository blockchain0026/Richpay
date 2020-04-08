using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Models.Queries
{
    public class IpWhitelistEntry
    {
        public int IpWhitelistId { get; set; }
        public string Address { get; set; }
    }
}
