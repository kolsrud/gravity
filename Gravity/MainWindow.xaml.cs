using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Gravity
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
		private Timer _timer;
        private Timer _guiTimer;
        private readonly List<Planet> Planets = new List<Planet>();
        private readonly List<MovingBody> Bodies = new List<MovingBody>();

		public MainWindow()
		{
			InitializeComponent();
            Canvas.Focus();
        }

        private bool _inTick = false;
        private void Tick(object? state)
        {
            if (_inTick)
                return;

            _inTick = true;
            Step();
            _inTick = false;
        }

        private static bool _inTickGui;
        private void TickGui(object? state)
        {
            if (_inTickGui)
                return;

            _inTickGui = true;
            Application.Current.Dispatcher.Invoke(() => Planets.ForEach(SetPosition));
            _inTickGui = false;
        }

        public static double Scale = 1_500_000; // 1 pixel = x m
        public static double StepScale = 60*20; // seconds

        private Position GetCanvasCenter()
        {
            return new Position(Canvas.ActualWidth / 2, Canvas.ActualHeight / 2);
        }

        private void Canvas_OnLoaded(object sender, RoutedEventArgs e)
        {
            const int earthRadius = 6_371_000;
            const int moonRadius = 1_737_100;
            var earth = new Planet(earthRadius, 5.9725e+24, (0, 0), Brushes.Blue);
            earth.SetVelocity(20, Math.PI / 2);
//            var moon = new Planet(Scale, 3474_200, 7.348e+22, (384_400_000, 0), Brushes.LightGray);
            var moon = new Planet(moonRadius, 7.348e+22, (384_400_000, 0), Brushes.LightGray);
            moon.SetVelocity(1022, -Math.PI / 2);
            // var moon = new Planet(Scale, 3474_200, 7.348e+22, (184_400_000, 0), Brushes.LightGray);
            //            var moon = new Planet(Scale, 3474_200, 7.348e+22, (6_400_000, 0), Brushes.LightGray);
            var moon2 = new Planet(moonRadius, 7.348e+22, (300_000_000, 0), Brushes.LightGray);
            moon2.SetVelocity(622, -Math.PI / 2);
            var moon3 = new Planet(moonRadius, 7.348e+22, (200_000_000, 0), Brushes.LightGray);
            moon3.SetVelocity(822, -Math.PI / 2);
            var moon4 = new Planet(moonRadius, 7.348e+22, (-100_000_000, 0), Brushes.LightGray);
            moon4.SetVelocity(1222, Math.PI / 2);

            Planets.Add(earth);
            Planets.Add(moon);
            Planets.Add(moon2);
            Planets.Add(moon3);
            Planets.Add(moon4);
            Planets.ForEach(p => AddElement(Canvas, p));
            _timer = new Timer(Tick, this, 0, 1000/500);
            _guiTimer = new Timer(TickGui, this, 0, 1000/50);
        }

        public void Step()
        {
            var gravityWells = Planets.Select(p => (p.Position, p.Mass)).ToList();
            double minDistance = double.MaxValue;
            foreach (var movingBody in Planets.Concat(Bodies))
            {
                movingBody.Move(StepScale);
                var bodies = gravityWells.Where(well => well.Position != movingBody.Position).ToList();
                minDistance = Math.Min(minDistance, movingBody.Accelerate(StepScale, bodies));
            }

            if (minDistance < 1_000_000)
                StepScale = 10;
            else if (minDistance < 10_000_000)
                StepScale = 60;
            else
                StepScale = 60 * 20;
        }

        private double Mass(double d, int exp)
        {
            return d*Math.Pow(10, 22);
        }

        private void AddElement(Canvas canvas, MovingBody body)
        {
            foreach (var graphicsElement in body.GetGraphics())
            {
                canvas.Children.Add(graphicsElement.UiElement);
            }
        }

        private void SetPosition(MovingBody body)
        {
            var center = GetCanvasCenter();
            foreach (var graphicsElement in body.GetGraphics())
            {
                graphicsElement.SetGraphicsPosition(center);
            }
        }

        private void Canvas_OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Step();
        }
	}

	class Constants
    {
        public const double G = 6.674e-11;
    }

    class Planet : MovingBody
    {
        public double Mass { get; }

        public Planet(double radius, double mass, (double, double) position, SolidColorBrush color)
            : base(new Circle(new Position(position), color, (int) radius))
        {
            Mass = mass;
        }
    }

    internal abstract class MovingBody
    {
        public GraphicsElement Graphics { get; }
        public Line VelocityVector { get; }

        public IEnumerable<GraphicsElement> GetGraphics()
        {
            return new[] {Graphics, VelocityVector};
        }

        public Position Position => Graphics.Position;
        public double VelocityX = 0;
        public double VelocityY = 0;

        public void SetVelocity(double velocity, double angle)
        {
            VelocityX = velocity * Math.Cos(angle);
            VelocityY = velocity * Math.Sin(angle);
        }

        protected MovingBody(GraphicsElement graphics)
        {
            Graphics = graphics;
            VelocityVector = new Line(graphics.Position, Brushes.Green);
        }

        public void Move(double stepScale)
        {
            Position.X += VelocityX * stepScale;
            Position.Y += VelocityY * stepScale;
            VelocityVector.Position.X = Position.X;
            VelocityVector.Position.Y = Position.Y;
        }

        public double Accelerate(double stepScale, List<(Position Position, double Mass)> gravityWells)
        {
            return gravityWells.Select(gw => Accelerate(stepScale, gw)).Min();
        }

        private double Accelerate(double stepScale, (Position Position, double Mass) gravityWell)
        {
            var d = Position.Distance(gravityWell.Position);
            var a = (Constants.G * gravityWell.Mass / (d * d));
            var angle = Position.Angle(gravityWell.Position);
            var dx = a * Math.Cos(angle) * stepScale;
            var dy = a * Math.Sin(angle) * stepScale;
            VelocityX += dx;
            VelocityY += dy;

            return d;
        }
    }

    abstract class GraphicsElement
    {
        public Position Position { get; set; }

        protected GraphicsElement(Position position)
        {
            Position = position;
        }

        public abstract Position GetGraphicsPosition(Position center);
        public UIElement UiElement { get; protected set; }

        public abstract void SetGraphicsPosition(Position center);
    }

    class Line : GraphicsElement
    {
        public Line(Position position, SolidColorBrush color) : base(position)
        {
            UiElement = new System.Windows.Shapes.Line
            {
                Stroke = color,
                StrokeThickness = 1,
                X1 = position.X,
                Y1 = position.Y,
                X2 = position.X + 100,
                Y2 = position.Y
            };
        }

        public override Position GetGraphicsPosition(Position center)
        {
            throw new NotImplementedException();
        }

        public override void SetGraphicsPosition(Position center)
        {
            UiElement.SetValue(Canvas.LeftProperty, center.X + (Position.X) / MainWindow.Scale);
            UiElement.SetValue(Canvas.TopProperty, center.Y + (Position.Y) / MainWindow.Scale);
        }
    }

    class Circle : GraphicsElement
    {
        public int Radius { get; }

        public Circle(Position position, SolidColorBrush color, int radius) : base(position)
        {
            Radius = radius;
            UiElement = new Ellipse
            {
                Width = radius * 2 / MainWindow.Scale,
                Height = radius * 2 / MainWindow.Scale,
                Fill = color
            };
        }

        public override Position GetGraphicsPosition(Position center)
        {
            return new Position(
                center.X + Position.X / MainWindow.Scale,
                center.Y + Position.Y / MainWindow.Scale
            );
        }

        public override void SetGraphicsPosition(Position center)
        {
            UiElement.SetValue(Canvas.LeftProperty, center.X + (Position.X - Radius) / MainWindow.Scale);
            UiElement.SetValue(Canvas.TopProperty, center.Y + (Position.Y - Radius) / MainWindow.Scale);
        }
    }
}
