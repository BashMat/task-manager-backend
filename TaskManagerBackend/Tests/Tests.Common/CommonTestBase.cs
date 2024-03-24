using Bogus;

namespace Tests.Common;

public class CommonTestBase
{
    private Lazy<Faker> FakerLazy { get; }
    protected Faker Faker => FakerLazy.Value;

    protected CommonTestBase()
    {
        FakerLazy = new Lazy<Faker>(() => new Faker());
    }
}