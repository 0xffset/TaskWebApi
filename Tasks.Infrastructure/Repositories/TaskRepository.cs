using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taks.Core.Entities.General;
using Taks.Core.Interfaces.IRepositories;
using Tasks.Infrastructure.Data;
using Task = Taks.Core.Entities.General.Task;

namespace Tasks.Infrastructure.Repositories
{
    public class TaskRepository : BaseRepository<Task>, ITaskRepository
    {
        public TaskRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }


    }
}
