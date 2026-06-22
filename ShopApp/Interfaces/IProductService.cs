using Shop.Domain.Models;

namespace Shop.App.Interfaces;

public interface IProductService
{
    List<Product> GetAllProducts();
    void AddProduct(Product product);
}
