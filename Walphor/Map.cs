using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace Walphor
{
    internal class Map
    {
        private Canvas canvas;
        public double X { get; set; }
        public double Y { get; set; }
        private Image image { get; set; }
        public Map(Canvas canvas)
        {
            this.canvas = canvas;
            Panel.SetZIndex(this.canvas, 0);
        }
        public virtual void SetPosition(double x, double y)
        {
            X = x;
            Y = y;

            if (image != null)
            {
                Canvas.SetLeft(image, x);
                Canvas.SetTop(image, y);
            }
        }
        // Метод перемiщення карти
        public void Move(double deltaX, double deltaY)
        {
            // Здiйснюємо рух карти
            foreach (var child in canvas.Children)
            {
                if (child is UIElement element)
                {
                    double currentX = Canvas.GetLeft(element);
                    double currentY = Canvas.GetTop(element);
                    Canvas.SetLeft(element, currentX + deltaX);
                    Canvas.SetTop(element, currentY + deltaY);
                }
            }
        }
    }
}
