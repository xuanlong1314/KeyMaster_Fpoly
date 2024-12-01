using KeyMaster_MVC.Models;
using KeyMaster_MVC.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KeyMaster_MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Products.OrderByDescending(p => p.Id).Include(p => p.Category).Include(p => p.Brand).ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
            if (ModelState.IsValid)
            {
                //code thêm dữ liệu
                product.Slug = product.Name.Replace(" ", "-");
                var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "sản phẩm đã tồn tại");
                    return View(product);
                }
                if (product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    product.Image = imageName;

                }
                _dataContext.Add(product);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm sản phẩm thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Thêm sản phẩm không thành công";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            return View(product);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductModel product, int Id)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            // Retrieve the product from the database
            var existed_product = _dataContext.Products.Find(product.Id);

            if (existed_product == null)
            {
                // Handle the case where the product doesn't exist
                TempData["error"] = "Product not found.";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                // Set the Slug for the product
                product.Slug = product.Name.Replace(" ", "-");

                if (product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);

                    // Check if the product has an existing image to delete
                    if (!string.IsNullOrEmpty(existed_product.Image))
                    {
                        string oldfilePath = Path.Combine(uploadsDir, existed_product.Image);
                        try
                        {
                            if (System.IO.File.Exists(oldfilePath))
                            {
                                System.IO.File.Delete(oldfilePath);
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "An error occurred while deleting the product image.");
                            return View(product); // If delete fails, return to the view
                        }
                    }

                    // Save the new image
                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageUpload.CopyToAsync(fs);
                    }

                    existed_product.Image = imageName; // Update the product's image path
                }

                // Update the product properties
                existed_product.Name = product.Name;
                existed_product.Description = product.Description;
                existed_product.Price = product.Price;
                existed_product.CategoryId = product.CategoryId;
                existed_product.BrandId = product.BrandId;
                // ... Update other properties if needed

                // Update the product in the database
                _dataContext.Products.Update(existed_product);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Cập nhật sản phẩm thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Cập nhật sản phẩm không thành công";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }

            return View(product);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int Id)
        {
            // Tìm sản phẩm theo Id
            ProductModel product = await _dataContext.Products.FindAsync(Id);

            if (product == null)
            {
                return NotFound(); // Trả về thông báo nếu sản phẩm không tồn tại
            }

            string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");

            // Kiểm tra nếu product.Image không null hoặc rỗng
            if (!string.IsNullOrEmpty(product.Image))
            {
                string oldfilePath = Path.Combine(uploadsDir, product.Image);
                try
                {
                    if (System.IO.File.Exists(oldfilePath))
                    {
                        // Xóa file ảnh cũ
                        System.IO.File.Delete(oldfilePath);
                    }
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi để debug (không hiển thị chi tiết lỗi tới người dùng)
                    Console.WriteLine($"Error deleting file: {ex.Message}");

                    // Thêm thông báo lỗi để hiển thị trong giao diện (nếu cần)
                    ModelState.AddModelError("", "Có lỗi xảy ra khi xóa tệp hình ảnh của sản phẩm.");
                }
            }

            // Xóa sản phẩm khỏi cơ sở dữ liệu
            _dataContext.Products.Remove(product);
            await _dataContext.SaveChangesAsync();

            // Thêm thông báo thành công
            TempData["success"] = "Xóa sản phẩm thành công";

            // Chuyển hướng người dùng về trang Index
            return RedirectToAction("Index");
        }
    }
}
