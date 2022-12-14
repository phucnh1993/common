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
        public static IEnumerable<object[]> HashAsyncParam => new List<object[]>
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
            }
        };
        
        [Theory]
        [MemberData(nameof(HashAsyncParam))]
        public async Task HashAsyncTest(
            object dataObject,
            HashAlgorithmName hashType,
            int length)
        {
            var resultOne = await dataObject.HashAsync(hashType);
            var resultTwo = await dataObject.HashAsync(hashType);
            Assert.Equal(resultOne, resultTwo);
            Assert.NotEmpty(resultOne);
            Assert.Equal(resultOne.Length, length);
        }

        #endregion HASH_ASYNC_TEST
    }
}