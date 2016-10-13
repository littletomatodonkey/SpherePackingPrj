using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;

namespace SpherePacking.MainWindow
{
    /// <summary>
    /// Discrete Element Methods(DEM)的模型
    /// </summary>
    class ModelDem3D
    {

        public delegate void RefreshWindowHandler(int iter, Matrix<double> m);

        public RefreshWindowHandler RefreshHandler;

        /// <summary>
        /// 迭代的采样时间，stepsize
        /// </summary>
        private const double deltaT = 4e-3; //para before is 4e-3

        /// <summary>
        /// 重力加速度
        /// </summary>
        private const double gravity = 9.8;

        /// <summary>
        /// 两个小球之间的嵌入的阈值
        /// 当其中一个小球移动后使得小球的相交距离大于这个值的时候，将两者的相交距离限制在该阈值
        /// 如果大于阈值，需要重新计算移动的向量
        /// </summary>
        private const double maxOverlapWithBall = 1e-1;

        /// <summary>
        /// 小球和墙之间的嵌入阈值
        /// 对小球求得的距离做限幅处理
        /// </summary>
        private double maxOverlapWithWall = 1e-1;

        /// <summary>
        /// 小球碰撞后相互之间产生的力的比例因子
        /// 力和小球之间的距离成正比
        /// 立方体边界时，值在1e1附近
        /// 圆柱体边界时，值在1e2附近
        /// 
        /// </summary>
        private double kns = 1e1;  //

        /// <summary>
        /// 小球和墙碰撞时产生的力的比例因子
        /// 在这里允许小球嵌入墙内一点，产生的力和嵌入的距离成正比
        /// 立方体边界时，值在1e2附近
        /// 圆柱体边界时，值在1e2附近
        /// </summary>
        private const double knw = 1e2;

        /// <summary>
        /// 小球膨胀后的速度衰减指数
        /// </summary>
        private const double velDecayRate = 1.0 / 2;

        /// <summary>
        /// 迭代的次数
        /// </summary>
        private const int iteration = 8000;

        /// <summary>
        /// 在每次迭代过程中求解出的孔隙率
        /// </summary>
        private Matrix<double> porosity;


        /// <summary>
        /// 小球的个数
        /// </summary>
        private int objNum;

        /// <summary>
        /// 维数
        /// 二维或三维
        /// </summary>
        private const int dim = 3;

        /// <summary>
        /// 小球的质量
        /// </summary>
        private Matrix<double> mass;

        private CubeBound cubeBound;

        private CylinderBound cylinderBound;

        /// <summary>
        /// 小球的半径矩阵
        /// </summary>
        private Matrix<double> radii;

        /// <summary>
        /// 小球的实时位置
        /// </summary>
        private Matrix<double> rtPos;

        /// <summary>
        /// 小球的实时速度
        /// </summary>
        private Matrix<double> rtVel;

        /// <summary>
        /// 小球的实时角加速度
        /// </summary>
        private Matrix<double> rtAcc;

        /// <summary>
        /// 用于产生随机数
        /// </summary>
        private Random random = new Random();

        /// <summary>
        /// 用于保存数据文件的文件夹
        /// </summary>
        private const string dirForSaveInfo = "./result/";

        /// <summary>
        /// 设置每一边的小球的个数
        /// 初始化小球的位置时会用到
        /// 容器的边长是这个rate倍
        /// </summary>
        private int nBase;

        #region 属性值
        public int ObjNum
        {
            get { return this.objNum; }
            private set { this.objNum = value; }
        }

        public Matrix<double> Radii
        {
            get { return this.radii; }
            private set { this.radii = value; }
        }

        public Matrix<double> RtPos
        {
            get { return this.rtPos; }
            private set { this.rtPos = value; }
        }

        public Matrix<double> RtVel
        {
            get { return this.rtVel; }
            private set { this.rtVel = value; }
        }

        public Matrix<double> RtAcc
        {
            get { return this.rtAcc; }
            private set { this.rtAcc = value; }
        }
        #endregion

        public ModelDem3D( int nbase, SpherePlot balls, SimpleModelForSave sModel = null )
        {
            this.objNum = balls.Spheres.Count();
            this.porosity = new Matrix<double>(iteration, 1);


            InitStatus(balls);

            if( PackingSystemSetting.SystemBoundType == BoundType.CubeType )
            {
                this.nBase = nbase;
                kns = 1e1;
                cubeBound = new CubeBound(nbase, nbase, nbase);
            }
            else if (PackingSystemSetting.SystemBoundType == BoundType.CylinderType)
            {
                kns = 1e2;
                cylinderBound = new CylinderBound(PackingSystemSetting.Radius, PackingSystemSetting.Height);
            }
            
        }

        /// <summary>
        /// 初始化模型
        /// 包括：计算小球的质量，并保存所有小球的初始位置和半径
        /// </summary>
        /// <param name="balls"></param>
        private void InitStatus(SpherePlot balls)
        {
            mass = new Matrix<double>(objNum, 1);
            radii = new Matrix<double>(objNum, 1);
            //Matrix<double>类初始化时都是0
            rtPos = new Matrix<double>(objNum, dim);
            rtVel = new Matrix<double>(objNum, dim);
            rtAcc = new Matrix<double>(objNum, dim);
            for (int i = 0; i < objNum; i++)
            {
                radii[i, 0] = balls.Spheres[i].GetRadius();
                mass[i, 0] = 4 / 3 * Math.PI * Math.Pow(radii[i, 0], 3);
                rtPos[i, 0] = balls.Spheres[i].GetCenter()[0];
                rtPos[i, 1] = balls.Spheres[i].GetCenter()[1];
                rtPos[i, 2] = balls.Spheres[i].GetCenter()[2];
            }

        }

        /// <summary>
        /// 迭代求解整个动态过程
        /// </summary>
        public void SolveProblem()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for(int i=0;i<iteration;i++)
            {
                ComputePos();
                ComputeBounds();
                ComputeVel();
                ComputeAcc();
                //ApplySaturationAcc();
                porosity[i, 0] = ComputePorosity(1e0);
                //if( i % 20 == 0 )
                if(true)
                {
                    RefreshHandler(i, rtPos);
                }
            }

            DataReadWriteHelper.RecordInfo("rePos" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt",dirForSaveInfo, rtPos);
            DataReadWriteHelper.RecordInfo("reVel" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt", dirForSaveInfo, rtVel);
            DataReadWriteHelper.RecordInfo("reAcc" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt", dirForSaveInfo, rtAcc);
            DataReadWriteHelper.RecordInfo("porosity" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt", dirForSaveInfo, porosity);
            DataReadWriteHelper.RecordInfo("radii" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt", dirForSaveInfo, radii);

            sw.Stop();

            Console.WriteLine( "Process Time : " + sw.ElapsedMilliseconds + "ms" );
        }

        /// <summary>
        /// 处理边界上的问题，对小球的位置进行限幅
        /// </summary>
        private void ComputeBounds()
        {
            for(int i=0;i<objNum;i++)
            {
                if (PackingSystemSetting.SystemBoundType == BoundType.CubeType)
                {
                    rtPos[i, 0] = (rtPos[i, 0] - radii[i, 0]) <= cubeBound.BoundOfObj[0, 0] ? cubeBound.BoundOfObj[0, 0] + radii[i, 0] : rtPos[i, 0];
                    rtPos[i, 0] = (rtPos[i, 0] + radii[i, 0]) >= cubeBound.BoundOfObj[0, 1] ? cubeBound.BoundOfObj[0, 1] - radii[i, 0] : rtPos[i, 0];
                    rtPos[i, 1] = (rtPos[i, 1] - radii[i, 0]) <= cubeBound.BoundOfObj[0, 2] ? cubeBound.BoundOfObj[0, 2] + radii[i, 0] : rtPos[i, 1];
                    rtPos[i, 1] = (rtPos[i, 1] + radii[i, 0]) >= cubeBound.BoundOfObj[0, 3] ? cubeBound.BoundOfObj[0, 3] - radii[i, 0] : rtPos[i, 1];
                    rtPos[i, 2] = (rtPos[i, 2] - radii[i, 0]) <= cubeBound.BoundOfObj[0, 4] ? cubeBound.BoundOfObj[0, 4] + radii[i, 0] : rtPos[i, 2];
                    //rtPos[i, 2] = (rtPos[i, 2] + radii[i, 0]) >= cubeBound.BoundOfObj[0, 5] ? cubeBound.BoundOfObj[0, 5] - radii[i, 0] : rtPos[i, 2];
                }
                else if (PackingSystemSetting.SystemBoundType == BoundType.CylinderType)
                {
                    double dx = CvInvoke.Norm(rtPos.GetRow(i).GetCols(0,2), Emgu.CV.CvEnum.NormType.L2);
                    double tan = Math.Atan2(rtPos[i,1],rtPos[i,0]);
                    if (dx+radii[i,0] > cylinderBound.boundOfObj[0, 0])
                    {
                        rtPos[i, 0] = (cylinderBound.boundOfObj[0, 0] - radii[i, 0]) * Math.Cos(tan);
                        rtPos[i, 1] = (cylinderBound.boundOfObj[0, 0] - radii[i, 0]) * Math.Sin(tan);
                    }
                    rtPos[i, 2] = (rtPos[i, 2] - radii[i, 0]) <= cylinderBound.boundOfObj[0, 1] ? cylinderBound.boundOfObj[0, 1] + radii[i, 0] : rtPos[i, 2];
                    //rtPos[i, 2] = (rtPos[i, 2] + radii[i, 0]) >= cylinderBound.boundOfObj[0, 2] ? cylinderBound.boundOfObj[0, 2] - radii[i, 0] : rtPos[i, 2];
                }
                
            }
        }

        /// <summary>
        /// 计算角实时加速度
        /// </summary>
        private void ComputeAcc()
        {
            Matrix<double> force = new Matrix<double>(objNum, dim);
            Matrix<double> m = new Matrix<double>(1, dim);
            if( false )
            {
                Parallel.For(0, objNum, (i) => 
                {
                    force[i, 2] = -mass[i, 0] * gravity;  //计算重力
                    //计算小球之间相互的力
                    for (int j = 0; j < objNum; j++)
                    {
                        if (j != i)
                        {
                            double dx = ComputeDx(i, j);
                            if (dx < radii[i, 0] + radii[j, 0])
                            {
                                m = kns * (rtPos.GetRow(i) - rtPos.GetRow(j));

                                for (int k = 0; k < dim; k++)
                                {
                                    force[i, k] += m[0, k];

                                    rtVel[i, k] = rtVel[i, k] * velDecayRate;
                                    rtVel[j, k] = rtVel[j, k] * velDecayRate;
                                }
                            }
                        }
                    }

                    //计算墙壁和小球之间的力
                    if (PackingSystemSetting.SystemBoundType == BoundType.CubeType)
                    {
                        if (rtPos[i, 0] - radii[i, 0] < 0)
                        {
                            force[i, 0] += knw * (-rtPos[i, 0] + radii[i, 0]);
                            rtVel[i, 0] = rtVel[i, 0] * velDecayRate;
                        }
                        if (rtPos[i, 0] + radii[i, 0] > nBase)
                        {
                            force[i, 0] += knw * (nBase - rtPos[i, 0] - radii[i, 0]);
                            rtVel[i, 0] = rtVel[i, 0] * velDecayRate;
                        }
                        if (rtPos[i, 1] - radii[i, 0] < 0)
                        {
                            force[i, 1] += knw * (-rtPos[i, 1] + radii[i, 0]);
                            rtVel[i, 1] = rtVel[i, 1] * velDecayRate;
                        }
                        if (rtPos[i, 1] + radii[i, 0] > nBase)
                        {
                            force[i, 1] += knw * (nBase - rtPos[i, 1] - radii[i, 0]);
                            rtVel[i, 1] = rtVel[i, 1] * velDecayRate;
                        }
                        if (rtPos[i, 2] - radii[i, 0] < 0)
                        {
                            force[i, 2] += knw * (-rtPos[i, 2] + radii[i, 0]);
                            rtVel[i, 2] = rtVel[i, 2] * velDecayRate;
                        }
                        //高度限幅
                        //if (rtPos[i, 2] + radii[i, 0] > objNum/nBase/nBase)
                        //{
                        //    force[i, 2] += knw * (nBase - rtPos[i, 2] - radii[i, 0]);
                        //    rtVel[i, 2] = rtVel[i, 2] * velDecayRate;
                        //}
                    }
                    else if (PackingSystemSetting.SystemBoundType == BoundType.CylinderType)
                    {
                        double dx = CvInvoke.Norm(rtPos.GetRow(i).GetCols(0, 1), Emgu.CV.CvEnum.NormType.L2);
                        double angle = Math.Atan2(rtPos[i, 1], rtPos[i, 0]);
                        double rotAngle = angle - Math.PI / 2;
                        double vx, vy;
                        if (dx + radii[i, 0] > cylinderBound.Radius)
                        {
                            force[i, 0] += Math.Cos(angle) * knw * (-dx - radii[i, 0] + cylinderBound.Radius);
                            force[i, 1] += Math.Sin(angle) * knw * (-dx - radii[i, 0] + cylinderBound.Radius);

                            //旋转坐标轴
                            vx = rtVel[i, 0] * Math.Cos(rotAngle) + rtVel[i, 1] * Math.Sin(rotAngle);
                            vy = -rtVel[i, 0] * Math.Sin(rotAngle) + rtVel[i, 1] * Math.Cos(rotAngle);

                            //圆心法线方向速度衰减
                            vy = vy * velDecayRate;

                            //将速度坐标轴变换回来
                            rtVel[i, 0] = vx * Math.Cos(rotAngle) - vy * Math.Sin(rotAngle);
                            rtVel[i, 1] = vx * Math.Sin(rotAngle) + vy * Math.Cos(rotAngle);
                        }

                        if (rtPos[i, 2] - radii[i, 0] < 0)
                        {
                            force[i, 2] += knw * (-rtPos[i, 2] + radii[i, 0]);
                            rtVel[i, 2] = rtVel[i, 2] * velDecayRate;
                        }
                        //高度限幅
                        //if (rtPos[i, 2] + radii[i, 0] > cylinderBound.Height)
                        //{
                        //    force[i, 2] += knw * (cylinderBound.Height - rtPos[i, 2] - radii[i, 0]);
                        //    rtVel[i, 2] = rtVel[i, 2] * velDecayRate;
                        //}

                    }

                    rtAcc[i, 0] = force[i, 0] / mass[i, 0];
                    rtAcc[i, 1] = force[i, 1] / mass[i, 0];
                    rtAcc[i, 2] = force[i, 2] / mass[i, 0];
 
                });
            }
            else
            {
                for (int i = 0; i < objNum; i++)
                {
                    force[i, 2] = -mass[i, 0] * gravity;  //计算重力
                    //计算小球之间相互的力
                    for (int j = 0; j < objNum; j++)
                    {
                        if (j != i)
                        {
                            double dx = ComputeDx(i, j);
                            if (dx < radii[i, 0] + radii[j, 0])
                            {
                                m = kns * (rtPos.GetRow(i) - rtPos.GetRow(j));

                                for (int k = 0; k < dim; k++)
                                {
                                    force[i, k] += m[0, k];

                                    rtVel[i, k] = rtVel[i, k] * velDecayRate;
                                    rtVel[j, k] = rtVel[j, k] * velDecayRate;
                                }
                            }
                        }
                    }

                    //计算墙壁和小球之间的力
                    if (PackingSystemSetting.SystemBoundType == BoundType.CubeType)
                    {
                        if (rtPos[i, 0] - radii[i, 0] < 0)
                        {
                            force[i, 0] += knw * (-rtPos[i, 0] + radii[i, 0]);
                            rtVel[i, 0] = rtVel[i, 0] * velDecayRate;
                        }
                        if (rtPos[i, 0] + radii[i, 0] > nBase)
                        {
                            force[i, 0] += knw * (nBase - rtPos[i, 0] - radii[i, 0]);
                            rtVel[i, 0] = rtVel[i, 0] * velDecayRate;
                        }
                        if (rtPos[i, 1] - radii[i, 0] < 0)
                        {
                            force[i, 1] += knw * (-rtPos[i, 1] + radii[i, 0]);
                            rtVel[i, 1] = rtVel[i, 1] * velDecayRate;
                        }
                        if (rtPos[i, 1] + radii[i, 0] > nBase)
                        {
                            force[i, 1] += knw * (nBase - rtPos[i, 1] - radii[i, 0]);
                            rtVel[i, 1] = rtVel[i, 1] * velDecayRate;
                        }
                        if (rtPos[i, 2] - radii[i, 0] < 0)
                        {
                            force[i, 2] += knw * (-rtPos[i, 2] + radii[i, 0]);
                            rtVel[i, 2] = rtVel[i, 2] * velDecayRate;
                        }
                        //高度限幅
                        //if (rtPos[i, 2] + radii[i, 0] > objNum/nBase/nBase)
                        //{
                        //    force[i, 2] += knw * (nBase - rtPos[i, 2] - radii[i, 0]);
                        //    rtVel[i, 2] = rtVel[i, 2] * velDecayRate;
                        //}
                    }
                    else if (PackingSystemSetting.SystemBoundType == BoundType.CylinderType)
                    {
                        double dx = CvInvoke.Norm(rtPos.GetRow(i).GetCols(0, 1), Emgu.CV.CvEnum.NormType.L2);
                        double angle = Math.Atan2(rtPos[i, 1], rtPos[i, 0]);
                        double rotAngle = angle - Math.PI / 2;
                        double vx, vy;
                        if (dx + radii[i, 0] > cylinderBound.Radius)
                        {
                            force[i, 0] += Math.Cos(angle) * knw * (-dx - radii[i, 0] + cylinderBound.Radius);
                            force[i, 1] += Math.Sin(angle) * knw * (-dx - radii[i, 0] + cylinderBound.Radius);

                            //旋转坐标轴
                            vx = rtVel[i, 0] * Math.Cos(rotAngle) + rtVel[i, 1] * Math.Sin(rotAngle);
                            vy = -rtVel[i, 0] * Math.Sin(rotAngle) + rtVel[i, 1] * Math.Cos(rotAngle);

                            //圆心法线方向速度衰减
                            vy = vy * velDecayRate;

                            //将速度坐标轴变换回来
                            rtVel[i, 0] = vx * Math.Cos(rotAngle) - vy * Math.Sin(rotAngle);
                            rtVel[i, 1] = vx * Math.Sin(rotAngle) + vy * Math.Cos(rotAngle);
                        }

                        if (rtPos[i, 2] - radii[i, 0] < 0)
                        {
                            force[i, 2] += knw * (-rtPos[i, 2] + radii[i, 0]);
                            rtVel[i, 2] = rtVel[i, 2] * velDecayRate;
                        }
                        //高度限幅
                        //if (rtPos[i, 2] + radii[i, 0] > cylinderBound.Height)
                        //{
                        //    force[i, 2] += knw * (cylinderBound.Height - rtPos[i, 2] - radii[i, 0]);
                        //    rtVel[i, 2] = rtVel[i, 2] * velDecayRate;
                        //}

                    }

                    rtAcc[i, 0] = force[i, 0] / mass[i, 0];
                    rtAcc[i, 1] = force[i, 1] / mass[i, 0];
                    rtAcc[i, 2] = force[i, 2] / mass[i, 0];
                }
            }
            


        }

        /// <summary>
        /// 计算实时速度
        /// </summary>
        private void ComputeVel()
        {
            if (PackingSystemSetting.IsParaCompute)
            {
                Parallel.For(0, objNum, (i) =>
                {
                    rtVel[i, 0] += rtAcc[i, 0] * deltaT;
                    rtVel[i, 1] += rtAcc[i, 1] * deltaT;
                    rtVel[i, 2] += rtAcc[i, 2] * deltaT;
                });
            }
            else
            {
                for (int i = 0; i < objNum; i++)
                {
                    rtVel[i, 0] += rtAcc[i, 0] * deltaT;
                    rtVel[i, 1] += rtAcc[i, 1] * deltaT;
                    rtVel[i, 2] += rtAcc[i, 2] * deltaT;
                }
            }
            
        }

        /// <summary>
        /// 计算实时位置
        /// </summary>
        private void ComputePos()
        {
            if( PackingSystemSetting.IsParaCompute )
            {
                Parallel.For(0, objNum, (i) =>
                    {
                        rtPos[i, 0] += rtVel[i, 0] * deltaT;
                        rtPos[i, 1] += rtVel[i, 1] * deltaT;
                        rtPos[i, 2] += rtVel[i, 2] * deltaT;
                    });
            }
            else
            {
                for (int i = 0; i < objNum; i++)
                {
                    rtPos[i, 0] += rtVel[i, 0] * deltaT;
                    rtPos[i, 1] += rtVel[i, 1] * deltaT;
                    rtPos[i, 2] += rtVel[i, 2] * deltaT;
                }
            }
            
        }

        /// <summary>
        /// 计算两个圆的圆心之间的距离
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        private double ComputeDx(int index1, int index2)
        {
            double dx = CvInvoke.Norm(rtPos.GetRow(index1) - rtPos.GetRow(index2), Emgu.CV.CvEnum.NormType.L2);

            return dx;
        }

        /// <summary>
        /// 求解孔隙率
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private double ComputePorosity( double eps )
        {
            ulong allNum = 1;
            ulong po = 0;

            switch( PackingSystemSetting.SystemBoundType )
            {
                case BoundType.CubeType:
                    allNum = (ulong)(cubeBound.XLength * cubeBound.YLength * cubeBound.Height / 2 / Math.Pow(eps, 3));
                    for (double i = 0; i < cubeBound.XLength;i+=eps )
                    {
                        for(double j=0;j<cubeBound.YLength;j+=eps)
                        {
                            for(double k=0;k<cubeBound.Height/2;k+=eps)
                            {
                                if( IsPointInSpheres(i,j,k) )
                                {
                                    po++;
                                }
                            }
                        }
                    }
                    break;
                case BoundType.CylinderType:
                    allNum = (ulong)(cylinderBound.Height * Math.PI * cylinderBound.Radius * cylinderBound.Radius / 2 / Math.Pow(eps,3));
                    for (double i = -cylinderBound.Radius; i < cylinderBound.Radius;i+=eps )
                    {
                        for(double j=-Math.Sqrt(cylinderBound.Radius*cylinderBound.Radius-i*i);j<Math.Sqrt(cylinderBound.Radius*cylinderBound.Radius-i*i);j++)
                        {
                            for(double k=0;k<cylinderBound.Height/2;k++)
                            {
                                if (true)
                                {
                                    po++;
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            return 1 - po * 1.0 / allNum;
        }

        /// <summary>
        /// 判断小球是否处于小球中
        /// 用于计算孔隙率
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public bool IsPointInSpheres( double x, double y, double z )
        {
            bool res = false;
            if(objNum != 0)
            {
                for(int i=0;i<objNum;i++)
                {
                    if( ((x-rtPos[i,0])*(x-rtPos[i,0]) + (y-rtPos[i,1])*(y-rtPos[i,1]) + (z-rtPos[i,2])*(z-rtPos[i,2]) ) < radii[i,0]*radii[i,0] )
                    {
                        res = true;
                        break;
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 对加速度限幅
        /// </summary>
        private void ApplySaturationAcc()
        {
            double maxG = 3 * gravity;
            for(int i=0;i<rtAcc.Rows;i++)
            {
                for(int j=0;j<rtAcc.Cols;j++)
                {
                    rtAcc[i, j] = rtAcc[i, j] < maxG ? rtAcc[i, j] : maxG;
                    rtAcc[i, j] = rtAcc[i, j] > -maxG ? rtAcc[i, j] : -maxG;
                }
            }
        }

        /// <summary>
        /// 更新小球的位置和半径信息
        /// 当重新从文件中导入小球信息以后需要更新
        /// </summary>
        /// <param name="balls"></param>
        public void UpdateBallInfo( SpherePlot balls )
        {
            for(int i=0;i<balls.Spheres.Count();i++)
            {
                radii[i, 0] = balls.Spheres[i].GetRadius();
                rtPos[i, 0] = balls.Spheres[i].GetCenter()[0];
                rtPos[i, 1] = balls.Spheres[i].GetCenter()[1];
                rtPos[i, 2] = balls.Spheres[i].GetCenter()[2];
            }
        }
    }

    /// <summary>
    /// 用于存储为json文件的简化DEM模型
    /// </summary>
    class SimpleModelForSave
    {
        /// <summary>
        /// 小球的半径
        /// </summary>
        public Matrix<double> Radii;

        /// <summary>
        /// 小球当前位置
        /// </summary>
        public Matrix<double> RtPos;

        /// <summary>
        /// 小球当前速度
        /// </summary>
        public Matrix<double> RtVel;

        /// <summary>
        /// 小球当前加速度
        /// </summary>
        public Matrix<double> RtAcc;

        public SimpleModelForSave( ModelDem3D model )
        {
            if( model != null )
            {
                this.Radii = model.Radii;
                this.RtPos = model.RtPos;
                this.RtVel = model.RtVel;
                this.RtAcc = model.RtAcc;
            }
            
        }
    }
}
