using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Gravity.Universe
{
    public class Universe
    {
        public const double G = 6.674e-11;

    }

    class Planet : MovingBody
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