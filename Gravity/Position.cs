using System;

namespace Gravity
{
    public class Position
    {
        public double X;
        public double Y;

        public Position(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Position((double x, double y) p) : this(p.x, p.y) { }

        public double Distance()
        {
            return Distance((0,0));
        }

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
            return Subtract(p).Angle();
        }

        public Position Scale(double scale)
        {
            return new Position(X / scale, Y / scale);
        }

        public Position Add(Position p)
        {
            return new Position(p.X + X, p.Y + Y);
        }

        public Position Add(Vector v)
        {
            return Add(new Position(v.ComponentX, v.ComponentY));
        }

        public Position Subtract(Position p)
        {
            return new Position(p.X - X, p.Y - Y);
        }

        public Position Subtract(double d)
        {
            return new Position(X - d, Y - d);
        }

        public double Angle()
        {
            return Math.Atan2(Y, X);
        }
    }
}