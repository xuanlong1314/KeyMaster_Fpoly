using KeyMaster_MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KeyMaster_MVC.Repository
{
    public class SeedData
    {
        public static async Task SeedingDataAsync(DataContext _context, UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context.Database.Migrate();

            // Seed Brands và Products nếu chưa tồn tại
            if (!_context.Brands.Any())
            {
                CategoryModel dienthoai = new CategoryModel { Name = "dienthoai", Slug = "dienthoai", Description = "dienthoai is best", Status = 1 };
                CategoryModel MayTinh = new CategoryModel { Name = "MayTinh", Slug = "MayTinh", Description = "MayTinh GF65 is best", Status = 1 };

                BrandModel Apple = new BrandModel { Name = "Apple", Slug = "Apple", Description = "Apple is best", Status = 1 };
                BrandModel MSI = new BrandModel { Name = "MSI", Slug = "MSI", Description = "MSI is best", Status = 1 };

                _context.Products.AddRange(
                    new ProductModel { Name = "Iphone 12", Slug = "Iphone 12", Description = "Iphone 12 is best", Image = "1.jpg", Category = dienthoai, Brand = Apple, Price = 1000 },
                    new ProductModel { Name = "MSI GF65", Slug = "MSI GF65", Description = "MSI GF65 is best", Image = "2.jpg", Category = MayTinh, Brand = MSI, Price = 1000 }
                );

                _context.SaveChanges();
            }

            // Tạo role "Admin" nếu chưa tồn tại
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                var adminRole = new IdentityRole("Admin");
                await roleManager.CreateAsync(adminRole);
            }

            // Tạo user "admin" nếu chưa tồn tại
            var adminEmail = "admin@gmail.com";
            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin == null)
            {
                var adminUser = new AppUserModel
                {
                    UserName = "admin",
                    Email = adminEmail,
                    Occupation = "Administrator",
                    EmailConfirmed = true // Xác nhận email cho admin
                };

                var createResult = await userManager.CreateAsync(adminUser, "admin@123"); // Đặt mật khẩu mạnh
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin"); // Gán quyền Admin
                }
                else
                {
                    throw new Exception($"Error creating admin user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
