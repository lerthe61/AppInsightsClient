using System;
using System.Data;
using System.Threading.Tasks;

namespace AppInsightsClient
{
    public interface IAppInsightsDownloader
    {
        Task<DataTable> DownloadAsync(DateTime date);
    }
}