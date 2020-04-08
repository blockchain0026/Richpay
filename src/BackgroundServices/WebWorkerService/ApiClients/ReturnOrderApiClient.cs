using Newtonsoft.Json;
using Ordering.Domain.Model.Orders;
using Ordering.Domain.Model.ShopApis;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebWorkerService.Extensions;
using WebWorkerService.Infrastructure.Handlers;

namespace WebWorkerService.ApiClients
{
    public class ReturnOrderApiClient : IReturnOrderApiClient
    {
        private readonly IShopApiRepository _shopApiRepository;

        public ReturnOrderApiClient(IShopApiRepository shopApiRepository)
        {
            _shopApiRepository = shopApiRepository ?? throw new ArgumentNullException(nameof(shopApiRepository));
        }


        public async Task ReturnOrderResult(string url, string orderTrackingNumber, string shopOrderId, string shopId, string dateCreated, string dateConfirmed, int orderAmount, int orderStatus)
        {
            var client = new HttpClient(new LoggingHandler(new HttpClientHandler()))
            {
                BaseAddress = new Uri(url)
            };

            string tdstud = string.Empty;

            if (orderStatus == OrderStatus.Submitted.Id)
            {
                tdstud = "1";
            }
            else if (orderStatus == OrderStatus.AwaitingPayment.Id)
            {
                tdstud = "0";
            }
            else if (orderStatus == OrderStatus.Success.Id)
            {
                tdstud = "2";
            }
            else
            {
                tdstud = "1";
            }


            var md5Hash = await this.GetMd5Hash(
                orderTrackingNumber,
                shopOrderId,
                shopId,
                dateCreated,
                dateConfirmed,
                orderAmount.ToString(),
                tdstud
                );

            var data = JsonConvert.SerializeObject(new
            {
                id = orderTrackingNumber,
                lsh = shopOrderId,
                user = shopId,
                optime = dateCreated,
                paytime = dateConfirmed,
                money = orderAmount.ToString(),
                tdstud = tdstud,
                ch = md5Hash
            }, Newtonsoft.Json.Formatting.None);

            var body = new StringContent(data, Encoding.UTF8, "application/json");
            Console.WriteLine("Return order result to new pay. Body:" + body);

            //Fire and forget.
            PostWithoutWait(client, body).Forget();
        }

        private async Task<string> GetMd5Hash(string id, string lsh,
            string user, string optime, string paytime, string money, string tdstud)
        {
            var apiKey = await _shopApiRepository.GetApiKeyByShopIdAsync(user);
            using (var cryptoMD5 = System.Security.Cryptography.MD5.Create())
            {
                var str = id
                    + lsh
                    + user
                    + optime
                    + paytime
                    + money
                    + tdstud;


                //將字串編碼成 UTF8 位元組陣列
                var bytes = Encoding.UTF8.GetBytes(str + apiKey);

                //取得雜湊值位元組陣列
                var hash = cryptoMD5.ComputeHash(bytes);

                //取得 MD5
                var ch = BitConverter.ToString(hash)
                  .Replace("-", String.Empty)
                  .ToLower();

                return ch;
            }
        }

        private async Task PostWithoutWait(HttpClient client, StringContent body)
        {
            try
            {
                var response = await client.PostAsync(string.Empty, body);
                Console.WriteLine("Return order result to new pay. Result:" + response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Return order result to new pay. Result:" + ex.Message);
            }
        }
    }
}
