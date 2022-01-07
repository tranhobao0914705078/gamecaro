using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace caro
{
    public enum EndGame
    {
        draw,
        player1,
        player2,
        com
    }
    class caroChess
    {
        public static Pen pen;
        public static SolidBrush sbWhite;
        public static SolidBrush sbBlack;
        public static SolidBrush sbGreen;
        private chess[,] arrayChess;
        private boardChess boardChess;
        private Stack<chess> stack_undo; 
        private Stack<chess> stack_redo;
        private int play;
        private bool ready;
        private EndGame endgame;
        private int gameMode;

        public bool Ready { get => ready;}
        public int GameMode { get => gameMode; }

        public caroChess()
        {
            pen = new Pen(Color.Black);
            sbBlack = new SolidBrush(Color.Black);
            sbWhite = new SolidBrush(Color.White);
            sbGreen = new SolidBrush(Color.FromArgb(128, 255, 128));
            boardChess = new boardChess(20,20);
            arrayChess = new chess[boardChess.NumberLine, boardChess.NumberColumn];
            stack_undo = new Stack<chess>();
            stack_redo = new Stack<chess>();
            play = 1;
           
        }

        public void drawChess(Graphics g)
        {
            boardChess.drawChess(g);
        }

        public void createArrayChess()
        {
            for(int i = 0; i < boardChess.NumberLine; i++)
            {
                for(int j = 0; j < boardChess.NumberColumn; j++)
                {
                    arrayChess[i, j] = new chess(i,j,new Point(j*chess.width, i*chess.height),0);
                }
            }
        }

        #region play chess
        public bool playChess(int MouseX, int MouseY, Graphics g)
        {

            if (MouseX % chess.width == 0 || MouseY % chess.height == 0)
                return false;
            int Column = MouseX / chess.width;
            int Line = MouseY / chess.height;
            if(arrayChess[Column, Line].Own1 != 0)
            {
                return false;
            }

            switch (play)
            {
                case 1:
                    arrayChess[Column, Line].Own1 = 1;
                    boardChess.drawSoldier(g, arrayChess[Column, Line].Address1, sbBlack);
                    play = 2;
                    break;
                case 2:
                    arrayChess[Column, Line].Own1 = 2;
                    boardChess.drawSoldier(g, arrayChess[Column, Line].Address1, sbWhite);
                    play = 1;
                    break;
                default:
                    MessageBox.Show("Có lỗi xảy ra!!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
            chess c = new chess(arrayChess[Column, Line].Line1, arrayChess[Column, Line].Column1, arrayChess[Column, Line].Address1, arrayChess[Column, Line].Own1);
            stack_undo.Push(c);
            stack_redo.Push(c);

            return true;
        }
        #endregion

        public void redrawChess(Graphics g)
        {
            foreach(chess a in stack_undo)
            {
                if(a.Own1 == 1)
                {
                    boardChess.drawSoldier(g, a.Address1, sbBlack);
                }
                else if(a.Own1 == 2)
                {
                    boardChess.drawSoldier(g, a.Address1, sbWhite);
                }
            }
        }
        #region PvsP

        public void startPlayervsPlayer(Graphics g)
        {
            ready = true;
            stack_undo = new Stack<chess>();
            stack_redo = new Stack<chess>();
            play = 1;
            gameMode = 1;
            createArrayChess();
            drawChess(g);
        }
        
        public  void startPlayervsCom(Graphics g)
        {
            ready = true;
            stack_undo = new Stack<chess>();
            stack_redo = new Stack<chess>();
            play = 1;
            gameMode = 2;
            createArrayChess();
            drawChess(g);
            startAI(g);
        }
        #endregion

        #region undo and redo
        public void undo(Graphics g)
        {
            if(stack_undo.Count != 0)
            {
                chess a = stack_undo.Pop();
                //stack_undo.Push(new chess(a.Line1, a.Column1, a.Address1, a.Own1));
                arrayChess[a.Line1, a.Column1].Own1 = 0;
                boardChess.deleteSoldier(g, a.Address1, sbGreen);
            }
            else
            {
                MessageBox.Show("Không có quân cờ trên bàn cờ!!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void redo(Graphics g)
        {
            if (stack_redo.Count != 0)
            {
                chess b = stack_redo.Pop();
                //stack_redo.Push(new chess(b.Line1, b.Column1, b.Address1, b.Own1));
                arrayChess[b.Line1, b.Column1].Own1 = b.Own1;
                boardChess.drawSoldier(g, b.Address1, b.Own1 == 1 ? sbBlack : sbWhite);
            }
            else
            {
                MessageBox.Show("Không có quân cờ trên bàn cờ!!!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region win and lose

        public void endPlayChess()
        {
            switch (endgame)
            {
                case EndGame.draw:
                    MessageBox.Show("Hòa cờ!!!", "Thông Báo");
                    break;
                case EndGame.player1:
                    MessageBox.Show("Người chơi 1 thắng!!!", "Thông Báo");
                    break;
                case EndGame.player2:
                    MessageBox.Show("Người chơi 2 thắng!!!", "Thông Báo");
                    break;
                case EndGame.com:
                    MessageBox.Show("Máy thắng!!!", "Thông Báo");
                    break;
            }
            ready = false;
        }
        public bool checkVictory()
        {
            // if hòa cờ
            if(stack_redo.Count == boardChess.NumberColumn * boardChess.NumberLine)
            {
                endgame = EndGame.draw;
                return true;
            }
            foreach(chess c in stack_redo)
            {
                if (verticalBrowsing(c.Column1, c.Line1, c.Own1) || horizontalBrowsing(c.Column1,c.Line1,c.Own1) || cross_browse(c.Column1, c.Line1, c.Own1) || reverse_browse(c.Column1, c.Line1, c.Own1))
                {
                    endgame = c.Own1 == 1 ? EndGame.player1 : EndGame.player2;
                    return true;
                }
            }
            return false;
        }

        // duyệt dọc
        private bool verticalBrowsing(int crrCol, int crrLine, int crrOwn)
        {
            if (crrLine > boardChess.NumberLine - 5)
                return false;
            int dem;
            for (dem = 1; dem < 5; dem++)
            {
                if (arrayChess[crrLine + dem, crrCol].Own1 != crrOwn)
                    return false;
            }          
            if (crrLine == 0 || crrLine + dem == boardChess.NumberLine)
                return true;
            if (arrayChess[crrLine - 1, crrCol].Own1 == 0 || arrayChess[crrLine + dem,crrCol].Own1 == 0)
                return true;
            return false;
        }
        // duyệt ngang
        private bool horizontalBrowsing(int crrCol, int crrLine, int crrOwn)
        {
            if (crrCol > boardChess.NumberColumn - 5)
                return false;
            int dem;
            for (dem = 1; dem < 5; dem++)
            {
                if (arrayChess[crrLine, crrCol + dem].Own1 != crrOwn)
                    return false;
            }
            if (crrCol == 0 || crrCol + dem == boardChess.NumberColumn)
                return true;
            if (arrayChess[crrLine, crrCol - 1].Own1 == 0 || arrayChess[crrLine, crrCol + dem].Own1 == 0)
                return true;
            return false;
        }

        // duyệt chéo xuôi
        private bool cross_browse(int crrCol, int crrLine, int crrOwn)
        {
            if (crrLine > boardChess.NumberLine - 5 || crrCol > boardChess.NumberColumn - 5)
                return false;
            int dem;
            for (dem = 1; dem < 5; dem++)
            {
                if (arrayChess[crrLine + dem, crrCol + dem].Own1 != crrOwn)
                    return false;
            }
            if (crrLine == 0 || crrLine + dem == boardChess.NumberLine || crrCol == 0 || crrCol + dem == boardChess.NumberColumn)
                return true;
            if (arrayChess[crrLine - 1, crrCol - 1].Own1 == 0 || arrayChess[crrLine + dem, crrCol + dem].Own1 == 0)
                return true;
            return false;
        }

        // duyệt chéo ngược
        private bool reverse_browse(int crrCol, int crrLine, int crrOwn)
        {
            if (crrLine < 4 || crrCol > boardChess.NumberColumn - 5)
                return false;
            int dem;
            for (dem = 1; dem < 5; dem++)
            {
                if (arrayChess[crrLine - dem, crrCol + dem].Own1 != crrOwn)
                    return false;
            }
            if (crrLine == 4 || crrLine == boardChess.NumberLine - 1 || crrCol == 0 || crrCol + dem == boardChess.NumberColumn)
                return true;
            if (arrayChess[crrLine + 1, crrCol - 1].Own1 == 0 || arrayChess[crrLine - dem, crrCol + dem].Own1 == 0)
                return true;
            return false;
        }
        #endregion
        // heuristic
        #region AI
        private long[] array_point_attack = new long[7] {0, 9, 54, 162, 1458, 13112, 118008 };
        private long[] array_point_defensive = new long[7] {0, 3, 27, 99, 729, 6561, 59049};
        public void startAI(Graphics g)
        {
            if(stack_redo.Count == 0)
            {
                playChess(boardChess.NumberColumn / 2 * chess.width + 1, boardChess.NumberLine / 2 * chess.height + 1, g);
            }
            else
            {
                chess c = search_direction();
                playChess(c.Address1.X + 1, c.Address1.Y + 1, g);
            }
        }
        
        private chess search_direction()
        {
            chess chessResult = new chess();
            long max_point = 0;
            for(int i = 0; i < boardChess.NumberLine; i++)
            {
                for(int j = 0; j < boardChess.NumberColumn; j++)
                {
                    if(arrayChess[i,j].Own1 == 0)
                    {
                        long point_attack = point_attack_verticalBrowsing(i,j) + point_attack_horizontalBrowsing(i, j) + point_attack_reverse_browse(i, j) + point_attack_cross_browse(i, j);
                        long point_defensive = point_defensive_verticalBrowsing(i, j) + point_defensive_horizontalBrowsing(i, j) + point_defensive_reverse_browse(i, j) + point_defensive_cross_browse(i, j);
                        long point_tamp = point_attack > point_defensive ? point_attack : point_defensive;
                        if (max_point < point_tamp)
                        {
                            max_point = point_tamp;
                            chessResult = new chess(arrayChess[i, j].Line1, arrayChess[i, j].Column1, arrayChess[i, j].Address1, arrayChess[i, j].Own1);
                        }
                    }
                }
            }

            return chessResult;
        }
        #region tấn công
        private long point_attack_verticalBrowsing(int currLine, int currCol)
        {
            long total_score = 0;
            int soQuanTa = 0;
            int soQuanDich = 0;
            for(int dem = 1; dem < 6 && currLine + dem < boardChess.NumberLine; dem++)
            {
                if (arrayChess[currLine + dem, currCol].Own1 == 1)
                {
                    soQuanTa++;
                }
                else if (arrayChess[currLine + dem, currCol].Own1 == 2)
                {
                    soQuanDich++;
                    break;
                }
                else
                    break;
            }
            for (int dem  = 1; dem < 6 && currLine - dem >= 0; dem++)
            {
                if (arrayChess[currLine - dem, currCol].Own1 == 1)
                {
                    soQuanTa++;
                }
                else if (arrayChess[currLine - dem, currCol].Own1 == 2)
                {
                    soQuanDich++;
                    break;
                }
                else
                    break;
            }
            if (soQuanDich == 2)
                return 0;
            total_score -= array_point_defensive[soQuanDich + 1]*2;
            total_score += array_point_attack[soQuanTa];
            return total_score;
        }
        private long point_attack_horizontalBrowsing(int currLine, int currCol)
        {
            long total_score = 0;
            int soQuanTa = 0;
            int soQuanDich = 0;
            for (int dem  = 1; dem < 6 && currCol + dem < boardChess.NumberColumn; dem++)
            {
                if (arrayChess[currLine, currCol + dem].Own1 == 1)
                {
                    soQuanTa++;
                }
                else if (arrayChess[currLine, currCol + dem].Own1 == 2)
                {
                    soQuanDich++;
                    break;
                }
                else
                    break;
            }
            for (int dem  = 1; dem < 6 && currCol - dem >= 0; dem++)
            {
                if (arrayChess[currLine, currCol - dem].Own1 == 1)
                {
                    soQuanTa++;
                }
                else if (arrayChess[currLine, currCol - dem].Own1 == 2)
                {
                    soQuanDich++;
                    break;
                }
                else
                    break;
            }
            if (soQuanDich == 2)
                return 0;
            total_score -= array_point_defensive[soQuanDich + 1] * 2;
            total_score += array_point_attack[soQuanTa];
            return total_score;
        }
        private long point_attack_reverse_browse(int currLine, int currCol)
        {
            long total_score = 0;
            int soQuanTa = 0;
            int soQuanDich = 0;
            for (int dem  = 1; dem < 6 && currCol + dem < boardChess.NumberColumn && currLine - dem >= 0; dem++)
            {
                if (arrayChess[currLine - dem, currCol + dem].Own1 == 1)
                {
                    soQuanTa++;
                }
                else if (arrayChess[currLine - dem, currCol + dem].Own1 == 2)
                {
                    soQuanDich++;
                    break;
                }
                else
                    break;
            }
            for (int dem  = 1; dem < 6 && currCol - dem >= 0 && currLine + dem < boardChess.NumberLine; dem++)
            {
                if (arrayChess[currLine + dem, currCol - dem].Own1 == 1)
                {
                    soQuanTa++;
                }
                else if (arrayChess[currLine + dem, currCol - dem].Own1 == 2)
                {
                    soQuanDich++;
                    break;
                }
                else
                    break;
            }
            if (soQuanDich == 2)
                return 0;
            total_score -= array_point_defensive[soQuanDich + 1] * 2;
            total_score += array_point_attack[soQuanTa];
            return total_score;
        }
        private long point_attack_cross_browse(int currLine, int currCol)
        {
            long total_score = 0;
            int soQuanTa = 0;
            int soQuanDich = 0;
            for (int dem  = 1; dem < 6 && currCol + dem < boardChess.NumberColumn && currLine + dem < boardChess.NumberLine; dem++)
            {
                if (arrayChess[currLine + dem, currCol + dem].Own1 == 1)
                {
                    soQuanTa++;
                }
                else if (arrayChess[currLine + dem, currCol + dem].Own1 == 2)
                {
                    soQuanDich++;
                    break;
                }
                else
                    break;
            }
            for (int dem  = 1; dem < 6 && currCol - dem >= 0 && currLine - dem >= 0; dem++)
            {
                if (arrayChess[currLine - dem, currCol - dem].Own1 == 1)
                {
                    soQuanTa++;
                }
                else if (arrayChess[currLine - dem, currCol - dem].Own1 == 2)
                {
                    soQuanDich++;
                    break;
                }
                else
                    break;
            }
            if (soQuanDich == 2)
                return 0;
            total_score -= array_point_defensive[soQuanDich + 1] * 2;
            total_score += array_point_attack[soQuanTa];
            return total_score;
        }
        #endregion

        #region phòng ngự
        private long point_defensive_verticalBrowsing(int currLine, int currCol)
        {
            long total_score = 0;
            int soQuanTa = 0;
            int soQuanDich = 0;
            for (int dem  = 1; dem < 6 && currLine + dem < boardChess.NumberLine; dem++)
            {
                if (arrayChess[currLine + dem, currCol].Own1 == 1)
                {
                    soQuanTa++;
                    break;
                }
                else if (arrayChess[currLine + dem, currCol].Own1 == 2)
                {
                    soQuanDich++;
                }
                else
                    break;
            }
            for (int dem  = 1; dem < 6 && currLine - dem >= 0; dem++)
            {
                if (arrayChess[currLine - dem, currCol].Own1 == 1)
                {
                    soQuanTa++;
                    break;
                }
                else if (arrayChess[currLine - dem, currCol].Own1 == 2)
                {
                    soQuanDich++;
                }
                else
                    break;
            }
            if (soQuanTa == 2)
                return 0;
            total_score += array_point_defensive[soQuanDich];
            return total_score;
        }
        private long point_defensive_horizontalBrowsing(int currLine, int currCol)
        {
            long total_score = 0;
            int soQuanTa = 0;
            int soQuanDich = 0;
            for (int dem  = 1; dem < 6 && currCol + dem < boardChess.NumberColumn; dem++)
            {
                if (arrayChess[currLine, currCol + dem].Own1 == 1)
                {
                    soQuanTa++;
                    break;
                }
                else if (arrayChess[currLine, currCol + dem].Own1 == 2)
                {
                    soQuanDich++;
                }
                else
                    break;
            }
            for (int dem  = 1; dem < 6 && currCol - dem >= 0; dem++)
            {
                if (arrayChess[currLine, currCol - dem].Own1 == 1)
                {
                    soQuanTa++;
                    break;
                }
                else if (arrayChess[currLine, currCol - dem].Own1 == 2)
                {
                    soQuanDich++;
                }
                else
                    break;
            }
            if (soQuanTa == 2)
                return 0;
            total_score += array_point_defensive[soQuanDich];
            return total_score;
        }
        private long point_defensive_reverse_browse(int currLine, int currCol)
        {
            long total_score = 0;
            int soQuanTa = 0;
            int soQuanDich = 0;
            for (int dem  = 1; dem < 6 && currCol + dem < boardChess.NumberColumn && currLine - dem >= 0; dem++)
            {
                if (arrayChess[currLine - dem, currCol + dem].Own1 == 1)
                {
                    soQuanTa++;
                    break;
                }
                else if (arrayChess[currLine - dem, currCol + dem].Own1 == 2)
                {
                    soQuanDich++;
                }
                else
                    break;
            }
            for (int dem  = 1; dem < 6 && currCol - dem >= 0 && currLine + dem < boardChess.NumberLine; dem++)
            {
                if (arrayChess[currLine + dem, currCol - dem].Own1 == 1)
                {
                    soQuanTa++;
                    break;
                }
                else if (arrayChess[currLine + dem, currCol - dem].Own1 == 2)
                {
                    soQuanDich++;
                }
                else
                    break;
            }
            if (soQuanTa == 2)
                return 0;
            total_score += array_point_defensive[soQuanTa];
            return total_score;
        }
        private long point_defensive_cross_browse(int currLine, int currCol)
        {
            long total_score = 0;
            int soQuanTa = 0;
            int soQuanDich = 0;
            for (int dem  = 1; dem < 6 && currCol + dem < boardChess.NumberColumn && currLine + dem < boardChess.NumberLine; dem++)
            {
                if (arrayChess[currLine + dem, currCol + dem].Own1 == 1)
                {
                    soQuanTa++;
                    break;
                }
                else if (arrayChess[currLine + dem, currCol + dem].Own1 == 2)
                {
                    soQuanDich++;
                }
                else
                    break;
            }
            for (int dem  = 1; dem < 6 && currCol - dem >= 0 && currLine - dem >= 0; dem++)
            {
                if (arrayChess[currLine - dem, currCol - dem].Own1 == 1)
                {
                    soQuanTa++;
                    break;
                }
                else if (arrayChess[currLine - dem, currCol - dem].Own1 == 2)
                {
                    soQuanDich++;
                }
                else
                    break;
            }
            if (soQuanTa == 2)
                return 0;
            total_score += array_point_defensive[soQuanTa];
            return total_score;
        }
        #endregion

        #endregion
    }
}
