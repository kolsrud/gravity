using System;

namespace Gravity.Universe
{
    internal class SpaceShip : MovingBody
    {
        public Vector Direction;
        private int _directionTicks = 0;

        public SpaceShip(string name, (double, double) position, (double, double) velocity) : base(name, new Position(position), new Vector(velocity))
        {
            Direction = new Vector(0);
        }

        public void Rotate(int ticks)
        {
            _directionTicks += ticks;
            Direction.Angle = -Math.PI * _directionTicks / 100;
        }

        private const int BurnFactor = 10;
        public void Burn()
        {
            VelocityX += Direction.ComponentX * BurnFactor;
            VelocityY += Direction.ComponentY * BurnFactor;
        }
    }
}