using NUnit.Framework;
using StoreSport.Models;
using StoreSport.Models.ViewModels;

namespace SportsStore.Tests;

[TestFixture]
public class ViewModelsTests
{
    [Test]
    public void PagingInfo_DefaultValues_AreSetCorrectly()
    {
        // Act
        var pagingInfo = new PagingInfo();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(pagingInfo.TotalItems, Is.EqualTo(0));
            Assert.That(pagingInfo.ItemsPerPage, Is.EqualTo(0));
            Assert.That(pagingInfo.CurrentPage, Is.EqualTo(0));
        });
    }

    [Test]
    public void PagingInfo_TotalPages_CalculatesCorrectly()
    {
        // Arrange
        var pagingInfo = new PagingInfo
        {
            TotalItems = 10,
            ItemsPerPage = 4,
            CurrentPage = 1
        };

        // Act & Assert
        Assert.That(pagingInfo.TotalPages, Is.EqualTo(3)); // 10 items / 4 per page = 3 pages
    }

    [Test]
    public void PagingInfo_TotalPages_HandlesExactDivision()
    {
        // Arrange
        var pagingInfo = new PagingInfo
        {
            TotalItems = 8,
            ItemsPerPage = 4,
            CurrentPage = 1
        };

        // Act & Assert
        Assert.That(pagingInfo.TotalPages, Is.EqualTo(2)); // 8 items / 4 per page = 2 pages
    }

    [Test]
    public void PagingInfo_TotalPages_HandlesRemainder()
    {
        // Arrange
        var pagingInfo = new PagingInfo
        {
            TotalItems = 9,
            ItemsPerPage = 4,
            CurrentPage = 1
        };

        // Act & Assert
        Assert.That(pagingInfo.TotalPages, Is.EqualTo(3)); // 9 items / 4 per page = 2.25 -> 3 pages
    }

    [Test]
    public void PagingInfo_TotalPages_HandlesZeroItems()
    {
        // Arrange
        var pagingInfo = new PagingInfo
        {
            TotalItems = 0,
            ItemsPerPage = 4,
            CurrentPage = 1
        };

        // Act & Assert
        Assert.That(pagingInfo.TotalPages, Is.EqualTo(0)); // 0 items / 4 per page = 0 pages
    }

    [Test]
    public void PagingInfo_TotalPages_HandlesZeroItemsPerPage()
    {
        // Arrange
        var pagingInfo = new PagingInfo
        {
            TotalItems = 10,
            ItemsPerPage = 0,
            CurrentPage = 1
        };

        // Act & Assert
        Assert.That(pagingInfo.TotalPages, Is.EqualTo(0)); // Division by zero should return 0
    }

    [Test]
    public void PagingInfo_TotalPages_HandlesLargeNumbers()
    {
        // Arrange
        var pagingInfo = new PagingInfo
        {
            TotalItems = 1000000,
            ItemsPerPage = 100,
            CurrentPage = 1
        };

        // Act & Assert
        Assert.That(pagingInfo.TotalPages, Is.EqualTo(10000)); // 1,000,000 items / 100 per page = 10,000 pages
    }

    [Test]
    public void PagingInfo_CanSetAllProperties()
    {
        // Arrange
        var pagingInfo = new PagingInfo
        {
            TotalItems = 25,
            ItemsPerPage = 5,
            CurrentPage = 3
        };

        // Act & Assert
        Assert.Multiple(() =>
        {
            Assert.That(pagingInfo.TotalItems, Is.EqualTo(25));
            Assert.That(pagingInfo.ItemsPerPage, Is.EqualTo(5));
            Assert.That(pagingInfo.CurrentPage, Is.EqualTo(3));
            Assert.That(pagingInfo.TotalPages, Is.EqualTo(5)); // 25 items / 5 per page = 5 pages
        });
    }

    [Test]
    public void ProductsListViewModel_DefaultValues_AreSetCorrectly()
    {
        // Act
        var viewModel = new ProductsListViewModel();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel.Products, Is.Not.Null);
            Assert.That(viewModel.Products, Is.Empty);
            Assert.That(viewModel.PagingInfo, Is.Not.Null);
            Assert.That(viewModel.PagingInfo.TotalItems, Is.EqualTo(0));
        });
    }

    [Test]
    public void ProductsListViewModel_CanSetProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { ProductId = 1, Name = "Product 1", Description = "Desc 1", Price = 10.00m, Category = "Cat 1" },
            new() { ProductId = 2, Name = "Product 2", Description = "Desc 2", Price = 20.00m, Category = "Cat 2" }
        };

        // Act
        var viewModel = new ProductsListViewModel
        {
            Products = products
        };

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel.Products.Count(), Is.EqualTo(2));
            Assert.That(viewModel.Products.First().Name, Is.EqualTo("Product 1"));
            Assert.That(viewModel.Products.Last().Name, Is.EqualTo("Product 2"));
        });
    }

    [Test]
    public void ProductsListViewModel_CanSetPagingInfo()
    {
        // Arrange
        var pagingInfo = new PagingInfo
        {
            TotalItems = 10,
            ItemsPerPage = 4,
            CurrentPage = 2
        };

        // Act
        var viewModel = new ProductsListViewModel
        {
            PagingInfo = pagingInfo
        };

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel.PagingInfo, Is.EqualTo(pagingInfo));
            Assert.That(viewModel.PagingInfo.TotalItems, Is.EqualTo(10));
            Assert.That(viewModel.PagingInfo.ItemsPerPage, Is.EqualTo(4));
            Assert.That(viewModel.PagingInfo.CurrentPage, Is.EqualTo(2));
            Assert.That(viewModel.PagingInfo.TotalPages, Is.EqualTo(3));
        });
    }

    [Test]
    public void ProductsListViewModel_CanSetBothProductsAndPagingInfo()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { ProductId = 1, Name = "Product 1", Description = "Desc 1", Price = 10.00m, Category = "Cat 1" },
            new() { ProductId = 2, Name = "Product 2", Description = "Desc 2", Price = 20.00m, Category = "Cat 2" }
        };

        var pagingInfo = new PagingInfo
        {
            TotalItems = 10,
            ItemsPerPage = 4,
            CurrentPage = 1
        };

        // Act
        var viewModel = new ProductsListViewModel
        {
            Products = products,
            PagingInfo = pagingInfo
        };

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel.Products.Count(), Is.EqualTo(2));
            Assert.That(viewModel.PagingInfo.TotalItems, Is.EqualTo(10));
            Assert.That(viewModel.PagingInfo.TotalPages, Is.EqualTo(3));
        });
    }

    [Test]
    public void ProductsListViewModel_ProductsCanBeEmpty()
    {
        // Act
        var viewModel = new ProductsListViewModel
        {
            Products = new List<Product>()
        };

        // Assert
        Assert.That(viewModel.Products, Is.Empty);
    }

    [Test]
    public void ProductsListViewModel_ProductsCanBeNull()
    {
        // Act
        var viewModel = new ProductsListViewModel
        {
            Products = null!
        };

        // Assert
        Assert.That(viewModel.Products, Is.Null);
    }

    [Test]
    public void ProductsListViewModel_PagingInfoCanBeNull()
    {
        // Act
        var viewModel = new ProductsListViewModel
        {
            PagingInfo = null!
        };

        // Assert
        Assert.That(viewModel.PagingInfo, Is.Null);
    }

    [Test]
    public void PagingInfo_TotalPages_IsReadOnlyProperty()
    {
        // Arrange
        var pagingInfo = new PagingInfo
        {
            TotalItems = 10,
            ItemsPerPage = 4
        };

        // Act
        var totalPages = pagingInfo.TotalPages;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(totalPages, Is.EqualTo(3));
            
            // Verify it's calculated property (can't be set directly)
            var propertyInfo = typeof(PagingInfo).GetProperty("TotalPages");
            Assert.That(propertyInfo?.CanWrite, Is.False);
        });
    }
}
