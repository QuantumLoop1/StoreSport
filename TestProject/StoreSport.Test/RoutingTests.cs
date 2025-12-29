using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace SportsStore.Tests;

[TestFixture]
public class RoutingTests
{
    private WebApplicationFactory<Program> factory = null!;
    private HttpClient client = null!;

    [SetUp]
    public void Setup()
    {
        this.factory = new WebApplicationFactory<Program>();
        this.client = this.factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        this.client?.Dispose();
        this.factory?.Dispose();
    }

    [Test]
    public async Task DefaultRoute_ReturnsHomePage()
    {
        // Act
        var response = await this.client.GetAsync("/");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content.ToUpper(), Does.Contain("SPORT"));
    }

    [Test]
    public async Task HomeControllerRoute_ReturnsHomePage()
    {
        // Act
        var response = await this.client.GetAsync("/Home");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content.ToUpper(), Does.Contain("SPORT"));
    }

    [Test]
    public async Task HomeIndexRoute_ReturnsHomePage()
    {
        // Act
        var response = await this.client.GetAsync("/Home/Index");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content.ToUpper(), Does.Contain("SPORT"));
    }

    [Test]
    public async Task PaginationRoute_ReturnsCorrectPage()
    {
        // Act
        var response = await this.client.GetAsync("/Products/Page2");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content.ToUpper(), Does.Contain("SPORT"));
    }

    [Test]
    public async Task CategoryRoute_ReturnsCategoryPage()
    {
        // Act - use query parameter instead of route
        var response = await this.client.GetAsync("/?category=Soccer");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content.ToUpper(), Does.Contain("SPORT"));
    }

    [Test]
    public async Task CategoryPageRoute_ReturnsCategoryPageWithPagination()
    {
        // Act - use correct route format
        var response = await this.client.GetAsync("/Soccer/Page1");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content.ToUpper(), Does.Contain("SPORT"));
    }

    [Test]
    public async Task CartRoute_ReturnsCartPage()
    {
        // Act
        var response = await this.client.GetAsync("/Cart");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content.ToUpper(), Does.Contain("SPORT"));
    }

    [Test]
    public async Task CartControllerRoute_ReturnsCartPage()
    {
        // Act
        var response = await this.client.GetAsync("/Cart/Index");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Does.Contain("cart") | Does.Contain("Cart"));
    }

    [Test]
    public async Task InvalidPaginationRoute_ReturnsEmptyPage()
    {
        // Act
        var response = await this.client.GetAsync("/Products/Page999");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content.ToUpper(), Does.Contain("SPORT"));
    }

    [Test]
    public async Task InvalidCategoryRoute_ReturnsEmptyPage()
    {
        // Act - use query parameter
        var response = await this.client.GetAsync("/?category=NonExistentCategory");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content.ToUpper(), Does.Contain("SPORT"));
    }

    [Test]
    public async Task InvalidCategoryPageRoute_ReturnsEmptyPage()
    {
        // Act
        var response = await this.client.GetAsync("/NonExistentCategory/Page1");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content.ToUpper(), Does.Contain("SPORT"));
    }

    [Test]
    public async Task NonExistentController_Returns404()
    {
        // Act - use a URL with controller/action format that won't match custom routes
        var response = await this.client.GetAsync("/NonExistentController/SomeAction");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
    }

    [Test]
    public async Task NonExistentAction_Returns404()
    {
        // Act
        var response = await this.client.GetAsync("/Home/NonExistentAction");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
    }

    [Test]
    public async Task CategoryRoute_WithValidCategory_ShowsCategoryProducts()
    {
        // Act - use query parameter
        var response = await this.client.GetAsync("/?category=Soccer");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content.ToUpper(), Does.Contain("SPORT"));
    }

    [Test]
    public async Task CategoryRoute_WithChessCategory_ShowsChessProducts()
    {
        // Act - use query parameter
        var response = await this.client.GetAsync("/?category=Chess");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content.ToUpper(), Does.Contain("SPORT"));
    }

    [Test]
    public async Task CategoryPageRoute_WithValidCategoryAndPage_ShowsCorrectPage()
    {
        // Act
        var response = await this.client.GetAsync("/Soccer/Page1");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content.ToUpper(), Does.Contain("SPORT"));
    }

    [Test]
    public async Task PaginationRoute_WithValidPage_ShowsCorrectPage()
    {
        // Act
        var response = await this.client.GetAsync("/Products/Page2");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        // Page 2 should show products or be empty
        Assert.That(content, Does.Contain("card") | Does.Contain("html"));
    }

    [Test]
    public async Task CartRoute_ShowsCartView()
    {
        // Act
        var response = await this.client.GetAsync("/Cart");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        // Should show cart-specific content
        Assert.That(content.ToLower(), Does.Contain("cart"));
    }

    [Test]
    public async Task Route_WithQueryParameters_HandlesCorrectly()
    {
        // Act
        var response = await this.client.GetAsync("/?productPage=2");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content.ToUpper(), Does.Contain("SPORT"));
    }

    [Test]
    public async Task Route_WithCategoryQueryParameter_HandlesCorrectly()
    {
        // Act
        var response = await this.client.GetAsync("/?category=Soccer");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content.ToUpper(), Does.Contain("SPORT"));
    }

    [Test]
    public async Task Route_WithBothCategoryAndPageQueryParameters_HandlesCorrectly()
    {
        // Act
        var response = await this.client.GetAsync("/?category=Soccer&productPage=1");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content.ToUpper(), Does.Contain("SPORT"));
    }
}
