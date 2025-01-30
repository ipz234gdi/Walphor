using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Walphor
{
    internal class Wall : GameObject
    {
        public int Id { get; set; }
        private Rectangle rectangle;

        public Wall(double x, double y, double width, double height, Canvas canvas, int id) : base(x, y, width, height, null, canvas)
        {
            Id = id;
            InitializeRectangle();
        }

        private void InitializeRectangle()
        {
            rectangle = new Rectangle
            {
                Width = this.Width,
                Height = this.Height,
                Fill = Brushes.Gray,
                Opacity = 0
            };

            this.canvas.Children.Add(rectangle);
            SetPosition(X, Y);
        }
        public void Move(double deltaX, double deltaY)
        {
            SetPosition(X + deltaX, Y + deltaY);
        }
        public override void SetPosition(double x, double y)
        {
            base.SetPosition(x, y);

            if (rectangle != null)
            {
                double leftPosition = x;
                double topPosition = y;

                Canvas.SetLeft(rectangle, leftPosition);
                Canvas.SetTop(rectangle, topPosition);
            }
        }
        public void SetColor(Color color)
        {
            if (rectangle != null)
            {
                rectangle.Fill = new SolidColorBrush(color);
            }
        }

        public override GameObject Clone(Canvas newCanvas, double X, double Y)
        {
            return new Wall(X, Y, Width, Height, newCanvas, Id);
        }
    }
}
