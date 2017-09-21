using LBT.Cache;
using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using LBT.Resources;

namespace LBT.ModelViews
{
    public class PopulatedCurrency : BaseModelView
    {
        public CurrencyType CurrencyType { get; set; }

        public string Title { get; set; }

        public string ClassName { get { return String.Format("ClaAccessChangedInfoList ClaAccessChangedInfo_{0}", CurrencyType); } }

        public string AccountId { get; set; }

        public string IBAN { get; set; }

        public string SWIFT { get; set; }

        private PopulatedCurrency()
        {
        }

        public static PopulatedCurrency[] GetClaAccessChangedCurrency(DefaultContext db, decimal fromAmount, CurrencyType fromCurrencyType)
        {
            var populatedCurrencies = new List<PopulatedCurrency>();

            Currency[] currencies = CurrencyCache.GetIndex(db);
            if (fromCurrencyType != CurrencyType.CZK && currencies.All(c => c.CurrencyType != fromCurrencyType))
                return populatedCurrencies.ToArray();

            BankAccount[] bankAccounts = BankAccountCache.GetUniqueAccessAccounts(db);
            if (currencies.Any(c => bankAccounts.All(ba => ba.CurrencyType != c.CurrencyType)))
                return populatedCurrencies.ToArray();

            // Přidání CZK
            decimal amountTo;
            CurrencyHelper.TryConvertTo(fromAmount, fromCurrencyType, CurrencyType.CZK, currencies, out amountTo);
            BankAccount bankAccount = bankAccounts.Single(ba => ba.CurrencyType == CurrencyType.CZK);
            string title = String.Format(ListItemsResource.Currency_CZKItemTitle, CurrencyType.CZK, amountTo);
            var populatedCurrency = new PopulatedCurrency
                                        {
                                            CurrencyType = CurrencyType.CZK,
                                            Title = title,
                                            AccountId = bankAccount.AccountId,
                                            SWIFT = bankAccount.SWIFT,
                                            IBAN = bankAccount.IBAN
                                        };
            populatedCurrencies.Add(populatedCurrency);

            // Přidání ostatních měn
            foreach (Currency currency in currencies)
            {
                CurrencyHelper.TryConvertTo(fromAmount, fromCurrencyType, currency.CurrencyType, currencies, out amountTo);
                bankAccount = bankAccounts.Single(ba => ba.CurrencyType == currency.CurrencyType);
                title = String.Format(ListItemsResource.Currency_OtherItemTitle, currency.CurrencyType, currency.ExchangeRateToCZK, CurrencyType.CZK, amountTo);
                populatedCurrency = new PopulatedCurrency
                                        {
                                            CurrencyType = currency.CurrencyType,
                                            Title = title,
                                            AccountId = bankAccount.AccountId,
                                            IBAN = bankAccount.IBAN,
                                            SWIFT = bankAccount.SWIFT
                                        };
                populatedCurrencies.Add(populatedCurrency);
            }

            return populatedCurrencies.ToArray();
        }
    }
}