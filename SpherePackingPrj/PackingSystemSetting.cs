using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpherePacking.MainWindow
{
    /// <summary>
    /// 系统设置
    /// </summary>
    class PackingSystemSetting
    {
        /// <summary>
        /// 圆柱体边界的半径
        /// 标准值是100
        /// </summary>
        public static double Radius = 5;

        /// <summary>
        /// 圆柱体边界的高
        /// 标准值是40
        /// </summary>
        public static double Height = 4;

        /// <summary>
        /// 是否采用并行计算，在for循环的时候或许可以加快运算速度呢
        /// </summary>
        public static bool IsParaCompute = false;

        public static BoundType SystemBoundType = BoundType.CubeType;

        /// <summary>
        /// 存储运行结果的文件夹
        /// </summary>
        public static string ResultDir = "./result/";


        /// <summary>
        /// 生成日志的文件夹
        /// </summary>
        public static string LogDir = "./log/";

        /// <summary>
        /// 第三方库文件夹
        /// </summary>
        public static string LibDir = "./libs/";

        public PackingSystemSetting( double radius, double height )
        {
            Radius = radius;
            Height = height;
        }
    }
}
