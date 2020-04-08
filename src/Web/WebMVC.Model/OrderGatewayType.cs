using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Models
{
    public enum OrderGatewayType
    {
        WechatBarcode= 1,
        WechatMerchant = 2,
        AlipayBarcode= 3,
        AlipayMerchant = 4,
        AlipayTransaction = 5,
        AlipayBank = 6,
        AlipayEnvelop = 7,
        AlipayEnvelopPassword = 8
    }
}
