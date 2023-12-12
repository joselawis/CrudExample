namespace CrudTests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        // Arrange
        const int input1 = 10,
            input2 = 5;
        const int expected = 15;

        // Act
        var actual = MyMath.Add(input1, input2);

        // Assert
        Assert.Equal(expected, actual);
    }
}
