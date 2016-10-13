using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpherePacking.MainWindow
{
    /// <summary>
    /// 烧结过程的模拟
    /// 使得小球可以由部分程度的重叠
    /// </summary>
    class SinteringHelper
    {
        /// <summary>
        /// 模拟烧结过程
        /// 将小球的半径按照rate的比例进行放大，使得小球之间有一定程度的相交
        /// </summary>
        /// <param name="balls"></param>
        /// <param name="rate"></param>
        public static void SinteringProcess(SpherePlot balls, double rate = 1.2 )
        {
            for(int i=0;i<balls.Spheres.Count();i++)
            {
                balls.SetRadius(balls.Spheres[i].GetRadius()*rate,i);
            }
        }
    }
}
