using MediatR;
using Pairing.Domain.Model.CloudDevices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class CloudDeviceCreatedDomainEvent : INotification
    {
        public CloudDeviceCreatedDomainEvent(CloudDevice cloudDevice, string userId, string name, string number, string loginUsername, string loginPassword, string apiKey, DateTime dateCreated)
        {
            CloudDevice = cloudDevice;
            UserId = userId;
            Name = name;
            Number = number;
            LoginUsername = loginUsername;
            LoginPassword = loginPassword;
            ApiKey = apiKey;
            DateCreated = dateCreated;
        }

        public CloudDevice CloudDevice { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string LoginUsername { get; set; }
        public string LoginPassword { get; set; }
        public string ApiKey { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
