using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;

namespace SpherePacking.MainWindow
{
    class CubeBound : IBound
    {
        private double xLength;

        private double yLength;

        private double height;

        /// <summary>
        /// 
        /// </summary>
        private const double minOverlap = 1e-2;

        /// <summary>
        /// 小球在运动过程中的边界
        /// [xmin, xmax, ymin, ymax, zmin, zmax]
        /// </summary>
        private Matrix<double> boundOfObj;

        public Matrix<double> BoundOfObj
        {
            get
            {
                return this.boundOfObj;
            }
        }

        /// <summary>
        /// Xlength属性
        /// </summary>
        public double XLength
        {
            get
            {
                return this.xLength;
            }
            private set
            {
                this.xLength = value;
            }
        }

        /// <summary>
        /// ylength属性
        /// </summary>
        public double YLength
        {
            get
            {
                return this.yLength;
            }
            private set
            {
                this.yLength = value;
            }
        }

        /// <summary>
        /// height属性
        /// </summary>
        public double Height
        {
            get
            {
                return this.height;
            }
            private set
            {
                this.height = value;
            }
        }

        public CubeBound( double xLen, double yLen, double height )
        {
            this.xLength    = xLen;
            this.yLength    = yLen;
            this.height     = height;
            this.boundOfObj = new Matrix<double>(1, 6);

            ComputeBounds();
        }

        /// <summary>
        /// 计算系统的边界
        /// </summary>
        public void ComputeBounds()
        {
            boundOfObj[0, 0] = 0 - minOverlap;
            boundOfObj[0, 1] = xLength + minOverlap;
            boundOfObj[0, 2] = 0 - minOverlap;
            boundOfObj[0, 3] = yLength + minOverlap;
            boundOfObj[0, 4] = 0 - minOverlap;
            boundOfObj[0, 5] = height + minOverlap;
        }


        /// <summary>
        /// 判断是否穿过边界
        /// 问题：X会影响Z方向的相交的判断
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public AcrossBoundType IsAcrossBound(Matrix<double> pos, double r)
        {
            AcrossBoundType bt = AcrossBoundType.None;

            //if (pos[0,0] - r < boundOfObj[0, 0])
            //    bt = AcrossBoundType.LessThanXmin;
            //else if (pos[0,0] + r > boundOfObj[0, 0])
            //    bt = AcrossBoundType.MoreThanXmax;
            //else if (pos[0, 1] - r < boundOfObj[0, 1])
            //    bt = AcrossBoundType.LessThanYmin;
            //else if (pos[0, 1] + r > boundOfObj[0, 1])
            //    bt = AcrossBoundType.MoreThanYmax;
            //else if (pos[0, 2] - r < boundOfObj[0, 1])
            //    bt = AcrossBoundType.LessThanZmin;
            //else if (pos[0, 2] + r > boundOfObj[0, 2])
            //    bt = AcrossBoundType.MoreThanZmax;


            return bt;
        }
    }
}
