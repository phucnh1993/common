namespace PhucNH.Commons.Extensions.Tests
{
    public class ConvertExtensionTest
    {
        public ConvertExtensionTest()
        {

        }

        [Fact]
        public void ConvertUlong_Test()
        {
            string happyCase = "123";
            ulong ulongHappyCase = happyCase.ToULong();
            Assert.Equal(123UL, ulongHappyCase);
        }
    }
}