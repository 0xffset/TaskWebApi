using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taks.Core.Entities.General;

namespace Taks.Core.Interfaces.IRepositories
{
    public interface ICategoryRepository : IBaseRepository<TaskCategory>
    {
        Task<bool> CategoryExistById(int id);
    }
}
