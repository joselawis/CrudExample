using FluentAssertions;

namespace CrudTests;

public class PersonsControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PersonsControllerIntegrationTest(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    #region Index

    [Fact]
    public async void Index_ShouldReturnView()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/Persons/Index");

        // Assert
        response.Should().BeSuccessful(); // 2xx response
    }

    #endregion
}
