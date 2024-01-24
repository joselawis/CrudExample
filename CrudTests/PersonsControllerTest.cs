using AutoFixture;
using CrudExample.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CrudTests;

public class PersonsControllerTest
{
    private readonly Mock<ICountriesService> _countriesServiceMock;
    private readonly Fixture _fixture;
    private readonly Mock<ILogger<PersonsController>> _loggerMock;
    private readonly PersonsController _personsController;
    private readonly Mock<IPersonsService> _personsServiceMock;

    public PersonsControllerTest()
    {
        _fixture = new Fixture();

        _personsServiceMock = new Mock<IPersonsService>();
        _countriesServiceMock = new Mock<ICountriesService>();
        _loggerMock = new Mock<ILogger<PersonsController>>();

        var personsService = _personsServiceMock.Object;
        var countriesService = _countriesServiceMock.Object;
        var logger = _loggerMock.Object;

        _personsController = new PersonsController(personsService, countriesService, logger);
    }

    #region Index

    [Fact]
    public async Task Index_ShouldReturnIndexViewWithPersonsList()
    {
        // Arrange
        var personResponseList = _fixture.Create<List<PersonResponse>>();

        _personsServiceMock
            .Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(personResponseList);

        _personsServiceMock
            .Setup(
                temp =>
                    temp.GetSortedPersons(
                        It.IsAny<List<PersonResponse>>(),
                        It.IsAny<string>(),
                        It.IsAny<SortOrderOptions>()
                    )
            )
            .ReturnsAsync(personResponseList);

        // Act
        var response = await _personsController.Index(
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<SortOrderOptions>()
        );

        // Assert
        response.Should().BeOfType<ViewResult>();
        var viewResult = response.As<ViewResult>();
        viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
        viewResult.ViewData.Model.Should().Be(personResponseList);
    }

    #endregion

    #region Create

    [Fact]
    public async Task Create_IfNoModelErrors_ShouldReturnCreateView()
    {
        // Arrange
        var personAddRequest = _fixture.Create<PersonAddRequest>();

        var personResponse = _fixture.Create<PersonResponse>();

        var countries = _fixture.Create<List<CountryResponse>>();

        _countriesServiceMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);

        _personsServiceMock
            .Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
            .ReturnsAsync(personResponse);

        // Act
        var response = await _personsController.Create(personAddRequest);

        // Assert
        response.Should().BeOfType<RedirectToActionResult>();
        var redirectResult = response.As<RedirectToActionResult>();
        redirectResult.ActionName.Should().Be("Index");
    }

    #endregion
}
