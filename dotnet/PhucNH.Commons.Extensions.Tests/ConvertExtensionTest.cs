namespace PhucNH.Commons.Extensions.Tests
{
    public class ConvertExtensionTest
    {
        public ConvertExtensionTest()
        {

        }

        #region CONVERT
        public static IEnumerable<object?[]> ConvertParam => new List<object?[]>
        {
            new object?[]
            {
                "100",
                "100"
            },
            new object?[]
            {
                "",
                "0"
            },
        };

        [Theory]
        [MemberData(nameof(ConvertParam))]
        public void Convert_Test(
            object data,
            string result)
        {
            var result_1 = data.ToULong();
            Assert.Equal(ulong.Parse(result), result_1);
            var result_2 = data.ToLong();
            Assert.Equal(long.Parse(result), result_2);
            var result_3 = data.ToUInt();
            Assert.Equal(uint.Parse(result), result_3);
            var result_4 = data.ToInt();
            Assert.Equal(int.Parse(result), result_4);
            var result_5 = data.ToUShort();
            Assert.Equal(ushort.Parse(result), result_5);
            var result_6 = data.ToShort();
            Assert.Equal(short.Parse(result), result_6);
            var result_7 = data.ToByte();
            Assert.Equal(byte.Parse(result), result_7);
            var result_8 = data.ToSbyte();
            Assert.Equal(sbyte.Parse(result), result_8);
        }

        public static IEnumerable<object?[]> ConvertBooleanParam => new List<object?[]>
        {
            new object?[]
            {
                "True",
                "True"
            },
            new object?[]
            {
                "",
                "False"
            },
        };

        [Theory]
        [MemberData(nameof(ConvertBooleanParam))]
        public void ConvertBoolean_Test(
            object data,
            string result)
        {
            var result_1 = data.ToBoolean();
            Assert.Equal(bool.Parse(result), result_1);
        }
        #endregion CONVERT
    }
}