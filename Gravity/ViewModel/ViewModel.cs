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
        public TextBlock ScaleText;
        public TextBlock Distance;
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
            ScaleText = new TextBlock
            {
                Text = "Scale: " + scale,
                Foreground = Brushes.LightGray
            };
            Distance = new TextBlock
            {
                Text = "Distance: " + SpaceShipViewModels[0].Position.Distance(Center.Position),
                Foreground = Brushes.LightGray
            };
            Speed = new TextBlock
            {
                Text = "Velocity: " + SpaceShipViewModels[0].RelativeVelocity(Center.Velocity),
                Foreground = Brushes.LightGray
            };

            AddToCanvas(canvas, SelectedBody, 0);
            AddToCanvas(canvas, ScaleText, 1);
            AddToCanvas(canvas, Distance, 2);
            AddToCanvas(canvas, Speed, 3);
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
            Distance.Text = "Distance: " + FormatDistance(SpaceShipViewModels[0].Position.Distance(Center.Position));
            ScaleText.Text = "Scale: " + scale;
            Speed.Text = "Velocity: " + SpaceShipViewModels[0].RelativeVelocity(Center.Velocity);
            UpdatePositions(canvasCenter, scale);
            UpdateVectors();
        }

        private string FormatDistance(double distance)
        {
            if (distance > 100_000_000)
                return $"{distance / 1_000_000:# ##0.0} 10^3 km";
            // if (distance > 1_000_000)
            //     return $"{distance / 100_000:#0.0} 10^2 km";
            if (distance > 100_000)
                return $"{distance / 1000:# ##0.0} km";
            return $"{distance:#0} m";
        }
    }
}