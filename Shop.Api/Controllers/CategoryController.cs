using Microsoft.AspNetCore.Mvc;
using Shop.Api.Interfaces;
using Shop.Api.Requests.Categories;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Services;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")] //https://ip:port/api/v1
public class CategoryController(ICategoryService _categoryService, IImageService _imageService):ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromForm] CategoryCreateRequest dto)
    {
        string imageName = String.Empty;
        if(dto.Image!=null)
        {
            imageName = await _imageService.SaveFileAsync(dto.Image);
        }
        //TODO: Created status
        dto.Url = imageName;
        int? id = await _categoryService.CreateCategoryAsync(dto as CategoryCreateDTO);
        return Ok($"Category created {id}"); //200 status
    }
}
