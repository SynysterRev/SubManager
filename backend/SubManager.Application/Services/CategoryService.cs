using SubManager.Application.DTO.Category;
using SubManager.Application.Exceptions;
using SubManager.Application.Extensions;
using SubManager.Application.Interfaces;
using SubManager.Domain.Repositories;

namespace SubManager.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepo;

        public CategoryService(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var Categories = await _categoryRepo.GetAllAsync();
            return Categories.Select(c => c.ToDto()).ToList();
        }

        public async Task<CategoryDto> GetCategoryById(int id)
        {
            var Category = await _categoryRepo.GetByIdAsync(id);
            if (Category == null)
            {
                throw new NotFoundException($"Category with ID {id} not found");
            }
            return Category.ToDto();
        }
    }
}
