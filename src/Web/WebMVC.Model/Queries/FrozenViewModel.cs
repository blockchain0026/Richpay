using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Models.Queries
{
    public class FrozenRecord
    {
        public int FrozenId { get; set; }
        public string FrozenStatus { get; set; }
        public string FrozenType { get; set; }

        public decimal Amount { get; set; }

        public string OrderTrackingNumber { get; set; }
        public int? WithdrawalId { get; set; }
        public string ByAdminId { get; set; }
        public string ByAdminName { get; set; }
        public string Description { get; set; }

        public string DateFroze { get; set; }
        public string DateUnfroze { get; set; }
    }
}
