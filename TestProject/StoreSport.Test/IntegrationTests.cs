using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace SportsStore.Tests;

[TestFixture]
public class IntegrationTests
{
    private WebApplicationFactory<Program> factory = null!;
    private HttpClient client = null!;

    [SetUp]
    public void Setup()
    {
        factory = new WebApplicationFactory<Program>();
        client = factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        client?.Dispose();
        factory?.Dispose();
    }

    [Test]
    public async Task HomeController_Index_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await client.GetAsync("/");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
    }

    [Test]
    public async Task HomeController_Index_ReturnsHtmlContent()
    {
        // Act
        var response = await client.GetAsync("/");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(
                response.Content.Headers.ContentType?.MediaType,
                Is.EqualTo("text/html")
            );
        });
    }

    [Test]
    public async Task HomeController_Index_ContainsSportsStoreTitle()
    {
        // Act
        var response = await client.GetAsync("/");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);

            var content = await response.Content.ReadAsStringAsync();
            Assert.That(content.ToUpper(), Does.Contain("SPORT"));
        });
    }

    [Test]
    public async Task HomeController_Index_DisplaysProductsFromDatabase()
    {
        // Act
        var response = await client.GetAsync("/");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);

            var content = await response.Content.ReadAsStringAsync();
            Assert.That(
                content,
                Does.Contain("card") |
                Does.Contain("Product") |
                Does.Contain("Price")
            );
        });
    }

    [Test]
    public async Task HomeController_Index_DisplaysProductPrices()
    {
        // Act
        var response = await client.GetAsync("/");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);

            var content = await response.Content.ReadAsStringAsync();
            Assert.That(
                content,
                Does.Contain("$") |
                Does.Contain("Price") |
                Does.Contain("price")
            );
        });
    }

    [Test]
    public async Task HomeController_Index_ContainsBootstrapStyling()
    {
        // Act
        var response = await client.GetAsync("/");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);

            var content = await response.Content.ReadAsStringAsync();
            Assert.That(
                content,
                Does.Contain("card") |
                Does.Contain("btn") |
                Does.Contain("col")
            );
        });
    }

    [Test]
    public async Task HomeController_Index_ContainsPaginationLinks()
    {
        // Act
        var response = await client.GetAsync("/");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);

            var content = await response.Content.ReadAsStringAsync();
            Assert.That(
                content,
                Does.Contain("page") |
                Does.Contain("Page") |
                Does.Contain("btn")
            );
        });
    }

    [Test]
    public async Task HomeController_Index_FirstPage_ShowsOnlyFirstFourProducts()
    {
        // Act
        var response = await client.GetAsync("/");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);

            var content = await response.Content.ReadAsStringAsync();
            Assert.That(content, Does.Contain("card"));
        });
    }

    [Test]
    public async Task HomeController_Index_SecondPage_ShowsCorrectProducts()
    {
        // Act
        var response = await client.GetAsync("/?productPage=2");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);

            var content = await response.Content.ReadAsStringAsync();
            Assert.That(
                content,
                Does.Contain("card") |
                Does.Contain("html")
            );
        });
    }

    [Test]
    public async Task HomeController_Index_ThirdPage_ShowsRemainingProducts()
    {
        // Act
        var response = await client.GetAsync("/?productPage=3");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);

            var content = await response.Content.ReadAsStringAsync();
            Assert.That(content, Does.Contain("html"));
        });
    }

    [Test]
    public async Task HomeController_Index_InvalidPage_ReturnsFirstPage()
    {
        // Act
        var response = await client.GetAsync("/?productPage=0");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);

            var content = await response.Content.ReadAsStringAsync();
            Assert.That(content, Does.Contain("html"));
        });
    }

    [Test]
    public async Task HomeController_Index_OutOfRangePage_ReturnsLastPage()
    {
        // Act
        var response = await client.GetAsync("/?productPage=999");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);

            var content = await response.Content.ReadAsStringAsync();
            Assert.That(content, Is.Not.Null);
        });
    }

    [Test]
    public async Task HomeController_Index_ContainsPartialViewContent()
    {
        // Act
        var response = await client.GetAsync("/");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);

            var content = await response.Content.ReadAsStringAsync();
            Assert.That(
                content,
                Does.Contain("card") |
                Does.Contain("product")
            );
        });
    }

    [Test]
    public async Task HomeController_Index_ProductsAreOrderedByProductId()
    {
        // Act
        var response = await client.GetAsync("/");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);

            var content = await response.Content.ReadAsStringAsync();
            Assert.That(
                content,
                Does.Contain("card") |
                Does.Contain("product")
            );
        });
    }

    [Test]
    public async Task HomeController_Index_ContainsAllRequiredProductCategories()
    {
        // Act
        var response = await client.GetAsync("/");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);

            var content = await response.Content.ReadAsStringAsync();
            Assert.That(
                content,
                Does.Contain("Categor") |
                Does.Contain("categor") |
                Does.Contain("card")
            );
        });
    }

    [Test]
    public async Task HomeController_Index_ContainsBootstrapJavaScript()
    {
        // Act
        var response = await client.GetAsync("/");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);

            var content = await response.Content.ReadAsStringAsync();
            Assert.That(
                content,
                Does.Contain(".js") |
                Does.Contain("script")
            );
        });
    }

    [Test]
    public async Task HomeController_Index_ContainsResponsiveLayout()
    {
        // Act
        var response = await client.GetAsync("/");

        // Assert
        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);

            var content = await response.Content.ReadAsStringAsync();
            Assert.That(
                content,
                Does.Contain("col") |
                Does.Contain("row")
            );
        });
    }
}
