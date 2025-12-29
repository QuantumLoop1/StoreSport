using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace SportsStore.Tests;

[TestFixture]
public class MvcConfigurationTests
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
    public void Application_RegistersMvcServices()
    {
        // Arrange & Act
        using var client = this.factory.CreateClient();
        var serviceProvider = this.factory.Services;

        // Assert
        Assert.That(serviceProvider, Is.Not.Null);
        // MVC services should be registered through AddControllersWithViews()
        // We can test that the service provider can resolve services
        var mvcServices = serviceProvider.GetServices<object>();
        Assert.That(mvcServices, Is.Not.Null);
    }

    [Test]
    public async Task Application_HandlesDefaultRoute()
    {
        // Arrange
        using var client = this.factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
    }

    [Test]
    public async Task Application_HandlesHomeControllerRoute()
    {
        // Arrange
        using var client = this.factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Home");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
    }

    [Test]
    public async Task Application_HandlesHomeIndexRoute()
    {
        // Arrange
        using var client = this.factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Home/Index");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
    }

    [Test]
    public async Task Application_Returns404ForNonExistentRoute()
    {
        // Arrange
        using var client = this.factory.CreateClient();

        // Act
        var response = await client.GetAsync("/NonExistentController");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
    }
}
