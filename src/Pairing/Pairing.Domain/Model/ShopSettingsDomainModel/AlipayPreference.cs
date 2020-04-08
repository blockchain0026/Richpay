using Pairing.Domain.Exceptions;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Model.ShopSettingsDomainModel
{
    public class AlipayPreference : ValueObject
    {
        public int SecondsBeforePayment { get; private set; }
        public bool IsAmountUnchangeable { get; private set; }
        public bool IsAccountUnchangeable { get; private set; }
        public bool IsH5RedirectByScanEnabled { get; private set; }
        public bool IsH5RedirectByClickEnabled { get; private set; }
        public bool IsH5RedirectByPickingPhotoEnabled { get; private set; }

        public AlipayPreference(int secondsBeforePayment, bool isAmountUnchangeable, bool isAccountUnchangeable, bool isH5RedirectByScanEnabled, bool isH5RedirectByClickEnabled, bool isH5RedirectByPickingPhotoEnabled)
        {
            //Checking the seconds before payment is positive and less than 10000.
            if (secondsBeforePayment < 0 || secondsBeforePayment >= 10000)
            {
                throw new PairingDomainException("跳转等待时间值必须为正整数且小于10000" + ". At AlipayPreference()");
            }

            //Check at lease one type of h5 redirect is enabled.
            if (!isH5RedirectByScanEnabled && !isH5RedirectByClickEnabled &&  !isH5RedirectByPickingPhotoEnabled)
            {
                throw new PairingDomainException("至少需开启一种H5跳转方式" + "。 At AlipayPreference()");
            }


            SecondsBeforePayment = secondsBeforePayment;
            IsAmountUnchangeable = isAmountUnchangeable;
            IsAccountUnchangeable = isAccountUnchangeable;
            IsH5RedirectByScanEnabled = isH5RedirectByScanEnabled;
            IsH5RedirectByClickEnabled = isH5RedirectByClickEnabled;
            IsH5RedirectByPickingPhotoEnabled = isH5RedirectByPickingPhotoEnabled;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return SecondsBeforePayment;
            yield return IsAmountUnchangeable;
            yield return IsAccountUnchangeable;
            yield return IsH5RedirectByScanEnabled;
            yield return IsH5RedirectByClickEnabled;
            yield return IsH5RedirectByPickingPhotoEnabled;
        }
    }
}
