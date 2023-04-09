using ScottPlot;

namespace cli_life
{
    public class LifePlot
    {
        public static void SaveBoardPlot(Board board, string name, string filePath)
        {
            var plt = new Plot(board.Width, board.Height);

            var data = new double[board.Columns, board.Rows];

            for (int i = 0; i < board.Columns; i++)
                for (int j = 0; j < board.Rows; j++)
                    data[i, j] = board.Cells[i, j].IsAlive ? 1 : 0;

            var hm = plt.AddHeatmap(data, lockScales: false);
            hm.CellWidth = board.CellSize;
            hm.CellHeight = board.CellSize;

            plt.Title(name);
            plt.SaveFig(filePath);
        }
    }
}
