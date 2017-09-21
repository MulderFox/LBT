using System.ComponentModel.DataAnnotations;

namespace LBT.Models
{
    public class PropertiesBag
    {
        [Key]
        public string Key { get; set; }

        public string Value { get; set; }
    }
}