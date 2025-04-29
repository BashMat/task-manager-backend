#region Usings

using Microsoft.AspNetCore.Mvc.Testing;
using TaskManagerBackend.Tests.Common;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests;

public class IntegrationTestBase : CommonTestBase, IClassFixture<MsSqlTests>, IDisposable
{
    public const string UserName = "test";
    public const string Email = "test@test.dev";
    public const string Password = "test";
    
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