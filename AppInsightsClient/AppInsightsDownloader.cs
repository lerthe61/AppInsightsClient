using System;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace AppInsightsClient
{
    public sealed class AppInsightsDownloader : IAppInsightsDownloader
    {
        private const string DATE_FORMAT = "yyyy-MM-ddT00:00:00.000Z";
        private const string URL =
            "https://api.applicationinsights.io/beta/apps/{0}/{1}?{1}={2}";
        private const string QUERY_TYPE = "query";

        private const string QUERY =
            "union (customEvents" +
            "| where timestamp >= datetime({0}) and timestamp<datetime({1}))" +
            "| where name has 'COUNTER tracking' " +
            "| order by timestamp desc";
        
        private readonly string _appInsightsKey;
        private readonly string _appInsightsId;
        private readonly IAppInsightsDataParser _appInsightsDataParser;


        public AppInsightsDownloader(string appInsightsKey, string appInsightsId, IAppInsightsDataParser appInsightsDataParser)
        {
            _appInsightsKey = appInsightsKey;
            _appInsightsId = appInsightsId;
            _appInsightsDataParser = appInsightsDataParser;
        }

        public async Task<DataTable> DownloadAsync(DateTime date)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("x-api-key", _appInsightsKey);
                var parameterString = BuildQuery(date);
                var req = string.Format(URL, _appInsightsId, QUERY_TYPE, parameterString);
                HttpResponseMessage response = await client.GetAsync(req);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                return _appInsightsDataParser.Parse(content);
            }
        }

        private string BuildQuery(DateTime date)
        {
            var from = date.ToString(DATE_FORMAT);
            var to = date.AddDays(1).ToString(DATE_FORMAT);
            var request = string.Format(QUERY, from, to);
            return HttpUtility.UrlEncode(request);
        }
    }

}