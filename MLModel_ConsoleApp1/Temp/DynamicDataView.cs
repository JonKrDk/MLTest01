namespace MLModel_ConsoleApp1.Temp
{
    using Microsoft.ML;
    using Microsoft.ML.Data;
    using System;
    using System.Collections.Generic;

    public class DynamicDataView : IDataView
    {
        private readonly IList<Dictionary<string, object>> _rows;
        private readonly DataViewSchema _schema;

        public DynamicDataView(IList<Dictionary<string, object>> rows, IDictionary<string, Type> columnTypes)
        {
            _rows = rows;

            var builder = new DataViewSchema.Builder();

            foreach (var col in columnTypes)
            {
                builder.AddColumn(col.Key, GetDataViewType(col.Value));
            }

            _schema = builder.ToSchema();
        }

        public bool CanShuffle => false;
        public DataViewSchema Schema => _schema;

        public long? GetRowCount() => _rows.Count;

        public DataViewRowCursor GetRowCursor(
            IEnumerable<DataViewSchema.Column> columnsNeeded,
            Random rand = null)
            => new Cursor(this, columnsNeeded);

        public DataViewRowCursor[] GetRowCursorSet(
            IEnumerable<DataViewSchema.Column> columnsNeeded, int n, Random rand = null)
            => new[] { GetRowCursor(columnsNeeded, rand) };

        private static DataViewType GetDataViewType(Type t)
        {
            if (t == typeof(int)) return NumberDataViewType.Int32;
            if (t == typeof(float)) return NumberDataViewType.Single;
            if (t == typeof(double)) return NumberDataViewType.Double;
            if (t == typeof(bool)) return BooleanDataViewType.Instance;
            if (t == typeof(string)) return TextDataViewType.Instance;

            throw new NotSupportedException($"Unsupported type: {t}");
        }

        private class Cursor : DataViewRowCursor
        {
            private readonly DynamicDataView _parent;
            private readonly List<DataViewSchema.Column> _activeCols;
            private int _index = -1;

            public Cursor(DynamicDataView parent, IEnumerable<DataViewSchema.Column> neededCols)
            {
                _parent = parent;
                _activeCols = new List<DataViewSchema.Column>(neededCols);
            }

            public override DataViewSchema Schema => _parent.Schema;

            public override long Position => _index;
            public override long Batch => 0;

            public override bool MoveNext()
            {
                _index++;
                return _index < _parent._rows.Count;
            }

            public override ValueGetter<TValue> GetGetter<TValue>(DataViewSchema.Column column)
            {
                return (ref TValue value) =>
                {
                    var row = _parent._rows[_index];
                    value = (TValue)row[column.Name];
                };
            }

            public override bool IsColumnActive(DataViewSchema.Column column)
                => _activeCols.Contains(column);
        }
    }
}
