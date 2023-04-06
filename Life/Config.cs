using System;
using System.Collections.Generic;
using System.Text;

namespace cli_life
{
    public class Config
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }
    }
}
