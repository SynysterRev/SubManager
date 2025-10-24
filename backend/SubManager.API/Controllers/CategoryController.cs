using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubManager.Application.DTO.Category;
using SubManager.Application.Interfaces;

namespace SubManager.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryDto>>> GetAllCategories()
        {
            return await _categoryService.GetAllCategoriesAsync();
        }
    }
}
