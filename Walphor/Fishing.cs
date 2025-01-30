using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Animation;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Xml.Linq;

namespace Walphor
{
    internal class Fishing
    {
        private Canvas canvasFishing;
        private Grid gridGame;
        private UIManager uiManager;
        private Character player_battle_org;
        private GameEngine gameEngine;
        private bool isVisible;
        private Image exitButtonImage;

        private double cost;
        private bool isBuoyMoving = true;
        private Image buoyImage;
        private Image fishImage;
        private Storyboard storyboard;
        private DoubleAnimation buoyAnimation;
        private Random random = new Random();

        private TextBox resultTextBox;
        private List<Fish> fishTypes = new List<Fish>
        {
            new Fish("Карась", 10),
            new Fish("Щука", 20),
            new Fish("Сом", 15),
            new Fish("Лящ", 12),
            new Fish("Форель", 25),
            new Fish("Окунь", 18),
            new Fish("Короп", 30),
            new Fish("Судак", 22),
            new Fish("Голавль", 14),
            new Fish("Осетер", 35),
            new Fish("Минь", 17),
            new Fish("Хек", 28),
        };

        public Fishing(Grid gridGame, Character character, UIManager uIManager, GameEngine gameEngine)
        {
            this.gridGame = gridGame;
            this.player_battle_org = character;
            this.uiManager = uIManager;
            this.gameEngine = gameEngine;
            InitializeCanvas();
        }

        private void InitializeCanvas()
        {
            canvasFishing = new Canvas
            {
                Name = "Fishing",
                Width = 1280,
                Height = 720,
                Visibility = Visibility.Hidden
            };
            gridGame.Children.Add(canvasFishing);
            Panel.SetZIndex(canvasFishing, 1);

            gridGame.Focusable = true;
            gridGame.Focus();

            resultTextBox = new TextBox
            {
                Text = "",
                FontSize = 21,
                IsHitTestVisible = false,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                Foreground = Brushes.Black,
                TextAlignment = TextAlignment.Center,
                Width = 250,
                Height = 50,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                
            };
            Canvas.SetZIndex(resultTextBox, 5);
            Canvas.SetLeft(resultTextBox, 1280 / 2.0 - 130);
            Canvas.SetTop(resultTextBox, 920 / 2.0 - 170);
            canvasFishing.Children.Add(resultTextBox);

            exitButtonImage = new Image
            {
                Width = 30,
                Height = 30,
                Source = new BitmapImage(new Uri("pack://application:,,,/Data/Button/Exit_button.png")),
                Visibility = Visibility.Collapsed,
            };

            // Додаємо подiї для змiни кольору Border при наведеннi та натисканнi
            exitButtonImage.MouseEnter += CustomButtonText_MouseEnter;
            exitButtonImage.MouseLeave += CustomButtonText_MouseLeave;
            exitButtonImage.MouseLeftButtonDown += Exit_Button_Click;
            Canvas.SetZIndex(exitButtonImage, 5);
            Canvas.SetLeft(exitButtonImage, 1280 / 2.0 + 240);
            Canvas.SetTop(exitButtonImage, 920 / 2.0 - 170);
            canvasFishing.Children.Add(exitButtonImage);

            AddImage("pack://application:,,,/Data/Fishing/FishingMenu.png", 1280 / 2.0, 920 / 2.0 - 100, 584, 168);

            fishImage = AddImage("pack://application:,,,/Data/Fishing/Fish.png", 1280 / 2.0, 920 / 2.0 - 68, 44, 32);

            buoyImage = AddImage("pack://application:,,,/Data/Fishing/Buiok.png", 1280 / 2.0, 920 / 2.0 - 100, 20, 64);

            CreateBuoyAnimation();

            gridGame.KeyDown += OnKeyDown;
            
        }
        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            ToggleVisibility();
            exitButtonImage.Visibility = Visibility.Hidden;
        }

        private void CustomButtonText_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Image Exit_button)
            {
                Exit_button.Source = new BitmapImage(new Uri("pack://application:,,,/Data/Button/Exit_button_2.png"));
            }
        }

        private void CustomButtonText_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Image Exit_button)
            {
                Exit_button.Source = new BitmapImage(new Uri("pack://application:,,,/Data/Button/Exit_button.png"));
            }
        }
        private void CreateBuoyAnimation()
        {
            storyboard = new Storyboard();
            buoyAnimation = new DoubleAnimation
            {
                From = 360,
                To = 900,
                Duration = new Duration(TimeSpan.FromSeconds((-cost + 110) / 150)),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            Storyboard.SetTarget(buoyAnimation, buoyImage);
            Storyboard.SetTargetProperty(buoyAnimation, new PropertyPath(Canvas.LeftProperty));
            storyboard.Children.Add(buoyAnimation);
            storyboard.Begin();
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && isBuoyMoving)
            {
                storyboard.Pause(canvasFishing);
                isBuoyMoving = false;
                Debug.WriteLine("Fish");

                double buoyPosition = Canvas.GetLeft(buoyImage);
                if (buoyPosition >= Canvas.GetLeft(fishImage) - 16 && buoyPosition <= Canvas.GetLeft(fishImage) + 36)
                {
                    CatchFish();
                } else resultTextBox.Text = "Нiчого не пiймали";
                exitButtonImage.Visibility = Visibility.Visible;
            }
        }
        private void CatchFish()
        {
            var possibleFishes = fishTypes.Where(f => f.Cost <= cost).ToList();
            if (possibleFishes.Count > 0)
            {
                var caughtFish = possibleFishes[random.Next(possibleFishes.Count)];
                resultTextBox.Text = $"Ви пiймали: {caughtFish.Name}\nВартiсть: {caughtFish.Cost}";
                Debug.WriteLine(resultTextBox.Text);
                player_battle_org.Money += caughtFish.Cost;
                uiManager.UpdateMoney(player_battle_org.Money);
            }
            else
            {
                resultTextBox.Text = "Нiчого не пiймали";
                Debug.WriteLine(resultTextBox.Text);
            }
            
        }
        private void SetRandomFishPosition()
        {
            int minX = 368;
            int maxX = 864;
            cost = random.Next(10, 100);
            Debug.WriteLine(cost);
            int fishX = random.Next(minX, maxX);
            Canvas.SetLeft(fishImage, fishX);
        }
        private Image AddImage(string imagePath, double x, double y, double width, double height)
        {
            Image image = new Image();

            BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
            image.Source = bitmapImage;
            image.Width = width;
            image.Height = height;

            Canvas.SetLeft(image, x - width / 2.0);
            Canvas.SetTop(image, y - height / 2.0);
            canvasFishing.Children.Add(image);

            return image;
        }
        public void ToggleVisibility()
        {
            var animation = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(0.2),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            if (!isVisible)
            {
                SetRandomFishPosition();
                resultTextBox.Text = "";
                canvasFishing.Visibility = Visibility.Visible;
                exitButtonImage.Visibility = Visibility.Hidden;
                isBuoyMoving = true;
                gameEngine.inFishing = true;
                gameEngine.ToggleCursorVisibility(true);
                animation.From = 0;
                animation.To = 1;

                storyboard.Begin(canvasFishing, true);
            }
            else
            {
                animation.From = 1;
                animation.To = 0;
                animation.Completed += (s, e) =>
                {
                    canvasFishing.Visibility = Visibility.Hidden;
                    storyboard.Stop(canvasFishing);
                };
                gameEngine.inFishing = false;
                isBuoyMoving = false;
                gameEngine.ToggleCursorVisibility(false);
            }

            canvasFishing.BeginAnimation(Canvas.OpacityProperty, animation);
            isVisible = !isVisible;
        }
    }
    public class Fish
    {
        public string Name { get; set; }
        public int Cost { get; set; }

        public Fish(string name, int cost)
        {
            Name = name;
            Cost = cost;
        }
    }
}
