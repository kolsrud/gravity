using System;
using System.Collections.Generic;

namespace Gravity.Universe
{
    internal class SpaceShip : MovingBody
    {
        public Vector Direction;
        private int _directionTicks = 0;

        private bool _engineFiring = false;

        public SpaceShip(string name, (double, double) position, (double, double) velocity) : base(name, new Position(position), new Vector(velocity))
        {
            Direction = new Vector(0);
        }

        public void Rotate(int ticks)
        {
            _directionTicks += ticks;
            Direction.Angle = -Math.PI * _directionTicks / 100;
        }

        public void Burn(bool status)
        {
            _engineFiring = status;
        }

        private const int BurnFactor = 10;

        public override double Accelerate(double timeScale, List<(Position Position, double Mass)> gravityWells)
        {
            if (_engineFiring)
            {
                VelocityX += Direction.ComponentX * BurnFactor;
                VelocityY += Direction.ComponentY * BurnFactor;
            }

            return base.Accelerate(timeScale, gravityWells);
        }
    }
}