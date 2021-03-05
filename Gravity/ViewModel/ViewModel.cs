using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Gravity.Universe;

namespace Gravity.ViewModel
{
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

        public void ToggleAccelerationVectors()
        {
            PlanetViewModels.ForEach(p => p.ToggleAccelerationVector());
        }

        public void ToggleVelocityVectors()
        {
            PlanetViewModels.ForEach(p => p.ToggleVelocityVector());
        }
    }

    internal class PlanetViewModel
    {
        public Ellipse PlanetGraphics { get; }
        private bool VelocityVectorActive = true;
        public Line VelocityVector { get; }
        private bool AccelerationVectorActive = true;
        public Line AccelerationVector { get; }
        public TextBlock Name { get; }

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

            Name = new TextBlock
            {
                Text = Planet.Name,
                Foreground = Brushes.White
            };
        }

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
            Canvas.SetLeft(Name, x);
            Canvas.SetTop(Name, y);
        }

        public void UpdateVectors(double scale)
        {
            VelocityVector.X2 = Planet.VelocityX / 50;
            VelocityVector.Y2 = Planet.VelocityY / 50;

            AccelerationVector.X2 = Planet.AccelerationX * 1_000;
            AccelerationVector.Y2 = Planet.AccelerationY * 1_000;
        }

        public void AddToCanvas(Canvas canvas)
        {
            canvas.Children.Add(PlanetGraphics);
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
    }

}