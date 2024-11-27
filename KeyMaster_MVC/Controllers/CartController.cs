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
            List<CartItemModel> cartItems = HttpContext.Session.Getjson<List<CartItemModel>>("cart") ?? new List<CartItemModel>();
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
            List<CartItemModel> cart = HttpContext.Session.Getjson<List<CartItemModel>>("cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItem == null)
            {
                cart.Add(new CartItemModel(product));
            }
            else
            {
                cartItem.Quantity += 1;
            }
            HttpContext.Session.Setjson("cart", cart);

            TempData["success"] = "Add Item to cart successfully";

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> Decrease(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.Getjson<List<CartItemModel>>("cart");
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(p => p.ProductId == Id);
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("cart");
            }
            else
            {
                HttpContext.Session.Setjson("cart", cart);
            }
            TempData["success"] = "Decrease Item quantity to cart successfully";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Increase(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.Getjson<List<CartItemModel>>("cart");
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItem.Quantity >= 1)
            {
                ++cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(p => p.ProductId == Id);
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("cart");
            }
            else
            {
                HttpContext.Session.Setjson("cart", cart);
            }
            TempData["success"] = "Increase Item quantity to cart successfully";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Remove(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.Getjson<List<CartItemModel>>("cart");
            cart.RemoveAll(p => p.ProductId == Id);

            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("cart");
            }
            else
            {
                HttpContext.Session.Setjson("cart", cart);
            }
            TempData["success"] = "Remove Item of cart successfully";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Clear(int Id)
        {
            HttpContext.Session.Remove("cart");
            TempData["success"] = "Clear all Item cart successfully";
            return RedirectToAction("Index");
        }
    }
}
