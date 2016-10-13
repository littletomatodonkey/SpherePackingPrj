using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;

namespace SpherePacking.MainWindow
{
    /// <summary>
    /// 系统的边界interface
    /// </summary>
    interface IBound
    {
        void ComputeBounds();

        /// <summary>
        /// 判断小球是否和物体相交
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        AcrossBoundType IsAcrossBound(Matrix<double> pos, double r);

    }

    /// <summary>
    /// 和边界相交的类型
    /// </summary>
    enum AcrossBoundType
    {
        /// <summary>
        /// 没有和边界
        /// </summary>
        None             = 0,

        LessThanXmin     = 1,
        MoreThanXmax     = 2,
        LessThanYmin     = 3,
        MoreThanYmax     = 4,
        LessThanZmin     = 5,
        MoreThanZmax     = 6,

        /// <summary>
        /// 在圆柱中，小球和四周的边界相交时情形
        /// </summary>
        OutOfRadius      = 7,


        MoreThanHeight   = 8,
    }

    /// <summary>
    /// 边界的类型
    /// </summary>
    enum BoundType
    {
        CubeType = 0,

        CylinderType = 1,
    }
}
