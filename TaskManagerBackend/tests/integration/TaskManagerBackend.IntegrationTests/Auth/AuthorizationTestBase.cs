#region Usings

using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Auth;

public class AuthorizationTestBase : IntegrationTestBase, IClassFixture<MsSqlTests>, IDisposable
{
    protected AuthorizationTestBase(MsSqlTests fixture) : base(fixture) { }
}