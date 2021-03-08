using System;
using System.Collections.Generic;
using System.Linq;

namespace Gravity.Universe
{
    internal abstract class MovingBody
    {
        public string Name;
        public Position Position;
        public Vector Velocity;
        public Vector Acceleration;

        internal MovingBody(string name, Position position, Vector velocity)
        {
            Name = name;
            Position = position;
            Velocity = velocity;
            Acceleration = Vector.NullVector;
        }

        public void Move(double timeScale)
        {
            var newP = Position.Add(Velocity.Scale(timeScale));
            Position.X = newP.X;
            Position.Y = newP.Y;
        }

        public virtual void Accelerate(double timeScale, List<(Position Position, double Mass)> gravityWells)
        {
            Acceleration = Vector.Add(gravityWells.Where(well => well.Position != Position).Select(Accelerate));
            var v = Velocity.Add(Acceleration.Scale(timeScale));
            Velocity.Amplitude = v.Amplitude;
            Velocity.Angle = v.Angle;
        }

        private Vector Accelerate((Position Position, double Mass) gravityWell)
        {
            var d = Position.Distance(gravityWell.Position);
            var a = (Universe.G * gravityWell.Mass / (d * d));
            var angle = Position.Angle(gravityWell.Position);
            return new Vector(angle, a);
        }
    }
}