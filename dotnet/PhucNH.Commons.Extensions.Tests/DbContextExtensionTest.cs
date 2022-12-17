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
    }
}