using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Walphor
{
    internal class CardManager
    {
        private static CardManager _instance;

        public static CardManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CardManager();
                }
                return _instance;
            }
        }

        public List<Card> AllCards { get; } = new List<Card>();

        private CardManager() { }
    }

    internal class Card : GameObject
    {
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

            CardManager.Instance.AllCards.Add(this);

            if (!isClone)
            {
                costTextBlock = CreateTextBlock($"Цiна: {cost}", x, y + height + 16, 12, Brushes.Gold);
                canvas.Children.Add(costTextBlock);
            }

            infoTextBlock = CreateTextBlock(GetCardInfo(), x, y, 16, Brushes.White);
            border = CreateInfoBorder(x, y, width, height);
            canvas.Children.Add(border);
        }

        public static Card Create(double x, double y, double width, double height, string name, double def, double atck, double algo, double health, double cost, string imageSource, Canvas canvas, string infoCard)
        {
            return new Card(x, y, width, height, name, def, atck, algo, health, cost, imageSource, canvas, infoCard, false, 0);
        }

        private void Image_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"Картка {Name} клiкнута!");
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!Shop.isSelectingCards)
            {
                image.Opacity = 0.8;
                infoTextBlock.Visibility = Visibility.Visible;
                border.Visibility = Visibility.Visible;
            }
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!Shop.isSelectingCards)
            {
                image.Opacity = 1.0;
                infoTextBlock.Visibility = Visibility.Hidden;
                border.Visibility = Visibility.Hidden;
            }
        }

        public static void ClearByName(string name, Canvas canvas)
        {
            var toRemove = CardManager.Instance.AllCards.Where(card => card.Name == name).ToList();
            foreach (var card in toRemove)
            {
                canvas.Children.Remove(card.image);
                CardManager.Instance.AllCards.Remove(card);
                Debug.WriteLine($"Картка {card.Name} видалена");
            }
        }

        public static void MoveCardToCanvas(string cardName, Canvas newCanvas, double newX, double newY)
        {
            var cardsToMove = CardManager.Instance.AllCards.Where(card => card.Name == cardName).ToList();

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

        private TextBlock CreateTextBlock(string text, double x, double y, int fontSize, Brush color)
        {
            var textBlock = new TextBlock
            {
                Text = text,
                FontSize = fontSize,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                Foreground = color,
                Background = Brushes.Black,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = Width,
                Height = 28,
                Padding = new Thickness(0, 4, 0, 0)
            };

            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);
            Canvas.SetZIndex(textBlock, 2);
            return textBlock;
        }

        private Border CreateInfoBorder(double x, double y, double width, double height)
        {
            double borderLeftX = x < 200 ? x + width : x - 200;
            double borderTopY = (Canvas.Height - y < height + 44) ? y - 88 : y;

            var border = new Border
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
                Visibility = Visibility.Hidden
            };

            Canvas.SetLeft(border, borderLeftX);
            Canvas.SetZIndex(border, 3);
            Canvas.SetTop(border, borderTopY);
            return border;
        }

        private string GetCardInfo()
        {
            return $"Iм'я: {Name}\n" +
                   $"Оборона: +{Def}\n" +
                   $"Здоров'я: +{Health}\n" +
                   $"Атака: +{Atck}\n" +
                   $"Алгоритм: +{Algo}\n" +
                   $"Цiна: {Cost}\n\n" +
                   $"{InfoCard}";
        }
    }
}
