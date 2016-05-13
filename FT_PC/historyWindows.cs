using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace FT_PC
{
    public partial class historyWindows : UserControl
    {
        private Color lineColor = Color.FromArgb(240, 244, 249);
        private Color focusedColor = Color.FromArgb(102, 206, 255);
        public historyWindows()
        {
            InitializeComponent();
        }

        List<listBoxItem> list = new List<listBoxItem>();
        private void historyWindows_Load(object sender, EventArgs e)
        {
            lb_historyFiles.ItemHeight = 30;
            lb_historyFiles.DrawItem += new System.Windows.Forms.DrawItemEventHandler(listBox1_DrawItem);
            //lb_historyFiles.Items.Add(new listBoxItem("111", "2222"));

            listBoxItem item;
            for (int i = 0; i <= 20; i++)
            {
                item = new listBoxItem(null, i.ToString(), (i * 8).ToString());
                list.Add(item);
                lb_historyFiles.Items.Add(item);
            }
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                using (SolidBrush brush = new SolidBrush(focusedColor))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }
            }
            else
            {
                if (e.Index % 2 == 0)
                {
                    using (SolidBrush brush = new SolidBrush(lineColor))
                    {
                        e.Graphics.FillRectangle(brush, e.Bounds);
                    }
                }
            }

            Brush myBrush = Brushes.Black;  //初始化字体颜色=黑色
            StringFormat strFmt = new System.Drawing.StringFormat();
            strFmt.LineAlignment = StringAlignment.Center; //文本垂直居中
            strFmt.Trimming = StringTrimming.EllipsisCharacter;//超出矩形范围的部分用省略号表示

            if (list[e.Index].fileImg != null) //在项右侧 画图标  
            {
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;

                e.Graphics.DrawImage(list[e.Index].fileImg, (e.Bounds.Right+2), e.Bounds.Top+2, 20, 26);
            }

            SizeF size = e.Graphics.MeasureString(list[e.Index].timeToReceived, e.Font); //获取项文本尺寸
            RectangleF rectF = new RectangleF((e.Bounds.Right - size.Width - 6), e.Bounds.Top, (size.Width+6), (e.Bounds.Height));
            e.Graphics.DrawString(list[e.Index].timeToReceived, e.Font, myBrush, rectF,strFmt);

            RectangleF rectFExpImg = new RectangleF((e.Bounds.Left + 26 + 20), e.Bounds.Top, (e.Bounds.Width - 46 - size.Width + 9), e.Bounds.Height);
            e.Graphics.DrawString(list[e.Index].fileNmae, e.Font, myBrush, rectFExpImg, strFmt);
        }

    }
}
