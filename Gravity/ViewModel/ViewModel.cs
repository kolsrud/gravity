using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Gravity.ViewModel
{
    class ViewModel
    {
        public List<PlanetViewModel> PlanetViewModels { get; private set; }
        public List<SpaceShipViewModel> SpaceShipViewModels { get; private set; }
        public List<MovingBodyViewModel> MovingBodies => PlanetViewModels.Concat<MovingBodyViewModel>(SpaceShipViewModels).ToList();

        public Position Center { get; private set; }

        public void Initialize(double scale, Canvas canvas, Universe.Universe universe)
        {
            PlanetViewModels = universe.Planets.Select(p => new PlanetViewModel(scale, p)).ToList();
            SpaceShipViewModels = universe.SpaceShips.Select(s => new SpaceShipViewModel(s)).ToList();
            Center = MovingBodies[1].Position;

            foreach (var body in MovingBodies)
            {
                body.AddToCanvas(canvas);
                body.Selected += (MovingBodyViewModel sender, object args) => Center = sender.Position;
            }
        }

        private void UpdatePositions(Position canvasCenter, double scale)
        {
            MovingBodies.ForEach(m => m.UpdatePosition(canvasCenter, Center, scale));
        }

        private void UpdateVectors(double scale)
        {
            MovingBodies.ForEach(m => m.UpdateVectors(scale));
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
            UpdateVectors(scale);
        }
    }
}