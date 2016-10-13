using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV.Structure;
using Kitware.VTK;

namespace SpherePacking.MainWindow
{
    /// <summary>
    /// 对于实际过程中的圆柱体来说，小球的直径为50um左右，圆柱体边界的高是2mm，直径是1cm
    /// 将小球的平均直径视为单位1
    /// 则圆柱体的高是40，半径是100
    /// </summary>
    class CylinderEdgePlot
    {
        private double radius;

        private double height;

        /// <summary>
        /// 初始化圆柱形边界
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="height"></param>
        public CylinderEdgePlot( double radius, double height)
        {
            this.radius = radius;
            this.height = height;
        }


        public void PlotCylinderEdge(vtkRenderer renderer,  byte[] bgColor)
        {
            int numOfLine = 360;
            vtkPoints points = vtkPoints.New();
            for (int i = 0; i < 360*2; i += 360*2 / numOfLine)
            {
                points.InsertNextPoint(radius * Math.Cos(i * Math.PI / 180), radius * Math.Sin(i * Math.PI / 180), 0);
                points.InsertNextPoint(radius * Math.Cos(i * Math.PI / 180), radius * Math.Sin(i * Math.PI / 180), height);
            }

            vtkPolyData pd = vtkPolyData.New();

            vtkLine line = vtkLine.New();

            vtkCellArray cellArr = vtkCellArray.New();

            for (int i = 0; i < numOfLine / 2; i++)
            {
                line.GetPointIds().SetId(0, i * 2);
                line.GetPointIds().SetId(1, i * 2 + 1);
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




            vtkRegularPolygonSource circle = vtkRegularPolygonSource.New();
            circle.GeneratePolygonOff();
            circle.SetNumberOfSides(50);
            circle.SetRadius(radius);
            circle.SetCenter(0, 0, 0);

            vtkPolyDataMapper mappper = vtkPolyDataMapper.New();
            mappper.SetInputConnection(circle.GetOutputPort());
            

            vtkActor actor02 = vtkActor.New();
            actor02.SetMapper(mappper);

            renderer.AddActor(actor02);

            vtkRegularPolygonSource circle01 = vtkRegularPolygonSource.New();

            circle01.GeneratePolygonOff();
            circle01.SetNumberOfSides(50);
            circle01.SetRadius(radius);
            circle01.SetCenter(0, 0, height);

            vtkPolyDataMapper mappper01 = vtkPolyDataMapper.New();
            mappper01.SetInputConnection(circle01.GetOutputPort());

            vtkActor actor01 = vtkActor.New();
            actor01.SetMapper(mappper01);

            renderer.AddActor( actor01 );

        }


    }
}
