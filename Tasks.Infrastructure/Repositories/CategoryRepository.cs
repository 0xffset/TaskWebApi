using Microsoft.EntityFrameworkCore;
using Taks.Core.Entities.General;
using Taks.Core.Interfaces.IRepositories;
using Tasks.Infrastructure.Data;

namespace Tasks.Infrastructure.Repositories
{
    public class CategoryRepository : BaseRepository<TaskCategory>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<bool> CategoryExistById(int id)
        {
            return await _dbContext.TaskCategories.AnyAsync(x => x.Id == id);
        }
    }
}
