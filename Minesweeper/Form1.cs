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
    public partial class Form1 : Form
    {
        MouseEventArgs _e;
        private bool win;//判断是否胜利
        private bool start = false;
        private bool lose;//判断是否已经踩到地雷
        public int mineNumber;//地雷数量
        public int _mineNumber;//地雷数量
        private int GameSize_X, GameSize_Y;//游戏大小
        public static float GameTime;//游戏用时
        private bool[,] mines;//雷的坐标
        private int[,] numbers;//假如一个格子没有雷，用它来存储8个方向上雷的个数
        public bool[,] open;//打开的坐标 
        public bool[,] flags;//棋子坐标
        private Point boardLocation = new Point(8, 45);//block初始位置
        SoundPlayer Leftclick = new SoundPlayer(Properties.Resources.left);//设置鼠标左键音乐
        SoundPlayer Rightclick = new SoundPlayer(Properties.Resources.right);//设置鼠标右键音乐
        SoundPlayer bomb = new SoundPlayer(Properties.Resources.bomb);//设置踩雷的音效
        SoundPlayer Win = new SoundPlayer(Properties.Resources.winsound);//设置赢了的音效
        public static int row = 9;
        public static int col = 9;
        public static int minenum = 10;

        /*    扫雷游戏的算法     */
        public bool isArea(int x, int y)//判断是否在游戏的有效范围内
        {
            if (x >= 0 && x < GameSize_X && y >= 0 && y < GameSize_Y)
                return true;
            else
                return false;
        }

        public bool isMine(int x, int y)//判断是否是雷
        {
            if (!isArea(x, y))
                throw new Exception("不在游戏的有效范围内");
            return mines[x, y];
        }

        public int getNumber(int x, int y)//判断地雷附近的数字
        {
            if (!isArea(x, y))
                throw new Exception("不在游戏的有效范围内");
            return numbers[x, y];
        }

        public void Blockopen(int x, int y)//判断block是否被打开
        {
            if (isArea(x, y))
            {
                if (!flags[x, y])
                    open[x, y] = true;
                if (getNumber(x, y) == 0) //floodfill算法，深度遍历周围的环境
                {
                    for (int i = x - 1; i <= x + 1; i++)
                        for (int j = y - 1; j <= y + 1; j++)
                            if (isArea(i, j) && !open[i, j] && !isMine(i, j) && !flags[i, j]) 
                                Blockopen(i, j);
                }
            }
        }

        private void generateMines(int mineNumber)//Fisher-Yates洗牌算法随机生成雷区
        {
            Random rd = new Random();
            mines = new bool[GameSize_X, GameSize_Y];
            numbers = new int[GameSize_X, GameSize_Y];
            open = new bool[GameSize_X, GameSize_Y];
            flags = new bool[GameSize_X, GameSize_Y];
            int x = 0, y = 0;

            for (int i = 0; i < GameSize_X; i++)
                for (int j = 0; j < GameSize_Y; j++)
                {
                    mines[i, j] = false;//出于安全性考虑，其实必要性不强，因为初始状态下就为false
                    numbers[i, j] = 0;
                    open[i, j] = false;
                    flags[i, j] = false;
                }

            for (int i = 0; i < mineNumber; i++)
            {
                x = i / GameSize_Y;
                y = i % GameSize_Y;
                mines[x, y] = true;
            }

            for (int i = GameSize_X * GameSize_Y - 1; i >= 0; i--)
            {
                int iX = i / GameSize_Y;
                int iY = i % GameSize_Y;
                int randNumber = (rd.Next(0, i + 1));//范围在0--i
                int randX = randNumber / GameSize_Y;
                int randY = randNumber % GameSize_Y;
                swap(iX, iY, randX, randY);
            }
        }

        private void swap(int x1, int y1, int x2, int y2)//交换两个坐标
        {
            bool t = mines[x1, y1];
            mines[x1, y1] = mines[x2, y2];
            mines[x2, y2] = t;
        }

        private void calculateNumber()//用于判断格子8个方向上雷的数量
        {
            for (int i = 0; i < GameSize_X; i++)
            {
                for (int j = 0; j < GameSize_Y; j++)
                {

                    if (mines[i, j])
                        numbers[i, j] = -1;
                    else
                        numbers[i, j] = 0;
                    for (int n = i - 1; n <= i + 1; n++)
                        for (int m = j - 1; m <= j + 1; m++)
                            if (isArea(n, m) && isMine(n, m))
                                numbers[i, j]++;
                }
            }
            return;
        }

        private void isWin()//判断是否胜利
        {
            int num = 0;
            if (mineNumber == 0)//判断游戏是否结束
            {
                for (int i = 0; i < GameSize_X; i++)
                {
                    for (int j = 0; j < GameSize_Y; j++)
                    {
                        if (mines[i, j] == true && flags[i, j] == true)
                            num++;
                    }
                }
            }
            if (num == _mineNumber && start == true)
            {
                win = true;
                timer1.Enabled = false;
                pictureBox3.Image = Properties.Resources.win;
                Win.Play();
                record r = new record();
                r.ShowDialog();
            }
        }

        private void isLose()
        {
            if(lose)
            {
                timer1.Enabled = false;
                pictureBox3.Image = Properties.Resources.lose;
                bomb.Play();
            }
        }

        public Form1()
        {
            InitializeComponent();
            //使用双缓冲，消除闪烁现象
            this.StartPosition = FormStartPosition.Manual; //窗体的位置由Location属性决定
            this.Location = (Point)new Size(716, 322); //窗体的起始位置为0,0 ,现在将其尽量设置在屏幕中间
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);//禁止擦除背景
            SetStyle(ControlStyles.DoubleBuffer, true);//双缓冲
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Leftclick.Play();
            Form setform = new setting(this);
            setform.ShowDialog();
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.Gray;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Control);
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            pictureBox2.BackColor = Color.Gray;
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            pictureBox2.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Control);
        }

        private void panel1_MouseEnter(object sender, EventArgs e)
        {
            panel1.BackColor = Color.Gray;
        }

        private void panel1_MouseLeave(object sender, EventArgs e)
        {
            panel1.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.ActiveCaption);
        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            Leftclick.Play();
            Application.Exit();
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if(setting.OK)
            {
                col = setting.Col;
                row = setting.Row;
                minenum = setting.MineNum;
                setting.OK = false;
            }
            Leftclick.Play();
            _e = e;
            pictureBox3.Image = Properties.Resources.start;
            win = lose = false;
            start = true;
            for (int i = 0; i < GameSize_X; i++)
                for (int j = 0; j < GameSize_Y; j++)
                {
                    mines[i, j] = false;//出于安全性考虑，其实必要性不强，因为初始状态下就为false
                    numbers[i, j] = 0;
                    open[i, j] = false;
                    flags[i, j] = false;
                }
            GameSize_X = row;
            GameSize_Y = col;
            mineNumber = _mineNumber = minenum;
            this.Width = 8 + row * 44 + 4 + 36 * 2 + 8;
            this.Height = 4 + col * 44 + 36 + 4;
            this.Location = (Point)new Size(960 - this.Width / 2, 540 - this.Height / 2); //窗体的起始位置为0,0 ,现在将其尽量设置在屏幕中间
            label1.Location = new Point(this.Width - 72, this.Height * 7 / 25);
            label3.Location = new Point(this.Width - 72, this.Height * 13 / 20);
            label2.Location = new Point(this.Width - 63, this.Height * 7 / 25 + 34);
            label4.Location = new Point(this.Width - 63, this.Height * 13 / 20 + 34);
            panel1.Width = row * 44 - 8;
            panel1.Height = 36;
            pictureBox1.Location = new Point(8 + row * 44 , 4);
            pictureBox2.Location = new Point(8 + row * 44 + 44, 4);
            pictureBox3.Location = new Point(panel1.Width / 2 - 18, 0);
            generateMines(mineNumber);
            label1.Text = mineNumber.ToString() + "/" + _mineNumber.ToString();
            label3.Text = "0.00";
            calculateNumber();
            timer1.Enabled = false;
            GameTime = 0f;
            this.Invalidate();
        }

        private void pictureBox3_MouseEnter(object sender, EventArgs e)
        {
            panel1.BackColor = Color.Gray;
        }

        private void pictureBox3_MouseLeave(object sender, EventArgs e)
        {
            panel1.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.ActiveCaption);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            panel1_MouseClick(sender, _e);
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (!lose && !win) 
            {
                int x = 0, y = 0;
                x = int.Parse(((e.X - boardLocation.X) / (36 + 8)).ToString());
                y = int.Parse(((e.Y - boardLocation.Y) / (36 + 8)).ToString());
                if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right) //左右键
                {
                    if (e.X >= boardLocation.X && e.X <= boardLocation.X + (GameSize_X) * 36
                        && e.Y >= boardLocation.Y && e.Y <= boardLocation.Y + (GameSize_Y) * 36)
                    {
                        timer1.Enabled = true;
                        timer1.Interval = 100;
                    }
                }
                if (e.Button == MouseButtons.Left)
                {
                    Leftclick.Play();
                    if (isArea(x, y))
                    {
                        if (isMine(x, y) && !flags[x, y])
                        {
                            //Game Over
                            open[x, y] = true;
                            if (!lose)
                                this.Invalidate();
                            lose = true;
                        }

                        else if (flags[x, y])
                        {
                            flags[x, y] = false;
                            mineNumber++;
                        }

                        else
                            Blockopen(x, y);
                        label1.Text = mineNumber.ToString() + "/" + _mineNumber.ToString();
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    Rightclick.Play();
                    if (isArea(x, y))
                        flags[x, y] = !flags[x, y];
                    if (isArea(x, y))
                        if (!open[x, y])
                            if (flags[x, y]) mineNumber--;
                            else mineNumber++;
                    label1.Text = mineNumber.ToString() + "/" + _mineNumber.ToString();
                }
                if (!lose && !win)
                    this.Invalidate();
            }
            label1.Text = mineNumber.ToString() + "/" + _mineNumber.ToString();
        }

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!lose && !win && e.Button == MouseButtons.Left) 
            {
                Leftclick.Play();
                int x = 0, y = 0;
                x = int.Parse(((e.X - boardLocation.X) / (36 + 8)).ToString());
                y = int.Parse(((e.Y - boardLocation.Y) / (36 + 8)).ToString());
                for (int i = x - 1; i <= x + 1; i++)
                    for (int j = y - 1; j <= y + 1; j++)
                    {
                        if (isArea(i, j))
                        {
                            if (!flags[i, j])
                            {
                                open[i, j] = true;
                                if (isMine(i, j))
                                    lose = true;
                            }
                            if (getNumber(i, j) == 0 && !flags[i, j]) 
                                Blockopen(i, j);
                        }
                    }
                this.Invalidate();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            float x, y;
            for (int i = 0; i <= GameSize_X; i++)
            {
                if (i < GameSize_X)
                {
                    for (int j = 0; j < GameSize_Y; j++)
                    {
                        x = boardLocation.X + i * 36F + i * 8;
                        y = boardLocation.Y + j * 36F + j * 8;
                        if (open[i, j])
                            if (isMine(i, j))
                            e.Graphics.DrawImage(Properties.Resources.mine, x, y, 36, 36);
                        else
                        {
                            int a = getNumber(i, j);
                            switch (a)
                            {
                                case 1:
                                    e.Graphics.DrawImage(Properties.Resources._1, x, y, 36, 36);
                                    break;
                                case 2:
                                    e.Graphics.DrawImage(Properties.Resources._2, x, y, 36, 36);
                                    break;
                                case 3:
                                    e.Graphics.DrawImage(Properties.Resources._3, x, y, 36, 36);
                                    break;
                                case 4:
                                    e.Graphics.DrawImage(Properties.Resources._4, x, y, 36, 36);
                                    break;
                                case 5:
                                    e.Graphics.DrawImage(Properties.Resources._5, x, y, 36, 36);
                                    break;
                                case 6:
                                    e.Graphics.DrawImage(Properties.Resources._6, x, y, 36, 36);
                                    break;
                                case 7:
                                    e.Graphics.DrawImage(Properties.Resources._7, x, y, 36, 36);
                                    break;
                                case 8:
                                    e.Graphics.DrawImage(Properties.Resources._8, x, y, 36, 36);
                                    break;
                                default:
                                    e.Graphics.DrawImage(Properties.Resources._0, x, y, 36, 36);
                                    break;
                            }
                        }
                        else
                        {
                            if (flags[i, j])
                                e.Graphics.DrawImage(Properties.Resources.flag, x, y, 36, 36);
                            else
                                e.Graphics.DrawImage(Properties.Resources.block, x, y, 36, 36);
                        }
                    }
                }
            }
            isWin();
            isLose();
        }

        public void Set_Form1()
        {
            if (setting.OK)
            {
                col = setting.Col;
                row = setting.Row;
                minenum = setting.MineNum;
                setting.OK = false;
            }
            Leftclick.Play();
            pictureBox3.Image = Properties.Resources.start;
            win = lose = false;
            start = true;
            for (int i = 0; i < GameSize_X; i++)
                for (int j = 0; j < GameSize_Y; j++)
                {
                    mines[i, j] = false;//出于安全性考虑，其实必要性不强，因为初始状态下就为false
                    numbers[i, j] = 0;
                    open[i, j] = false;
                    flags[i, j] = false;
                }
            GameSize_X = row;
            GameSize_Y = col;
            mineNumber = _mineNumber = minenum;
            this.Width = 8 + row * 44 + 4 + 36 * 2 + 8;
            this.Height = 4 + col * 44 + 36 + 4;
            this.Location = (Point)new Size(960 - this.Width / 2, 540 - this.Height / 2); //窗体的起始位置为0,0 ,现在将其尽量设置在屏幕中间
            label1.Location = new Point(this.Width - 72, this.Height * 7 / 25);
            label3.Location = new Point(this.Width - 72, this.Height * 13 / 20);
            label2.Location = new Point(this.Width - 63, this.Height * 7 / 25 + 34);
            label4.Location = new Point(this.Width - 63, this.Height * 13 / 20 + 34);
            panel1.Width = row * 44 - 8;
            panel1.Height = 36;
            pictureBox1.Location = new Point(8 + row * 44, 4);
            pictureBox2.Location = new Point(8 + row * 44 + 44, 4);
            pictureBox3.Location = new Point(panel1.Width / 2 - 18, 0);
            generateMines(mineNumber);
            label1.Text = mineNumber.ToString() + "/" + _mineNumber.ToString();
            label3.Text = "0.00";
            calculateNumber();
            timer1.Enabled = false;
            GameTime = 0f;
            this.Invalidate();
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            GameTime += 0.1f;
            label3.Text = GameTime.ToString(("#0.00 "));
        }

    }
}