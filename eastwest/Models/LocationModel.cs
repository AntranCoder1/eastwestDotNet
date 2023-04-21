using System.ComponentModel.DataAnnotations;

namespace eastwest.Models
{
    public class LocationModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Loc_Barcodes { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime updated_at { get; set; } = DateTime.Now;
    }
}
