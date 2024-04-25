using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taks.Core.Entities.ViewModels;

namespace Taks.Core.Interfaces.IServices
{
    public interface ITaskService : IBaseService<TaskViewModel>
    {
        Task<TaskViewModel> Create(TaskCreateViewModel model, CancellationToken cancellationToken);
        Task Update(TaskUpdateViewModel model, CancellationToken cancellationToken);
        Task Delete(int id, CancellationToken cancellationToken);

    }
}
