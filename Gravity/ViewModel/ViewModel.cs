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

        public Label SelectedBody;
        public Label ScaleText;
        public Label TimeScaleText;
        public Label Distance;
        public Label Speed;
        public Label Acceleration;

        public MovingBodyViewModel Center { get; private set; }

        public void Initialize(double scale, Canvas canvas, Universe.Universe universe)
        {
            PlanetViewModels = universe.Planets.Select(p => new PlanetViewModel(scale, p)).ToList();
            SpaceShipViewModels = universe.SpaceShips.Select(s => new SpaceShipViewModel(s)).ToList();
            Center = MovingBodies[0];
            SelectedBody = new Label("Current reference:", Center.Name.Text);
            ScaleText = new Label("Scale:");
            TimeScaleText = new Label("Time scale:");
            Distance = new Label("Distance:");
            Speed = new Label("v:");
            Acceleration = new Label("Δv:");

            AddToCanvas(canvas, SelectedBody, 0);
            AddToCanvas(canvas, ScaleText, 1);
            AddToCanvas(canvas, TimeScaleText, 2);
            AddToCanvas(canvas, Distance, 3);
            AddToCanvas(canvas, Speed, 4);
            AddToCanvas(canvas, Acceleration, 5);
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
            SelectedBody.SetText(sender.Name.Text);
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

        public void UpdateGraphics(Position canvasCenter, double scale, double timeScale)
        {
            ScaleText.SetText(scale);
            TimeScaleText.SetText(timeScale);
            Distance.SetText(FormatDistance(SpaceShipViewModels[0].Position.Distance(Center.Position)));
            Speed.SetText(SpaceShipViewModels[0].RelativeVelocity(Center.Velocity).ToString("m/s"));
            Acceleration.SetText(SpaceShipViewModels[0].Acceleration.ToString("m/s²"));
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

    class Label : TextBlock
    {
        private readonly string _label;

        public Label(string label)
        {
            _label = label;
            Foreground = Brushes.LightGray;
            SetText("");
        }

        public Label(string label, string defaultText) : this(label)
        {
            SetText(defaultText);
        }

        public void SetText(string text)
        {
            Text = _label + " " + text;
        }

        public void SetText(double d)
        {
            Text = _label + " " + d;
        }

        public void SetText(Vector relativeVelocity)
        {
            Text = _label + " " + relativeVelocity;
        }
    }
}