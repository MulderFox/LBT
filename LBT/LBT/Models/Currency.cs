using LBT.Cache;
using LBT.DAL;
using LBT.Helpers;
using LBT.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace LBT.Models
{
    public enum CurrencyType
    {
        CZK = 0,
        EUR = 1,
        USD = 2
    }

    public class Currency
    {
        public int CurrencyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_Currency_Name", ResourceType = typeof(FieldResource))]
        public CurrencyType CurrencyType { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [RegularExpression(Regex.OnlyCzechNumbers, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RegexNumber_ErrorMessage")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RangeNumberPositive_ErrorMessage")]
        [Display(Name = "Currency_ExchangeRateToCZK_Name", ResourceType = typeof(FieldResource))]
        public decimal ExchangeRateToCZK { get; set; }

        public void CopyFrom(Currency currency)
        {
            ExchangeRateToCZK = currency.ExchangeRateToCZK;
        }

        public ModelStateDictionary Validate(DefaultContext db)
        {
            var modelStateDictionary = new ModelStateDictionary();

            Currency[] otherCurrencies = CurrencyCache.GetIndex(db, CurrencyId);
            ValidateUniqueCurrency(otherCurrencies, ref modelStateDictionary);
            ValidateExchangeRate(ref modelStateDictionary);

            return modelStateDictionary;
        }

        private void ValidateUniqueCurrency(IEnumerable<Currency> otherCurrencies, ref ModelStateDictionary modelStateDictionary)
        {
            if (otherCurrencies.All(oc => oc.CurrencyType != CurrencyType))
                return;

            modelStateDictionary.AddModelError(BaseCache.CurrencyTypeField, String.Format(ValidationResource.Global_OwnUnique_ErrorMessage, FieldResource.Global_Currency_Name));
        }

        private void ValidateExchangeRate(ref ModelStateDictionary modelStateDictionary)
        {
            if (ExchangeRateToCZK >= 0)
                return;

            modelStateDictionary.AddModelError(BaseCache.ExchangeRateToCZKField, String.Format(ValidationResource.Global_RangeNumberPositive_ErrorMessage, FieldResource.Currency_ExchangeRateToCZK_Name));
        }
    }
}