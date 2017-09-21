using LBT.Models;
using LBT.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LBT.ModelViews
{
    public class BankAccountHistoryClaAccess : BaseModelView
    {
        public int BankAccountHistoryId { get; set; }

        public BankAccountDetails BankAccount { get; set; }

        [Editable(false)]
        [Display(Name = "Global_Date_Name", ResourceType = typeof(FieldResource))]
        public DateTime Date { get; set; }

        [Display(Name = "BankAccountHistory_Ammount_Name", ResourceType = typeof(FieldResource))]
        public decimal Ammount { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Range(0, Int32.MaxValue, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RangeIntPositive_ErrorMessage")]
        [Display(Name = "BankAccountHistory_Ammount_Name", ResourceType = typeof(FieldResource))]
        public int AmmountView { get; set; }

        [Editable(false)]
        [StringLength(3, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength2_ErrorMessage")]
        [Display(Name = "Global_Currency_Name", ResourceType = typeof(FieldResource))]
        public string Currency { get; set; }

        [Editable(false)]
        [StringLength(255, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        [Display(Name = "BankAccountHistory_Exchange_Name", ResourceType = typeof(FieldResource))]
        public string Exchange { get; set; }

        [Editable(false)]
        [StringLength(10, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        public string BankCode { get; set; }

        [Editable(false)]
        [StringLength(255, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_StringLength3_ErrorMessage")]
        public string BankName { get; set; }

        [Editable(false)]
        [Display(Name = "BankAccountHistory_Ks_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public int? Ks { get; set; }

        [Display(Name = "BankAccountHistory_Vs_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public Int64? Vs { get; set; }

        [Display(Name = "BankAccountHistory_Ss_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public Int64? Ss { get; set; }

        // TODO: Dodělat poznámku z bankovního účtu
        [Display(Name = "Global_Note_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Note { get; set; }

        public BankAccountHistoryClaAccess()
        {
            
        }

        public BankAccountHistoryClaAccess(BankAccountHistory bankAccountHistory)
        {
            BankAccountHistoryId = bankAccountHistory.BankAccountHistoryId;
            BankAccount = BankAccountDetails.GetModelView(bankAccountHistory.BankAccount);
            Date = bankAccountHistory.Date;
            Ammount = bankAccountHistory.Ammount;
            AmmountView = bankAccountHistory.AmmountView;
            Currency = bankAccountHistory.Currency;
            Exchange = bankAccountHistory.Exchange;
            BankCode = bankAccountHistory.BankCode;
            BankName = bankAccountHistory.BankName;
            Ks = bankAccountHistory.Ks;
            Vs = bankAccountHistory.Vs;
            Ss = bankAccountHistory.Ss;
            Note = bankAccountHistory.Note;
        }

        public static BankAccountHistoryClaAccess GetModelView(BankAccountHistory bankAccountHistory)
        {
            if (bankAccountHistory == null)
                return null;

            var bankAccountHistoryClaAccess = new BankAccountHistoryClaAccess(bankAccountHistory);
            return bankAccountHistoryClaAccess;
        }
    }

    public class BankAccountHistoryClaAccessIndex : BaseModelView
    {
        public int BankAccountHistoryId { get; set; }

        public BankAccountDetails BankAccount { get; set; }

        public DateTime Date { get; set; }

        public decimal Ammount { get; set; }

        public string Currency { get; set; }

        public CurrencyType CurrencyType
        {
            get
            {
                CurrencyType currencyType;
                Enum.TryParse(Currency, out currencyType);
                return currencyType;
            }
        }

        public Int64? Vs { get; set; }

        public BankAccountHistoryClaAccessIndex(BankAccountHistory bankAccountHistory)
        {
            BankAccountHistoryId = bankAccountHistory.BankAccountHistoryId;
            BankAccount = BankAccountDetails.GetModelView(bankAccountHistory.BankAccount);
            Date = bankAccountHistory.Date;
            Ammount = bankAccountHistory.Ammount;
            Currency = bankAccountHistory.Currency;
            Vs = bankAccountHistory.Vs;
        }

        public static BankAccountHistoryClaAccessIndex[] GetModelView(BankAccountHistory[] bankAccountHistories)
        {
            if (bankAccountHistories == null)
                return null;

            BankAccountHistoryClaAccessIndex[] accountHistoryClaAccessIndices = bankAccountHistories.Select(bah => new BankAccountHistoryClaAccessIndex(bah)).ToArray();
            return accountHistoryClaAccessIndices;
        }
    }

    public class BankAccountHistoryDetails : BaseModelView
    {
        public int BankAccountHistoryId { get; set; }

        public BankAccountDetails BankAccount { get; set; }

        [Display(Name = "Global_Date_Name", ResourceType = typeof(FieldResource))]
        public DateTime Date { get; set; }

        [Display(Name = "BankAccountHistory_Ammount_Name", ResourceType = typeof(FieldResource))]
        public decimal Ammount { get; set; }

        public string Currency { get; set; }

        [Display(Name = "BankAccountHistory_Exchange_Name", ResourceType = typeof(FieldResource))]
        public string Exchange { get; set; }

        public string BankCode { get; set; }

        public string BankName { get; set; }

        [Display(Name = "BankAccountHistory_Ks_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public int? Ks { get; set; }

        [Display(Name = "BankAccountHistory_Vs_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public Int64? Vs { get; set; }

        [Display(Name = "BankAccountHistory_Ss_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public Int64? Ss { get; set; }

        [Display(Name = "Global_Note_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Note { get; set; }

        public BankAccountHistoryDetails(BankAccountHistory bankAccountHistory)
        {
            BankAccountHistoryId = bankAccountHistory.BankAccountHistoryId;
            BankAccount = BankAccountDetails.GetModelView(bankAccountHistory.BankAccount);
            Date = bankAccountHistory.Date;
            Ammount = bankAccountHistory.Ammount;
            Currency = bankAccountHistory.Currency;
            Exchange = bankAccountHistory.Exchange;
            BankCode = bankAccountHistory.BankCode;
            BankName = bankAccountHistory.BankName;
            Ks = bankAccountHistory.Ks;
            Vs = bankAccountHistory.Vs;
            Ss = bankAccountHistory.Ss;
            Note = bankAccountHistory.Note;
        }

        public static BankAccountHistoryDetails GetModelView(BankAccountHistory bankAccountHistory)
        {
            if (bankAccountHistory == null)
                return null;

            var bankAccountHistoryDetails = new BankAccountHistoryDetails(bankAccountHistory);
            return bankAccountHistoryDetails;
        }
    }

    public class BankAccountHistoryEdit : BaseModelView
    {
        public int BankAccountHistoryId { get; set; }

        public BankAccountDetails BankAccount { get; set; }

        [Display(Name = "Global_Date_Name", ResourceType = typeof(FieldResource))]
        public DateTime Date { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_Required_ErrorMessage")]
        [Range(0, Int32.MaxValue, ErrorMessageResourceType = typeof(ValidationResource), ErrorMessageResourceName = "Global_RangeIntPositive_ErrorMessage")]
        [Display(Name = "BankAccountHistory_Ammount_Name", ResourceType = typeof(FieldResource))]
        public int AmmountView { get; set; }

        public string Currency { get; set; }

        [Display(Name = "BankAccountHistory_Exchange_Name", ResourceType = typeof(FieldResource))]
        public string Exchange { get; set; }

        public string BankCode { get; set; }

        public string BankName { get; set; }

        [Display(Name = "BankAccountHistory_Ks_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public int? Ks { get; set; }

        [Display(Name = "BankAccountHistory_Vs_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public Int64? Vs { get; set; }

        [Display(Name = "BankAccountHistory_Ss_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public Int64? Ss { get; set; }

        // TODO: Dodělat poznámku z bankovního účtu
        [Display(Name = "Global_Note_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Note { get; set; }

        public BankAccountHistoryEdit()
        {
            
        }

        public BankAccountHistoryEdit(BankAccountHistory bankAccountHistory)
        {
            BankAccountHistoryId = bankAccountHistory.BankAccountHistoryId;
            BankAccount = BankAccountDetails.GetModelView(bankAccountHistory.BankAccount);
            Date = bankAccountHistory.Date;
            AmmountView = bankAccountHistory.AmmountView;
            Currency = bankAccountHistory.Currency;
            Exchange = bankAccountHistory.Exchange;
            BankCode = bankAccountHistory.BankCode;
            BankName = bankAccountHistory.BankName;
            Ks = bankAccountHistory.Ks;
            Vs = bankAccountHistory.Vs;
            Ss = bankAccountHistory.Ss;
            Note = bankAccountHistory.Note;
        }

        public static BankAccountHistoryEdit GetModelView(BankAccountHistory bankAccountHistory)
        {
            if (bankAccountHistory == null)
                return null;

            var bankAccountHistoryEdit = new BankAccountHistoryEdit(bankAccountHistory);
            return bankAccountHistoryEdit;
        }
    }

    public class BankAccountHistoryIndex : BaseModelView
    {
        public int BankAccountHistoryId { get; set; }

        public BankAccountDetails BankAccount { get; set; }

        public DateTime Date { get; set; }

        public decimal Ammount { get; set; }

        public string Currency { get; set; }

        public Int64? Vs { get; set; }

        public Int64? Ss { get; set; }

        public BankAccountHistoryIndex(BankAccountHistory bankAccountHistory)
        {
            BankAccountHistoryId = bankAccountHistory.BankAccountHistoryId;
            BankAccount = BankAccountDetails.GetModelView(bankAccountHistory.BankAccount);
            Date = bankAccountHistory.Date;
            Ammount = bankAccountHistory.Ammount;
            Currency = bankAccountHistory.Currency;
            Vs = bankAccountHistory.Vs;
            Ss = bankAccountHistory.Ss;
        }

        public static BankAccountHistoryIndex[] GetModelView(BankAccountHistory[] bankAccountHistories)
        {
            if (bankAccountHistories == null)
                return null;

            BankAccountHistoryIndex[] accountHistoryIndices = bankAccountHistories.Select(bah => new BankAccountHistoryIndex(bah)).ToArray();
            return accountHistoryIndices;
        }
    }

    public class BankAccountHistoryDelete : BaseModelView
    {
        [Display(Name = "Global_Date_Name", ResourceType = typeof(FieldResource))]
        public DateTime Date { get; set; }

        [Display(Name = "BankAccountHistory_Ammount_Name", ResourceType = typeof(FieldResource))]
        public decimal Ammount { get; set; }

        public string Currency { get; set; }

        [Display(Name = "BankAccountHistory_Exchange_Name", ResourceType = typeof(FieldResource))]
        public string Exchange { get; set; }

        public string BankCode { get; set; }

        public string BankName { get; set; }

        [Display(Name = "BankAccountHistory_Ks_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public int? Ks { get; set; }

        [Display(Name = "BankAccountHistory_Vs_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public Int64? Vs { get; set; }

        [Display(Name = "BankAccountHistory_Ss_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public Int64? Ss { get; set; }

        [Display(Name = "Global_Note_Name", ResourceType = typeof(FieldResource))]
        [DisplayFormat(NullDisplayText = NullDisplayText)]
        public string Note { get; set; }

        public BankAccountHistoryDelete(BankAccountHistory bankAccountHistory)
        {
            Date = bankAccountHistory.Date;
            Ammount = bankAccountHistory.Ammount;
            Currency = bankAccountHistory.Currency;
            Exchange = bankAccountHistory.Exchange;
            BankCode = bankAccountHistory.BankCode;
            BankName = bankAccountHistory.BankName;
            Ks = bankAccountHistory.Ks;
            Vs = bankAccountHistory.Vs;
            Ss = bankAccountHistory.Ss;
            Note = bankAccountHistory.Note;
        }

        public static BankAccountHistoryDelete GetModelView(BankAccountHistory bankAccountHistory)
        {
            if (bankAccountHistory == null)
                return null;

            var bankAccountHistoryDelete = new BankAccountHistoryDelete(bankAccountHistory);
            return bankAccountHistoryDelete;
        }
    }
}