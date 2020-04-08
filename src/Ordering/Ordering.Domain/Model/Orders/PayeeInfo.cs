using Ordering.Domain.Exceptions;
using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.Model.Orders
{
    public class PayeeInfo : ValueObject
    {
        public string TraderId { get; private set; }
        public int? QrCodeId { get; private set; }
        public string FourthPartyId { get; private set; }
        public string FourthPartyName { get; private set; }
        public string FourthPartyOrderPaymentUrl { get; private set; }
        public string FourthPartyOrderNumber { get; private set; }
        public decimal? ToppestTradingRate { get; private set; }

        public PayeeInfo(string traderId, int? qrCodeId, string fourthPartyId, string fourthPartyName, decimal? toppestTradingRate)
        {
            TraderId = traderId;
            QrCodeId = qrCodeId;
            FourthPartyId = fourthPartyId;
            FourthPartyName = fourthPartyName;
            ToppestTradingRate = toppestTradingRate;
        }

        public static PayeeInfo ToPlatform(string traderId, int qrCodeId, decimal toppestTradingRate)
        {
            //Let the database do the rate checking.


            return new PayeeInfo(
                traderId, qrCodeId, null, null, toppestTradingRate);
        }

        public static PayeeInfo ToFourthParty(string fourthPartyId, string fourthPartyName)
        {
            return new PayeeInfo(
                null,
                null,
                fourthPartyId,
                fourthPartyName,
                null);
        }

        public PayeeInfo Paired(string fourthPartyOrderPaymentUrl, string fourthPartyOrderNumber)
        {
            if (!IsUrlValid(fourthPartyOrderPaymentUrl))
            {
                throw new OrderingDomainException("无效的支付页面链结" + ". At PayeeInfo.ToFourthParty()");
            }
            if (string.IsNullOrEmpty(fourthPartyOrderNumber))
            {
                throw new OrderingDomainException("无效的四方订单编号" + ". At PayeeInfo.ToFourthParty()");
            }


            this.FourthPartyOrderPaymentUrl = fourthPartyOrderPaymentUrl;
            this.FourthPartyOrderNumber = fourthPartyOrderNumber;

            return this;
        }

        private bool IsUrlValid(string url)
        {
            //Checking the qrcode url is a valid http/https url.
            var isUrlValid = Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return isUrlValid;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return TraderId;
            yield return QrCodeId;
            yield return FourthPartyId;
            yield return FourthPartyName;
            yield return FourthPartyOrderPaymentUrl;
            yield return FourthPartyOrderNumber;
        }
    }
}
