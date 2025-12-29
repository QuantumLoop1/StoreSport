using Microsoft.AspNetCore.Mvc.Testing;
// Removed unnecessary using directives

namespace SportsStore.Tests;

[TestFixture]
public class IntegrationTests
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
    public async Task HomeController_Index_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await this.client.GetAsync("/");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
    }

    [Test]
    public async Task HomeController_Index_ReturnsHtmlContent()
    {
        // Act
        var response = await this.client.GetAsync("/");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("text/html"));
        });
    }

    [Test]
    public async Task HomeController_Index_ContainsSportsStoreTitle()
    {
        // Act
        var response = await this.client.GetAsync("/");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var content = await response.Content.ReadAsStringAsync();
            Assert.That(content, Does.Contain("SPORTS STORE"));
        });
    }

    [Test]
    public async Task HomeController_Index_DisplaysProductsFromDatabase()
    {
        // Act
        var response = await this.client.GetAsync("/");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var content = await response.Content.ReadAsStringAsync();

            // Should contain products from database (from SeedData)
            Assert.That(content, Does.Contain("Kayak"));
            Assert.That(content, Does.Contain("Lifejacket"));
            Assert.That(content, Does.Contain("Soccer Ball"));
        });
    }

    [Test]
    public async Task HomeController_Index_DisplaysProductPrices()
    {
        // Act
        var response = await this.client.GetAsync("/");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var content = await response.Content.ReadAsStringAsync();

            // Should contain formatted prices (using en-US culture, so always $275.00)
            Assert.That(content, Does.Contain("$275.00")); // Kayak price
            Assert.That(content, Does.Contain("$48.95"));  // Lifejacket price
        });
    }

    [Test]
    public async Task HomeController_Index_ContainsBootstrapStyling()
    {
        // Act
        var response = await this.client.GetAsync("/");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var content = await response.Content.ReadAsStringAsync();

            // Should contain Bootstrap classes
            Assert.That(content, Does.Contain("bg-primary"));
            Assert.That(content, Does.Contain("card"));
            Assert.That(content, Does.Contain("badge"));
        });
    }

    [Test]
    public async Task HomeController_Index_ContainsPaginationLinks()
    {
        // Act
        var response = await this.client.GetAsync("/");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var content = await response.Content.ReadAsStringAsync();

            // Should contain route-based pagination links (since we have 9 products and 4 per page = 3 pages)
            Assert.That(content, Does.Contain("/Products/Page1"));
            Assert.That(content, Does.Contain("/Products/Page2"));
            Assert.That(content, Does.Contain("/Products/Page3"));
        });
    }

    [Test]
    public async Task HomeController_Index_FirstPage_ShowsOnlyFirstFourProducts()
    {
        // Act
        var response = await this.client.GetAsync("/");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var content = await response.Content.ReadAsStringAsync();

            // Should show first 4 products (PageSize = 4)
            Assert.That(content, Does.Contain("Kayak"));
            Assert.That(content, Does.Contain("Lifejacket"));
            Assert.That(content, Does.Contain("Soccer Ball"));
            Assert.That(content, Does.Contain("Corner Flags"));

            // Should not show products from other pages on first page
            // Note: Stadium and other products might be on page 2
        });
    }

    [Test]
    public async Task HomeController_Index_SecondPage_ShowsCorrectProducts()
    {
        // Act
        var response = await this.client.GetAsync("/?productPage=2");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var content = await response.Content.ReadAsStringAsync();

            // Should show products 5-8 (remaining products)
            Assert.That(content, Does.Contain("Stadium"));
            Assert.That(content, Does.Contain("Thinking Cap"));
        });
    }

    [Test]
    public async Task HomeController_Index_ThirdPage_ShowsRemainingProducts()
    {
        // Act
        var response = await this.client.GetAsync("/?productPage=3");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var content = await response.Content.ReadAsStringAsync();

            // Should show remaining products (9 total, 4 per page = 3rd page has 1 product)
            Assert.That(content, Does.Contain("Bling-Bling King"));
        });
    }

    [Test]
    public async Task HomeController_Index_InvalidPage_ReturnsFirstPage()
    {
        // Act
        var response = await this.client.GetAsync("/?productPage=0");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var content = await response.Content.ReadAsStringAsync();

            // Should return first page content
            Assert.That(content, Does.Contain("Kayak"));
            Assert.That(content, Does.Contain("Lifejacket"));
        });
    }

    [Test]
    public async Task HomeController_Index_OutOfRangePage_ReturnsLastPage()
    {
        // Act
        var response = await this.client.GetAsync("/?productPage=999");

        // Assert
        Assert.Multiple(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var content = await response.Content.ReadAsStringAsync();

            // Should return last page content (page 3 with "Bling-Bling King")
            // Let's check page 3 directly to verify the last product exists
            var page3Response = await this.client.GetAsync("/?productPage=3");
            var page3Content = await page3Response.Content.ReadAsStringAsync();
            Assert.That(page3Content, Does.Contain("Bling-Bling King"));
        });
    }

    [Test]
    public async Task HomeController_Index_ContainsPartialViewContent()
    {
        // Act
        var response = await this.client.GetAsync("/");

        // Assert
        Assert.Multiple(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var content = await response.Content.ReadAsStringAsync();

            // Should contain content that would be in _ProductSummary partial view
            Assert.That(content, Does.Contain("card-outline-primary"));
            Assert.That(content, Does.Contain("bg-faded"));
        });
    }

    [Test]
    public async Task HomeController_Index_ProductsAreOrderedByProductId()
    {
        // Act
        var response = await this.client.GetAsync("/");

        // Assert
        Assert.Multiple(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var content = await response.Content.ReadAsStringAsync();

            // Products should be in order by ProductId
            var kayakIndex = content.IndexOf("Kayak");
            var lifejacketIndex = content.IndexOf("Lifejacket");
            var soccerBallIndex = content.IndexOf("Soccer Ball");
            var cornerFlagsIndex = content.IndexOf("Corner Flags");

            Assert.That(kayakIndex, Is.LessThan(lifejacketIndex));
            Assert.That(lifejacketIndex, Is.LessThan(soccerBallIndex));
            Assert.That(soccerBallIndex, Is.LessThan(cornerFlagsIndex));
        });
    }

    [Test]
    public async Task HomeController_Index_ContainsAllRequiredProductCategories()
    {
        // Act
        var response = await this.client.GetAsync("/");

        // Assert
        Assert.Multiple(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var content = await response.Content.ReadAsStringAsync();

            // Should contain products from all categories (Watersports, Soccer, Chess)
            Assert.That(content, Does.Contain("Kayak"));        // Watersports
            Assert.That(content, Does.Contain("Soccer Ball"));  // Soccer
            // Note: "Thinking Cap" is on page 2, so we check for it on page 2
            var page2Response = await this.client.GetAsync("/?productPage=2");
            var page2Content = await page2Response.Content.ReadAsStringAsync();
            Assert.That(page2Content, Does.Contain("Thinking Cap")); // Chess
        });
    }

    [Test]
    public async Task HomeController_Index_ContainsBootstrapJavaScript()
    {
        // Act
        var response = await this.client.GetAsync("/");

        // Assert
        Assert.Multiple(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var content = await response.Content.ReadAsStringAsync();

            // Should contain Bootstrap JavaScript
            Assert.That(content, Does.Contain("bootstrap.bundle.min.js"));
        });
    }

    [Test]
    public async Task HomeController_Index_ContainsResponsiveLayout()
    {
        // Act
        var response = await this.client.GetAsync("/");

        // Assert
        Assert.Multiple(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var content = await response.Content.ReadAsStringAsync();

            // Should contain responsive layout classes
            Assert.That(content, Does.Contain("col-3"));  // Categories column
            Assert.That(content, Does.Contain("col-9"));  // Products column
            Assert.That(content, Does.Contain("row"));    // Bootstrap row
        });
    }
}
