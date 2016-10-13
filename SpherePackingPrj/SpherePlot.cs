using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Kitware.VTK;

namespace SpherePacking.MainWindow
{
    class SpherePlot
    {
        private int numOfSpheres;

        /// <summary>
        /// z方向上的小球的个数与x、y方向上的小球个数之比
        /// </summary>
        private int zRate;

        /// <summary>
        /// 图像中存储的小球，可以直接对小球的半径以及位置等进行操作，再对控件进行刷新，即可实时更新小球位置及大小
        /// </summary>
        private vtkSphereSource[] spheres;

        /// <summary>
        /// 产生随机数
        /// </summary>
        private Random rnd = new Random();

        public vtkSphereSource[] Spheres
        {
            get
            {
                return this.spheres;
            }
        }

        /// <summary>
        /// 初始化SpherePlot对象
        /// </summary>
        /// <param name="num"></param>
        public SpherePlot(int num, int zrate)
        {
            this.numOfSpheres = num * num * num * zrate;
            this.spheres = new vtkSphereSource[numOfSpheres];
            this.zRate = zrate;
            int index = 0;

            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    for (int k = 0; k < zRate * num; k++)
                    {
                        spheres[index] = vtkSphereSource.New();
                        spheres[index].SetRadius(ComputeRandomRadius());

                        spheres[index].SetCenter(i + 0.3 + 0.4 * rnd.NextDouble(),
                                                j + 0.3 + 0.4 * rnd.NextDouble(),
                                                k + 0.3 + 0.4 * rnd.NextDouble());
                        //设置小球的可视化精度，精度越高，渲染越慢；精度越低，效果越差。
                        spheres[index].SetPhiResolution(20);
                        spheres[index++].SetThetaResolution(20);
                    }
                }
            }

            
        }

        public SpherePlot(double radius, double height)
        {
            this.numOfSpheres = (int)(3*radius*radius/4/Math.Pow(0.5,3));
            this.spheres = new vtkSphereSource[numOfSpheres];
            int index = 0;
            double h = 1.0;
            while(index < numOfSpheres)
            {
                int min = (int)(-radius/Math.Sqrt(2));
                int max = (int)(radius/Math.Sqrt(2));
                for(int i=min;i<=max;i+=2)
                {
                    for(int j=min;j<=max;j+=2)
                    {
                        spheres[index] = vtkSphereSource.New();
                        spheres[index].SetRadius(ComputeRandomRadius());
                        spheres[index].SetCenter( i+0.2*(rnd.NextDouble()-0.5), 
                                                  j+0.2*(rnd.NextDouble()-0.5),
                                                  h + 0.2 * (rnd.NextDouble() - 0.5));

                        spheres[index].SetPhiResolution(20);
                        spheres[index++].SetThetaResolution(20);

                        if(index >= numOfSpheres)
                        {
                            break;
                        }
                    }
                    if (index >= numOfSpheres)
                    {
                        break;
                    }
                }
                h = h + 1.4;
            }

        }

        /// <summary>
        /// 计算小球的半径
        /// 小球半径符合对数正态分布，之后需要修改这个函数
        /// </summary>
        /// <returns></returns>
        private double ComputeRandomRadius()
        {
            if (PackingSystemSetting.SystemBoundType == BoundType.CubeType)
                return (0.4 + rnd.NextDouble() * 0.2) / 1.5;
            else if (PackingSystemSetting.SystemBoundType == BoundType.CylinderType)
                return (0.3 + 0.4 * rnd.NextDouble());
            else
                return 0.0;
        }

        /// <summary>
        /// 在renderer上绘制小球
        /// </summary>
        /// <param name="renderer"></param>
        public void PlotSphereInRender( vtkRenderer renderer )
        {
            if (this.spheres == null)
                return;
            for (int i = 0; i < numOfSpheres;i++ )
            {
                vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
                mapper.SetInputConnection(spheres[i].GetOutputPort());
                vtkActor actor = vtkActor.New();
                actor.SetMapper(mapper);
                renderer.AddActor(actor);
            }
        }

        /// <summary>
        /// 设置小球的半径
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="index">小球的index</param>
        /// <returns></returns>
        public ErrorCode SetRadius( double radius, int index )
        {
            ErrorCode errorCode = ErrorCode.None;
            if (spheres.Count() - 1 < index)
            {
                errorCode = ErrorCode.IndexOutOfRange;
            }
            else
            {
                spheres[index].SetRadius(radius);
            }

            return errorCode;
        }

        /// <summary>
        /// 设置小球的球心和半径
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="radius"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public ErrorCode SetSphereProperity(double x, double y, double z, double radius, int index)
        {
            ErrorCode errorCode = ErrorCode.None;
            if( spheres.Count()-1 < index )
            {
                errorCode = ErrorCode.IndexOutOfRange;
            }
            else
            {
                spheres[index].SetCenter(x, y, z);
                spheres[index].SetRadius(radius);
            }
            return errorCode;
        }

        /// <summary>
        /// 设置小球的球心
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="index">小球的index</param>
        /// <returns></returns>
        public ErrorCode SetCenter(double x, double y, double z, int index)
        {
            ErrorCode errorCode = ErrorCode.None;
            if (spheres.Count() - 1 < index)
            {
                errorCode = ErrorCode.IndexOutOfRange;
            }
            else
            {
                spheres[index].SetCenter(x, y, z);
            }
            return errorCode;
        }

        /// <summary>
        /// 返回所有小球的距离矩阵
        /// 为二维矩阵
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private Matrix<double> ComputeDistanceMatrix()
        {
            Matrix<double> dists = new Matrix<double>(numOfSpheres, numOfSpheres);
            for (int i = 0; i < spheres.Length; i++)
            {
                for (int j = 0; j < spheres.Length; j++)
                {
                    dists[i, j] = Math.Sqrt(Math.Pow((spheres[i].GetCenter()[0] - spheres[j].GetCenter()[0]), 2) +
                                            Math.Pow((spheres[i].GetCenter()[1] - spheres[j].GetCenter()[1]), 2) +
                                            Math.Pow((spheres[i].GetCenter()[2] - spheres[j].GetCenter()[2]), 2));
                }
            }
            return dists;
        }

        /// <summary>
        /// 计算两个小球之间的距离
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public double ComputeDistanceTwoPoints(int i, int j)
        {
            if (i >= numOfSpheres || j >= numOfSpheres)
                return -1;
            return Math.Sqrt(Math.Pow((spheres[i].GetCenter()[0] - spheres[j].GetCenter()[0]), 2) +
                              Math.Pow((spheres[i].GetCenter()[1] - spheres[j].GetCenter()[1]), 2) +
                              Math.Pow((spheres[i].GetCenter()[2] - spheres[j].GetCenter()[2]), 2));
        }

        /// <summary>
        /// 判断两个小球是否相交
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public bool IsTwoSpheresIntersect(int i, int j)
        {
            //这里的Error还没有考虑进去
            if (i >= numOfSpheres || j >= numOfSpheres)
                return false;
            return Math.Sqrt(Math.Pow((spheres[i].GetCenter()[0] - spheres[j].GetCenter()[0]), 2) +
                              Math.Pow((spheres[i].GetCenter()[1] - spheres[j].GetCenter()[1]), 2) +
                              Math.Pow((spheres[i].GetCenter()[2] - spheres[j].GetCenter()[2]), 2))
                              < (spheres[i].GetRadius() + spheres[j].GetRadius());
        }

        /// <summary>
        /// 更新所有小球的半径
        /// </summary>
        public void UpdateSpheresRadius(Matrix<double> radius)
        {
            if (radius.Rows != spheres.Count())
            {
                Console.WriteLine(@"spheres' count is not equal with radius' count");
                return;
            }
            for (int i = 0; i < spheres.Count(); i++)
            {
                spheres[i].SetRadius(radius[i,0]);
            }

        }

        /// <summary>
        /// 更新所有小球的位置信息
        /// </summary>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        /// <param name="zPos"></param>
        public void UpdateSpheresLocation(Matrix<double> pos)
        {
            if( pos.Rows != spheres.Count() || pos.Cols != 3 )
            {
                Console.WriteLine(@"x or y or z's count is not equal with radius' count");
            }
            else
            {
                for (int i = 0; i < spheres.Count(); i++)
                {
                    spheres[i].SetCenter(pos[i, 0], pos[i, 1], pos[i,2]);
                }
            }
        }

        /// <summary>
        /// 从简化的模型中更新小球的半径和位置
        /// </summary>
        /// <param name="sModel"></param>
        public void ReloadInfo(SimpleModelForSave sModel)
        {
            UpdateSpheresLocation(sModel.RtPos);
            UpdateSpheresRadius(sModel.Radii);
        }
    }
}
