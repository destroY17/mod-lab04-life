using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Linq;

namespace cli_life
{
    public class Figure
    {
        public string Name { get; }
        public int Rows { get; }
        public int Columns { get; }
        public string Encoding { get; }
        public Cell[,] Cells { get; }

        public Figure(string name, int rows, int columns, string encoding)
        {
            Name = name;
            Rows = rows;
            Columns = columns;
            Encoding = encoding;
            Cells = Decoding();
        }

        private Cell[,] Decoding()
        {
            var cells = new Cell[Columns, Rows];

            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    cells[x, y] = new Cell()
                    {
                        IsAlive = Encoding[x * (Columns - 1) + y].Equals('1')
                    };
                }
            }
            return cells;
        }

        public static List<Figure> LoadFromJson(string filePath)
        {
            var json = File.ReadAllText(filePath);

            using (var document = JsonDocument.Parse(json))
            {
                var figuresCollections = document.RootElement.EnumerateArray();
                var figures = new List<Figure>();

                foreach(var figure in figuresCollections)
                {
                    var name = figure.GetProperty("Name").GetString();
                    var rows = figure.GetProperty("Rows").GetInt32();
                    var columns = figure.GetProperty("Columns").GetInt32();
                    var encoding = string.Join("", figure.GetProperty("Encoding").EnumerateArray()
                        .Select(row => row.GetString()));

                    figures.Add(new Figure(name, rows, columns, encoding));
                }
                return figures;
            }
        }
    }
}
