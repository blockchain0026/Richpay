using Microsoft.EntityFrameworkCore;
using Pairing.Domain.Model.CloudDevices;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pairing.Infrastructure.Repositories
{
    public class CloudDeviceRepository
     : ICloudDeviceRepository
    {
        private readonly PairingContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public CloudDeviceRepository(PairingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public CloudDevice Add(CloudDevice cloudDevice)
        {
            return _context.CloudDevices.Add(cloudDevice).Entity;

        }

        public void Update(CloudDevice cloudDevice)
        {
            _context.Entry(cloudDevice).State = EntityState.Modified;
        }

        public async Task<CloudDevice> GetByCloudDeviceIdAsync(int cloudDeviceId)
        {
            var cloudDevice = await _context
                                .CloudDevices
                                .FirstOrDefaultAsync(b => b.Id == cloudDeviceId);
            if (cloudDevice == null)
            {
                cloudDevice = _context
                            .CloudDevices
                            .Local
                            .FirstOrDefault(b => b.Id == cloudDeviceId);
            }
            if (cloudDevice != null)
            {
                await _context.Entry(cloudDevice)
                    .Reference(b => b.CloudDeviceStatus).LoadAsync();
            }

            return cloudDevice;
        }

        public async Task<CloudDevice> GetByUserIdAsync(string userId)
        {
            var cloudDevice = await _context
                    .CloudDevices
                    .FirstOrDefaultAsync(b => b.UserId == userId);

            if (cloudDevice == null)
            {
                cloudDevice = _context
                            .CloudDevices
                            .Local
                            .FirstOrDefault(b => b.UserId == userId);
            }
            if (cloudDevice != null)
            {
                await _context.Entry(cloudDevice)
                    .Reference(b => b.CloudDeviceStatus).LoadAsync();
            }

            return cloudDevice;
        }

        public void Delete(CloudDevice cloudDevice)
        {
            if (cloudDevice != null)
            {
                _context.CloudDevices.Remove(cloudDevice);
            }
        }
    }
}
