using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace caro
{
    public partial class Form1 : Form
    {
        private caroChess caro;
        private Graphics grs;

        public Form1()
        {
            InitializeComponent();
            caro = new caroChess();
            caro.createArrayChess();
            grs = pnlChess.CreateGraphics();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            lblText.Text = "Chào mừng đến với trò chơi cờ caro!!! \nNgười chơi sẽ chiến thắng khi có 5 ô \ncờ nằm trên 1 hàng hoặc chéo nhau \nvà không bị chặn ở hai đầu";
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblText.Location = new Point(lblText.Location.X, lblText.Location.Y - 2);
            if (lblText.Location.Y  + lblText.Height < 0)
            {
                lblText.Location = new Point(lblText.Location.X, grRule.Height);
            }
        }

        private void pnlChess_Paint(object sender, PaintEventArgs e)
        {
            caro.drawChess(grs);
            caro.redrawChess(grs);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void pnlChess_MouseClick(object sender, MouseEventArgs e)
        {
            if (!caro.Ready)
                return;
            if (caro.playChess(e.Y, e.X, grs))
            {
                if (caro.checkVictory())
                    caro.endPlayChess();
                else
                {
                    if (caro.GameMode == 2)
                    {
                        caro.startAI(grs);
                        if (caro.checkVictory())
                            caro.endPlayChess();
                    }
                }
            }
        }
        private void playerVsPlayerToolStripMenuItem_Click(object sender, EventArgs e)  
        {
            if(MessageBox.Show("Bạn đã sẵn sàn?", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                grs.Clear(pnlChess.BackColor);
                caro.startPlayervsPlayer(grs);
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            caro.undo(grs);
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            caro.redo(grs);
        }

        private void btnPlayervsCom_Click(object sender, EventArgs e)
        {
            grs.Clear(pnlChess.BackColor);
            caro.startPlayervsCom(grs);
        }
    }
}
