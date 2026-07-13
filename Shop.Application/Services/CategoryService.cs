using AutoMapper;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Configurations;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;

namespace Shop.Application.Services;

public class CategoryService(ICategoryRepository _repository, IMapper _mapper, IFilePathProvider _filePathProvider) : ICategoryService
{
    /// <summary>
    /// Метод формує "правильний" шлях до картинки
    /// </summary>
    /// <param name="dto"></param>
    private void FixedImageForCategory(CategoryReadDTO dto)
    {
        dto.Url = $"{_filePathProvider.Categories}/{dto.Url}";
    }
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
            FixedImageForCategory(dto);
        }
        return dto;
    }
    public async Task<List<CategoryReadDTO>?> GetAllCategoriesAsync()
    {
        List<Category>? categories = await _repository.GetAllCategoriesAsync();
        List<CategoryReadDTO>? dtos = null;
        if (categories != null && categories.Count > 0)
        {
            dtos = _mapper.Map<List<CategoryReadDTO>>(categories);
            dtos.ForEach(dto => FixedImageForCategory(dto));
        }
       
        return dtos;
    }

    public async Task<CategoryReadDTO?> GetCategoryBySlugAsync(string slug)
    {
        CategoryReadDTO? dto = null;
        var category = await _repository.GetCategoryBySlugAsync(slug);
        if (category != null)
        {
            dto = _mapper.Map<CategoryReadDTO>(category);
            FixedImageForCategory(dto);
        }
        return dto;
    }

    public async Task<List<CategoryReadDTO>?> GetAllCategoriesByParentIdAsync(int id)
    {
        var categories = await _repository.GetCategoriesByParentIdAsync(id);
        if (categories != null)
        {
            var dtos = _mapper.Map<List<CategoryReadDTO>>(categories);
            dtos.ForEach(dto => FixedImageForCategory(dto));
            return dtos;
        }
        return null;
    }
}
