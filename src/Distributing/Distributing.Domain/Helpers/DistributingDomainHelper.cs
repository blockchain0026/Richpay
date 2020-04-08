using Distributing.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Distributing.Domain.Helpers
{
    public static class DistributingDomainHelper
    {
        public static string GetMethodString()
        {
            return new StackTrace(true).GetFrame(1).GetMethod().Name;
        }

        public static void ValidateDecimalPoint(decimal value, int point)
        {
            if (decimal.Round(value, point) != value)
            {
                throw new DistributingDomainException("The transfer amount must not has more than 3 points" +
                    ". At " + "." + new StackTrace(true).GetFrame(1).GetMethod().Name + "()");
            }
        }
    }
}
