using Distributing.Domain.Events;
using Distributing.Domain.Model.Commissions;
using MediatR;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.QrCodes;
using WebMVC.Applications.Queries.ShopAgents;
using WebMVC.Applications.Queries.Shops;
using WebMVC.Applications.Queries.TraderAgents;
using WebMVC.Applications.Queries.Traders;

namespace WebMVC.Applications.DomainEventHandlers.DistributingDomain
{
    /// <summary>
    /// If it is a trader balance, Update the available balance of his QR Codes, 
    ///    and update target qr code entry's pairing info.
    /// Update target QrCode status.
    /// Update view model: update balance.
    /// </summary>
    public class BalanceDistributedDomainEventHandler
            : INotificationHandler<BalanceDistributedDomainEvent>
    {
        private readonly IQrCodeQueries _qrCodeQueries;
        private readonly ITraderAgentQueries _traderAgentQueries;
        private readonly ITraderQueries _traderQueries;
        private readonly IShopAgentQueries _shopAgentQueries;
        private readonly IShopQueries _shopQueries;

        private readonly IQrCodeRepository _qrCodeRepository;

        public BalanceDistributedDomainEventHandler(IQrCodeQueries qrCodeQueries, ITraderAgentQueries traderAgentQueries, ITraderQueries traderQueries, IShopAgentQueries shopAgentQueries, IShopQueries shopQueries, IQrCodeRepository qrCodeRepository)
        {
            _qrCodeQueries = qrCodeQueries ?? throw new ArgumentNullException(nameof(qrCodeQueries));
            _traderAgentQueries = traderAgentQueries ?? throw new ArgumentNullException(nameof(traderAgentQueries));
            _traderQueries = traderQueries ?? throw new ArgumentNullException(nameof(traderQueries));
            _shopAgentQueries = shopAgentQueries ?? throw new ArgumentNullException(nameof(shopAgentQueries));
            _shopQueries = shopQueries ?? throw new ArgumentNullException(nameof(shopQueries));
            _qrCodeRepository = qrCodeRepository ?? throw new ArgumentNullException(nameof(qrCodeRepository));
        }

        //private readonly IShopAgentQueries _shopAgentQueries;

        public async Task Handle(BalanceDistributedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var balance = domainEvent.Balance;

            //Update balance record.
            if (balance.GetUserType.Id == UserType.TraderAgent.Id)
            {
                //Update trader agent.
                /*var traderAgentVM = await _traderAgentQueries.GetTraderAgent(balance.UserId);
                if (traderAgentVM != null)
                {
                    traderAgentVM.Balance.AmountAvailable = balance.AmountAvailable;
                    _traderAgentQueries.Update(traderAgentVM);

                    //await _traderAgentQueries.SaveChangesAsync();
                }*/
                //await _traderAgentQueries.UpdateBalance(balance.UserId, balance.AmountAvailable);
            }
            else if (domainEvent.Balance.GetUserType.Id == UserType.Trader.Id)
            {
                //Find all qr codes and update balance.
                /*var userQrCodes = await _qrCodeRepository.GetByUserIdAsync(balance.UserId);
                foreach (var userQrCode in userQrCodes)
                {
                    userQrCode.BalanceUpdated(balance.AmountAvailable);

                    //Update target QR code pairing data and status.
                    if (userQrCode.Id == domainEvent.QrCodeId)
                    {
                        userQrCode.OrderSuccess(
                            domainEvent.Distribution.Order.TrackingNumber,
                            domainEvent.Distribution.Order.Amount,
                            domainEvent.Distribution.Order.DateCreated
                            );
                    }

                    _qrCodeRepository.Update(userQrCode);

                    //Update Qr code view model.
                    var userQrCodeVM = await _qrCodeQueries.GetQrCodeEntryAsync(userQrCode.Id);

                    if (userQrCodeVM != null)
                    {
                        //Update available balance.
                        userQrCodeVM.PairingInfo.AvailableBalance = userQrCode.PairingData.AvailableBalance;

                        if (userQrCode.Id == domainEvent.QrCodeId)
                        {
                            //Update success data and quota left data.
                            userQrCodeVM.PairingInfo.TotalSuccess = userQrCode.PairingData.TotalSuccess;
                            userQrCodeVM.PairingInfo.TotalFailures = userQrCode.PairingData.TotalFailures;
                            userQrCodeVM.PairingInfo.HighestConsecutiveSuccess = userQrCode.PairingData.HighestConsecutiveSuccess;
                            userQrCodeVM.PairingInfo.HighestConsecutiveFailures = userQrCode.PairingData.HighestConsecutiveFailures;
                            userQrCodeVM.PairingInfo.CurrentConsecutiveSuccess = userQrCode.PairingData.CurrentConsecutiveSuccess;
                            userQrCodeVM.PairingInfo.CurrentConsecutiveFailures = userQrCode.PairingData.CurrentConsecutiveFailures;
                            userQrCodeVM.PairingInfo.SuccessRateInPercent = (int)(userQrCode.PairingData.SuccessRate * 100);

                            userQrCodeVM.PairingInfo.QuotaLeftToday = userQrCode.PairingData.QuotaLeftToday;
                        }



                        userQrCodeVM.PairingStatus = userQrCode.GetPairingStatus.Name;
                        userQrCodeVM.PairingStatusDescription = userQrCode.PairingStatusDescription;
                        _qrCodeQueries.Update(userQrCodeVM);
                    }
                }
                await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();*/

                //await _qrCodeQueries.SaveChangesAsync();


                //Update view model.
                /*var traderVm = await _traderQueries.GetTrader(balance.UserId);

                if (traderVm != null)
                {
                    traderVm.Balance.AmountAvailable = balance.AmountAvailable;
                    _traderQueries.Update(traderVm);
                    //await _traderQueries.SaveChangesAsync();
                }*/
                //await _traderQueries.UpdateBalance(balance.UserId, balance.AmountAvailable);
            }
            else if (domainEvent.Balance.GetUserType.Id == UserType.ShopAgent.Id)
            {
                /*var shopAgentVm = await _shopAgentQueries.GetShopAgent(balance.UserId);

                if (shopAgentVm != null)
                {
                    shopAgentVm.Balance.AmountAvailable = balance.AmountAvailable;
                    _shopAgentQueries.Update(shopAgentVm);
                    //await _shopAgentQueries.SaveChangesAsync();
                }*/
                //await _shopAgentQueries.UpdateBalance(balance.UserId, balance.AmountAvailable);
            }
            else if (domainEvent.Balance.GetUserType.Id == UserType.Shop.Id)
            {
                /*var shopVm = await _shopQueries.GetShop(balance.UserId);

                if (shopVm != null)
                {
                    shopVm.Balance.AmountAvailable = balance.AmountAvailable;
                    _shopQueries.Update(shopVm);
                    //await _shopQueries.SaveChangesAsync();
                }*/

                //await _shopQueries.UpdateBalance(balance.UserId, balance.AmountAvailable);
            }

        }
    }
}
