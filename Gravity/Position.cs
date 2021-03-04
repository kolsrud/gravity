using System;

namespace Gravity
{
    class Position
    {
        public double X;
        public double Y;

        public Position(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Position((double x, double y) p) : this(p.x, p.y) { }

        public double Distance(Position p)
        {
            return Distance((p.X, p.Y));
        }
        public double Distance((double x, double y) p)
        {
            var x = Math.Abs(X - p.x);
            var y = Math.Abs(Y - p.y);

            return Math.Sqrt(x * x + y * y);
        }

        public double Angle(Position p)
        {
            return SubTract(p).Angle();
        }

        public Position SubTract(Position p)
        {
            return new Position(p.X - X, p.Y - Y);
        }

        public double Angle()
        {
            return Math.Atan2(Y, X);
        }
    }
}