using AppProductList.Data;
using AppProductList.Data.Entities;
using ASP_ProductList.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ASP_ProductList.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public EFAppContext _context { get; set; }

        public HomeController(ILogger<HomeController> logger, EFAppContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var model = _context.Products
                 .Include(i => i.ProductImages)
                 .Select(x => new ProductItemViewModel
                 {
                     Id = x.Id,
                     Name = x.Name,
                     Price = x.Price,
                     Images = x.ProductImages.Select(t => new ProductImageItemVM
                     {
                         Path = "/images/" + t.Name
                     }).ToList()
                 });

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductAddViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            Product product = new();
            product.Name = model.Name;
            product.Price = model.Price;

            _context.Products.Add(product);
            _context.SaveChanges();

            List<ProductImage> images = new List<ProductImage>();

            foreach (var item in model.Images)
            {
                string ext = Path.GetExtension(item.FileName);
                string fileName = Path.GetRandomFileName() + ext;

                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "products", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    item.CopyTo(stream);
                }

                ProductImage pImage = new ProductImage
                {
                    Name = fileName,
                    Product = product
                };
                images.Add(pImage);
            }
            _context.ProductImages.AddRange(images);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
