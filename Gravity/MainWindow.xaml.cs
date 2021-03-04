using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Gravity.Universe;

namespace Gravity
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
		private Timer _timer;
        private Timer _guiTimer;
        private readonly Universe.Universe Universe = new Universe.Universe();
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
            Application.Current.Dispatcher.Invoke(() => Universe.Planets.ForEach(SetPosition));
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
            Universe.Planets.ForEach(p => AddElement(Canvas, p));
            _timer = new Timer(Tick, this, 0, 1000/500);
            _guiTimer = new Timer(TickGui, this, 0, 1000/50);
        }

        public void Step()
        {
            var gravityWells = Universe.Planets.Select(p => (p.Position, p.Mass)).ToList();
            double minDistance = double.MaxValue;
            foreach (var movingBody in Universe.Planets.Concat(Bodies))
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
