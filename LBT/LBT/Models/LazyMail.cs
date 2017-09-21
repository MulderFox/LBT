using System;
using System.ComponentModel.DataAnnotations;

namespace LBT.Models
{
    public class LazyMail
    {
        public int LazyMailId { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string TextBody { get; set; }

        public DateTime TimeToSend { get; set; }        
    }
}