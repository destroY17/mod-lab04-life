using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
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

        public void LoadFromFile(string filePath)
        {
            int rows = File.ReadLines(filePath).Count();
            int columns = 0;

            using (StreamReader sr = new StreamReader(filePath))
            {
                for (int row = 0; row < Rows; row++)
                {
                    var line = sr.ReadLine().ToCharArray();
                    columns = line.Length;

                    for (int column = 0; column < Columns; column++)
                    {
                        Cells[column, row].IsAlive = line[column].Equals('1');
                    }
                }

                if (rows != Rows || columns != Columns)
                    throw new FileLoadException("Incorrect file format");
            }

        }

        public void SaveToFile(string filePath = "lastState.txt")
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                for (int row = 0; row < Rows; row++)
                {
                    for (int column = 0; column < Columns; column++)
                    {
                        if (Cells[column, row].IsAlive)
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

                    if (x < Columns && y < Rows)
                    {
                        Cells[x, y].IsAlive = figureCellAlive;
                    }
                    else if (x == Columns && y == Rows)
                    {
                        Cells[0, 0].IsAlive = figureCellAlive;
                    }
                    else if (x == Columns)
                    {
                        Cells[0, y].IsAlive = figureCellAlive;
                    }
                    else if (y == Rows)
                    {
                        Cells[x, 0].IsAlive = figureCellAlive;
                    }
                }
            }
        }

        public int GetIsAliveCount()
        {
            return Cells.Cast<Cell>().Count(cell => cell.IsAlive);
        }

        public bool IsHorizontalSimmetry()
        {
            int middle = 
        }
    }
}
