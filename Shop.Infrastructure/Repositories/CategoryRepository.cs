using Shop.Application.Interfaces.Repository;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories;

public class CategoryRepository(ShopDbContext _context) : ICategoryRepository
{
    public async Task<int?> AddCategoryAsync(Category category)
    {
        _context.Categories.Add(category);
        _context.SaveChangesAsync();
        return category.Id;
    }
}
