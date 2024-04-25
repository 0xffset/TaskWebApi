using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taks.Core.Entities.General;
using Task = Taks.Core.Entities.General.Task;

namespace Taks.Core.Interfaces.IRepositories
{
    public interface ITaskRepository : IBaseRepository<Task>
    {
    }
}
