#region Usings

using TaskManagerBackend.Tests.Common;
using Xunit;

#endregion

namespace TaskManagerBackend.Domain.Tests.Data;

public class StringAttributeTestBase : UnitTestsBase
{
    public static TheoryData<string, string> GetNonEmptyStringTestData()
    {
        return new TheoryData<string, string>
               {
                   { "username", "username" },
                   { "username ", "username" },
                   { " username", "username" },
                   { " username ", "username" },
                   { "      username           ", "username" },
                   {
                       "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce tincidunt, erat nec finibus euismod, felis augue consequat turpis, sit amet interdum erat nisi sit amet leo.",
                       "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce tincidunt, erat nec finibus euismod, felis augue consequat turpis, sit amet interdum erat nisi sit amet leo."
                   },
                   {
                       "   \n\n\n\r\t\n\n\n\n         Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce tincidunt, erat nec finibus euismod, felis augue consequat turpis, sit amet interdum erat nisi sit amet leo.    \t\r\n\n\n\n\t\r\n   ",
                       "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce tincidunt, erat nec finibus euismod, felis augue consequat turpis, sit amet interdum erat nisi sit amet leo."
                   }
               };
    }

    public static TheoryData<string?> GetEmptyStringTestData()
    {
        return new TheoryData<string?>
               {
                   null,
                   " ",
                   "         ",
                   "   \n\n\r\n\n\t         "
               };
    }
}