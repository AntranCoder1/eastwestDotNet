using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eastwest.Models
{
    public class ProductImageModel
    {
        [Key]
        public int Id { get; set; }
        [Required]

        [ForeignKey("ProductModel")]
        public int productId { get; set; }
        public ProductModel ProductModel { get; set; }
        public string? image { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime updated_at { get; set; } = DateTime.Now;
    }
}
