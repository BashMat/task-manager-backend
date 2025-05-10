#region Usings

using Bogus;

#endregion

namespace TaskManagerBackend.Tests.Common;

public class CommonTestBase
{
    public const string CategoryTraitName = "Category";
    public const string CategoryTraitValueUnitTests = "Unit";
    public const string CategoryTraitValueIntegrationTests = "Integration";
    
    private Lazy<Faker> FakerLazy { get; }
    protected Faker Faker => FakerLazy.Value;

    protected CommonTestBase()
    {
        FakerLazy = new Lazy<Faker>(() => new Faker());
    }
}