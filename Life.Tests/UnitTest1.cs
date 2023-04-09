using cli_life;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Life.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private Board InitBoard()
        {
            var config = JsonManagement.ReadJson<Config>("appConfig.json");

            return new Board(
                width: config.Width,
                height: config.Height,
                cellSize: config.CellSize,
                liveDensity: config.LiveDensity
                );
        }

        private void EmptyBoard(Board board)
        {
            board.Cells.Cast<Cell>().ToList().ForEach(cell => cell.IsAlive = false);
        }

        [TestMethod]
        public void CheckReadJsonConfig()
        {
            var board = InitBoard();

            Assert.IsTrue(board.Width == 1000 && board.Height == 1000 &&
                board.CellSize == 100 && board.Columns == 10 &&
                board.Rows == 10);
        }

        [TestMethod]
        public void CheckReadJsonFigures()
        {
            var figures = Figure.LoadFromJson("figures.json");
            Assert.IsTrue(figures.Count == 17);
        }

        [TestMethod]
        public void CheckLoadBoard()
        {
            var board = InitBoard();
            board.LoadFromFile("LoadTest.txt");

            Assert.IsTrue(board.Cells[0, 0].IsAlive);
            Assert.IsTrue(board.Cells[1, 1].IsAlive);
            Assert.IsTrue(board.Cells[7, 3].IsAlive);
            Assert.IsTrue(board.Cells[9, 9].IsAlive);
        }

        [TestMethod]
        public void CheckSaveBoard()
        {
            var board = InitBoard();
            board.SaveToFile("SaveTest.txt");
            var expected = board;

            board = InitBoard();
            board.LoadFromFile("SaveTest.txt");

            Assert.AreEqual(expected, board);
        }

        [TestMethod]
        public void CheckNumbersOfFigures()
        {
            var figures = Figure.LoadFromJson("figures.json");

            var board = InitBoard();
            board.LoadFromFile("NumbersOfFigures.txt");

            var count = board.GetNumbersOfFigures(figures);

            Assert.IsTrue(count["Block"] == 1);
            Assert.IsTrue(count["Box"] == 1);
            Assert.IsTrue(count["Loaf"] == 1);
        }

        [TestMethod]
        public void CheckAddFigures()
        {
            var figures = Figure.LoadFromJson("figures.json");

            var board = InitBoard();
            EmptyBoard(board);

            board.AddFigure(figures[0], 0, 0);
            board.AddFigure(figures[1], 4, 0);
            board.AddFigure(figures[16], 5, 5);

            Assert.IsTrue(board.GetNumberOfFigure(figures[0]) == 1);
            Assert.IsTrue(board.GetNumberOfFigure(figures[1]) == 1);
            Assert.IsTrue(board.GetNumberOfFigure(figures[16]) == 1);
        }

        [TestMethod]
        public void CheckIsAliveCount()
        {
            var board = InitBoard();
            board.LoadFromFile("LoadTest.txt");

            Assert.IsTrue(board.GetIsAliveCount() == 4);
        }

        [TestMethod]
        public void CheckSimmetry()
        {
            var board = InitBoard();
            board.LoadFromFile("SymmetryTest.txt");

            Assert.IsTrue(board.IsHorizontalSimmetry()
                && board.IsVerticalSimmetry());
        }
    }
}
