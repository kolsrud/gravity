using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Gravity.Universe;

namespace Gravity.ViewModel
{
    class ViewModel
    {
        public List<PlanetViewModel> PlanetViewModels { get; private set; }
        public List<SpaceShipViewModel> SpaceShipViewModels { get; private set; }
        public List<MovingBodyViewModel> MovingBodies => PlanetViewModels.Concat<MovingBodyViewModel>(SpaceShipViewModels).ToList();

        public void Initialize(double scale, Canvas canvas, Universe.Universe universe)
        {
            PlanetViewModels = universe.Planets.Select(p => new PlanetViewModel(scale, p)).ToList();
            SpaceShipViewModels = universe.SpaceShips.Select(s => new SpaceShipViewModel(s)).ToList();
			MovingBodies.ForEach(m => m.AddToCanvas(canvas));
        }

        private void UpdatePositions(Position canvasCenter, double scale)
        {
            MovingBodies.ForEach(m => m.UpdatePosition(canvasCenter, scale));
        }

        private void UpdateVectors(double scale)
        {
            MovingBodies.ForEach(m => m.UpdateVectors(scale));
        }

        public void ToggleAccelerationVectors()
        {
            MovingBodies.ForEach(m => m.ToggleAccelerationVector());
        }

        public void ToggleVelocityVectors()
        {
            MovingBodies.ForEach(m => m.ToggleVelocityVector());
        }

        public void UpdateGraphics(Position canvasCenter, double scale)
        {
            UpdatePositions(canvasCenter, scale);
            UpdateVectors(scale);
        }
    }

    internal abstract class MovingBodyViewModel
    {
        private bool VelocityVectorActive = true;
        public Line VelocityVector { get; }
        private bool AccelerationVectorActive = true;
        public Line AccelerationVector { get; }
        public TextBlock Name { get; }

        private readonly MovingBody _m;
        protected MovingBodyViewModel(MovingBody m)
        {
            _m = m;

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

            Name = new TextBlock
            {
                Text = m.Name,
                Foreground = Brushes.White
            };
        }

        public void UpdateVectors(double scale)
        {
            VelocityVector.X2 = _m.VelocityX / 50;
            VelocityVector.Y2 = _m.VelocityY / 50;

            AccelerationVector.X2 = _m.AccelerationX * 1_000;
            AccelerationVector.Y2 = _m.AccelerationY * 1_000;
        }

        public void UpdatePosition(Position p)
        {
            SetPosition(VelocityVector, p);
            SetPosition(AccelerationVector, p);
            SetPosition(Name, p);
        }

        public static void SetPosition(UIElement e, Position p)
        {
            Canvas.SetLeft(e, p.X);
            Canvas.SetTop(e, p.Y);
        }

        public virtual void AddToCanvas(Canvas canvas)
        {
            canvas.Children.Add(VelocityVector);
            canvas.Children.Add(AccelerationVector);
            canvas.Children.Add(Name);
        }

        public void ToggleAccelerationVector()
        {
            AccelerationVectorActive = !AccelerationVectorActive;
            AccelerationVector.StrokeThickness = AccelerationVectorActive ? 1 : 0;
        }

        public void ToggleVelocityVector()
        {
            VelocityVectorActive = !VelocityVectorActive;
            VelocityVector.StrokeThickness = VelocityVectorActive ? 1 : 0;
        }

        protected Position GetCenter(Position canvasCenter, double scale)
        {
            return canvasCenter.Add(_m.Position.Scale(scale));
        }

        public abstract void UpdatePosition(Position canvasCenter, double scale);
    }
}