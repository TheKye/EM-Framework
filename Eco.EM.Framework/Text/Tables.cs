using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Eco.EM.Framework.Text
{
    public class Row
    {
        public string[] Values { get; set; }
    }

    public class Column
    {
        public string Title { get; set; }
    }

    public class Table
    {
        private const int MaxWidth = 250;

        public Column[] Columns { get; private set; }
        public List<Row> Rows { get; set; } = new List<Row>();

        public Table(int columns)
        {
            this.Columns = new Column[columns].Select(c => new Column()).ToArray();
        }
    }

    public static class Tables
    {
        private const int MaxWidth = 250;

        public static Table New(int columns) => new Table(columns);

        public static string ToString(Table table)
        {
            var lines = new List<string>();
            var columnWidth = (int)Math.Floor((decimal)(MaxWidth / table.Columns.Length));

            var columnLine = string.Empty;
            foreach (var column in table.Columns)
            {
                if (!string.IsNullOrEmpty(column.Title))
                {
                    columnLine += column.Title.PadRight(columnWidth, ' ');
                }
                else
                {
                    columnLine += "Title".PadRight(columnWidth, ' ');
                }
            }

            foreach (var row in table.Rows)
            {
                var line = string.Empty;
                for (int i = 0; i < table.Columns.Length; i++)
                {
                    line += row.Values[i].PadRight(columnWidth, ' ');
                }

                lines.Add(line);
            }

            return string.Join(System.Environment.NewLine, lines);
        }
    }
}