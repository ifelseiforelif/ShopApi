using AutoMapper;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;

namespace Shop.Application.Services;

public class CategoryService(ICategoryRepository _repository, IMapper _mapper, ICachingService _cachingService) : ICategoryService
{
    //TODO: додати Automapper
    public async Task<int?> CreateCategoryAsync(CategoryCreateDTO dto)
    {
        var category = _mapper.Map<Category>(dto);
        return await _repository.AddCategoryAsync(category);
    }

    public async Task<CategoryReadDTO?> GetCategoryByIdAsync(int id)
    {
        CategoryReadDTO? dto = null;
        var category = await _repository.GetCategoryByIdAsync(id);
        if (category != null)
        {
            dto = _mapper.Map<CategoryReadDTO>(category);
        }
        return dto;
    }
    public async Task<List<CategoryReadDTO>?> GetAllCategoriesAsync()
    {
        string keyCaching = "Categories";
        var cache = await _cachingService.GetAsync<List<CategoryReadDTO>>(keyCaching);
        if (cache == null)
        {
            List<Category>? categories = await _repository.GetAllCategoriesAsync();
            
            if (categories != null && categories.Count > 0)
            {
                cache = _mapper.Map<List<CategoryReadDTO>>(categories);
                await _cachingService.SetAsync(keyCaching, cache, null);
            }
        }
        
        return cache;
    }
}
