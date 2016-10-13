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
        /// 立方体的边长
        /// </summary>
        private int sideLen;

        private int height;

        /// <summary>
        /// 立方体的offset
        /// </summary>
        private double[] offset;

        public CubeEdgePlot(int len, int height)
        {
            this.sideLen = len;
            this.height = height;
            this.offset = new double[3] {0, 0, 0 };
        }

        public CubeEdgePlot(int len, int height, double[] offset)
        {
            this.sideLen = len;
            this.height = height;
            this.offset = offset;
        }

        /// <summary>
        /// 绘制立方体的边缘
        /// </summary>
        /// <param name="renderer"></param>
        public void PlotEdge(vtkRenderer renderer, byte[] bgColor)
        {
            vtkPoints points = vtkPoints.New();
            points.InsertNextPoint(0 + offset[0], 0 + offset[1], 0 + offset[2]);
            points.InsertNextPoint(sideLen + offset[0], 0 + offset[1], 0 + offset[2]);
            points.InsertNextPoint(sideLen + offset[0], sideLen + offset[1], 0 + offset[2]);
            points.InsertNextPoint(0 + offset[0], sideLen + offset[1], 0 + offset[2]);

            points.InsertNextPoint(0 + offset[0], sideLen + offset[1], height + offset[2]);
            points.InsertNextPoint(sideLen + offset[0], sideLen + offset[1], height + offset[2]);
            points.InsertNextPoint(sideLen + offset[0], 0 + offset[1], height + offset[2]);
            points.InsertNextPoint(0 + offset[0], 0 + offset[1], height + offset[2]);

            vtkPolyData pd = vtkPolyData.New();

            vtkLine line = vtkLine.New();

            vtkCellArray cellArr = vtkCellArray.New();

            //描出边框
            for (int i = 0; i < 4; i++)
            {
                line.GetPointIds().SetId(0, i);
                line.GetPointIds().SetId(1, (i + 1) % 4);
                cellArr.InsertNextCell(line);

                line.GetPointIds().SetId(0, i + 4);
                line.GetPointIds().SetId(1, (i + 1) % 4 + 4);
                cellArr.InsertNextCell(line);

                line.GetPointIds().SetId(0, i);
                line.GetPointIds().SetId(1, 7 - i);
                cellArr.InsertNextCell(line);

            }

            IntPtr iColor = Marshal.AllocHGlobal(bgColor.Length);
            Marshal.Copy(bgColor, 0, iColor, bgColor.Length);

            vtkUnsignedCharArray colors = vtkUnsignedCharArray.New();

            colors.SetNumberOfComponents(3);
            for (int i = 0; i < cellArr.GetNumberOfCells(); i++)
            {
                colors.InsertNextTupleValue(iColor);
            }

            pd.SetPoints(points);
            pd.SetLines(cellArr);
            pd.GetCellData().SetScalars(colors);

            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInput(pd);
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            actor.GetProperty().SetLineWidth(3);
            
            Marshal.FreeHGlobal(iColor);

            renderer.AddActor(actor);
        }

    }
}
