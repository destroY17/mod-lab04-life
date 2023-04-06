using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;


namespace cli_life
{
    public class Program
    {
        private static Board _board;

        private static void Reset()
        {
            var config = JsonManagement.ReadJson<Config>("appConfig.json");

            _board = new Board(
                width: config.Width,
                height: config.Height,
                cellSize: config.CellSize,
                liveDensity: config.LiveDensity
                );
        }

        private static void Render()
        {
            for (int row = 0; row < _board.Rows; row++)
            {
                for (int col = 0; col < _board.Columns; col++)   
                {
                    var cell = _board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        Console.Write('*');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.Write('\n');
            }
        }

        private static void Start()
        {
            Console.Clear();
            Render();
            Console.WriteLine($"IsAlive = {_board.GetIsAliveCount()}");
            _board.Advance();
        }

        private static void KeyManage()
        {
            if (true)
            {
                var key = Console.ReadKey();

                switch (key.KeyChar)
                {
                    case 's':
                        Console.WriteLine("Saving...");
                        _board.SaveToFile();
                        Console.WriteLine("Saved");
                        break;
                    case 'l':
                        Console.WriteLine("Loading new board...");
                        _board.LoadFromFile("Data.txt");
                        Console.WriteLine("Loaded.");
                        break;
                    case 'k':
                        Console.WriteLine("Loading figures...");
                        var figures = Figure.LoadFromJson("figures.json");
                        Console.WriteLine("Loading is complete.");

                        _board.AddFigure(figures[0], 9, 9);
                        Console.WriteLine("Figure is adding");
                        break;
                }
            }
        }

        public static void Main(string[] args)
        {
            Reset();

            while (true)
            {
                Start();
                KeyManage();
                //Thread.Sleep(1000);
            }
        }
    }
}