using Shop.Api.Interfaces;
using Shop.Domain.Models;

namespace Shop.Api.Services;

public class ProductService : IProductService
{
    private List<Product> _products = new();
    public ProductService()
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
    }

    public void AddProduct(Product product)
    {
        _products.Add(product);
    }

    public List<Product> GetAllProducts()
    {
       
        return _products;
    }
}
