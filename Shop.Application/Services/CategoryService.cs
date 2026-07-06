using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;

namespace Shop.Application.Services;

public class CategoryService(ICategoryRepository _repository) : ICategoryService
{
    //TODO: додати Automapper
    public async Task<int?> CreateCategoryAsync(CategoryCreateDTO dto)
    {
        return await _repository.AddCategoryAsync(new Category()
        {
            Name = dto.Name,
            Slug = dto.Slug,
            Url = dto.Url,
            ParentId = dto.ParentId,
        });
    }

    public async Task<CategoryReadDTO?> GetCategoryByIdAsync(int id)
    {
        CategoryReadDTO? dto = null;
        var category = await _repository.GetCategoryByIdAsync(id);
        if (category != null)
        {
            dto = new CategoryReadDTO()
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                Url = category.Url,
                IsActive = category.IsActive,
                ParentId = category.ParentId,
                Products = category.Products
                            .Select(p => p.Id)
                            .ToList()
            };


        }
        return dto;
    }
    public async Task<List<CategoryReadDTO>?> GetAllCategoriesAsync()
    {
        List<Category>? categories = await _repository.GetAllCategoriesAsync();
        List<CategoryReadDTO> dtos = null;
        if (categories != null && categories.Count > 0)
        {
            dtos = new List<CategoryReadDTO>();
            categories.ForEach(category =>
            {
                dtos.Add(new CategoryReadDTO()
                {
                    Id = category.Id,
                    Slug = category.Slug,
                    Name = category.Name,
                    Url = category.Url,
                    ParentId = category.ParentId
                });
            });
        }
        return dtos;
    }
}
