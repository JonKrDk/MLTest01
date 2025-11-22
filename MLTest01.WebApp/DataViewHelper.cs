using Microsoft.ML;
using System.Data;

namespace MLTest01.WebApp
{
    public static class DataViewHelper
    {
        public static DataTable? ToDataTable(this IDataView dataView)
        {
            DataTable? dataTable = null;

            if (dataView is not null)
            {
                dataTable = new DataTable();
                var preview = dataView.Preview();
                dataTable.Columns.AddRange(preview.Schema.Select(column => new DataColumn(column.Name, typeof(object))).ToArray());
                foreach (var row  in preview.RowView)
                {
                    var dataRow = dataTable.NewRow();
                    foreach (var column in row.Values)
                    {
                        dataRow[column.Key] = column.Value ?? DBNull.Value;
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }
    }
}
