using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels.Json
{
    public class KTQuery
    {
        public string generalSearch { get; set; }
        public string downlineSearch { get; set; }
        public string downlineTraderSearch { get; set; }
        public string downlineShopSearch { get; set; }

        public string userId { get; set; }

        #region Withdrawals
        public string userType { get; set; }
        public string status { get; set; }
        #endregion

        #region Qr Codes

        public string qrCodeType { get; set; }
        public string paymentChannel { get; set; }
        public string paymentScheme { get; set; }
        public string qrCodeStatus { get; set; }
        public string pairingStatus { get; set; }

        #endregion

        #region Shop Gateway
        public string shopGatewayType { get; set; }
        #endregion


        #region Order
        public string DateFrom { get; set; }
        public string DateTo { get; set; }

        public string OrderType { get; set; }
        public string OrderStatus { get; set; }
        public string OrderPaymentChannel { get; set; }
        public string OrderPaymentScheme { get; set; }
        #endregion
    }
}
