using FluentAssertions;
using Xunit;

namespace TaskManagerBackend.Application.Tests.Services.Email
{
    public class WhenValidatingEmailAddressFormat : EmailServiceTestBase
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
        public void ServiceReturnsResponseWithMessageAndNullDataIfUserDoesNotExist(string emailAddress, 
                                                                                   bool isEmailAddressFormatCorrect)
        {
            ValidateEmailAddressFormat(emailAddress).Should().Be(isEmailAddressFormatCorrect);
        }
    }
}