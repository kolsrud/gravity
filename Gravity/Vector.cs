using System;

namespace Gravity
{
    public class Vector
    {
        public double Angle { get; set; }
        public double Amplitude { get; set; }

        public double ComponentX => Amplitude * Math.Cos(Angle);
        public double ComponentY => Amplitude * Math.Sin(Angle);

        public Vector(double angle, double amplitude = 1)
        {
            Angle = angle;
            Amplitude = amplitude;
        }

        public Vector((double angle, double amplitude) v) : this(v.angle, v.amplitude){}
    }
}