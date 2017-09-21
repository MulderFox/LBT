using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LBT.Models
{
    public class Manual
    {
        public int ManualId { get; set; }

        [ForeignKey("ManualType")]
        public int ManualTypeId { get; set; }

        [Column("ManualTypeId")]
        public virtual ManualType ManualType { get; set; }

        [Required]
        [StringLength(128)]
        public string Title { get; set; }

        public int Order { get; set; }

        [Required]
        public string RelativeFilePath { get; set; }

        public bool IsDownloadable { get; set; }

        public bool IsAccessForAuthGuest { get; set; }

        public void CopyFrom(Manual manual)
        {
            ManualTypeId = manual.ManualTypeId;
            Title = manual.Title;

            if (!String.IsNullOrEmpty(manual.RelativeFilePath))
            {
                RelativeFilePath = manual.RelativeFilePath;
            }

            Order = manual.Order;
            IsDownloadable = manual.IsDownloadable;
            IsAccessForAuthGuest = manual.IsAccessForAuthGuest;
        }
    }
}