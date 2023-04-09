using System;

namespace cli_life
{
    public class Program
    {
        private static Board _board;
        private static int step = 0;

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
            for (int col = 0; col < _board.Columns; col++)
            {
                for (int row = 0; row < _board.Rows; row++)   
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

        private static void NextGen()
        {
            Console.Clear();
            _board.Advance();
            Render();
            Console.WriteLine($"IsAlive = {_board.GetIsAliveCount()}");
        }

        private static void KeyManage()
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey();

                switch (key.KeyChar)
                {
                    case ' ':
                        NextGen();
                        step++;
                        break;
                    case 'r':
                        Reset();
                        step = 0;
                        break;
                    case 's':
                        Console.WriteLine("\nSaving...");
                        _board.SaveToFile();
                        LifePlot.SaveBoardPlot(_board, $"gen-{step}", $"gen-{step}.png");
                        Console.WriteLine("Saved");
                        break;
                    case 'l':
                        Console.WriteLine("\nLoading new board...");
                        _board.LoadFromFile();
                        Console.WriteLine("Loaded.");
                        break;
                    case 'k':
                        Console.WriteLine("\nLoading figures...");
                        var figures = Figure.LoadFromJson("figures.json");
                        Console.WriteLine("Loading is complete.");

                        Console.WriteLine("Enter the name of figure:");
                        var choice = Console.ReadLine();
                        Console.WriteLine("Enter coordinates :");
                        var coord = Console.ReadLine();

                        foreach (var figure in figures)
                        {
                            if (figure.Name == choice)
                                _board.AddFigure(figure, Int32.Parse(coord.Split(' ')[0]), 
                                    Int32.Parse(coord.Split(' ')[1]));
                        }
                        Console.WriteLine("Figure is adding");
                        break;
                    case 'x':
                        if (_board.IsHorizontalSimmetry())
                            Console.WriteLine("\nHorizontal symmetry exists");
                        else
                            Console.WriteLine("\nHorizontal symmetry does not exist");
                        break;
                    case 'y':
                        if (_board.IsVerticalSimmetry())
                            Console.WriteLine("\nVertical symmetry exists");
                        else
                            Console.WriteLine("\nVertical symmetry does not exist");
                        break;
                    case 'f':
                        figures = Figure.LoadFromJson("figures.json");
                        var counts = _board.GetNumbersOfFigures(figures);
                        Console.WriteLine();

                        foreach(var count in counts)
                        {
                            Console.WriteLine($"{count.Key} count = {count.Value}");
                        }
                        break;
                }
            }
        }

        public static void Main(string[] args)
        {
            Reset();

            while (true)
            {
                KeyManage();   
            }
        }
    }
}