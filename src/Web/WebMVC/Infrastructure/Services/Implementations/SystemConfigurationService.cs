using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Infrastructure.Configurations;
using WebMVC.Infrastructure.Services;

namespace WebMVC.Infrastructure.Services.Implementations
{
    public class SystemConfigurationService : ISystemConfigurationService
    {
        private readonly SystemConfigurations _settings;
        private readonly IWritableOptions<SiteBaseInfo> _siteBaseInfoSettings;
        private readonly IWritableOptions<Payment> _paymentSettings;
        private readonly IWritableOptions<SystemNotificationSound> _systemNotificationSoundSettings;
        private readonly IWritableOptions<UserNotification> _userNotificationSettings;
        private readonly IWritableOptions<WithdrawalAndDeposit> _withdrawalAndDepositSettings;
        private readonly IWritableOptions<PaymentChannel> _paymentChannelSettings;
        private readonly IWritableOptions<QrCodeConf> _qrCodeConfSettings;

        public SystemConfigurationService(
            //SystemConfigurations settings, 
            IWritableOptions<SiteBaseInfo> siteBaseInfoSettings,
            IWritableOptions<Payment> paymentSettings,
            IWritableOptions<SystemNotificationSound> systemNotificationSoundSettings,
            IWritableOptions<UserNotification> userNotificationSettings,
            IWritableOptions<WithdrawalAndDeposit> withdrawalAndDepositSettings,
            IWritableOptions<PaymentChannel> paymentChannelSettings,
            IWritableOptions<QrCodeConf> qrCodeConfSettings)
        {
            //_settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _siteBaseInfoSettings = siteBaseInfoSettings ?? throw new ArgumentNullException(nameof(siteBaseInfoSettings));
            _paymentSettings = paymentSettings ?? throw new ArgumentNullException(nameof(paymentSettings));
            _systemNotificationSoundSettings = systemNotificationSoundSettings ?? throw new ArgumentNullException(nameof(systemNotificationSoundSettings));
            _userNotificationSettings = userNotificationSettings ?? throw new ArgumentNullException(nameof(userNotificationSettings));
            _withdrawalAndDepositSettings = withdrawalAndDepositSettings ?? throw new ArgumentNullException(nameof(withdrawalAndDepositSettings));
            _paymentChannelSettings = paymentChannelSettings ?? throw new ArgumentNullException(nameof(paymentChannelSettings));
            _qrCodeConfSettings = qrCodeConfSettings ?? throw new ArgumentNullException(nameof(qrCodeConfSettings));
        }

        public WithdrawalAndDeposit GetWithdrawalAndDeposit()
        {
            return _withdrawalAndDepositSettings.Value;
        }

        public SiteBaseInfo GetSiteInfoAsync()
        {
            return _siteBaseInfoSettings.Value;
        }

        public Payment GetPaymentAsync()
        {
            return _paymentSettings.Value;
        }

        public SystemNotificationSound GetSystemNotificationSoundAsync()
        {
            return _systemNotificationSoundSettings.Value;
        }

        public UserNotification GetUserNotificationAsync()
        {
            return _userNotificationSettings.Value;
        }

        public WithdrawalAndDeposit GetWithdrawalAndDepositAsync()
        {
            return _withdrawalAndDepositSettings.Value;
        }

        public PaymentChannel GetPaymentChannelAsync()
        {
            return _paymentChannelSettings.Value;
        }

        public QrCodeConf GetQrCodeConfAsync()
        {
            return _qrCodeConfSettings.Value;
        }


        public void UpdateSiteBaseInfo(SiteBaseInfo info)
        {
            //var currentBaseInfo = _siteBaseInfoSettings;

            var siteTile = !string.IsNullOrEmpty(info.SiteTitle) ?
                info.SiteTitle : throw new ArgumentNullException(nameof(info.SiteTitle));
            var contactAddress = !string.IsNullOrEmpty(info.ContactAddress) ?
                info.ContactAddress : throw new ArgumentNullException(nameof(info.ContactAddress));
            var contactEmail = !string.IsNullOrEmpty(info.ContactEmail) ?
                info.ContactEmail : throw new ArgumentNullException(nameof(info.ContactEmail));
            var consumerHotline = !string.IsNullOrEmpty(info.ConsumerHotline) ?
                info.ConsumerHotline : throw new ArgumentNullException(nameof(info.ConsumerHotline));
            var consumerServiceQQ = !string.IsNullOrEmpty(info.ConsumerServiceQQ) ?
                info.ConsumerServiceQQ : throw new ArgumentNullException(nameof(info.ConsumerServiceQQ));
            var consumerServiceSkype = !string.IsNullOrEmpty(info.ConsumerServiceSkype) ?
                info.ConsumerServiceSkype : throw new ArgumentNullException(nameof(info.ConsumerServiceSkype));


            string businessHoursFrom = CheckDateTimeString(info.BusinessHoursFrom) ?
                info.BusinessHoursFrom : throw new ArgumentException("无效参数: " + nameof(info.BusinessHoursFrom));


            string businessHoursTo = CheckDateTimeString(info.BusinessHoursTo) ?
                info.BusinessHoursTo : throw new ArgumentException("无效参数: " + nameof(info.BusinessHoursTo));

            _siteBaseInfoSettings.Update(s =>
            {
                s.SiteTitle = siteTile;
                s.ContactAddress = contactAddress;
                s.ContactEmail = contactEmail;
                s.ConsumerHotline = consumerHotline;
                s.ConsumerServiceQQ = consumerServiceQQ;
                s.ConsumerServiceSkype = consumerServiceSkype;
                s.BusinessHoursFrom = businessHoursFrom;
                s.BusinessHoursTo = businessHoursTo;
            });
        }

        public void UpdatePayment(Payment payment)
        {
            var paymentTimeoutInSecond = payment.PaymentTimeoutInSecond;

            var sellRequestCountdownInSecond = payment.SellRequestCountdownInSecond;

            //Need to check the seconds before payment is positive.

            _paymentSettings.Update(s =>
            {
                s.PaymentTimeoutInSecond = paymentTimeoutInSecond;
                s.SellRequestCountdownInSecond = sellRequestCountdownInSecond;
                s.AlipayPaymentTemplate = payment.AlipayPaymentTemplate;
            });
        }

        public void UpdateSystemNotificationSound(SystemNotificationSound systemNotificationSound)
        {
            var withdraw = systemNotificationSound.Withdraw;
            var deposit = systemNotificationSound.Deposit;
            var member = systemNotificationSound.Member;
            var qrCode = systemNotificationSound.QrCode;
            var newOrder = systemNotificationSound.NewOrder;
            var orderTimeout = systemNotificationSound.OrderTimeout;

            _systemNotificationSoundSettings.Update(s =>
            {
                s.Withdraw = withdraw;
                s.Deposit = deposit;
                s.Member = member;
                s.QrCode = qrCode;
                s.NewOrder = newOrder;
                s.OrderTimeout = orderTimeout;
            });
        }

        public void UpdateUserNotification(UserNotification userNotification)
        {
            var orderTimeout = userNotification.OrderTimeout;
            var successDeposit = userNotification.SuccessDeposit;

            _userNotificationSettings.Update(s =>
            {
                s.OrderTimeout = orderTimeout;
                s.SuccessDeposit = successDeposit;
            });
        }

        public void UpdateWithdrawalAndDeposit(WithdrawalAndDeposit withdrawalAndDeposit)
        {
            var eachAmountUpperLimit = withdrawalAndDeposit.WithdrawalTemplate.EachAmountUpperLimit;
            var eachAmountLowerLimit = withdrawalAndDeposit.WithdrawalTemplate.EachAmountLowerLimit;
            var dailyAmountLimit = withdrawalAndDeposit.WithdrawalTemplate.DailyAmountLimit;
            var dailyFrequencyLimit = withdrawalAndDeposit.WithdrawalTemplate.DailyFrequencyLimit;
            var commissionInThousandth = withdrawalAndDeposit.WithdrawalTemplate.CommissionInThousandth;

            if (eachAmountLowerLimit > eachAmountUpperLimit)
            {
                throw new ArgumentOutOfRangeException("单笔最高限额需大于或等于单笔最低限额。");
            }

            var withdrawalTemplate = new WithdrawalTemplate
            {
                EachAmountUpperLimit = eachAmountUpperLimit >= 0 ? eachAmountUpperLimit : throw new ArgumentOutOfRangeException("Invalid Param: " + nameof(eachAmountUpperLimit)),
                EachAmountLowerLimit = eachAmountLowerLimit >= 0 ? eachAmountLowerLimit : throw new ArgumentOutOfRangeException("Invalid Param: " + nameof(eachAmountLowerLimit)),
                DailyAmountLimit = dailyAmountLimit >= 0 ? dailyAmountLimit : throw new ArgumentOutOfRangeException("Invalid Param: " + nameof(dailyAmountLimit)),
                DailyFrequencyLimit = dailyFrequencyLimit >= 0 ? dailyFrequencyLimit : throw new ArgumentOutOfRangeException("Invalid Param: " + nameof(dailyFrequencyLimit)),
                CommissionInThousandth = commissionInThousandth >= 0 && commissionInThousandth <= 1000 ? commissionInThousandth : throw new ArgumentOutOfRangeException("Invalid Param: " + nameof(commissionInThousandth))
            };

            var withdrawalOpenFrom = CheckDateTimeString(withdrawalAndDeposit.WithdrawalOpenFrom) ?
                withdrawalAndDeposit.WithdrawalOpenFrom : throw new ArgumentException("无效参数: " + nameof(withdrawalAndDeposit.WithdrawalOpenFrom));
            var withdrawalOpenTo = CheckDateTimeString(withdrawalAndDeposit.WithdrawalOpenTo) ?
                withdrawalAndDeposit.WithdrawalOpenTo : throw new ArgumentException("无效参数: " + nameof(withdrawalAndDeposit.WithdrawalOpenTo));

            var depositOpenFrom = CheckDateTimeString(withdrawalAndDeposit.DepositOpenFrom) ?
                withdrawalAndDeposit.DepositOpenFrom : throw new ArgumentException("无效参数: " + nameof(withdrawalAndDeposit.DepositOpenFrom));
            var depositOpenTo = CheckDateTimeString(withdrawalAndDeposit.DepositOpenTo) ?
                withdrawalAndDeposit.DepositOpenTo : throw new ArgumentException("无效参数: " + nameof(withdrawalAndDeposit.DepositOpenTo));

            _withdrawalAndDepositSettings.Update(s =>
            {
                s.WithdrawalTemplate = withdrawalTemplate;
                s.WithdrawalOpenFrom = withdrawalOpenFrom;
                s.WithdrawalOpenTo = withdrawalOpenTo;
                s.DepositOpenFrom = depositOpenFrom;
                s.DepositOpenTo = depositOpenTo;
            });
        }

        public void UpdatePaymentChannel(PaymentChannel paymentChannel)
        {
            var openFrom = CheckDateTimeString(paymentChannel.OpenFrom) ?
                paymentChannel.OpenFrom : throw new ArgumentException("无效参数: " + nameof(paymentChannel.OpenFrom));
            var openTo = CheckDateTimeString(paymentChannel.OpenTo) ?
                paymentChannel.OpenTo : throw new ArgumentException("无效参数: " + nameof(paymentChannel.OpenTo));
            var autoToggle = paymentChannel.AutoToggle;

            _paymentChannelSettings.Update(s =>
            {
                s.OpenFrom = openFrom;
                s.OpenTo = openTo;
                s.AutoToggle = autoToggle;
            });
        }

        public void UpdateQrCodeConf(QrCodeConf qrCodeConf)
        {
            //Need to check value.

            _qrCodeConfSettings.Update(s =>
            {
                s.RiskControlTemplate = qrCodeConf.RiskControlTemplate;
            });
        }


        #region Custom Function
        public IEnumerable<SelectListItem> GetDateTimeSelectList(bool isDay = false, bool isHour = false, bool isMinute = false, bool isSecond = false, int? selectedItemValue = null)
        {
            var dateTimeSelectList = new List<SelectListItem>();


            dateTimeSelectList.Add(new SelectListItem()
            {
                Value = "",
                Text = "Select Time...",
                Selected = selectedItemValue != null ? false : true
            });

            int min = 0;
            int max = 0;
            if (isDay)
            {
                min = 1;
                max = 30;
            }
            else if (isHour)
            {
                min = 0;
                max = 23;
            }
            else if (isMinute)
            {
                min = 0;
                max = 59;
            }
            else if (isSecond)
            {
                min = 0;
                max = 59;
            }
            else
            {
                throw new ArgumentException("Must specify one type of datetime list.");
            }
            bool selected = false;
            string selectedValue;
            if (selectedItemValue.HasValue)
            {
                if (selectedItemValue.Value < 10)
                {
                    selectedValue = "0" + selectedItemValue.Value.ToString();
                }
                else
                {
                    selectedValue = selectedItemValue.Value.ToString();
                }

            }
            else
            {
                selectedValue = "-1";
            }

            for (int i = min; i <= max; i++)
            {
                var itemValue = i < 10 ? "0" + i.ToString() : i.ToString();
                dateTimeSelectList.Add(new SelectListItem()
                {
                    Value = itemValue,
                    Text = itemValue,
                    //ToString(Nullable<T>) on a null value will return "" .
                    Selected = itemValue == selectedValue ? true : false
                });

                if (itemValue == selectedValue)
                {
                    selected = true;
                }

            }
            if (!selected)
            {
                throw new ArgumentOutOfRangeException("No specified value found in select list.");
            }

            return dateTimeSelectList;
        }

        public bool CheckDateTimeString(string dateTimeString)
        {
            return DateTime.TryParseExact(dateTimeString, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt);
        }

        #endregion
    }
}

