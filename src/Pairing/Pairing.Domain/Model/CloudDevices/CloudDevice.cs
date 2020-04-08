using Pairing.Domain.Events;
using Pairing.Domain.Exceptions;
using Pairing.Domain.Model.Shared;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Model.CloudDevices
{
    public class CloudDevice : Entity, IAggregateRoot
    {
        public string UserId { get; private set; }

        public CloudDeviceStatus CloudDeviceStatus { get; private set; }
        private int _cloudDeviceStatusId;

        public string Name { get; private set; }
        public string Number { get; private set; }
        public string LoginUsername { get; private set; }
        public string LoginPassword { get; private set; }
        public string ApiKey { get; private set; }
        public DateTime DateCreated { get; private set; }

        public CloudDeviceStatus GetCloudDeviceStatus => CloudDeviceStatus.From(this._cloudDeviceStatusId);


        protected CloudDevice()
        {
        }

        public CloudDevice(string userId, string name, string number, string loginUsername, string loginPassword, string apiKey, DateTime dateCreated)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Number = number ?? throw new ArgumentNullException(nameof(number));
            LoginUsername = loginUsername ?? throw new ArgumentNullException(nameof(loginUsername));
            LoginPassword = loginPassword ?? throw new ArgumentNullException(nameof(loginPassword));
            ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            DateCreated = dateCreated;

            _cloudDeviceStatusId = CloudDeviceStatus.Offline.Id;

            this.AddDomainEvent(new CloudDeviceCreatedDomainEvent(
                this,
                userId,
                name,
                number,
                loginUsername,
                loginPassword,
                apiKey,
                dateCreated
                ));
        }

        public static CloudDevice From(string userId, string name, string cloudDeviceNumber, string cloudDeviceUsername, string cloudDevicePassword, IDateTimeService dateTimeService)
        {
            //Checking the user id is not null.
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new PairingDomainException("Invalid user id." + ". At CloudDevice.From()");

            }
            //Checking the name is not null.
            if (string.IsNullOrWhiteSpace(name) || name.Length > 50)
            {
                throw new PairingDomainException("无效的命名，需小于25字" + ". At CloudDevice.From()");
            }

            //Validate the device username, password and device number.
            if (string.IsNullOrWhiteSpace(cloudDeviceUsername) || cloudDeviceUsername.Length > 50)
            {
                throw new PairingDomainException("无效的云手机账号，需小于25字" + ". At CloudDevice.From()");
            }
            if (string.IsNullOrWhiteSpace(cloudDevicePassword) || cloudDevicePassword.Length > 50)
            {
                throw new PairingDomainException("无效的云手机密码，需小于50个英数" + ". At CloudDevice.From()");
            }
            if (string.IsNullOrWhiteSpace(cloudDeviceNumber) || cloudDeviceNumber.Length > 50)
            {
                throw new PairingDomainException("无效的云手机编号，需小于50个英数" + ". At CloudDevice.From()");
            }


            var cloudDevice = new CloudDevice(
                userId,
                name,
                cloudDeviceNumber,
                cloudDeviceUsername,
                cloudDevicePassword,
                GenerateApiKey(),
                dateTimeService.GetCurrentDateTime()
                );

            return cloudDevice;
        }

        public void ResetApiKey()
        {
            this.ApiKey = GenerateApiKey();
        }

        public void Online()
        {
            this._cloudDeviceStatusId = CloudDeviceStatus.Online.Id;

            this.AddDomainEvent(new CloudDeviceOnlineDomainEvent(
                this,
                _cloudDeviceStatusId
                ));
        }

        public void Offline()
        {
            this._cloudDeviceStatusId = CloudDeviceStatus.Offline.Id;

            this.AddDomainEvent(new CloudDeviceOfflineDomainEvent(
                this,
                _cloudDeviceStatusId
                ));
        }


        private static string GenerateApiKey()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
