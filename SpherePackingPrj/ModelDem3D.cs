﻿using System;
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
        /// Here is the once-per-class call to initialize the log object
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 迭代的采样时间，stepsize
        /// 最小之前设置为1e-4
        /// </summary>
        private double DeltaT
        {
            get { return PackingSystemSetting.StepLength; }
        }

        /// <summary>
        /// 重力加速度
        /// </summary>
        private const double gravity = 9.8;

        /// <summary>
        /// 两个小球之间的嵌入的阈值
        /// 当其中一个小球移动后使得小球的相交距离大于这个值的时候，将两者的相交距离限制在该阈值
        /// 如果大于阈值，需要重新计算移动的向量
        /// </summary>
        private const double maxOverlapWithBall = 10000;

        /// <summary>
        /// 小球和墙之间的嵌入阈值
        /// 对小球求得的距离做限幅处理
        /// </summary>
        private const double maxOverlapWithWall = 1e-1;

        /// <summary>
        /// 小球碰撞后相互之间产生的力的比例因子
        /// 力和小球之间的距离成正比
        /// 
        /// </summary>
        // private double kns = 5e3;  //
        private double kns = 9.8e3;  //

        /// <summary>
        /// 小球和墙碰撞时产生的力的比例因子
        /// 在这里允许小球嵌入墙内一点，产生的力和嵌入的距离成正比
        /// </summary>
        //private const double knw = 2e3 * 1.25*1.25*1.25;
        private const double knw = 3.9e3;

        /// <summary>
        /// 小球碰撞后的速度衰减指数
        /// </summary>
        private const double velDecayRate = 0.9;

        /// <summary>
        /// 小球在仿真过程中的最大加速度
        /// 用于限幅
        /// </summary>
        private double MaxAcc { get { return 40 * gravity; } }

        /// <summary>
        /// 小球在仿真过程中的最大速度
        /// 用于限幅
        /// </summary>
        private double MaxVel = 20;

        #region 局部小球碰撞统计信息的相关变量
        /// <summary>
        /// 更新附近小球的迭代周期————每隔这么多代重新计算一次每个小球的附近的小球的下标
        /// </summary>
        private int updateLocalBallsIndexIter = 100;
        
        /// <summary>
        /// 小球附近的小球的信息
        /// 在计算小球是否和其他小球发生碰撞时，只需要计算这些下标对应的小球即可。
        /// </summary>
        private List<List<int>> localBallsIndex;

        /// <summary>
        /// 判定是否在最近的需要进行碰撞计算的小球列表中的阈值
        /// 如果两个小球的球面的最短距离(d-r1-r2)小于该阈值，则在小球的下标被存储到列表中
        /// </summary>
        private double LocalBallsDistThreshold
        {
            get
            {
                return 2 * MaxVel * updateLocalBallsIndexIter * DeltaT;
            }
        }

        #endregion
        /// <summary>
        /// 迭代的次数
        /// </summary>
        private int iteration
        {
            get
            {
                return PackingSystemSetting.IterationNum;
            }
        }

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
        /// 系统的能量(动能+重力势能)
        /// </summary>
        private Matrix<double> energy;

        /// <summary>
        /// 当前处于的迭代次数
        /// 保存当前的迭代次数，方便重新导入模型时使用
        /// </summary>
        private int currIter = 0;

        /// <summary>
        /// 小球三个方向上的速度是否需要衰减
        /// </summary>
        private Matrix<double> shouldVelBeDecayed;


        /// <summary>
        /// 两两小球之间的距离矩阵，大小为onjNum X objNum
        ///     如果用这种存储所有距离数据的方法去计算小球的距离，小球个数大于3000多时，会内存溢出=·=
        /// </summary>
        private Matrix<double> distances;

        /// <summary>
        /// 用于产生随机数
        /// </summary>
        private Random random = new Random();

        #region 属性值
        public int ObjNum
        {
            get { return this.objNum; }
            //private set { this.objNum = value; }
        }

        public Matrix<double> Radii
        {
            get { return this.radii; }
            //private set { this.radii = value; }
        }

        public Matrix<double> RtPos
        {
            get { return this.rtPos; }
            //private set { this.rtPos = value; }
        }

        public Matrix<double> RtVel
        {
            get { return this.rtVel; }
            //private set { this.rtVel = value; }
        }

        public Matrix<double> RtAcc
        {
            get { return this.rtAcc; }
            //private set { this.rtAcc = value; }
        }

        public Matrix<double> Energy
        {
            get { return this.energy; }
            //private set { this.energy = value; }
        }

        public int CurrIter
        {
            get { return currIter; }
        }
        #endregion

        public ModelDem3D(SpherePlot balls, SimpleModelForSave sModel = null )
        {
            this.objNum = balls.Spheres.Count();
            this.porosity = new Matrix<double>(iteration, 1);

            InitStatus(balls);

            if( PackingSystemSetting.SystemBoundType == BoundType.CubeType )
            {
                //kns = 1e3;
                cubeBound = new CubeBound(PackingSystemSetting.CubeLength, PackingSystemSetting.CubeLength, PackingSystemSetting.CubeLength);
            }
            else if (PackingSystemSetting.SystemBoundType == BoundType.CylinderType)
            {
                //kns = 1e3;
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
            distances = new Matrix<double>(objNum, objNum);
            energy = new Matrix<double>(iteration, 1);
            for (int i = 0; i < objNum; i++)
            {
                radii[i, 0] = balls.Spheres[i].GetRadius();
                mass[i, 0] = 4 / 3 * Math.PI * Math.Pow(radii[i, 0], 3);
                rtPos[i, 0] = balls.Spheres[i].GetCenter()[0];
                rtPos[i, 1] = balls.Spheres[i].GetCenter()[1];
                rtPos[i, 2] = balls.Spheres[i].GetCenter()[2];

                //给每个小球一个初始化向下的速度
                rtVel[i, 2] = -MaxVel;
            }
            localBallsIndex = new List<List<int>>();
            for (int i = 0; i < objNum; i++)
                localBallsIndex.Add(new List<int>());
            shouldVelBeDecayed = new Matrix<double>(objNum, dim);
        }

        /// <summary>
        /// 迭代求解整个动态过程
        /// </summary>
        public void SolveProblem()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            // find max velocity and acceleration of all balls
            double maxV = 0, maxA = 0;
            for (int j = 0; j < objNum; j++)
            {
                if (maxV <= CvInvoke.Norm(rtVel.GetRow(j), Emgu.CV.CvEnum.NormType.L2))
                    maxV = CvInvoke.Norm(rtVel.GetRow(j), Emgu.CV.CvEnum.NormType.L2);
                if (maxA <= CvInvoke.Norm(rtAcc.GetRow(j), Emgu.CV.CvEnum.NormType.L2))
                    maxA = CvInvoke.Norm(rtAcc.GetRow(j), Emgu.CV.CvEnum.NormType.L2);
            }
            MaxVel = maxV;

            CuteTools.ComputeMatDist(rtPos, ref distances);
            //开始求解时，会先找出所有小球的附近小球的下标
            UpdateLocalBallsIndex();

            //Stopwatch siter = new Stopwatch();
            for(;currIter<iteration;currIter++)
            {
                //siter.Restart();
                CuteTools.ComputeMatDist(rtPos, ref distances);
                //Console.WriteLine( "compute matreix dist: " + siter.ElapsedMilliseconds );
                //每隔100次，更新一下小球的附近小球的下标
                if( currIter % 100 == 0 )
                {
                    //siter.Restart();
                    UpdateLocalBallsIndex();
                    //Console.WriteLine("update local balls index: " + siter.ElapsedMilliseconds);
                }
                shouldVelBeDecayed.SetValue(0);
                //siter.Restart();
                ComputeAcc(currIter);
                //Console.WriteLine("compute acc: " + siter.ElapsedMilliseconds);
                //siter.Restart();
                ComputeVel();
                //Console.WriteLine("compute vel: " + siter.ElapsedMilliseconds);
                //siter.Restart();
                ComputePos();
                //Console.WriteLine("compute pos: " + siter.ElapsedMilliseconds);
                //ComputeBounds();

                //计算最大速度与加速度
                maxV = 0; maxA = 0;
                for (int j = 0; j < objNum; j++)
                {
                    if (maxV <= CvInvoke.Norm(rtVel.GetRow(j), Emgu.CV.CvEnum.NormType.L2))
                        maxV = CvInvoke.Norm(rtVel.GetRow(j), Emgu.CV.CvEnum.NormType.L2);
                    if (maxA <= CvInvoke.Norm(rtAcc.GetRow(j), Emgu.CV.CvEnum.NormType.L2))
                        maxA = CvInvoke.Norm(rtAcc.GetRow(j), Emgu.CV.CvEnum.NormType.L2);
                }
                MaxVel = maxV;
                
                //siter.Restart();
                energy[currIter, 0] = ComputeEnergy();
                //Console.WriteLine("compute energy: " + siter.ElapsedMilliseconds);
                string s = String.Format("current iteration : {0:D4}, system energy : {1:F4}, elapsed time : {2} ms, max Vel is: {3}, max Acc is {4} .", 
                                    currIter, energy[currIter,0], sw.ElapsedMilliseconds, maxV, maxA );
                log.Info( s );
                //if( i % 20 == 0 )
                if(true)
                {
                    RefreshHandler(currIter, rtPos);
                }
            }

            DataReadWriteHelper.RecordInfo("rePos" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt",PackingSystemSetting.ResultDir, rtPos * PackingSystemSetting.ResolutionUmPerSysUnit);
            DataReadWriteHelper.RecordInfo("reVel" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt", PackingSystemSetting.ResultDir, rtVel);
            DataReadWriteHelper.RecordInfo("reAcc" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt", PackingSystemSetting.ResultDir, rtAcc);
            DataReadWriteHelper.RecordInfo("radii" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt", PackingSystemSetting.ResultDir, radii * PackingSystemSetting.ResolutionUmPerSysUnit);
            DataReadWriteHelper.RecordInfo("energy" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt", PackingSystemSetting.ResultDir, energy);

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
        /// 计算实时加速度
        /// </summary>
        private void ComputeAcc(int iter = -1)
        {
            Matrix<double> force = new Matrix<double>(objNum, dim);
            
            //计算加速度时采用并行计算会蜜汁慢速，所以在此直接就用普通的方法了[捂脸]
            //if( PackingSystemSetting.IsParaCompute )
            if(false)
            {
                Parallel.For(0, objNum, (i) =>
                {
                    ComputeAccForBall(i,  ref force);
                });
            }
            else
            {
                for (int i = 0; i < objNum; i++)
                {
                    ComputeAccForBall(i, ref force, iter);
                }
                //ApplySaturationAcc();
            }
            
        }

        /// <summary>
        /// 更新每个小球附近的小球的下标
        /// 在每次迭代中会判断这些小球是否与该小球相交
        /// 如果小球12和小球13相近，则只记录12的附近有13
        /// </summary>
        private void UpdateLocalBallsIndex()
        {
            for (int index = 0; index < objNum;index++ )
            {
                localBallsIndex[index].Clear();
                for (int j = index + 1; j < objNum; j++)
                {
                    //double dx = ComputeDx(index, j) - radii[j, 0] - radii[index, 0];
                    double dx = distances[j, index] - radii[j, 0] - radii[index, 0];
                    if (dx < LocalBallsDistThreshold)
                    {
                        localBallsIndex[index].Add(j);
                    }
                }
            }
        }
        
        /// <summary>
        /// 计算下标为index的小球的加速度
        /// iter = -1 采用全部遍历的方法求解距离
        /// iter >= 0 找到与小球相距小于一定阈值的小球的下标，只对这些小球求解距离
        /// </summary>
        /// <param name="index"></param>
        private void ComputeAccForBall(int index, ref Matrix<double> force, int iter = -1)
        {
            Matrix<double> m = new Matrix<double>(1, dim);
            force[index, 2] = -mass[index, 0] * gravity;  //计算重力
            
            //计算小球之间相互的力
            //计算所有小球距离
            if( iter == -1 )
            {
                //全部求取的情况
                //for (int j = 0; j < objNum;j++ )
                //{
                //    if( j != index )
                //    {
                //        double dx = ComputeDx( index, j );
                //        if( dx < radii[j,0]+radii[index,0] )
                //        {
                //            m = kns * (rtPos.GetRow(index) - rtPos.GetRow(j));
                //            for(int k=0;k<dim;k++)
                //            {
                //                force[index, k] += m[0, k];

                //                rtVel[index, k] = rtVel[index, k] * velDecayRate;
                //                rtVel[j, k] = rtVel[j, k] * velDecayRate;
                //            }
                //        }
                //    }
                //}

                //对于每个小球，当它与后面的小球相交时，则将后面的小球的受力情况也更新一下
                //由N*N的时间复杂度变为N*N/2，数量级没变，但是减少了几乎一半的运行时间
                for (int j = index + 1; j < objNum; j++)
                {
                    //double dx = ComputeDx(index, j) - radii[index, 0] - radii[j, 0];  // 
                    double dx = distances[index, j] - radii[index, 0] - radii[j, 0];  // can be used if distance info be recorded
                    if (dx < 0)
                    {
                        m = rtPos.GetRow(index) - rtPos.GetRow(j);
                        m = -dx * kns * m / CvInvoke.Norm(m, Emgu.CV.CvEnum.NormType.L2);
                        for (int k = 0; k < dim; k++)
                        {
                            force[index, k] += m[0, k];
                            force[j, k] -= m[0, k];

                            shouldVelBeDecayed[index, k] = 1;
                            shouldVelBeDecayed[j, k] = 1;
                        }
                    }
                }
            }
            //存储小球附近的小球信息，不去计算那些在短时间不会碰撞的小球的距离
            else
            {
                //for (int j = index + 1; j < objNum; j++)
                //{
                //    double dx = ComputeDx(index, j);
                //    if (dx < radii[index, 0] + radii[j, 0])
                //    {
                //        if( !localBallsIndex[index].Contains(j) )
                //        {
                //            string text = string.Format("distance threshold is {0}, now {1} and {2}'s dist is {3}, but was not considered...", LocalBallsDistThreshold, index, j, dx - (radii[index, 0] + radii[j, 0]));
                //            log.Info(text);
                //            Console.WriteLine(text);
                //        }
                //        m = kns * (rtPos.GetRow(index) - rtPos.GetRow(j));
                //        for (int k = 0; k < dim; k++)
                //        {
                //            force[index, k] += m[0, k];
                //            force[j, k] -= m[0, k];

                //            shouldBeComputed[index, k] = 1;
                //            shouldBeComputed[j, k] = 1;
                //        }

                //    }
                //}

                for (int j = 0; j < localBallsIndex[index].Count; j++)
                {
                //double dx = ComputeDx(index, localBallsIndex[index][j]) - radii[index, 0] - radii[localBallsIndex[index][j], 0];
                double dx = distances[index, localBallsIndex[index][j]] - radii[index, 0] - radii[localBallsIndex[index][j], 0];
                if (dx < 0)
                {
                        m = rtPos.GetRow(index) - rtPos.GetRow(localBallsIndex[index][j]);
                        m = -dx * kns * m / CvInvoke.Norm(m, Emgu.CV.CvEnum.NormType.L2);

                        for (int k = 0; k < dim; k++)
                        {
                            force[index, k] += m[0, k];
                            force[localBallsIndex[index][j], k] -= m[0, k];

                            shouldVelBeDecayed[index, k] = 1;
                            shouldVelBeDecayed[localBallsIndex[index][j], k] = 1;                            
                        }
                    }
                }

            }

            //计算墙壁和小球之间的力
            if (PackingSystemSetting.SystemBoundType == BoundType.CubeType)
            {
                if (rtPos[index, 0] - radii[index, 0] < 0)
                {
                    force[index, 0] += knw * (-rtPos[index, 0] + radii[index, 0]);
                    shouldVelBeDecayed[index, 0] = 1;
                }
                if (rtPos[index, 0] + radii[index, 0] > PackingSystemSetting.CubeLength)
                {
                    force[index, 0] += knw * (PackingSystemSetting.CubeLength - rtPos[index, 0] - radii[index, 0]);
                    shouldVelBeDecayed[index, 0] = 1;
                }
                if (rtPos[index, 1] - radii[index, 0] < 0)
                {
                    force[index, 1] += knw * (-rtPos[index, 1] + radii[index, 0]);
                    shouldVelBeDecayed[index, 1] = 1;
                }
                if (rtPos[index, 1] + radii[index, 0] > PackingSystemSetting.CubeLength)
                {
                    force[index, 1] += knw * (PackingSystemSetting.CubeLength - rtPos[index, 1] - radii[index, 0]);
                    shouldVelBeDecayed[index, 1] = 1;
                }
                if (rtPos[index, 2] - radii[index, 0] < 0)
                {
                    force[index, 2] += knw * (-rtPos[index, 2] + radii[index, 0]);
                    shouldVelBeDecayed[index, 2] = 1;

                    //rtVel[index, 2] = -rtVel[index, 2];
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
                double dx = CvInvoke.Norm(rtPos.GetRow(index).GetCols(0, 2), Emgu.CV.CvEnum.NormType.L2);
                double angle = Math.Atan2(rtPos[index, 1], rtPos[index, 0]);
                double rotAngle = angle - Math.PI / 2;
                double vx, vy;
                if (dx + radii[index, 0] > cylinderBound.Radius)
                {
                    force[index, 0] += Math.Cos(angle) * knw * (-dx - radii[index, 0] + cylinderBound.Radius);
                    force[index, 1] += Math.Sin(angle) * knw * (-dx - radii[index, 0] + cylinderBound.Radius);

                    //简化的方法
                    shouldVelBeDecayed[index, 0] = 1;
                    shouldVelBeDecayed[index, 1] = 1;

                    //符合实际情况的方法
                    ////旋转坐标轴
                    //vx = rtVel[index, 0] * Math.Cos(rotAngle) + rtVel[index, 1] * Math.Sin(rotAngle);
                    //vy = -rtVel[index, 0] * Math.Sin(rotAngle) + rtVel[index, 1] * Math.Cos(rotAngle);

                    //vy = -vy ;

                    //////将速度坐标轴变换回来
                    //rtVel[index, 0] = vx * Math.Cos(rotAngle) - vy * Math.Sin(rotAngle);
                    //rtVel[index, 1] = vx * Math.Sin(rotAngle) + vy * Math.Cos(rotAngle);
                }

                if (rtPos[index, 2] - radii[index, 0] < 0)
                {
                    force[index, 2] += knw * (-rtPos[index, 2] + radii[index, 0]);
                    shouldVelBeDecayed[index, 2] = 1;
                    //rtVel[index, 2] = -rtVel[index, 2];
                    //rtVel[index, 2] = rtVel[index, 2] * velDecayRate;
                }
                //高度限幅
                //if (rtPos[i, 2] + radii[i, 0] > cylinderBound.Height)
                //{
                //    force[i, 2] += knw * (cylinderBound.Height - rtPos[i, 2] - radii[i, 0]);
                //    rtVel[i, 2] = rtVel[i, 2] * velDecayRate;
                //}

            }

            ///加速度的计算
            for (int k = 0; k < 3;k++ )
            {
                rtAcc[index, k] = force[index, k] / mass[index, 0];
            }
                
        }

        /// <summary>
        /// 计算实时速度
        /// </summary>
        private void ComputeVel()
        {
            //CvInvoke.AccumulateProduct(,);
            Parallel.For(0, objNum, (i) =>
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (shouldVelBeDecayed[i, j] == 1)
                            rtVel[i, j] *= velDecayRate;
                    }
                });

            if (PackingSystemSetting.IsParaCompute)
            {
                Parallel.For(0, objNum, (i) =>
                {
                    rtVel[i, 0] += rtAcc[i, 0] * DeltaT;
                    rtVel[i, 1] += rtAcc[i, 1] * DeltaT;
                    rtVel[i, 2] += rtAcc[i, 2] * DeltaT;
                });
            }
            else
            {
                for (int i = 0; i < objNum; i++)
                {
                    rtVel[i, 0] += rtAcc[i, 0] * DeltaT;
                    rtVel[i, 1] += rtAcc[i, 1] * DeltaT;
                    rtVel[i, 2] += rtAcc[i, 2] * DeltaT;
                }
            }

            //ApplySaturationVel();
        }

        /// <summary>
        /// 计算实时位置
        /// </summary>
        private void ComputePos()
        {
            //Matrix<double> oldPos = new Matrix<double>( objNum, dim );
            //rtPos.CopyTo( oldPos );
            //for (int i = 0; i < objNum;i++ )
            //{
            //    bool acceptNewPos = true;
            //    //将更新后的位置信息存储在临时的变量中
            //    for (int j = 0; j < dim;j++ )
            //        rtPos[i, j] += rtVel[i, j] * DeltaT;

            //    switch (PackingSystemSetting.SystemBoundType)
            //    {
            //        case BoundType.CylinderType:
            //            double xy = Math.Sqrt(rtPos[i, 0] * rtPos[i, 0] + rtPos[i, 1] * rtPos[i, 1]);
            //            if (xy + radii[i, 0] > PackingSystemSetting.Radius || rtPos[i, 2] < radii[i, 0])
            //            {
            //                if (xy + radii[i, 0] > PackingSystemSetting.Radius)
            //                {
            //                    shouldVelBeDecayed[i, 0] = 1;
            //                    shouldVelBeDecayed[i, 1] = 1;
            //                }
            //                else
            //                    shouldVelBeDecayed[i, 2] = 1;
            //                acceptNewPos = false;
            //            }
            //            break;
            //        case BoundType.CubeType:
            //            break;
            //        default:
            //            break;
            //    }

            //    if( acceptNewPos )
            //    {
            //        Matrix<double> dists = CuteTools.ComputePointToPoints(rtPos.GetRow(i), rtPos);
            //        for (int j = 0; j < localBallsIndex[i].Count; j++)
            //        {
            //            if (dists[localBallsIndex[i][j], 0] < radii[i, 0] + radii[localBallsIndex[i][j], 0])
            //            {
            //                acceptNewPos = false;
            //                break;
            //            }
            //        }
            //    }
            //    if (!acceptNewPos)
            //    {
            //        for (int j = 0; j < dim; j++)
            //            rtPos[i, j] = oldPos[i, j];
            //    }
                
            //}


            //return;
            if (PackingSystemSetting.IsParaCompute)
            {
                Parallel.For(0, objNum, (i) =>
                    {
                        rtPos[i, 0] += rtVel[i, 0] * DeltaT;
                        rtPos[i, 1] += rtVel[i, 1] * DeltaT;
                        rtPos[i, 2] += rtVel[i, 2] * DeltaT;
                        switch (PackingSystemSetting.SystemBoundType)
                        {
                            case BoundType.CylinderType:
                                double xy = Math.Sqrt(rtPos[i, 0] * rtPos[i, 0] + rtPos[i, 1] * rtPos[i, 1]);
                                if (xy + radii[i, 0] > PackingSystemSetting.Radius)
                                {
                                    rtPos[i, 0] = rtPos[i, 0] / xy * (PackingSystemSetting.Radius - radii[i, 0]);
                                    rtPos[i, 1] = rtPos[i, 1] / xy * (PackingSystemSetting.Radius - radii[i, 0]);
                                }
                                rtPos[i, 2] = Math.Max(radii[i, 0], rtPos[i, 2]);
                                break;
                            case BoundType.CubeType:
                                break;
                            default:
                                break;
                        }
                    });
            }
            else
            {
                for (int i = 0; i < objNum; i++)
                {
                    rtPos[i, 0] += rtVel[i, 0] * DeltaT;
                    rtPos[i, 1] += rtVel[i, 1] * DeltaT;
                    rtPos[i, 2] += rtVel[i, 2] * DeltaT;
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
        /// 判断点(x,y,z)是否在下标为n的小球体内
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool IsPointInReferedSphere(double x, double y, double z, int i)
        {
            bool res = false;
            if ((i < objNum || i >= 0) &&((x - rtPos[i, 0]) * (x - rtPos[i, 0]) + (y - rtPos[i, 1]) * (y - rtPos[i, 1]) + (z - rtPos[i, 2]) * (z - rtPos[i, 2])) < radii[i, 0] * radii[i, 0])
            {
                res = true;
            }
            return res;
        }


        /// <summary>
        /// 更新小球的位置、半径和质量信息
        /// 当重新从文件中导入小球信息以后需要更新
        /// </summary>
        /// <param name="balls"></param>
        public void UpdateBallInfo( SpherePlot balls )
        {
            for(int i=0;i<balls.Spheres.Count();i++)
            {
                radii[i, 0] = balls.Spheres[i].GetRadius();
                mass[i, 0] = 4 / 3 * Math.PI * Math.Pow(radii[i, 0], 3);
                rtPos[i, 0] = balls.Spheres[i].GetCenter()[0];
                rtPos[i, 1] = balls.Spheres[i].GetCenter()[1];
                rtPos[i, 2] = balls.Spheres[i].GetCenter()[2];
            }
        }

        /// <summary>
        /// 更新小球的位置、速度、角加速度、半径和质量信息
        /// 导入文件时，利用重新生成的SimpleModelForSave类进行更新
        /// </summary>
        /// <param name="balls"></param>
        public void UpdateBallInfo(SimpleModelForSave sModel)
        {
            currIter = sModel.currIter;
            for (int i = 0; i < sModel.Radii.Rows;i++ )
            {
                radii[i, 0] = sModel.Radii[i, 0];
                mass[i, 0] = 4 / 3 * Math.PI * Math.Pow(radii[i, 0], 3);
                for(int j=0;j<sModel.RtPos.Cols;j++)
                {
                    rtPos[i, j] = sModel.RtPos[i, j];
                    rtVel[i, j] = sModel.RtVel[i, j];
                    rtAcc[i, j] = sModel.RtAcc[i, j];
                }
            }
        }

        /// <summary>
        /// 计算目前小球的能量
        /// 动能+重力势能
        /// </summary>
        /// <returns></returns>
        private double ComputeEnergy()
        {
            double energy = 0.0;
            for (int i = 0; i < mass.Rows;i++ )
            {
                energy += 0.5 * mass[i, 0] * rtVel.GetRow(i).Norm + mass[i, 0] * gravity * (RtPos[i, 2]-radii[i,0]);
            }
            return energy;
        }

        /// <summary>
        /// 对数组进行限幅
        /// 每一行作为一个向量，对其幅值进行限幅
        /// </summary>
        /// <param name="data">矩阵</param>
        /// <param name="max">最大值</param>
        private void ApplySaturation( ref Matrix<double> data, double max  )
        {
            Matrix<double> square = new Matrix<double>(data.Rows, 3); //平方
            Matrix<double> norm = new Matrix<double>(data.Rows, 1);  //平方和
            Matrix<double> res = new Matrix<double>( data.Rows, 1 );  //平方和的开方
            CvInvoke.AccumulateSquare( data, square );  
            
            CvInvoke.Add(square.GetCol(0), square.GetCol(1), norm);
            CvInvoke.Add(square.GetCol(2), norm, norm);
            CvInvoke.Sqrt( norm, res );

            for (int i = 0; i < data.Rows;i++ )
            {
                if (res[i, 0] > max)
                {
                    for (int j = 0; j < data.Cols; j++)
                        data[i, j] = data[i, j] * max / res[i, 0];
                }
            }
        }

        /// <summary>
        /// 对速度限幅
        /// 奈何ref参数不能放在lambda表达式中。。
        /// </summary>
        private void ApplySaturationVel()
        {
            Matrix<double> square = new Matrix<double>(rtVel.Rows, 3); //平方
            Matrix<double> norm = new Matrix<double>(rtVel.Rows, 1);  //平方和
            Matrix<double> res = new Matrix<double>(rtVel.Rows, 1);  //平方和的开方
            CvInvoke.AccumulateSquare(rtVel, square);

            CvInvoke.Add(square.GetCol(0), square.GetCol(1), norm);
            CvInvoke.Add(square.GetCol(2), norm, norm);
            CvInvoke.Sqrt(norm, res);

            Parallel.For(0, rtVel.Rows, (i) =>
            {
                if (res[i, 0] > MaxVel)
                {
                    for (int j = 0; j < rtVel.Cols; j++)
                        rtVel[i, j] = rtVel[i, j] * MaxVel / res[i, 0];
                }
            });
        }

        /// <summary>
        /// 对加速度限幅
        /// 奈何ref参数不能放在lambda表达式中。。
        /// </summary>
        private void ApplySaturationAcc()
        {
            Matrix<double> square = new Matrix<double>(rtAcc.Rows, 3); //平方
            Matrix<double> norm = new Matrix<double>(rtAcc.Rows, 1);  //平方和
            Matrix<double> res = new Matrix<double>(rtAcc.Rows, 1);  //平方和的开方
            CvInvoke.AccumulateSquare(rtAcc, square);

            CvInvoke.Add(square.GetCol(0), square.GetCol(1), norm);
            CvInvoke.Add(square.GetCol(2), norm, norm);
            CvInvoke.Sqrt(norm, res);

            Parallel.For(0, rtAcc.Rows, (i) =>
            {
                if (res[i, 0] > MaxAcc)
                {
                    for (int j = 0; j < rtAcc.Cols; j++)
                        rtAcc[i, j] = rtAcc[i, j] * MaxAcc / res[i, 0];
                }
            });
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
        /// 当前所处的迭代次数
        /// </summary>
        public int currIter;

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
                this.currIter = model.CurrIter;
            }
        }
    }
}
