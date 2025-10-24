using SubManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubManager.Domain.Repositories
{
    public interface ICategoryRepository
    {
        /// <summary>
        /// Get all categories in the database
        /// </summary>
        /// <returns>A list of all categories.</returns>
        public Task<List<Category>> GetAllAsync();

        /// <summary>
        /// Get a category by its unique ID
        /// </summary>
        /// <param name="id">The ID of the category to retrieve.</param>
        /// <returns>The category if found; otherwise, null.</returns>
        public Task<Category?> GetByIdAsync(int id);
    }
}
