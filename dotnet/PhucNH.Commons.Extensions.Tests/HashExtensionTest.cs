using System.Security.Cryptography;

namespace PhucNH.Commons.Extensions.Tests
{
    public class HashExtensionTest
    {
        public HashExtensionTest()
        {
        }

        #region OBJECT_PARAM
        public static object Obj => new
        {
            Id = int.MinValue,
            Name = "Name Test"
        };
        #endregion OBJECT_PARAM

        #region HASH_ASYNC_TEST
        public static IEnumerable<object?[]> HashAsyncParam => new List<object?[]>
        {
            new object[]
            {
                Obj,
                HashAlgorithmName.MD5,
                24
            },
            new object[]
            {
                Obj,
                HashAlgorithmName.SHA1,
                28
            },
            new object[]
            {
                Obj,
                HashAlgorithmName.SHA256,
                44
            },
            new object[]
            {
                Obj,
                HashAlgorithmName.SHA384,
                64
            },
            new object[]
            {
                Obj,
                HashAlgorithmName.SHA512,
                88
            },
            new object?[]
            {
                null,
                HashAlgorithmName.SHA512,
                88
            },
            new object?[]
            {
                new byte[0],
                HashAlgorithmName.SHA512,
                88
            }
        };

        [Theory]
        [MemberData(nameof(HashAsyncParam))]
        public async Task HashAsyncTest(
            object dataObject,
            HashAlgorithmName hashType,
            int length)
        {
            try
            {
                var resultOne = await dataObject.HashAsync(hashType);
                var resultTwo = await dataObject.HashAsync(hashType);
                Assert.Equal(resultOne, resultTwo);
                Assert.NotEmpty(resultOne);
                Assert.Equal(resultOne.Length, length);
            }
            catch (Exception ex)
            {
                Assert.Equal("Please check argument [dataObject], value is empty. (Parameter 'HashAsync')", ex.Message);
            }
        }
        #endregion HASH_ASYNC_TEST

        #region HASH_MAC_ASYNC_TEST
        public static IEnumerable<object?[]> HashMacAsyncParam => new List<object?[]>
        {
            new object[]
            {
                Obj,
                "ABCDEFGH",
                HashAlgorithmName.MD5,
                24
            },
            new object[]
            {
                Obj,
                "ABCDEFGH",
                HashAlgorithmName.SHA1,
                28
            },
            new object[]
            {
                Obj,
                "ABCDEFGH",
                HashAlgorithmName.SHA256,
                44
            },
            new object[]
            {
                Obj,
                "ABCDEFGH",
                HashAlgorithmName.SHA384,
                64
            },
            new object[]
            {
                Obj,
                "ABCDEFGH",
                HashAlgorithmName.SHA512,
                88
            },
            new object?[]
            {
                null,
                "ABCDEFGH",
                HashAlgorithmName.SHA512,
                88
            },
            new object[]
            {
                Obj,
                "",
                HashAlgorithmName.SHA512,
                88
            },
            new object?[]
            {
                null,
                null,
                HashAlgorithmName.SHA512,
                88
            }
        };

        [Theory]
        [MemberData(nameof(HashMacAsyncParam))]
        public async Task HashMacAsyncTest(
            object dataObject,
            string key,
            HashAlgorithmName hashType,
            int length)
        {
            try
            {
            var resultOne = await dataObject.HashMacAsync(key, hashType);
            var resultTwo = await dataObject.HashMacAsync(key, hashType);
            Assert.Equal(resultOne, resultTwo);
            Assert.NotEmpty(resultOne);
            Assert.Equal(resultOne.Length, length);
            }
            catch(Exception ex)
            {
                Assert.Equal("Please check argument [dataObject] or [key], value is empty. (Parameter 'HashMacAsync')", ex.Message);
            }
        }
        #endregion HASH_MAC_ASYNC_TEST
    }
}