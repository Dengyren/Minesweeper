using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class record : Form
    {
        SoundPlayer Leftclick = new SoundPlayer(Properties.Resources.left);//设置鼠标左键音乐
        public record()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Leftclick.Play();
            this.Close();
        }

        private void record_Load(object sender, EventArgs e)
        {
            this.Location = (Point)new Size(760, 340); //窗体的起始位置为0,0 ,现在将其尽量设置在屏幕中间
            label7.Text=Form1.row.ToString()+"行";
            label8.Text = Form1.col.ToString() + "列";
            label9.Text = Form1.minenum.ToString() + "个";
            label10.Text = Form1.GameTime.ToString("#0.00 ") + "秒";
            label11.Text= System.DateTime.Now.ToString();
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
               pictureBox1.BackColor = Color.Gray;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Control);
        }
    }
}
