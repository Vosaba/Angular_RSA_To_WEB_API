using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string ZipCode { get; set; }
        [Required]
        public string Country { get; set; }
        public string Image { get; set; }
    }

    public class User
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Key_N { get; set; }
        public string Key_D { get; set; }
    }


}