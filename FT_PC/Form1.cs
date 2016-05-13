using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FT_PC
{
    public partial class mainForm : Form
    {
        private Color defaultColor = Color.FromArgb(15, 121, 255);
        private Color mouseOnColor = Color.FromArgb(3, 100, 200);
        public connectWindows cWindow;
        public historyWindows hWindow;
        public mainForm()
        {
            InitializeComponent();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.None;
            cWindow = new connectWindows();
            hWindow = new historyWindows();
            initUI();
        }


        public void initUI()
        {
            cWindow.Show();
            gbWindows.Controls.Clear();
            gbWindows.Controls.Add(cWindow);
        }

        private void exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_connetWindow_Click(object sender, EventArgs e)
        {
            cWindow.Show();
            gbWindows.Controls.Clear();
            gbWindows.Controls.Add(cWindow);
        }

        private void btn_historyWindow_Click(object sender, EventArgs e)
        {
            hWindow.Show();
            gbWindows.Controls.Clear();
            gbWindows.Controls.Add(hWindow);
        }

        private void minSize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void exit_MouseEnter(object sender, EventArgs e)
        {
            exit.BackColor = mouseOnColor;
        }

        private void minSize_MouseEnter(object sender, EventArgs e)
        {
            minSize.BackColor = mouseOnColor;
        }

        private void exit_MouseLeave(object sender, EventArgs e)
        {
            exit.BackColor = defaultColor;
        }

        private void minSize_MouseLeave(object sender, EventArgs e)
        {
            minSize.BackColor = defaultColor;
        }

        #region 设置窗体的拖动
        Point mouseOff;//鼠标移动位置变量
        bool leftFlag;//标签是否为左键
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOff = new Point(-e.X, -e.Y); //得到变量的值
                leftFlag = true;                  //点击左键按下时标注为true;
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                Point mouseSet = Control.MousePosition;
                mouseSet.Offset(mouseOff.X, mouseOff.Y);  //设置移动后的位置
                Location = mouseSet;
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                leftFlag = false;//释放鼠标后标注为false;
            }
        }
        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOff = new Point(-e.X, -e.Y); //得到变量的值
                leftFlag = true;                  //点击左键按下时标注为true;
            }
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                Point mouseSet = Control.MousePosition;
                mouseSet.Offset(mouseOff.X, mouseOff.Y);  //设置移动后的位置
                Location = mouseSet;
            }
        }

        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                leftFlag = false;//释放鼠标后标注为false;
            }
        }
        #endregion
    }
}
