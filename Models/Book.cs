using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectEcomerceFinal.Models
{
    [Table("Book")]
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string BookName { get; set; }

        public double? Price { get; set; }
        public string? Image { get; set; }

        [Required]
        public int? GenreId { get; set; }


        public Genre genre { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public List<CartDetail> CartDetails { get; set; }



    }
}
