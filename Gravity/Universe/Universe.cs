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
            var earth = new Planet("Earth", earthRadius, 5.9725e+24, (0, 0), (0, 0), Brushes.Blue);
            var moon = new Planet("Moon", moonRadius, 7.348e+22, (384_400_000, 0), (1022, -Math.PI / 2), Brushes.LightGray);
            var moon2 = new Planet("Moon 2", moonRadius, 7.348e+22, (300_000_000, 0), (622, -Math.PI / 2), Brushes.LightGray);
            var moon3 = new Planet("Moon 3", moonRadius, 7.348e+22, (200_000_000, 0), (822, -Math.PI / 2), Brushes.LightGray);
            var moon4 = new Planet("Moon 4", moonRadius, 7.348e+22, (-100_000_000, 0), (1122, Math.PI / 2), Brushes.LightGray);
            var moon5 = new Planet("Moon 5", moonRadius, 7.348e+22, (-120_000_000, 0), (1322, Math.PI / 2), Brushes.LightGray);

            Planets.AddRange(new[] {earth});
            Planets.AddRange(new[] {moon});
            Planets.AddRange(new []{moon2, moon3, moon4});
        }

        public double Step(int timeScale)
        {
            Planets.ForEach(p => p.Move(timeScale));
            var gravityWells = Planets.Select(p => (p.Position, p.Mass)).ToList();
            return Planets.Select(p => p.Accelerate(timeScale, gravityWells)).Min();
        }
    }

    internal class Planet : MovingBody
    {
        public double Radius { get; }
        public double Mass { get; }
        public SolidColorBrush Color { get; }

        public Planet(string name, double radius, double mass, (double, double) position, (double, double) velocity, SolidColorBrush color)
            : base(name, new Position(position))
        {
            Radius = radius;
            Mass = mass;
            Color = color;
            SetVelocity(velocity.Item1, velocity.Item2);
        }
    }

    internal abstract class MovingBody
    {
        public string Name;
        public Position Position;
        public double VelocityX = 0;
        public double VelocityY = 0;
        public double AccelerationX = 0;
        public double AccelerationY = 0;

        internal MovingBody(string name, Position position)
        {
            Name = name;
            Position = position;
        }

        public void SetVelocity(double velocity, double angle)
        {
            VelocityX = velocity * Math.Cos(angle);
            VelocityY = velocity * Math.Sin(angle);
        }

        public void Move(int timeScale)
        {
            Position.X += VelocityX * timeScale;
            Position.Y += VelocityY * timeScale;
        }

        public double Accelerate(int timeScale, List<(Position Position, double Mass)> gravityWells)
        {
            AccelerationX = 0;
            AccelerationY = 0;
            var minDistance = gravityWells.Where(well => well.Position != Position).Select(Accelerate).Min();
            VelocityX += AccelerationX * timeScale;
            VelocityY += AccelerationY * timeScale;
            return minDistance;
        }

        private double Accelerate((Position Position, double Mass) gravityWell)
        {
            var d = Position.Distance(gravityWell.Position);
            var a = (Universe.G * gravityWell.Mass / (d * d));
            var angle = Position.Angle(gravityWell.Position);
            AccelerationX += a * Math.Cos(angle);
            AccelerationY += a * Math.Sin(angle);
            return d;
        }
    }

}