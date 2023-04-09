using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace cli_life
{
    public class Board
    {
        public Cell[,] Cells { get; }
        public readonly int CellSize;

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }

        private readonly Random rand = new Random();

        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(liveDensity);
        }

        public void LoadFromFile(string filePath = "Data.txt")
        {
            int rows = File.ReadAllLines(filePath).Count();

            using StreamReader sr = new StreamReader(filePath);
            for (int row = 0; row < rows; row++)
            {
                var line = sr.ReadLine().ToCharArray();
                int columns = line.Length;

                for (int col = 0; col < columns; col++)
                {
                    Cells[row, col].IsAlive = line[col].Equals('1');
                }
            }
        }

        public void SaveToFile(string filePath = "lastState.txt")
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                for (int col = 0; col < Columns; col++)
                {
                    for (int row = 0; row < Rows; row++)
                    {
                        if (Cells[col, row].IsAlive)
                            sw.Write('1');
                        else
                            sw.Write('0');
                    }
                    sw.WriteLine();
                }
            }
        }

        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }

        public void Advance()
        {
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
        }

        public void ConnectNeighbors()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    int xL = (x > 0) ? x - 1 : Columns - 1;
                    int xR = (x < Columns - 1) ? x + 1 : 0;

                    int yT = (y > 0) ? y - 1 : Rows - 1;
                    int yB = (y < Rows - 1) ? y + 1 : 0;

                    Cells[x, y].neighbors.Add(Cells[xL, yT]);
                    Cells[x, y].neighbors.Add(Cells[x, yT]);
                    Cells[x, y].neighbors.Add(Cells[xR, yT]);
                    Cells[x, y].neighbors.Add(Cells[xL, y]);
                    Cells[x, y].neighbors.Add(Cells[xR, y]);
                    Cells[x, y].neighbors.Add(Cells[xL, yB]);
                    Cells[x, y].neighbors.Add(Cells[x, yB]);
                    Cells[x, y].neighbors.Add(Cells[xR, yB]);
                }
            }
        }

        public void AddFigure(Figure figure, int xStart, int yStart)
        {
            if (figure.Rows > Rows || figure.Columns > Columns)
                throw new ArgumentOutOfRangeException("Figure size can't be more than board");

            if (xStart < 0 || xStart > Columns - 1 ||
                yStart < 0 || yStart > Rows - 1)
            {
                throw new ArgumentOutOfRangeException("Incorrect start coordinates");
            }

            for (int x = xStart; x < xStart + figure.Columns; x++)
            {
                for (int y = yStart; y < yStart + figure.Rows; y++)
                {
                    var figureCellAlive = figure.Cells[x - xStart, y - yStart].IsAlive;

                    int xBoard = x >= Columns ? x - Columns : x;
                    int yBoard = y >= Rows ? y - Rows : y;

                    Cells[xBoard, yBoard].IsAlive = figureCellAlive;
                }
            }
        }

        public int GetIsAliveCount() => Cells.Cast<Cell>().Count(cell => cell.IsAlive);

        public bool IsHorizontalSimmetry()
        {
            int middle = Columns / 2;

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < middle; col++)
                {
                    if (Cells[col, row].IsAlive != Cells[Columns - 1 - col, row].IsAlive)
                        return false;
                }
            }
            return true;
        }

        public bool IsVerticalSimmetry()
        {
            int middle = Rows / 2;

            for (int col = 0; col < Columns; col++)
            {
                for (int row = 0; row < middle; row++)
                {
                    if (Cells[col, row].IsAlive != Cells[col, Rows - 1 - row].IsAlive)
                        return false;
                }
            }
            return true;
        }

        private bool ContainsFigureInFrame(int xStart, int yStart, Figure figure)
        {
            if (!CheckNeighborCorners(xStart, yStart, figure))
                return false;

            for (int col = 0; col < figure.Columns; col++)
            {
                int xBoard = xStart + col;

                if (xBoard >= Columns)
                    xBoard -= Columns;

                for (int row = 0; row < figure.Rows; row++)
                {
                    int yBoard = yStart + row;

                    if (yBoard >= Rows)
                        yBoard -= Rows;

                    if (figure.Cells[col, row].IsAlive != Cells[xBoard, yBoard].IsAlive)
                        return false;


                    if (col == 0)
                    {
                        var upperCol = xBoard - 1 < 0 ?
                            Columns - 1 : xBoard - 1;

                        if (Cells[upperCol, yBoard].IsAlive)
                            return false;
                    }
                    else if (col == figure.Columns - 1)
                    {
                        var lowerCol = xBoard + 1 >= Columns ?
                            xBoard + 1 - Columns : xBoard + 1;

                        if (Cells[lowerCol, yBoard].IsAlive)
                            return false;
                    }

                    if (row == 0)
                    {
                        var upperRow = yBoard - 1 < 0 ?
                            Rows - 1 : yBoard - 1;

                        if (Cells[xBoard, upperRow].IsAlive)
                            return false;
                    }
                    else if (row == figure.Rows - 1)
                    {
                        var lowerRow = yBoard + 1 >= Rows ?
                            yBoard + 1 - Rows : yBoard + 1;

                        if (Cells[xBoard, lowerRow].IsAlive)
                            return false;
                    }
                }
            }
            return true;
        }

        private bool CheckNeighborCorners(int xStart, int yStart, Figure figure)
        {
            int left = xStart - 1 < 0 ? Columns - 1 : xStart - 1;

            int right = xStart + figure.Columns;
            right = right >= Columns ? right - Columns : right;

            int up = yStart - 1 < 0 ? Rows - 1 : yStart - 1;

            int down = yStart + figure.Rows;
            down = down >= Rows ? down - Rows : down;

            return !Cells[left, up].IsAlive && !Cells[left, down].IsAlive &&
                !Cells[right, up].IsAlive && !Cells[right, down].IsAlive;
        }

        public int GetNumberOfFigure(Figure figure)
        {
            if (figure.Columns > Columns || figure.Rows > Rows)
                return 0;

            int countFigures = 0;
            for (int xBoard = 0; xBoard < Columns; xBoard++)
            {
                for (int yBoard = 0; yBoard < Rows; yBoard++)
                {
                    if (ContainsFigureInFrame(xBoard, yBoard, figure))
                        countFigures++;
                }
            }
            return countFigures;
        }

        public Dictionary<string, int> GetNumbersOfFigures(IEnumerable<Figure> figures)
        {
            var numbers = new Dictionary<string, int>();

            foreach (var figure in figures)
            {
                if (numbers.ContainsKey(figure.Name))
                    numbers[figure.Name] += GetNumberOfFigure(figure);
                else
                    numbers.Add(figure.Name, GetNumberOfFigure(figure));
            }
            return numbers;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Board))
                throw new ArgumentException();

            Board other = obj as Board;
            
            if (Width != other.Width ||  Height != other.Height
                || CellSize != other.CellSize)
                return false;

            for (int col = 0; col < Columns; col++)
            {
                for (int row = 0; row < Rows; row++)
                {
                    if (other.Cells[col, row].IsAlive != Cells[col, row].IsAlive)
                        return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
