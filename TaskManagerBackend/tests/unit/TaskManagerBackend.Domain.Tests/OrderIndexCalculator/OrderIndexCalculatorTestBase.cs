using TaskManagerBackend.Tests.Common;
using Xunit;

namespace TaskManagerBackend.Domain.Tests.OrderIndexCalculator;

public abstract class OrderIndexCalculatorTestBase : UnitTestBase
{
    public static TheoryData<decimal?, decimal> GetValidTestData()
    {
        return new TheoryData<decimal?, decimal>
               {
                   { null, 0m },
                   { 0m, 0m },
                   { 0.1m, 0.05m },
                   { 0.5m, 0.25m },
                   { 0.54m, 0.27m },
                   { 0.541m, 0.27m },
                   { 0.542m, 0.27m },
                   { 0.549m, 0.27m },
                   { 0.549999999999999999999999999m, 0.27m },
                   { 0.5499999999999999999999999999m, 0.28m },
                   { 0.54999999999999999999999999999m, 0.28m },
                   { 0.55m, 0.28m },
                   { 0.569999999999999999999999999m, 0.28m },
                   { 0.56999999999999999999999999999m, 0.28m },
                   { 0.57m, 0.28m },
                   { 0.5700000000000000000000000001m, 0.28m },
                   { 0.57000000000000000000000000014999m, 0.28m },
                   { 0.57000000000000000000000000015m, 0.29m },
                   { 0.570000000000000000000000001m, 0.29m },
                   { 0.571m, 0.29m },
                   { 0.571999999999999999999999999m, 0.29m },
                   { 0.5719999999999999999999999999m, 0.29m },
                   { 0.57199999999999999999999999999m, 0.29m },
                   { 0.572m, 0.29m },
                   { 0.58m, 0.29m },
                   { 1m, 0.5m },
                   { 10m, 5m },
                   { 100_000m, 50_000m }
               };
    }
}