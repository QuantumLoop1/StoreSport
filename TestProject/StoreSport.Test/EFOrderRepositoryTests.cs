using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using StoreSport.Models;

namespace SportsStore.Tests;

[TestFixture]
public class EfOrderRepositoryTests
{
    private StoreDbContext context = null!;
    private EFOrderRepository repository = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<StoreDbContext>()
            .UseSqlServer($"Server=(localdb)\\mssqllocaldb;Database=TestDb_{Guid.NewGuid()};Trusted_Connection=True;MultipleActiveResultSets=true")
            .Options;

        this.context = new StoreDbContext(options);
        this.context.Database.EnsureCreated();
        this.repository = new EFOrderRepository(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Database.EnsureDeleted();
        this.context.Dispose();
    }

    [Test]
    public void Orders_ReturnsQueryableOrders()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Category = "Test Category",
            Price = 10.00m
        };
        this.context.Products.Add(product);
        this.context.SaveChanges();

        var order = new Order
        {
            Name = "Test User",
            Address = "123 Test St",
            City = "Test City",
            State = "Test State",
            Country = "Test Country"
        };
        order.Lines = new List<CartLine>
        {
            new() { Product = product, Quantity = 2 }
        };
        this.context.Orders.Add(order);
        this.context.SaveChanges();

        // Act
        var result = this.repository.Orders;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
        });
    }

    [Test]
    public void SaveOrder_WithNewOrder_AddsOrderToDatabase()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Category = "Test Category",
            Price = 10.00m
        };
        this.context.Products.Add(product);
        this.context.SaveChanges();

        var order = new Order
        {
            Name = "Test User",
            Address = "123 Test St",
            City = "Test City",
            State = "Test State",
            Country = "Test Country"
        };
        order.Lines = new List<CartLine>
        {
            new() { Product = product, Quantity = 2 }
        };

        // Act
        this.repository.SaveOrder(order);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(this.context.Orders.Count(), Is.EqualTo(1));
            Assert.That(this.context.Orders.First().Name, Is.EqualTo("Test User"));
            Assert.That(this.context.Orders.Include(o => o.Lines).First().Lines, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void SaveOrder_WithExistingOrder_UpdatesOrderInDatabase()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Category = "Test Category",
            Price = 10.00m
        };
        this.context.Products.Add(product);
        this.context.SaveChanges();

        var order = new Order
        {
            Name = "Test User",
            Address = "123 Test St",
            City = "Test City",
            State = "Test State",
            Country = "Test Country"
        };
        this.context.Orders.Add(order);
        this.context.SaveChanges();

        // Act
        order.Name = "Updated User";
        this.repository.SaveOrder(order);

        // Assert
        var updatedOrder = this.context.Orders.First();
        Assert.That(updatedOrder.Name, Is.EqualTo("Updated User"));
    }

    [Test]
    public void SaveOrder_WithOrderLines_AttachesProducts()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Category = "Test Category",
            Price = 10.00m
        };
        this.context.Products.Add(product);
        this.context.SaveChanges();

        var order = new Order
        {
            Name = "Test User",
            Address = "123 Test St",
            City = "Test City",
            State = "Test State",
            Country = "Test Country"
        };
        order.Lines = new List<CartLine>
        {
            new() { Product = product, Quantity = 2 }
        };

        // Act
        this.repository.SaveOrder(order);

        // Assert
        var savedOrder = this.context.Orders.Include(o => o.Lines).ThenInclude(l => l.Product).First();
        Assert.Multiple(() =>
        {
            Assert.That(savedOrder.Lines, Has.Count.EqualTo(1));
            Assert.That(savedOrder.Lines.First().Product.Name, Is.EqualTo("Test Product"));
        });
    }

    [Test]
    public void Orders_IncludesLinesAndProducts()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Category = "Test Category",
            Price = 10.00m
        };
        this.context.Products.Add(product);
        this.context.SaveChanges();

        var order = new Order
        {
            Name = "Test User",
            Address = "123 Test St",
            City = "Test City",
            State = "Test State",
            Country = "Test Country"
        };
        order.Lines = new List<CartLine>
        {
            new() { Product = product, Quantity = 2 }
        };
        this.context.Orders.Add(order);
        this.context.SaveChanges();

        // Act
        var result = this.repository.Orders.First();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Lines, Is.Not.Null);
            Assert.That(result.Lines, Has.Count.EqualTo(1));
            Assert.That(result.Lines.First().Product, Is.Not.Null);
            Assert.That(result.Lines.First().Product.Name, Is.EqualTo("Test Product"));
        });
    }
}