using LBT.Models;
using LBT.Resources;
using System;
using System.Linq;

namespace LBT.Helpers
{
    public static class CurrencyHelper
    {
        public static decimal ConvertTo(decimal fromAmount, CurrencyType fromCurrencyType, CurrencyType toCurrencyType, Currency[] currencies)
        {
            if (fromCurrencyType == toCurrencyType)
                return fromAmount;

            if ((fromCurrencyType != CurrencyType.CZK && currencies.All(c => c.CurrencyType != fromCurrencyType)) ||
                toCurrencyType != CurrencyType.CZK && currencies.All(c => c.CurrencyType != toCurrencyType))
            {
                throw new Exception(String.Format(ValidationResource.Currency_CannotProccessCurrencyChange_ErrorMessage, fromCurrencyType, toCurrencyType));
            }

            decimal amountTo = fromAmount;
            Currency currency;

            // Převod konta na CZK
            if (fromCurrencyType != CurrencyType.CZK)
            {
                currency = currencies.Single(c => c.CurrencyType == fromCurrencyType);
                amountTo = amountTo * currency.ExchangeRateToCZK;
            }

            // Převod konta na aktuální měnu
            if (toCurrencyType != CurrencyType.CZK)
            {
                currency = currencies.Single(c => c.CurrencyType == toCurrencyType);
                amountTo = amountTo / currency.ExchangeRateToCZK;
            }

            return amountTo;
        }

        public static bool TryConvertTo(decimal fromAmount, CurrencyType fromCurrencyType, CurrencyType toCurrencyType, Currency[] currencies, out decimal toAmount)
        {
            try
            {
                toAmount = ConvertTo(fromAmount, fromCurrencyType, toCurrencyType, currencies);
                return true;
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
                toAmount = default(decimal);
                return false;
            }
        }
    }
}