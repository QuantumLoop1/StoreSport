using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SportsStore.Tests;

[TestFixture]
public class ApplicationConfigurationTests
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
    public void Application_UsesCorrectEnvironment()
    {
        // Arrange & Act
        using var client = this.factory.CreateClient();
        var environment = this.factory.Services.GetRequiredService<IWebHostEnvironment>();

        // Assert
        Assert.That(environment.EnvironmentName, Is.EqualTo("Development"));
    }

    [Test]
    public void Application_LoadsConfiguration()
    {
        // Arrange & Act
        using var client = this.factory.CreateClient();
        var configuration = this.factory.Services.GetRequiredService<IConfiguration>();

        // Assert
        Assert.That(configuration, Is.Not.Null);
    }

    [Test]
    public async Task Application_RespondsToHealthCheck()
    {
        // Arrange
        using var client = this.factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert
        Assert.That(response.IsSuccessStatusCode, Is.True);
    }

    [Test]
    public void Application_HasCorrectContentRoot()
    {
        // Arrange & Act
        using var client = this.factory.CreateClient();
        var environment = this.factory.Services.GetRequiredService<IWebHostEnvironment>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(environment.ContentRootPath, Is.Not.Null);
            Assert.That(Directory.Exists(environment.ContentRootPath), Is.True);
        });
    }

    [Test]
    public void Application_HasCorrectWebRoot()
    {
        // Arrange & Act
        using var client = this.factory.CreateClient();
        var environment = this.factory.Services.GetRequiredService<IWebHostEnvironment>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(environment.WebRootPath, Is.Not.Null);
            Assert.That(Directory.Exists(environment.WebRootPath), Is.True);
        });
    }
}
