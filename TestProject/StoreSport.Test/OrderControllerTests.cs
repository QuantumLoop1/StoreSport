using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using StoreSport.Controllers;
using StoreSport.Models;

namespace SportsStore.Tests;

[TestFixture]
public class OrderControllerTests
{
  private Mock<IOrderRepository> mockOrderRepository = null!;
    private Cart testCart = null!;
    private OrderController controller = null!;

    [SetUp]
    public void Setup()
    {
     this.mockOrderRepository = new Mock<IOrderRepository>();
        this.testCart = new Cart();
        this.controller = new OrderController(this.mockOrderRepository.Object, this.testCart);
    }

    [TearDown]
    public void TearDown()
    {
     this.controller.Dispose();
    }

    [Test]
    public void Checkout_GET_ReturnsViewWithNewOrder()
    {
 // Act
  var result = this.controller.Checkout();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ViewResult>());
            Assert.That(result.Model, Is.InstanceOf<Order>());
   });
    }

    [Test]
    public void Checkout_POST_WithEmptyCart_ReturnsViewWithModelError()
  {
  // Arrange
 var order = new Order { Name = "Test User" };
      // testCart is empty by default

    // Act
var result = this.controller.Checkout(order);

      // Assert
        Assert.Multiple(() =>
        {
     Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.InstanceOf<ViewResult>());
    Assert.That(this.controller.ModelState.IsValid, Is.False);
      Assert.That(this.controller.ModelState.ContainsKey(""), Is.True);
        });
    }

    [Test]
    public void Checkout_POST_WithValidOrder_SavesOrderAndClearsCart()
 {
   // Arrange
 var order = new Order
        {
     Name = "Test User",
   Address = "Test Address",
  City = "Test City",
 State = "Test State",
            Country = "Test Country"
  };

  var product = new Product { ProductId = 1, Name = "Test Product", Description = "Desc", Category = "Test", Price = 10.00m };
  this.testCart.AddItem(product, 2);

     // Act
        var result = this.controller.Checkout(order);

        // Assert
    this.mockOrderRepository.Verify(x => x.SaveOrder(It.IsAny<Order>()), Times.Once);

    Assert.Multiple(() =>
        {
    Assert.That(result, Is.Not.Null);
 Assert.That(result, Is.InstanceOf<RedirectToPageResult>());

    // Verify cart was cleared
            Assert.That(this.testCart.Lines, Is.Empty);
    });
    }

    [Test]
    public void Checkout_POST_WithInvalidModel_ReturnsView()
  {
 // Arrange
        var order = new Order(); // Invalid - missing required fields
        var product = new Product { ProductId = 1, Name = "Test Product", Description = "Desc", Category = "Test", Price = 10.00m };
        this.testCart.AddItem(product, 2);

      // Manually add model error to simulate validation failure
 this.controller.ModelState.AddModelError("Name", "Name is required");

        // Act
   var result = this.controller.Checkout(order);

        // Assert
        Assert.Multiple(() =>
        {
     Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ViewResult>());
            Assert.That(this.controller.ModelState.IsValid, Is.False);
        });

    this.mockOrderRepository.Verify(x => x.SaveOrder(It.IsAny<Order>()), Times.Never);
    }

    [Test]
    public void Checkout_POST_WithValidOrder_SetsOrderLinesFromCart()
    {
        // Arrange
   var order = new Order
        {
        Name = "Test User",
       Address = "Test Address",
     City = "Test City",
          State = "Test State",
    Country = "Test Country"
    };

 var product = new Product { ProductId = 1, Name = "Test Product", Description = "Desc", Category = "Test", Price = 10.00m };
  this.testCart.AddItem(product, 2);

        // Act
        this.controller.Checkout(order);

    // Assert
        Assert.Multiple(() =>
        {
 Assert.That(order.Lines, Is.Not.Null);
          Assert.That(order.Lines, Has.Count.EqualTo(1));
          Assert.That(order.Lines.First().Product?.Name, Is.EqualTo("Test Product"));
     Assert.That(order.Lines.First().Quantity, Is.EqualTo(2));
        });
    }
}
