using SubManager.Application.DTO.Category;

namespace SubManager.Application.Interfaces
{
    public interface ICategoryService
    {
        /// <summary>
        /// Get all categories, mapped to DTOs.
        /// </summary>
        /// <returns>A list of CategoryDto.</returns>
        public Task<List<CategoryDto>> GetAllCategoriesAsync();

        /// <summary>
        /// Get a specific category by its unique ID, mapped to a DTO.
        /// </summary>
        /// <param name="id">The ID of the category to retrieve.</param>
        /// <returns>The CategoryDto corresponding to the ID.</returns>
        public Task<CategoryDto> GetCategoryById(int id);
    }
}
