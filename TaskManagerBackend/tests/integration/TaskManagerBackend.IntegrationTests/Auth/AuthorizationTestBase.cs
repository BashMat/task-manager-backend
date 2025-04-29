#region Usings

using Microsoft.AspNetCore.Mvc.Testing;
using TaskManagerBackend.Tests.Common;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Auth;

public class AuthorizationTestBase : CommonTestBase, IClassFixture<MsSqlTests>, IDisposable
{
    private readonly WebApplicationFactory<TaskManagerBackend.Application.Program> _webApplicationFactory;

    protected TaskManagerBackendHttpClient HttpClient { get; init; }

    protected AuthorizationTestBase(MsSqlTests fixture)
    {
        var clientOptions = new WebApplicationFactoryClientOptions();
        clientOptions.AllowAutoRedirect = false;

        _webApplicationFactory = new CustomWebApplicationFactory(fixture);
        HttpClient = _webApplicationFactory.CreateClient(clientOptions);
    }

    public void Dispose()
    {
        _webApplicationFactory.Dispose();
    }
}