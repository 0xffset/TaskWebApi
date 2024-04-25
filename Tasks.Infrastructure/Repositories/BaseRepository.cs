using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Taks.Core.Common;
using Taks.Core.Entities.ViewModels;
using Taks.Core.Exceptions;
using Taks.Core.Interfaces.IRepositories;
using Tasks.Infrastructure.Data;

namespace Tasks.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _dbContext;
        protected DbSet<T> DbSet => _dbContext.Set<T>();

        public BaseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<T>> GetAll(CancellationToken cancellationToken = default)
        {
            List<T> data = await _dbContext.Set<T>()
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return data;
        }

        public async Task<IEnumerable<T>> GetAll(List<Expression<Func<T, object>>> includeExpressions, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsQueryable();

            if (includeExpressions != null)
            {
                query = includeExpressions.Aggregate(query, (current, includeExpression) => current.Include(includeExpression));
            }

            List<T> entities = await query.AsNoTracking().ToListAsync(cancellationToken);
            return entities;
        }

        public virtual async Task<PaginatedDataViewModel<T>> GetPaginatedData(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbContext.Set<T>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking();

            List<T> data = await query.ToListAsync(cancellationToken);
            int totalCount = await _dbContext.Set<T>().CountAsync(cancellationToken);

            return new PaginatedDataViewModel<T>(data, totalCount);
        }

        public async Task<PaginatedDataViewModel<T>> GetPaginatedData(int pageNumber, int pageSize, List<ExpressionFilter> filters, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();

            // Apply search criteria if provided
            if (filters != null && filters.Any())
            {
                Expression<Func<T, bool>>? expressionTree = ExpressionBuilder.ConstructAndExpressionTree<T>(filters);
                query = query.Where(expressionTree);
            }

            // Pagination
            List<T> data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            int totalCount = await query.CountAsync(cancellationToken);

            return new PaginatedDataViewModel<T>(data, totalCount);
        }

        public virtual async Task<PaginatedDataViewModel<T>> GetPaginatedData(List<Expression<Func<T, object>>> includeExpressions, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbContext.Set<T>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsQueryable();

            if (includeExpressions != null)
            {
                query = includeExpressions.Aggregate(query, (current, includeExpression) => current.Include(includeExpression));
            }

            List<T> data = await query.AsNoTracking().ToListAsync(cancellationToken);
            int totalCount = await _dbContext.Set<T>().CountAsync(cancellationToken);

            return new PaginatedDataViewModel<T>(data, totalCount);
        }

        public async Task<PaginatedDataViewModel<T>> GetPaginatedData(int pageNumber, int pageSize, List<ExpressionFilter> filters, string sortBy, string sortOrder, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();

            // Apply search criteria if provided
            if (filters != null && filters.Any())
            {
                Expression<Func<T, bool>>? expressionTree = ExpressionBuilder.ConstructAndExpressionTree<T>(filters);
                query = query.Where(expressionTree);
            }

            // Add sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                Expression<Func<T, object>> orderByExpression = GetOrderByExpression<T>(sortBy);
                query = sortOrder?.ToLower() == "desc" ? query.OrderByDescending(orderByExpression) : query.OrderBy(orderByExpression);
            }

            // Pagination
            List<T> data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            int totalCount = await query.CountAsync(cancellationToken);

            return new PaginatedDataViewModel<T>(data, totalCount);
        }

        private Expression<Func<T, object>> GetOrderByExpression<T>(string propertyName)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            MemberExpression property = Expression.Property(parameter, propertyName);
            UnaryExpression conversion = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(conversion, parameter);
        }

        public virtual async Task<T> GetById<Tid>(Tid id, CancellationToken cancellationToken = default)
        {
            T? data = await _dbContext.Set<T>()
                .FindAsync(id, cancellationToken);
            return data ?? throw new NotFoundException("No data found");
        }

        public virtual async Task<T> GetByIdPredicate<Tid>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
        {
            T? data = await _dbContext.Set<T>()
                             .FirstOrDefaultAsync(expression, cancellationToken);

            return data ?? throw new NotFoundException("No data found");
        }

        public virtual async Task<T> GetById<Tid>(List<Expression<Func<T, object>>> includeExpressions, Tid id, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsQueryable();

            if (includeExpressions != null)
            {
                query = includeExpressions.Aggregate(query, (current, include) => current.Include(include));
            }

            T? data = await query.SingleOrDefaultAsync(x => EF.Property<Tid>(x, "Id").Equals(id), cancellationToken);

            return data ?? throw new NotFoundException("No data found");
        }

        public async Task<bool> IsExists<Tvalue>(string key, Tvalue value, CancellationToken cancellationToken = default)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            MemberExpression property = Expression.Property(parameter, key);
            ConstantExpression constant = Expression.Constant(value);
            BinaryExpression equality = Expression.Equal(property, constant);
            Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);

            return await _dbContext.Set<T>().AnyAsync(lambda, cancellationToken);
        }

        //Before update existence check
        public async Task<bool> IsExistsForUpdate<Tid>(Tid id, string key, string value, CancellationToken cancellationToken = default)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            MemberExpression property = Expression.Property(parameter, key);
            ConstantExpression constant = Expression.Constant(value);
            BinaryExpression equality = Expression.Equal(property, constant);

            MemberExpression idProperty = Expression.Property(parameter, "Id");
            BinaryExpression idEquality = Expression.NotEqual(idProperty, Expression.Constant(id));

            BinaryExpression combinedExpression = Expression.AndAlso(equality, idEquality);
            Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);

            return await _dbContext.Set<T>().AnyAsync(lambda, cancellationToken);
        }


        public async Task<T> Create(T model, CancellationToken cancellationToken = default)
        {
            _ = await _dbContext.Set<T>().AddAsync(model, cancellationToken);
            _ = await _dbContext.SaveChangesAsync(cancellationToken);
            return model;
        }

        public async Task CreateRange(List<T> model, CancellationToken cancellationToken = default)
        {
            await _dbContext.Set<T>().AddRangeAsync(model, cancellationToken);
            _ = await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(T model, CancellationToken cancellationToken = default)
        {
            _ = _dbContext.Set<T>().Update(model);
            _ = await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(T model, CancellationToken cancellationToken = default)
        {
            _ = _dbContext.Set<T>().Remove(model);
            _ = await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task SaveChangeAsync(CancellationToken cancellationToken = default)
        {
            _ = await _dbContext.SaveChangesAsync(cancellationToken);
        }

    }
}
