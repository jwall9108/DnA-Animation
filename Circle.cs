using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;

using System.Windows;

namespace Circles
{
     class Circle : Shape
    {
        public EllipseGeometry _Circle;
        public double X { get; set; }
        public double Y { get; set; }
        public double angle { get; set; }
        public Point Center
        {
            get {return new Point(X  , Y );}
            set {}
        }
        public double _Height
        {
            get {return this.Height;}
            set {if ((value > 0)) {this.Height = value;}}
        }
        public double _Width
        {
            get{return this.Width;}
            set{if ((value > 0)){this.Width = value;}}
        }

        public Circle ()
        {
            _Circle = new EllipseGeometry();
            this.Height = 9;
            this.Width = 9;
            this.Fill = Brushes.Red;
            this.Stroke = Brushes.Black;
        }

        protected override Geometry DefiningGeometry
        {
            get {_Circle.RadiusX = this.Width / 2;
              _Circle.RadiusY = this.Height / 2;
              return _Circle;}
        }
                     
        public override Transform GeometryTransform
        {
            get{return Transform.Identity;}
        }

      
    }
}
