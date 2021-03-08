using System;
using System.Collections.Generic;
using System.Windows;
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
        public Line BurnVector { get; }
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

            BurnVector = new Line
            {
                Stroke = Brushes.Yellow,
                StrokeThickness = 1,
                X1 = -5,
                X2 = -20,
                RenderTransform = rotation
            };
        }

        protected override IEnumerable<UIElement> GetCustomGraphics()
        {
            return new UIElement[] {SpaceShipGraphics, OrientationVector, BurnVector};
        }

        public override void UpdatePosition(Position canvasCenter, Position universeCenter, double scale)
        {
            BurnVector.Visibility = _spaceShip.EngineFiring ? Visibility.Visible : Visibility.Hidden;
            var center = GetCenter(canvasCenter, universeCenter, scale);
            SetPosition(SpaceShipGraphics, center.Subtract(1));
            SetPosition(OrientationVector, center);
            SetPosition(BurnVector, center);
            rotation.Angle = _spaceShip.Direction.Angle*(180/Math.PI);
            base.UpdatePosition(center);
        }
    }
}