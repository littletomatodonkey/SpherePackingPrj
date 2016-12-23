using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kitware.VTK;

namespace SpherePacking.MainWindow
{
    /// <summary>
    /// 可视化切片图像序列的窗口类
    /// </summary>
    public partial class VisiualizeSlicesWnd : Form
    {
        /// <summary>
        /// 切片图像的宽度
        /// </summary>
        private int width;

        /// <summary>
        /// 切片图像的高度
        /// </summary>
        private int height;

        /// <summary>
        /// 开始序号
        /// </summary>
        private int startIndex;

        /// <summary>
        /// 结束序号
        /// 注：endIndex的切片图像也会被显示，所以要求这个序号对应的图像存在
        /// </summary>
        private int endIndex;

        /// <summary>
        /// 切片序列的格式
        /// 注 ： 要求切片图像是bmp格式的
        ///     example : @"initial/%03d.bmp"
        /// </summary>
        private string fnFormat;

        public VisiualizeSlicesWnd(int h, int w, int si, int ei, string format)
        {
            InitializeComponent();
            width = w;
            height = h;
            startIndex = si;
            endIndex = ei;
            fnFormat = format;
        }

        /// <summary>
        /// 进行可视化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VisiualizeSlicesWnd_Load(object sender, EventArgs e)
        {
            vtkRenderer renderer = rwcShowSlices.RenderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(1, 1, 1);
            CuteTools.ShowImageSeries(fnFormat, height, width, startIndex, endIndex, renderer);
            rwcShowSlices.Refresh();
        }

        /// <summary>
        /// 窗口大小改变时，重新改变控件的大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VisiualizeSlicesWnd_SizeChanged(object sender, EventArgs e)
        {
            int margin = 10;
            int x = margin;
            int y = margin;
            int width = this.Width - 3*margin;
            int height = this.Height - 6*margin;
            if( width > 0 && height > 0 )
            {
                rwcShowSlices.Bounds = new Rectangle(  x, y, width, height  );
            }
            
        }
    }
}
