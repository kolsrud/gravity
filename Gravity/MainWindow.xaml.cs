using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        private readonly ViewModel.ViewModel ViewModel = new ViewModel.ViewModel();

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
            Application.Current.Dispatcher.Invoke(SetPosition);
            _inTickGui = false;
        }

        public static double Scale = 1_500_000; // 1 pixel = x m
        // public static double Scale = 750_000; // 1 pixel = x m
        public static double TimeScale = 60*5; // seconds

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
        }

        private void SetPosition()
        {
            var center = GetCanvasCenter();
            ViewModel.UpdateGraphics(center, Scale);
        }

        private void Canvas_OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                    ViewModel.ToggleAccelerationVectors();
                    break;
                case Key.V:
                    ViewModel.ToggleVelocityVectors();
                    break;
                case Key.Up:
                    Universe.SpaceShips.ForEach(s => s.Burn());
                    break;
                case Key.Left:
                    Universe.SpaceShips.ForEach(s => s.Rotate(25));
                    break;
                case Key.Right:
                    Universe.SpaceShips.ForEach(s => s.Rotate(-25));
                    break;
                case Key.Z:
                    Scale *= 0.75;
                    break;
                case Key.X:
                    Scale /= 0.75;
                    break;
                case Key.Q:
                    TimeScale *= 0.75;
                    break;
                case Key.W:
                    TimeScale /= 0.75;
                    break;
                default:
                    Step();
                    TickGui(this);
                    break;
            }
        }
    }
}
