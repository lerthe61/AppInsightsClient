using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Newtonsoft.Json;

namespace AppInsightsClient
{
    public sealed class AppInsightsDataParser : IAppInsightsDataParser
    {
        private readonly Predicate<string> _isJsonColumnPredicate;

        public AppInsightsDataParser(Predicate<string> isJsonColumnPredicate)
        {
            if (isJsonColumnPredicate == null) throw new ArgumentNullException(nameof(isJsonColumnPredicate));

            _isJsonColumnPredicate = isJsonColumnPredicate;
        }

        public DataTable Parse(string raw)
        {
            var dataTable = new DataTable();

            var data = JsonConvert.DeserializeObject<RootObject>(raw);

            var table = data.Tables.First();
            var records = new List<Dictionary<string, object>>(table.Rows.Count);

            // Parse data
            foreach (var row in table.Rows)
            {
                var record = new Dictionary<string, object>();
                PopulateRecord(record, row, table.Columns);
                records.Add(record);
            }

            // populate DataTable
            var uniqueColumns = records.SelectMany(_ => _.Keys).Distinct().ToList();
            foreach (var column in uniqueColumns)
            {
                dataTable.Columns.Add(column);
            }
            foreach (var record in records)
            {
                var row = dataTable.NewRow();
                foreach (var key in record.Keys)
                {
                    row[key] = record[key];
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        private void PopulateRecord(Dictionary<string, object> record, List<object> row, List<Column> tableColumns)
        {
            for (int i = 0; i < tableColumns.Count; i++)
            {
                if (_isJsonColumnPredicate(tableColumns[i].ColumnName))
                {
                    AddJson(record, row[i]);
                }
                else
                {
                    AddObject(record, tableColumns[i].ColumnName, row[i]);
                }
            }
        }

        private void AddJson(Dictionary<string, object> record, object obj)
        {
            var str = (string)obj;
            if (string.IsNullOrEmpty(str)) return;

            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(str);
            foreach (var key in dict.Keys)
            {
                record.Add(key, dict[key]);
            }
        }

        private void AddObject(Dictionary<string, object> record, string columnName, object obj)
        {
            record.Add(columnName, obj);
        }
    }

}