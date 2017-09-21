using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LBT.Models
{
    public class ManualType
    {
        public int ManualTypeId { get; set; }

        [Required]
        [StringLength(128)]
        public string Title { get; set; }

        public int Order { get; set; }

        public virtual ICollection<Manual> Manuals { get; set; }

        public void CopyFrom(ManualType manualType)
        {
            Title = manualType.Title;
            Order = manualType.Order;
        }
    }
}