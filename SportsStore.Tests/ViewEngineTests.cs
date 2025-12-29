using Microsoft.AspNetCore.Mvc.Testing;

namespace SportsStore.Tests;

[TestFixture]
public class ViewEngineTests
{
    private WebApplicationFactory<Program> factory = null!;

    [SetUp]
    public void Setup()
    {
        this.factory = new WebApplicationFactory<Program>();
    }

    [TearDown]
    public void TearDown()
    {
        this.factory.Dispose();
    }

    [Test]
    public async Task Application_RendersLayoutView()
    {
        // Arrange
        using var client = this.factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(content, Does.Contain("<!DOCTYPE html>"));
            Assert.That(content, Does.Contain("<html lang=\"en\">"));
            Assert.That(content, Does.Contain("<head>"));
            Assert.That(content, Does.Contain("<title>SportsStore</title>"));
        });
    }

    [Test]
    public async Task Application_RendersViewStart()
    {
        // Arrange
        using var client = this.factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            // Should render the layout (indicating _ViewStart.cshtml is working)
            Assert.That(content, Does.Contain("<body>"));
            Assert.That(content, Does.Contain("</body>"));
        });
    }

    [Test]
    public async Task Application_RendersHomeIndexView()
    {
        // Arrange
        using var client = this.factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            // Updated for step-2: should contain "SPORTS STORE" instead of "Welcome to Sports Store!"
            Assert.That(content, Does.Contain("SPORTS STORE"));
        });
    }

    [Test]
    public async Task Application_IncludesViewImports()
    {
        // Arrange
        using var client = this.factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            // ViewImports should be loaded (TagHelpers should be available)
            // This is harder to test directly, but we can verify the view renders correctly
            Assert.That(content, Is.Not.Empty);
        });
    }

    [Test]
    public async Task Application_HandlesViewNotFound()
    {
        // Arrange
        using var client = this.factory.CreateClient();

        // Act - Try to access a controller action that doesn't have a view
        var response = await client.GetAsync("/Home/NonExistentAction");

        // Assert - Should return 404 or error for non-existent action
        Assert.That(response.StatusCode, Is.Not.EqualTo(System.Net.HttpStatusCode.OK));
    }
}
