using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace ProjectEcomerceFinal.Models
{
    [Table("Order")]
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Required]
        public int OrderStatusId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public OrderStatus OrderStatus { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }




    }
}
