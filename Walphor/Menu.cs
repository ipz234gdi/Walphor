using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Xml.Linq;

namespace Walphor
{
    internal class Menu
    {
        private Canvas canvasMenu;
        private Canvas canvasGame;
        private GameEngine engine;
        private Character character;
        private Grid gridGame;
        private UIManager uiManager;
        private SoundManager soundManager;
        private UICreator uiCreator;
        private bool isVisible = true;
        public Menu(Grid gridGame, Canvas canvasGame, GameEngine engine, UIManager uIManager, Character character, SoundManager soundManager)
        {
            this.gridGame = gridGame;
            this.canvasGame = canvasGame;
            this.engine = engine;
            this.uiManager = uIManager;
            this.character = character;
            this.soundManager = soundManager;

            InitializeMenu();
        }

        private void InitializeMenu()
        {
            canvasMenu = new Canvas
            {
                Name = "MenuCanvas",
                Width = 1280,
                Height = 720,

                Visibility = Visibility.Visible,
            };

            uiManager.ToggleBlurEffect(true);
            ToggleVisibility();
            gridGame.Children.Add(canvasMenu);

            var saveGame = new SaveGame();

            Panel.SetZIndex(canvasMenu, 100);
            canvasMenu.RenderTransform = new TranslateTransform();

            AddImage("pack://application:,,,/Data/Menu/Menu.png", (canvasMenu.Width / 2.0), (canvasMenu.Height / 2.0), 384, 512);

            AddButton(1280 / 2.0 - 180, 920 / 2.0 - 340, "Грати", canvasMenu, () =>
            {
                ToggleVisibility();
                engine.StartTimer();
                uiCreator.taskBorder.Visibility = Visibility.Visible;
                uiManager.MoveImage("Coin", 100, 20);
                uiManager.ToggleBlurEffect(false);

                uiManager.OpacityObject(uiCreator.levelTextBlock, 1);
                uiManager.OpacityImage("Level", 1);
                uiManager.OpacityObject(uiCreator.taskTextBlock, 1);

                engine.inMenu = false;
                engine.SaveCurrentGame();
                engine.LoadGame(character.Name);
                soundManager.PlayClickSound(engine.GetSoundPath("klik.mp3"));
            });

            AddButton(1280 / 2.0 - 180, 920 / 2.0 - 275, "Збереження", canvasMenu, () =>
            {
                engine.SaveCurrentGame();
                soundManager.PlayClickSound(engine.GetSoundPath("klik.mp3"));
            });

            AddButton(1280 / 2.0 - 180, 920 / 2.0 - 210, "Профіль", canvasMenu, () =>
            {
                uiManager.UpdateStatistics(GameEngine.GetDatabasePath());
                uiCreator.ProfileBorder.Visibility = Visibility.Visible;
                uiCreator.ProfileButton.Visibility = Visibility.Visible;
                ToggleVisibility();
                engine.inMenu = false;
                soundManager.PlayClickSound(engine.GetSoundPath("klik.mp3"));
            });

            AddButton(1280 / 2.0 - 180, 920 / 2.0 - 145, "Правила гри", canvasMenu, () =>
            {
                engine.inRules = true;
                uiManager.ShowRulesMenu();
                soundManager.PlayClickSound(engine.GetSoundPath("klik.mp3"));
            });

            AddButton(1280 / 2.0 - 180, 920 / 2.0 - 80, "Статистика", canvasMenu, () =>
            {
                uiManager.UpdateStatistics(GameEngine.GetDatabasePath());
                uiManager.StatisticsVisible();
                ToggleVisibility();
                engine.inMenu = false;
                soundManager.PlayClickSound(engine.GetSoundPath("klik.mp3"));
            });

            AddButton(1280 / 2.0 - 180, 920 / 2.0 - 10, "Налаштування", canvasMenu, () =>
            {
                engine.inSettings = true;
                uiManager.ShowSettings();
                soundManager.PlayClickSound(engine.GetSoundPath("klik.mp3"));
            });

            AddButton(1280 / 2.0 - 180, 920 / 2.0 + 55, "Вихiд", canvasMenu, () =>
            {
                soundManager.PlayClickSound(engine.GetSoundPath("klik.mp3"));
                Application.Current.Shutdown();

            });

        }
        public void SetUIManager(UIManager uiManager, UICreator uiCreator)
        {
            this.uiManager = uiManager;
            this.uiCreator = uiCreator;
        }
        public void AddImage(string imagePath, double x, double y, double width, double height)
        {
            Image image = new Image();

            BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
            image.Source = bitmapImage;
            image.Width = width;
            image.Height = height;

            Canvas.SetLeft(image, x - width / 2.0);
            Canvas.SetTop(image, y - height / 2.0);
            canvasMenu.Children.Add(image);
        }
        private void AddButton(double x, double y, string name, Canvas canvas, Action action)
        {
            TextBlock infoTextBlock = new TextBlock
            {
                Text = name,
                FontSize = 32,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                Width = 370
            };
            double initialCenterX = x + infoTextBlock.Width / 2;

            Canvas.SetLeft(infoTextBlock, x);
            Canvas.SetZIndex(infoTextBlock, 3);
            Canvas.SetTop(infoTextBlock, y);
            canvas.Children.Add(infoTextBlock);

            infoTextBlock.MouseEnter += (sender, e) =>
            {
                var textBlock = sender as TextBlock;
                if (textBlock != null)
                {
                    textBlock.Text = "> " + name + " <";
                    double centerX = initialCenterX - textBlock.ActualWidth / 2;
                    Canvas.SetLeft(textBlock, centerX);
                }
            };

            infoTextBlock.MouseLeave += (sender, e) =>
            {
                var textBlock = sender as TextBlock;
                if (textBlock != null)
                {
                    textBlock.Text = name;
                    Canvas.SetLeft(textBlock, x);
                }
            };

            infoTextBlock.MouseLeftButtonDown += (sender, e) => action?.Invoke();
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
                uiManager.ToggleBlurEffect(true);
                engine.StopTimer();
                uiManager.MoveImage("Coin", -2000, 20);
                uiManager.OpacityObject(uiCreator.levelTextBlock, 0);
                uiManager.OpacityImage("Level", 0);
                uiManager.OpacityObject(uiCreator.taskTextBlock, 0);
                uiCreator.taskBorder.Visibility = Visibility.Hidden;
                canvasMenu.Visibility = Visibility.Visible;
                animation.From = 0;
                animation.To = 1;
            }
            else
            {
                animation.From = 1;
                animation.To = 0;
                animation.Completed += (s, e) => canvasMenu.Visibility = Visibility.Hidden;
                
            }

            canvasMenu.BeginAnimation(Canvas.OpacityProperty, animation);
            isVisible = !isVisible;

        }
        
    }
}
