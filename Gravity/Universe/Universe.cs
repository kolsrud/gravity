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
            var earth = new Planet("Earth", earthRadius, 5.9725e+24, (0, 0), (0, 0), Brushes.Blue);
            var moon = new Planet("Moon", moonRadius, 7.348e+22, (384_400_000, 0), (up, 1022), Brushes.LightGray);
            var moon2 = new Planet("Moon 2", moonRadius, 7.348e+22, (300_000_000, 0), (up, 622), Brushes.LightGray);
            var moon3 = new Planet("Moon 3", moonRadius, 7.348e+22, (200_000_000, 0), (up, 822), Brushes.LightGray);
            var moon4 = new Planet("Moon 4", moonRadius, 7.348e+22, (-100_000_000, 0), (down, 1122), Brushes.LightGray);
            var moon5 = new Planet("Moon 5", moonRadius, 7.348e+22, (-120_000_000, 0), (down, 1322), Brushes.LightGray);

            Planets.Add(earth);
            Planets.Add(moon);
            // Planets.AddRange(new []{moon2, moon3, moon4});

//            var apollo = new SpaceShip("Apollo", (-150_000_000, 0), (up, 1000));
            var apollo = new SpaceShip("Apollo", (-150_000_000, 0), (down, 1000));
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