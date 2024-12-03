using KeyMaster_MVC.Areas.Admin.Repository;
using KeyMaster_MVC.Models;
using KeyMaster_MVC.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KeyMaster_MVC.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IEmailSender _emailSender;
        public CheckoutController(DataContext context,IEmailSender emailSender)
        {
            _dataContext = context;
            _emailSender = emailSender;
        }
        public async Task<IActionResult> Checkout()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var ordercode = Guid.NewGuid().ToString();
                var orderItems = new OrderModel();
                orderItems.OrderCode = ordercode;
                orderItems.UserName = userEmail;
                orderItems.Status = 1;
                orderItems.CreatedDate = DateTime.Now;
                _dataContext.Add(orderItems);
                _dataContext.SaveChanges();
                List<CartItemModel> cartItems = HttpContext.Session.Getjson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                foreach (var cart in cartItems)
                {
                    var orderdetails = new OrderDetails();
                    orderdetails.UserName = userEmail;
                    orderdetails.OrderCode = ordercode;
                    orderdetails.ProductId = cart.ProductId;
                    orderdetails.Price = cart.Price;
                    orderdetails.Quantity = cart.Quantity;
                    _dataContext.Add(orderdetails);
                    _dataContext.SaveChanges();
                }
                HttpContext.Session.Remove("Cart");
                //send email
                TempData["success"] = "Đăng nhập thành công";
                var receiver = userEmail;
                var subject = "Đặt hàng thành công";
                var message = "Đặt hàng thành công,vui lòng chờ nhận hàng";
                await _emailSender.SendEmailAsync(receiver, subject, message);

                TempData["Success"] = "Đơn hàng đã được tạo,vui lòng chờ duyệt đơn hàng";
                return RedirectToAction("Index", "Cart");

            }
            return View();
        }
    }
}
