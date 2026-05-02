#region Usings

using FluentAssertions;
using TaskManagerBackend.Domain.Shared.Data;
using Xunit;

#endregion

namespace TaskManagerBackend.Domain.Tests.Data;

public class WhenCreatingOptional : StringAttributeTestBase
{
    [Theory]
    [MemberData(nameof(GetNonEmptyStringTestData))]
    public void StringAttributeIsCreatedAndIsNotNullAndItsValueIsTrimmedIfValueIsNonEmpty(string stringValue, string expectedValue)
    {
        StringAttribute? attribute = StringAttribute.CreateOptional(stringValue);

        attribute.Should().NotBeNull();
        attribute.Value.Should().Be(expectedValue);
    }
    
    [Theory]
    [MemberData(nameof(GetEmptyStringTestData))]
    public void StringAttributeIsCreatedAndIsNullIfValueIsEmpty(string? value)
    { 
        StringAttribute? attribute = StringAttribute.CreateOptional(value);

        attribute.Should().BeNull();
    }
    
    [Fact]
    public void StringAttributeIsCreatedForRandomWord()
    {
        string value = Faker.Random.Word();
        
        StringAttribute? attribute = StringAttribute.CreateOptional(value);

        attribute.Should().NotBeNull();
        attribute.Value.Should().Be(value);
    }
    
    [Fact]
    public void StringAttributeIsCreatedForRandomText()
    {
        string value = Faker.Random.Words();
        
        StringAttribute? attribute = StringAttribute.CreateOptional(value);

        attribute.Should().NotBeNull();
        attribute.Value.Should().Be(value);
    }
}