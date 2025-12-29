using NUnit.Framework;
using SportsStore.Models;
using StoreSport.Models;

namespace SportsStore.Tests;

[TestFixture]
public class SessionCartTests
{
    [Test]
    public void SessionCart_InheritsFromCart()
    {
        // Arrange & Act
        var cart = new SessionCart();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(cart, Is.InstanceOf<Cart>());
            Assert.That(cart.Lines, Is.Not.Null);
        });
    }

    [Test]
    public void SessionCart_CanAddItems()
    {
        // Arrange
        var cart = new SessionCart();
        var product = new Product { ProductId = 1, Name = "Test Product", Price = 10.00m };

        // Act
        cart.AddItem(product, 2);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(cart.Lines, Has.Count.EqualTo(1));
            Assert.That(cart.Lines[0].Product.Name, Is.EqualTo("Test Product"));
            Assert.That(cart.Lines[0].Quantity, Is.EqualTo(2));
        });
    }

    [Test]
    public void SessionCart_CanRemoveItems()
    {
        // Arrange
        var cart = new SessionCart();
        var product = new Product { ProductId = 1, Name = "Test Product", Price = 10.00m };
        cart.AddItem(product, 2);

        // Act
        cart.RemoveLine(product);

        // Assert
        Assert.That(cart.Lines, Is.Empty);
    }

    [Test]
    public void SessionCart_CanClearItems()
    {
        // Arrange
        var cart = new SessionCart();
        var product = new Product { ProductId = 1, Name = "Test Product", Price = 10.00m };
        cart.AddItem(product, 2);

        // Act
        cart.Clear();

        // Assert
        Assert.That(cart.Lines, Is.Empty);
    }

    [Test]
    public void SessionCart_CanAddMultipleItems()
    {
        // Arrange
        var cart = new SessionCart();
        var product1 = new Product { ProductId = 1, Name = "Product 1", Price = 10.00m };
        var product2 = new Product { ProductId = 2, Name = "Product 2", Price = 15.00m };

        // Act
        cart.AddItem(product1, 2);
        cart.AddItem(product2, 1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(cart.Lines, Has.Count.EqualTo(2));
            Assert.That(cart.ComputeTotalValue(), Is.EqualTo(35.00m));
        });
    }

    [Test]
    public void SessionCart_CanUpdateQuantity()
    {
        // Arrange
        var cart = new SessionCart();
        var product = new Product { ProductId = 1, Name = "Test Product", Price = 10.00m };
        cart.AddItem(product, 2);

        // Act
        cart.AddItem(product, 1); // Should update quantity to 3

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(cart.Lines, Has.Count.EqualTo(1));
            Assert.That(cart.Lines[0].Quantity, Is.EqualTo(3));
        });
    }

    [Test]
    public void SessionCart_WithNullSession_DoesNotThrow()
    {
        // Arrange
        var cart = new SessionCart { Session = null };
        var product = new Product { ProductId = 1, Name = "Test Product", Price = 10.00m };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            cart.AddItem(product, 2);
            cart.RemoveLine(product);
            cart.Clear();
        });
    }
}
