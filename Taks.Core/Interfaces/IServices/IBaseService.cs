using System.Linq.Expressions;
using Taks.Core.Common;
using Taks.Core.Entities.ViewModels;

namespace Taks.Core.Interfaces.IServices
{
    public interface IBaseService<TViewModel>
        where TViewModel : class
    {
        Task<IEnumerable<TViewModel>> GetAll(CancellationToken cancellationToken);
        Task<PaginatedDataViewModel<TViewModel>> GetPaginatedData(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<PaginatedDataViewModel<TViewModel>> GetPaginatedData(int pageNumber, int pageSize, List<ExpressionFilter> filters, CancellationToken cancellationToken);
        Task<PaginatedDataViewModel<TViewModel>> GetPaginatedData(int pageNumber, int pageSize, List<ExpressionFilter> filters, string sortBy, string sortOrder, CancellationToken cancellationToken);
        Task<TViewModel> GetById<Tid>(Tid id, CancellationToken cancellationToken);
        Task<TViewModel> GetByIdPredicate<Tid>(Tid id, Expression<Func<TViewModel, bool>> expression, CancellationToken cancellationToken);
        Task<bool> IsExists<Tvalue>(string key, Tvalue value, CancellationToken cancellationToken);
        Task<bool> IsExistsForUpdate<Tid>(Tid id, string key, string value, CancellationToken cancellationToken);
    }
}
