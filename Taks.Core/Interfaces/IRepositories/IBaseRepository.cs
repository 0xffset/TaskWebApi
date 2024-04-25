using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Taks.Core.Common;
using Taks.Core.Entities.ViewModels;

namespace Taks.Core.Interfaces.IRepositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll(CancellationToken cancellationToken);
        Task<IEnumerable<T>> GetAll(List<Expression<Func<T, object>>> includeExpressions, CancellationToken cancellationToken = default);
        Task<PaginatedDataViewModel<T>> GetPaginatedData(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<PaginatedDataViewModel<T>> GetPaginatedData(int pageNumber, int pageSize, List<ExpressionFilter> filters, CancellationToken cancellationToken);
        Task<PaginatedDataViewModel<T>> GetPaginatedData(int pageNumber, int pageSize, List<ExpressionFilter> filters, string sortBy, string sortOrder, CancellationToken cancellationToken);
        Task<PaginatedDataViewModel<T>> GetPaginatedData(List<Expression<Func<T, object>>> includeExpressions, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<T> GetById<Tid>(Tid id, CancellationToken cancellationToken);
        Task<T> GetByIdPredicate<Tid>(Expression<Func<T,bool>> predicate, CancellationToken cancellationToken = default);
        Task<T> GetById<Tid>(List<Expression<Func<T, object>>> includeExpressions, Tid id, CancellationToken cancellationToken);
        Task<bool> IsExists<Tvalue>(string key, Tvalue value, CancellationToken cancellationToken);
        Task<bool> IsExistsForUpdate<Tid>(Tid id, string key, string value, CancellationToken cancellationToken);
        Task<T> Create(T model, CancellationToken cancellationToken);
        Task CreateRange(List<T> model, CancellationToken cancellationToken);
        Task Update(T model, CancellationToken cancellationToken);
        Task Delete(T model, CancellationToken cancellationToken);
        Task SaveChangeAsync(CancellationToken cancellationToken);
    }
}
