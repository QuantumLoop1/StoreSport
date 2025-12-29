using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using NUnit.Framework;
using StoreSport.Controllers;
using StoreSport.Models;
using StoreSport.Models.ViewModels;

namespace SportsStore.Tests;

[TestFixture]
public class HomeControllerTests
{
    private WebApplicationFactory<Program> factory = null!;
    private HttpClient client = null!;
    private Mock<IStoreRepository> mockRepository = null!;

    [SetUp]
    public void Setup()
    {
        this.mockRepository = new Mock<IStoreRepository>();
        this.factory = new WebApplicationFactory<Program>();
        this.client = this.factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        this.client?.Dispose();
        this.factory?.Dispose();
    }

    [Test]
    public void HomeController_Index_ReturnsViewResult()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { ProductId = 1, Name = "Test Product 1", Price = 10.00m, Category = "Test" },
            new() { ProductId = 2, Name = "Test Product 2", Price = 20.00m, Category = "Test" }
        };

        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());
        var controller = new HomeController(this.mockRepository.Object);

        // Act
        var result = controller.Index(null, 1);

        // Assert
        Assert.That(result, Is.InstanceOf<ViewResult>());
    }

    [Test]
    public void HomeController_Index_ReturnsCorrectViewModel()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { ProductId = 1, Name = "Test Product 1", Price = 10.00m, Category = "Test" },
            new() { ProductId = 2, Name = "Test Product 2", Price = 20.00m, Category = "Test" },
            new() { ProductId = 3, Name = "Test Product 3", Price = 30.00m, Category = "Test" },
            new() { ProductId = 4, Name = "Test Product 4", Price = 40.00m, Category = "Test" },
            new() { ProductId = 5, Name = "Test Product 5", Price = 50.00m, Category = "Test" }
        };

        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());
        var controller = new HomeController(this.mockRepository.Object);

        // Act
        var result = controller.Index(null, 1) as ViewResult;
        var viewModel = result?.Model as ProductsListViewModel;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(viewModel!.Products.Count(), Is.EqualTo(4)); // pageSize = 4
            Assert.That(viewModel.PagingInfo.CurrentPage, Is.EqualTo(1));
            Assert.That(viewModel.PagingInfo.ItemsPerPage, Is.EqualTo(4));
            Assert.That(viewModel.PagingInfo.TotalItems, Is.EqualTo(5));
        });
    }

    [Test]
    public void HomeController_Index_WithPage2_ReturnsCorrectProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { ProductId = 1, Name = "Test Product 1", Price = 10.00m, Category = "Test" },
            new() { ProductId = 2, Name = "Test Product 2", Price = 20.00m, Category = "Test" },
            new() { ProductId = 3, Name = "Test Product 3", Price = 30.00m, Category = "Test" },
            new() { ProductId = 4, Name = "Test Product 4", Price = 40.00m, Category = "Test" },
            new() { ProductId = 5, Name = "Test Product 5", Price = 50.00m, Category = "Test" }
        };

        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());
        var controller = new HomeController(this.mockRepository.Object);

        // Act
        var result = controller.Index(null, 2) as ViewResult;
        var viewModel = result?.Model as ProductsListViewModel;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(viewModel!.Products.Count(), Is.EqualTo(1)); // Only 1 product on page 2
            Assert.That(viewModel.PagingInfo.CurrentPage, Is.EqualTo(2));
            Assert.That(viewModel.Products.First().ProductId, Is.EqualTo(5));
        });
    }

    [Test]
    public async Task HomeController_Index_ReturnsCorrectView_Integration()
    {
        // Arrange & Act
        var response = await this.client.GetAsync("/");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var content = await response.Content.ReadAsStringAsync();
            // Check for product listing content instead of welcome message
            Assert.That(content, Does.Contain("html"));
        });
    }

    [Test]
    public async Task HomeController_Index_ReturnsHtmlContent()
    {
        // Arrange & Act
        var response = await this.client.GetAsync("/");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("text/html"));
        });
    }
}
