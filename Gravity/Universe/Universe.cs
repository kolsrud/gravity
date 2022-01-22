using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Gravity.Universe
{
    public class Universe
    {
        public const double G = 6.674e-11;

        internal List<Planet> Planets = new List<Planet>();
        internal List<SpaceShip> SpaceShips = new List<SpaceShip>();
        private List<MovingBody> MovingBodies => Planets.Concat<MovingBody>(SpaceShips).ToList();

        public Universe()
        {
            const int earthRadius = 6_371_000;
            const int moonRadius = 1_737_100;
            const double up = -Math.PI / 2;
            const double down = -up;
            var sun = new Planet("Sun", 696.34e+6, 1.989e+30, (0,0), (0,0), Brushes.Yellow);
            var mercury = new Planet("Mercury", 2.4397e+6, 3.285e+23, (46.0012e+9, 0), (up, 58_980), Brushes.LightGray);
            var venus = new Planet("Venus", 6.0518e+6, 4.867e+24, (108.48e+9, 0), (up, 35_260), Brushes.Bisque);
            var earth = new Planet("Earth", earthRadius, 5.9725e+24, (147.1e+9, 0), (up, 30_290), Brushes.Blue);
            var moon = new Planet(earth, "Moon", moonRadius, 7.348e+22, (384_400_000, 0), (up, 1022), Brushes.LightGray);
            var mars = new Planet(sun, "Mars", 3.389e+6, 6.39e+23, (206.7e+9, 0), (up, 26_500), Brushes.Red);
            var jupiter = new Planet(sun, "Jupiter", 69.911e+6, 1.8982e+27, (740.52e+9, 0), (up, 13_720), Brushes.Red);

            var saturn = new Planet(sun, "Saturn", 58.232e+6, 5.683e+26, (1352.55e+9, 0), (up, 10_180), Brushes.Yellow);
            var uranus = new Planet(sun, "Uranus", 25.362e+6, 8.681e+25, (2741.302e+9, 0), (up, 7_110), Brushes.Blue);
            var neptune = new Planet(sun, "Neptune", 24.622e+6, 1.024e+26, (4444.449e+9, 0), (up, 5_500), Brushes.Green);
            var moon2 = new Planet(earth, "Moon 2", moonRadius, moon.Mass, (300_000_000, 0), (up, 622), Brushes.LightGray);
            var moon3 = new Planet(earth, "Moon 3", moonRadius, moon.Mass, (200_000_000, 0), (up, 822), Brushes.LightGray);
            var moon4 = new Planet(earth, "Moon 4", moonRadius, moon.Mass, (-100_000_000, 0), (down, 1122), Brushes.LightGray);
            var moon5 = new Planet("Moon 5", moonRadius, moon.Mass, (-500_000_000, 0), (down, 822), Brushes.LightGray);

            var solarSystem = new[] { earth, moon, sun, mercury, venus, mars, jupiter, saturn, uranus, neptune };
            // Planets.Add(earth);
            // Planets.Add(sun);
            // Planets.AddRange(new []{mercury, venus});
            // Planets.Add(moon);
            // Planets.Add(mars);
            // Planets.AddRange(new []{jupiter, saturn, uranus, neptune});
            // Planets.AddRange(new []{moon2, moon3, moon4});
            // Planets.AddRange(solarSystem);

            var moonSysten = new[] { earth, moon, moon2, moon3, moon4, moon5 };
            Planets.AddRange(moonSysten);

            var apollo = new SpaceShip(earth, "Apollo", (-150_000_000, 0), (down, 1000));
            SpaceShips.Add(apollo);
        }

        public void Step(double timeScale)
        {
            MovingBodies.ForEach(m => m.Move(timeScale));
            var gravityWells = Planets.Select(p => (p.Position, p.Mass)).ToList();
            foreach (var m in MovingBodies)
            {
                m.Accelerate(timeScale, gravityWells);
            }
        }
    }
}