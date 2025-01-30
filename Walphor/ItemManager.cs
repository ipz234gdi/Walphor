using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Xml.Linq;

namespace Walphor
{
    internal class ItemManager : GameObject
    {
        public int Id { get; set; }
        private ItemManager(double x, double y, double width, double height, string imageSource, Canvas canvas, int id)
        : base(x, y, width, height, imageSource, canvas)
        {
            Id = id;
            ZPosition(1);
        }
        public static ItemManager Create(double x, double y, double width, double height, string imageSource, Canvas canvas, int id)
        {
            return new ItemManager(x, y, width, height, imageSource, canvas, id);
        }
        public void SetSize(double width, double height)
        {
            Width = width;
            Height = height;
            image.Width = width;
            image.Height = height;
        }
        public override Point GetPosition()
        {
            return base.GetPosition();
        }
        public override void SetPosition(double x, double y)
        {
            base.SetPosition(x, y);
        }
        public void Move(double deltaX, double deltaY)
        {
            SetPosition(X + deltaX, Y + deltaY);
        }
        public override GameObject Clone(Canvas newCanvas, double X, double Y)
        {
            return new ItemManager(X, Y, Width, Height, ImageSource, newCanvas, Id);
        }
    }
}
