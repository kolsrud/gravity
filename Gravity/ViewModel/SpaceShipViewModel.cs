using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Gravity.Universe;

namespace Gravity.ViewModel
{
    internal class SpaceShipViewModel : MovingBodyViewModel
    {
        private readonly SpaceShip _spaceShip;
        public Ellipse SpaceShipGraphics { get; }
        public Line OrientationVector { get; }
        private RotateTransform rotation = new RotateTransform();

        public SpaceShipViewModel(SpaceShip s) : base(s)
        {
            _spaceShip = s;

            SpaceShipGraphics = new Ellipse()
            {
                Width = 2,
                Height = 2,
                Fill = Brushes.White
            };

            OrientationVector = new Line
            {
                Stroke = Brushes.White,
                StrokeThickness = 1,
                X1 = 5,
                X2 = 20,
                RenderTransform = rotation
            };
        }

        public override void AddToCanvas(Canvas canvas)
        {
            base.AddToCanvas(canvas);
            canvas.Children.Add(SpaceShipGraphics);
            canvas.Children.Add(OrientationVector);
        }

        public override void UpdatePosition(Position canvasCenter, Position universeCenter, double scale)
        {
            var center = GetCenter(canvasCenter, universeCenter, scale);
            SetPosition(SpaceShipGraphics, center.Subtract(1));
            SetPosition(OrientationVector, center);
            rotation.Angle = _spaceShip.Direction.Angle*(180/Math.PI);
            base.UpdatePosition(center);
        }
    }
}