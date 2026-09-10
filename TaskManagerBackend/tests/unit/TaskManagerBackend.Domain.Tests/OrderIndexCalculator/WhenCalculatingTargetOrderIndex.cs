using FluentAssertions;
using Xunit;

namespace TaskManagerBackend.Domain.Tests.OrderIndexCalculator;

public class WhenCalculatingTargetOrderIndex : OrderIndexCalculatorTestBase
{
    [Theory]
    [MemberData(nameof(GetValidTestData))]
    public void OrderedIndexIsCalculatedCorrectlyForValidInputData(decimal? input, decimal expectedValue)
    {
        decimal actualValue = Features.Tracking.TrackingLogEntry.OrderIndexCalculator.CalculateTargetOrderIndex(input);

        actualValue.Should().Be(expectedValue);
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(-0.1)]
    [InlineData(-0.0000000000000000000000000001)]
    [InlineData(100_000.000000001)]
    [InlineData(100_001)]
    public void ArgumentExceptionIsThrownIfArgumentIsOutOfRange(decimal input)
    { 
        Func<decimal> action = () => Features.Tracking.TrackingLogEntry.OrderIndexCalculator.CalculateTargetOrderIndex(input);

        action.Should().ThrowExactly<ArgumentException>();
    }
}