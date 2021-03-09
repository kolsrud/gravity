using System;
using System.Collections.Generic;

namespace Gravity.Universe
{
    internal class SpaceShip : MovingBody
    {
        public Vector Direction = new Vector(0);
        private int _directionTicks = 0;
        private bool _engineFiring = false;

        public SpaceShip(string name, (double, double) position, (double, double) velocity) : base(name, new Position(position), new Vector(velocity))
        {
        }

        public void Rotate(int ticks)
        {
            _directionTicks += ticks;
            Direction = new Vector(-Math.PI * _directionTicks / 200, 1);
        }

        public bool EngineFiring => _engineFiring;

        public void Burn(bool status)
        {
            _engineFiring = status;
        }

        private const int BurnFactor = 10;

        public override void Accelerate(double timeScale, List<(Position Position, double Mass)> gravityWells)
        {
            if (_engineFiring)
            {
                Velocity = Velocity.Add(Direction.Scale(BurnFactor));
            }

            base.Accelerate(timeScale, gravityWells);
        }
    }
}