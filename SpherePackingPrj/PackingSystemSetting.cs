using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpherePacking.MainWindow
{
    /// <summary>
    /// 系统设置
    /// </summary>
    class PackingSystemSetting
    {
        /// <summary>
        /// 小球的最大半径
        /// </summary>
        public static double MaxBallRadius = 0.5;

        /// <summary>
        /// 圆柱体边界的半径
        /// 标准值是100
        /// </summary>
        public static double Radius = 5;

        /// <summary>
        /// 容器的高
        /// 标准值是40
        /// </summary>
        public static double Height = 4;

        /// <summary>
        /// 长方体容器的边长
        /// 小球的个数是CubeLength*CubeLength*Height
        /// </summary>
        public static double CubeLength = 3;

        /// <summary>
        /// Z方向上的小球的个数与x、y方向上的小球的个数的比
        /// </summary>
        public static double ZRate = 3;


        /// <summary>
        /// 系统的精度
        /// 将该系统中的所有值乘以该值，得到的信息以像素为单位
        /// 像素值/系统单位
        /// </summary>
        public static double ResolutionPixelPerSysUnit
        {
            get
            {
                return ActualSampleParameter.ActualSampleParaDict[PackingSystemSetting.ParticleSizeType].MaxDiameter / 
                        (2 * MaxBallRadius) / ActualSampleParameter.PixelResolution;
            }
        }

        /// <summary>
        /// 系统精度
        ///   用于小球将系统单位转化为以um为单位
        ///   um/系统单位
        /// </summary>
        public static double ResolutionUmPerSysUnit
        {
            get
            {
                return ActualSampleParameter.ActualSampleParaDict[PackingSystemSetting.ParticleSizeType].MaxDiameter / (2 * MaxBallRadius) * 1e6;
            }
        }

        /// <summary>
        /// 所有小球的个数
        /// 对于 长方体容器条件：
        ///     BallsNumber = Z_RATE * CubeLength * CubeLength * Height;
        /// 对于 圆柱体容器条件：
        ///     BallsNumber = 2 * Radius * Radius * Height * Z_RATE ;
        /// </summary>
        public static int BallsNumber
        {
            get
            {
                return ( SystemBoundType == BoundType.CubeType )?
                                ((int)(ZRate * CubeLength * CubeLength * Height)):
                                ((int)(2*Radius*Radius*Height*ZRate));
            }
        }

        /// <summary>
        /// 模型求解时的迭代次数
        /// </summary>
        public static int IterationNum = 10000;

        /// <summary>
        /// 是否采用并行计算，在for循环的时候或许可以加快运算速度呢
        /// </summary>
        public static bool IsParaCompute = true;

        public static BoundType SystemBoundType = BoundType.CubeType;

        /// <summary>
        /// 存储运行结果的文件夹
        /// </summary>
        public static string ResultDir = String.Format("./result/{0}/", DateTime.Now.ToString("yyyyMMdd-HHmmss"));


        /// <summary>
        /// 生成日志的文件夹
        /// </summary>
        public static string LogDir = "./log/";

        /// <summary>
        /// 第三方库文件夹
        /// </summary>
        public static string LibDir = "./libs/";

        /// <summary>
        /// 保存系统上次系统设置的文件夹，下次启动时，采用该文件进行系统配置
        /// 如果文件不存在，则采用默认的设置
        /// </summary>
        public static string SettingFilename = "./result/global-settings.json";

        /// <summary>
        /// 是否进行可视化
        /// </summary>
        public static bool IsVisualize = true;

        /// <summary>
        /// 步长，一般设置为1e-4到5e-3之间
        /// 越大，仿真速度越快，但系统容易震荡；越慢，系统稳定时间越长
        /// </summary>
        public static double StepLength = 4e-3;

        /// <summary>
        /// 小球粒径类型
        /// </summary>
        public static ActualSampleType ParticleSizeType = ActualSampleType.FirstBatch30_50;
    }

    class PackSysSettingForSave
    {
        public double Radius;

        public double Height;

        public double CubeLength;

        public double Z_RATE;

        public int BallsNumber;

        public int IterationNum;

        public bool IsParaCompute;

        public BoundType SystemBoundType;

        public string LogDir;

        public string LibDir;

        public bool IsVisualize;

        public double StepLength;

        public PackSysSettingForSave( )
        {
            this.Radius = PackingSystemSetting.Radius;
            this.Height = PackingSystemSetting.Height;
            this.CubeLength = PackingSystemSetting.CubeLength;
            this.Z_RATE = PackingSystemSetting.ZRate;
            this.BallsNumber = PackingSystemSetting.BallsNumber;
            this.IterationNum = PackingSystemSetting.IterationNum;
            this.IsParaCompute = PackingSystemSetting.IsParaCompute;
            this.SystemBoundType = PackingSystemSetting.SystemBoundType;
            this.LogDir = PackingSystemSetting.LogDir;
            this.LibDir = PackingSystemSetting.LibDir;
            this.IsVisualize = PackingSystemSetting.IsVisualize;
            this.StepLength = PackingSystemSetting.StepLength;
        }

        /// <summary>
        /// 利用从json文件导入的系统配置类重新配置系统
        /// 产生结果的文件夹在这里没有导入，建议是按照时间命名；
        /// 可以保证在每次运行时，使所有的结果在同一文件夹下
        /// </summary>
        public void ExportToSysSetting()
        {
            PackingSystemSetting.Radius = this.Radius;
            PackingSystemSetting.Height = this.Height;
            PackingSystemSetting.CubeLength = this.CubeLength;
            PackingSystemSetting.IterationNum = this.IterationNum;
            PackingSystemSetting.ZRate = this.Z_RATE;
            PackingSystemSetting.IsParaCompute = this.IsParaCompute;
            PackingSystemSetting.SystemBoundType = this.SystemBoundType;
            PackingSystemSetting.LogDir = this.LogDir;
            PackingSystemSetting.LibDir = this.LibDir;
            PackingSystemSetting.IsVisualize = this.IsVisualize;
            PackingSystemSetting.StepLength = this.StepLength;
            //if (PackingSystemSetting.Radius < 2)
            //{
            //    MessageBox.Show("在此设置半径最小值为2！");
            //    PackingSystemSetting.Radius = 2;
            //}
        }
    }
}
