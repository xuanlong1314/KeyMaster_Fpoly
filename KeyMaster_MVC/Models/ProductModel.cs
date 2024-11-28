﻿using KeyMaster_MVC.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeyMaster_MVC.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập tên Sản Phẩm")]
        public string Name { get; set; }

        public string Slug { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập Mô tả Sản Phẩm")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập Giá Sản Phẩm")]
        [Range(0.01, double.MaxValue)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "Danh mục không hợp lệ")]
        public int CategoryId { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "Thương hiệu không hợp lệ")]
        public int BrandId { get; set; }


        public CategoryModel Category { get; set; }
        public BrandModel Brand { get; set; }
        public string Image { get; set; }

        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }
    }
}
