using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pairing.Domain.Model.CloudDevices
{
    public interface ICloudDeviceRepository : IRepository<CloudDevice>
    {
        CloudDevice Add(CloudDevice cloudDevice);
        void Update(CloudDevice cloudDevice);
        void Delete(CloudDevice cloudDevice);

        Task<CloudDevice> GetByCloudDeviceIdAsync(int cloudDeviceId);
        Task<CloudDevice> GetByUserIdAsync(string userId);
    }
}
