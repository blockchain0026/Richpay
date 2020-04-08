using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Model.Orders;
using Ordering.Domain.Model.ShopApis;
using Ordering.Infrastructure;
using WebMVC.ApiModels.OrderApiModels;
using WebMVC.Infrastructure.Services;
using WebMVC.Models;

namespace WebMVC.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderApiController : ControllerBase
    {
        private readonly OrderingContext _context;
        private readonly IApiService _apiService;
        private readonly IShopApiRepository _shopApiRepository;

        public OrderApiController(OrderingContext context, IApiService apiService, IShopApiRepository shopApiRepository)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _shopApiRepository = shopApiRepository ?? throw new ArgumentNullException(nameof(shopApiRepository));
        }

        // POST: api/order
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder([FromBody]CreateOrderApiModel data)
        {
            try
            {
                //Authentication
                var apiKey = await _shopApiRepository.GetApiKeyByShopIdAsync(data.user);
                if (string.IsNullOrEmpty(apiKey))
                {
                    return BadRequest(new { status = "1", msg = "Api Key 验证失败。" });
                }
                using (var cryptoMD5 = System.Security.Cryptography.MD5.Create())
                {
                    var str = data.lsh
                        + data.money
                        + data.user
                        + data.time
                        + data.type
                        + data.reurl
                        + data.okreurl
                        + apiKey;

                    //將字串編碼成 UTF8 位元組陣列
                    var bytes = Encoding.UTF8.GetBytes(str);

                    //取得雜湊值位元組陣列
                    var hash = cryptoMD5.ComputeHash(bytes);

                    //取得 MD5
                    var md5 = BitConverter.ToString(hash)
                      .Replace("-", String.Empty);

                    if (!md5.Equals(data.ch, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return BadRequest(new { status = "1", msg = "Api Key 验证失败。" });
                    }
                }

                /*var orderId = await _apiService.CreateOrderAsync(
                    data.user,
                    data.lsh,
                    data.money,
                    data.reurl,
                    data.okreurl,
                    (OrderGatewayType)int.Parse(data.type)
                    );*/
                /*var orderTrackingNumber = await _apiService.CreateOrderAsync(
                                    data.user,
                                    data.lsh,
                                    data.money,
                                    data.reurl,
                                    data.okreurl,
                                    (OrderGatewayType)int.Parse(data.type)
                                    );*/
                var orderTrackingNumber = await _apiService.CreateOrderAsync(
                                    data.user,
                                    data.lsh,
                                    data.money,
                                    data.reurl,
                                    data.okreurl,
                                    (OrderGatewayType)int.Parse(data.type)
                                    );


                return Ok(new { status = "0", url = "https://www.richpay168.com/pay?orderTrackingNumber=" + orderTrackingNumber });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "1", msg = ex.Message });
            }
        }

        // POST: api/order/status
        [HttpPost("status")]
        public async Task<ActionResult> PostOrderStatus([FromBody]GetOrderApiModel data)
        {
            try
            {
                //Authentication
                var apiKey = await _shopApiRepository.GetApiKeyByShopIdAsync(data.user);
                if (string.IsNullOrEmpty(apiKey))
                {
                    return BadRequest(new { status = "1", msg = "Api Key 验证失败。" });
                }
                using (var cryptoMD5 = System.Security.Cryptography.MD5.Create())
                {
                    var str = data.lsh
                        + data.time
                        + data.user
                        + apiKey;

                    //將字串編碼成 UTF8 位元組陣列
                    var bytes = Encoding.UTF8.GetBytes(str);

                    //取得雜湊值位元組陣列
                    var hash = cryptoMD5.ComputeHash(bytes);

                    //取得 MD5
                    var md5 = BitConverter.ToString(hash)
                      .Replace("-", String.Empty);

                    if (!md5.Equals(data.ch, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return BadRequest(new { status = "1", msg = "Api Key 验证失败。" });
                    }
                }


                var order = await _context.Orders
                    .Include(o => o.OrderStatus)
                    .Where(o => o.ShopInfo.ShopId == data.user && o.ShopInfo.ShopOrderId == data.lsh)
                    .Select(o => new
                    {
                        o.IsExpired,
                        o.OrderStatus,
                        o.Amount
                    })
                    .FirstOrDefaultAsync();

                if (order is null)
                {
                    return Ok(new { status = "1", msg = "查无此订单" });
                }
                else
                {
                    var tdstud = 0;
                    if (order.OrderStatus.Id == OrderStatus.Success.Id)
                    {
                        tdstud = 2;
                    }
                    else if (order.IsExpired)
                    {
                        tdstud = 1;
                    }

                    return Ok(new { status = "0", tdstud = tdstud.ToString(), money = order.Amount.ToString() });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "1", msg = ex.Message });
            }
        }

        // POST: api/order/autoconfirm
        [HttpPost("autoconfirm")]
        public async Task<ActionResult> PostOrderAutoConfirm([FromBody]CreateOrderApiModel data)
        {
            try
            {
                //Authentication
                var apiKey = await _shopApiRepository.GetApiKeyByShopIdAsync(data.user);
                if (string.IsNullOrEmpty(apiKey))
                {
                    return BadRequest(new { status = "1", msg = "Api Key 验证失败。" });
                }
                using (var cryptoMD5 = System.Security.Cryptography.MD5.Create())
                {
                    var str = data.lsh
                        + data.money
                        + data.user
                        + data.time
                        + data.type
                        + data.reurl
                        + data.okreurl
                        + apiKey;

                    //將字串編碼成 UTF8 位元組陣列
                    var bytes = Encoding.UTF8.GetBytes(str);

                    //取得雜湊值位元組陣列
                    var hash = cryptoMD5.ComputeHash(bytes);

                    //取得 MD5
                    var md5 = BitConverter.ToString(hash)
                      .Replace("-", String.Empty);

                    if (!md5.Equals(data.ch, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return BadRequest(new { status = "1", msg = "Api Key 验证失败。" });
                    }
                }

                /*var orderId = await _apiService.CreateOrderAsync(
                    data.user,
                    data.lsh,
                    data.money,
                    data.reurl,
                    data.okreurl,
                    (OrderGatewayType)int.Parse(data.type)
                    );*/
                /*var orderTrackingNumber = await _apiService.CreateOrderAsync(
                                    data.user,
                                    data.lsh,
                                    data.money,
                                    data.reurl,
                                    data.okreurl,
                                    (OrderGatewayType)int.Parse(data.type)
                                    );*/
                var orderTrackingNumber = await _apiService.CreateOrderAndConfirmAsync(
                                    data.user,
                                    data.lsh,
                                    data.money,
                                    data.reurl,
                                    data.okreurl,
                                    (OrderGatewayType)int.Parse(data.type)
                                    );


                return Ok(new { status = "0", url = "https://www.richpay168.com/pay?orderTrackingNumber=" + orderTrackingNumber });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "1", msg = ex.Message });
            }
        }
    }
}
