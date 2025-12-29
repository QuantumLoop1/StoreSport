using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using StoreSport.Models;

namespace SportsStore.Tests;

[TestFixture]
public class RepositoryTests
{
    private ServiceProvider serviceProvider = null!;
    private StoreDbContext context = null!;
    private IStoreRepository repository = null!;

    [SetUp]
 public void Setup()
    {
        var services = new ServiceCollection();
        services.AddDbContext<StoreDbContext>(options =>
    options.UseSqlServer($"Server=(localdb)\\mssqllocaldb;Database=TestDb_{Guid.NewGuid()};Trusted_Connection=True;MultipleActiveResultSets=true"));
        services.AddScoped<IStoreRepository, EFStoreRepository>();

        this.serviceProvider = services.BuildServiceProvider();
    this.context = this.serviceProvider.GetRequiredService<StoreDbContext>();
        this.repository = this.serviceProvider.GetRequiredService<IStoreRepository>();

        // Ensure database is created
        this.context.Database.EnsureCreated();

        // Seed test data
        this.SeedTestData();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Database.EnsureDeleted();
        this.context.Dispose();
        this.serviceProvider.Dispose();
    }

    private void SeedTestData()
    {
        var products = new List<Product>
        {
      new() { Name = "Test Product 1", Description = "Description 1", Price = 10.99m, Category = "Test" },
            new() { Name = "Test Product 2", Description = "Description 2", Price = 20.99m, Category = "Test" },
            new() { Name = "Test Product 3", Description = "Description 3", Price = 30.99m, Category = "Another" }
     };

        this.context.Products.AddRange(products);
      this.context.SaveChanges();
    }

    [Test]
    public void EFStoreRepository_Products_ReturnsAllProducts()
    {
 // Act
   var products = this.repository.Products.ToList();

        // Assert
        Assert.Multiple(() =>
        {
          Assert.That(products, Has.Count.EqualTo(3));
            Assert.That(products.Select(p => p.Name), Contains.Item("Test Product 1"));
            Assert.That(products.Select(p => p.Name), Contains.Item("Test Product 2"));
    Assert.That(products.Select(p => p.Name), Contains.Item("Test Product 3"));
        });
    }

    [Test]
    public void EFStoreRepository_Products_SupportsLinqQueries()
    {
        // Act
        var expensiveProducts = this.repository.Products
     .Where(p => p.Price > 20)
          .ToList();

   // Assert
        Assert.Multiple(() =>
        {
    Assert.That(expensiveProducts, Has.Count.EqualTo(2));
   Assert.That(expensiveProducts.Select(p => p.Name), Contains.Item("Test Product 2"));
 Assert.That(expensiveProducts.Select(p => p.Name), Contains.Item("Test Product 3"));
     });
    }

    [Test]
    public void EFStoreRepository_Products_SupportsOrdering()
    {
        // Act
        var orderedProducts = this.repository.Products
            .OrderBy(p => p.Price)
 .ToList();

  // Assert
     Assert.Multiple(() =>
        {
    Assert.That(orderedProducts[0].Name, Is.EqualTo("Test Product 1"));
    Assert.That(orderedProducts[1].Name, Is.EqualTo("Test Product 2"));
            Assert.That(orderedProducts[2].Name, Is.EqualTo("Test Product 3"));
        });
    }

    [Test]
    public void EFStoreRepository_Products_SupportsFilteringByCategory()
  {
      // Act
        var testCategoryProducts = this.repository.Products
 .Where(p => p.Category == "Test")
       .ToList();

        // Assert
        Assert.Multiple(() =>
      {
     Assert.That(testCategoryProducts, Has.Count.EqualTo(2));
            Assert.That(testCategoryProducts.Select(p => p.Name), Contains.Item("Test Product 1"));
  Assert.That(testCategoryProducts.Select(p => p.Name), Contains.Item("Test Product 2"));
   });
    }

    [Test]
    public void EFStoreRepository_Products_SupportsPagination()
    {
        // Act
var firstPage = this.repository.Products
.OrderBy(p => p.ProductId)
            .Take(2)
            .ToList();

        var secondPage = this.repository.Products
  .OrderBy(p => p.ProductId)
   .Skip(2)
     .Take(2)
            .ToList();

        // Assert
        Assert.Multiple(() =>
        {
    Assert.That(firstPage, Has.Count.EqualTo(2));
 Assert.That(secondPage, Has.Count.EqualTo(1));
            Assert.That(firstPage.Select(p => p.Name), Contains.Item("Test Product 1"));
     Assert.That(firstPage.Select(p => p.Name), Contains.Item("Test Product 2"));
            Assert.That(secondPage.Select(p => p.Name), Contains.Item("Test Product 3"));
        });
    }

    [Test]
    public void EFStoreRepository_Products_ReturnsIQueryable()
    {
        // Act
        var products = this.repository.Products;

        // Assert
        Assert.That(products, Is.InstanceOf<IQueryable<Product>>());
    }
}
