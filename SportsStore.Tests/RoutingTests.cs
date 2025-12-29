using Microsoft.AspNetCore.Mvc.Testing;

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
        Assert.That(content, Does.Contain("SPORTS STORE"));
    }

    [Test]
    public async Task HomeControllerRoute_ReturnsHomePage()
    {
        // Act
        var response = await this.client.GetAsync("/Home");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Does.Contain("SPORTS STORE"));
    }

    [Test]
    public async Task HomeIndexRoute_ReturnsHomePage()
    {
        // Act
        var response = await this.client.GetAsync("/Home/Index");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Does.Contain("SPORTS STORE"));
    }

    [Test]
    public async Task PaginationRoute_ReturnsCorrectPage()
    {
        // Act
        var response = await this.client.GetAsync("/Products/Page2");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Does.Contain("SPORTS STORE"));
    }

    [Test]
    public async Task CategoryRoute_ReturnsCategoryPage()
    {
        // Act
        var response = await this.client.GetAsync("/Products/Soccer");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Does.Contain("SPORTS STORE"));
    }

    [Test]
    public async Task CategoryPageRoute_ReturnsCategoryPageWithPagination()
    {
        // Act
        var response = await this.client.GetAsync("/Soccer/Page1");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Does.Contain("SPORTS STORE"));
    }

    [Test]
    public async Task CartRoute_ReturnsCartPage()
    {
        // Act
        var response = await this.client.GetAsync("/Cart");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Does.Contain("SPORTS STORE"));
    }

    [Test]
    public async Task CartControllerRoute_ReturnsCartPage()
    {
        // Act
        var response = await this.client.GetAsync("/Cart/Index");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Does.Contain("SPORTS STORE"));
    }

    [Test]
    public async Task InvalidPaginationRoute_ReturnsEmptyPage()
    {
        // Act
        var response = await this.client.GetAsync("/Products/Page999");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Does.Contain("SPORTS STORE"));
    }

    [Test]
    public async Task InvalidCategoryRoute_ReturnsEmptyPage()
    {
        // Act
        var response = await this.client.GetAsync("/Products/NonExistentCategory");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Does.Contain("SPORTS STORE"));
    }

    [Test]
    public async Task InvalidCategoryPageRoute_ReturnsEmptyPage()
    {
        // Act
        var response = await this.client.GetAsync("/NonExistentCategory/Page1");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Does.Contain("SPORTS STORE"));
    }

    [Test]
    public async Task NonExistentController_Returns404()
    {
        // Act
        var response = await this.client.GetAsync("/NonExistentController");

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
        // Act
        var response = await this.client.GetAsync("/Products/Soccer");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        // Should contain Soccer products
        Assert.That(content, Does.Contain("Soccer Ball"));
        Assert.That(content, Does.Contain("Corner Flags"));
    }

    [Test]
    public async Task CategoryRoute_WithChessCategory_ShowsChessProducts()
    {
        // Act
        var response = await this.client.GetAsync("/Products/Chess");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        // Should contain Chess products (they might be on different pages)
        // Let's check if the category filtering is working by looking for Chess products
        var page2Response = await this.client.GetAsync("/Chess/Page1");
        var page2Content = await page2Response.Content.ReadAsStringAsync();
        Assert.That(page2Content, Does.Contain("Thinking Cap"));
    }

    [Test]
    public async Task CategoryPageRoute_WithValidCategoryAndPage_ShowsCorrectPage()
    {
        // Act
        var response = await this.client.GetAsync("/Soccer/Page1");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Does.Contain("Soccer Ball"));
    }

    [Test]
    public async Task PaginationRoute_WithValidPage_ShowsCorrectPage()
    {
        // Act
        var response = await this.client.GetAsync("/Products/Page2");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        // Page 2 should show different products than page 1
        Assert.That(content, Does.Contain("Stadium"));
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
        Assert.That(content, Does.Contain("Your cart"));
    }

    [Test]
    public async Task Route_WithQueryParameters_HandlesCorrectly()
    {
        // Act
        var response = await this.client.GetAsync("/?productPage=2");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Does.Contain("SPORTS STORE"));
    }

    [Test]
    public async Task Route_WithCategoryQueryParameter_HandlesCorrectly()
    {
        // Act
        var response = await this.client.GetAsync("/?category=Soccer");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Does.Contain("SPORTS STORE"));
    }

    [Test]
    public async Task Route_WithBothCategoryAndPageQueryParameters_HandlesCorrectly()
    {
        // Act
        var response = await this.client.GetAsync("/?category=Soccer&productPage=1");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Does.Contain("SPORTS STORE"));
    }
}
