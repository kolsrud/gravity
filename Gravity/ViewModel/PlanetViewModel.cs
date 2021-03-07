using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;
using Gravity.Universe;

namespace Gravity.ViewModel
{
    internal class PlanetViewModel : MovingBodyViewModel
    {
        public Ellipse PlanetGraphics { get; }

        public Planet Planet { get; }

        public PlanetViewModel(double scale, Planet planet) : base(planet)
        {
            Planet = planet;
            PlanetGraphics = new Ellipse()
            {
                Fill = planet.Color
            };
            SetGraphicsWidth(scale);
        }

        public override void UpdatePosition(Position canvasCenter, Position universeCenter, double scale)
        {
            var center = GetCenter(canvasCenter, universeCenter, scale);
            SetGraphicsWidth(scale);
            SetPosition(PlanetGraphics, center.Subtract(Planet.Radius / scale));
            base.UpdatePosition(center);
        }

        private void SetGraphicsWidth(double scale)
        {
            var r = Planet.Radius * 2 / scale;
            PlanetGraphics.Width = r;
            PlanetGraphics.Height = r;
        }

        protected override IEnumerable<UIElement> GetCustomGraphics()
        {
            return new[] {PlanetGraphics};
        }
    }
}