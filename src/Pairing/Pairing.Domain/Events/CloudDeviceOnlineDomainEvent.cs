using MediatR;
using Pairing.Domain.Model.CloudDevices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Events
{
    public class CloudDeviceOnlineDomainEvent : INotification
    {
        public CloudDeviceOnlineDomainEvent(CloudDevice cloudDevice, int cloudDeviceStatusId)
        {
            CloudDevice = cloudDevice;
            CloudDeviceStatusId = cloudDeviceStatusId;
        }

        public CloudDevice CloudDevice { get; set; }
        public int CloudDeviceStatusId { get; set; }
    }
}
