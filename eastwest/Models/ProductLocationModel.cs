using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eastwest.Models
{
    public class ProductLocationModel
    {
        [Key]
        public int Id { get; set; }
        [Required]

        [ForeignKey("ProductModel")]
        public int productId { get; set; }
        public ProductModel Product { get; set; }

        [ForeignKey("LocationModel")]
        public int locationId { get; set; }
        public LocationModel Location { get; set; }

        public int? quantity { get; set; }

        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime updated_at { get; set; } = DateTime.Now;
        public string? skuProduct { get; set; }
        public string? locBarcode { get; }
    }
}
