using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace caro
{
    class boardChess
    {
        private int numberLine;
        private int numberColumn;

        public boardChess()
        {
            numberLine = 0;
            numberColumn = 0;
        }

        public boardChess(int nLine, int nColumn)
        {
            numberLine = nLine;
            numberColumn = nColumn;
        }

        public int NumberLine
        {
            get { return numberLine; }
        }

        public int NumberColumn
        {
            get { return numberColumn; }
        }

        public void drawChess(Graphics g)
        {
            for (int i = 0; i < numberColumn; i++)
            {
                g.DrawLine(caroChess.pen, i * chess.width, 0, i * chess.width, numberLine * chess.height);
            }
            for (int j = 0; j < numberLine; j++)
            {
                g.DrawLine(caroChess.pen, 0, j * chess.height, numberColumn * chess.width, j * chess.height);
            }
        }

        public void drawSoldier(Graphics g, Point point, SolidBrush sb)
        {
            g.FillEllipse(sb, point.X + 2, point.Y + 2, chess.width - 4, chess.height - 4);
            //vẽ quân cờ và chỉnh chiều rộng và chiều cao
        }

        public void deleteSoldier(Graphics g, Point p, SolidBrush sb)
        {
            g.FillRectangle(sb, p.X + 1, p.Y + 1, chess.width - 2, chess.height - 2);
            // vẽ đè lên quân cờ
        }
    }
}
