using KeyMaster_MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace KeyMaster_MVC.Repository
{
	public class SeedData
	{
		public static void SeedingData(DataContext _context)
		{
			_context.Database.Migrate();
			if (!_context.Brands.Any())
			{
				CategoryModel dienthoai = new CategoryModel { Name = "dienthoai", Slug = "dienthoai", Description = "dienthoai is best", Status = 1 };
				CategoryModel MayTinh = new CategoryModel { Name = "MayTinh", Slug = "MayTinh", Description = "MayTinh GF65 is best", Status = 1 };

				BrandModel Apple = new BrandModel { Name = "Apple", Slug = "Apple", Description = "Apple is best", Status = 1 };
				BrandModel MSI = new BrandModel { Name = "MSI", Slug = "MSI", Description = "MSI is best", Status = 1 };

				_context.Products.AddRange(
					new ProductModel { Name = "Iphone 12", Slug = "Iphone 12", Description = "Iphone 12 is best", Image = "1.jpg", Category = dienthoai,Brand=Apple ,Price = 1000 },
					new ProductModel { Name = "MSI GF65", Slug = "MSI GF65", Description = "MSI GF65 is best", Image = "2.jpg", Category = MayTinh,Brand=MSI, Price = 1000 }
				);
				_context.SaveChanges();
			}
		}
	}
}
