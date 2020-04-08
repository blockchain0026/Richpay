using Ordering.Domain.Events;
using Ordering.Domain.Exceptions;
using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ordering.Domain.Model.ShopApis
{
    public class ShopApi : Entity, IAggregateRoot
    {
        public string ShopId { get; private set; }
        public string ApiKey { get; private set; }

        private readonly List<IpWhitelist> _ipWhitelists;
        public IReadOnlyCollection<IpWhitelist> IpWhitelists => _ipWhitelists;


        protected ShopApi()
        {
            this._ipWhitelists = new List<IpWhitelist>();
        }

        public ShopApi(string shopId, string apiKey) : this()
        {
            ShopId = shopId ?? throw new ArgumentNullException(nameof(shopId));
            ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));

            this.AddDomainEvent(new ShopApiCreatedDomainEvent(
                this,
                shopId,
                apiKey
                ));
        }

        public static ShopApi From(string shopId)
        {
            //Checking the shop id is provided.
            if (string.IsNullOrEmpty(shopId))
            {
                throw new OrderingDomainException("无效的商户ID");
            }

            //Generating new Api key.
            var apiKey = Guid.NewGuid().ToString("N");

            return new ShopApi(shopId, apiKey);
        }

        public void UpdateWholeIpWhitelists(List<string> ipWhitelists)
        {
            //Remove all.
            this._ipWhitelists.Clear();

            //Add new ip.
            foreach (var ip in ipWhitelists)
            {
                this._ipWhitelists.Add(new IpWhitelist(ip));
            }

            this.AddDomainEvent(new ShopApiWhitelistAddedDomainEvent(this, this.ApiKey));
        }

        public void RemoveIpFromWhitelist(int ipWhitelistId)
        {
            var existingIpWhitelist = this._ipWhitelists
                .Where(i => i.Id == ipWhitelistId).FirstOrDefault();

            if (existingIpWhitelist == null)
            {
                throw new OrderingDomainException("查无指定IP");
            }

            this._ipWhitelists.Remove(existingIpWhitelist);

            this.AddDomainEvent(new ShopApiKeyGeneratedDomainEvent(this, this.ApiKey));
        }

        public void GenerateNewApiKey()
        {
            this.ApiKey = Guid.NewGuid().ToString("N");
            this.AddDomainEvent(new ShopApiKeyGeneratedDomainEvent(
                this,
                ApiKey
                ));
        }

        public bool IsIpInWhitelists(string ip)
        {
            var result = this._ipWhitelists.Any(i => i.GetAddress() == ip);

            return result;
        }
    }
}
