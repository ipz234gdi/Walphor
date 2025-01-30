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
    internal class GameObject
    {
        protected internal Image image;
        protected Canvas canvas;

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        
        public string ImageSource { get; set; }


        public GameObject(double x, double y, double width, double height, string imageSource, Canvas canvas)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            
            this.canvas = canvas;
            ImageSource = imageSource;

            InitializeImage();
            SetPosition(x, y);
        }
        private void InitializeImage()
        {
            if (!string.IsNullOrWhiteSpace(ImageSource))
            {
                image = new Image
                {
                    Width = Width,
                    Height = Height,
                    Source = new BitmapImage(new Uri(ImageSource, UriKind.RelativeOrAbsolute))
                };
                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);

                canvas.Children.Add(image);
            }
        }
        public void SetImageSource(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentException("Шлях до зображення не може бути null або порожнiм", nameof(source));
            }
            ImageSource = source;
            image.Source = new BitmapImage(new Uri(source, UriKind.RelativeOrAbsolute));
        }
        public Image GetImage()
        {
            return image;
        }

        public void ChangeSize(double width, double height)
        {
            Width = width;
            Height = height;
            image.Width = width;
            image.Height = height;
        }
        public void ZPosition(int z)
        {
            if (image != null)
            {
                Canvas.SetZIndex(image, z);
            }
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
        public virtual Point GetPosition()
        {
            return new Point(X, Y);
        }
        public virtual GameObject Clone(Canvas newCanvas, double x, double y)
        {
            return new GameObject(X, Y, Width, Height, ImageSource, newCanvas);
        }
        public virtual GameObject Clone(Canvas newCanvas, double x, double y, string name, int index)
        {
            return new GameObject(X, Y, Width, Height, ImageSource, newCanvas);
        }
    }
}
