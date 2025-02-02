using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

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

        private const double MENU_WIDTH = 1280;
        private const double MENU_HEIGHT = 720;
        private const double BUTTON_OFFSET_X = -180;
        private const double BUTTON_OFFSET_Y_START = -340;
        private const double BUTTON_GAP = 65;
        private const double BUTTON_CENTER_X = MENU_WIDTH / 2.0 + BUTTON_OFFSET_X;

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
                Width = MENU_WIDTH,
                Height = MENU_HEIGHT,
                Visibility = Visibility.Visible,
            };

            uiManager.ToggleBlurEffect(true);
            ToggleVisibility();
            gridGame.Children.Add(canvasMenu);

            var saveGame = new SaveGame();

            Panel.SetZIndex(canvasMenu, 100);
            canvasMenu.RenderTransform = new TranslateTransform();

            AddImage("pack://application:,,,/Data/Menu/Menu.png", MENU_WIDTH / 2.0, MENU_HEIGHT / 2.0, 384, 512);

            string[] buttonNames = { "Грати", "Збереження", "Профіль", "Правила гри", "Статистика", "Налаштування", "Вихiд" };
            Action[] buttonActions = { StartGame, SaveGame, OpenProfile, OpenRules, OpenStats, OpenSettings, ExitGame };

            for (int i = 0; i < buttonNames.Length; i++)
            {
                AddButton(BUTTON_CENTER_X, MENU_HEIGHT / 2.0 + BUTTON_OFFSET_Y_START + i * BUTTON_GAP, buttonNames[i], canvasMenu, buttonActions[i]);
            }
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

        private void StartGame()
        {
            ToggleVisibility();
            engine.StartTimer();
            uiCreator.taskBorder.Visibility = Visibility.Visible;
            uiManager.MoveImage("Coin", 100, 20);
            uiManager.ToggleBlurEffect(false);
            engine.inMenu = false;
            engine.SaveCurrentGame();
            engine.LoadGame(character.Name);
            soundManager.PlayClickSound(engine.GetSoundPath("klik.mp3"));
        }

        private void SaveGame()
        {
            engine.SaveCurrentGame();
            soundManager.PlayClickSound(engine.GetSoundPath("klik.mp3"));
        }

        private void OpenProfile()
        {
            uiManager.UpdateStatistics(GameEngine.GetDatabasePath());
            uiCreator.ProfileBorder.Visibility = Visibility.Visible;
            uiCreator.ProfileButton.Visibility = Visibility.Visible;
            ToggleVisibility();
            engine.inMenu = false;
            soundManager.PlayClickSound(engine.GetSoundPath("klik.mp3"));
        }

        private void OpenRules()
        {
            engine.inRules = true;
            uiManager.ShowRulesMenu();
            soundManager.PlayClickSound(engine.GetSoundPath("klik.mp3"));
        }

        private void OpenStats()
        {
            uiManager.UpdateStatistics(GameEngine.GetDatabasePath());
            uiManager.StatisticsVisible();
            ToggleVisibility();
            engine.inMenu = false;
            soundManager.PlayClickSound(engine.GetSoundPath("klik.mp3"));
        }

        private void OpenSettings()
        {
            engine.inSettings = true;
            uiManager.ShowSettings();
            soundManager.PlayClickSound(engine.GetSoundPath("klik.mp3"));
        }

        private void ExitGame()
        {
            soundManager.PlayClickSound(engine.GetSoundPath("klik.mp3"));
            Application.Current.Shutdown();
        }
    }
}
