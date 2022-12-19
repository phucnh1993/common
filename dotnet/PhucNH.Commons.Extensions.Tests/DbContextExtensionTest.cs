using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PhucNH.Commons.Bases.Models;
using PhucNH.Commons.Extensions.Tests.Fakers;

namespace PhucNH.Commons.Extensions.Tests
{
    public class DbContextExtensionTest
    {
        private readonly DbContextFaker _dbContext;

        public DbContextExtensionTest()
        {
            var contextOptions = new DbContextOptionsBuilder<DbContextFaker>()
                .UseInMemoryDatabase("BlDbTest")
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            _dbContext = new DbContextFaker(contextOptions);
        }

        public static IEnumerable<object?[]> GetDbSetParam => new List<object?[]>
        {
            new object?[]
            {
                "BaseItems",
                true,
                ""
            },
            new object?[]
            {
                "ABC",
                false,
                "DbSet of DbContext is null at [DbContextFaker] - [ABC]"
            },
        };

        [Theory]
        [MemberData(nameof(GetDbSetParam))]
        public void GetDbSet_Test(
            string entityName,
            bool isNotNull,
            string exceptionMessage)
        {
            if (_dbContext.BaseItems.Count() == 0)
            {
                _dbContext.BaseItems.AddRange(new List<BaseItem<ulong>>
                {
                    new BaseItem<ulong>
                    {
                        Id = 1,
                        Name = "ABC",
                    },
                    new BaseItem<ulong>
                    {
                        Id = 2,
                        Name = "XYZ",
                    }
                });
                _dbContext.SaveChanges();
            }
            try
            {
                var result = _dbContext.GetDbSet<BaseItem<ulong>>(entityName);
                if (isNotNull)
                {
                    Assert.NotNull(result);
                }
            }
            catch (Exception ex)
            {
                Assert.Equal(exceptionMessage, ex.Message);
            }
            try
            {
                var result = _dbContext.GetDbSet(entityName);
                if (isNotNull)
                {
                    Assert.NotNull(result);
                }
            }
            catch (Exception ex)
            {
                Assert.Equal(exceptionMessage, ex.Message);
            }
        }

        public static IEnumerable<object?[]> ListingParam => new List<object?[]>
        {
            new object?[]
            {
                "BaseItems",
                true,
                new BaseOrder
                {
                    ColumnOrder = "Name",
                    IsDesc = true,
                    PageIndex = 1,
                    PageSize = 10
                },
            },
            new object?[]
            {
                "ABC",
                false,
                new BaseOrder
                {
                    ColumnOrder = "Name",
                    IsDesc = true,
                    PageIndex = 1,
                    PageSize = 10
                },
            },
        };

        [Theory]
        [MemberData(nameof(ListingParam))]
        public async void Listing_Test(
            string entityName,
            bool isNotNull,
            BaseOrder order)
        {
            if (_dbContext.BaseItems.Count() == 0)
            {
                _dbContext.BaseItems.AddRange(new List<BaseItem<ulong>>
                {
                    new BaseItem<ulong>
                    {
                        Id = 1,
                        Name = "ABC",
                    },
                    new BaseItem<ulong>
                    {
                        Id = 2,
                        Name = "XYZ",
                    }
                });
                _dbContext.SaveChanges();
            }
            try
            {
                var query = _dbContext.GetDbSet<BaseItem<ulong>>(entityName);
                if (isNotNull)
                {
                    Assert.NotNull(query);
                }
                var result = await query
                    .BuildListing<BaseItem<ulong>, BaseOrder>(order)
                    .ToListAsync();
                Assert.NotEmpty(result);
            }
            catch (Exception ex)
            {
                Assert.NotNull(ex.Message);
            }
            try
            {
                var query = _dbContext.GetDbSet(entityName);
                if (isNotNull)
                {
                    Assert.NotNull(query);
                }
                var result = query.BuildListing(order);
                Assert.NotEmpty(result);
            }
            catch (Exception ex)
            {
                Assert.NotNull(ex.Message);
            }
        }

        public static IEnumerable<object?[]> CountParam => new List<object?[]>
        {
            new object?[]
            {
                "BaseItems",
                true,
                new BaseOrder
                {
                    ColumnOrder = "Name",
                    IsDesc = true,
                    PageIndex = 1,
                    PageSize = 10
                },
            },
            new object?[]
            {
                "ABC",
                false,
                new BaseOrder
                {
                    ColumnOrder = "Name",
                    IsDesc = true,
                    PageIndex = 1,
                    PageSize = 10
                },
            },
        };

        [Theory]
        [MemberData(nameof(CountParam))]
        public async void Count_Test(
            string entityName,
            bool isNotNull,
            BaseOrder order)
        {
            if (_dbContext.BaseItems.Count() == 0)
            {
                _dbContext.BaseItems.AddRange(new List<BaseItem<ulong>>
                {
                    new BaseItem<ulong>
                    {
                        Id = 1,
                        Name = "ABC",
                    },
                    new BaseItem<ulong>
                    {
                        Id = 2,
                        Name = "XYZ",
                    }
                });
                _dbContext.SaveChanges();
            }
            try
            {
                var query = _dbContext.GetDbSet<BaseItem<ulong>>(entityName);
                if (isNotNull)
                {
                    Assert.NotNull(query);
                }
                var result = await query
                    .BuildListing<BaseItem<ulong>, BaseOrder>(order)
                    .ToListAsync();
                Assert.NotEmpty(result);
            }
            catch (Exception ex)
            {
                Assert.NotNull(ex.Message);
            }
            try
            {
                var query = _dbContext.GetDbSet(entityName);
                if (isNotNull)
                {
                    Assert.NotNull(query);
                }
                var result = query.BuildListing(order);
                Assert.NotEmpty(result);
            }
            catch (Exception ex)
            {
                Assert.NotNull(ex.Message);
            }
        }
    }
}