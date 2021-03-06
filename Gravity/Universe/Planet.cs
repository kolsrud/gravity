using System.Windows.Media;

namespace Gravity.Universe
{

    internal class Planet : MovingBody
    {
        public double Radius { get; }
        public double Mass { get; }
        public SolidColorBrush Color { get; }

        public Planet(string name, double radius, double mass, (double, double) position, (double, double) velocity, SolidColorBrush color)
            : base(name, new Position(position), new Vector(velocity))
        {
            Radius = radius;
            Mass = mass;
            Color = color;
        }
    }

}