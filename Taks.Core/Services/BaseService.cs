using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Taks.Core.Common;
using Taks.Core.Entities.ViewModels;
using Taks.Core.Interfaces.IMapper;
using Taks.Core.Interfaces.IRepositories;
using Taks.Core.Interfaces.IServices;

namespace Taks.Core.Services
{
    public class BaseService<T, TViewModel> : IBaseService<TViewModel>
       where T : class
       where TViewModel : class
    {
        private readonly IBaseMapper<T, TViewModel> _viewModelMapper;
        private readonly IBaseRepository<T> _repository;

        public BaseService(
            IBaseMapper<T, TViewModel> viewModelMapper,
            IBaseRepository<T> repository)
        {
            _viewModelMapper = viewModelMapper;
            _repository = repository;
        }

        public virtual async Task<IEnumerable<TViewModel>> GetAll(CancellationToken cancellationToken)
        {
            return _viewModelMapper.MapList(await _repository.GetAll(cancellationToken));
        }

        public virtual async Task<PaginatedDataViewModel<TViewModel>> GetPaginatedData(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var paginatedData = await _repository.GetPaginatedData(pageNumber, pageSize, cancellationToken);
            var mappedData = _viewModelMapper.MapList(paginatedData.Data);
            var paginatedDataViewModel = new PaginatedDataViewModel<TViewModel>(mappedData.ToList(), paginatedData.TotalCount);
            return paginatedDataViewModel;
        }

        public virtual async Task<PaginatedDataViewModel<TViewModel>> GetPaginatedData(int pageNumber, int pageSize, List<ExpressionFilter> filters, CancellationToken cancellationToken)
        {
            var paginatedData = await _repository.GetPaginatedData(pageNumber, pageSize, filters, cancellationToken);
            var mappedData = _viewModelMapper.MapList(paginatedData.Data);
            var paginatedDataViewModel = new PaginatedDataViewModel<TViewModel>(mappedData.ToList(), paginatedData.TotalCount);
            return paginatedDataViewModel;
        }

        public virtual async Task<PaginatedDataViewModel<TViewModel>> GetPaginatedData(int pageNumber, int pageSize, List<ExpressionFilter> filters, string sortBy, string sortOrder, CancellationToken cancellationToken)
        {
            var paginatedData = await _repository.GetPaginatedData(pageNumber, pageSize, filters, sortBy, sortOrder, cancellationToken);
            var mappedData = _viewModelMapper.MapList(paginatedData.Data);
            var paginatedDataViewModel = new PaginatedDataViewModel<TViewModel>(mappedData.ToList(), paginatedData.TotalCount);
            return paginatedDataViewModel;
        }

        public virtual async Task<TViewModel> GetById<Tid>(Tid id, CancellationToken cancellationToken)
        {
            return _viewModelMapper.MapModel(await _repository.GetById(id, cancellationToken));
        }

        public virtual async Task<bool> IsExists<Tvalue>(string key, Tvalue value, CancellationToken cancellationToken)
        {
            return await _repository.IsExists(key, value?.ToString(), cancellationToken);
        }

        public virtual async Task<bool> IsExistsForUpdate<Tid>(Tid id, string key, string value, CancellationToken cancellationToken)
        {
            return await _repository.IsExistsForUpdate(id, key, value, cancellationToken);
        }

        public Task<TViewModel> GetByIdPredicate<Tid>(Tid id, Expression<Func<TViewModel, bool>> expression, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
