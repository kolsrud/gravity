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
        private readonly ViewModel ViewModel = new ViewModel();

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
            var center = GetCanvasCenter();
            Application.Current.Dispatcher.Invoke(SetPosition);
            _inTickGui = false;
        }

        public static double Scale = 1_500_000; // 1 pixel = x m
        public static int TimeScale = 60*20; // seconds

        private Position GetCanvasCenter()
        {
            return new Position(Canvas.ActualWidth / 2, Canvas.ActualHeight / 2);
        }

        private void Canvas_OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Initialize(Scale, Canvas, Universe);
            TickGui(this);
            _timer = new Timer(Tick, this, 0, 1000/500);
            _guiTimer = new Timer(TickGui, this, 0, 1000/50);
        }

        public void Step()
        {
            var minDistance = Universe.Step(TimeScale);

            if (minDistance < 1_000_000)
                TimeScale = 10;
            else if (minDistance < 10_000_000)
                TimeScale = 60;
            else
                TimeScale = 60 * 20;
        }

        private void SetPosition()
        {
            var center = GetCanvasCenter();
            ViewModel.UpdatePositions(center, Scale);
            ViewModel.UpdateVectors(Scale);
        }

        private void Canvas_OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Step();
            TickGui(this);
        }
    }

    class ViewModel
    {
        public List<PlanetViewModel> PlanetViewModels { get; private set; }

        public void Initialize(double scale, Canvas canvas, Universe.Universe universe)
        {
            PlanetViewModels = universe.Planets.Select(p => new PlanetViewModel(scale, p)).ToList();
            PlanetViewModels.ForEach(p => p.AddToCanvas(canvas));
        }

        public void UpdatePositions(Position canvasCenter, double scale)
        {
            PlanetViewModels.ForEach(p => p.UpdatePosition(canvasCenter, scale));
        }

        public void UpdateVectors(double scale)
        {
            PlanetViewModels.ForEach(p => p.UpdateVectors(scale));
        }
    }

    internal class PlanetViewModel
    {
        public Ellipse PlanetGraphics { get; }
        public Line VelocityVector { get; }
        public Line AccelerationVector { get; }
        public Planet Planet { get; }

        public PlanetViewModel(double scale, Planet planet)
        {
            Planet = planet;
            PlanetGraphics = new Ellipse()
            {
                Width = planet.Radius * 2 / scale,
                Height = planet.Radius * 2 / scale,
                Fill = planet.Color
            };

            VelocityVector = new Line
            {
                Stroke = Brushes.Green,
                StrokeThickness = 1
            };

            AccelerationVector = new Line
            {
                Stroke = Brushes.Red,
                StrokeThickness = 1
            };
        }

        private int i;
        public void UpdatePosition(Position canvasCenter, double scale)
        {
            var x = canvasCenter.X + Planet.Position.X / scale;
            var y = canvasCenter.Y + Planet.Position.Y / scale;
            var px = x - Planet.Radius / scale;
            var py = y - Planet.Radius / scale;
            Canvas.SetLeft(PlanetGraphics, px);
            Canvas.SetTop(PlanetGraphics, py);
            Canvas.SetLeft(VelocityVector, x);
            Canvas.SetTop(VelocityVector, y);
            Canvas.SetLeft(AccelerationVector, x);
            Canvas.SetTop(AccelerationVector, y);
        }

        public void UpdateVectors(double scale)
        {
            VelocityVector.X2 = Planet.VelocityX / 50;
            VelocityVector.Y2 = Planet.VelocityY / 50;

            AccelerationVector.X2 = Planet.AccelerationX*1_000;
            AccelerationVector.Y2 = Planet.AccelerationY*1_000;
        }

        public void AddToCanvas(Canvas canvas)
        {
            canvas.Children.Add(PlanetGraphics);
            canvas.Children.Add(VelocityVector);
            canvas.Children.Add(AccelerationVector);
        }
    }
}
