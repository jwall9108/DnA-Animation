using System.Windows;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Circles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private int NumberOfCircles = 30;
        private int Cnt = 40;
        private double rotation;
        private double GrowthFactor;

        private System.Timers.Timer m_renderTimer;
        private System.Timers.Timer m_renderTimer1;

        private AutoResetEvent m_renderSignal = new AutoResetEvent(false);
        private AutoResetEvent m_renderSignal1 = new AutoResetEvent(false);
        private Circle[] _Circles;
        private List<Line> lLines = new List<Line>();
        private List<Circle> lCircles = new List<Circle>();
        private List<Line> lCirclesConnector = new List<Line>();

        public MainWindow()
        {
            InitializeComponent();
            _Circles = (from i in Enumerable.Range(0, NumberOfCircles)
                        select new Circle { }).ToArray();

            int il = 0;
            foreach (Circle C in _Circles)
            {
                canvas1.Children.Add(C);
                Canvas.SetLeft(C, il * 10);
                il++;
            }

            draw_circle(30);

            m_renderTimer = new System.Timers.Timer(100);
            m_renderTimer.Elapsed += (_, __) => m_renderSignal.Set();
            m_renderTimer.Enabled = true;
            Task.Factory.StartNew(() => RenderLoop(), TaskCreationOptions.LongRunning);

            m_renderTimer1 = new System.Timers.Timer(100);
            m_renderTimer1.Elapsed += (_, __) => m_renderSignal1.Set();
            m_renderTimer1.Enabled = true;
            Task.Factory.StartNew(() => RenderLoop1(), TaskCreationOptions.LongRunning);
        }

        private void RenderLoop()
        {
            while (true)
            {
                m_renderSignal.WaitOne();
                Dispatcher.BeginInvoke((Action)RenderCircles, DispatcherPriority.Send, null);
            }
        }

        private void RenderLoop1()
        {
            while (true)
            {
                m_renderSignal1.WaitOne();
                Dispatcher.BeginInvoke((Action)RenderCircles1, DispatcherPriority.Send, null);
            }
        }

        private void RenderCircles()
        {
            try
            {
                List<Point> points = new List<Point>();
                foreach (Line l in lLines)
                { canvas1.Children.Remove(l); }
                lLines.Clear();
                int num = 0;
                foreach (Circle C in _Circles)
                {
                    Canvas.SetLeft(C, Canvas.GetLeft(C) + .5057);
                    Canvas.SetTop(C, GrowthFactor * (Math.Sin(Canvas.GetLeft(C) * 50)) + 120);

                    C.Y = Canvas.GetTop(C);
                    C.X = Canvas.GetLeft(C);
                    points.Add(new Point(C.X, C.Y));

                    if (points.Count == 2)
                    {
                        Point pCenter = new Point(points[0].X + (points[1].X - points[0].X) / 2,
                        points[0].Y + (points[1].Y - points[0].Y) / 2);

                        if (isEven(num))
                        {
                            lLines.Add(CreateLine(points[0], pCenter, Brushes.Blue));
                            lLines.Add(CreateLine(pCenter, points[1], Brushes.White));
                        }
                        else
                        {
                            lLines.Add(CreateLine(pCenter, points[1], Brushes.White));
                            lLines.Add(CreateLine(points[0], pCenter, Brushes.Red));
                        }
                        points.Clear();
                        num++;
                    }
                }
                ConnectOutsidePoints();
            }

            catch (Exception) { };

        }

        private void RenderCircles1()
        {
            foreach (Line l in lCirclesConnector)
            { canvas1.Children.Remove(l); }
            lCirclesConnector.Clear();
            for (int i = 0; i < lCircles.Count; i++)
            {
                Canvas.SetLeft(lCircles[i], Cnt * Math.Cos(lCircles[i].angle) + 400);
                Canvas.SetTop(lCircles[i], Cnt * Math.Sin(lCircles[i].angle + rotation) + 50);
                lCircles[i].angle += .03;
                lCircles[i].X = Canvas.GetLeft(lCircles[i]);
                lCircles[i].Y = Canvas.GetTop(lCircles[i]);
                if (i != 0)
                { lCirclesConnector.Add(CreateLine(lCircles[i].Center, lCircles[21 - i].Center, Brushes.White)); }
            }

            foreach (Line l in lCirclesConnector)
            {
                canvas1.Children.Add(l);
            }

        }

        private void draw_circle(int radius)
        {
            double angle = 0.0;
            double angle_stepsize = 0.3;

            // go through all angles from 0 to 2 * PI radians
            while (angle < 2 * Math.PI)
            {
                Circle c = new Circle();
                Canvas.SetLeft(c, radius * Math.Cos(angle) + 400);
                Canvas.SetTop(c, radius * Math.Sin(angle) + 50);
                c.X = Canvas.GetLeft(c);
                c.Y = Canvas.GetTop(c);
                c.angle = angle;
                canvas1.Children.Add(c);
                angle += angle_stepsize;
                lCircles.Add(c);
            }
        }

        private void ConnectOutsidePoints()
        {
            List<Point> epoints = new List<Point>();
            List<Point> opoints = new List<Point>();

            for (int i = 0; i < NumberOfCircles; i++)
            { if (isEven(i)) { epoints.Add(new Point(_Circles[i].X, _Circles[i].Y)); } else { opoints.Add(new Point(_Circles[i].X, _Circles[i].Y)); } }
            for (int i = 0; i < epoints.Count; i++) { if (i != 0) { lLines.Add(CreateLine(epoints[i - 1], epoints[i], Brushes.Red)); } }
            for (int i = 0; i < opoints.Count; i++) { if (i != 0) { lLines.Add(CreateLine(opoints[i - 1], opoints[i], Brushes.Red)); } }

            foreach (Line l in lLines) { canvas1.Children.Add(l); }
        }

        private bool isEven(int num)
        { if (num % 2 == 0) { return true; } else { return false; } }

        private Line CreateLine(Point one, Point two, Brush color)
        {
            Line line = new Line();
            line.Visibility = Visibility.Visible;
            line.StrokeThickness = 3;
            line.Stroke = color;

            line.X1 = one.X; line.Y1 = one.Y;
            line.X2 = two.X; line.Y2 = two.Y;
            return line;
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GrowthFactor = slider1.Value;
        }

        private void slider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            rotation = slider2.Value;
        }

    }
}
