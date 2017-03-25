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


        public void AddCylinderEdgeToActors(byte[] bgColor, ref vtkActorCollection actors)
        {
            vtkProperty pp = vtkProperty.New();
            pp.SetOpacity(0.7);
            pp.SetColor(bgColor[0], bgColor[1], bgColor[2]);
            pp.SetLineWidth(5);
            pp.SetLighting(false);

            vtkRegularPolygonSource circle = vtkRegularPolygonSource.New();
            circle.GeneratePolygonOn();
            circle.SetNumberOfSides(50);
            circle.SetRadius(radius);
            circle.SetCenter(0, 0, 0);

            vtkPolyDataMapper mappper = vtkPolyDataMapper.New();
            mappper.SetInputConnection(circle.GetOutputPort());
            
            vtkActor actor = vtkActor.New();
            actor.SetProperty(pp);
            actor.SetMapper(mappper);
            actors.AddItem(actor);

            actor.SetProperty( pp );
            actors.AddItem( actor );


            vtkLineSource ls = vtkLineSource.New();
            ls.SetPoint1( 0, 0, 0 );
            ls.SetPoint2(0, 0, height);
            vtkTubeFilter tf = vtkTubeFilter.New();
            tf.SetInputConnection( ls.GetOutputPort() );
            tf.SetRadius( radius );
            tf.SetNumberOfSides( 100 );
            tf.CappingOff();

            vtkPolyDataMapper dm = vtkPolyDataMapper.New();
            dm.SetInputConnection( tf.GetOutputPort() );
            
            vtkActor a2 = vtkActor.New();
            a2.SetMapper(dm);

            
            
            a2.SetProperty( pp );

            pp.SetOpacity(0.5);
            actor.SetProperty( pp );

            actors.AddItem( a2 );
        }


    }
}
