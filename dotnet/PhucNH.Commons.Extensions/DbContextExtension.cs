using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;

namespace PhucNH.Commons.Extensions
{
    public static class DbContextExtension
    {
        #region DB_TRANSACTION
        /// <summary>
        /// Declare a read uncommit transaction scope.
        /// </summary>
        /// <value>A read uncommit transaction scope.</value>
        public static readonly TransactionOptions ReadOption = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadUncommitted,
            Timeout = TimeSpan.FromSeconds(5)
        };
        #endregion DB_TRANSACTION

        #region DB_CONTEXT
        /// <summary>
        /// Get DbSet in a DbContext by name.
        /// </summary>
        /// <param name="context">DbContext have DbSet.</param>
        /// <param name="entityName">Entity name in DbContext.</param>
        /// <typeparam name="TEntity">Entity data type.</typeparam>
        /// <returns>IQueryable for query entity.</returns>
        public static IQueryable<TEntity> GetDbSet<TEntity>(
            this DbContext context,
            string entityName)
        {
            var type = context.GetType();
            var resultProperty = type.GetProperties()
                .FirstOrDefault(x => x.Name.ToLower().Contains(entityName.ToLower()));
            if (resultProperty == null)
            {
                throw new NullReferenceException($"DbSet of DbContext is null at [{type.Name}] - [{entityName}]");
            }
            var result = resultProperty.GetValue(context);
            var returnValue = result as IQueryable<TEntity>;
            return returnValue;
        }
        #endregion DB_CONTEXT

        #region DB_PAGING
        /// <summary>
        /// Get data paging and order from object.
        /// </summary>
        /// <param name="query">Content of query database.</param>
        /// <param name="filter">Content of paging and order.</param>
        /// <typeparam name="TEntity">Data type of entity in database.</typeparam>
        /// <typeparam name="TFilter">Data type of filter object.</typeparam>
        /// <returns>A query object.</returns>
        public static IQueryable<TEntity> BuildListingOrderPaging<TEntity, TFilter>(
            this IQueryable<TEntity> query,
            TFilter filter)
        {
            try
            {
                var type = typeof(TFilter);
                var orderFieldProperty = type.GetProperty(BaseConstant.ColumnOrder);
                var isDescProperty = type.GetProperty(BaseConstant.IsDesc);
                var pageSizeProperty = type.GetProperty(BaseConstant.PageSize);
                var getOffsetMethod = type.GetMethod(BaseConstant.GetOffset);
                if (orderFieldProperty == null ||
                    isDescProperty == null ||
                    getOffsetMethod == null ||
                    pageSizeProperty == null)
                {
                    throw new ArgumentNullException(
                        nameof(filter),
                        $"Argument is null any properties, please check at {nameof(BuildListingOrderPaging)}");
                }
                var orderField = orderFieldProperty.GetValue(filter)?.ToString() ?? string.Empty;
                var isDesc = isDescProperty.GetValue(filter).ToBoolean();
                var pageOffset = getOffsetMethod.Invoke(filter, null).ToInt();
                var pageSize = pageSizeProperty.GetValue(filter).ToInt();
                query = query.BuildOrder(orderField, isDesc).BuildPaging(pageOffset, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(nameof(BuildListingOrderPaging), ex);
            }
            return query;
        }

        public static IQueryable<T> BuildPaging<T>(
            this IQueryable<T> query,
            int offset,
            int pageSize)
        {
            return query
                .Skip(offset)
                .Take(pageSize);
        }

        public static IQueryable<T> BuildOrder<T>(
            this IQueryable<T> query,
            string orderFieldName,
            bool isDesc)
        {
            var objectType = typeof(T) ??
                Activator.CreateInstance<Type>();
            PropertyInfo property;
            property = !string.IsNullOrEmpty(orderFieldName) ?
                objectType
                    .GetProperty(orderFieldName) :
                objectType
                    .GetProperties()
                    .FirstOrDefault(
                        x => x.GetCustomAttribute(
                            typeof(KeyAttribute)) != null);
            if (property != null)
            {
                property = objectType
                        .GetProperties()
                        .FirstOrDefault(
                            x => x.Name
                                .ToLower()
                                .Contains(BaseConstant.Id)) ??
                    objectType
                        .GetProperties()
                        .FirstOrDefault();
                if (property == null)
                    return query;
            }
            ParameterExpression parameter = Expression
                .Parameter(query.ElementType, string.Empty);
            MemberExpression propertyMember = Expression
                .Property(parameter, property.Name);
            LambdaExpression lambda = Expression
                .Lambda(propertyMember, parameter);
            string methodName = isDesc ? BaseConstant.OrderBy : BaseConstant.OrderByDescending;
            Expression methodCallExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { query.ElementType, propertyMember.Type },
                query.Expression,
                Expression.Quote(lambda));
            return query
                .Provider
                .CreateQuery<T>(methodCallExpression);
        }
        #endregion DB_PAGING

        #region DB_COMMIT
        public static async Task<List<T>> ToListNoLockAsync<T>(
            this IQueryable<T> query,
            CancellationToken cancellationToken = default)
        {
            using (var transactionScope = new TransactionScope(
                TransactionScopeOption.Required,
                ReadOption,
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await query
                    .ToListAsync(cancellationToken);
                transactionScope.Complete();
                return result;
            }
        }

        public static async Task<List<T>> ToListNoLockAsync<T>(
            this IQueryable<T> query,
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            using (var transactionScope = new TransactionScope(
                TransactionScopeOption.Required,
                ReadOption,
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await query
                    .Where(predicate)
                    .ToListAsync(cancellationToken);
                transactionScope.Complete();
                return result;
            }
        }

        public static async Task<T> FirstOrDefaultNoLockAsync<T>(
            this IQueryable<T> query,
            CancellationToken cancellationToken = default)
        {
            using (var transactionScope = new TransactionScope(
                TransactionScopeOption.Required,
                ReadOption,
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await query.FirstOrDefaultAsync(cancellationToken);
                transactionScope.Complete();
                return result;
            }
        }

        public static async Task<T> FirstOrDefaultNoLockAsync<T>(
            this IQueryable<T> query,
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            using (var transactionScope = new TransactionScope(
                TransactionScopeOption.Required,
                ReadOption,
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await query.FirstOrDefaultAsync(
                    predicate,
                    cancellationToken);
                transactionScope.Complete();
                return result;
            }
        }

        public static async Task<int> CountNoLockAsync<T>(
        this IQueryable<T> query,
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
        {
            using (var transactionScope = new TransactionScope(
                TransactionScopeOption.Required,
                ReadOption,
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await query.CountAsync(
                    predicate,
                    cancellationToken);
                transactionScope.Complete();
                return result;
            }
        }

        public static async Task<int> CountNoLockAsync<T>(
            this IQueryable<T> query,
            CancellationToken cancellationToken = default)
        {
            using (var transactionScope = new TransactionScope(
                TransactionScopeOption.Required,
                ReadOption,
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await query.CountAsync(
                    cancellationToken);
                transactionScope.Complete();
                return result;
            }
        }

        public static async Task<long> LongCountNoLockAsync<T>(
            this IQueryable<T> query,
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            using (var transactionScope = new TransactionScope(
                TransactionScopeOption.Required,
                ReadOption,
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await query.LongCountAsync(
                    predicate,
                    cancellationToken);
                transactionScope.Complete();
                return result;
            }
        }

        public static async Task<long> LongCountNoLockAsync<T>(
            this IQueryable<T> query,
            CancellationToken cancellationToken = default)
        {
            using (var transactionScope = new TransactionScope(
                TransactionScopeOption.Required,
                ReadOption,
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await query.LongCountAsync(
                    cancellationToken);
                transactionScope.Complete();
                return result;
            }
        }

        public static async Task<ulong> ULongCountNoLockAsync<T>(
            this IQueryable<T> query,
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            var result = await query.LongCountNoLockAsync(
                    predicate,
                    cancellationToken);
            return (ulong)result;
        }

        public static async Task<ulong> ULongCountNoLockAsync<T>(
            this IQueryable<T> query,
            CancellationToken cancellationToken = default)
        {
            var result = await query.LongCountNoLockAsync(
                    cancellationToken);
            return (ulong)result;
        }
        #endregion DB_COMMIT
    }
}