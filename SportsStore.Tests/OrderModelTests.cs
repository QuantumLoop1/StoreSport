using System.ComponentModel.DataAnnotations;
using SportsStore.Models;

namespace SportsStore.Tests;

[TestFixture]
public class OrderModelTests
{
    [Test]
    public void Order_DefaultValues_AreCorrect()
    {
        // Act
        var order = new Order();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(order.OrderId, Is.EqualTo(0));
            Assert.That(order.Lines, Is.Not.Null);
            Assert.That(order.Lines, Is.Empty);
            Assert.That(order.GiftWrap, Is.False);
        });
    }

    [Test]
    public void Order_RequiredFields_AreValidated()
    {
        // Arrange
        var order = new Order();
        var context = new ValidationContext(order);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(order, context, results, true);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.False);
            Assert.That(results, Has.Count.GreaterThan(0));
        });

        var requiredFields = new[] { "Name", "Line1", "City", "State", "Country" };
        foreach (var field in requiredFields)
        {
            Assert.That(results.Any(r => r.MemberNames.Contains(field)), Is.True,
                $"Field {field} should be required");
        }
    }

    [Test]
    public void Order_WithValidData_IsValid()
    {
        // Arrange
        var order = new Order
        {
            Name = "Test User",
            Line1 = "123 Test Street",
            Line2 = "Apt 1",
            Line3 = "Building A",
            City = "Test City",
            State = "Test State",
            Zip = "12345",
            Country = "Test Country",
            GiftWrap = true
        };

        var context = new ValidationContext(order);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(order, context, results, true);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(results, Is.Empty);
        });
    }

    [Test]
    public void Order_Lines_CanBeSetAndRetrieved()
    {
        // Arrange
        var order = new Order();
        var cartLine = new CartLine
        {
            Product = new Product { ProductId = 1, Name = "Test Product", Price = 10.00m },
            Quantity = 2
        };

        // Act
        order.Lines.Add(cartLine);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(order.Lines, Has.Count.EqualTo(1));
            Assert.That(order.Lines.First().Product.Name, Is.EqualTo("Test Product"));
            Assert.That(order.Lines.First().Quantity, Is.EqualTo(2));
        });
    }

    [Test]
    public void Order_OptionalFields_CanBeNull()
    {
        // Arrange
        var order = new Order
        {
            Name = "Test User",
            Line1 = "123 Test Street",
            City = "Test City",
            State = "Test State",
            Country = "Test Country",
            Line2 = null,
            Line3 = null,
            Zip = null
        };

        var context = new ValidationContext(order);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(order, context, results, true);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(results, Is.Empty);
        });
    }
}
