using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace Gravity.ViewModel
{
    class ViewModel
    {
        public List<PlanetViewModel> PlanetViewModels { get; private set; }
        public List<SpaceShipViewModel> SpaceShipViewModels { get; private set; }
        public List<MovingBodyViewModel> MovingBodies => PlanetViewModels.Concat<MovingBodyViewModel>(SpaceShipViewModels).ToList();

        public TextBlock SelectedBody;
        public TextBlock Speed;

        public MovingBodyViewModel Center { get; private set; }

        public void Initialize(double scale, Canvas canvas, Universe.Universe universe)
        {
            PlanetViewModels = universe.Planets.Select(p => new PlanetViewModel(scale, p)).ToList();
            SpaceShipViewModels = universe.SpaceShips.Select(s => new SpaceShipViewModel(s)).ToList();
            Center = MovingBodies[0];
            SelectedBody = new TextBlock
            {
                Text = "Current reference: " + Center.Name.Text,
                Foreground = Brushes.LightGray
            };
            Speed = new TextBlock
            {
                Text = "Velocity: " + SpaceShipViewModels[0].RelativeVelocity(Center.Velocity),
                Foreground = Brushes.LightGray
            };

            AddToCanvas(canvas, SelectedBody, 0);
            AddToCanvas(canvas, Speed, 1);
            foreach (var body in MovingBodies)
            {
                body.AddToCanvas(canvas);
                body.Selected += OnSelectionChanged;
            }
        }

        private void AddToCanvas(Canvas canvas, TextBlock textBlock, int position)
        {
            canvas.Children.Add(textBlock);
            Canvas.SetLeft(textBlock, 10);
            Canvas.SetTop(textBlock, 10 + 20*position);
        }

        private void OnSelectionChanged(MovingBodyViewModel sender, object args)
        {
            Center = sender;
            SelectedBody.Text = "Current reference: " + sender.Name.Text;
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
            Speed.Text = "Velocity: " + SpaceShipViewModels[0].RelativeVelocity(Center.Velocity);
            UpdatePositions(canvasCenter, scale);
            UpdateVectors();
        }
    }
}