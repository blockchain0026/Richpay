using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;

namespace WebMVC.Infrastructure.Services
{
    public interface IApiService
    {
        Task<string> GenerateShopApiKey(string shopId, string generateByUserId);
        Task<string> CreateOrderAsync(string shopId, string shopOrderId, decimal shopOrderAmount, string shopReturnUrl, string shopOkReturnUrl,
            OrderGatewayType orderGatewayType);
        Task<string> CreateOrderAndConfirmAsync(string shopId, string shopOrderId, decimal shopOrderAmount, string shopReturnUrl, string shopOkReturnUrl,
            OrderGatewayType orderGatewayType);
        Task ConfirmOrder(int orderId);
        Task AddIpToWhitelist(string shopId, List<string> ips, string addByUserId);
    }
}
