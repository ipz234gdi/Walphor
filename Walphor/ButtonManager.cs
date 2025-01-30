using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace Walphor
{
    internal class ButtonManager : GameObject
    {
        public string Tag { get; set; }
        public event RoutedEventHandler Click;
        

        public ButtonManager(double x, double y, double width, double height, string imageSource, Canvas canvas)
        : base(x, y, width, height, imageSource, canvas)
        {
            ZPosition(10);
            image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
            image.Cursor = Cursors.Hand;
            Tag = imageSource;
        }

        public static ButtonManager Create(double x, double y, double width, double height, string imageSource, Canvas canvas)
        {
            return new ButtonManager(x, y, width, height, imageSource, canvas);
        }

        public void ToggleVisibility(bool isVisible)
        {
            var animation = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(0.2),
                From = isVisible ? 0 : 1,
                To = isVisible ? 1 : 0,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            animation.Completed += (s, e) =>
            {
                if (!isVisible)
                {
                    Task.Delay(1000).ContinueWith(t =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            image.Visibility = Visibility.Hidden;
                            RemoveObject();
                        });
                    });
                }
            };

            if (isVisible) image.Visibility = Visibility.Visible;
            image.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Click?.Invoke(sender, new RoutedEventArgs());
        }

        public void RemoveObject()
        {
            if (canvas != null && image != null)
            {
                canvas.Children.Remove(image);
            }
        }
    }

}
