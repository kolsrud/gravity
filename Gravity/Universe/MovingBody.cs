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

        internal MovingBody(MovingBody relative, string name, Position position, Vector velocity) :
            this(name, position.Add(relative.Position), velocity.Add(relative.Velocity))
        {
        }

        public void Move(double timeScale)
        {
            var newP = Position.Add(Velocity.Scale(timeScale));
            Position.X = newP.X;
            Position.Y = newP.Y;
        }

        public virtual Vector BaseAcceleration => Vector.NullVector;

        public void Accelerate(double timeScale, List<(Position Position, double Mass)> gravityWells)
        {
            Acceleration = gravityWells.Where(well => well.Position != Position).Select(Accelerate)
                .Aggregate(BaseAcceleration, Vector.Add);
            Velocity = Velocity.Add(Acceleration.Scale(timeScale));
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