using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LBT.Resources;

namespace LBT.Models
{
    public enum BankAccountType
    {
        ApplicationAccess = 0,
        TeamMeeting = 1,
        LgsOrMspEveningOrWorkshopsOrLeaders = 2,
        DavidKotasekTraining = 3,
        Others = 4
    }

    public class BankAccount
    {
        public int BankAccountId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Owner { get; set; }

        [Required]
        public BankAccountType BankAccountType { get; set; }

        [Required]
        public CurrencyType CurrencyType { get; set; }

        [NotMapped]
        public string BankAccountTypeLocalizedText { get { return GetApplicationAccessLongName(BankAccountType); } }

        [NotMapped]
        public string BankAccountTypeLocalizedAcronym
        {
            get
            {
                switch (BankAccountType)
                {
                    case BankAccountType.ApplicationAccess:
                        return ListItemsResource.BankAccountType_ApplicationAccess;

                    case BankAccountType.TeamMeeting:
                        return ListItemsResource.BankAccountType_TeamMeeting;

                    case BankAccountType.LgsOrMspEveningOrWorkshopsOrLeaders:
                        return ListItemsResource.BankAccountType_LgsOrMspEveningOrWorkshopsOrLeaders;

                    case BankAccountType.DavidKotasekTraining:
                        return ListItemsResource.BankAccountType_DavidKotasekTraining;

                    default:
                        return ListItemsResource.BankAccountType_Others;
                }
            }
        }

        [Required]
        public string AccountId { get; set; }

        [Required]
        [StringLength(128)]
        public string Token { get; set; }

        [Required]
        public DateTime ValidTo { get; set; }

        [Required]
        public DateTime TransactionStartDate { get; set; }

        public Int64? LastDownloadId { get; set; }

        public virtual ICollection<BankAccountHistory> BankAccountHistories { get; set; }

        public virtual ICollection<BankAccountUser> BankAccountUsers { get; set; }

        [Required]
        public string IBAN { get; set; }

        [Required]
        public string SWIFT { get; set; }

        public static Dictionary<BankAccountType, string> GetTranslationDictionaryForBankAccountType()
        {
            var translationDictionary = new Dictionary<BankAccountType, string>();
            foreach (BankAccountType bankAccountType in Enum.GetValues(typeof(BankAccountType)))
            {
                translationDictionary[bankAccountType] = GetApplicationAccessLongName(bankAccountType);
            }
            return translationDictionary;
        }

        private static string GetApplicationAccessLongName(BankAccountType bankAccountType)
        {
            switch (bankAccountType)
            {
                case BankAccountType.ApplicationAccess:
                    return ListItemsResource.BankAccountType_ApplicationAccess_LongName;

                case BankAccountType.TeamMeeting:
                    return ListItemsResource.BankAccountType_TeamMeeting_LongName;

                case BankAccountType.LgsOrMspEveningOrWorkshopsOrLeaders:
                    return ListItemsResource.BankAccountType_LgsOrMspEveningOrWorkshopsOrLeaders_LongName;

                case BankAccountType.DavidKotasekTraining:
                    return ListItemsResource.BankAccountType_DavidKotasekTraining_LongName;

                case BankAccountType.Others:
                    return ListItemsResource.BankAccountType_Others_LongName;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void CopyFrom(BankAccount bankAccount)
        {
            Title = bankAccount.Title;
            Owner = bankAccount.Owner;
            BankAccountType = bankAccount.BankAccountType;
            CurrencyType = bankAccount.CurrencyType;
            AccountId = bankAccount.AccountId;
            Token = bankAccount.Token;
            ValidTo = bankAccount.ValidTo;
            TransactionStartDate = bankAccount.TransactionStartDate;
            LastDownloadId = bankAccount.LastDownloadId;
            IBAN = bankAccount.IBAN;
            SWIFT = bankAccount.SWIFT;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool NeedResetLastDownloadId(BankAccount other)
        {
            bool needResetLastDownloadId = false;
            needResetLastDownloadId |= Token != other.Token;
            needResetLastDownloadId |= TransactionStartDate != other.TransactionStartDate;

            return needResetLastDownloadId;
        }
    }
}