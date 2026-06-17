using Microsoft.AspNetCore.Mvc;
using ShopDomain.Models;

namespace ShopApp.Controllers
{
    // http://localhost:port/api/product
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController:ControllerBase
    {
        private List<Product> _products = new();
        [HttpGet]
        public List<Product> GetProducts()
        {
            _products.Add(new Product()
            {
                Title = "Milk",
                Price = 40.9f
            });
            _products.Add(new Product()
            {
                Title = "Bread",
                Price = 30.5f
            });
            return _products;
        }
    }
}
