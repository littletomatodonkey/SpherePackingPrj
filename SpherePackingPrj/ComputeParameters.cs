using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace SpherePacking.MainWindow
{
    /// <summary>
    /// 计算图像切片系列的相关参数
    /// </summary>
    class ComputeParameters
    {
        /// <summary>
        /// 计算文件夹中的所有的bmp文件组成的图像序列的孔隙率
        /// 默认情况下，白色是孔隙相，黑色是固体相，此时不需要对图像进行反相
        /// example :
        ///         ComputeParameters.ComputePorosity(dlg.SelectedPath, ref porosity, true)
        /// </summary>
        /// <param name="folder">文件夹名称</param>
        /// <param name="porosity">孔隙率</param>
        /// <param name="needReverse">是否需要反相</param>
        /// <returns></returns>
        public static bool ComputePorosity(string folder, ref double porosity, bool needReverse = false)
        {
            bool res = true;
            porosity = 0.0;
            if( Directory.Exists(folder) )
            {
                var files = Directory.GetFiles(folder, "*.bmp");
                Matrix<double> pMat = new Matrix<double>(files.Count(), 1);
                Mat img;
                int index = 0;
                foreach (var file in files)
                {
                    img = CvInvoke.Imread(file, Emgu.CV.CvEnum.LoadImageType.Grayscale);
                    if (needReverse)
                    {
                        CvInvoke.BitwiseNot(img, img);
                    }
                    pMat[index++, 0] = CvInvoke.CountNonZero(img) * 1.0 / (img.Cols * img.Rows);
                    porosity += pMat[index - 1, 0];
                }
                porosity /= files.Count();
                DataReadWriteHelper.RecordInfo("porosity.txt", PackingSystemSetting.ResultDir, pMat, false);
            }
            else
            {
                res = false;
            }

            return res;
        }

        /// <summary>
        /// 计算文件名符合特定格式的文件名的图像序列的孔隙率
        /// 默认情况下，白色是孔隙相，黑色是固体相，此时不需要对图像进行反相
        /// example :
        ///         ComputeParameters.ComputePorosity(@".\result\", "{0:D4}.bmp", 10, 21, ref porosity);
        /// </summary>
        /// /// <param name="folder">文件夹名称</param>
        /// <param name="pattern">文件名的格式</param>
        /// <param name="startIndex">起始index</param>
        /// <param name="endIndex">末尾index</param>
        /// <param name="porosity">孔隙率</param>
        /// <param name="needReverse">是否需要反相</param>
        /// <returns></returns>
        public static bool ComputePorosity(string folder, string pattern, int startIndex, int endIndex, ref double porosity, bool needReverse = false)
        {
            bool res = true;
            string fn = "";
            porosity = 0.0;
            Matrix<double> pMat = new Matrix<double>(endIndex-startIndex+1, 1);
            Mat img;
            for (int i = startIndex; i <= endIndex;i++ )
            {
                fn = string.Format(folder + pattern, i);
                img = CvInvoke.Imread(fn, Emgu.CV.CvEnum.LoadImageType.Grayscale);
                if(img.IsEmpty)
                {
                    res = false;
                    break;
                }
                if( needReverse )
                {
                    CvInvoke.BitwiseNot( img, img);
                }
                pMat[i - startIndex, 0] = CvInvoke.CountNonZero(img) * 1.0 / (img.Cols * img.Rows);
                porosity += pMat[i - startIndex, 0];
            }
            if( res )
            {
                DataReadWriteHelper.RecordInfo("porosity.txt", PackingSystemSetting.ResultDir, pMat, false);
            }
            return res;
        }

        public static bool ComputeSurfaceArea()
        {
            bool res = true;


            return res;
        }
    }
}
