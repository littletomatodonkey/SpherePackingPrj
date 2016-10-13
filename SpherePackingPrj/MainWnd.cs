using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kitware.VTK;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Threading;
using System.IO;
using System.Collections;
using Newtonsoft.Json;
using System.Diagnostics;

namespace SpherePacking.MainWindow
{
    public partial class MainWnd : Form
    {

        //Here is the once-per-class call to initialize the log object
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 每个边上小球的个数
        /// 相当于立方体容器的边长
        /// </summary>
        private const int N_BASE = 10;

        /// <summary>
        /// Z方向上的小球的个数与x、y方向上的小球的个数的比
        /// </summary>
        private const int Z_RATE = 2;

        /// <summary>
        /// 在区域内放置的小球的个数
        /// </summary>
        private const int NUM_SPHERE = N_BASE * N_BASE * N_BASE;

        /// <summary>
        /// 窗体中的renderer
        /// </summary>
        vtkRenderer renderer;

        /// <summary>
        /// 容器表面绘制模型
        /// </summary>
        private CubeSurfacePlot cubeSurfacePlot;

        /// <summary>
        /// 小球模型
        /// </summary>
        private SpherePlot balls;

        /// <summary>
        /// 3D的DEM模型
        /// </summary>
        private ModelDem3D modelDem3D;

        /// <summary>
        /// 开启一个线程用于实时显示求解的结果，防止界面卡死
        /// </summary>
        private Thread solveThread;

        private Stopwatch stopWatch = new Stopwatch();

        #region resize窗口时用到的变量
        /// <summary>
        /// 用以存储窗体中所有的控件名称
        /// </summary>
        private ArrayList initialCrlName = new ArrayList();

        /// <summary>
        /// 窗体中所有的控件原始位置的X值
        /// </summary>
        private ArrayList ctrlLocationX = new ArrayList();
        private ArrayList ctrlLocationY = new ArrayList();//用以存储窗体中所有的控件原始位置
        private ArrayList ctrlSizeWidth = new ArrayList();//用以存储窗体中所有的控件原始的水平尺寸
        private ArrayList ctrlSizeHeight = new ArrayList();//用以存储窗体中所有的控件原始的垂直尺寸
        private int initFormSizeWidth;//用以存储窗体原始的水平尺寸
        private int initFormSizeHeight;//用以存储窗体原始的垂直尺寸

        //private double FormSizeChangedX;//用以存储相关父窗体/容器的水平变化量
        //private double FormSizeChangedY;//用以存储相关父窗体/容器的垂直变化量 

        private int gResizeCnt = 0;//为防止递归遍历控件时产生混乱，故专门设定一个全局计数器
        #endregion

        public MainWnd()
        {
            InitializeComponent();

            #region 初始化resize相关的变量
            GetInitialFormSize();

            GetAllCtrlLocation(this);
            GetAllCtrlSize(this);
            #endregion

            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            renderer = vtkRenderer.New();
            renderer = rwcVolumeDisp.RenderWindow.GetRenderers().GetFirstRenderer(); //将此renderer和显示的窗体控件相关联
            renderer.SetBackground(0.7, 0.7, 0.7); //设置背景

            cbBoundType.SelectedIndex = 0;

            InitToolTipSetting();
            switch (PackingSystemSetting.SystemBoundType)
            {
                case BoundType.CubeType:
                    InitStatusForCube();
                    break;
                case BoundType.CylinderType:
                    InitStatusForCylinder();
                    break;
                default:
                    break;
            }

            modelDem3D = new ModelDem3D(N_BASE, balls);
            modelDem3D.RefreshHandler = new ModelDem3D.RefreshWindowHandler(RefreshWindow);

            AddLightToRenderer(renderer);
            SetCameraToRenderer(renderer);

            rwcVolumeDisp.Refresh();

            GenerateDirsIfNotExist();

        }

        /// <summary>
        /// 生成存储相关信息的文件夹
        /// </summary>
        private void GenerateDirsIfNotExist()
        {
            if( !Directory.Exists(PackingSystemSetting.ResultDir) )
            {
                Directory.CreateDirectory(PackingSystemSetting.ResultDir);
            }

            if (!Directory.Exists(PackingSystemSetting.LogDir))
            {
                Directory.CreateDirectory(PackingSystemSetting.LogDir);
            }

            if (!Directory.Exists(PackingSystemSetting.LibDir))
            {
                Directory.CreateDirectory(PackingSystemSetting.LibDir);
            }
        }

        /// <summary>
        /// 初始化tooltip
        /// </summary>
        private void InitToolTipSetting()
        {
            ttShowInfo.SetToolTip(this.btnLoadSpheresJson, "从json文件中导入小球的半径和位置信息");
            ttShowInfo.SetToolTip(this.btnSaveSpheres, "将小球信息保存到文件中");
            ttShowInfo.SetToolTip(this.btnSaveImg, "保存当前rendererwindow到图像中");
            ttShowInfo.SetToolTip(this.btnSinter, "模拟烧结过程，将每个小圆的半径设增大为原来的1.2倍");
            ttShowInfo.SetToolTip(this.btnSolveProblem, "求解问题");
            ttShowInfo.SetToolTip(this.btnTest, "测试按钮，用于测试函数功能");
            ttShowInfo.SetToolTip(this.btnGenerateSlices, "将当前容器中的小球信息生成切片(图像序列)");
        }

        /// <summary>
        /// 定时测试，可以看出直接修改spheres的属性再刷新即可
        /// 当时是设置管线，即inputconnection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerUpdate_Tick(object sender, EventArgs e)
        {


        }

        /// <summary>
        /// 设置renderer中的light
        /// </summary>
        /// <param name="renderer"></param>
        private void AddLightToRenderer( vtkRenderer renderer )
        {
            vtkLight light1 = vtkLight.New();
            light1.SetColor(0, 1, 0);
            light1.SetPosition(-N_BASE, -N_BASE, -N_BASE);
            light1.SetFocalPoint(renderer.GetActiveCamera().GetFocalPoint()[0],
                                renderer.GetActiveCamera().GetFocalPoint()[1],
                                renderer.GetActiveCamera().GetFocalPoint()[2]);

            vtkLight light2 = vtkLight.New();
            light2.SetColor(0, 0, 1);
            light2.SetPosition(N_BASE*2, N_BASE*2, N_BASE*2);
            light2.SetFocalPoint(renderer.GetActiveCamera().GetFocalPoint()[0],
                                renderer.GetActiveCamera().GetFocalPoint()[1],
                                renderer.GetActiveCamera().GetFocalPoint()[2]);
            renderer.AddLight(light2);
            renderer.AddLight(light1);
        }

        /// <summary>
        /// 设置renderer中的camera
        /// 除了位置以外，参数都是按照网上的参数设置的，之后可以修改和优化
        /// </summary>
        /// <param name="renderer"></param>
        private void SetCameraToRenderer(vtkRenderer renderer)
        {
            vtkCamera camera = vtkCamera.New();
            camera.SetClippingRange(0.0475, 2.3786);
            camera.SetPosition(-N_BASE, -N_BASE, N_BASE * 3);
            //camera.ComputeViewPlaneNormal();
            //camera.SetViewUp(10, 10, 0);
            renderer.SetActiveCamera( camera );
        }

        /// <summary>
        /// 向renderer中添加坐标轴
        /// </summary>
        /// <param name="renderer"></param>
        private void AddCoordianteToRenderer(vtkRenderer renderer )
        {
            vtkCubeAxesActor axes = vtkCubeAxesActor.New();
            axes.SetCamera( renderer.GetActiveCamera() );
            axes.SetBounds(0,N_BASE,0,N_BASE,0,N_BASE);
            axes.SetOrigin(0,0,0);
            
            renderer.AddActor( axes );
        }

        /// <summary>
        /// 刷新rendererwindow
        /// </summary>
        private void RefreshWindow(int iter, Matrix<double> pos)
        {
            log.Info("current iteration : " + iter);
            //Console.WriteLine("current iteration : " + iter);
            for (int i = 0; i < balls.Spheres.Count();i++ )
            {
                //Console.WriteLine("%dth sphere pos: (" + pos[i, 0] + ", " + +pos[i, 1] + ", " + pos[i, 2] + ")");
                balls.SetCenter(pos[i,0], pos[i,1], pos[i,2], i);
            }

            if( (iter+1) % 100 == 0 )
            {
                DataReadWriteHelper.SaveModelDemAsSimple(modelDem3D, String.Format("./result/ballsInfo{0}_{1}.json", PackingSystemSetting.SystemBoundType, DateTime.Now.ToString("yyyyMMdd-HHmmss")));
            }

            try
            {
                this.Invoke((EventHandler)(delegate
                {
                    //rwcVolumeDisp.Refresh();
                    tbStatus.Text = "iteration : " + iter;
                    //SaveRendererAsPic(String.Format("./result/iter{0:D5}.png", iter));

                }));
            }
            catch(Exception ex)
            {
                log.Error(ex);
                Console.WriteLine("Thread has been aborted at iteration : " + iter);
                //Console.WriteLine(ex.ToString());
            }
            
        }

        /// <summary>
        /// 开始或者停止求解DEM问题
        /// 此处开启线程，防止主界面在求解过程中被卡死
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSolveProblem_Click(object sender, EventArgs e)
        {
            //有时不是Running，而是WaitSleepJoin，需要查一下
            if ((solveThread != null) && (solveThread.ThreadState == System.Threading.ThreadState.Running || solveThread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                btnSolveProblem.Text = "求解问题";
                solveThread.Abort();
                    
            }
            else
            {
                //在异步线程中启动程序，可以防止解算时界面卡死的情况
                ThreadStart ts = new ThreadStart(delegate { modelDem3D.SolveProblem(); });
                solveThread = new Thread(ts);
                solveThread.Start();
                btnSolveProblem.Text = "停止求解";
            }

            Console.WriteLine(solveThread.ThreadState);
            
        }

        /// <summary>
        /// 关闭界面前，释放可能在运行的线程，防止发生异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWnd_FormClosing(object sender, FormClosingEventArgs e)
        {
            if( solveThread != null)
            {
                Console.WriteLine("in : " + solveThread.ThreadState);
                if( ( solveThread.ThreadState == System.Threading.ThreadState.WaitSleepJoin
                                       ||solveThread.ThreadState == System.Threading.ThreadState.Running
                                       ||solveThread.ThreadState == System.Threading.ThreadState.Suspended) )
                {

                    solveThread.Abort();
                    solveThread.Join();
                    Console.WriteLine(solveThread.ThreadState);
                }
            }
        }

        /// <summary>
        /// 以png形式存储当前窗口的状态
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        private bool SaveRendererAsPic(string fn)
        {
            bool res = true;

            vtkWindowToImageFilter screenShot = vtkWindowToImageFilter.New();
            screenShot.SetInput( rwcVolumeDisp.RenderWindow );
            //screenShot.SetMagnification(3);
            screenShot.SetInputBufferTypeToRGB();
            screenShot.ReadFrontBufferOff();
            screenShot.Update();

            vtkPNGWriter writer = vtkPNGWriter.New();
            writer.SetFileName( fn );
            writer.SetInputConnection( screenShot.GetOutputPort() );
            writer.Write();

            return res;
        }



        /// <summary>
        /// 初始化边界为立方体的情况
        /// </summary>
        private void InitStatusForCube()
        {
            balls = new SpherePlot(N_BASE, Z_RATE);
            balls.PlotSphereInRender(renderer);

            CubeEdgePlot edgePlot = new CubeEdgePlot(N_BASE, 2 * N_BASE);
            edgePlot.PlotEdge(renderer, new byte[] { 255, 0, 0 });

            cubeSurfacePlot = new CubeSurfacePlot(N_BASE);
            //cubeSurfacePlot.PlotSurfaceAll(renderer);

            AddCoordianteToRenderer(renderer);
        }

        /// <summary>
        /// 初始化边界为圆柱体的情况
        /// </summary>
        private void InitStatusForCylinder()
        {
            balls = new SpherePlot(PackingSystemSetting.Radius, PackingSystemSetting.Height);
            balls.PlotSphereInRender( renderer );

            //标准参数应该是100，40
            CylinderEdgePlot plot = new CylinderEdgePlot( PackingSystemSetting.Radius, PackingSystemSetting.Height );
            plot.PlotCylinderEdge(renderer, new byte[] { 255, 0, 0 });

        }

#region 窗体Resize相关
        private void MainWnd_SizeChanged(object sender, EventArgs e)
        {
            gResizeCnt = 0;
            int counter = 0;
            if (this.Size.Width < initFormSizeWidth || this.Size.Height < initFormSizeHeight)
            //如果窗体的大小在改变过程中小于窗体尺寸的初始值，则窗体中的各个控件自动重置为初始尺寸，且窗体自动添加滚动条
            {

                foreach (Control iniCtrl in initialCrlName)
                {
                    iniCtrl.Width = (int)ctrlSizeWidth[counter];
                    iniCtrl.Height = (int)ctrlSizeHeight[counter];
                    Point point = new Point();
                    point.X = (int)ctrlLocationX[counter];
                    point.Y = (int)ctrlLocationY[counter];
                    iniCtrl.Bounds = new Rectangle(point, iniCtrl.Size);
                    counter++;
                }
                this.AutoScroll = true;
            }
            else
            //否则，重新设定窗体中所有控件的大小（窗体内所有控件的大小随窗体大小的变化而变化）
            {
                this.AutoScroll = false;
                ResetAllCrlState(this);
            }
        }

        /// <summary>
        /// 获得并存储窗体中各控件的初始位置
        /// </summary>
        /// <param name="ctrlContainer"></param>
        private void GetAllCtrlLocation(Control ctrlContainer)
        {
            foreach (Control iCtrl in ctrlContainer.Controls)
            {

                if (iCtrl.Controls.Count > 0)
                    GetAllCtrlLocation(iCtrl);
                initialCrlName.Add(iCtrl);
                ctrlLocationX.Add(iCtrl.Location.X);
                ctrlLocationY.Add(iCtrl.Location.Y);


            }
        }

        /// <summary>
        /// 获得并存储窗体中各控件的初始尺寸
        /// </summary>
        /// <param name="CrlContainer"></param>
        private void GetAllCtrlSize(Control CrlContainer)
        {
            foreach (Control iCrl in CrlContainer.Controls)
            {
                if (iCrl.Controls.Count > 0)
                    GetAllCtrlSize(iCrl);
                ctrlSizeWidth.Add(iCrl.Width);
                ctrlSizeHeight.Add(iCrl.Height);
            }
        }

        /// <summary>
        /// 获得并存储窗体的初始尺寸
        /// </summary>
        private void GetInitialFormSize()
        {
            initFormSizeWidth = this.Size.Width;
            initFormSizeHeight = this.Size.Height;
        }

        public void ResetAllCrlState(Control CrlContainer)//重新设定窗体中各控件的状态（在与原状态的对比中计算而来）
        {
            double changeX = (double)this.Size.Width / (double)initFormSizeWidth;
            double changeY = (double)this.Size.Height / (double)initFormSizeHeight;

            foreach (Control kCtrl in CrlContainer.Controls)
            {
                if (kCtrl.Controls.Count > 0)
                {
                    ResetAllCrlState(kCtrl);
                }
                Point point = new Point();
                point.X = (int)((int)ctrlLocationX[gResizeCnt] * changeX);
                point.Y = (int)((int)ctrlLocationY[gResizeCnt] * changeY);
                kCtrl.Width = (int)((int)ctrlSizeWidth[gResizeCnt] * changeX);
                kCtrl.Height = (int)((int)ctrlSizeHeight[gResizeCnt] * changeY);
                kCtrl.Bounds = new Rectangle(point, kCtrl.Size);
                gResizeCnt++;
            }
        }
#endregion


        #region Event handler
        private void cbBoundType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 模拟烧结过程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSinter_Click(object sender, EventArgs e)
        {
            if( balls != null )
            {
                SinteringHelper.SinteringProcess(balls);
                rwcVolumeDisp.Refresh();
            }
        }

        /// <summary>
        /// 将小球信息导入文件中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveSpheres_Click(object sender, EventArgs e)
        {
            if( balls != null )
            {
                DataReadWriteHelper.SaveModelDemAsSimple(modelDem3D, String.Format("./result/ballsInfo{0}_{1}.json", PackingSystemSetting.SystemBoundType, DateTime.Now.ToString("yyyyMMdd-HHmmss")));
            }
            else
            {
                log.Warn("balls is null now!!");
            }
        }

        /// <summary>
        /// 从json文件中导入小球信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadSpheresJson_Click(object sender, EventArgs e)
        {
            SimpleModelForSave sModel;
            openFiledDlg.InitialDirectory = System.Environment.CurrentDirectory +  "\\result";
            openFiledDlg.Filter = "json file|*.json";
            if( openFiledDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DataReadWriteHelper.LoadSimpleModelFromFile(openFiledDlg.FileName, out sModel);
                if (sModel != null && sModel.Radii.Rows == balls.Spheres.Count())
                {
                    balls.ReloadInfo(sModel);
                    modelDem3D.UpdateBallInfo(balls);
                    rwcVolumeDisp.Refresh();
                }
                else
                {
                    log.Warn("file error or number of balls is not corrent");
                    MessageBox.Show("file error or number of balls is not corrent");
                }
            }
            
        }

        /// <summary>
        /// 测试按钮，用于测试一些函数的功能等
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click(object sender, EventArgs e)
        {
            CuteTools.ShowImageSeries(@"D:/MyDesktop/imgSlice/%04d.bmp", 100, 100, 0, 99);
        }

        private void btnSaveImg_Click(object sender, EventArgs e)
        {
            SaveRendererAsPic(String.Format("./result/screenshot{0}.png",DateTime.Now.ToString("yyyyMMddHHmmss")));
        }


        /// <summary>
        /// 生成测试图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenerateSlices_Click(object sender, EventArgs e)
        {
            double xmin = 0, xmax = 0, ymin = 0, ymax = 0, zmin = 0, zmax = 0, m = 0.1;
            if( PackingSystemSetting.SystemBoundType == BoundType.CubeType )
            {
                xmin = N_BASE * m;
                xmax = N_BASE * (1 - m);

                ymin = N_BASE * m;
                ymax = N_BASE * (1 - m);

                zmin = 0;
                zmax = N_BASE / (1 - 2 * m);
            }
            else if( PackingSystemSetting.SystemBoundType == BoundType.CylinderType )
            {
                m = Math.Sqrt(2) / 2;
                xmin = -PackingSystemSetting.Radius * m;
                xmax =  PackingSystemSetting.Radius * m;

                ymin = -PackingSystemSetting.Radius * m;
                ymax = PackingSystemSetting.Radius * m;

                zmin = 0;
                zmax = PackingSystemSetting.Height / 1.2;
            }

            string dir = String.Format("./result/imgSlice/{0}/", DateTime.Now.ToString("yyyyMMddHHmmss"));
            Directory.CreateDirectory( dir );
            GenerateSlices(xmin, xmax, ymin, ymax, zmin, zmax, 100, 100, 100, dir + "{0:D4}.bmp");

        }

        #endregion

        /// <summary>
        /// 生成图片切片
        /// </summary>
        /// <param name="xmin"></param>
        /// <param name="xmax"></param>
        /// <param name="ymin"></param>
        /// <param name="ymax"></param>
        /// <param name="zmin"></param>
        /// <param name="zmax"></param>
        /// <param name="sizeX"></param>
        /// <param name="sizeY"></param>
        /// <param name="sizeZ"></param>
        /// <param name="fn"></param>
        private void GenerateSlices(double xmin, double xmax, double ymin, double ymax, double zmin, double zmax, int sizeX, int sizeY, int sizeZ, string fn)
        {
            double xStep = (xmax - xmin) / (sizeX - 1);
            double yStep = (ymax - ymin) / (sizeY - 1);
            double zStep = (zmax - zmin) / (sizeZ - 1);

            //为保证每个方向上的步长一致，在此采用最小步长
            double step = Math.Min(Math.Min(xStep, yStep), zStep);

            Parallel.For(0, sizeZ, (i) =>
                {
                    Matrix<byte> m = new Matrix<byte>(sizeX, sizeY);
                    double z = zmin + step * i;
                    for (int j = 0; j < sizeX; j++)
                    {
                        for (int k = 0; k < sizeY; k++)
                        {
                            if (modelDem3D.IsPointInSpheres(xmin + j * step, ymin + k * step, z))
                            {
                                m[j, k] = 255;
                            }
                        }
                    }
                    CvInvoke.Imwrite(String.Format(fn, i), m);
                });


        }


    }
}
