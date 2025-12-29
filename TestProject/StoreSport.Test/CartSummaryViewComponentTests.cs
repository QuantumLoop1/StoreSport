using Microsoft.AspNetCore.Mvc.ViewComponents;
using NUnit.Framework;
using StoreSport.Components;
using StoreSport.Models;

namespace SportsStore.Tests;

[TestFixture]
public class CartSummaryViewComponentTests
{
    private Cart testCart = null!;
    private CartSummaryViewComponent component = null!;

    [SetUp]
    public void Setup()
    {
        this.testCart = new Cart();
        this.component = new CartSummaryViewComponent(this.testCart);
    }

    [Test]
    public void Invoke_ReturnsViewWithCart()
    {
        // Arrange
        var product = new Product { ProductId = 1, Name = "Test Product", Price = 10.00m };
        this.testCart.AddItem(product, 2);

        // Act
        var result = this.component.Invoke();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ViewViewComponentResult>());
        });
    }

    [Test]
    public void Invoke_WithEmptyCart_ReturnsViewWithEmptyCart()
    {
        // Act
        var result = this.component.Invoke();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ViewViewComponentResult>());
            Assert.That(this.testCart.Lines, Is.Empty);
        });
    }

    [Test]
    public void Invoke_WithMultipleItems_ReturnsViewWithCorrectCart()
    {
        // Arrange
        var product1 = new Product { ProductId = 1, Name = "Product 1", Price = 10.00m };
        var product2 = new Product { ProductId = 2, Name = "Product 2", Price = 15.00m };
        this.testCart.AddItem(product1, 2);
        this.testCart.AddItem(product2, 1);

        // Act
        var result = this.component.Invoke();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ViewViewComponentResult>());
            Assert.That(this.testCart.Lines, Has.Count.EqualTo(2));
        });
    }

    [Test]
    public void Invoke_WithCartItems_ComputesCorrectTotal()
    {
        // Arrange
        var product1 = new Product { ProductId = 1, Name = "Product 1", Price = 10.00m };
        var product2 = new Product { ProductId = 2, Name = "Product 2", Price = 15.00m };
        this.testCart.AddItem(product1, 2); // 20.00
        this.testCart.AddItem(product2, 1); // 15.00

        // Act
        var result = this.component.Invoke();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(this.testCart.ComputeTotalValue(), Is.EqualTo(35.00m));
        });
    }
}