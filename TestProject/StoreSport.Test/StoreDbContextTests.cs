using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using StoreSport.Models;

namespace SportsStore.Tests;

[TestFixture]
public class StoreDbContextTests
{
    private ServiceProvider serviceProvider = null!;
    private StoreDbContext context = null!;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddDbContext<StoreDbContext>(options =>
     options.UseSqlServer($"Server=(localdb)\\mssqllocaldb;Database=TestDb_{Guid.NewGuid()};Trusted_Connection=True;MultipleActiveResultSets=true"));

        this.serviceProvider = services.BuildServiceProvider();
        this.context = this.serviceProvider.GetRequiredService<StoreDbContext>();
        this.context.Database.EnsureCreated();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Database.EnsureDeleted();
        this.context.Dispose();
        this.serviceProvider.Dispose();
    }

    [Test]
    public void StoreDbContext_CanBeCreated()
    {
        // Act & Assert
        Assert.Multiple(() =>
        {
            Assert.That(this.context, Is.Not.Null);
            Assert.That(this.context.Products, Is.Not.Null);
        });
    }

    [Test]
    public void StoreDbContext_Products_CanAddProduct()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Category = "Test Category"
        };

        // Act
        this.context.Products.Add(product);
        this.context.SaveChanges();

        // Assert
        Assert.Multiple(() =>
        {
            var savedProduct = this.context.Products.FirstOrDefault(p => p.Name == "Test Product");
            Assert.That(savedProduct, Is.Not.Null);
            Assert.That(savedProduct!.Name, Is.EqualTo("Test Product"));
            Assert.That(savedProduct.Description, Is.EqualTo("Test Description"));
            Assert.That(savedProduct.Price, Is.EqualTo(99.99m));
            Assert.That(savedProduct.Category, Is.EqualTo("Test Category"));
        });
    }

    [Test]
    public void StoreDbContext_Products_CanUpdateProduct()
    {
        // Arrange
        var product = new Product
        {
            Name = "Original Name",
            Description = "Original Description",
            Price = 50.00m,
            Category = "Original Category"
        };

        this.context.Products.Add(product);
        this.context.SaveChanges();

        // Act
        product.Name = "Updated Name";
        product.Price = 75.00m;
        this.context.SaveChanges();

        // Assert
        Assert.Multiple(() =>
        {
            var updatedProduct = this.context.Products.FirstOrDefault(p => p.ProductId == product.ProductId);
            Assert.That(updatedProduct, Is.Not.Null);
            Assert.That(updatedProduct!.Name, Is.EqualTo("Updated Name"));
            Assert.That(updatedProduct.Price, Is.EqualTo(75.00m));
        });
    }

    [Test]
    public void StoreDbContext_Products_CanDeleteProduct()
    {
        // Arrange
        var product = new Product
        {
            Name = "To Delete",
            Description = "Will be deleted",
            Price = 25.00m,
            Category = "Delete Category"
        };

        this.context.Products.Add(product);
        this.context.SaveChanges();

        var productId = product.ProductId;

        // Act
        this.context.Products.Remove(product);
        this.context.SaveChanges();

        // Assert
        var deletedProduct = this.context.Products.FirstOrDefault(p => p.ProductId == productId);
        Assert.That(deletedProduct, Is.Null);
    }

    [Test]
    public void StoreDbContext_Products_CanQueryProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Name = "Product 1", Description = "Desc 1", Price = 10.00m, Category = "Category A" },
    new() { Name = "Product 2", Description = "Desc 2", Price = 20.00m, Category = "Category B" },
  new() { Name = "Product 3", Description = "Desc 3", Price = 30.00m, Category = "Category A" }
        };

        this.context.Products.AddRange(products);
        this.context.SaveChanges();

        // Act
        var categoryAProducts = this.context.Products
              .Where(p => p.Category == "Category A")
         .ToList();

        var expensiveProducts = this.context.Products
            .Where(p => p.Price > 15.00m)
          .ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(categoryAProducts, Has.Count.EqualTo(2));
            Assert.That(expensiveProducts, Has.Count.EqualTo(2));
        });
    }

    [Test]
    public void StoreDbContext_Products_SupportsOrdering()
    {
        // Arrange
        var products = new List<Product>
  {
            new() { Name = "Product C", Description = "Desc C", Price = 30.00m, Category = "Test" },
          new() { Name = "Product A", Description = "Desc A", Price = 10.00m, Category = "Test" },
       new() { Name = "Product B", Description = "Desc B", Price = 20.00m, Category = "Test" }
        };

        this.context.Products.AddRange(products);
        this.context.SaveChanges();

        // Act
        var orderedByName = this.context.Products
               .OrderBy(p => p.Name)
               .ToList();

        var orderedByPrice = this.context.Products
.OrderByDescending(p => p.Price)
            .ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(orderedByName[0].Name, Is.EqualTo("Product A"));
            Assert.That(orderedByName[1].Name, Is.EqualTo("Product B"));
            Assert.That(orderedByName[2].Name, Is.EqualTo("Product C"));

            Assert.That(orderedByPrice[0].Price, Is.EqualTo(30.00m));
            Assert.That(orderedByPrice[1].Price, Is.EqualTo(20.00m));
            Assert.That(orderedByPrice[2].Price, Is.EqualTo(10.00m));
        });
    }

    [Test]
    public void StoreDbContext_Products_SupportsPagination()
    {
        // Arrange
        var products = new List<Product>();
        for (int i = 1; i <= 10; i++)
        {
            products.Add(new Product
            {
                Name = $"Product {i}",
                Description = $"Description {i}",
                Price = i * 10.00m,
                Category = "Test"
            });
        }

        this.context.Products.AddRange(products);
        this.context.SaveChanges();

        // Act
        var firstPage = this.context.Products
   .OrderBy(p => p.ProductId)
      .Take(3)
   .ToList();

        var secondPage = this.context.Products
         .OrderBy(p => p.ProductId)
  .Skip(3)
            .Take(3)
      .ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(firstPage, Has.Count.EqualTo(3));
            Assert.That(secondPage, Has.Count.EqualTo(3));
            Assert.That(firstPage[0].Name, Is.EqualTo("Product 1"));
            Assert.That(secondPage[0].Name, Is.EqualTo("Product 4"));
        });
    }

    [Test]
    public void StoreDbContext_Products_ProductIdIsAutoGenerated()
    {
        // Arrange
        var product = new Product
        {
            Name = "Auto ID Test",
            Description = "Testing auto ID",
            Price = 15.00m,
            Category = "Test"
        };

        // Act
        this.context.Products.Add(product);
        this.context.SaveChanges();

        // Assert
        Assert.That(product.ProductId, Is.GreaterThan(0));
    }

    [Test]
    public void StoreDbContext_Products_CanHandleDecimalPrecision()
    {
        // Arrange
        var product = new Product
        {
            Name = "Precision Test",
            Description = "Testing decimal precision",
            Price = 123.45m,
            Category = "Test"
        };

        // Act
        this.context.Products.Add(product);
        this.context.SaveChanges();

        // Assert
        Assert.Multiple(() =>
        {
            var savedProduct = this.context.Products.FirstOrDefault(p => p.Name == "Precision Test");
            Assert.That(savedProduct, Is.Not.Null);
            Assert.That(savedProduct!.Price, Is.EqualTo(123.45m));
        });
    }
}
