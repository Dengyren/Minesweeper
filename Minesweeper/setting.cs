using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class setting : Form
    {
        public Form1 parent;
        public static bool OK = false;
        public static int MineNum;
        public static int Row;
        public static int Col;
        public setting()
        {
            InitializeComponent();
            //使用双缓冲，消除闪烁现象
            this.StartPosition = FormStartPosition.Manual; //窗体的位置由Location属性决定
            this.Width = 8 + Form1.row * 44 + 4 + 36 * 2 + 8;
            this.Height = 4 + Form1.col * 44 + 36 + 4;
            this.Location = (Point)new Size(960 - this.Width / 2 - 3, 540 - this.Height / 2); //窗体的起始位置为0,0 ,现在将其尽量设置在屏幕中间
            this.Invalidate();
            this.Icon = Icon.FromHandle(Properties.Resources.logo.GetHicon());
        }

        public setting(Form1 parent)
        {
            InitializeComponent();
            //使用双缓冲，消除闪烁现象
            this.StartPosition = FormStartPosition.Manual; //窗体的位置由Location属性决定
            this.Width = 8 + Form1.row * 44 + 4 + 36 * 2 + 8;
            this.Height = 4 + Form1.col * 44 + 36 + 4;
            this.Location = (Point)new Size(960 - this.Width / 2 - 3, 540 - this.Height / 2); //窗体的起始位置为0,0 ,现在将其尽量设置在屏幕中间
            this.Invalidate();
            this.Icon = Icon.FromHandle(Properties.Resources.logo.GetHicon());
            this.parent = parent;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                Row = int.Parse(numericUpDown1.Value.ToString());
                Col = int.Parse(numericUpDown2.Value.ToString());
                MineNum = int.Parse(numericUpDown3.Value.ToString());
                if (Row * Col <= MineNum)
                {
                    MessageBox.Show("你的雷太多了没法玩", "提示");
                    return;
                }
                else
                {
                    OK = true;
                    this.parent.Set_Form1();
                    this.Close();       
                }
            }
            else if(radioButton1.Checked)
            {
                Row = 9;
                Col = 9;
                MineNum = 10;
                OK = true;
                this.parent.Set_Form1();
                this.Close();               
            }
            else if (radioButton2.Checked)
            {
                Row = 16;
                Col = 16;
                MineNum = 40;
                OK = true;
                this.parent.Set_Form1();
                this.Close();
            }
            else if (radioButton3.Checked)
            {
                Row = 30;
                Col = 16;
                MineNum = 99;
                OK = true;
                this.parent.Set_Form1();
                this.Close();
            }
            else
            {
                MessageBox.Show("请对难度进行选择");
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = true;
            numericUpDown2.Enabled = true;
            numericUpDown3.Enabled = true;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = false;
            numericUpDown2.Enabled = false;
            numericUpDown3.Enabled = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = false;
            numericUpDown2.Enabled = false;
            numericUpDown3.Enabled = false;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = false;
            numericUpDown2.Enabled = false;
            numericUpDown3.Enabled = false;
        }            
        
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_MouseEnter_1(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.Gray;
        }

        private void pictureBox1_MouseLeave_1(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Control);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
