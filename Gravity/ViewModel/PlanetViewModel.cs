using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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
                Width = planet.Radius * 2 / scale,
                Height = planet.Radius * 2 / scale,
                Fill = planet.Color
            };
        }

        public override void UpdatePosition(Position canvasCenter, Position universeCenter, double scale)
        {
            var center = GetCenter(canvasCenter, universeCenter, scale);
            SetPosition(PlanetGraphics, center.Subtract(Planet.Radius / scale));
            base.UpdatePosition(center);
        }

        protected override IEnumerable<UIElement> GetCustomGraphics()
        {
            return new[] {PlanetGraphics};
        }
    }
}