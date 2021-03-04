using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Gravity.Universe
{
    public class Universe
    {
        public const double G = 6.674e-11;

        internal List<Planet> Planets = new List<Planet>();

        public Universe()
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

            Planets.AddRange(new[] {earth, moon, moon2, moon3, moon4});
        }
    }

    internal class Planet : MovingBody
    {
        public double Mass { get; }

        public Planet(double radius, double mass, (double, double) position, SolidColorBrush color)
            : base(new Circle(new Position(position), color, (int)radius))
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
            return new[] { Graphics, VelocityVector };
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
            var a = (Universe.G * gravityWell.Mass / (d * d));
            var angle = Position.Angle(gravityWell.Position);
            var dx = a * Math.Cos(angle) * stepScale;
            var dy = a * Math.Sin(angle) * stepScale;
            VelocityX += dx;
            VelocityY += dy;

            return d;
        }
    }

}