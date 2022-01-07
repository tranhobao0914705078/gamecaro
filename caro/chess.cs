using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace caro
{
    class chess
    {
        public const int width = 25;
        public const int height = 25;

        private int Line;
        private int Column;
        private Point Address;
        private int Own;

        public chess(int line, int column, Point address, int own)
        {
            Line = line;
            Column = column;
            Address = address;
            Own = own;
        }

        public chess()
        {
        }

        public int Line1 { get => Line; set => Line = value; }
        public int Column1 { get => Column; set => Column = value; }
        public Point Address1 { get => Address; set => Address = value; }
        public int Own1 { get => Own; set => Own = value; }
    }
}
