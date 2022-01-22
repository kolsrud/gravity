using System;

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

        public SpaceShip(MovingBody relative, string name, (double, double) position, (double, double) velocity) :
            base(relative, name, new Position(position), new Vector(velocity))
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

        private const double BurnFactor = 0.1;

        public override Vector BaseAcceleration => _engineFiring ? Direction.Scale(BurnFactor) : Vector.NullVector;
    }
}