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

        /// <summary>
        /// Get DbSet in a DbContext by name.
        /// </summary>
        /// <param name="context">DbContext have DbSet.</param>
        /// <param name="entityName">Entity name in DbContext/</param>
        /// <returns>IQueryable for query entity.</returns>
        public static IQueryable GetDbSet(
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
            var returnValue = result as IQueryable;
            return returnValue;
        }
        #endregion DB_CONTEXT

        #region DB_SKIP
        /// <summary>
        /// Skip value with count type UInt64
        /// </summary>
        /// <param name="query">Queryable.</param>
        /// <param name="count">Number count.</param>
        /// <returns>A new queryable.</returns>
        public static IQueryable Skip(
            this IQueryable query,
            ulong count)
        {
            query.ValidateNullObject(nameof(Skip), ConstantExtension.ArgumentQueryNull);
            return query.Provider.CreateQuery(
               Expression.Call(
                   null,
                   new Func<IQueryable, ulong, IQueryable>(Skip).Method,
                   query.Expression, Expression.Constant(count)));
        }

        /// <summary>
        /// Skip value with count type UInt64
        /// </summary>
        /// <param name="query">Queryable.</param>
        /// <param name="count">Number count.</param>
        /// <typeparam name="TSource">Data type of queryable object.</typeparam>
        /// <returns>A new queryable.</returns>
        public static IQueryable<TSource> Skip<TSource>(
            this IQueryable<TSource> query,
            ulong count)
        {
            query.ValidateNullObject(nameof(Skip), ConstantExtension.ArgumentQueryNull);
            return query.Provider.CreateQuery<TSource>(
               Expression.Call(
                   null,
                   new Func<IQueryable<TSource>, ulong, IQueryable<TSource>>(Skip).Method,
                   query.Expression, Expression.Constant(count)));
        }
        #endregion DB_SKIP

        #region DB_TAKE
        /// <summary>
        /// Skip value with count type UInt64
        /// </summary>
        /// <param name="query">Queryable.</param>
        /// <param name="count">Number count.</param>
        /// <returns>A new queryable.</returns>
        public static IQueryable Take(
            this IQueryable query,
            ushort count)
        {
            query.ValidateNullObject(nameof(Take), ConstantExtension.ArgumentQueryNull);
            return query.Provider.CreateQuery(
               Expression.Call(
                   null,
                   new Func<IQueryable, ushort, IQueryable>(Take).Method,
                   query.Expression, Expression.Constant(count)));
        }

        /// <summary>
        /// Skip value with count type UInt64
        /// </summary>
        /// <param name="query">Queryable.</param>
        /// <param name="count">Number count.</param>
        /// <typeparam name="TSource">Data type of queryable object.</typeparam>
        /// <returns>A new queryable.</returns>
        public static IQueryable<TSource> Take<TSource>(
            this IQueryable<TSource> query,
            ushort count)
        {
            query.ValidateNullObject(nameof(Take), ConstantExtension.ArgumentQueryNull);
            return query.Provider.CreateQuery<TSource>(
               Expression.Call(
                   null,
                   new Func<IQueryable<TSource>, ushort, IQueryable<TSource>>(Take).Method,
                   query.Expression, Expression.Constant(count)));
        }
        #endregion DB_TAKE

        #region DB_PAGING

        /// <summary>
        /// Get data paging and order from object.
        /// </summary>
        /// <param name="query">Content of query database.</param>
        /// <param name="filter">Content of paging and order.</param>
        /// <typeparam name="TEntity">Data type of entity in database.</typeparam>
        /// <typeparam name="TFilter">Data type of filter object.</typeparam>
        /// <returns>A query object.</returns>
        public static IQueryable<TEntity> BuildListing<TEntity, TFilter>(
            this IQueryable<TEntity> query,
            TFilter filter)
        {
            query.ValidateNullObject(nameof(BuildListing), ConstantExtension.ArgumentQueryNull);
            var type = typeof(TFilter);
            var orderFieldProperty = type.GetProperty(ConstantExtension.ColumnOrder);
            var isDescProperty = type.GetProperty(ConstantExtension.IsDesc);
            var pageSizeProperty = type.GetProperty(ConstantExtension.PageSize);
            var getOffsetMethod = type.GetMethod(ConstantExtension.GetOffset);
            nameof(BuildListing).ValidateNullObjects(
                "Argument is null at [filter] property.",
                orderFieldProperty,
                isDescProperty,
                getOffsetMethod,
                pageSizeProperty);
            var orderField = orderFieldProperty
                .GetValue(filter)
                .ToString(ConstantExtension.Empty);
            var isDesc = isDescProperty
                .GetValue(filter)
                .ToBoolean();
            var pageOffset = getOffsetMethod
                .Invoke(filter, null)
                .ToULong();
            var pageSize = pageSizeProperty
                .GetValue(filter)
                .ToUShort();
            query = query
                .BuildOrder(orderField, isDesc)
                .BuildPaging(pageOffset, pageSize);
            return query;
        }

        /// <summary>
        /// Get data paging and order from object.
        /// </summary>
        /// <param name="query">Content of query database.</param>
        /// <param name="filter">Content of paging and order.</param>
        /// <returns>A query object.</returns>
        public static IQueryable BuildListing(
            this IQueryable query,
            object filter)
        {
            query.ValidateNullObject(nameof(BuildListing), ConstantExtension.ArgumentQueryNull);
            var type = filter.GetType();
            var orderFieldProperty = type.GetProperty(ConstantExtension.ColumnOrder);
            var isDescProperty = type.GetProperty(ConstantExtension.IsDesc);
            var pageSizeProperty = type.GetProperty(ConstantExtension.PageSize);
            var getOffsetMethod = type.GetMethod(ConstantExtension.GetOffset);
            nameof(BuildListing).ValidateNullObjects(
                "Argument is null at [filter] property.",
                orderFieldProperty,
                isDescProperty,
                getOffsetMethod,
                pageSizeProperty);
            var orderField = orderFieldProperty
                .GetValue(filter)
                .ToString(ConstantExtension.Empty);
            var isDesc = isDescProperty
                .GetValue(filter)
                .ToBoolean();
            var pageOffset = getOffsetMethod
                .Invoke(filter, null)
                .ToULong();
            var pageSize = pageSizeProperty
                .GetValue(filter)
                .ToUShort();
            return query
                .BuildOrder(orderField, isDesc)
                .BuildPaging(pageOffset, pageSize);
        }

        /// <summary>
        /// Build query get a page.
        /// </summary>
        /// <param name="query">Base query.</param>
        /// <param name="offset">Offset value.</param>
        /// <param name="pageSize">Page size value.</param>
        /// <typeparam name="TSource">Data type of query.</typeparam>
        /// <returns>A new query with page size.</returns>
        public static IQueryable<TSource> BuildPaging<TSource>(
            this IQueryable<TSource> query,
            ulong offset,
            ushort pageSize)
        {
            return query
                .Skip(offset)
                .Take(pageSize);
        }

        /// <summary>
        /// Build query get a page.
        /// </summary>
        /// <param name="query">Base query.</param>
        /// <param name="offset">Offset value.</param>
        /// <param name="pageSize">Page size value.</param>
        /// <returns>A new query with page size.</returns>
        public static IQueryable BuildPaging(
            this IQueryable query,
            ulong offset,
            ushort pageSize)
        {
            return query
                .Skip(offset)
                .Take(pageSize);
        }

        /// <summary>
        /// Get property for query order by with field name.
        /// </summary>
        /// <param name="orderFieldName">Field name order.</param>
        /// <param name="objectType">Object type will order.</param>
        /// <returns>Property info.</returns>
        public static PropertyInfo GetPropertyOrder(
            this string orderFieldName,
            Type objectType)
        {
            var property = !string.IsNullOrEmpty(orderFieldName) ?
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
                                .Contains(ConstantExtension.Id)) ??
                    objectType
                        .GetProperties()
                        .FirstOrDefault();
                property.ValidateNullObject(nameof(GetPropertyOrder), ConstantExtension.ArgumentPropertyNull);
            }
            return property;
        }

        /// <summary>
        /// Get order expression of query.
        /// </summary>
        /// <param name="property">Property want to order.</param>
        /// <param name="queryElementType">Element type of query.</param>
        /// <param name="isDesc">Type order.</param>
        /// <param name="queryExpression">Query expression base.</param>
        /// <returns>A new expression.</returns>
        public static Expression GetOrderExpression(
            this PropertyInfo property,
            Type queryElementType,
            bool isDesc,
            Expression queryExpression)
        {
            ParameterExpression parameter = Expression
                .Parameter(queryElementType, string.Empty);
            MemberExpression propertyMember = Expression
                .Property(parameter, property.Name);
            LambdaExpression lambda = Expression
                .Lambda(propertyMember, parameter);
            string methodName = isDesc ?
                ConstantExtension.OrderBy :
                ConstantExtension.OrderByDescending;
            return Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { queryElementType, propertyMember.Type },
                queryExpression,
                Expression.Quote(lambda));
        }

        /// <summary>
        /// Build order query.
        /// </summary>
        /// <param name="query">Base query.</param>
        /// <param name="orderFieldName">Order field name.</param>
        /// <param name="isDesc">Order by type descending.</param>
        /// <typeparam name="TSource">Data type of query.</typeparam>
        /// <returns>A new query.</returns>
        public static IQueryable<TSource> BuildOrder<TSource>(
            this IQueryable<TSource> query,
            string orderFieldName,
            bool isDesc)
        {
            query.ValidateNullObject(nameof(BuildOrder), ConstantExtension.ArgumentQueryNull);
            var objectType = typeof(TSource) ??
                Activator.CreateInstance<Type>();
            var property = orderFieldName.GetPropertyOrder(objectType);
            Expression methodCallExpression = property
                .GetOrderExpression(query.ElementType, isDesc, query.Expression);
            return query
                .Provider
                .CreateQuery<TSource>(methodCallExpression);
        }

        /// <summary>
        /// Build order query.
        /// </summary>
        /// <param name="query">Base query.</param>
        /// <param name="orderFieldName">Order field name.</param>
        /// <param name="isDesc">Order by type descending.</param>
        /// <returns>A new query.</returns>
        public static IQueryable BuildOrder(
            this IQueryable query,
            string orderFieldName,
            bool isDesc)
        {
            query.ValidateNullObject(nameof(BuildOrder), ConstantExtension.ArgumentQueryNull);
            var objectType = query.ElementType;
            var property = orderFieldName.GetPropertyOrder(objectType);
            Expression methodCallExpression = property
                .GetOrderExpression(query.ElementType, isDesc, query.Expression);
            return query
                .Provider
                .CreateQuery(methodCallExpression);
        }

        /// <summary>
        /// Get any property is not order or paging.
        /// </summary>
        /// <typeparam name="TRequest">A object inclue request info.</typeparam>
        /// <returns>A list property info.</returns>
        public static List<PropertyInfo> GetNonOrderPagingProperties<TRequest>()
        {
            var requestType = typeof(TRequest) ??
                Activator.CreateInstance<Type>();
            return requestType
                .GetProperties()
                .Where(
                    x => !x.Name.Equals(ConstantExtension.ColumnOrder) &&
                        !x.Name.Equals(ConstantExtension.IsDesc) &&
                        !x.Name.Equals(ConstantExtension.PageIndex) &&
                        !x.Name.Equals(ConstantExtension.PageSize))
                .ToList();
        }
        #endregion DB_PAGING

        #region DB_COMPARE
        /// <summary>
        /// Method support build a equal for a property of entity of method WHERE.
        /// </summary>
        /// <param name="propertyInfo">Property info of entity.</param>
        /// <param name="paramName">Name is map of entity.</param>
        /// <param name="value">Value compare.</param>
        /// <typeparam name="TEntity">Type of entity.</typeparam>
        /// <typeparam name="TValue">Type of value.</typeparam>
        /// <returns>A expression compare equal.</returns>
        public static Expression<Func<TEntity, bool>> BuildEqual<TEntity, TValue>(
            this PropertyInfo propertyInfo,
            string paramName,
            TValue value)
        {
            var declareType = propertyInfo.DeclaringType;
            declareType.ValidateNullObject(nameof(BuildEqual), ConstantExtension.ArgumentPropertyTypeNull);
            var propertyParam = Expression.Parameter(declareType, paramName);
            var propertyExp = Expression.Property(propertyParam, propertyInfo);
            return Expression.Lambda<Func<TEntity, bool>>(
                Expression.Equal(propertyExp, Expression.Constant(value, typeof(TValue))),
                new[] { propertyParam });
        }

        /// <summary>
        /// Build mapping property between two object.
        /// </summary>
        /// <typeparam name="TEntity">Current entity object type.</typeparam>
        /// <typeparam name="TResult">New entity object type.</typeparam>
        /// <returns>A enumerable mapping.</returns>
        public static IEnumerable<MemberAssignment> BuildMapping<TEntity, TResult>()
        {
            IEnumerable<MemberAssignment> results = null;
            var source = Expression.Parameter(typeof(TEntity), "o");
            var resultType = typeof(TResult);
            var sourceType = typeof(TEntity);
            var resultProperties = resultType.GetProperties();
            if (resultProperties.Any())
            {
                results = resultProperties.Select(
                    p => sourceType.GetProperty(p.Name) != null &&
                        sourceType.GetProperty(p.Name).GetType() == p.GetType() ?
                        Expression.Bind(p, Expression.Property(source, p.Name)) :
                        Expression.Bind(p, Expression.Default(resultType.GetProperty(p.Name).GetType())));
            }
            return results;
        }

        /// <summary>
        /// Get method compare by property name.
        /// </summary>
        /// <param name="property">Property info.</param>
        /// <returns>Method info.</returns>
        public static MethodInfo GetMethodInfo(
            this PropertyInfo property)
        {
            var type = property.GetType();
            if (property.Name.ToLower().Contains("from"))
            {
                return type.GetMethod(">=");
            }
            if (property.Name.ToLower().Contains("to"))
            {
                return type.GetMethod("<=");
            }
            if (type == typeof(string))
            {
                return type.GetMethod("Contains");
            }
            return type.GetMethod("==");
        }
        #endregion DB_COMPARE

        #region DB_SELECT
        /// <summary>
        /// Build a single get entity by ID.
        /// </summary>
        /// <param name="query">Root query.</param>
        /// <param name="id">ID compare.</param>
        /// <typeparam name="TEntity">Type of entity in query.</typeparam>
        /// <typeparam name="TId">Type of ID compare.</typeparam>
        /// <returns>A new query with filter compare ID.</returns>
        public static IQueryable<TEntity> BuildSingle<TEntity, TId>(
            this IQueryable<TEntity> query,
            TId id)
        {
            query.ValidateNullObject(nameof(BuildSingle), ConstantExtension.ArgumentQueryNull);
            var entityType = typeof(TEntity);
            entityType.ValidateNullObject(nameof(BuildSingle), ConstantExtension.ArgumentPropertyTypeNull);
            var entityProperty = entityType.GetProperty("Id");
            entityProperty.ValidateNullObject(nameof(BuildSingle), ConstantExtension.ArgumentPropertyNull);
            var compareExpression = entityProperty.BuildEqual<TEntity, TId>(
                string.Empty,
                id);
            var methodCallExpression = Expression.Call(
                    typeof(Queryable),
                    "Where",
                    new Type[] { query.ElementType },
                    query.Expression,
                    Expression.Quote(compareExpression));
            query = query.Provider.CreateQuery<TEntity>(
                methodCallExpression);
            return query;
        }

        /// <summary>
        /// Build a select new item from query.
        /// </summary>
        /// <param name="query">Base query.</param>
        /// <typeparam name="TEntity">Current entity type.</typeparam>
        /// <typeparam name="TResult">New entity type.</typeparam>
        /// <returns>New query.</returns>
        public static IQueryable<TResult> BuildSelect<TEntity, TResult>(
            this IQueryable<TEntity> query)
        {
            query.ValidateNullObject(
                nameof(BuildSelect),
                ConstantExtension.ArgumentQueryNull);
            var source = Expression.Parameter(typeof(TEntity), "o");
            var resultType = typeof(TResult);
            var resultProperties = resultType.GetProperties();
            var bindings = resultProperties.Select(
                p => Expression.Bind(resultType?.GetProperty(p.Name), Expression.Property(source, p.Name)));
            bindings.ValidateNullObject(
                nameof(BuildSelect),
                ConstantExtension.ArgumentBindingNull);
            var result = Expression.MemberInit(Expression.New(resultType), bindings);
            var objectExpression = Expression.Lambda(result, source);
            var methodCallExpression = Expression.Call(
                typeof(IQueryable),
                "Select",
                new Type[] { query.ElementType, resultType },
                query.Expression,
                Expression.Quote(objectExpression));
            methodCallExpression.ValidateNullObject(
                nameof(BuildSelect),
                ConstantExtension.ArgumentMethodNull);
            return query.Provider.CreateQuery<TResult>(methodCallExpression);
        }
        #endregion DB_SELECT

        #region DB_WHERE
        /// <summary>
        /// Build a search data from query.
        /// </summary>
        /// <param name="query">Base query.</param>
        /// <param name="request">Request query.</param>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <typeparam name="TRequest">Request type.</typeparam>
        /// <returns>New query.</returns>
        public static IQueryable<TEntity> BuildSearch<TEntity, TRequest>(
            this IQueryable<TEntity> query,
            TRequest request)
        {
            query.ValidateNullObject(
                nameof(BuildSearch),
                ConstantExtension.ArgumentQueryNull);
            var objectType = typeof(TEntity) ??
                Activator.CreateInstance<Type>();
            var requestProperties = GetNonOrderPagingProperties<TRequest>();
            if (!requestProperties.Any())
                return query;
            foreach (var requestProperty in requestProperties)
            {
                var requestValue = requestProperty.GetValue(request);
                var objectProperty = objectType.GetProperty(requestProperty.Name);
                if (requestValue == null && objectProperty == null)
                    continue;
                MethodInfo method = requestProperty.GetMethodInfo();
                if (method == null)
                    continue;

                method.Invoke(null, new object[] {
                    requestProperty.Name, requestValue
                });
                ParameterExpression parameter = Expression
                    .Parameter(query.ElementType, string.Empty);
                MemberExpression propertyMember = Expression
                    .Property(parameter, objectProperty.Name);
                LambdaExpression objectExpression = Expression
                    .Lambda(propertyMember, parameter);
                Expression methodCallExpression = Expression.Call(objectExpression, method);

                query = query.Provider.CreateQuery<TEntity>(methodCallExpression);
            }
            return query;
        }
        #endregion DB_WHERE

        #region DB_COMMIT
        /// <summary>
        /// Get list with no lock.
        /// </summary>
        /// <param name="query">Query base.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <typeparam name="TSource">Data type of query.</typeparam>
        /// <returns>A list data.</returns>
        public static async Task<List<TSource>> ToListNoLockAsync<TSource>(
            this IQueryable<TSource> query,
            CancellationToken cancellationToken = default)
        {
            query.ValidateNullObject(nameof(ToListNoLockAsync), ConstantExtension.ArgumentQueryNull);
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

        /// <summary>
        /// Get list with no lock.
        /// </summary>
        /// <param name="query">Query base.</param>
        /// <param name="predicate">Query predicate.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <typeparam name="TSource">Data type of query.</typeparam>
        /// <returns>A list data.</returns>
        public static async Task<List<TSource>> ToListNoLockAsync<TSource>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            query.ValidateNullObject(nameof(ToListNoLockAsync), ConstantExtension.ArgumentQueryNull);
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

        /// <summary>
        /// Get once with no lock.
        /// </summary>
        /// <param name="query">Query base.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <typeparam name="TSource">Data type of query.</typeparam>
        /// <returns>A object data.</returns>
        public static async Task<TSource> FirstOrDefaultNoLockAsync<TSource>(
            this IQueryable<TSource> query,
            CancellationToken cancellationToken = default)
        {
            query.ValidateNullObject(nameof(FirstOrDefaultNoLockAsync), ConstantExtension.ArgumentQueryNull);
            using (var transactionScope = new TransactionScope(
                TransactionScopeOption.Required,
                ReadOption,
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await query
                    .FirstOrDefaultAsync(cancellationToken);
                transactionScope.Complete();
                return result;
            }
        }

        /// <summary>
        /// Get once with no lock.
        /// </summary>
        /// <param name="query">Query base.</param>
        /// <param name="predicate">Query predicate.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <typeparam name="TSource">Data type of query.</typeparam>
        /// <returns>A object data.</returns>
        public static async Task<TSource> FirstOrDefaultNoLockAsync<TSource>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            query.ValidateNullObject(nameof(FirstOrDefaultNoLockAsync), ConstantExtension.ArgumentQueryNull);
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

        /// <summary>
        /// Count with no lock
        /// </summary>
        /// <param name="query">Query base.</param>
        /// <param name="predicate">Query predicate.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <typeparam name="TSource">Data type of query.</typeparam>
        /// <returns>A number.</returns>
        public static async Task<int> CountNoLockAsync<TSource>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            query.ValidateNullObject(nameof(CountNoLockAsync), ConstantExtension.ArgumentQueryNull);
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

        /// <summary>
        /// Count with no lock
        /// </summary>
        /// <param name="query">Query base.</param>
        /// <param name="predicate">Query predicate.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <typeparam name="TSource">Data type of query.</typeparam>
        /// <returns>A number.</returns>
        public static async Task<int> CountNoLockAsync<TSource>(
            this IQueryable<TSource> query,
            CancellationToken cancellationToken = default)
        {
            query.ValidateNullObject(nameof(CountNoLockAsync), ConstantExtension.ArgumentQueryNull);
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

        /// <summary>
        /// Count with no lock
        /// </summary>
        /// <param name="query">Query base.</param>
        /// <param name="predicate">Query predicate.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <typeparam name="TSource">Data type of query.</typeparam>
        /// <returns>A number.</returns>
        public static async Task<long> LongCountNoLockAsync<TSource>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            query.ValidateNullObject(nameof(LongCountNoLockAsync), ConstantExtension.ArgumentQueryNull);
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

        /// <summary>
        /// Count with no lock
        /// </summary>
        /// <param name="query">Query base.</param>
        /// <param name="predicate">Query predicate.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <typeparam name="TSource">Data type of query.</typeparam>
        /// <returns>A number.</returns>
        public static async Task<long> LongCountNoLockAsync<TSource>(
            this IQueryable<TSource> query,
            CancellationToken cancellationToken = default)
        {
            query.ValidateNullObject(nameof(LongCountNoLockAsync), ConstantExtension.ArgumentQueryNull);
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

        /// <summary>
        /// Count with no lock
        /// </summary>
        /// <param name="query">Query base.</param>
        /// <param name="predicate">Query predicate.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <typeparam name="TSource">Data type of query.</typeparam>
        /// <returns>A number.</returns>
        public static async Task<ulong> ULongCountNoLockAsync<TSource>(
            this IQueryable<TSource> query,
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            query.ValidateNullObject(nameof(ULongCountNoLockAsync), ConstantExtension.ArgumentQueryNull);
            var result = await query.LongCountNoLockAsync(
                    predicate,
                    cancellationToken);
            return (ulong)result;
        }

        /// <summary>
        /// Count with no lock
        /// </summary>
        /// <param name="query">Query base.</param>
        /// <param name="predicate">Query predicate.</param>
        /// <param name="cancellationToken">Cancel token.</param>
        /// <typeparam name="TSource">Data type of query.</typeparam>
        /// <returns>A number.</returns>
        public static async Task<ulong> ULongCountNoLockAsync<TSource>(
            this IQueryable<TSource> query,
            CancellationToken cancellationToken = default)
        {
            query.ValidateNullObject(nameof(ULongCountNoLockAsync), ConstantExtension.ArgumentQueryNull);
            var result = await query.LongCountNoLockAsync(
                    cancellationToken);
            return (ulong)result;
        }
        #endregion DB_COMMIT
    }
}