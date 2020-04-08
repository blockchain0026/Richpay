using Distributing.Domain.Events;
using Distributing.Domain.Exceptions;
using Distributing.Domain.Model.Distributions;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributing.Domain.Model.Commissions
{
    public class Commission : Entity, IAggregateRoot
    {
        //public string CommissionId { get; private set; }
        public int BalanceId { get; private set; }
        public string UserId { get; private set; }
        public int ChainNumber { get; private set; }

        public UserType UserType { get; private set; }
        private int _userTypeId;

        public int? UplineCommissionId { get; private set; }
        public decimal RateAlipay { get; private set; }
        public decimal RateWechat { get; private set; }
        public decimal RateRebateAlipay { get; private set; }
        public decimal RateRebateWechat { get; private set; }
        public bool IsEnabled { get; private set; }

        public UserType GetUserType => UserType.From(this._userTypeId);


        protected Commission()
        {
        }

        public Commission(int balanceId, string userId, int userTypeId, int chainNumber,
            int? uplineCommissionId, decimal rateAlipay, decimal rateWechat, decimal rateRebateAlipay, decimal rateRebateWechat, bool isEnabled) : this()
        {
            //CommissionId = Guid.NewGuid().ToString();
            BalanceId = balanceId;
            _userTypeId = userTypeId;
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            ChainNumber = chainNumber;
            UplineCommissionId = uplineCommissionId;
            RateAlipay = rateAlipay;
            RateWechat = rateWechat;
            RateRebateAlipay = rateRebateAlipay;
            RateRebateWechat = rateRebateWechat;
            IsEnabled = isEnabled;

            this.AddDomainEvent(new CommissionCreatedDomainEvent(
                this,
                userId,
                userTypeId,
                uplineCommissionId,
                rateAlipay,
                rateWechat,
                rateRebateAlipay,
                rateRebateWechat,
                isEnabled
                ));
        }

        public static Commission FromTradeUser(int balanceId, string userId, int chainNumber, UserType userType, decimal rateAlipay, decimal rateWechat, bool isEnabled, Commission uplineCommission = null)
        {
            //Checking the user's id is provided.
            if (string.IsNullOrEmpty(userId))
            {
                throw new DistributingDomainException("The trade user id must be provided" + ". At Commission.FromTradeUser()");
            }

            if (uplineCommission != null)
            {
                //Checking the upline commssion's Id isn't null or empty.
                /*if (string.IsNullOrEmpty(uplineCommission.CommissionId))
                {
                    throw new DistributingDomainException("Invalid Param: " + nameof(uplineCommission) + " " + nameof(userId) + ". At Commission.FromTradeUser()");
                }*/
                if (uplineCommission.IsTransient())
                {
                    throw new DistributingDomainException("Invalid Param: " + nameof(uplineCommission) + " " + nameof(userId) + ". At Commission.FromTradeUser()");
                }

                //Checking the upline is a trader agent.
                if (uplineCommission._userTypeId != UserType.TraderAgent.Id)
                {
                    throw new DistributingDomainException("The upline must be a trader agent" + ". At Commission.FromTradeUser()");
                }
            }

            //Checking the user type is correct.
            if (userType.Id != UserType.Trader.Id && userType.Id != UserType.TraderAgent.Id)
            {
                throw new DistributingDomainException("Invalid Param: " + nameof(userType) + ". At Commission.FromTradeUser()");
            }

            //Checking the rate is larger than or equal to 0 and less than or equal to 1.
            if (rateAlipay < 0 || rateWechat < 0 || rateAlipay >= 1 || rateWechat >= 1)
            {
                throw new DistributingDomainException("The rates must larger than or equal to 0 and less than or equal to 1" + ". At Commission.FromTradeUser()");
            }

            //Checking the rate doesn't has more than 3 points.
            if (decimal.Round(rateAlipay, 3) != rateAlipay || decimal.Round(rateWechat, 3) != rateWechat)
            {
                throw new DistributingDomainException("The rates must not has more than 3 points" + ". At Commission.FromTradeUser()");
            }

            //The rates must less than upline commission's rates.
            if (uplineCommission != null)
            {
                if (rateAlipay >= uplineCommission.RateAlipay || rateWechat >= uplineCommission.RateWechat)
                {
                    throw new DistributingDomainException("The rates must be less than upline's rates" + ". At Commission.FromTradeUser()");
                }
            }
            var commission = new Commission(
                balanceId,
                userId,
                userType.Id,
                chainNumber,
                uplineCommission != null ? uplineCommission.Id : (int?)null, //Set to empty if there is no upline.
                rateAlipay,
                rateWechat,
                0,
                0,
                isEnabled
                );


            return commission;
        }

        public static Commission FromShopUser(int balanceId, string userId, int chainNumber, UserType userType, decimal rateRebateAlipay, decimal rateRebateWechat, bool isEnabled, Commission uplineCommission = null)
        {
            //Checking the user's id is provided.
            if (string.IsNullOrEmpty(userId))
            {
                throw new DistributingDomainException("The trade user id must be provided" + ". At Commission.FromShopUser()");
            }

            if (uplineCommission != null)
            {
                //Checking the upline commssion's Id isn't null or empty.
                /*if (string.IsNullOrEmpty(uplineCommission.CommissionId))
                {
                    throw new DistributingDomainException("Invalid Param: " + nameof(uplineCommission) + " " + nameof(userId) + ". At Commission.FromShopUser()");
                }*/
                if (uplineCommission.IsTransient())
                {
                    throw new DistributingDomainException("Invalid Param: " + nameof(uplineCommission) + " " + nameof(userId) + ". At Commission.FromShopUser()");
                }

                //Checking the upline is a shop agent.
                if (uplineCommission._userTypeId != UserType.ShopAgent.Id)
                {
                    throw new DistributingDomainException("The upline must be a shop agent" + ". At Commission.FromShopUser()");
                }
            }

            //Checking the user type is correct.
            if (userType.Id != UserType.Shop.Id && userType.Id != UserType.ShopAgent.Id)
            {
                throw new DistributingDomainException("Invalid Param: " + nameof(userType) + ". At Commission.FromShopUser()");
            }

            //Checking the rate is larger than or equal to 0 and less than or equal to 1.
            if (rateRebateAlipay < 0 || rateRebateWechat < 0 || rateRebateAlipay >= 1 || rateRebateWechat >= 1)
            {
                throw new DistributingDomainException("The rates must larger than or equal to 0 and less than or equal to 1" + ". At Commission.FromShopUser()");
            }

            //Checking the rate doesn't has more than 3 points.
            if (decimal.Round(rateRebateAlipay, 3) != rateRebateAlipay || decimal.Round(rateRebateWechat, 3) != rateRebateWechat)
            {
                throw new DistributingDomainException("The rates must not has more than 3 points" + ". At Commission.FromShopUser()");
            }

            //The rates must larger than upline commission's rates.
            if (uplineCommission != null)
            {
                if (rateRebateAlipay <= uplineCommission.RateRebateAlipay || rateRebateWechat <= uplineCommission.RateRebateWechat)
                {
                    throw new DistributingDomainException("The rates must be larger than upline's rates" + ". At Commission.FromShopUser()");
                }
            }

            var commission = new Commission(
                balanceId,
                userId,
                userType.Id,
                chainNumber,
                uplineCommission != null ? uplineCommission.Id : (int?)null, //Set to empty if there is no upline.
                0,
                0,
                rateRebateAlipay,
                rateRebateWechat,
                isEnabled
                );


            return commission;
        }

        public void Enable()
        {
            this.IsEnabled = true;

            this.AddDomainEvent(new CommissionEnabledDomainEvent(
                this,
                this.IsEnabled
                ));
        }

        public void Disable()
        {
            this.IsEnabled = false;

            this.AddDomainEvent(new CommissionDisabledDomainEvent(
                this,
                this.IsEnabled
                ));
        }

        public void UpdateRate(decimal rateAlipay, decimal rateWechat, Commission uplineCommission = null)
        {
            //Checking this is trader user commission.
            if (this._userTypeId != UserType.TraderAgent.Id && this._userTypeId != UserType.Trader.Id)
            {
                throw new DistributingDomainException("Trader users doesn't have rebate rate." + ". At Commission.UpdateRate()");
            }

            //Checking the upline commission is the right one.
            if (this.UplineCommissionId != null &&
                (uplineCommission == null || uplineCommission.IsTransient() || uplineCommission.Id != this.UplineCommissionId))
            {
                throw new DistributingDomainException("The upline commission is not the right one" + ". At Commission.UpdateRate()");
            }

            //Checking the rate is larger than or equal to 0 and less than or equal to 1.
            if (rateAlipay < 0 || rateWechat < 0 || rateAlipay >= 1 || rateWechat >= 1)
            {
                throw new DistributingDomainException("The rates must larger than or equal to 0 and less than or equal to 1" + ". At Commission.UpdateRate()");
            }

            //Checking the rate doesn't has more than 3 points.
            if (decimal.Round(rateAlipay, 3) != rateAlipay || decimal.Round(rateWechat, 3) != rateWechat)
            {
                throw new DistributingDomainException("The rates must not has more than 3 points" + ". At Commission.UpdateRate()");
            }

            //The rates must less than upline commission's rates.
            if (this.UplineCommissionId != null)
            {
                if (rateAlipay >= uplineCommission.RateAlipay)
                {
                    //Unless the rate is 0, throw exception.
                    if (rateAlipay != 0)
                    {
                        throw new DistributingDomainException("The rates must be less than upline's rates" + ". At Commission.UpdateRate()");
                    }
                }
                if (rateWechat >= uplineCommission.RateWechat)
                {
                    //Unless the rate is 0, throw exception.
                    if (rateWechat != 0)
                    {
                        throw new DistributingDomainException("费率必须小于上级代理费率" + ". At Commission.UpdateRate()");
                    }
                }
            }


            this.RateAlipay = rateAlipay;
            this.RateWechat = rateWechat;


            this.AddDomainEvent(new CommissionRateUpdatedDomainEvent(
                this,
                this.RateAlipay,
                this.RateWechat
                ));
        }

        public void UpdateRebateRate(decimal rateRebateAlipay, decimal rateRebateWechat, Commission uplineCommission = null)
        {
            //Checking this is shop user commission.
            if (this._userTypeId != UserType.ShopAgent.Id && this._userTypeId != UserType.Shop.Id)
            {
                throw new DistributingDomainException("Shop users can only update rebate rate." + ". At Commission.UpdateRebateRate()");
            }

            //Checking the upline commission is the right one.
            if (this.UplineCommissionId != null &&
                (uplineCommission == null || uplineCommission.IsTransient() || uplineCommission.Id != this.UplineCommissionId))
            {
                throw new DistributingDomainException("The upline commission is not the right one" + ". At Commission.UpdateRebateRate()");
            }

            //Checking the rate is larger than or equal to 0 and less than or equal to 1.
            if (rateRebateAlipay < 0 || rateRebateWechat < 0 || rateRebateAlipay >= 1 || rateRebateWechat >= 1)
            {
                throw new DistributingDomainException("The rates must larger than or equal to 0 and less than or equal to 1" + ". At Commission.UpdateRebateRate()");
            }

            //Checking the rate doesn't has more than 3 points.
            if (decimal.Round(rateRebateAlipay, 3) != rateRebateAlipay || decimal.Round(rateRebateWechat, 3) != rateRebateWechat)
            {
                throw new DistributingDomainException("The rates must not has more than 3 points" + ". At Commission.UpdateRebateRate()");
            }

            //The rates must larger than upline commission's rates.
            if (this.UplineCommissionId != null)
            {
                if (rateRebateAlipay <= uplineCommission.RateRebateAlipay)
                {
                    //Unless the rate is 0, throw exception.
                    if (uplineCommission.RateRebateAlipay != 0 || rateRebateAlipay != 0)
                    {
                        throw new DistributingDomainException("The rates must be larger than upline's rates" + ". At Commission.UpdateRate()");
                    }
                }
                if (rateRebateWechat <= uplineCommission.RateRebateWechat)
                {
                    //Unless the rate is 0, throw exception.
                    if (uplineCommission.RateRebateWechat != 0 || rateRebateWechat != 0)
                    {
                        throw new DistributingDomainException("The rates must be larger than upline's rates" + ". At Commission.UpdateRate()");
                    }
                }
            }


            this.RateRebateAlipay = rateRebateAlipay;
            this.RateRebateWechat = rateRebateWechat;

            this.AddDomainEvent(new CommissionRebateRateUpdatedDomainEvent(
                this,
                this.RateRebateAlipay,
                this.RateRebateWechat
                ));
        }

        public void UplineDeleted()
        {
            this.UplineCommissionId = null;
        }

        public void UplineAssigned(Commission uplineCommission)
        {
            if (this._userTypeId == UserType.TraderAgent.Id
                || this._userTypeId == UserType.Trader.Id)
            {
                //Checking the upline is a trader agent.
                if (uplineCommission.UserType.Id != UserType.TraderAgent.Id)
                {
                    throw new DistributingDomainException("User assigns to be trader user's upline must be a trader agent" + ". At Commission.UplineAssigned()");
                }

                //Checking the upline's rate is high enough.
                if (this.RateAlipay >= uplineCommission.RateAlipay || this.RateWechat >= uplineCommission.RateWechat)
                {
                    throw new DistributingDomainException("The rates of upline assigned is too low. Must be higher than the downline." + ". At Commission.UplineAssigned()");
                }
            }
            if (this._userTypeId == UserType.ShopAgent.Id
                || this._userTypeId == UserType.Shop.Id)
            {
                //Checking the upline is a shop agent.
                if (uplineCommission.UserType.Id != UserType.ShopAgent.Id)
                {
                    throw new DistributingDomainException("User assigns to be shop user's upline must be a shop agent" + ". At Commission.UplineAssigned()");
                }

                //Checking the upline's rate is low enough.
                if (this.RateRebateAlipay <= uplineCommission.RateRebateAlipay || this.RateRebateWechat <= uplineCommission.RateRebateWechat)
                {
                    throw new DistributingDomainException("The rates of upline assigned is too high. Must be less than the downline." + ". At Commission.UplineAssigned()");
                }
            }

            this.UplineCommissionId = uplineCommission.Id;
        }

    }
}
