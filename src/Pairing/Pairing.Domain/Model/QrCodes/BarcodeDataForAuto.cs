using Pairing.Domain.Exceptions;
using Pairing.Domain.Model.CloudDevices;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Model.QrCodes
{
    public class BarcodeDataForAuto : ValueObject
    {
        public BarcodeDataForAuto(string cloudDeviceUsername, string cloudDevicePassword, string cloudDeviceNumber)
        {
            this.CloudDeviceUsername = cloudDeviceUsername;
            this.CloudDevicePassword = cloudDevicePassword;
            this.CloudDeviceNumber = cloudDeviceNumber;
        }

        public string CloudDeviceUsername { get; private set; }
        public string CloudDevicePassword { get; private set; }
        public string CloudDeviceNumber { get; private set; }

        public static BarcodeDataForAuto From(CloudDevice cloudDevice)
        {
            if (cloudDevice is null)
            {
                throw new PairingDomainException("The cloud device must be provided" + ". At BarcodeDataForAuto.From()");
            }
            var cloudDeviceUsername = cloudDevice.LoginUsername;
            var cloudDevicePassword = cloudDevice.LoginPassword;
            var cloudDeviceNumber = cloudDevice.Number;

            if (string.IsNullOrWhiteSpace(cloudDeviceUsername) || cloudDeviceUsername.Length > 50)
            {
                throw new PairingDomainException("无效的云手机账号，需小于25字" + ". At BarcodeDataForAuto()");
            }
            if (string.IsNullOrWhiteSpace(cloudDevicePassword) || cloudDevicePassword.Length > 50)
            {
                throw new PairingDomainException("无效的云手机密码，需小于50个英数" + ". At BarcodeDataForAuto()");
            }
            if (string.IsNullOrWhiteSpace(cloudDeviceNumber) || cloudDeviceNumber.Length > 50)
            {
                throw new PairingDomainException("无效的云手机编号，需小于50个英数" + ". At BarcodeDataForAuto()");
            }

            return new BarcodeDataForAuto(cloudDeviceUsername, cloudDevicePassword, cloudDeviceNumber);
        }


        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return CloudDeviceUsername;
            yield return CloudDevicePassword;
            yield return CloudDeviceNumber;
        }
    }
}
