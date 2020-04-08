using Pairing.Domain.Exceptions;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Model.QrCodes
{
    public class MerchantData : ValueObject
    {
        public MerchantData(string appId, string alipayPublicKey, string wechatApiCertificate, string privateKey, string merchantId)
        {
            AppId = appId ?? throw new ArgumentNullException(nameof(appId));
            AlipayPublicKey = alipayPublicKey;
            WechatApiCertificate = wechatApiCertificate;
            PrivateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
            MerchantId = merchantId ?? throw new ArgumentNullException(nameof(merchantId));
        }

        public string AppId { get; private set; }
        public string AlipayPublicKey { get; private set; }
        public string WechatApiCertificate { get; private set; }
        public string PrivateKey { get; private set; }
        public string MerchantId { get; private set; }



        public static MerchantData FromAlipay(string appId, string alipayPublicKey, string privateKey, string merchantId)
        {
            //Checking the all param is valid.
            if (string.IsNullOrWhiteSpace(appId))
            {
                throw new PairingDomainException("无效的应用Id" + ". At MerchantData.FromAlipay()");
            }
            if (string.IsNullOrWhiteSpace(alipayPublicKey))
            {
                throw new PairingDomainException("无效的支付宝公钥" + ". At MerchantData.FromAlipay()");
            }
            if (string.IsNullOrWhiteSpace(privateKey))
            {
                throw new PairingDomainException("无效的私钥" + ". At MerchantData.FromAlipay()");
            }
            if (string.IsNullOrWhiteSpace(merchantId))
            {
                throw new PairingDomainException("无效的商户编号" + ". At MerchantData.FromAlipay()");
            }

            return new MerchantData(appId, alipayPublicKey, null, privateKey, merchantId);
        }
       
        public static MerchantData FromWechat(string appId, string wechatApiCertificate, string privateKey, string merchantId)
        {
            //Checking the all param is valid.
            if (string.IsNullOrWhiteSpace(appId))
            {
                throw new PairingDomainException("无效的应用Id" + ". At MerchantData.FromWechat()");
            }
            if (string.IsNullOrWhiteSpace(wechatApiCertificate))
            {
                throw new PairingDomainException("无效的Api证书" + ". At MerchantData.FromWechat()");
            }
            if (string.IsNullOrWhiteSpace(privateKey))
            {
                throw new PairingDomainException("无效的私钥" + ". At MerchantData.FromWechat()");
            }
            if (string.IsNullOrWhiteSpace(merchantId))
            {
                throw new PairingDomainException("无效的商户编号" + ". At MerchantData.FromWechat()");
            }

            return new MerchantData(appId, null, wechatApiCertificate, privateKey, merchantId);
        }


        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return AppId;
            yield return AlipayPublicKey;
            yield return WechatApiCertificate;
            yield return PrivateKey;
            yield return MerchantId;
        }
    }
}
