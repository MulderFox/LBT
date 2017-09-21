using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace LBT.Models
{
    public class BankAccountHistory
    {
        public int BankAccountHistoryId { get; set; }

        [ForeignKey("BankAccount")]
        public int BankAccountId { get; set; }

        [Column("BankAccountId")]
        public virtual BankAccount BankAccount { get; set; }

        public Int64 TransactionId { get; set; }

        public DateTime Date { get; set; }

        public decimal Ammount { get; set; }

        [NotMapped]
        public int AmmountView { get; set; }

        [StringLength(3)]
        public string Currency { get; set; }

        [NotMapped]
        public CurrencyType CurrencyType
        {
            get
            {
                CurrencyType currencyType;
                Enum.TryParse(Currency, out currencyType);
                return currencyType;
            }
        }

        [StringLength(255)]
        public string Exchange { get; set; }

        [StringLength(10)]
        public string BankCode { get; set; }

        [StringLength(255)]
        public string BankName { get; set; }

        public int? Ks { get; set; }

        public Int64? Vs { get; set; }

        [NotMapped]
        public string LyonessId
        {
            get
            {
                string vsString = Vs.GetValueOrDefault().ToString(CultureInfo.InvariantCulture);
                if (!Vs.HasValue || vsString.Length != 9)
                    return "000.000.000.000";

                string lyonessId = String.Format("{0}.000.{1}.{2}", vsString.Substring(0, 3), vsString.Substring(3, 3), vsString.Substring(6, 3));
                return lyonessId;
            }
        }

        public Int64? Ss { get; set; }

        public string Note { get; set; }

        public static Int64 GetVs(string lyonessId)
        {
            if (String.IsNullOrEmpty(lyonessId) || lyonessId.Length != 15)
                return 0;

            string vs = lyonessId.Replace(".", String.Empty).Remove(3, 3);
            Int64 vsNumber;
            return !Int64.TryParse(vs, out vsNumber) ? 0 : vsNumber;
        }
    }
}