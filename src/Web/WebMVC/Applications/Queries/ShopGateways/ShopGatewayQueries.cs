using Microsoft.EntityFrameworkCore;
using Pairing.Domain.Model.ShopGateways;
using Pairing.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Data;
using WebMVC.Extensions;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.ShopGateways
{
    public class ShopGatewayQueries : IShopGatewayQueries
    {
        private readonly PairingContext _pairingContext;
        private readonly ApplicationDbContext _applicationContext;

        public ShopGatewayQueries(PairingContext pairingContext, ApplicationDbContext applicationContext)
        {
            _pairingContext = pairingContext ?? throw new ArgumentNullException(nameof(pairingContext));
            _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
        }



        public async Task<ShopGatewayEntry> GetShopGatewayEntryAsync(int shopGateWayId)
        {
            return await _applicationContext.ShopGatewayEntrys.Where(w => w.ShopGatewayId == shopGateWayId).FirstOrDefaultAsync();
        }


        public async Task<ShopGatewayEntry> GetMatchedShopGatewayAsync(string shopId, string paymentChannel, string paymentScheme)
        {
            return await _applicationContext.ShopGatewayEntrys
                .Where(w => w.ShopId == shopId
                && w.PaymentChannel == paymentChannel
                && w.PaymentScheme == paymentScheme
                )
                .FirstOrDefaultAsync();
        }




        public async Task<List<ShopGatewayEntry>> GetShopGatewayEntrysAsync(int? pageIndex, int? take, string searchString = null, string sortField = null,
            string shopGatewayType = null, string paymentChannel = null, string paymentScheme = null,
            string direction = SortDirections.Descending)
        {
            var result = new List<ShopGatewayEntry>();

            IQueryable<ShopGatewayEntry> shopGatewayEntrys = null;

            shopGatewayEntrys = _applicationContext.ShopGatewayEntrys
                .Include(w => w.AlipayPreferenceInfo)
                .Where(s => s.ShopGatewayType.Contains(shopGatewayType ?? string.Empty)
                && s.PaymentChannel.Contains(paymentChannel ?? string.Empty)
                && s.PaymentScheme.Contains(paymentScheme ?? string.Empty)
                );

            IQueryable<ShopGatewayEntry> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = shopGatewayEntrys
                    .Where(w => w.ShopGatewayId.ToString() == searchString
                    || w.FourthPartyGatewayId.ToString() == searchString
                    || w.ShopId == searchString


                    );
            }
            else
            {
                searchResult = shopGatewayEntrys;
            }

            result = await this.GetSortedRecords(
                searchResult,
                pageIndex,
                take,
                sortField,
                direction
                );

            return result;
        }

        public async Task<int> GetShopGatewayEntrysTotalCount(string searchString = null,
            string shopGatewayType = null, string paymentChannel = null, string paymentScheme = null)
        {
            IQueryable<ShopGatewayEntry> shopGatewayEntrys = null;

            shopGatewayEntrys = _applicationContext.ShopGatewayEntrys
                .Where(s => s.ShopGatewayType.Contains(shopGatewayType ?? string.Empty)
                && s.PaymentChannel.Contains(paymentChannel ?? string.Empty)
                && s.PaymentScheme.Contains(paymentScheme ?? string.Empty)
                );


            if (!string.IsNullOrEmpty(searchString))
            {
                return await shopGatewayEntrys
                    .Include(w => w.AlipayPreferenceInfo)
                    .Where(w => w.ShopGatewayId.ToString() == searchString
                    || w.FourthPartyGatewayId.ToString() == searchString
                    || w.ShopId == searchString


                    ).CountAsync();
            }
            else
            {
                return await shopGatewayEntrys.CountAsync();
            }
        }


        public async Task<List<ShopGatewayEntry>> GetShopGatewayEntrysByShopIdAsync(
            string shopId, int? pageIndex, int? take, string searchString = null, string sortField = null,
            string shopGatewayType = null, string paymentChannel = null, string paymentScheme = null,
            string direction = SortDirections.Descending)
        {
            var result = new List<ShopGatewayEntry>();

            IQueryable<ShopGatewayEntry> shopGatewayEntrys = null;

            shopGatewayEntrys = _applicationContext.ShopGatewayEntrys
                .Include(w => w.AlipayPreferenceInfo)
                .Where(s => s.ShopId == shopId
                && s.ShopGatewayType.Contains(shopGatewayType ?? string.Empty)
                && s.PaymentChannel.Contains(paymentChannel ?? string.Empty)
                && s.PaymentScheme.Contains(paymentScheme ?? string.Empty)
                );

            IQueryable<ShopGatewayEntry> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = shopGatewayEntrys
                    .Where(w => w.ShopGatewayId.ToString() == searchString
                    || w.FourthPartyGatewayId.ToString() == searchString
                    || w.ShopId == searchString


                    );
            }
            else
            {
                searchResult = shopGatewayEntrys;
            }

            result = await this.GetSortedRecords(
                searchResult,
                pageIndex,
                take,
                sortField,
                direction
                );

            return result;
        }

        public async Task<int> GetShopGatewayEntrysTotalCountByShopIdAsync(string shopId, string searchString = null,
            string shopGatewayType = null, string paymentChannel = null, string paymentScheme = null)
        {
            IQueryable<ShopGatewayEntry> shopGatewayEntrys = null;

            shopGatewayEntrys = _applicationContext.ShopGatewayEntrys
                .Include(w => w.AlipayPreferenceInfo)
                .Where(s => s.ShopId == shopId
                && s.ShopGatewayType.Contains(shopGatewayType ?? string.Empty)
                && s.PaymentChannel.Contains(paymentChannel ?? string.Empty)
                && s.PaymentScheme.Contains(paymentScheme ?? string.Empty)
                );


            if (!string.IsNullOrEmpty(searchString))
            {
                return await shopGatewayEntrys
                    .Where(w => w.ShopGatewayId.ToString() == searchString
                    || w.FourthPartyGatewayId.ToString() == searchString
                    || w.ShopId == searchString


                    ).CountAsync();
            }
            else
            {
                return await shopGatewayEntrys.CountAsync();
            }
        }


        public ShopGatewayEntry MapFromEntity(ShopGateway entity)
        {
            //Check the entity is not null.
            if (entity is null)
            {
                throw new ArgumentNullException("The qr code entity must be provided.");
            }

            AlipayPreferenceInfo alipayPreferenceInfo = null;
            if (entity.AlipayPreference != null)
            {
                alipayPreferenceInfo = new AlipayPreferenceInfo
                {
                    SecondsBeforePayment = entity.AlipayPreference.SecondsBeforePayment,
                    IsAmountUnchangeable = entity.AlipayPreference.IsAmountUnchangeable,
                    IsAccountUnchangeable = entity.AlipayPreference.IsAccountUnchangeable,
                    IsH5RedirectByScanEnabled = entity.AlipayPreference.IsH5RedirectByScanEnabled,
                    IsH5RedirectByClickEnabled = entity.AlipayPreference.IsH5RedirectByClickEnabled,
                    IsH5RedirectByPickingPhotoEnabled = entity.AlipayPreference.IsH5RedirectByPickingPhotoEnabled
                };
            }

            //Build entry view model.
            var shopGatewayVM = new ShopGatewayEntry
            {
                ShopGatewayId = entity.Id,
                ShopId = entity.ShopId,
                ShopGatewayType = entity.GetShopGatewayType.Name,
                PaymentChannel = entity.GetPaymentChannel.Name,
                PaymentScheme = entity.GetPaymentScheme.Name,
                FourthPartyGatewayId = entity.FourthPartyGatewayId,
                AlipayPreferenceInfo = alipayPreferenceInfo,
                DateCreated = entity.DateCreated.ToFullString()
            };

            return shopGatewayVM;
        }


        private async Task<List<ShopGatewayEntry>> GetSortedRecords(
            IQueryable<ShopGatewayEntry> shopGatewayEntrys,
            int? pageIndex, int? take, string sortField = null, string direction = SortDirections.Descending)
        {
            var result = new List<ShopGatewayEntry>();

            if (pageIndex != null && take != null)
            {
                if (!string.IsNullOrEmpty(sortField))
                {
                    if (sortField == "DateCreated")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await shopGatewayEntrys
                               .OrderBy(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await shopGatewayEntrys
                               .OrderByDescending(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "ShopGatewayId")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await shopGatewayEntrys
                               .OrderBy(f => f.ShopGatewayId)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await shopGatewayEntrys
                               .OrderByDescending(f => f.ShopGatewayId)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "ShopGatewayType")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await shopGatewayEntrys
                               .OrderBy(f => f.ShopGatewayType)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await shopGatewayEntrys
                               .OrderByDescending(f => f.ShopGatewayType)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "PaymentChannel")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await shopGatewayEntrys
                               .OrderBy(f => f.PaymentChannel)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await shopGatewayEntrys
                               .OrderByDescending(f => f.PaymentChannel)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "PaymentScheme")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await shopGatewayEntrys
                               .OrderBy(f => f.PaymentScheme)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await shopGatewayEntrys
                               .OrderByDescending(f => f.PaymentScheme)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "FourthPartyGatewayId")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await shopGatewayEntrys
                                .OrderBy(f => f.FourthPartyGatewayId)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await shopGatewayEntrys
                               .OrderByDescending(f => f.FourthPartyGatewayId)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await shopGatewayEntrys
                               .OrderBy(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await shopGatewayEntrys
                               .OrderByDescending(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                }
                else
                {
                    result = await shopGatewayEntrys
                       .OrderByDescending(f => f.DateCreated)
                       .Skip((int)take * (int)pageIndex)
                       .Take((int)take)
                       .ToListAsync();
                }
            }
            else
            {
                result = await shopGatewayEntrys.ToListAsync();
            }

            return result;
        }




        public ShopGatewayEntry Add(ShopGatewayEntry shopGatewayEntry)
        {
            return _applicationContext.ShopGatewayEntrys.Add(shopGatewayEntry).Entity;
        }

        public void Update(ShopGatewayEntry shopGatewayEntry)
        {
            _applicationContext.Entry(shopGatewayEntry).State = EntityState.Modified;
        }

        public void Delete(ShopGatewayEntry shopGatewayEntry)
        {
            if (shopGatewayEntry != null)
            {
                _applicationContext.ShopGatewayEntrys.Remove(shopGatewayEntry);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _applicationContext.SaveChangesAsync();
        }
    }
}
