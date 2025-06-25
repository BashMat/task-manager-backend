#region Usings

using FluentAssertions;
using Xunit;

#endregion

namespace TaskManagerBackend.Domain.Tests.Email;

public class WhenValidating : EmailValidatorTestBase
{
    [Theory]
    [InlineData("test@test.com", true)]
    [InlineData("test@test.ru", true)]
    [InlineData("test@test.net", true)]
    [InlineData("Test@test.com", true)]
    [InlineData("test@Test.com", true)]
    [InlineData("test@Test.CoM", true)]
    [InlineData("te st@test.com", false)]
    [InlineData("test@te st.com", false)]
    [InlineData("test test@test.com", false)]
    public void ValidationIsCorrectForVariousAddresses(string emailAddress, 
                                                       bool isEmailAddressFormatCorrect)
    {
        Validate(emailAddress).Should().Be(isEmailAddressFormatCorrect);
    }
}