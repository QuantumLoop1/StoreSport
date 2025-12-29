using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;
using SportsStore.Components;
using SportsStore.Models;
using SportsStore.Models.Repository;

namespace SportsStore.Tests;

[TestFixture]
public class NavigationMenuViewComponentTests
{
    private Mock<IStoreRepository> mockRepository = null!;
    private NavigationMenuViewComponent viewComponent = null!;
    private ViewComponentContext viewComponentContext = null!;

    [SetUp]
    public void Setup()
    {
        this.mockRepository = new Mock<IStoreRepository>();
        this.viewComponent = new NavigationMenuViewComponent(this.mockRepository.Object);

        // Setup ViewComponentContext
        var httpContext = new DefaultHttpContext();
        var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

        this.viewComponentContext = new ViewComponentContext
        {
            ViewContext = new ViewContext
            {
                HttpContext = httpContext,
                ViewData = viewData,
                TempData = tempData,
                RouteData = new RouteData()
            }
        };

        // Use reflection to set the ViewContext since it's read-only
        var viewContextField = typeof(ViewComponent).GetField("_viewContext",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        viewContextField?.SetValue(this.viewComponent, this.viewComponentContext.ViewContext);
    }

    [Test]
    public void NavigationMenuViewComponent_Invoke_ReturnsViewWithCategories()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { ProductId = 1, Name = "Product 1", Category = "Watersports" },
            new() { ProductId = 2, Name = "Product 2", Category = "Soccer" },
            new() { ProductId = 3, Name = "Product 3", Category = "Chess" },
            new() { ProductId = 4, Name = "Product 4", Category = "Watersports" },
            new() { ProductId = 5, Name = "Product 5", Category = "Soccer" }
        };

        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        // Act
        var result = this.viewComponent.Invoke();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ViewViewComponentResult>());
            var viewResult = result as ViewViewComponentResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult?.ViewData?.Model, Is.InstanceOf<IEnumerable<string>>());

            var categories = viewResult!.ViewData!.Model as IEnumerable<string>;
            var enumerable = categories!.ToList();
            Assert.That(enumerable, Is.Not.Null);
            Assert.That(enumerable?.Count, Is.EqualTo(3));
            Assert.That(enumerable, Contains.Item("Chess"));
            Assert.That(enumerable, Contains.Item("Soccer"));
            Assert.That(enumerable, Contains.Item("Watersports"));
        });
    }

    [Test]
    public void NavigationMenuViewComponent_Invoke_ReturnsCategoriesInAlphabeticalOrder()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { ProductId = 1, Name = "Product 1", Category = "Zebra" },
            new() { ProductId = 2, Name = "Product 2", Category = "Alpha" },
            new() { ProductId = 3, Name = "Product 3", Category = "Beta" }
        };

        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        // Act
        var result = this.viewComponent.Invoke();

        // Assert
        Assert.Multiple(() =>
        {
            var viewResult = result as ViewViewComponentResult;
            var categories = viewResult?.ViewData?.Model as IEnumerable<string>;
            var categoriesList = categories?.ToList();

            Assert.That(categoriesList?[0], Is.EqualTo("Alpha"));
            Assert.That(categoriesList![1], Is.EqualTo("Beta"));
            Assert.That(categoriesList[2], Is.EqualTo("Zebra"));
        });
    }

    [Test]
    public void NavigationMenuViewComponent_Invoke_ReturnsDistinctCategories()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { ProductId = 1, Name = "Product 1", Category = "Soccer" },
            new() { ProductId = 2, Name = "Product 2", Category = "Soccer" },
            new() { ProductId = 3, Name = "Product 3", Category = "Soccer" },
            new() { ProductId = 4, Name = "Product 4", Category = "Chess" }
        };

        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        // Act
        var result = this.viewComponent.Invoke();

        // Assert
        Assert.Multiple(() =>
        {
            var viewResult = result as ViewViewComponentResult;
            var categories = viewResult?.ViewData?.Model as IEnumerable<string>;
            var categoriesList = categories?.ToList();

            Debug.Assert(categoriesList != null, nameof(categoriesList) + " != null");
            Assert.That(categoriesList, Has.Count.EqualTo(2));
            Assert.That(categoriesList, Contains.Item("Chess"));
            Assert.That(categoriesList, Contains.Item("Soccer"));
        });
    }

    [Test]
    public void NavigationMenuViewComponent_Invoke_HandlesEmptyProductList()
    {
        // Arrange
        var products = new List<Product>();
        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        // Act
        var result = this.viewComponent.Invoke();

        // Assert
        Assert.Multiple(() =>
        {
            var viewResult = result as ViewViewComponentResult;
            var categories = viewResult?.ViewData?.Model as IEnumerable<string>;

            Assert.That(categories, Is.Not.Null);
            Assert.That(categories?.Count(), Is.EqualTo(0));
        });
    }

    [Test]
    public void NavigationMenuViewComponent_Invoke_SetsSelectedCategoryInViewBag()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { ProductId = 1, Name = "Product 1", Category = "Soccer" }
        };

        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        // Set up route data with category
        this.viewComponentContext.ViewContext.RouteData.Values["category"] = "Soccer";

        // Create a new ViewComponentContext with the updated RouteData
        var newViewComponentContext = new ViewComponentContext
        {
            ViewContext = this.viewComponentContext.ViewContext
        };

        // Update the ViewComponent's ViewComponentContext using reflection
        var viewComponentContextField = typeof(ViewComponent).GetField("_viewComponentContext",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        viewComponentContextField?.SetValue(this.viewComponent, newViewComponentContext);

        // Act
        var result = this.viewComponent.Invoke();

        // Assert
        var viewResult = result as ViewViewComponentResult;
        Assert.That(viewResult?.ViewData?["SelectedCategory"], Is.EqualTo("Soccer"));
    }

    [Test]
    public void NavigationMenuViewComponent_Invoke_HandlesNullCategoryInRouteData()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { ProductId = 1, Name = "Product 1", Category = "Soccer" }
        };

        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        // Route data without category
        this.viewComponentContext.ViewContext.RouteData.Values.Clear();

        // Act
        var result = this.viewComponent.Invoke();

        // Assert
        var viewResult = result as ViewViewComponentResult;
        Assert.That(viewResult?.ViewData?["SelectedCategory"], Is.Null);
    }

    [Test]
    public void NavigationMenuViewComponent_Invoke_HandlesNullRouteData()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { ProductId = 1, Name = "Product 1", Category = "Soccer" }
        };

        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        // Set RouteData to null using reflection
        var routeDataField = typeof(ViewComponent).GetField("_routeData",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        routeDataField?.SetValue(this.viewComponent, null);

        // Act
        var result = this.viewComponent.Invoke();

        // Assert
        var viewResult = result as ViewViewComponentResult;
        Assert.That(viewResult?.ViewData?["SelectedCategory"], Is.Null);
    }

    [Test]
    public void NavigationMenuViewComponent_Invoke_HandlesProductsWithNullCategories()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { ProductId = 1, Name = "Product 1", Category = "Soccer" },
            new() { ProductId = 2, Name = "Product 2", Category = null! },
            new() { ProductId = 3, Name = "Product 3", Category = "Chess" },
            new() { ProductId = 4, Name = "Product 4", Category = "Soccer" } // Duplicate category
        };

        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        // Act
        var result = this.viewComponent.Invoke();

        // Assert
        Assert.Multiple(() =>
        {
            var viewResult = result as ViewViewComponentResult;
            var categories = viewResult?.ViewData?.Model as IEnumerable<string>;
            var categoriesList = categories?.ToList();

            // Should have 3 unique categories (Soccer, Chess, and null), null should be included
            Debug.Assert(categoriesList != null, nameof(categoriesList) + " != null");
            Assert.That(categoriesList, Has.Count.EqualTo(3));
            Assert.That(categoriesList, Contains.Item("Chess"));
            Assert.That(categoriesList, Contains.Item("Soccer"));
            Assert.That(categoriesList, Contains.Item(null));
        });
    }
}
