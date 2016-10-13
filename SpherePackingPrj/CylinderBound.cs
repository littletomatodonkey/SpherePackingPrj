using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;

namespace SpherePacking.MainWindow
{
    class CylinderBound:IBound
    {
        private double radius;

        private double height;

        private double minOverlap;

        public double Height
        {
            get { return this.height; }
            private set { this.height = value; }
        }

        public double Radius
        {
            get{return this.radius;}
            private set { this.radius = value; }
        }

        /// <summary>
        /// 系统的边界
        /// [rmax, minHeight, maxHeight]
        /// </summary>
        public Matrix<double> boundOfObj;

        public CylinderBound( double radius, double height, double overlap = 1e-2 )
        {
            this.radius = radius;
            this.height = height;
            this.minOverlap = overlap;
            this.boundOfObj = new Matrix<double>(1, 3);

            ComputeBounds();
        }

        /// <summary>
        /// 计算系统的边界
        /// </summary>
        public void ComputeBounds()
        {
            this.boundOfObj[0, 0] = radius + minOverlap;
            this.boundOfObj[0, 1] = 0 - minOverlap;
            this.boundOfObj[0, 2] = height + minOverlap;
        }


        public AcrossBoundType IsAcrossBound(Matrix<double> pos, double r)
        {
            AcrossBoundType bt = AcrossBoundType.None;


            return bt;
        }

        public bool IsAcrossBottom( Matrix<double> pos, double r )
        {
            return (pos[0, 2] - r) < boundOfObj[0,0];
        }

        public bool IsAcrossTop( Matrix<double> pos, double r )
        {
            return (pos[0, 2] + r) > boundOfObj[0, 2];
        }

        public bool IsAcrossCylinderSeufce( Matrix<double> pos, double r )
        {
            return (Math.Sqrt(pos[0, 0] * pos[0, 0] + pos[0, 1] * pos[0, 1]) + r) >= boundOfObj[0,1];
        }


    }
}
