using System;
using System.Collections.Generic;
using System.Linq;

namespace Gravity.Universe
{
    internal abstract class MovingBody
    {
        public string Name;
        public Position Position;
        public double VelocityX = 0;
        public double VelocityY = 0;
        public double AccelerationX = 0;
        public double AccelerationY = 0;

        internal MovingBody(string name, Position position, Vector velocity)
        {
            Name = name;
            Position = position;
            VelocityX = velocity.ComponentX;
            VelocityY = velocity.ComponentY;
        }

        public void Move(double timeScale)
        {
            Position.X += VelocityX * timeScale;
            Position.Y += VelocityY * timeScale;
        }

        public virtual double Accelerate(double timeScale, List<(Position Position, double Mass)> gravityWells)
        {
            if (!gravityWells.Any())
                return double.MaxValue;

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