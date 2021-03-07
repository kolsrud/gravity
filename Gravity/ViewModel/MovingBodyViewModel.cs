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

        private readonly MovingBody _m;
        public Position Position => _m.Position;

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
        }

        public void UpdateVectors(double scale)
        {
            VelocityVector.X2 = _m.VelocityX / 50;
            VelocityVector.Y2 = _m.VelocityY / 50;

            AccelerationVector.X2 = _m.AccelerationX * 1_000;
            AccelerationVector.Y2 = _m.AccelerationY * 1_000;
        }

        public void UpdatePosition(Position p)
        {
            SetPosition(VelocityVector, p);
            SetPosition(AccelerationVector, p);
            SetPosition(Name, p);
        }

        public static void SetPosition(UIElement e, Position p)
        {
            Canvas.SetLeft(e, p.X);
            Canvas.SetTop(e, p.Y);
        }

        public virtual void AddToCanvas(Canvas canvas)
        {
            canvas.Children.Add(VelocityVector);
            canvas.Children.Add(AccelerationVector);
            canvas.Children.Add(Name);
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