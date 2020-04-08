using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Applications.Queries.ShopSettings
{
    public interface IShopSettingsQueries
    {
        Task<List<decimal>> GetOrderAmountOptionsByShopIdAsync(string shopId);
    }
}
