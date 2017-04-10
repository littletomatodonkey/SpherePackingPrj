using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace SpherePacking.MainWindow
{
    class GenerateRandomNumber
    {
        private static Random rnd = new Random((int)(DateTime.Now.Ticks / 10000));

        public static double AverageRandom(double min, double max)//产生(min,max)之间均匀分布的随机数
        {
            return min + rnd.NextDouble() * (max - min);
        }

        /// <summary>
        /// 返回一个符合(miu, sigma)的正态分布的随机数
        /// </summary>
        /// <param name="miu"></param>
        /// <param name="sigma"></param>
        /// <returns></returns>
        public static double NormalDistribution(double miu, double sigma)
        {
            double u1, u2, v1 = 0, v2 = 0, s = 0, z1 = 0;
            while (s > 1 || s == 0)
            {
                u1 = rnd.NextDouble();
                u2 = rnd.NextDouble();
                v1 = 2 * u1 - 1;
                v2 = 2 * u2 - 1;
                s = v1 * v1 + v2 * v2;
            }
            z1 = Math.Sqrt(-2 * Math.Log(s) / s) * v1;
            return z1*sigma+miu; //返回两个服从正态分布N(0,1)的随机数z0 和 z1
        }

        /// <summary>
        /// 产生随机正态正态分布的数
        /// log(N)符合正态分布
        /// 产生的随机数在
        /// </summary>
        /// <param name="miu">传入的对数正态分布的平均值,为log(N)的平均值</param>
        /// <param name="sigma">出入的对数正态分布的标准差,为log(N)的标准差</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public static double RandomLogNormal(double miu, double sigma, double min, double max)//产生对数正态分布随机数
        {
            double m = 0.0;
            do
            {
                m = NormalDistribution(miu, sigma);
                m = Math.Exp(m);
            }while( m < min || m > max );
            return m;
        }

        /// <summary>
        /// 对数正态分布测试
        /// </summary>
        public static void TestLogNormalNumber()
        {
            int num = 10000;
            double min = ActualSampleParameter.ActualSampleParaDict[PackingSystemSetting.ParticleSizeType].MinDiameter;
            double max = ActualSampleParameter.ActualSampleParaDict[PackingSystemSetting.ParticleSizeType].MaxDiameter;
            double mu = ActualSampleParameter.ActualSampleParaDict[PackingSystemSetting.ParticleSizeType].LogMiu;
            double sigma = ActualSampleParameter.ActualSampleParaDict[PackingSystemSetting.ParticleSizeType].LogSigma;
            Matrix<double> m = new Matrix<double>(num, 1);
            for (int i = 0; i < num; i++)
            {
                m[i, 0] = RandomLogNormal(mu, sigma, min, max);
            }

            MCvScalar s = CvInvoke.Mean(m);
            DataReadWriteHelper.RecordInfo("data.txt", @"D:\MyDesktop\", m * 1e6, false);
            Console.WriteLine( "random log normal number generation test : {0}", s.V0);

        }

        /// <summary>
        ///产生均匀随机分布的随机数测试
        /// </summary>
        public static void TestAverageRandomNumber()
        {
            int num = 10000;
            double min = 1e-5;
            double max = 3e-5;
            Matrix<double> m = new Matrix<double>(num, 1);
            for (int i = 0; i < num; i++)
            {
                m[i, 0] = GenerateRandomNumber.AverageRandom(min, max);
            }
            Emgu.CV.Structure.MCvScalar s = CvInvoke.Mean(m);
            Console.WriteLine("random average number generation: min is {0}, max is {1}, average is {2}", min, max, s.V0);
        }


    }

    /// <summary>
    /// 产生的随机数的类型
    /// </summary>
    enum RandomType
    {
        /// <summary>
        /// 均匀随机分布
        /// </summary>
        AverageRandomType = 0,

        /// <summary>
        /// 对数正态分布
        /// </summary>
        RandomLogNormalType = 1,

        /// <summary>
        /// 正态分布
        /// </summary>
        RandomNormalType = 2,
    }
}
