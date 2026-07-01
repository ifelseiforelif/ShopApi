using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Services;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")] //https://ip:port/api/v1
public class CategoryController(ICategoryService _categoryService):ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDTO dto)
    {
        //TODO: Created status
        int? id = await _categoryService.CreateCategoryAsync(dto);
        return Ok($"Category created {id}"); //200 status
    }
}
