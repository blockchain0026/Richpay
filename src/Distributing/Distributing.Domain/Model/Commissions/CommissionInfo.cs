using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Model.Commissions
{
    public class CommissionInfo
    {
        public int Id { get; set; }
        public int BalanceId { get; set; }
        public bool IsEnabled { get; set; }
        public int? UplineCommissionId { get; set; }
        public decimal Rate { get; set; }
        public string UserId { get; set; }
        public int ChainNumber { get; set; }
    }
}
