using System.Data;

namespace AppInsightsClient
{
    public interface IAppInsightsDataParser
    {
        DataTable Parse(string raw);
    }
}