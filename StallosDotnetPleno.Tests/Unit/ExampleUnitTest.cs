namespace StallosDotnetPleno.Tests.Unit
{
    public class ExampleUnitTest
    {
        [Fact]
        public async Task Test_Sucess()
        {
            //Arrange
            var a = 1;
            var result = new Random().Next();

            //Act
            var b = a + result;

            //Assert
            Assert.Equal(result + a, b);
        }
    }
}
