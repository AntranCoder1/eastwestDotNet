using System.ComponentModel.DataAnnotations;

namespace eastwest.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? name { get; set; }
        public string email { get; set; }
        public int? phone { get; set; }
        public string password { get; set; }
        public string? profile_image { get; set; }
        public string? reset_password_token { get; set; }
        public DateTime? last_login { get; set; }
        public DateTime? last_active { get; set; }
        public int? isInvite { get; set; }
        public int? verify { get; set; }
        public DateTime? created_at { get; set; } = DateTime.Now;
        public DateTime? updated_at { get; set; } = DateTime.Now;
        public int? isAdmin { get; set; }
    }
}
