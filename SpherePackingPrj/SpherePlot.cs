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
        /// <summary>
        /// 小球的球心在每个网格点上的x、y、z每个单独方向上的最大偏移和最小偏移之差
        /// 小球的半径的最大值 Rmax = 0.5 - MaxCenterBias/2;
        /// example : 
        ///     网格点是(0,0,0)，则小球的球心的范围是(-0.1,-0.1,-0.1) ~ (0.1,0.1,0.1)
        ///     小球的半径的最大值是 0.4
        /// </summary>
        private const double MaxCenterBias = 0.2;

        /// <summary>
        /// 最大的半径
        /// </summary>
        public double MaxRadius { get { return 0.5 - MaxCenterBias / 2; } }
        
        /// <summary>
        /// 小球的半径的最大偏差（相对于0.5）
        /// 小球的半径范围是 [0.5-MaxRadiusBias ~ 0.5]*Rmax/0.5
        /// 这个是根据实际样片的参数进行修改的，因为
        ///     最小半径/最大半径 = 实际样片的最小半径/最大半径
        /// example :
        ///     假设小球的最大半径是0.4，则小球的半径的范围是 0.24 ~ 0.4
        /// </summary>
        private double MaxRadiusBias = 0.2;

        /// <summary>
        /// 图像中存储的小球，可以直接对小球的半径以及位置等进行操作，再对控件进行刷新，即可实时更新小球位置及大小
        /// </summary>
        private vtkSphereSource[] spheres;

        /// <summary>
        /// 产生随机数
        /// </summary>
        private Random rnd = new Random((int)(DateTime.Now.Ticks / 900));

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
        public SpherePlot()
        {
            InitializeSpheresPos();
        }


        /// <summary>
        /// 初始小球的个数
        /// 根据不同的系统边界条件进行小球位置和半径的初始化
        /// 产生的小球在圆柱体边界条件下时，位置十分紧密，但是在立方体情况下，小球的致密程度相似
        ///     newest method on 2017/03/25
        /// </summary>
        private void InitializeSpheresPos()
        {
            this.spheres = new vtkSphereSource[PackingSystemSetting.BallsNumber];
            int index = 0;
            int failCnt = 0;
            //在一个平面内某个小球最大插入失败的次数，如果一个小球在800个随机产生的位置均没有被插入，则该层被铺满
            int maxFailCnt = 800; 
            // 当前层的高度（设置之后插入到该层的小球的Z值）
            double thisLayerHeight = 0;
            bool canBeAdded = true;
            double rndR = 0.0, rndAngle = 0.0, rndXy = 0.0, x = 0.0, y = 0.0;
            //该层最大的半径值（之后设置高度时会用到）
            double thisLayerMaxRadius = 0.0;
            //当当前层某个小球被插入失败的次数达到最大失败次数时，设置当前层为满，并将产生的位置添加到小球中
            bool thisLayerFull = false;
            List<double> listX = new List<double>();
            List<double> listY = new List<double>();
            List<double> listR = new List<double>();
            while (index < PackingSystemSetting.BallsNumber)
            {
                for (int i = 0; i < listX.Count;i++ )
                {
                    spheres[index] = vtkSphereSource.New();
                    spheres[index].SetRadius(listR[i]);
                    spheres[index].SetCenter(listX[i], listY[i], thisLayerHeight);

                    spheres[index].SetPhiResolution(20);
                    spheres[index++].SetThetaResolution(20);

                    if (index >= PackingSystemSetting.BallsNumber)
                        break;
                }
                //为确保小球之间互相不重叠，这个值需要被设置为thisLayerMaxRadius，但是容易出现很大的孔隙
                //  在这里设置为thisLayerMaxRadius/2，小球尽可能保持不重叠的基础上尽可能地致密
                thisLayerHeight += thisLayerMaxRadius / 2;

                listX.Clear();
                listY.Clear();
                listR.Clear();
                thisLayerFull = false;
                while( !thisLayerFull )
                {
                    failCnt = 0;
                    rndR = ComputeRandomRadius(RandomType.RandomLogNormalType);
                    if( PackingSystemSetting.SystemBoundType == BoundType.CylinderType )
                    {
                        rndXy = rnd.NextDouble() * (PackingSystemSetting.Radius - rndR);
                        rndAngle = rnd.NextDouble() * 2 * Math.PI;
                    }
                    while (failCnt < maxFailCnt)
                    {
                        canBeAdded = true;
                        if( PackingSystemSetting.SystemBoundType == BoundType.CubeType )
                        {
                            x = rndR + rnd.NextDouble() * (PackingSystemSetting.CubeLength - 2 * rndR);
                            y = rndR + rnd.NextDouble() * (PackingSystemSetting.CubeLength  - 2 * rndR);
                        }
                        else if( PackingSystemSetting.SystemBoundType == BoundType.CylinderType )
                        {
                            x = rndXy * Math.Cos(rndAngle);
                            y = rndXy * Math.Sin(rndAngle);
                        }
                        
                        for (int i = 0; i < listX.Count; i++)
                        {
                            if ((listX[i] - x) * (listX[i] - x) + (listY[i] - y) * (listY[i] - y) < (listR[i] + rndR) * (listR[i] + rndR))
                            {
                                canBeAdded = false;
                                break;
                            }
                        }
                        // 只有小球被添加到该层中时，才会重新生成随机的半径，否则添加的小球的半径会偏小
                        if (canBeAdded)
                        {
                            listX.Add(x);
                            listY.Add(y);
                            listR.Add(rndR);
                            thisLayerMaxRadius = Math.Max(thisLayerMaxRadius, rndR);
                            rndR = ComputeRandomRadius(RandomType.RandomLogNormalType);
                            failCnt = 0;
                        }
                        else
                        {
                            failCnt++;
                            if( PackingSystemSetting.SystemBoundType == BoundType.CylinderType )
                            {
                                rndXy = rnd.NextDouble() * (PackingSystemSetting.Radius - rndR);
                                rndAngle = rnd.NextDouble() * 2 * Math.PI;
                            }
                        }
                    }
                    if (failCnt >= maxFailCnt)
                    {
                        thisLayerFull = true;
                    }
                }
                //当前层的高度修正
                thisLayerHeight += thisLayerMaxRadius;
            }
        }

        /// <summary>
        /// 初始化小球的位置和半径
        ///     original method ----- 产生的小球的位置十分松散，耗费了大量的初始迭代时间
        /// </summary>
        /// <param name="len">立方体的边长的一半</param>
        /// <param name="offset">对于原圆柱体，底面圆心为零位；对于立方体，底面左下点为零位</param>
        private void InitializeSpheres(double len, double offset)
        {
            this.spheres = new vtkSphereSource[PackingSystemSetting.BallsNumber];
            int index = 0;
            double h = 0;
            double min = -len;
            double max = len;
            double step = 0.5;
            if( PackingSystemSetting.SystemBoundType == BoundType.CubeType )
            {
                max = PackingSystemSetting.CubeLength + min;
            }

            while (index < PackingSystemSetting.BallsNumber)
            {
                for (double i = min + offset; i < max + offset; i += step)
                {
                    for (double j = min + offset; j < max + offset; j += step)
                    {
                        spheres[index] = vtkSphereSource.New();
                        spheres[index].SetRadius(ComputeRandomRadius(RandomType.RandomLogNormalType));
                        spheres[index].SetCenter(i + MaxCenterBias / 2 * (rnd.NextDouble() - 0.5) + step/2, 
                                                 j + MaxCenterBias / 2 * (rnd.NextDouble() - 0.5) + step/2, 
                                                 h + step/2);
                        
                        spheres[index].SetPhiResolution(20);
                        spheres[index++].SetThetaResolution(20);

                        if (index >= PackingSystemSetting.BallsNumber)
                        {
                            break;
                        }
                    }
                    if (index >= PackingSystemSetting.BallsNumber)
                    {
                        break;
                    }
                }
                h = h + step;
            }
        }

        /// <summary>
        /// 计算小球的半径
        /// 小球的半径的最大值 Rmax = 0.5 - MaxCenterBias/2, 确保在一个单位1的立方体内不会重叠
        /// 此处设置的是均匀随机分布，小球半径符合还可能符合对数正态分布等
        /// </summary>
        /// <returns></returns>
        private double ComputeRandomRadius(RandomType type)
        {
            //小球的半径是：[0.5-MaxRadiusBias ~ 0.5]*Rmax/0.5
            double radius = 0.0;
            switch( type )
            {
                case RandomType.AverageRandomType:
                    radius = GenerateRandomNumber.AverageRandom((0.5 - MaxCenterBias) * (0.5 - MaxCenterBias / 2) / 0.5, 0.5 * (0.5 - MaxCenterBias / 2)/0.5);
                    break;
                case RandomType.RandomLogNormalType:
                    //第一批样品30~50um的的参数
                    double maxD = ActualSampleParameter.ActualSampleParaDict[PackingSystemSetting.ParticleSizeType].MaxDiameter;
                    double minD = ActualSampleParameter.ActualSampleParaDict[PackingSystemSetting.ParticleSizeType].MinDiameter;
                    double sigma = ActualSampleParameter.ActualSampleParaDict[PackingSystemSetting.ParticleSizeType].LogSigma;
                    double miu = ActualSampleParameter.ActualSampleParaDict[PackingSystemSetting.ParticleSizeType].LogMiu;
                    MaxRadiusBias = (1 - minD / maxD) / 2;

                    double reso = MaxRadius / (maxD / 2);
                    radius = GenerateRandomNumber.RandomLogNormal(miu, 
                                                                  sigma, 
                                                                  minD,
                                                                  maxD) / 2;
                    radius = radius * reso;
                    
                    break;
                case RandomType.RandomNormalType:
                    break;
            }
            return radius;
        }

        /// <summary>
        /// 在renderer上绘制小球
        /// </summary>
        /// <param name="renderer"></param>
        public void PlotSphereInRender( vtkRenderer renderer, ref vtkActorCollection actors )
        {
            if (this.spheres == null)
                return;
            for (int i = 0; i < PackingSystemSetting.BallsNumber; i++)
            {
                vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
                mapper.SetInputConnection(spheres[i].GetOutputPort());
                vtkActor actor = vtkActor.New();
                actor.SetMapper(mapper);
                actors.AddItem( actor );
                //renderer.AddActor(actor);
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
            Matrix<double> dists = new Matrix<double>(PackingSystemSetting.BallsNumber, PackingSystemSetting.BallsNumber);
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
            if (i >= PackingSystemSetting.BallsNumber || j >= PackingSystemSetting.BallsNumber)
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
            if (i >= PackingSystemSetting.BallsNumber || j >= PackingSystemSetting.BallsNumber)
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
