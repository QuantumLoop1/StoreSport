using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using StoreSport.Components;
using StoreSport.Models;

namespace SportsStore.Tests;

[TestFixture]
public class NavigationMenuViewComponentTests
{
    private Mock<IStoreRepository> mockRepository = null!;
    private NavigationMenuViewComponent viewComponent = null!;

    [SetUp]
    public void Setup()
    {
        mockRepository = new Mock<IStoreRepository>();
        viewComponent = new NavigationMenuViewComponent(mockRepository.Object);


        var httpContext = new DefaultHttpContext();
        var routeData = new RouteData();
        var actionContext = new ActionContext(
            httpContext,
            routeData,
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
        );

        var viewData = new ViewDataDictionary(
            new EmptyModelMetadataProvider(),
            new ModelStateDictionary()
        );

        var tempData = new TempDataDictionary(
            httpContext,
            Mock.Of<ITempDataProvider>()
        );

        var viewContext = new ViewContext(
            actionContext,
            Mock.Of<IView>(),
            viewData,
            tempData,
            TextWriter.Null,
            new HtmlHelperOptions()
        );

        viewComponent.ViewComponentContext = new ViewComponentContext
        {
            ViewContext = viewContext
        };
    }

    [Test]
    public void NavigationMenuViewComponent_Invoke_ReturnsViewWithCategories()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { ProductId = 1, Name = "Product 1", Category = "Watersports", Price = 10m, Description = "Desc 1" },
            new() { ProductId = 2, Name = "Product 2", Category = "Soccer",      Price = 20m, Description = "Desc 2" },
            new() { ProductId = 3, Name = "Product 3", Category = "Chess",       Price = 30m, Description = "Desc 3" },
            new() { ProductId = 4, Name = "Product 4", Category = "Watersports", Price = 40m, Description = "Desc 4" },
            new() { ProductId = 5, Name = "Product 5", Category = "Soccer",      Price = 50m, Description = "Desc 5" }
        };

        mockRepository
            .Setup(r => r.Products)
            .Returns(products.AsQueryable());

        // Act
        var result = viewComponent.Invoke();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ViewViewComponentResult>());

            var viewResult = result as ViewViewComponentResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult!.ViewData!.Model, Is.InstanceOf<IEnumerable<string>>());

            var categories = ((IEnumerable<string>)viewResult.ViewData.Model!).ToList();

            Assert.That(categories.Count, Is.EqualTo(3));
            Assert.That(categories, Contains.Item("Chess"));
            Assert.That(categories, Contains.Item("Soccer"));
            Assert.That(categories, Contains.Item("Watersports"));
        });
    }

    [Test]
    public void NavigationMenuViewComponent_Invoke_ReturnsCategoriesInAlphabeticalOrder()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { ProductId = 1, Name = "Product 1", Category = "Zebra", Price = 10m, Description = "Desc 1" },
            new() { ProductId = 2, Name = "Product 2", Category = "Alpha", Price = 20m, Description = "Desc 2" },
            new() { ProductId = 3, Name = "Product 3", Category = "Beta",  Price = 30m, Description = "Desc 3" }
        };

        mockRepository
            .Setup(r => r.Products)
            .Returns(products.AsQueryable());

        // Act
        var result = viewComponent.Invoke();

        // Assert
        Assert.Multiple(() =>
        {
            var viewResult = result as ViewViewComponentResult;
            var categories = ((IEnumerable<string>)viewResult!.ViewData!.Model!).ToList();

            Assert.That(categories[0], Is.EqualTo("Alpha"));
            Assert.That(categories[1], Is.EqualTo("Beta"));
            Assert.That(categories[2], Is.EqualTo("Zebra"));
        });
    }

    [Test]
    public void NavigationMenuViewComponent_Invoke_ReturnsDistinctCategories()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { ProductId = 1, Name = "Product 1", Category = "Soccer", Price = 10m },
            new() { ProductId = 2, Name = "Product 2", Category = "Soccer", Price = 20m },
            new() { ProductId = 3, Name = "Product 3", Category = "Soccer", Price = 30m },
            new() { ProductId = 4, Name = "Product 4", Category = "Chess",  Price = 40m }
        };

        mockRepository
            .Setup(r => r.Products)
            .Returns(products.AsQueryable());

        // Act
        var result = viewComponent.Invoke();

        // Assert
        Assert.Multiple(() =>
        {
            var viewResult = result as ViewViewComponentResult;
            var categories = ((IEnumerable<string>)viewResult!.ViewData!.Model!).ToList();

            Assert.That(categories, Has.Count.EqualTo(2));
            Assert.That(categories, Contains.Item("Chess"));
            Assert.That(categories, Contains.Item("Soccer"));
        });
    }

    [Test]
    public void NavigationMenuViewComponent_Invoke_HandlesEmptyProductList()
    {
        // Arrange
        mockRepository
            .Setup(r => r.Products)
            .Returns(new List<Product>().AsQueryable());

        // Act
        var result = viewComponent.Invoke();

        // Assert
        var viewResult = result as ViewViewComponentResult;
        var categories = (IEnumerable<string>)viewResult!.ViewData!.Model!;

        Assert.That(categories, Is.Empty);
    }

    [Test]
    public void NavigationMenuViewComponent_Invoke_SetsSelectedCategoryInViewBag()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { ProductId = 1, Name = "Product 1", Category = "Soccer", Price = 10m }
        };

        mockRepository
            .Setup(r => r.Products)
            .Returns(products.AsQueryable());

        var httpContext = new DefaultHttpContext();
        var routeData = new RouteData();
        routeData.Values["category"] = "Soccer";

        var actionContext = new ActionContext(
            httpContext,
            routeData,
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
        );

        var viewContext = new ViewContext(
            actionContext,
            Mock.Of<IView>(),
            new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()),
            new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>()),
            TextWriter.Null,
            new HtmlHelperOptions()
        );

        viewComponent.ViewComponentContext = new ViewComponentContext
        {
            ViewContext = viewContext
        };

        // Act
        var result = viewComponent.Invoke();

        // Assert
        var viewResult = result as ViewViewComponentResult;
        Assert.That(viewResult!.ViewData["SelectedCategory"], Is.EqualTo("Soccer"));
    }
}
