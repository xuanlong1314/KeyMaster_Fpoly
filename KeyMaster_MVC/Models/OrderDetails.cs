using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyMaster_MVC.Models
{
    public class OrderDetails
    {
        [Key]
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string UserName { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập Giá Sản Phẩm")]
        [Range(0.01, double.MaxValue)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }  // Khóa ngoại là long
        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }

    }
}
