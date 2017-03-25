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
using Microsoft.VisualBasic;

namespace SpherePacking.MainWindow
{
    public partial class MainWnd : Form
    {
        /// <summary>
        /// Here is the once-per-class call to initialize the log object
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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

        /// <summary>
        /// 用于计时
        /// </summary>
        private Stopwatch stopWatch = new Stopwatch();

        private vtkActorCollection actorCollection = new vtkActorCollection();

        /// <summary>
        /// 主线程的ID号
        /// </summary>
        private static int mainThreadID;

        /// <summary>
        /// 判断当前线程是否为主线程
        /// </summary>
        private bool IsMainThread
        {
            get
            {
                return mainThreadID == Thread.CurrentThread.ManagedThreadId;
            }
        }

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

        private int gResizeCnt = 0;//为防止递归遍历控件时产生混乱，故专门设定一个全局计数器
        #endregion

        public MainWnd()
        {
            InitializeComponent();

            InitializeFormSize();

            log4net.Config.BasicConfigurator.Configure(new log4net.Appender.FileAppender(new log4net.Layout.PatternLayout("%date [%thread] %level %logger line%L - %message%newline%exception"),
                                String.Format("{0}log-{1}.log", PackingSystemSetting.LogDir, DateTime.Now.ToString("yyyyMMdd-HHmmss")), false));
            mainThreadID = Thread.CurrentThread.ManagedThreadId;
            ImportSystemSettings();

            //新开窗口实现可视化
            //VisiualizeSlicesWnd vwnd = new VisiualizeSlicesWnd(100, 100, 0, 99, @"humanThreeDim/final/%03d.bmp");
            //VisiualizeSlicesWnd vwnd = new VisiualizeSlicesWnd(100, 100, 0, 99, @"humanThreeDim/original/%04d.bmp");
            //VisiualizeSlicesWnd vwnd = new VisiualizeSlicesWnd(100, 100, 0, 99, @"humanThreeDim/initial/%03d.bmp");

            //VisiualizeSlicesWnd vwnd = new VisiualizeSlicesWnd(64, 64, 0, 62, @"Z10101/final/%03d.bmp");
            //VisiualizeSlicesWnd vwnd = new VisiualizeSlicesWnd(64, 64, 400, 463, @"Z10101/original/Z10101__rec%04d.bmp");
            //VisiualizeSlicesWnd vwnd = new VisiualizeSlicesWnd(64, 64, 0, 62, @"Z10101/initial/%03d.bmp");

            //VisiualizeSlicesWnd vwnd = new VisiualizeSlicesWnd(64, 64, 0, 63, @"H1010202/final/%03d.bmp");
            //VisiualizeSlicesWnd vwnd = new VisiualizeSlicesWnd(64, 64, 1000, 1063, @"H1010202/original/H1010202__rec%04d.bmp");
            //VisiualizeSlicesWnd vwnd = new VisiualizeSlicesWnd(64, 64, 0, 63, @"H1010202/initial/%03d.bmp");

            //vwnd.Show();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeSystem();
        }

        /// <summary>
        /// 初始化小球堆积系统
        /// 在寻找窗口中RenderWindowControl时，需要通过遍历的方式去寻找，因为在修改配置信息之后，需要在窗口中重新添加这个控件
        /// </summary>
        private void InitializeSystem()
        {
            foreach (Control c in this.Controls)
            {
                if (c.GetType() == rwcVolumeDisp.GetType())
                {
                    renderer = (c as RenderWindowControl).RenderWindow.GetRenderers().GetFirstRenderer(); //将此renderer和显示的窗体控件相关联
                    renderer.ResetCamera();
                    renderer.RemoveAllLights();
                    renderer.SetBackground(0.7, 0.7, 0.7); //设置背景
                    AddLightToRenderer(renderer);
                    SetCameraToRenderer(renderer);
                }
            }

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

            //添加Actors到renderer中，用于显示
            while (actorCollection.GetNumberOfItems() != 0)
            {
                renderer.AddActor(actorCollection.GetLastActor());
                actorCollection.RemoveItem(actorCollection.GetLastActor());
            }

            modelDem3D = new ModelDem3D(balls);
            modelDem3D.RefreshHandler = new ModelDem3D.RefreshWindowHandler(RefreshWindow);

            GenerateDirsIfNotExist();
        }

        /// <summary>
        /// 在系统启动时导入系统设置
        /// 文件的路径为："./result/global-settings.json"
        /// 若文件不存在，则导入默认配置
        /// </summary>
        private void ImportSystemSettings()
        {
            PackSysSettingForSave setting;
            setting = DataReadWriteHelper.LoadSimpleModelFromFile<PackSysSettingForSave>("./result/global-settings.json");
            if (setting == null)
            {
                string info = "packing system settings file not found! choose the default settings";
                log.Warn(info);
                ShowText( info, IsMainThread );
            }
            else
            {
                setting.ExportToSysSetting();
            }
        }

        /// <summary>
        /// 生成存储相关信息的文件夹
        /// 如果是多级路径，则也会依次新建
        /// </summary>
        private void GenerateDirsIfNotExist()
        {
            if (!Directory.Exists(PackingSystemSetting.ResultDir))
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
        private void AddLightToRenderer(vtkRenderer renderer)
        {
            vtkLight light1 = vtkLight.New();
            light1.SetColor(0, 1, 0);
            light1.SetPosition(-PackingSystemSetting.CubeLength, -PackingSystemSetting.CubeLength, -PackingSystemSetting.CubeLength);
            light1.SetFocalPoint(renderer.GetActiveCamera().GetFocalPoint()[0],
                                renderer.GetActiveCamera().GetFocalPoint()[1],
                                renderer.GetActiveCamera().GetFocalPoint()[2]);

            vtkLight light2 = vtkLight.New();
            //light2.SetColor(0, 0, 1);
            light2.SetColor(0, 1, 0);
            light2.SetPosition(PackingSystemSetting.CubeLength * 2, PackingSystemSetting.CubeLength * 2, PackingSystemSetting.CubeLength * 2);
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
            camera.SetPosition(-PackingSystemSetting.CubeLength, -PackingSystemSetting.CubeLength, PackingSystemSetting.CubeLength * 3);
            //camera.ComputeViewPlaneNormal();
            //camera.SetViewUp(10, 10, 0);
            renderer.SetActiveCamera(camera);
        }

        /// <summary>
        /// 向renderer中添加坐标轴
        /// </summary>
        /// <param name="renderer"></param>
        private void AddCoordianteToActorCollection(ref vtkActorCollection actors)
        {
            vtkCubeAxesActor axes = vtkCubeAxesActor.New();
            axes.SetCamera(renderer.GetActiveCamera());
            axes.SetBounds(0, PackingSystemSetting.CubeLength, 0, PackingSystemSetting.CubeLength, 0, PackingSystemSetting.CubeLength);
            axes.SetOrigin(0, 0, 0);
            actors.AddItem(axes);
        }

        /// <summary>
        /// 刷新rendererwindow
        /// </summary>
        private void RefreshWindow(int iter, Matrix<double> pos)
        {
            //Console.WriteLine("current iteration : " + iter);
            for (int i = 0; i < balls.Spheres.Count(); i++)
            {
                //Console.WriteLine("%dth sphere pos: (" + pos[i, 0] + ", " + +pos[i, 1] + ", " + pos[i, 2] + ")");
                balls.SetCenter(pos[i, 0], pos[i, 1], pos[i, 2], i);
            }

            // 每隔100次迭代 保存一次信息
            if ((iter + 1) % 100 == 0)
            //if( false )
            {
                DataReadWriteHelper.SaveObjAsJsonFile(new SimpleModelForSave(modelDem3D), String.Format("{0}/ballsInfo{1}_{2:D4}.json", PackingSystemSetting.ResultDir, PackingSystemSetting.SystemBoundType, iter));
            }

            try
            {
                ShowText("iteration : " + iter, IsMainThread);
                if( PackingSystemSetting.IsVisualize )
                {
                    RefreshRenderWindowCtrl(IsMainThread);
                }
                //SaveRendererAsPic(String.Format("./result/iter{0:D5}.png", iter));
            }
            catch (Exception ex)
            {
                log.Error(ex);
                ShowText( String.Format("error : {0}\r\nThread has been aborted at iteration : {1}", 
                                    ex.ToString(), iter), IsMainThread );
            }
        }

        /// <summary>
        /// 关闭界面前，释放可能在运行的线程，防止发生异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWnd_FormClosing(object sender, FormClosingEventArgs e)
        {
            log.Info("program is being closed...");
            if (solveThread != null)
            {
                Console.WriteLine("in : " + solveThread.ThreadState);
                if ((solveThread.ThreadState == System.Threading.ThreadState.WaitSleepJoin
                                       || solveThread.ThreadState == System.Threading.ThreadState.Running
                                       || solveThread.ThreadState == System.Threading.ThreadState.Suspended))
                {
                    solveThread.Abort();
                    solveThread.Join();
                    Console.WriteLine(solveThread.ThreadState);
                }
            }
            //System.Environment.Exit(0);
        }

        /// <summary>
        /// 以png形式存储当前RenderWindowControl窗口的状态
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        private bool SaveRendererAsPic(string fn)
        {
            bool res = true;

            RenderWindowControl rwc = new RenderWindowControl();
            foreach (Control c in this.Controls)
            {
                if ((rwc = (c as RenderWindowControl)) != null)
                {
                    CuteTools.SaveRendererWindowsAsPic(fn, ref rwc);
                }
            }
            return res;
        }

        /// <summary>
        /// 初始化边界为立方体的情况
        /// </summary>
        private void InitStatusForCube()
        {
            balls = new SpherePlot();
            balls.PlotSphereInRender(renderer, ref actorCollection);

            CubeEdgePlot edgePlot = new CubeEdgePlot(PackingSystemSetting.CubeLength, 2 * PackingSystemSetting.CubeLength);
            edgePlot.AddContainerEdgeToActorCollection(new byte[] { 255, 0, 0 }, ref actorCollection);

            cubeSurfacePlot = new CubeSurfacePlot(PackingSystemSetting.CubeLength);
            //cubeSurfacePlot.PlotSurfaceAll(renderer);

            AddCoordianteToActorCollection(ref actorCollection);
        }

        /// <summary>
        /// 初始化边界为圆柱体的情况
        /// </summary>
        private void InitStatusForCylinder()
        {

            balls = new SpherePlot();
            balls.PlotSphereInRender(renderer, ref actorCollection);

            //标准参数应该是100，40
            CylinderEdgePlot plot = new CylinderEdgePlot(PackingSystemSetting.Radius, PackingSystemSetting.Height);
            plot.AddCylinderEdgeToActors(new byte[] { 0, 1, 1 }, ref actorCollection);


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
        /// 初始化关于窗口resize相关的变量
        /// 因为RenderWindow需要重新添加，因此首先需要清空之前保存的信息
        /// </summary>
        private void InitializeFormSize()
        {
            initialCrlName.Clear();
            ctrlLocationX.Clear();
            ctrlLocationY.Clear();
            ctrlSizeWidth.Clear();
            ctrlSizeHeight.Clear();

            GetInitialFormSize();
            GetAllCtrlLocation(this);
            GetAllCtrlSize(this);
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
        /// <summary>
        /// 测试按钮，用于测试一些函数的功能等
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click(object sender, EventArgs e)
        {
            //CuteTools.ShowImageSeries(@"./humanThreeDim/H1010202__rec_dpn/final/%03d.bmp", 64, 64, 0, 63);
            Console.WriteLine( IsMainThread );
            GenerateRandomNumber.TestLogNormalNumber();
            
        }

        /// <summary>
        /// 对新的两两小球之间的距离计算的时间进行评价
        /// </summary>
        private void TestComputeDistEffect()
        {
            int num = 500;
            Matrix<double> mean = new Matrix<double>(1, 1);
            Matrix<double> stddev = new Matrix<double>(1, 1);
            mean[0, 0] = 0;
            stddev[0, 0] = 1;
            Matrix<double> pos = new Matrix<double>(num, 2);
            CvInvoke.Randn(pos, mean, stddev);

            stopWatch.Restart();
            Matrix<double> dists = CuteTools.ComputeMatDist(pos);
            
            Console.WriteLine(stopWatch.ElapsedMilliseconds);

            stopWatch.Restart();
            for (int i = 0; i < num;i++ )
            {
                for(int j=i+1;j<num;j++)
                {
                    dists[i, j] = CvInvoke.Norm( pos.GetRow(i) - pos.GetRow(j), Emgu.CV.CvEnum.NormType.L2 );
                    dists[j, i] = dists[i, j];
                }
            }
            Console.WriteLine(stopWatch.ElapsedMilliseconds);
        }
        
        private void tmiSettings_Click(object sender, EventArgs e)
        {
            GlobalSettingsWnd settingsWnd = new GlobalSettingsWnd();
            settingsWnd.ReloadSystemSetting += ReloadSystemSetting;
            settingsWnd.ShowDialog();
        }

        /// <summary>
        /// 回放功能
        ///     读取系列小球的位置信息文件并按照一定时间间隔进行更新
        ///     因为回放界面会卡死，因此将其放在一个线程中执行，保证界面可以被操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmiPlayback_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "选择小球信息所在的文件夹";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ThreadStart ts = new ThreadStart(delegate 
                    {
                        PlayBack( dialog.SelectedPath );
                    });
                Thread th = new Thread(ts);
                th.IsBackground = true;
                th.Start();
            }
        }

        /// <summary>
        /// 模拟小球的烧结过程
        /// 方法：将小球的半径按照一定的比例进行增大，使小球之间相交
        ///     to be done : 实际的烧结过程中，小球的半径的扩大方法需要改变，或者可以缩小小球所在的空间，使得小球相交等
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmiSimulateSinter_Click(object sender, EventArgs e)
        {
            if (balls != null)
            {
                SinteringHelper.SinteringProcess(balls);
                rwcVolumeDisp.Refresh();
            }
        }

        /// <summary>
        /// 可视化切片图像序列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmiVisualzieSlices_Click(object sender, EventArgs e)
        {
            string str = @"humanThreeDim/final/%03d.bmp";
            //新开窗口实现可视化
            VisiualizeSlicesWnd vwnd = new VisiualizeSlicesWnd(100, 100, 0, 99, @"humanThreeDim/final/%03d.bmp");
            vwnd.Show();
        }

        /// <summary>
        /// 保存设置到文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmiSaveSettings_Click(object sender, EventArgs e)
        {
            DataReadWriteHelper.SaveObjAsJsonFile(new PackSysSettingForSave(), PackingSystemSetting.SettingFilename);
        }

        /// <summary>
        /// 保存小球的半径、位置、速度、加速度、能量信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmiSaveBallsInfo_Click(object sender, EventArgs e)
        {
            if (balls != null)
            {
                DataReadWriteHelper.SaveObjAsJsonFile(new SimpleModelForSave(modelDem3D), String.Format("./result/ballsInfo{0}_{1}.json", PackingSystemSetting.SystemBoundType, DateTime.Now.ToString("yyyyMMdd-HHmmss")));
                DataReadWriteHelper.RecordInfo("rePos" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt", PackingSystemSetting.ResultDir, modelDem3D.RtPos, false);
                DataReadWriteHelper.RecordInfo("reVel" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt", PackingSystemSetting.ResultDir, modelDem3D.RtVel, false);
                DataReadWriteHelper.RecordInfo("reAcc" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt", PackingSystemSetting.ResultDir, modelDem3D.RtAcc, false);
                DataReadWriteHelper.RecordInfo("radii" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt", PackingSystemSetting.ResultDir, modelDem3D.Radii, false);
                DataReadWriteHelper.RecordInfo("energy" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt", PackingSystemSetting.ResultDir, modelDem3D.Energy, false);
            }
            else
            {
                string warning = "balls is null now!!";
                log.Warn(warning);
                ShowText(warning, IsMainThread);
            }
        }

        /// <summary>
        /// 将当前RenderWindowControl中的图像保存至png文件中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmiSaveCurrentRenderer_Click(object sender, EventArgs e)
        {
            string info;
            string fn = String.Format("{0}screenshot{1}.png", PackingSystemSetting.ResultDir, DateTime.Now.ToString("yyyyMMhh-HHmmss"));
            if (SaveRendererAsPic(fn))
            {
                info = String.Format("renderer screenshot has been saved successfully, name is {0}", fn);
                log.Info(info);
            }
            else
            {
                info = String.Format("renderer screenshot has failed to be saved..");
                log.Warn(info);
            }
            ShowText(info, IsMainThread);
        }

        /// <summary>
        /// 从json文件中导入小球信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmiImportBallsInfo_Click(object sender, EventArgs e)
        {
            SimpleModelForSave sModel;
            openFiledDlg.InitialDirectory = System.Environment.CurrentDirectory + "\\result";
            openFiledDlg.Filter = "json file|*.json";
            if (openFiledDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                sModel = DataReadWriteHelper.LoadSimpleModelFromFile<SimpleModelForSave>(openFiledDlg.FileName);
                if (sModel != null && sModel.Radii.Rows == balls.Spheres.Count())
                {
                    balls.ReloadInfo(sModel);
                    modelDem3D.UpdateBallInfo(sModel);
                    rwcVolumeDisp.Refresh();
                    string info = String.Format("import balls info from {0}, reload info successfully!", openFiledDlg.FileName);
                    log.Info(info);
                    ShowText( info, IsMainThread );
                }
                else
                {
                    string warning = "file error or number of balls is not correct";
                    log.Warn(warning);
                    ShowText(warning, IsMainThread);
                }
            }
        }

        /// <summary>
        /// 将当前的三维小球堆积结果保存为切片图像系列
        /// 将小球内部的点视为固体相，小球外部的点视为孔隙相
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmiGenerateImageSlices_Click(object sender, EventArgs e)
        {
            ///m是
            double xmin = 0, xmax = 0, ymin = 0, ymax = 0, zmin = 0, zmax = 0, m = 0.0;
            if (PackingSystemSetting.SystemBoundType == BoundType.CubeType)
            {
                xmin = PackingSystemSetting.CubeLength * m;
                xmax = PackingSystemSetting.CubeLength * (1 - m);

                ymin = PackingSystemSetting.CubeLength * m;
                ymax = PackingSystemSetting.CubeLength * (1 - m);

                zmin = 0;
                string strZ = Interaction.InputBox("请输入采样切片的高度范围", "Zmax", "2", 100, 100);
                try
                {
                    zmax = Convert.ToDouble(strZ);// PackingSystemSetting.CubeLength / (1 - 2 * m);  
                }
                catch( Exception ex )
                {
                    string inf = "input Zmax format error!";
                    log.Error(inf);
                    tbStatus.Text = inf;
                    return;
                }
                
            }
            else if (PackingSystemSetting.SystemBoundType == BoundType.CylinderType)
            {
                m = Math.Sqrt(2) / 2;
                xmin = -PackingSystemSetting.Radius * m;
                xmax = PackingSystemSetting.Radius * m;

                ymin = -PackingSystemSetting.Radius * m;
                ymax = PackingSystemSetting.Radius * m;

                zmin = 0;
                string strZ = Interaction.InputBox("请输入采样切片的高度范围", "Zmax", "2", 100, 100);
                try
                {
                    zmax = Convert.ToDouble(strZ);// PackingSystemSetting.CubeLength / (1 - 2 * m);  
                }
                catch (Exception ex)
                {
                    string inf = "input Zmax format error!";
                    log.Error(inf);
                    tbStatus.Text = inf;
                    return;
                }
            }

            string dir = String.Format("{0}imgSlice/{1}/", PackingSystemSetting.ResultDir, DateTime.Now.ToString("yyyyMMddHHmmss"));
            Directory.CreateDirectory(dir);

            double reso = 2 * balls.MaxRadius * ActualSampleParameter.PixelResolution / ActualSampleParameter.ActualSampleParaDict[PackingSystemSetting.ParticleSizeType].MaxDiameter;
            string info = string.Format("the reso of the system is {0}, which all the radii are divided can get the images in pixels", reso);
            Console.WriteLine();
            log.Info(info);
            Console.WriteLine(info);

            ThreadStart ts = new ThreadStart(delegate { GenerateSlices(xmin, xmax, ymin, ymax, zmin, zmax, reso, dir + "{0:D4}.bmp"); });
            (new Thread(ts)
                {
                    IsBackground = true,
                }).Start();
        }

        /// <summary>
        /// 退出系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmiExitSystem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定退出?", "quit", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                this.Close();
            }
        }

        /// <summary>
        /// 求解DEM问题
        /// 此处开启线程，防止主界面在求解过程中被卡死
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmiSolveProblem_Click(object sender, EventArgs e)
        {
            //有时不是Running，而是WaitSleepJoin，需要查一下
            if ((solveThread != null) && (solveThread.ThreadState == System.Threading.ThreadState.Running || solveThread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                log.Warn("solving-problem thread is running now!");
            }
            else
            {
                //在异步线程中启动程序，可以防止解算时界面卡死的情况
                ThreadStart ts = new ThreadStart(delegate { modelDem3D.SolveProblem(); });
                solveThread = new Thread(ts);
                solveThread.IsBackground = true;
                solveThread.Start();
                log.Info(String.Format("start to sovle the problem, it contains {0} balls", PackingSystemSetting.BallsNumber));
            }
        }

        /// <summary>
        /// 终止求解DEM问题的线程(如果在运行的话)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmiStopSolveProblem_Click(object sender, EventArgs e)
        {
            string text;
            //有时不是Running，而是WaitSleepJoin，需要查一下
            if ((solveThread != null) && (solveThread.ThreadState == System.Threading.ThreadState.Running || solveThread.ThreadState == System.Threading.ThreadState.WaitSleepJoin || solveThread.ThreadState == System.Threading.ThreadState.Background))
            {
                text = "going to abort the sole thread...";
                log.Info(text);
                tbStatus.Text = text;
                solveThread.Abort();
                text = "solving-problem thread was aborted successfully...";
                log.Info(text);
                tbStatus.Text = text;
            }
            else
            {
                text = "solving-problem thread is not running now!";
                log.Warn(text);
                tbStatus.Text = text;
            }
        }

        private void tmiPreference_Click(object sender, EventArgs e)
        {

        }

        private void tmiComputePorosity_Click(object sender, EventArgs e)
        {
            double porosity = 0.0;
            string text = "";
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.RootFolder = Environment.SpecialFolder.MyComputer;
            dlg.ShowDialog();
            dlg.Description = "请选择英文路径，否则无法读取图像";
            //ComputeParameters.ComputePorosity(@"D:\MyDesktop\sliceImg\", "{0:D4}.bmp", 10, 21, ref porosity);
            if( !(dlg.SelectedPath.Length == 0) && ComputeParameters.ComputePorosity(dlg.SelectedPath, ref porosity, true))
            {
                text = string.Format("chosen folder is {0}, porosity is {1:0.000000}", dlg.SelectedPath, porosity);
            }
            else
            {
                text = "folder not chosen...";
            }
            ShowText(text, IsMainThread);
            log.Info( text );
        }

        private void tmiComputeTwoPointCorr_Click(object sender, EventArgs e)
        {

        }

        private void tmiComputeSurfaceArea_Click(object sender, EventArgs e)
        {

        }

        private void tmiComputeTortuosity_Click(object sender, EventArgs e)
        {

        }

        #endregion

        /// <summary>
        /// 生成图片切片
        /// </summary>
        /// <param name="xmin">xmin</param>
        /// <param name="xmax">xmax</param>
        /// <param name="ymin">ymin</param>
        /// <param name="ymax">ymax</param>
        /// <param name="zmin">zmin</param>
        /// <param name="zmax">zmax</param>
        /// <param name="reso">精度</param>
        /// <param name="fn">文件名</param>
        private void GenerateSlices(double xmin, double xmax, double ymin, double ymax, double zmin, double zmax, double reso, string fn)
        {
            stopWatch.Restart();

            string info;
            info = "begin to generate slices... please do not do any other operations...";
            log.Info(info);
            ShowText(info, IsMainThread);

            int sizeX = (int)((xmax - xmin) / reso);
            int sizeY = (int)((ymax - ymin) / reso);
            int sizeZ = (int)((zmax - zmin) / reso);


            // new  method 处理100X100X100图像的时间约为1s
            List<Matrix<byte>> pics = new List<Matrix<byte>>();
            for (int i = 0; i < sizeZ; i++)
            {
                pics.Add(new Matrix<byte>(sizeX, sizeY));
            }

            Parallel.For(0, PackingSystemSetting.BallsNumber, (n) =>
                {
                    int index_x_min = (int)(((modelDem3D.RtPos[n, 0] - modelDem3D.Radii[n, 0] - xmin)) / reso) - 1;
                    int index_x_max = (int)(((modelDem3D.RtPos[n, 0] + modelDem3D.Radii[n, 0] - xmin)) / reso) + 1;
                    int index_y_min = (int)(((modelDem3D.RtPos[n, 1] - modelDem3D.Radii[n, 0] - ymin)) / reso) - 1;
                    int index_y_max = (int)(((modelDem3D.RtPos[n, 1] + modelDem3D.Radii[n, 0] - ymin)) / reso) + 1;
                    int index_z_min = (int)(((modelDem3D.RtPos[n, 2] - modelDem3D.Radii[n, 0] - zmin)) / reso) - 1;
                    int index_z_max = (int)(((modelDem3D.RtPos[n, 2] + modelDem3D.Radii[n, 0] - zmin)) / reso) + 1;
                    index_x_min = index_x_min >= 0 ? index_x_min : 0;
                    index_x_min = index_x_min <= sizeX ? index_x_min : sizeX;
                    index_x_max = index_x_max >= 0 ? index_x_max : 0;
                    index_x_max = index_x_max <= sizeX ? index_x_max : sizeX;

                    index_y_min = index_y_min >= 0 ? index_y_min : 0;
                    index_y_min = index_y_min <= sizeY ? index_y_min : sizeY;
                    index_y_max = index_y_max >= 0 ? index_y_max : 0;
                    index_y_max = index_y_max <= sizeY ? index_y_max : sizeY;

                    index_z_min = index_z_min >= 0 ? index_z_min : 0;
                    index_z_min = index_z_min <= sizeZ ? index_z_min : sizeZ;
                    index_z_max = index_z_max >= 0 ? index_z_max : 0;
                    index_z_max = index_z_max <= sizeZ ? index_z_max : sizeZ;

                    for (int k = index_z_min; k < index_z_max; k++)
                    {
                        for (int i = index_x_min; i < index_x_max; i++)
                        {
                            for (int j = index_y_min; j < index_y_max; j++)
                            {
                                if (modelDem3D.IsPointInReferedSphere(xmin + i * reso, ymin + j * reso, zmin + k * reso, n))
                                {
                                    pics[k][i, j] |= 255;
                                }
                            }
                        }
                    }
                });

            for (int i = 0; i < sizeZ; i++)
            {
                CvInvoke.Imwrite(String.Format(fn, i), pics[i]);
            }


            // 处理100X100X100图像的时间约为150s
            //Parallel.For(0, sizeZ, (i) =>
            //    {
            //        Matrix<byte> m = new Matrix<byte>(sizeX, sizeY);
            //        double z = zmin + reso * i;
            //        for (int j = 0; j < sizeX; j++)
            //        {
            //            for (int k = 0; k < sizeY; k++)
            //            {
            //                if (modelDem3D.IsPointInSpheres(xmin + j * reso, ymin + k * reso, z))
            //                {
            //                    m[j, k] = 255;
            //                }
            //            }
            //        }
            //        CvInvoke.Imwrite(String.Format(fn, i), m);
            //    });

            info = String.Format("generate slices successfully, the folder is : {0}, and elapsed time is {1}ms", fn, stopWatch.ElapsedMilliseconds);
            log.Info(info);
            ShowText(info, IsMainThread);
            stopWatch.Reset();
        }

        

        /// <summary>
        /// 在更改系统设置之后，根据新的系统设置重新初始化vtk显示相关的小球、容器等
        /// </summary>
        private void ReloadSystemSetting()
        {
            RenderWindowControl rwc = new RenderWindowControl();

            for (int i = 0; i < this.Controls.Count; i++)
            {
                if ((rwc = (this.Controls[i] as RenderWindowControl)) != null)
                {
                    RenderWindowControl r = new RenderWindowControl()
                    {
                        Location = rwc.Location,
                        Size = rwc.Size,
                        AutoSizeMode = rwc.AutoSizeMode,
                        AutoScaleMode = rwc.AutoScaleMode,
                        Name = rwc.Name,
                        AutoScroll = rwc.AutoScroll,
                        BorderStyle = rwc.BorderStyle,
                    };
                    rwc.Dispose();
                    this.Controls.RemoveByKey("rwcVolumeDisp");
                    this.Controls.Add(r);
                    InitializeFormSize();
                    break;
                }
            }


            InitializeSystem();
        }

        /// <summary>
        /// 回放之前的数据
        /// 用于演示
        /// </summary>
        /// <param name="folder">json文件所在的文件夹</param>
        private bool PlayBack( string folder )
        {
            Console.WriteLine(  IsMainThread );
            SimpleModelForSave sModel;
            var files = Directory.GetFiles(folder, "*.json");
            bool res = true;
            string text = "";

            log.Info("beginning to play back files...");
            ShowText("playing back files, better not do other things at the same time...", IsMainThread);
            foreach (var file in files)
            {
                sModel = DataReadWriteHelper.LoadSimpleModelFromFile<SimpleModelForSave>(file);
                if (sModel != null && sModel.Radii.Rows == balls.Spheres.Count())
                {
                    balls.ReloadInfo(sModel);
                    modelDem3D.UpdateBallInfo(balls);
                    RefreshRenderWindowCtrl( IsMainThread);
                }
                else
                {
                    res = false;
                    text = String.Format("import balls info wrong!! wrong filename is {0}", file);
                    log.Warn(text);
                    ShowText(text, IsMainThread);
                    break;
                }
            }
            if( res )
            {
                text = "playing back files successfully...";
                log.Info(text);
                ShowText(text, IsMainThread);
            }
            return res;
        }

        /// <summary>
        /// 设置文本框
        /// 在主线程中被调用时，直接设置Text即可
        /// 有的不是在主线程中进行设置的，需要设置Invoke方法
        /// </summary>
        /// <param name="text">设置的消息内容</param>
        /// <param name="isMainThread">是否为主线程</param>
        private void ShowText( string text, bool isMainThread )
        {
            if (tbStatus.IsDisposed)
                return;
            try
            {
                if (isMainThread)
                {
                    tbStatus.Text = text;
                }
                else
                {
                    this.Invoke((EventHandler)(delegate
                    {
                        tbStatus.Text = text;
                    }));
                }
            }
            // 关闭窗口时控件被释放之后 再去刷新会触发异常
            catch (Exception ex)
            {
                log.Warn(ex.ToString());
            }
        }

        /// <summary>
        /// 刷新RenderWindowControl控件
        /// 主线程中直接刷新就行，非主线程则需要设置Invoke方法
        /// </summary>
        /// <param name="isMainThread"></param>
        private void RefreshRenderWindowCtrl(bool isMainThread)
        {
            if (this.Controls.Find("rwcVolumeDisp", false)[0] as RenderWindowControl == null 
                || (this.Controls.Find("rwcVolumeDisp", false)[0] as RenderWindowControl).IsDisposed)
            {
                return;
            }
            try
            {
                if (isMainThread)
                {
                    (this.Controls.Find("rwcVolumeDisp", false)[0] as RenderWindowControl).Refresh();
                }
                else
                {
                    this.Invoke((EventHandler)(delegate
                    {
                        (this.Controls.Find("rwcVolumeDisp", false)[0] as RenderWindowControl).Refresh();
                    }));
                }
            }
            // 关闭窗口时控件被释放之后 再去刷新会触发异常
            catch (Exception ex)
            {
                log.Warn(ex.ToString());
            }
            
        }

        

    }
}
