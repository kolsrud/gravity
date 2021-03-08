using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Gravity
{
    public class Vector
    {
        public double Angle { get; }
        public double Amplitude { get; }

        public double ComponentX => Amplitude * Math.Cos(Angle);
        public double ComponentY => Amplitude * Math.Sin(Angle);

        public Vector(double angle, double amplitude = 1)
        {
            Angle = angle;
            Amplitude = amplitude;
        }

        public Vector(Position p)
        {
            Angle = p.Angle();
            Amplitude = p.Distance();
        }

        public Vector((double angle, double amplitude) v) : this(v.angle, v.amplitude){}

        public Vector Add(Vector v)
        {
            return new Vector(new Position(ComponentX + v.ComponentX, ComponentY + v.ComponentY));
        }

        public Vector Subtract(Vector v)
        {
            return Add(new Vector(v.Angle + Math.PI, v.Amplitude));
        }

        public Vector Scale(double timeScale)
        {
            return new Vector(Angle, Amplitude*timeScale);
        }

        public static Vector Add(Vector v0, Vector v1)
        {
            return v0.Add(v1);
        }

        public static Vector NullVector => new Vector(0,0);

        public static Vector Add(IEnumerable<Vector> vs)
        {
            return vs.Aggregate(NullVector, Add);
        }

        public override string ToString()
        {
            var v = Amplitude > 500 ? $"{Amplitude/1000:#0.00} km/s" : $"{Amplitude:#0.00} m/s";

            return $"{v}, {(Angle / Math.PI):#0.00} π";
        }
    }
}