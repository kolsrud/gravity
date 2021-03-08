using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Gravity.Universe;

namespace Gravity.ViewModel
{
    internal abstract class MovingBodyViewModel
    {
        private bool VelocityVectorActive = true;
        public Line VelocityVector { get; }
        private bool AccelerationVectorActive = true;
        public Line AccelerationVector { get; }
        public TextBlock Name { get; }
        public Ellipse SelectionCircle { get; }

        private readonly MovingBody _m;
        public Position Position => _m.Position;
        public Vector Velocity => _m.Velocity;

        private const int SelectionCircleSize = 30;

        public event SelectionEventHandler Selected;
        internal delegate void SelectionEventHandler(MovingBodyViewModel sender, object args);

        private void OnSelected()
        {
            if (this.Selected != null)
                Selected(this, null);
        }

        protected MovingBodyViewModel(MovingBody m)
        {
            _m = m;

            VelocityVector = new Line
            {
                Stroke = Brushes.Green,
                StrokeThickness = 1
            };

            AccelerationVector = new Line
            {
                Stroke = Brushes.Red,
                StrokeThickness = 1
            };

            Name = new TextBlock
            {
                Text = m.Name,
                Foreground = Brushes.White
            };

            SelectionCircle = new Ellipse
            {
                Width = SelectionCircleSize,
                Height = SelectionCircleSize,
                Stroke = Brushes.Transparent,
                Fill = Brushes.Transparent,
                Visibility = Visibility.Visible
            };

            SelectionCircle.MouseEnter += (sender, args) => SelectionCircle.Stroke = Brushes.White;
            SelectionCircle.MouseLeave += (sender, args) => SelectionCircle.Stroke = Brushes.Transparent;
            SelectionCircle.MouseLeftButtonDown += (sender, args) => OnSelected();
        }

        public Vector RelativeVelocity(Vector referenceVector)
        {
            return _m.Velocity.Subtract(referenceVector);
        }

        public void UpdateVectors(Vector referenceVector)
        {
            var v = RelativeVelocity(referenceVector).Scale((double)1 / 50);
            VelocityVector.X2 = v.ComponentX;
            VelocityVector.Y2 = v.ComponentY;

            AccelerationVector.X2 = _m.Acceleration.ComponentX * 1_000;
            AccelerationVector.Y2 = _m.Acceleration.ComponentY * 1_000;
        }

        public void UpdatePosition(Position p)
        {
            SetPosition(VelocityVector, p);
            SetPosition(AccelerationVector, p);
            SetPosition(Name, p);
            SetPosition(SelectionCircle, p.Subtract((double) SelectionCircleSize/2));
        }

        public static void SetPosition(UIElement e, Position p)
        {
            Canvas.SetLeft(e, p.X);
            Canvas.SetTop(e, p.Y);
        }

        protected abstract IEnumerable<UIElement> GetCustomGraphics();

        public void AddToCanvas(Canvas canvas)
        {
            canvas.Children.Add(VelocityVector);
            canvas.Children.Add(AccelerationVector);
            foreach (var customGraphic in GetCustomGraphics())
            {
                canvas.Children.Add(customGraphic);
            }
            canvas.Children.Add(Name);
            canvas.Children.Add(SelectionCircle);
        }

        public void ToggleAccelerationVector()
        {
            AccelerationVectorActive = !AccelerationVectorActive;
            AccelerationVector.StrokeThickness = AccelerationVectorActive ? 1 : 0;
        }

        public void ToggleVelocityVector()
        {
            VelocityVectorActive = !VelocityVectorActive;
            VelocityVector.StrokeThickness = VelocityVectorActive ? 1 : 0;
        }

        protected Position GetCenter(Position canvasCenter, Position universeCenter, double scale)
        {
            return canvasCenter.Add(universeCenter.Subtract(_m.Position).Scale(scale));
        }

        public abstract void UpdatePosition(Position canvasCenter, Position universeCenter, double scale);
    }
}