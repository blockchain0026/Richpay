using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Infrastructure.Services
{
    public interface ISystemConfigurationService
    {
        public WithdrawalAndDeposit GetWithdrawalAndDeposit();

        public SiteBaseInfo GetSiteInfoAsync();
        public Payment GetPaymentAsync();
        public SystemNotificationSound GetSystemNotificationSoundAsync();
        public UserNotification GetUserNotificationAsync();
        public WithdrawalAndDeposit GetWithdrawalAndDepositAsync();
        public PaymentChannel GetPaymentChannelAsync();
        public QrCodeConf GetQrCodeConfAsync();

        public void UpdateSiteBaseInfo(SiteBaseInfo info);
        public void UpdatePayment(Payment payment);
        public void UpdateSystemNotificationSound(SystemNotificationSound systemNotificationSound);
        public void UpdateUserNotification(UserNotification userNotification);
        public void UpdateWithdrawalAndDeposit(WithdrawalAndDeposit withdrawalAndDeposit);
        public void UpdatePaymentChannel(PaymentChannel paymentChannel);

        public void UpdateQrCodeConf(QrCodeConf qrCodeConf);

        public IEnumerable<SelectListItem> GetDateTimeSelectList(bool isDay = false, bool isHour = false, bool isMinute = false, bool isSecond = false, int? selectedItemValue = null);

    }
}
