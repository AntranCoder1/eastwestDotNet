using System.ComponentModel.DataAnnotations;

namespace eastwest.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? SKU_product { get; set; }
        public string? Product_Name { get; set; }
        public string? UPC { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime updated_at { get; set; } = DateTime.Now;
    }
}
