#region Usings

using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.User;

public class UserTestBase : IntegrationTestBase, IClassFixture<MsSqlTests>, IDisposable
{
    protected UserTestBase(MsSqlTests fixture) : base(fixture) { }
}