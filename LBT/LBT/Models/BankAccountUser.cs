using System.ComponentModel.DataAnnotations.Schema;

namespace LBT.Models
{
    public class BankAccountUser
    {
        public int BankAccountUserId { get; set; }

        [Column("BankAccountId")]
        public virtual BankAccount BankAccount { get; set; }

        [ForeignKey("BankAccount")]
        public int BankAccountId { get; set; }

        [Column("UserId")]
        public virtual UserProfile User { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
    }
}