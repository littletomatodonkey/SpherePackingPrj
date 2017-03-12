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


        vtkVolume vol;
        vtkVolumeProperty vpro;
        vtkColorTransferFunction colorTransferFunction;



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
            //CuteTools.ShowImageSeries(fnFormat, height, width, startIndex, endIndex, renderer);
            //return;

            vtkBMPReader reader = vtkBMPReader.New();
            reader.SetFilePattern(fnFormat);

            reader.SetDataExtent(0, height - 1, 0, width - 1, startIndex, endIndex);

            reader.SetDataScalarTypeToUnsignedChar();
            reader.Update();

            vol = vtkVolume.New();

            vtkFixedPointVolumeRayCastMapper texMapper = vtkFixedPointVolumeRayCastMapper.New();

            texMapper.SetInput(reader.GetOutput());
            vol.SetMapper(texMapper);

            colorTransferFunction = vtkColorTransferFunction.New();
            colorTransferFunction.AddRGBPoint(0, tbR1.Value * 1.0 / 255, tbG1.Value * 1.0 / 255, tbB1.Value * 1.0 / 255);
            colorTransferFunction.AddRGBPoint(1, tbR2.Value * 1.0 / 255, tbG2.Value * 1.0 / 255, tbB2.Value * 1.0 / 255);
            colorTransferFunction.ClampingOn();

            vpro = vtkVolumeProperty.New();
            vtkPiecewiseFunction compositeOpacity = vtkPiecewiseFunction.New();
            compositeOpacity.AddPoint(0, 0);
            //compositeOpacity.AddPoint(120.0, 0.5);
            compositeOpacity.AddPoint(255.0, 1);
            compositeOpacity.ClampingOn();
            vpro.SetScalarOpacity(compositeOpacity);
            vpro.SetColor(colorTransferFunction);
            //vpro.SetInterpolationTypeToLinear();
            vpro.SetInterpolationTypeToNearest();
            //vpro.ShadeOn();
            vol.SetProperty(vpro);


            //画轴距图
            vol.SetOrientation(45, 45, 0);


            //rwcShowSlices.RenderWindow.GetRenderers().GetFirstRenderer().AddVolume( vol );

            rwcShowSlices.Refresh();


        }

        private void btnSaveRenderer_Click(object sender, EventArgs e)
        {
            string fn = String.Format("{0}screenshot{1}.png", PackingSystemSetting.ResultDir, DateTime.Now.ToString("yyyyMMhh-HHmmss"));
            CuteTools.SaveRendererWindowsAsPic(fn, ref rwcShowSlices);
        }

        private void tbRGB_Scroll(object sender, EventArgs e)
        {
            colorTransferFunction = vtkColorTransferFunction.New();
            colorTransferFunction.AddRGBPoint(0, tbR1.Value * 1.0 / 255, tbG1.Value * 1.0 / 255, tbB1.Value * 1.0 / 255);
            colorTransferFunction.AddRGBPoint(1, tbR2.Value * 1.0 / 255, tbG2.Value * 1.0 / 255, tbB2.Value * 1.0 / 255);
            colorTransferFunction.ClampingOn();

            vpro.SetColor(colorTransferFunction);

            //rwcShowSlices.Update();
            rwcShowSlices.Refresh();
        }

        private void btnSaveVtkFile_Click(object sender, EventArgs e)
        {
            //vtkXMLDataSetWriter writer = new vtkXMLDataSetWriter();
            //writer.SetFileName("testo.vti");

            


            

        }


        
    }
}
