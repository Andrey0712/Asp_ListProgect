using AppProductList.Data.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASP_ProductList.Models
{

    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public List<ProductImageItemVM> Images { get; set; }
    }

    public class ProductImageItemVM
    {
        public string Path { get; set; }
    }

    public class ProductImageToDelete
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public List<ProductImage> productViewModels { get; set; }
    }

    public class ProductImageToEdit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public List<ProductImage> productImages { get; set; }
        public List<IFormFile> Image { get; set; }//це для фото,які ми будемо додавати на вьюшці едіта.
        public List<string> delImage { get; set; }//це для фото,які ми будемо видаляти на вьюшці едіта.
    }


    public class AddProductViewModel
    {
        [Display(Name = "Назва")]
        public string Name { get; set; }
        [Display(Name = "Ціна")]
        public decimal Price { get; set; }
        [Display(Name = "Фото")]
        public List<IFormFile> Images { get; set; }
    }
    public class ProductOptionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
