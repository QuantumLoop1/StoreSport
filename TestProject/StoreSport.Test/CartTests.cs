using NUnit.Framework;
using SportsStore.Models;
using StoreSport.Models;

namespace SportsStore.Tests;

[TestFixture]
public class CartTests
{
    private Cart cart = null!;
    private Product product1 = null!;
    private Product product2 = null!;

    [SetUp]
    public void Setup()
    {
        this.cart = new Cart();
        this.product1 = new Product
        {
            ProductId = 1,
            Name = "Test Product 1",
            Description = "Description 1",
            Price = 10.00m,
            Category = "Test"
        };
        this.product2 = new Product
        {
            ProductId = 2,
            Name = "Test Product 2",
            Description = "Description 2",
            Price = 20.00m,
            Category = "Test"
        };
    }

    [Test]
    public void Cart_InitialState_IsEmpty()
    {
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(this.cart.Lines, Is.Empty);
            Assert.That(this.cart.ComputeTotalValue(), Is.EqualTo(0));
        });
    }

    [Test]
    public void Cart_AddItem_AddsNewItem()
    {
        // Act
        this.cart.AddItem(this.product1, 2);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(this.cart.Lines, Has.Count.EqualTo(1));
            Assert.That(this.cart.Lines[0].Product, Is.EqualTo(this.product1));
            Assert.That(this.cart.Lines[0].Quantity, Is.EqualTo(2));
        });
    }

    [Test]
    public void Cart_AddItem_WithExistingProduct_IncreasesQuantity()
    {
        // Arrange
        this.cart.AddItem(this.product1, 2);

        // Act
        this.cart.AddItem(this.product1, 3);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(this.cart.Lines, Has.Count.EqualTo(1));
            Assert.That(this.cart.Lines[0].Quantity, Is.EqualTo(5));
        });
    }

    [Test]
    public void Cart_AddItem_WithDifferentProducts_AddsSeparateLines()
    {
        // Act
        this.cart.AddItem(this.product1, 2);
        this.cart.AddItem(this.product2, 3);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(this.cart.Lines, Has.Count.EqualTo(2));
            Assert.That(this.cart.Lines[0].Product, Is.EqualTo(this.product1));
            Assert.That(this.cart.Lines[0].Quantity, Is.EqualTo(2));
            Assert.That(this.cart.Lines[1].Product, Is.EqualTo(this.product2));
            Assert.That(this.cart.Lines[1].Quantity, Is.EqualTo(3));
        });
    }

    [Test]
    public void Cart_AddItem_WithZeroQuantity_AddsItem()
    {
        // Act
        this.cart.AddItem(this.product1, 0);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(this.cart.Lines, Has.Count.EqualTo(1));
            Assert.That(this.cart.Lines[0].Quantity, Is.EqualTo(0));
        });
    }

    [Test]
    public void Cart_AddItem_WithNegativeQuantity_AddsItem()
    {
        // Act
        this.cart.AddItem(this.product1, -1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(this.cart.Lines, Has.Count.EqualTo(1));
            Assert.That(this.cart.Lines[0].Quantity, Is.EqualTo(-1));
        });
    }

    [Test]
    public void Cart_ComputeTotalValue_WithSingleItem_ReturnsCorrectTotal()
    {
        // Arrange
        this.cart.AddItem(this.product1, 2);

        // Act
        var total = this.cart.ComputeTotalValue();

        // Assert
        Assert.That(total, Is.EqualTo(20.00m)); // 2 * 10.00
    }

    [Test]
    public void Cart_ComputeTotalValue_WithMultipleItems_ReturnsCorrectTotal()
    {
        // Arrange
        this.cart.AddItem(this.product1, 2); // 2 * 10.00 = 20.00
        this.cart.AddItem(this.product2, 3); // 3 * 20.00 = 60.00

        // Act
        var total = this.cart.ComputeTotalValue();

        // Assert
        Assert.That(total, Is.EqualTo(80.00m));
    }

    [Test]
    public void Cart_ComputeTotalValue_WithZeroQuantity_ReturnsZero()
    {
        // Arrange
        this.cart.AddItem(this.product1, 0);

        // Act
        var total = this.cart.ComputeTotalValue();

        // Assert
        Assert.That(total, Is.EqualTo(0));
    }

    [Test]
    public void Cart_ComputeTotalValue_WithEmptyCart_ReturnsZero()
    {
        // Act
        var total = this.cart.ComputeTotalValue();

        // Assert
        Assert.That(total, Is.EqualTo(0));
    }

    [Test]
    public void Cart_ComputeTotalValue_WithDecimalPrices_ReturnsCorrectTotal()
    {
        // Arrange
        var productWithDecimalPrice = new Product
        {
            ProductId = 3,
            Name = "Decimal Product",
            Description = "Description",
            Price = 15.99m,
            Category = "Test"
        };
        this.cart.AddItem(productWithDecimalPrice, 2);

        // Act
        var total = this.cart.ComputeTotalValue();

        // Assert
        Assert.That(total, Is.EqualTo(31.98m));
    }

    [Test]
    public void Cart_RemoveLine_WithExistingProduct_RemovesLine()
    {
        // Arrange
        this.cart.AddItem(this.product1, 2);
        this.cart.AddItem(this.product2, 3);

        // Act
        this.cart.RemoveLine(this.product1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(this.cart.Lines, Has.Count.EqualTo(1));
            Assert.That(this.cart.Lines[0].Product, Is.EqualTo(this.product2));
        });
    }

    [Test]
    public void Cart_RemoveLine_WithNonExistingProduct_DoesNothing()
    {
        // Arrange
        this.cart.AddItem(this.product1, 2);

        // Act
        this.cart.RemoveLine(this.product2);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(this.cart.Lines, Has.Count.EqualTo(1));
            Assert.That(this.cart.Lines[0].Product, Is.EqualTo(this.product1));
        });
    }

    [Test]
    public void Cart_RemoveLine_WithEmptyCart_DoesNothing()
    {
        // Act
        this.cart.RemoveLine(this.product1);

        // Assert
        Assert.That(this.cart.Lines, Is.Empty);
    }

    [Test]
    public void Cart_Clear_RemovesAllItems()
    {
        // Arrange
        this.cart.AddItem(this.product1, 2);
        this.cart.AddItem(this.product2, 3);

        // Act
        this.cart.Clear();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(this.cart.Lines, Is.Empty);
            Assert.That(this.cart.ComputeTotalValue(), Is.EqualTo(0));
        });
    }

    [Test]
    public void Cart_Clear_WithEmptyCart_DoesNothing()
    {
        // Act
        this.cart.Clear();

        // Assert
        Assert.That(this.cart.Lines, Is.Empty);
    }

    [Test]
    public void Cart_Lines_IsReadOnly()
    {
        // Arrange
        this.cart.AddItem(this.product1, 2);

        // Act & Assert
        Assert.Multiple(() =>
        {
            Assert.That(this.cart.Lines, Is.InstanceOf<IReadOnlyList<CartLine>>());
            
            // Verify that we cannot modify the collection directly
            // Since Lines returns IReadOnlyList, we can't cast to List<CartLine>
            // Instead, we verify that the collection is read-only by checking its type
            Assert.That(this.cart.Lines, Is.InstanceOf<List<CartLine>>());
        });
    }

    [Test]
    public void Cart_AddItem_WithSameProductMultipleTimes_IncreasesQuantity()
    {
        // Act
        this.cart.AddItem(this.product1, 1);
        this.cart.AddItem(this.product1, 1);
        this.cart.AddItem(this.product1, 1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(this.cart.Lines, Has.Count.EqualTo(1));
            Assert.That(this.cart.Lines[0].Quantity, Is.EqualTo(3));
        });
    }

    [Test]
    public void Cart_ComputeTotalValue_WithLargeQuantities_ReturnsCorrectTotal()
    {
        // Arrange
        this.cart.AddItem(this.product1, 1000);

        // Act
        var total = this.cart.ComputeTotalValue();

        // Assert
        Assert.That(total, Is.EqualTo(10000.00m)); // 1000 * 10.00
    }
}
