using Ordering.Domain.Exceptions;
using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.Model.Orders
{
    public class ShopInfo : ValueObject
    {
        public ShopInfo(string shopId, string shopOrderId, string shopReturnUrl, string shopOkReturnUrl,
            decimal rateRebateShop, decimal rateRebateFinal)
        {
            if (string.IsNullOrEmpty(shopId))
            {
                throw new OrderingDomainException("无效的商户编号" + ". At ShopInfo.From()");
            }

            if (!IsUrlValid(shopReturnUrl))
            {
                throw new OrderingDomainException("无效的商户回调链结" + ". At ShopInfo.From()");
            }

            if (!IsUrlValid(shopOkReturnUrl))
            {
                throw new OrderingDomainException("无效的商户回调链结" + ". At ShopInfo.From()");
            }

            //Check the rebate rate of shop is in greater than 0 and less than 1, and has less than 3 points.
            if (decimal.Round(rateRebateShop, 3) != rateRebateShop || rateRebateShop <= 0 || rateRebateShop >= 1)
            {
                throw new OrderingDomainException("无效的商户返佣率" + ". At ShopInfo.From()");
            }

            //Check the rebate rate of shop is in greater than 0 and less than 1, and has less than 3 points.
            if (decimal.Round(rateRebateFinal, 3) != rateRebateFinal || rateRebateFinal <= 0 || rateRebateFinal >= 1)
            {
                throw new OrderingDomainException("无效的商户最终返佣率" + ". At ShopInfo.From()");
            }
            ShopId = shopId;
            ShopOrderId = shopOrderId;
            ShopReturnUrl = shopReturnUrl;
            ShopOkReturnUrl = shopOkReturnUrl;
            RateRebateShop = rateRebateShop;
            RateRebateFinal = rateRebateFinal;
        }

        public string ShopId { get; private set; }
        public string ShopOrderId { get; private set; }
        public string ShopReturnUrl { get; private set; }
        public string ShopOkReturnUrl { get; private set; }
        public decimal RateRebateShop { get; private set; }
        public decimal RateRebateFinal { get; private set; }


        private bool IsUrlValid(string url)
        {
            //Checking the qrcode url is a valid http/https url.
            var isUrlValid = Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return isUrlValid;
        }


        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return ShopId;
            yield return ShopOrderId;
            yield return ShopReturnUrl;
            yield return ShopOkReturnUrl;
            yield return RateRebateShop;
            yield return RateRebateFinal;
        }
    }
}
