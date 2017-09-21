using System.Globalization;
using LBT.Cache;
using LBT.DAL;
using LBT.Helpers;
using LBT.Models;
using LBT.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace LBT.ModelViews
{
    public class BankAccountCommon : BaseModelView
    {
        public static string GetInvoice(DefaultContext db, UserProfile userProfile, BankAccount bankAccountForClaAccess, DateTime currentDate, out string invoiceNumber)
        {
            invoiceNumber = PropertiesBagCache.GetClaAccessInvoiceNumber(db);

            var vocabulary = new Dictionary<string, string>
                                 {
                                     {"InvoiceNumber", invoiceNumber},
                                     {"VS", BankAccountHistory.GetVs(userProfile.LyonessId).ToString(CultureInfo.InvariantCulture)},
                                     {"ICO", userProfile.ICO},
                                     {"DIC", userProfile.DIC},
                                     {"Address", String.Format("{0} {1}\n{2}\n{3} {4}", userProfile.FirstName, userProfile.LastName, userProfile.Address, userProfile.PSC, userProfile.City)},
                                     {"AccountId", bankAccountForClaAccess.AccountId},
                                     {"IBAN", bankAccountForClaAccess.IBAN},
                                     {"SWIFT", bankAccountForClaAccess.SWIFT},
                                     {"Date", currentDate.ToString("dd.MM.yyyy")},
                                     {"Currency", userProfile.ClaAccessCurrency.ToString()},
                                     {"Price", userProfile.ClaAccessYearlyAccess.ToString(CultureInfo.InvariantCulture)}
                                 };

            string invoice = ProcessVocabulary(Properties.Resources.Invoice, vocabulary);
            return invoice;
        }
    }

    public class BankAccountIndex : BaseModelView
    {
        public int BankAccountId { get; set; }

        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        public string BankAccountTypeLocalizedAcronym { get; set; }

        [Display(Name = "Global_Currency_Name", ResourceType = typeof(FieldResource))]
        public CurrencyType CurrencyType { get; set; }

        [Display(Name = "BankAccount_ValidTo_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime ValidTo { get; set; }

        public BankAccountIndex(BankAccount bankAccount)
        {
            BankAccountId = bankAccount.BankAccountId;
            Title = bankAccount.Title;
            BankAccountTypeLocalizedAcronym = bankAccount.BankAccountTypeLocalizedAcronym;
            CurrencyType = bankAccount.CurrencyType;
            ValidTo = bankAccount.ValidTo;
        }

        public static BankAccountIndex[] GetIndexRows(DefaultContext db, string sortOrder)
        {
            BankAccount[] bankAccounts = BankAccountCache.GetIndex(db, sortOrder);

            var bankAccountIndices = new BankAccountIndex[bankAccounts.Length];
            for (int i = 0; i < bankAccounts.Length; i++)
            {
                bankAccountIndices[i] = new BankAccountIndex(bankAccounts[i]);
            }

            return bankAccountIndices;
        }
    }

    public class BankAccountEdit : BaseModelView
    {
        public int BankAccountId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "BankAccount_Owner_Name", ResourceType = typeof(FieldResource))]
        public string Owner { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "BankAccount_BankAccountType_Name", ResourceType = typeof(FieldResource))]
        public BankAccountType BankAccountType { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "BankAccount_AccountId_Name", ResourceType = typeof(FieldResource))]
        public string AccountId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "Global_Currency_Name", ResourceType = typeof(FieldResource))]
        public CurrencyType CurrencyType { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [StringLength(128, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "BankAccount_Token_Name", ResourceType = typeof(FieldResource))]
        public string Token { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Display(Name = "BankAccount_ValidTo_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ValidTo { get; set; }

        public DateTime TransactionStartDate { get; set; }

        [Display(Name = "BankAccount_BankAccountUsers_Name", ResourceType = typeof(FieldResource))]
        public int[] UserIds { get; set; }

        public bool SaveOnlyUsersOrToken { get; set; }

        [Display(Name = "BankAccount_IBAN_Name", ResourceType = typeof(FieldResource))]
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        public string IBAN { get; set; }

        [Display(Name = "BankAccount_SWIFT_Name", ResourceType = typeof(FieldResource))]
        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        public string SWIFT { get; set; }

        public BankAccountEdit()
        {

        }

        public BankAccountEdit(BankAccount bankAccount)
        {
            BankAccountId = bankAccount.BankAccountId;
            Title = bankAccount.Title;
            Owner = bankAccount.Owner;
            BankAccountType = bankAccount.BankAccountType;
            AccountId = bankAccount.AccountId;
            CurrencyType = bankAccount.CurrencyType;
            Token = bankAccount.Token;
            ValidTo = bankAccount.ValidTo;
            TransactionStartDate = bankAccount.TransactionStartDate;
            UserIds = bankAccount.BankAccountUsers.Select(bau => bau.UserId).ToArray();
            IBAN = bankAccount.IBAN;
            SWIFT = bankAccount.SWIFT;
        }

        public static BankAccountEdit GetModelView(BankAccount bankAccount)
        {
            if (bankAccount == null)
                return null;

            var bankAccountEdit = new BankAccountEdit(bankAccount);
            return bankAccountEdit;
        }

        public BankAccount GetModel(out int[] userIds, out bool saveOnlyUsersOrToken)
        {
            userIds = UserIds ?? new int[0];
            saveOnlyUsersOrToken = SaveOnlyUsersOrToken;

            var bankAccount = new BankAccount
            {
                BankAccountId = BankAccountId,
                Title = Title,
                Owner = Owner,
                BankAccountType = BankAccountType,
                AccountId = AccountId,
                CurrencyType = CurrencyType,
                Token = Token,
                TransactionStartDate = TransactionStartDate,
                ValidTo = ValidTo,
                IBAN = IBAN,
                SWIFT = SWIFT
            };
            return bankAccount;
        }

        public ModelStateDictionary Validate(DefaultContext db)
        {
            var modelStateDictionary = new ModelStateDictionary();

            ValidateTransactionStartDate();
            Meeting[] meetings = MeetingCache.GetIndex(db, BankAccountId);
            ValidateLinkedMeeting(meetings);

            BankAccount[] otherBankAccounts = BankAccountCache.GetIndex(db, BankAccountId);
            ValidateToken(otherBankAccounts, ref modelStateDictionary);
            ValidateValidTo(ref modelStateDictionary);

            return modelStateDictionary;
        }

        private void ValidateTransactionStartDate()
        {
            if (TransactionStartDate != default(DateTime))
                return;

            TransactionStartDate = DateTime.Now;
        }

        private void ValidateLinkedMeeting(Meeting[] meetings)
        {
            SaveOnlyUsersOrToken = meetings.Length > 0;
        }

        private void ValidateToken(IEnumerable<BankAccount> otherBankAccounts, ref ModelStateDictionary modelStateDictionary)
        {
            if (!String.IsNullOrEmpty(Token))
            {
                Token = Token.Trim();
            }

            if (String.IsNullOrEmpty(Token))
            {
                modelStateDictionary.AddModelError(BaseCache.TokenField, String.Format(ValidationResource.Global_Required_ErrorMessage, FieldResource.BankAccount_Token_Name));
                return;
            }

            if (otherBankAccounts.Any(oba => oba.Token.Equals(Token, StringComparison.InvariantCultureIgnoreCase)))
            {
                modelStateDictionary.AddModelError(BaseCache.TokenField, String.Format(ValidationResource.Global_Unique_ErrorMesage, FieldResource.BankAccount_Token_Name));
            }

            string bankTransactionUrl = String.Format(Properties.Settings.Default.BankTransactionUrl, Token);
            InternetResponse internetResponse = InternetRequest.SendRequest(bankTransactionUrl, InternetRequestType.OnlyStatusCode);
            if (internetResponse.HttpStatusCode != HttpStatusCode.OK)
            {
                modelStateDictionary.AddModelError(BaseCache.TokenField, String.Format(ValidationResource.Global_Valid_ErrorMessage, FieldResource.BankAccount_Token_Name));
            }
        }

        private void ValidateValidTo(ref ModelStateDictionary modelStateDictionary)
        {
            if (ValidTo >= DateTime.Now.Date)
                return;

            modelStateDictionary.AddModelError(BaseCache.ValidToField, String.Format(ValidationResource.Account_DateGreaterOrEqualThenNow_ErrorMessage, FieldResource.BankAccount_ValidTo_Name));
        }
    }

    public class BankAccountDetails : BaseModelView
    {
        public int BankAccountId { get; set; }

        [Display(Name = "Global_Title_Name", ResourceType = typeof(FieldResource))]
        public string Title { get; set; }

        [Display(Name = "BankAccount_Owner_Name", ResourceType = typeof(FieldResource))]
        public string Owner { get; set; }

        [Display(Name = "Global_Currency_Name", ResourceType = typeof(FieldResource))]
        public CurrencyType CurrencyType { get; set; }

        [Display(Name = "BankAccount_BankAccountType_Name", ResourceType = typeof(FieldResource))]
        public string BankAccountTypeLocalizedText { get; set; }

        public string BankAccountTypeLocalizedAcronym { get; set; }

        [Display(Name = "BankAccount_AccountId_Name", ResourceType = typeof(FieldResource))]
        public string AccountId { get; set; }

        [Display(Name = "BankAccount_Token_Name", ResourceType = typeof(FieldResource))]
        public string Token { get; set; }

        [Display(Name = "BankAccount_ValidTo_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ValidTo { get; set; }

        [Display(Name = "BankAccount_IBAN_Name", ResourceType = typeof(FieldResource))]
        public string IBAN { get; set; }

        [Display(Name = "BankAccount_SWIFT_Name", ResourceType = typeof(FieldResource))]
        public string SWIFT { get; set; }

        public BankAccountDetails(BankAccount bankAccount)
        {
            BankAccountId = bankAccount.BankAccountId;
            Title = bankAccount.Title;
            Owner = bankAccount.Owner;
            CurrencyType = bankAccount.CurrencyType;
            BankAccountTypeLocalizedText = bankAccount.BankAccountTypeLocalizedText;
            BankAccountTypeLocalizedAcronym = bankAccount.BankAccountTypeLocalizedAcronym;
            AccountId = bankAccount.AccountId;
            Token = bankAccount.Token;
            ValidTo = bankAccount.ValidTo;
            IBAN = bankAccount.IBAN;
            SWIFT = bankAccount.SWIFT;
        }

        public static BankAccountDetails GetModelView(BankAccount bankAccount)
        {
            if (bankAccount == null)
                return null;

            var bankAccountDetails = new BankAccountDetails(bankAccount);
            return bankAccountDetails;
        }
    }
}