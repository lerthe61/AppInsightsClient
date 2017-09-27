# DRAFT

Code snippet that allow to download data from Application Insights. AppInsightsDataParser also allow to expand values from columns that conatin json encoded data into separate columns

## Usage
```
var parser = new AppInsightsDataParser(_ => _ == "jsonColumn");
var client = new AppInsightsDownloader(key, id, parser);
var data = await client.DownloadAsync(...);
```

### ToDo
* Move Query out of client code
* write documentation