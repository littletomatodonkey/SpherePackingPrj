using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpherePacking.MainWindow
{
    /// <summary>
    /// 实际的玻璃微珠的尺寸相关参数
    /// </summary>
    class ActualSampleParameter
    {
        /// <summary>
        /// 实际玻璃微珠参数的字典
        /// 需要访问的话直接根据字典的访问方式进行访问即可
        /// </summary>
        public static Dictionary<ActualSampleType, ActualSampleInfo> ActualSampleParaDict = new Dictionary<ActualSampleType, ActualSampleInfo>()
            {
                ///30～50 um参数
                {ActualSampleType.FirstBatch30_50, new ActualSampleInfo()
                        {
                            MaxDiameter = 91.201e-6,
                            MinDiameter = 13.183e-6,
                            Diameter10 = 23.935E-6,
                            Diameter50 = 34.394E-6,
                            Diameter90 = 49.529E-6,
                            LogMiu = 3.534 - 6 * Math.Log(10),
                            LogSigma = 0.285571,
                        }},
                //50~70 um参数
                {ActualSampleType.FirstBatch50_70, new ActualSampleInfo()
                        {
                            MaxDiameter = 120.226e-6,
                            MinDiameter = 19.953e-6,
                            Diameter10 = 36.99E-6,
                            Diameter50 = 52.623E-6,
                            Diameter90 = 74.69E-6,
                            LogMiu = 3.96123- 6 * Math.Log(10),
                            LogSigma = 0.279829,
                        }},

            };

        /// <summary>
        /// 实际样片中一个像素所对应的长度(m为单位)
        /// </summary>
        public static double PixelResolution { get { return 0.97e-6; } }
    }

    /// <summary>
    /// 实际的玻璃微珠样片的参数，都是按照直径为来记录的
    /// </summary>
    class ActualSampleInfo
    {
        /// <summary>
        /// 最大直径
        /// </summary>
        public double MaxDiameter;

        /// <summary>
        /// 最小直径
        /// </summary>
        public double MinDiameter;

        /// <summary>
        /// 10%的直径阈值(直径小于该值的微珠的个数占总个数的10% )
        /// </summary>
        public double Diameter10;

        /// <summary>
        /// 50%的直径阈值(直径小于该值的微珠的个数占总个数的50% )
        /// </summary>
        public double Diameter50;

        /// <summary>
        /// 90%的直径阈值(直径小于该值的微珠的个数占总个数的90% )
        /// </summary>
        public double Diameter90;

        /// <summary>
        /// 对数正态分布的均值，即对粒径取对数之后的平均值
        /// 取对数之后，该参数与单位有关
        /// </summary>
        public double LogMiu;

        /// <summary>
        /// 对数正态分布的方差，即对粒径取对数之后的平均值
        /// 取对数之后，这个参数与单位无关
        /// </summary>
        public double LogSigma;
    }

    /// <summary>
    /// 实际样片的玻璃微珠的类型
    /// </summary>
    enum ActualSampleType
    {
        /// <summary>
        /// FirstBatchX_Y
        ///     第一批样品X~Y um的直径分布
        /// </summary>
        FirstBatch30_50                  =   0,
        FirstBatch50_70                  =   1,
        FirstBatch70_100                 =   2,

        /// <summary>
        /// FirstBatchX1_X2AndY1_Y2WithA_B
        ///     第一批样品X1~X2 um 与 Y1~Y2 um的直径的微珠按照A:B的比例混合
        /// </summary>
        FirstBatch30_50And50_70With1_1   =   10,
        FirstBatch30_50And50_70With1_3   =   11,
        FirstBatch30_50And50_70With3_1   =   12,

        FirstBatch30_50And70_100With1_1  =   20,
        FirstBatch30_50And70_100With1_3  =   21,
        FirstBatch30_50And70_100With3_1  =   22,
    }
}
