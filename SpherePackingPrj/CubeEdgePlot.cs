using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;

namespace SpherePacking.MainWindow
{
    class CubeEdgePlot
    {
        /// <summary>
        /// 长方体的边长
        /// </summary>
        private double sideLen;

        /// <summary>
        /// 长方体的高
        /// </summary>
        private double height;

        /// <summary>
        /// 长方体的offset
        /// </summary>
        private double[] offset;

        public CubeEdgePlot(double len, double height)
        {
            this.sideLen = len;
            this.height = height;
            this.offset = new double[3] {0, 0, 0 };
        }

        public CubeEdgePlot(double len, double height, double[] offset)
        {
            this.sideLen = len;
            this.height = height;
            this.offset = offset;
        }

        /// <summary>
        /// 绘制立长方体的边缘
        /// </summary>
        /// <param name="bgColor">边缘的颜色</param>
        /// <param name="actors">添加至该对象</param>
        public void AddContainerEdgeToActorCollection(byte[] bgColor, ref vtkActorCollection actors)
        {
            vtkProperty pp = vtkProperty.New();
            pp.SetOpacity(0.5);
            pp.SetColor(bgColor[0], bgColor[1], bgColor[2]);
            pp.SetLineWidth(5);
            pp.SetLighting(false);

            vtkCubeSource cs = vtkCubeSource.New();
            cs.SetCenter(sideLen/2, sideLen/2, height/2);
            cs.SetXLength( sideLen );
            cs.SetYLength(sideLen);
            cs.SetZLength(height);

            #region 采用描点的方式绘制长方体 只需要添加到mapper.SetInput(pd);即可
            //vtkPoints points = vtkPoints.New();
            //points.InsertNextPoint(0 + offset[0], 0 + offset[1], 0 + offset[2]);
            //points.InsertNextPoint(sideLen + offset[0], 0 + offset[1], 0 + offset[2]);
            //points.InsertNextPoint(sideLen + offset[0], sideLen + offset[1], 0 + offset[2]);
            //points.InsertNextPoint(0 + offset[0], sideLen + offset[1], 0 + offset[2]);

            //points.InsertNextPoint(0 + offset[0], sideLen + offset[1], height + offset[2]);
            //points.InsertNextPoint(sideLen + offset[0], sideLen + offset[1], height + offset[2]);
            //points.InsertNextPoint(sideLen + offset[0], 0 + offset[1], height + offset[2]);
            //points.InsertNextPoint(0 + offset[0], 0 + offset[1], height + offset[2]);

            //vtkPolyData pd = vtkPolyData.New();

            //vtkLine line = vtkLine.New();

            //vtkCellArray cellArr = vtkCellArray.New();

            ////描出边框
            //for (int i = 0; i < 4; i++)
            //{
            //    line.GetPointIds().SetId(0, i);
            //    line.GetPointIds().SetId(1, (i + 1) % 4);
            //    cellArr.InsertNextCell(line);

            //    line.GetPointIds().SetId(0, i + 4);
            //    line.GetPointIds().SetId(1, (i + 1) % 4 + 4);
            //    cellArr.InsertNextCell(line);

            //    line.GetPointIds().SetId(0, i);
            //    line.GetPointIds().SetId(1, 7 - i);
            //    cellArr.InsertNextCell(line);

            //}

            //IntPtr iColor = Marshal.AllocHGlobal(bgColor.Length);
            //Marshal.Copy(bgColor, 0, iColor, bgColor.Length);

            //vtkUnsignedCharArray colors = vtkUnsignedCharArray.New();

            //colors.SetNumberOfComponents(3);
            //for (int i = 0; i < cellArr.GetNumberOfCells(); i++)
            //{
            //    colors.InsertNextTupleValue(iColor);
            //}

            //pd.SetPoints(points);
            //pd.SetLines(cellArr);
            //pd.GetCellData().SetScalars(colors);
            //Marshal.FreeHGlobal(iColor);
            #endregion

            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            //mapper.SetInput(pd);
            mapper.SetInputConnection(cs.GetOutputPort());
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            actor.SetProperty(pp);

            actors.AddItem(actor);
        }

    }
}
