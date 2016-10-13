using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Kitware.VTK;

namespace SpherePacking.MainWindow
{
    /// <summary>
    /// 绘制立方体容器的表面
    /// </summary>
    class CubeSurfacePlot
    {
        /// <summary>
        /// 立方体的边长
        /// </summary>
        private int sideLen;

        /// <summary>
        /// 立方体的offset
        /// </summary>
        private double[] offset;

        public CubeSurfacePlot(int len)
        {
            this.sideLen = len;
            this.offset = new double[3] {0, 0, 0 };
        }

        public CubeSurfacePlot(int len, double[] offset)
        {
            this.sideLen = len;
            this.offset = offset;
        }
        
        /// <summary>
        /// 绘制立方体的五个面
        /// 根据index确定需要绘制哪个面
        /// </summary>
        /// <param name="index"></param>
        /// <param name="renderer"></param>
        public void PlotSurface(int index, vtkRenderer renderer )
        {
            vtkPlaneSource pSrc = vtkPlaneSource.New();
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            vtkActor actor = vtkActor.New();
            switch( index )
            {
                case 0:
                    pSrc.SetCenter(sideLen + 0.5 + offset[0], sideLen + 0.5 + offset[1], 0 + offset[2]);
                    pSrc.SetNormal(0, 0, 1);
                    pSrc.SetPoint1(sideLen + offset[0], 0 + offset[1], 0 + offset[2]);
                    pSrc.SetPoint2(0 + offset[0], sideLen + offset[1], 0 + offset[2]);
                    mapper.SetInput(pSrc.GetOutput());
                    actor.SetMapper(mapper);
                    renderer.AddActor(actor);

                    break;
                case 1:
                    pSrc.SetCenter(0 + offset[0], sideLen + 0.5 + offset[1], sideLen - 0.5 + offset[2]);
                    pSrc.SetNormal(1, 0, 0);
                    pSrc.SetPoint1(0 + offset[0], 0 + offset[1], sideLen + offset[2]);
                    pSrc.SetPoint2(0 + offset[0], sideLen + offset[1], 0 + offset[2]);
                    mapper.SetInput(pSrc.GetOutput());
                    actor.SetMapper(mapper);
                    renderer.AddActor(actor);

                    break;
                case 2:
                    pSrc.SetCenter(sideLen + offset[0], sideLen + 0.5 + offset[1], sideLen - 0.5 + offset[2]);
                    pSrc.SetNormal(1, 0, 0);
                    pSrc.SetPoint1(sideLen + offset[0], 0 + offset[1], sideLen + offset[2]);
                    pSrc.SetPoint2(sideLen + offset[0], sideLen + offset[1], 0 + offset[2]);
                    mapper.SetInput(pSrc.GetOutput());
                    actor.SetMapper(mapper);
                    renderer.AddActor(actor);

                    break;
                case 3:
                    pSrc.SetCenter(sideLen + 0.5 + offset[0], 0 + offset[1], sideLen - 0.5 + offset[2]);
                    pSrc.SetNormal(0, 1, 0);
                    pSrc.SetPoint1(sideLen + offset[0], 0 + offset[1], 0 + offset[2]);
                    pSrc.SetPoint2(0 + offset[0], 0 + offset[1], sideLen + offset[2]);
                    mapper.SetInput(pSrc.GetOutput());
                    actor.SetMapper(mapper);
                    renderer.AddActor(actor);

                    break;
                case 4:
                    pSrc.SetCenter(sideLen + 0.5 + offset[0], sideLen + offset[1], sideLen - 0.5 + offset[2]);
                    pSrc.SetNormal(0, 1, 0);
                    pSrc.SetPoint1(sideLen + offset[0], sideLen + offset[1], 0 + offset[2]);
                    pSrc.SetPoint2(0 + offset[0], sideLen + offset[1], sideLen + offset[2]);
                    mapper.SetInput(pSrc.GetOutput());
                    actor.SetMapper(mapper);
                    renderer.AddActor(actor);
                    break;
                case 5:
                    break;
                default:
                    break;
            }

            
        }

        /// <summary>
        /// 绘制立方体的所有的5个面
        /// 顶面不绘制，方便观察小球
        /// </summary>
        /// <param name="renderer"></param>
        public void PlotSurfaceAll( vtkRenderer renderer )
        {
            for(int i=0;i<5;i++)
            {
                PlotSurface( i, renderer );
            }
        }
    }
}
