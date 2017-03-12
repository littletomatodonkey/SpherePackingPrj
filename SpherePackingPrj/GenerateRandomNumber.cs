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
            Matrix<double> m = new Matrix<double>(1, 1);
            MCvScalar mm = new MCvScalar(miu);
            MCvScalar ms = new MCvScalar(sigma);

            do
            {
                CvInvoke.Randn(m, mm, ms);
                //log(N)符合正态分布，因此需要对其取指数
                CvInvoke.Exp(m, m);
            } while (m[0, 0] < min || m[0, 0] > max);

            return m[0, 0];
        }

        /// <summary>
        /// 对数正态分布测试
        /// </summary>
        public static void TestLogNormalNumber()
        {
            int num = 10000;
            double min = 13.183e-6;
            double max = 91.201e-6;
            double mu = 3.534-6*Math.Log(10, Math.E);
            double sigma = 0.285571;
            Matrix<double> m = new Matrix<double>(num, 1);
            for (int i = 0; i < num; i++)
            {
                m[i, 0] = RandomLogNormal(mu, sigma, min, max);
            }

            MCvScalar s = CvInvoke.Mean(m);
            DataReadWriteHelper.RecordInfo("data.txt", @"D:\MyDesktop\", m, false);
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
