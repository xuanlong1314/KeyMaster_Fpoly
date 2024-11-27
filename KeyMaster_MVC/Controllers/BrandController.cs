using KeyMaster_MVC.Models;
using KeyMaster_MVC.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KeyMaster_MVC.Controllers
{
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;

        // Change the constructor to public
        public BrandController(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IActionResult> Index(string Slug = "")
        {
            BrandModel brand = _dataContext.Brands.Where(c => c.Slug == Slug).FirstOrDefault();
            if (brand == null) return RedirectToAction("Index");

            var productsByBrand = _dataContext.Products.Where(p => p.BrandId == brand.Id);
            return View(await productsByBrand.OrderByDescending(p => p.Id).ToListAsync());
        }
    }
}
