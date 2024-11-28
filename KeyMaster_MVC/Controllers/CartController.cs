using KeyMaster_MVC.Models;
using KeyMaster_MVC.Models.ViewModels;
using KeyMaster_MVC.Repository;
using Microsoft.AspNetCore.Mvc;

namespace KeyMaster_MVC.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        public CartController(DataContext context)
        {
            _dataContext = context;
        }

        public IActionResult Index()
        {
            List<CartItemModel> cartItems = HttpContext.Session.Getjson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemViewModel cartVN = new()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price)
            };
            return View(cartVN);
        }
        public IActionResult Checkout()
        {
            return View("~/Views/Checkout/Index.cshtml");
        }
        public async Task<IActionResult> Add(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            List<CartItemModel> Cart = HttpContext.Session.Getjson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = Cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItem == null)
            {
                Cart.Add(new CartItemModel(product));
            }
            else
            {
                cartItem.Quantity += 1;
            }
            HttpContext.Session.Setjson("Cart", Cart);

            TempData["success"] = "Add Item to Cart successfully";

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> Decrease(int Id)
        {
            List<CartItemModel> Cart = HttpContext.Session.Getjson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = Cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
            }
            else
            {
                Cart.RemoveAll(p => p.ProductId == Id);
            }
            if (Cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.Setjson("Cart", Cart);
            }
            TempData["success"] = "Decrease Item quantity to Cart successfully";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Increase(int Id)
        {
            List<CartItemModel> Cart = HttpContext.Session.Getjson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = Cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItem.Quantity >= 1)
            {
                ++cartItem.Quantity;
            }
            else
            {
                Cart.RemoveAll(p => p.ProductId == Id);
            }
            if (Cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.Setjson("Cart", Cart);
            }
            TempData["success"] = "Increase Item quantity to Cart successfully";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Remove(int Id)
        {
            List<CartItemModel> Cart = HttpContext.Session.Getjson<List<CartItemModel>>("Cart");
            Cart.RemoveAll(p => p.ProductId == Id);

            if (Cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.Setjson("Cart", Cart);
            }
            TempData["success"] = "Remove Item of Cart successfully";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Clear(int Id)
        {
            HttpContext.Session.Remove("Cart");
            TempData["success"] = "Clear all Item Cart successfully";
            return RedirectToAction("Index");
        }
    }
}
