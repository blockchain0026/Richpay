using MediatR;
using Pairing.Domain.Model.CloudDevices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class CloudDeviceOfflineDomainEvent : INotification
    {
        public CloudDeviceOfflineDomainEvent(CloudDevice cloudDevice, int cloudDeviceStatusId)
        {
            CloudDevice = cloudDevice;
            CloudDeviceStatusId = cloudDeviceStatusId;
        }

        public CloudDevice CloudDevice { get; set; }
        public int CloudDeviceStatusId { get; set; }
    }
}
