#region Usings

using FluentAssertions;
using TaskManagerBackend.Domain.Data;
using TaskManagerBackend.Domain.Workflow;
using Xunit;

#endregion

namespace TaskManagerBackend.Domain.Tests.Data;

public class WhenCreatingRequired : StringAttributeTestBase
{
    [Theory]
    [MemberData(nameof(GetNonEmptyStringTestData))]
    public void StringAttributeIsCreatedAndItsValueIsTrimmedIfValueIsNonEmpty(string stringValue, string expectedValue)
    {
        StringAttribute attribute = StringAttribute.CreateRequired(stringValue);

        attribute.Value.Should().Be(expectedValue);
    }
    
    [Theory]
    [MemberData(nameof(GetEmptyStringTestData))]
    public void ExceptionIsThrownIfValueIsEmpty(string? value)
    { 
        // Fakes case when null value is passed
        string fakeNotNullValue = value!;
        
        Func<StringAttribute> action = () => StringAttribute.CreateRequired(fakeNotNullValue);

        action.Should().ThrowExactly<InvariantException>().And.ActionResultType.Should().Be(ActionResultType.UserError);
    }
    
    [Fact]
    public void StringAttributeIsCreatedForRandomWord()
    {
        string value = Faker.Random.Word();
        
        StringAttribute attribute = StringAttribute.CreateRequired(value);

        attribute.Value.Should().Be(value);
    }
    
    [Fact]
    public void StringAttributeIsCreatedForRandomText()
    {
        string value = Faker.Random.Words();
        
        StringAttribute attribute = StringAttribute.CreateRequired(value);

        attribute.Value.Should().Be(value);
    }
}