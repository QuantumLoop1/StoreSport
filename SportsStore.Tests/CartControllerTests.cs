using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using SportsStore.Models.Repository;
using SportsStore.Models.ViewModels;

namespace SportsStore.Tests;

// Wrapper for session testing

[TestFixture]
public class CartControllerTests
{
    private Mock<IStoreRepository> mockRepository = null!;
    private CartController controller = null!;
    private TestSession testSession = null!;
    private Mock<HttpContext> mockHttpContext = null!;

    [SetUp]
    public void Setup()
    {
        this.mockRepository = new Mock<IStoreRepository>();
        this.testSession = new TestSession();
        this.mockHttpContext = new Mock<HttpContext>();

        // Create a mock Cart for dependency injection
        var mockCart = new Cart();

        this.controller = new CartController(this.mockRepository.Object, mockCart);
        this.controller.ControllerContext = new ControllerContext { HttpContext = this.mockHttpContext.Object };

        this.mockHttpContext.Setup(c => c.Session).Returns(this.testSession);
    }

    [TearDown]
    public void TearDown()
    {
        this.controller.Dispose();
    }

    [Test]
    public void CartController_Index_GET_ReturnsViewWithEmptyCart()
    {
        // Arrange - testSession starts empty, so GetJson will return null

        // Act
        var result = this.controller.Index("/");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            Assert.That(viewResult!.Model, Is.InstanceOf<CartViewModel>());

            var viewModel = viewResult.Model as CartViewModel;
            Assert.That(viewModel!.Cart, Is.Not.Null);
            Assert.That(viewModel.Cart!.Lines, Is.Empty);
            Assert.That(viewModel.ReturnUrl, Is.EqualTo("/"));
        });
    }

    [Test]
    public void CartController_Index_GET_ReturnsViewWithExistingCart()
    {
        // Arrange
        var existingCart = new Cart();
        existingCart.AddItem(new Product { ProductId = 1, Name = "Test Product", Price = 10.00m }, 2);

        // Create a new controller with the cart that has items
        var controllerWithCart = new CartController(this.mockRepository.Object, existingCart);
        controllerWithCart.ControllerContext = new ControllerContext { HttpContext = this.mockHttpContext.Object };

        // Act
        var result = controllerWithCart.Index("/test");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            var viewModel = viewResult!.Model as CartViewModel;

            Assert.That(viewModel?.Cart?.Lines, Has.Count.EqualTo(1));
            Assert.That(viewModel?.Cart?.Lines[0].Product.Name, Is.EqualTo("Test Product"));
            Assert.That(viewModel!.ReturnUrl, Is.EqualTo("/test"));
        });
    }

    [Test]
    public void CartController_Index_GET_WithNullReturnUrl_UsesDefault()
    {
        // Arrange - testSession starts empty

        // Act
        var result = this.controller.Index(null!);

        // Assert
        Assert.Multiple(() =>
        {
            var viewResult = result as ViewResult;
            var viewModel = viewResult!.Model as CartViewModel;
            Assert.That(viewModel!.ReturnUrl, Is.EqualTo("Home"));
        });
    }

    [Test]
    public void CartController_Index_POST_WithValidProduct_AddsToCart()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 1,
            Name = "Test Product",
            Description = "Test Description",
            Price = 10.00m,
            Category = "Test"
        };

        var products = new List<Product> { product };
        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        var cart = new Cart();
        var controllerWithCart = new CartController(this.mockRepository.Object, cart);
        controllerWithCart.ControllerContext = new ControllerContext { HttpContext = this.mockHttpContext.Object };

        // Act
        var result = controllerWithCart.Index(1, "/test");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            var viewModel = viewResult!.Model as CartViewModel;

            Assert.That(viewModel?.Cart?.Lines, Has.Count.EqualTo(1));
            Assert.That(viewModel!.Cart!.Lines[0].Product.ProductId, Is.EqualTo(1));
            Assert.That(viewModel.Cart.Lines[0].Quantity, Is.EqualTo(1));
            Assert.That(viewModel.ReturnUrl, Is.EqualTo("/test"));

            // Verify that cart contains the product
            Assert.That(cart.Lines, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void CartController_Index_POST_WithExistingProduct_IncreasesQuantity()
    {
        // Arrange
        var product = new Product { ProductId = 1, Name = "Test Product", Price = 10.00m };

        var products = new List<Product> { product };
        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        var cart = new Cart();
        cart.AddItem(product, 2);
        var controllerWithCart = new CartController(this.mockRepository.Object, cart);
        controllerWithCart.ControllerContext = new ControllerContext { HttpContext = this.mockHttpContext.Object };

        // Act
        var result = controllerWithCart.Index(1, "/test");

        // Assert
        Assert.Multiple(() =>
        {
            var viewResult = result as ViewResult;
            var viewModel = viewResult!.Model as CartViewModel;

            Assert.That(viewModel?.Cart?.Lines, Has.Count.EqualTo(1));
            Assert.That(viewModel!.Cart!.Lines[0].Quantity, Is.EqualTo(3)); // 2 + 1
        });
    }

    [Test]
    public void CartController_Index_POST_WithNonExistentProduct_RedirectsToHome()
    {
        // Arrange
        var products = new List<Product>(); // Empty list, so product 999 won't be found
        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        // Act
        var result = this.controller.Index(999, "/test");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult!.ActionName, Is.EqualTo("Index"));
            Assert.That(redirectResult.ControllerName, Is.EqualTo("Home"));
        });
    }

    [Test]
    public void CartController_Index_POST_WithNullProduct_RedirectsToHome()
    {
        // Arrange
        var products = new List<Product>(); // Empty list, so product 1 won't be found
        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        // Act
        var result = this.controller.Index(1, "/test");

        // Assert
        Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
    }

    [Test]
    public void CartController_Index_POST_WithEmptyCart_CreatesNewCart()
    {
        // Arrange
        var product = new Product { ProductId = 1, Name = "Test Product", Price = 10.00m };

        var products = new List<Product> { product };
        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        // testSession starts empty, so GetJson will return null by default

        // Act
        var result = this.controller.Index(1, "/test");

        // Assert
        Assert.Multiple(() =>
        {
            var viewResult = result as ViewResult;
            var viewModel = viewResult!.Model as CartViewModel;

            Assert.That(viewModel!.Cart, Is.Not.Null);
            Assert.That(viewModel.Cart!.Lines, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void CartController_Index_POST_WithMultipleProducts_AddsToCorrectCart()
    {
        // Arrange
        var product1 = new Product { ProductId = 1, Name = "Product 1", Price = 10.00m };
        var product2 = new Product { ProductId = 2, Name = "Product 2", Price = 20.00m };

        var products = new List<Product> { product1, product2 };
        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        var cart = new Cart();
        cart.AddItem(product1, 1);
        var controllerWithCart = new CartController(this.mockRepository.Object, cart);
        controllerWithCart.ControllerContext = new ControllerContext { HttpContext = this.mockHttpContext.Object };

        // Act
        var result = controllerWithCart.Index(2, "/test");

        // Assert
        Assert.Multiple(() =>
        {
            var viewResult = result as ViewResult;
            var viewModel = viewResult!.Model as CartViewModel;

            Assert.That(viewModel?.Cart?.Lines, Has.Count.EqualTo(2));
            Assert.That(viewModel!.Cart!.Lines[0].Product.ProductId, Is.EqualTo(product1.ProductId));
            Assert.That(viewModel.Cart.Lines[1].Product.ProductId, Is.EqualTo(product2.ProductId));
        });
    }

    [Test]
    public void CartController_Index_POST_WithNullReturnUrl_UsesDefault()
    {
        // Arrange
        var product = new Product { ProductId = 1, Name = "Test Product", Price = 10.00m };
        var products = new List<Product> { product };
        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        var cart = new Cart();
        var controllerWithCart = new CartController(this.mockRepository.Object, cart);
        controllerWithCart.ControllerContext = new ControllerContext { HttpContext = this.mockHttpContext.Object };

        // Act
        var result = controllerWithCart.Index(1, null!);

        // Assert
        Assert.Multiple(() =>
        {
            var viewResult = result as ViewResult;
            var viewModel = viewResult!.Model as CartViewModel;
            Assert.That(viewModel!.ReturnUrl, Is.EqualTo("Home"));
        });
    }

    [Test]
    public void CartController_Index_POST_VerifiesRepositoryCall()
    {
        // Arrange
        var product = new Product { ProductId = 1, Name = "Test Product", Price = 10.00m };
        var products = new List<Product> { product };
        this.mockRepository.Setup(r => r.Products).Returns(products.AsQueryable());

        var cart = new Cart();
        var controllerWithCart = new CartController(this.mockRepository.Object, cart);
        controllerWithCart.ControllerContext = new ControllerContext { HttpContext = this.mockHttpContext.Object };

        // Act
        controllerWithCart.Index(1, "/test");

        // Assert
        this.mockRepository.Verify(r => r.Products, Times.Once);
    }

    [Test]
    public void CartController_Index_POST_WithException_HandlesGracefully()
    {
        // Arrange
        this.mockRepository.Setup(r => r.Products).Throws(new Exception("Database error"));

        // Act & Assert
        Assert.That(() => this.controller.Index(1, "/test"),
            Throws.Exception);
    }

    [Test]
    public void CartController_Remove_WithValidProductId_RemovesItemFromCart()
    {
        // Arrange
        var product = new Product { ProductId = 1, Name = "Test Product", Price = 10.00m };
        var cart = new Cart();
        cart.AddItem(product, 2);

        var controllerWithCart = new CartController(this.mockRepository.Object, cart);

        // Act
        var result = controllerWithCart.Remove(1, "/test");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ViewResult>());

            var viewResult = result as ViewResult;
            Assert.That(viewResult!.ViewName, Is.EqualTo("Index"));

            var viewModel = viewResult.Model as CartViewModel;
            Assert.That(viewModel!.Cart?.Lines, Is.Empty);
            Assert.That(viewModel.ReturnUrl, Is.EqualTo("/test"));
        });
    }

    [Test]
    public void CartController_Remove_WithNonExistentProductId_DoesNotThrowException()
    {
        // Arrange
        var product = new Product { ProductId = 1, Name = "Test Product", Price = 10.00m };
        var cart = new Cart();
        cart.AddItem(product, 2);

        var controllerWithCart = new CartController(this.mockRepository.Object, cart);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            var result = controllerWithCart.Remove(999, "/test");
            Assert.That(result, Is.Not.Null);
        });
    }

    [Test]
    public void CartController_Remove_WithNullReturnUrl_UsesDefault()
    {
        // Arrange
        var product = new Product { ProductId = 1, Name = "Test Product", Price = 10.00m };
        var cart = new Cart();
        cart.AddItem(product, 2);

        var controllerWithCart = new CartController(this.mockRepository.Object, cart);

        // Act
        var result = controllerWithCart.Remove(1, null!);

        // Assert
        Assert.Multiple(() =>
        {
            var viewResult = result as ViewResult;
            var viewModel = viewResult!.Model as CartViewModel;
            Assert.That(viewModel!.ReturnUrl, Is.EqualTo("Home"));
        });
    }

    [Test]
    public void CartController_Remove_WithMultipleItems_RemovesOnlySpecifiedItem()
    {
        // Arrange
        var product1 = new Product { ProductId = 1, Name = "Product 1", Price = 10.00m };
        var product2 = new Product { ProductId = 2, Name = "Product 2", Price = 15.00m };
        var cart = new Cart();
        cart.AddItem(product1, 2);
        cart.AddItem(product2, 1);

        var controllerWithCart = new CartController(this.mockRepository.Object, cart);

        // Act
        var result = controllerWithCart.Remove(1, "/test");

        // Assert
        Assert.Multiple(() =>
        {
            var viewResult = result as ViewResult;
            var viewModel = viewResult!.Model as CartViewModel;
            Assert.That(viewModel!.Cart?.Lines, Has.Count.EqualTo(1));
            Assert.That(viewModel.Cart?.Lines.First().Product.ProductId, Is.EqualTo(2));
        });
    }
}
