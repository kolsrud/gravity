using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Gravity.Universe;

namespace Gravity.ViewModel
{
    class ViewModel
    {
        public List<PlanetViewModel> PlanetViewModels { get; private set; }
        public List<SpaceShipViewModel> SpaceShipViewModels { get; private set; }
        public List<MovingBodyViewModel> MovingBodies => PlanetViewModels.Concat<MovingBodyViewModel>(SpaceShipViewModels).ToList();

        public MovingBodyViewModel Center { get; private set; }

        public void Initialize(double scale, Canvas canvas, Universe.Universe universe)
        {
            PlanetViewModels = universe.Planets.Select(p => new PlanetViewModel(scale, p)).ToList();
            SpaceShipViewModels = universe.SpaceShips.Select(s => new SpaceShipViewModel(s)).ToList();
            Center = MovingBodies[0];

            foreach (var body in MovingBodies)
            {
                body.AddToCanvas(canvas);
                body.Selected += (MovingBodyViewModel sender, object args) => Center = sender;
            }
        }

        private void UpdatePositions(Position canvasCenter, double scale)
        {
            MovingBodies.ForEach(m => m.UpdatePosition(canvasCenter, Center.Position, scale));
        }

        private void UpdateVectors()
        {
            MovingBodies.ForEach(m => m.UpdateVectors(Center.Velocity));
        }

        public void ToggleAccelerationVectors()
        {
            MovingBodies.ForEach(m => m.ToggleAccelerationVector());
        }

        public void ToggleVelocityVectors()
        {
            MovingBodies.ForEach(m => m.ToggleVelocityVector());
        }

        public void UpdateGraphics(Position canvasCenter, double scale)
        {
            UpdatePositions(canvasCenter, scale);
            UpdateVectors();
        }
    }
}