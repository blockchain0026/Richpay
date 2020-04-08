using Pairing.Domain.Exceptions;
using Pairing.Domain.Model.CloudDevices;
using Pairing.Domain.Model.ShopSettingsDomainModel;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Model.QrCodes
{
    public class PairingData : ValueObject
    {
        /*public bool IsOnline { get; private set; }
        public decimal MinCommissionRate { get; private set; }
        public decimal AvailableBalance { get; private set; }
        public string SpecifiedShopId { get; private set; }
        public decimal QuotaLeftToday { get; private set; }
        public DateTime? DateLastTraded { get; private set; }*/
        public int TotalSuccess { get; private set; }
        public int TotalFailures { get; private set; }
        public int HighestConsecutiveSuccess { get; private set; }
        public int HighestConsecutiveFailures { get; private set; }
        public int CurrentConsecutiveSuccess { get; private set; }
        public int CurrentConsecutiveFailures { get; private set; }
        public decimal SuccessRate { get; private set; }


        public static PairingData From(decimal toppestCommissionRate, decimal availableBalance, string specifiedShopId)
        {
            //Initailize
            var totalFailures = 0;
            var totalSuccess = 0;
            var highestConsecutiveSuccess = 0;
            var highestConsecutiveFailures = 0;
            var currentConsecutiveSuccess = 0;
            var currentConsecutiveFailures = 0;
            var successRate = 0;

            var pairingData = new PairingData(
                totalSuccess,
                totalFailures,
                highestConsecutiveSuccess,
                highestConsecutiveFailures,
                currentConsecutiveSuccess,
                currentConsecutiveFailures,
                successRate
                );

            return pairingData;
        }

        private PairingData(
            int totalSuccess, int totalFailures, int highestConsecutiveSuccess, int highestConsecutiveFailures, int currentConsecutiveSuccess, int currentConsecutiveFailures, decimal successRate)
        {
            TotalSuccess = totalSuccess;
            TotalFailures = totalFailures;
            HighestConsecutiveSuccess = highestConsecutiveSuccess;
            HighestConsecutiveFailures = highestConsecutiveFailures;
            CurrentConsecutiveSuccess = currentConsecutiveSuccess;
            CurrentConsecutiveFailures = currentConsecutiveFailures;
            SuccessRate = successRate;
        }

        /*public static PairingData From(decimal minCommissionRate, decimal availableBalance, ShopSettings shopSettings = null)
        {
            ValidateCommissionRate(minCommissionRate);
            ValidateBalance(availableBalance);

            return new PairingData(
                minCommissionRate,
                availableBalance,
                shopSettings?.ShopId
                );
        }*/



        public PairingData Online(CloudDevice cloudDevice)
        {
            //Checking the cloud device is provided.
            if (cloudDevice == null)
            {
                throw new PairingDomainException("The cloud device must be provided." + ". At PairingData.ValidateCommissionRate()");
            }
            //Checking the status is online.
            if (cloudDevice.GetCloudDeviceStatus.Id != CloudDeviceStatus.Online.Id)
            {
                throw new PairingDomainException("Can not set to online if cloud device is stay offline" + ". At PairingData.ValidateCommissionRate()");
            }

            var pairingData = new PairingData(
                this.TotalSuccess,
                this.TotalFailures,
                this.HighestConsecutiveSuccess,
                this.HighestConsecutiveFailures,
                this.CurrentConsecutiveSuccess,
                this.CurrentConsecutiveFailures,
                this.SuccessRate
                );

            return pairingData;
        }
        public PairingData Offline(CloudDevice cloudDevice)
        {
            //Checking the cloud device is provided.
            if (cloudDevice == null)
            {
                throw new PairingDomainException("The cloud device must be provided." + ". At PairingData.ValidateCommissionRate()");
            }
            //Checking the status is offline.
            if (cloudDevice.GetCloudDeviceStatus.Id != CloudDeviceStatus.Offline.Id)
            {
                throw new PairingDomainException("Can not set to online if cloud device is online" + ". At PairingData.ValidateCommissionRate()");
            }

            var pairingData = new PairingData(
                this.TotalSuccess,
                this.TotalFailures,
                this.HighestConsecutiveSuccess,
                this.HighestConsecutiveFailures,
                this.CurrentConsecutiveSuccess,
                this.CurrentConsecutiveFailures,
                this.SuccessRate
                );

            return pairingData;
        }

        public PairingData UpdateSuccessRateAndRelatedData(QrCodeOrder qrCodeOrder)
        {
            //Checking the order is provided.
            if (qrCodeOrder == null)
            {
                throw new PairingDomainException("The qr code order must be provided" + ". At PairingData.UpdateSuccessRateAndRelatedData()");
            }

            var totalSuccess = this.TotalSuccess;
            var totalFailures = this.TotalFailures;
            var highestConsecutiveSuccess = this.HighestConsecutiveSuccess;
            var highestConsecutiveFailures = this.HighestConsecutiveFailures;
            var currentConsecutiveSuccess = this.CurrentConsecutiveSuccess;
            var currentConsecutiveFailures = this.CurrentConsecutiveFailures;

            //Only calculate finished order
            if (!qrCodeOrder.IsSuccess() && !qrCodeOrder.IsFailed())
            {
                throw new PairingDomainException("Only finished order can be inluded in success rate data calculation" + ". At PairingData.UpdateSuccessRateAndRelatedData()");
            }

            //Calculate total success.
            if (qrCodeOrder.IsSuccess())
            {
                totalSuccess += 1;
            }

            //Calculate total failures.
            if (qrCodeOrder.IsFailed())
            {
                totalFailures += 1;
            }

            //Calculate consecutive data.
            if (qrCodeOrder.IsSuccess())
            {
                //If the current consecutive failures is higher than the record, update it.
                /*if (currentConsecutiveFailures > this.HighestConsecutiveFailures)
                {
                    highestConsecutiveFailures = currentConsecutiveFailures;
                }*/

                //The current order is success, so reset the current consecutive failures.
                currentConsecutiveFailures = 0;

                //add current consecutive success.
                currentConsecutiveSuccess += 1;

                //If updated current consecutive success is higher than the record, update it.
                if (currentConsecutiveSuccess > this.HighestConsecutiveSuccess)
                {
                    highestConsecutiveSuccess = currentConsecutiveSuccess;
                }
            }
            else
            {
                //If the current consecutive success is higher than the record, update it.
                /*if (currentConsecutiveSuccess > this.HighestConsecutiveSuccess)
                {
                    highestConsecutiveSuccess = currentConsecutiveSuccess;
                }*/

                //The current order is failed, so add the count.
                currentConsecutiveFailures += 1;

                //reset current consecutive success.
                currentConsecutiveSuccess = 0;

                //If updated current consecutive failures is higher than the record, update it.
                if (currentConsecutiveFailures > this.HighestConsecutiveFailures)
                {
                    highestConsecutiveFailures = currentConsecutiveFailures;
                }
            }

            //Calculate success rate (0.XX).
            var successRate = decimal.Round((decimal)totalSuccess / (decimal)(totalSuccess + totalFailures), 2);



            var pairingData = new PairingData(
                totalSuccess,
                totalFailures,
                highestConsecutiveSuccess,
                highestConsecutiveFailures,
                currentConsecutiveSuccess,
                currentConsecutiveFailures,
                successRate
                );

            return pairingData;
        }


        public PairingData UpdateSuccessRateAndRelatedData(bool isSuccess, DateTime dateCreated)
        {
            var totalSuccess = this.TotalSuccess;
            var totalFailures = this.TotalFailures;
            var highestConsecutiveSuccess = this.HighestConsecutiveSuccess;
            var highestConsecutiveFailures = this.HighestConsecutiveFailures;
            var currentConsecutiveSuccess = this.CurrentConsecutiveSuccess;
            var currentConsecutiveFailures = this.CurrentConsecutiveFailures;

            //Calculate total success.
            if (isSuccess)
            {
                totalSuccess += 1;
                //The current order is success, so reset the current consecutive failures.
                currentConsecutiveFailures = 0;

                //add current consecutive success.
                currentConsecutiveSuccess += 1;

                //If updated current consecutive success is higher than the record, update it.
                if (currentConsecutiveSuccess > this.HighestConsecutiveSuccess)
                {
                    highestConsecutiveSuccess = currentConsecutiveSuccess;
                }
            }
            else
            {
                totalFailures += 1;

                //The current order is failed, so add the count.
                currentConsecutiveFailures += 1;

                //reset current consecutive success.
                currentConsecutiveSuccess = 0;

                //If updated current consecutive failures is higher than the record, update it.
                if (currentConsecutiveFailures > this.HighestConsecutiveFailures)
                {
                    highestConsecutiveFailures = currentConsecutiveFailures;
                }
            }
            //Calculate success rate (0.XX).
            var successRate = decimal.Round((decimal)totalSuccess / (decimal)(totalSuccess + totalFailures), 2);



            var pairingData = new PairingData(
                totalSuccess,
                totalFailures,
                highestConsecutiveSuccess,
                highestConsecutiveFailures,
                currentConsecutiveSuccess,
                currentConsecutiveFailures,
                successRate
                );

            return pairingData;
        }



        public PairingData ResetCurrentConsecutiveFailures()
        {
            //Reset current consecutive failures may result in incorrect data calculation.

            var pairingData = new PairingData(
                this.TotalSuccess,
                this.TotalFailures,
                this.HighestConsecutiveSuccess,
                this.HighestConsecutiveFailures,
                this.CurrentConsecutiveSuccess,
                0,
                this.SuccessRate
                );

            return pairingData;
        }
        public PairingData ResetSuccessRateAndRelatedData()
        {
            var pairingData = new PairingData(
                0,
                0,
                0,
                0,
                0,
                0,
                0.00M
                );

            return pairingData;
        }


        public static void ValidateCommissionRate(decimal rate)
        {
            //Checking the commission rate is greater than 0 and less than 1.
            if (rate <= 0 || rate >= 1)
            {
                throw new PairingDomainException("The commission rates must be greater than 0 and less than 1" + ". At PairingData.ValidateCommissionRate()");
            }

            //Checking the commission rate has only three points.
            if (decimal.Round(rate, 3) != rate)
            {
                throw new PairingDomainException("The commission rates can only has 3 points" + ". At PairingData.ValidateCommissionRate()");
            }
        }

        public static void ValidateBalance(decimal balance)
        {
            //Checking the balance is greater than or equal to 0.
            if (balance < 0)
            {
                throw new PairingDomainException("The balance must be greater than or equal to 0" + ". At PairingData.ValidateBalance()");
            }

            //Checking the balance has only three points.
            if (decimal.Round(balance, 3) != balance)
            {
                throw new PairingDomainException("The balance can only has 3 points" + ". At PairingData.ValidateBalance()");
            }
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return TotalFailures;
            yield return TotalSuccess;
            yield return HighestConsecutiveSuccess;
            yield return HighestConsecutiveFailures;
            yield return CurrentConsecutiveFailures;
            yield return SuccessRate;
        }
    }
}
