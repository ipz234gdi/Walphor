using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Walphor
{
    internal class Card : GameObject
    {
        private static List<Card> AllCards = new List<Card>();
        private TextBlock infoTextBlock;
        private TextBlock costTextBlock;
        private Border border;
        public double Algo { get; set; }
        public double Def { get; set; }
        public double Atck { get; set; }
        public double Health { get; set; }
        public string Name { get; set; }
        public double Cost { get; set; }
        public string InfoCard { get; set; }
        public bool IsClone { get; private set; } = false;
        public Canvas Canvas { get; set; }
        public int Index { get; set; }
        private Card(double x, double y, double width, double height, string name, double def, double atck, double algo, double health, double cost, string imageSource, Canvas canvas, string infoCard, bool isClone, int index)
        : base(x, y, width, height, imageSource, canvas)
        {
            ZPosition(1);
            image.Cursor = Cursors.Hand;
            image.Tag = this;
            image.MouseEnter += Image_MouseEnter;
            image.MouseLeave += Image_MouseLeave;
            image.MouseLeftButtonDown += Image_MouseLeftButtonDown;

            Index = index;
            Def = def;
            Atck = atck;
            Algo = algo;
            Name = name;
            Health = health;
            Cost = cost;
            IsClone = isClone;
            InfoCard = infoCard;
            Canvas = canvas;
            AllCards.Add(this);

            if (!isClone)
            {
                costTextBlock = new TextBlock
                {
                    Text = $"Цiна: {cost}",
                    FontSize = 12,
                    FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                    Foreground = Brushes.Gold,
                    Background = Brushes.Black,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = width,
                    Height = 28,
                    Padding = new Thickness(0, 4, 0, 0)
                };

                Canvas.SetLeft(costTextBlock, x);
                Canvas.SetTop(costTextBlock, y + height + 16);
                Canvas.SetZIndex(costTextBlock, 2);
                canvas.Children.Add(costTextBlock);
            }

            infoTextBlock = new TextBlock
            {
                Text = $"Iм'я: {Name}\n" +
                       $"Оборона:  +{Def}\n" +
                       $"Здоров'я: +{Health}\n" +
                       $"Атака:        +{Atck}\n" +
                       $"Алгоритм: +{Algo}\n" +
                       $"Цiна:            {Cost}\n\n" +
                       $"{InfoCard}",
                FontSize = 16,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                Padding = new Thickness(10, 5, 10, 5),
            };
            double borderLeftX = x - 200;
            if (x < 200)
            {
                borderLeftX = x + width;
            }
            double borderTopY = y + 0;
            if (canvas.Height - y < height + 44)
            {
                borderTopY = y - 88;
                if (borderTopY < 0) borderTopY = 0;
            }

            border = new Border
            {
                Width = 200,
                Height = 300,
                Child = infoTextBlock,
                Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Data/Cards/Info.png")),
                    Stretch = Stretch.Fill
                },
                Opacity = 1,
                Visibility = System.Windows.Visibility.Hidden
            };
            Canvas.SetLeft(border, borderLeftX);
            Canvas.SetZIndex(border, 3);
            Canvas.SetTop(border, borderTopY);
            canvas.Children.Add(border);
        }
        public static Card Create(double x, double y, double width, double height, string name, double def, double atck, double algo, double health, double cost, string imageSource, Canvas canvas, string infoCard)
        {
            return new Card(x, y, width, height, name, def, atck, algo, health, cost, imageSource, canvas, infoCard, false, 13);
        }
        private void Image_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Картка клiкнута!");
            var card = sender as Image;
            if (card != null)
            {
                Debug.WriteLine("Це зображення картки: " + (card.Tag as Card)?.Name);
            }
        }
        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!Shop.isSelectingCards)
            {
                var image = sender as Image;
                if (image != null)
                {
                    image.Opacity = 0.8;
                    infoTextBlock.Visibility = System.Windows.Visibility.Visible;
                    border.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!Shop.isSelectingCards)
            {
                var image = sender as Image;
                if (image != null)
                {
                    image.Opacity = 1.0;
                    infoTextBlock.Visibility = System.Windows.Visibility.Hidden;
                    border.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }
        public static void ClearByName(string name, Canvas canvas)
        {
            List<Card> toRemove = new List<Card>();
            foreach (var card in AllCards)
            {
                if (card.Name == name)
                {
                    canvas.Children.Remove(card.image);
                    toRemove.Add(card);
                    Debug.WriteLine("Картка видалена" + card.Name);
                }
            }
            foreach (var card in toRemove)
            {
                AllCards.Remove(card);
            }
        }

        public static void MoveCardToCanvas(string cardName, Canvas newCanvas, double newX, double newY)
        {
            // Знаходимо всi картки з заданим iменем
            var cardsToMove = AllCards.Where(card => card.Name == cardName).ToList();

            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var card in cardsToMove)
                {
                    if (card.Canvas != null)
                    {
                        card.Canvas.Children.Remove(card.image);
                    }

                    card.Canvas = newCanvas;

                    Canvas.SetLeft(card.image, newX);
                    Canvas.SetTop(card.image, newY);

                    newCanvas.Children.Add(card.image);
                }
            });
        }
        public override GameObject Clone(Canvas newCanvas, double x, double y, string name, int index)
        {
            return new Card(x, y, Width, Height, name, Def, Atck, Algo, Health, Cost, ImageSource, newCanvas, InfoCard, true, index);
        }
    }
}
