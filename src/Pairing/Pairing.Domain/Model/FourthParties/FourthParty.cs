using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Model.FourthParties
{
    public class FourthParty : Entity, IAggregateRoot
    {
        public string ShopId { get; private set; }
        public string Name { get; private set; }
        public string SiteUrl { get; private set; }


        public bool IsOpen { get; private set; }



        protected FourthParty()
        {
        }

        public FourthParty(string shopId, string name, string siteUrl, bool isOpen)
        {
            ShopId = shopId ?? throw new ArgumentNullException(nameof(shopId));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            SiteUrl = siteUrl ?? throw new ArgumentNullException(nameof(siteUrl));
            IsOpen = isOpen;
        }
    }
}
