#region Usings

using Microsoft.AspNetCore.Mvc.Testing;
using TaskManagerBackend.Tests.Common;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests;

[Trait(CategoryTraitName, CategoryTraitValueIntegrationTests)]
public class IntegrationTestBase : CommonTestBase, IClassFixture<MsSqlTests>, IDisposable
{
    public const string UserName = "test";
    public const string Email = "test@test.test";
    public const string Password = "test-1234";
    
    private readonly WebApplicationFactory<TaskManagerBackend.Application.Program> _webApplicationFactory;

    protected TaskManagerBackendHttpClient HttpClient { get; init; }

    protected IntegrationTestBase(MsSqlTests fixture)
    {
        var clientOptions = new WebApplicationFactoryClientOptions();
        clientOptions.AllowAutoRedirect = false;

        _webApplicationFactory = new CustomWebApplicationFactory(fixture);
        HttpClient = _webApplicationFactory.CreateClient(clientOptions);
    }

    public virtual void Dispose()
    {
        _webApplicationFactory.Dispose();
    }
}