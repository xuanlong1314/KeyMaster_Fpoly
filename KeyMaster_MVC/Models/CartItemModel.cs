namespace KeyMaster_MVC.Models
{
    public class CartItemModel
    {
        public int ProductId { get; set; }  // Sửa từ long thành int
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public decimal Total
        {
            get { return Quantity * Price; }
        }
        public CartItemModel()
        {

        }
        public CartItemModel(ProductModel product)
        {
            ProductId = product.Id;  // Đồng bộ kiểu dữ liệu
            ProductName = product.Name;
            Quantity = 1;
            Price = product.Price;
            Image = product.Image;
        }

    }
}
