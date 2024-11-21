namespace TemplateTest;

public class UnitTest1
{

    [Fact]
    public void String_Concatenation_Test()
    {
        // Arrange
        string str1 = "Hello";
        string str2 = "World";

        // Act
        string result = $"{str1} {str2}";

        // Assert
        Assert.Equal("Hello World", result);
    }
    
}