using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Infrastructure.Services
{
    public interface IBalanceService
    {
        Task ChangeBalance(string type, string userId, decimal amount, string description, string createByUserId);
    }
}
