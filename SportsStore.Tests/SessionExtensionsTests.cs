using Microsoft.AspNetCore.Http;
using SportsStore.Models;

namespace SportsStore.Tests;

[TestFixture]
public class SessionExtensionsTests
{
    private TestSession testSession = null!;
    private Cart testCart = null!;

    [SetUp]
    public void Setup()
    {
        this.testSession = new TestSession();
        this.testCart = new Cart();
        this.testCart.AddItem(new Product
        {
            ProductId = 1,
            Name = "Test Product",
            Description = "Test Description",
            Price = 10.00m,
            Category = "Test"
        }, 2);
    }

    [Test]
    public void SetJson_WithValidObject_StoresJsonString()
    {
        // Act
        this.testSession.SetJson("testKey", this.testCart);

        // Assert
        Assert.Multiple(() =>
        {
            var storedJson = this.testSession.GetJson<Cart>("testKey");
            Assert.That(storedJson, Is.Not.Null);
            Assert.That(storedJson!.Lines, Has.Count.EqualTo(1));
            Assert.That(storedJson.Lines[0].Product.Name, Is.EqualTo("Test Product"));
        });
    }

    [Test]
    public void SetJson_WithNullObject_StoresNullJson()
    {
        // Act
        this.testSession.SetJson("testKey", (Cart?)null!);

        // Assert
        var storedJson = this.testSession.GetJson<Cart>("testKey");
        Assert.That(storedJson, Is.Null);
    }

    [Test]
    public void SetJson_WithPrimitiveType_StoresJsonString()
    {
        // Act
        this.testSession.SetJson("testKey", 42);

        // Assert
        var storedValue = this.testSession.GetJson<int>("testKey");
        Assert.That(storedValue, Is.EqualTo(42));
    }

    [Test]
    public void SetJson_WithString_StoresJsonString()
    {
        // Act
        this.testSession.SetJson("testKey", "test string");

        // Assert
        var storedValue = this.testSession.GetJson<string>("testKey");
        Assert.That(storedValue, Is.EqualTo("test string"));
    }

    [Test]
    public void GetJson_WithValidJson_ReturnsDeserializedObject()
    {
        // Arrange
        this.testSession.SetJson("testKey", this.testCart);

        // Act
        var result = this.testSession.GetJson<Cart>("testKey");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Lines, Has.Count.EqualTo(1));
            Assert.That(result.Lines[0].Product.Name, Is.EqualTo("Test Product"));
        });
    }

    [Test]
    public void GetJson_WithNullValue_ReturnsNull()
    {
        // Act
        var result = this.testSession.GetJson<Cart>("nonexistentKey");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetJson_WithEmptyString_ReturnsDefault()
    {
        // Arrange
        this.testSession.SetString("testKey", "");

        // Act
        var result = this.testSession.GetJson<Cart>("testKey");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetJson_WithInvalidJson_ThrowsException()
    {
        // Arrange
        this.testSession.SetString("testKey", "invalid json");

        // Act & Assert
        Assert.That(() => this.testSession.GetJson<Cart>("testKey"),
                   Throws.TypeOf<Newtonsoft.Json.JsonReaderException>());
    }

    [Test]
    public void GetJson_WithNullJson_ReturnsDefault()
    {
        // Arrange
        this.testSession.SetString("testKey", "null");

        // Act
        var result = this.testSession.GetJson<Cart>("testKey");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetJson_WithPrimitiveType_ReturnsCorrectValue()
    {
        // Arrange
        this.testSession.SetJson("testKey", 42);

        // Act
        var result = this.testSession.GetJson<int>("testKey");

        // Assert
        Assert.That(result, Is.EqualTo(42));
    }

    [Test]
    public void GetJson_WithString_ReturnsCorrectValue()
    {
        // Arrange
        this.testSession.SetJson("testKey", "test string");

        // Act
        var result = this.testSession.GetJson<string>("testKey");

        // Assert
        Assert.That(result, Is.EqualTo("test string"));
    }

    [Test]
    public void GetJson_WithEmptyObject_ReturnsEmptyObject()
    {
        // Arrange
        var emptyCart = new Cart();
        this.testSession.SetJson("testKey", emptyCart);

        // Act
        var result = this.testSession.GetJson<Cart>("testKey");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Lines, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void SetJson_WithComplexObject_StoresCorrectly()
    {
        // Arrange
        var complexObject = new { Name = "Test", Value = 42, Items = new[] { "item1", "item2" } };

        // Act
        this.testSession.SetJson("testKey", complexObject);

        // Assert
        var result = this.testSession.GetJson<object>("testKey");
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void SetJson_WithEmptyCart_StoresCorrectly()
    {
        // Arrange
        var emptyCart = new Cart();

        // Act
        this.testSession.SetJson("testKey", emptyCart);

        // Assert
        Assert.Multiple(() =>
        {
            var result = this.testSession.GetJson<Cart>("testKey");
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Lines, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void GetJson_WithComplexObject_ReturnsCorrectly()
    {
        // Arrange
        var complexObject = new { Name = "Test", Value = 42, Items = new[] { "item1", "item2" } };
        this.testSession.SetJson("testKey", complexObject);

        // Act
        var result = this.testSession.GetJson<object>("testKey");

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void GetJson_WithEmptyCart_ReturnsCorrectly()
    {
        // Arrange
        var emptyCart = new Cart();
        this.testSession.SetJson("testKey", emptyCart);

        // Act
        var result = this.testSession.GetJson<Cart>("testKey");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Lines, Has.Count.EqualTo(0));
        });
    }
}
