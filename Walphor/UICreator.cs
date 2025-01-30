using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Data.SQLite;

namespace Walphor
{
    internal class UICreator
    {
        public Canvas canvasUI;
        public TextBlock levelTextBlock;
        public TextBlock moneyTextBlock;
        public TextBlock level_NPCTextBlock;
        public TextBlock help_TextBlock;
        public TextBox profile_TextBox;
        public PasswordBox profile_pas_TextBox;
        public TextBlock profile_TextBlock;
        public TextBlock profile_pass_TextBlock;
        public Border ProfileBorder;
        public Button ProfileButton;
        public TextBlock warning_TextBlock;
        public Border profileWarningBorder;
        public Button warningButton;
        public Image exitButtonImage;

        public ScrollViewer statistics_ScrollViewer;
        public StackPanel statistics_StackPanel;

        public TextBlock statistics_TextBlock;
        public TextBlock statistics_TextNameBlock;
        public Border statistics_BorderName;
        public Border statistics_Border;

        public Border rulesMenuBorder;
        public TextBlock rulesTextBlock;
        public Button closeRulesMenuButton;

        public TextBlock taskTextBlock;
        public Border taskBorder;

        public Border gameEndBorder;
        public TextBlock gameEndTextBlock;
        public Button mainMenuButton;
        public Button replaseButton;
        
        public Border settingsBorder;
        public Slider volumeSlider;
        public TextBlock volumeTextBlock;
        public TextBlock gameRulesTextBlock;
        public Button closeButton;
        public Dictionary<string, Image> images = new Dictionary<string, Image>();
        public Dictionary<string, Rectangle> rectangles = new Dictionary<string, Rectangle>();

        public UICreator()
        {
        }

        public void InitializeUI(Grid gridGame, double money, int level, int level_NPC)
        {
            canvasUI = new Canvas
            {
                Name = "UICanvas",
                Visibility = Visibility.Visible,
            };

            gridGame.Children.Add(canvasUI);
            Panel.SetZIndex(canvasUI, 9);

            moneyTextBlock = new TextBlock
            {
                Text = money.ToString(),
                Width = 200,
                Height = 40,
                FontSize = 32,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                RenderTransform = new TranslateTransform(0, 8),
                Background = Brushes.Transparent,
            };
            Canvas.SetLeft(moneyTextBlock, 430);
            Canvas.SetTop(moneyTextBlock, 24);

            levelTextBlock = new TextBlock
            {
                Text = level.ToString(),
                Width = 40,
                Height = 40,
                FontSize = 30,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                RenderTransform = new TranslateTransform(0, 8),
                Background = Brushes.Transparent,
            };
            Canvas.SetLeft(levelTextBlock, 36);
            Canvas.SetTop(levelTextBlock, 24);
            Canvas.SetZIndex(levelTextBlock, 10);

            level_NPCTextBlock = new TextBlock
            {
                Text = level_NPC.ToString(),
                Width = 40,
                Height = 40,
                FontSize = 30,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                RenderTransform = new TranslateTransform(0, 8),
                Background = Brushes.Transparent,
            };
            Canvas.SetLeft(level_NPCTextBlock, 1224);
            Canvas.SetTop(level_NPCTextBlock, 24);
            Canvas.SetZIndex(level_NPCTextBlock, 10);
            canvasUI.Children.Add(moneyTextBlock);

            help_TextBlock = new TextBlock
            {
                Text = "",
                Width = 600,
                Height = 80,
                FontSize = 20,
                Foreground = new SolidColorBrush(Colors.Black),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                RenderTransform = new TranslateTransform(0, 4),
                Background = Brushes.Transparent,
            };
            Canvas.SetLeft(help_TextBlock, 20);
            Canvas.SetTop(help_TextBlock, 400);
            Canvas.SetZIndex(help_TextBlock, 10);
            canvasUI.Children.Add(help_TextBlock);

            AddImage("pack://application:,,,/Data/UI/Level.png", 4, 4, 84, 84, "Level");
            canvasUI.Children.Add(levelTextBlock);

            AddImage("pack://application:,,,/Data/UI/HP.png", 64, 20, 296, 52, "HP");
            AddRectangle(64, 24, 292, 44, "HP", Colors.Red);

            AddImage("pack://application:,,,/Data/UI/bibbing.png", 20, 64, 52, 276, "bibbing");
            AddRectangle(24, 336, 44, 0, "BIB", Colors.Yellow);

            AddImage("pack://application:,,,/Data/UI/Defense.png", 68, 72, 216, 28, "Defense");
            AddRectangle(72, 72, 208, 24, "DEF", Colors.Blue);

            AddImage("pack://application:,,,/Data/UI/Level_NPC.png", 1192, 4, 84, 96, "Level_NPC");
            canvasUI.Children.Add(level_NPCTextBlock);

            AddImage("pack://application:,,,/Data/UI/HP_NPC.png", 920, 20, 296, 52, "HP_NPC");
            AddRectangle(922, 24, 292, 44, "HP_NPC", Colors.Red);

            AddImage("pack://application:,,,/Data/UI/Defense_NPC.png", 1000, 72, 216, 28, "Defense_NPC");
            AddRectangle(1004, 72, 208, 24, "DEF_NPC", Colors.Blue);

            InitializeTask();

            AddImage("pack://application:,,,/Data/UI/Coin.png", 100, 20, 44, 52, "Coin");
        }

        public void AddImage(string imagePath, double x, double y, double width, double height, string name)
        {
            Image image = new Image
            {
                Source = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute)),
                Width = width,
                Height = height
            };

            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);

            Canvas.SetLeft(image, x);
            Canvas.SetTop(image, y);
            Canvas.SetZIndex(image, 10);
            images[name] = image;

            canvasUI.Children.Add(image);
        }

        public void AddRectangle(double x, double y, double width, double height, string name, Color color)
        {
            Rectangle rectangle = new Rectangle
            {
                Fill = new SolidColorBrush(color),
                Width = width,
                Height = height,
                Opacity = 1
            };
            Canvas.SetZIndex(rectangle, 9);
            Canvas.SetLeft(rectangle, x);
            Canvas.SetTop(rectangle, y);
            canvasUI.Children.Add(rectangle);
            rectangles[name] = rectangle;
        }

        public void InitializeProfile(UIManager uiManager)
        {
            profile_TextBox = new TextBox
            {
                Text = "",
                Width = 300,
                Height = 30,
                FontSize = 20,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                Padding = new Thickness(0, 4, 0, 0),
                RenderTransform = new TranslateTransform(0, 4),
                Background = Brushes.Transparent,
            };

            profile_pas_TextBox = new PasswordBox
            {
                Width = 300,
                Height = 30,
                FontSize = 20,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                Padding = new Thickness(0, 4, 0, 0),
                RenderTransform = new TranslateTransform(0, 4),
                Background = Brushes.Transparent,

            };

            profile_TextBlock = new TextBlock
            {
                Text = "Введiть iм'я:",
                Width = 300,
                Height = 30,
                FontSize = 20,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                RenderTransform = new TranslateTransform(0, 4),
                Background = Brushes.Transparent,
                TextAlignment = TextAlignment.Center
            };

            profile_pass_TextBlock = new TextBlock
            {
                Text = "Введiть пароль:",
                Width = 300,
                Height = 30,
                FontSize = 20,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                RenderTransform = new TranslateTransform(0, 4),
                Background = Brushes.Transparent,
                Margin = new Thickness(0, 10, 0, 0),
                TextAlignment = TextAlignment.Center
            };

            ProfileButton = new Button
            {
                Content = "Почали!",
                Width = 300,
                Height = 30,
                FontSize = 20,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                RenderTransform = new TranslateTransform(0, 4),
                Background = Brushes.Transparent,
                Visibility = Visibility.Visible,
                Margin = new Thickness(0, 10, 0, 0),
            };
            ProfileButton.Click += uiManager.ProfileButton_Click;

            StackPanel panel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 60)
            };
            panel.Children.Add(profile_TextBlock);
            panel.Children.Add(profile_TextBox);
            panel.Children.Add(profile_pass_TextBlock);
            panel.Children.Add(profile_pas_TextBox);

            Color orangeColor = Color.FromRgb(43, 45, 66);
            SolidColorBrush orangeBrush = new SolidColorBrush(orangeColor);
            ProfileBorder = new Border
            {
                Background = orangeBrush,
                Width = 350,
                Height = 220,
                Child = panel,
                VerticalAlignment = VerticalAlignment.Top,
                Visibility = Visibility.Visible,
                CornerRadius = new CornerRadius(10)
            };

            Canvas.SetLeft(ProfileBorder, ((canvasUI.Width / 2.0) - (ProfileBorder.Width / 2)));
            Canvas.SetTop(ProfileBorder, ((canvasUI.Height / 2.0) - (ProfileBorder.Height / 2)));
            Canvas.SetZIndex(ProfileBorder, 6);
            canvasUI.Children.Add(ProfileBorder);

            Canvas.SetLeft(ProfileButton, ((canvasUI.Width / 2.0) - (ProfileButton.Width / 2)));
            Canvas.SetTop(ProfileButton, ((canvasUI.Height / 2.0) + 60 - (ProfileButton.Height / 2)));
            Canvas.SetZIndex(ProfileButton, 10);
            canvasUI.Children.Add(ProfileButton);
        }

        public void InitializeSettings(UIManager uiManager)
        {
            Color orangeColor = Color.FromRgb(43, 45, 66);
            SolidColorBrush orangeBrush = new SolidColorBrush(orangeColor);

            StackPanel settingsPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 60)
            };

            volumeTextBlock = new TextBlock
            {
                Text = "<Гучнiсть>",
                Width = 300,
                Height = 30,
                FontSize = 20,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                RenderTransform = new TranslateTransform(0, 4),
                Background = Brushes.Transparent,
                TextAlignment = TextAlignment.Center
            };

            volumeSlider = new Slider
            {
                Width = 300,
                Height = 30,
                Minimum = 0,
                Maximum = 1,
                Value = 1,
                Margin = new Thickness(0, 10, 0, 10)
            };
            volumeSlider.ValueChanged += uiManager.VolumeSlider_ValueChanged;

            gameRulesTextBlock = new TextBlock
            {
                Text = "Правила гри:\n\n1. <E> взаємодiя \n\n2. <WASD> перемiщення \n\n3. <SPASE> наступний дiалог ",
                Width = 300,
                Height = 260,
                FontSize = 20,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                RenderTransform = new TranslateTransform(0, 4),
                Background = Brushes.Transparent,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(0, 10, 0, 10)
            };

            closeButton = new Button
            {
                Content = "Закрити",
                Width = 300,
                Height = 30,
                FontSize = 20,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                RenderTransform = new TranslateTransform(0, 4),
                Background = Brushes.Transparent,
                Margin = new Thickness(0, 10, 0, 10)
            };
            closeButton.Click += uiManager.CloseButton_Click;

            settingsPanel.Children.Add(volumeTextBlock);
            settingsPanel.Children.Add(volumeSlider);
            settingsPanel.Children.Add(gameRulesTextBlock);
            settingsPanel.Children.Add(closeButton);

            settingsBorder = new Border
            {
                Background = orangeBrush,
                Width = 350,
                Height = 490,
                Child = settingsPanel,
                VerticalAlignment = VerticalAlignment.Top,
                Visibility = Visibility.Hidden,
                CornerRadius = new CornerRadius(10)
            };

            Canvas.SetLeft(settingsBorder, ((canvasUI.Width / 2.0) - (settingsBorder.Width / 2)));
            Canvas.SetTop(settingsBorder, ((canvasUI.Height / 2.0) - (settingsBorder.Height / 2)));
            Canvas.SetZIndex(settingsBorder, 8);
            canvasUI.Children.Add(settingsBorder);
        }

        public void InitializeStatistics(string dbpath, UIManager uiManager)
        {
            statistics_ScrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Width = 600,
                Height = 300,
                Visibility = Visibility.Hidden,
            };

            statistics_StackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            Color orangeColor = Color.FromRgb(43, 45, 66);
            SolidColorBrush orangeBrush = new SolidColorBrush(orangeColor);

            string dbPath = $"Data Source={dbpath};Version=3;";
            using (var conn = new SQLiteConnection(dbPath))
            {
                conn.Open();
                string sql = "SELECT id, name, level, money, time FROM players";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            statistics_Border = new Border
                            {
                                Background = orangeBrush,
                                BorderThickness = new Thickness(2),
                                Margin = new Thickness(5),
                                Padding = new Thickness(10),
                                CornerRadius = new CornerRadius(10)
                            };

                            StackPanel rowStackPanel = new StackPanel
                            {
                                Orientation = Orientation.Horizontal
                            };

                            rowStackPanel.Children.Add(CreateTextBlock(reader["name"].ToString()));
                            rowStackPanel.Children.Add(CreateTextBlock(reader["level"].ToString()));
                            rowStackPanel.Children.Add(CreateTextBlock(reader["money"].ToString()));
                            rowStackPanel.Children.Add(CreateTextBlock(reader["time"].ToString()));

                            statistics_Border.Child = rowStackPanel;
                            statistics_StackPanel.Children.Add(statistics_Border);
                        }
                    }
                }
            }

            statistics_BorderName = new Border
            {
                Width = 510,
                Height = 80,
                Background = orangeBrush,
                BorderThickness = new Thickness(2),
                Margin = new Thickness(5),
                Padding = new Thickness(10),
                CornerRadius = new CornerRadius(10),
                Visibility = Visibility.Collapsed,
            };

            statistics_TextNameBlock = new TextBlock
            {
                Text = "Name          Level       Money       Time",
                Margin = new Thickness(10),
                FontSize = 20,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                RenderTransform = new TranslateTransform(0, 4),
                Background = Brushes.Transparent,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            statistics_BorderName.Child = statistics_TextNameBlock;

            exitButtonImage = new Image
            {
                Width = 70,
                Height = 70,
                Source = new BitmapImage(new Uri("pack://application:,,,/Data/Button/Exit_button.png")),
                Visibility = Visibility.Collapsed,
            };

            exitButtonImage.MouseEnter += uiManager.CustomButtonText_MouseEnter;
            exitButtonImage.MouseLeave += uiManager.CustomButtonText_MouseLeave;
            exitButtonImage.MouseLeftButtonDown += uiManager.Exit_Button_Click;

            statistics_ScrollViewer.Content = statistics_StackPanel;

            Canvas.SetLeft(statistics_ScrollViewer, ((canvasUI.Width / 2.0) - (statistics_ScrollViewer.Width / 2)));
            Canvas.SetTop(statistics_ScrollViewer, ((canvasUI.Height / 2.0) - (statistics_ScrollViewer.Height / 2)));
            Canvas.SetZIndex(statistics_ScrollViewer, 5);
            canvasUI.Children.Add(statistics_ScrollViewer);

            Canvas.SetLeft(statistics_BorderName, ((canvasUI.Width / 2.0) - 10 - (statistics_BorderName.Width / 2)));
            Canvas.SetTop(statistics_BorderName, (((canvasUI.Height / 2.0) - 200) - (statistics_BorderName.Height / 2)));
            Canvas.SetZIndex(statistics_BorderName, 10);
            canvasUI.Children.Add(statistics_BorderName);

            Canvas.SetLeft(exitButtonImage, ((canvasUI.Width / 2.0) + 300 - (exitButtonImage.Width / 2)));
            Canvas.SetTop(exitButtonImage, ((canvasUI.Height / 2.0) - 195 - (exitButtonImage.Height / 2)));
            Canvas.SetZIndex(exitButtonImage, 10);
            canvasUI.Children.Add(exitButtonImage);
        }

        public void InitializeGameEndMenu(UIManager uiManager)
        {
            Color orangeColor = Color.FromRgb(43, 45, 66);
            SolidColorBrush orangeBrush = new SolidColorBrush(orangeColor);

            gameEndTextBlock = new TextBlock
            {
                Text = "",
                Width = 300,
                Height = 30,
                FontSize = 20,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                RenderTransform = new TranslateTransform(0, 4),
                Background = Brushes.Transparent,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 10)
            };

            mainMenuButton = new Button
            {
                Content = "Вийти",
                Width = 300,
                Height = 30,
                FontSize = 20,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                RenderTransform = new TranslateTransform(0, 4),
                Background = Brushes.Transparent,
                Margin = new Thickness(0, 10, 0, 10)
            };
            mainMenuButton.Click += uiManager.MainMenuButton_Click;

            replaseButton = new Button
            {
                Content = "Загрузитись",
                Width = 300,
                Height = 30,
                FontSize = 20,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                RenderTransform = new TranslateTransform(0, 4),
                Background = Brushes.Transparent,
                Margin = new Thickness(0, 10, 0, 10)
            };
            replaseButton.Click += uiManager.ReplaseButton_Click;

            StackPanel gameEndPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 60)
            };
            gameEndPanel.Children.Add(gameEndTextBlock);
            gameEndPanel.Children.Add(mainMenuButton);
            gameEndPanel.Children.Add(replaseButton);

            gameEndBorder = new Border
            {
                Background = orangeBrush,
                Width = 350,
                Height = 220,
                Child = gameEndPanel,
                VerticalAlignment = VerticalAlignment.Top,
                Visibility = Visibility.Hidden,
                CornerRadius = new CornerRadius(10)
            };

            Canvas.SetLeft(gameEndBorder, ((canvasUI.Width / 2.0) - (gameEndBorder.Width / 2)));
            Canvas.SetTop(gameEndBorder, ((canvasUI.Height / 2.0) - (gameEndBorder.Height / 2)));
            Canvas.SetZIndex(gameEndBorder, 8);
            canvasUI.Children.Add(gameEndBorder);
        }

        public void InitializeRulesMenu(UIManager uiManager)
        {
            Color menuColor = Color.FromRgb(43, 45, 66);
            SolidColorBrush menuBrush = new SolidColorBrush(menuColor);

            StackPanel rulesPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 60)
            };

            rulesTextBlock = new TextBlock
            {
                Text = "Історія гри\r\nВи опинилися на острові після кораблетрощі. Ваше завдання - вибратись з нього, отримавши човен. Глава острова надасть вам човен, але спершу ви маєте довести, що достойні цього. Перемагайте, заробляйте коіни, спілкуйтесь з НПС та купуйте необхідні ресурси, щоб досягти мети та врятуватись з острова.\r\n\r\n" +
                "1. Заробляння коінів:\r\nЛовіть рибу біля води, щоб заробити коіни. Це ваш основний ресурс для подальшого прогресу.\r\n\r\n" +
                "2. Бої:\r\nДля битв підходьте до НПС, рівень якого збігається з вашим. Це можна зрозуміти по завданню. Будьте обережні та готові до бою!\r\n\r\n" +
                "3. Розмови з НПС:\r\nПідходьте до НПС, щоб почати розмову. Це може допомогти вам дізнатись більше про гру та отримати важливі підказки.\r\n\r\n" +
                "4. Купівля алкоголю:\r\nЗнайдіть спеціального НПС, який стоїть на півночі, в центрі села, щоб придбати алкоголь.\r\n\r\n" +
                "5. Втеча з острова:\r\nЩоб втекти з острова, потрібно перемогти кожного НПС. Тільки тоді ви отримаєте доступ до човна.\r\n\r\n" +
                "6. Програш:\r\nЯкщо ви програєте бій і повністю втратите здоров'я, вам доведеться почати заново.\r\n\r\n" +
                "7. Відновлення після бою:\r\nПісля кожного бою здоров'я та броня гравця та НПС повністю відновлюються.",

                Width = 1000,
                Height = 580,
                FontSize = 16,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                RenderTransform = new TranslateTransform(0, 4),
                Background = Brushes.Transparent,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 10, 0, 10),
                LineHeight = 20
            };

            closeRulesMenuButton = new Button
            {
                Content = "Закрити",
                Width = 300,
                Height = 30,
                FontSize = 20,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                RenderTransform = new TranslateTransform(0, 4),
                Background = Brushes.Transparent,
                Margin = new Thickness(0, 10, 0, 10)
            };
            closeRulesMenuButton.Click += uiManager.CloseRulesMenuButton_Click;

            rulesPanel.Children.Add(rulesTextBlock);
            rulesPanel.Children.Add(closeRulesMenuButton);

            rulesMenuBorder = new Border
            {
                Background = menuBrush,
                Width = 1100,
                Height = 700,
                Child = rulesPanel,
                VerticalAlignment = VerticalAlignment.Top,
                Visibility = Visibility.Hidden,
                CornerRadius = new CornerRadius(10)
            };

            Canvas.SetLeft(rulesMenuBorder, ((canvasUI.Width / 2.0) - (rulesMenuBorder.Width / 2)));
            Canvas.SetTop(rulesMenuBorder, ((canvasUI.Height / 2.0) - (rulesMenuBorder.Height / 2)));
            Canvas.SetZIndex(rulesMenuBorder, 8);
            canvasUI.Children.Add(rulesMenuBorder);
        }

        public void InitializeTask()
        {
            Color taskColor = Color.FromRgb(43, 45, 66);
            SolidColorBrush taskBrush = new SolidColorBrush(taskColor);

            taskBorder = new Border
            {
                Background = taskBrush,
                BorderThickness = new Thickness(2),
                Margin = new Thickness(5),
                Padding = new Thickness(10),
                CornerRadius = new CornerRadius(10),
                Height = 50,
            };

            taskTextBlock = new TextBlock
            {
                Text = "",
                FontSize = 16,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                RenderTransform = new TranslateTransform(0, 0),
                Background = Brushes.Transparent,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            taskBorder.Child = taskTextBlock;

            Canvas.SetLeft(taskBorder, 20);
            Canvas.SetTop(taskBorder, 100);
            Canvas.SetZIndex(taskBorder, 10);
            canvasUI.Children.Add(taskBorder);
        }

        public void ProfileWarning(string war, UIManager uiManager)
        {
            warning_TextBlock = new TextBlock
            {
                Text = war,
                Width = 300,
                Height = 30,
                FontSize = 20,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                RenderTransform = new TranslateTransform(0, 4),
                Background = Brushes.Transparent,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 10, 0, 0),
                TextAlignment = TextAlignment.Center
            };
            warningButton = new Button
            {
                Content = "ОК",
                Width = 300,
                Height = 30,
                FontSize = 20,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                RenderTransform = new TranslateTransform(0, 4),
                Background = Brushes.Transparent,
                Margin = new Thickness(0, 10, 0, 0),
            };
            warningButton.Click += uiManager.warningButton_Click;

            StackPanel panel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 20, 0, 20)
            };
            panel.Children.Add(warning_TextBlock);
            panel.Children.Add(warningButton);

            Color orangeColor = Color.FromRgb(43, 45, 66);
            SolidColorBrush orangeBrush = new SolidColorBrush(orangeColor);
            profileWarningBorder = new Border
            {
                Background = orangeBrush,
                Width = 300,
                Height = 220,
                Child = panel,
                Visibility = Visibility.Visible,
                CornerRadius = new CornerRadius(10)
            };

            Canvas.SetLeft(profileWarningBorder, (640 - (profileWarningBorder.Width / 2)));
            Canvas.SetTop(profileWarningBorder, (360 - (profileWarningBorder.Height / 2)));
            Canvas.SetZIndex(profileWarningBorder, 7);
            canvasUI.Children.Add(profileWarningBorder);
        }

        internal TextBlock CreateTextBlock(string text)
        {
            return statistics_TextBlock = new TextBlock
            {
                Text = text,
                Width = 100,
                Height = 30,
                Margin = new Thickness(10),
                FontSize = 20,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                RenderTransform = new TranslateTransform(0, 4),
                Background = Brushes.Transparent,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        public void UpdateLayout()
        {
            Canvas.SetLeft(ProfileButton, ((canvasUI.Width / 2.0) - (ProfileButton.Width / 2)));
            Canvas.SetTop(ProfileButton, ((canvasUI.Height / 2.0) + 60 - (ProfileButton.Height / 2)));
            Canvas.SetLeft(ProfileBorder, ((canvasUI.Width / 2.0) - (ProfileBorder.Width / 2)));
            Canvas.SetTop(ProfileBorder, ((canvasUI.Height / 2.0) - (ProfileBorder.Height / 2)));
            Canvas.SetLeft(settingsBorder, ((canvasUI.Width / 2.0) - (settingsBorder.Width / 2)));
            Canvas.SetTop(settingsBorder, ((canvasUI.Height / 2.0) - (settingsBorder.Height / 2)));
            Canvas.SetLeft(statistics_ScrollViewer, ((canvasUI.Width / 2.0) - (statistics_ScrollViewer.Width / 2)));
            Canvas.SetTop(statistics_ScrollViewer, ((canvasUI.Height / 2.0) - (statistics_ScrollViewer.Height / 2)));
            Canvas.SetLeft(statistics_BorderName, ((canvasUI.Width / 2.0) - 10 - (statistics_BorderName.Width / 2)));
            Canvas.SetTop(statistics_BorderName, (((canvasUI.Height / 2.0) - 200) - (statistics_BorderName.Height / 2)));
            Canvas.SetLeft(exitButtonImage, ((canvasUI.Width / 2.0) + 300 - (exitButtonImage.Width / 2)));
            Canvas.SetTop(exitButtonImage, ((canvasUI.Height / 2.0) - 195 - (exitButtonImage.Height / 2)));

            if (images.TryGetValue("animationImage", out Image animationImage))
            {
                animationImage.Width = canvasUI.Width;
            }

            Canvas.SetLeft(gameEndBorder, ((canvasUI.Width / 2.0) - (gameEndBorder.Width / 2)));
            Canvas.SetTop(gameEndBorder, ((canvasUI.Height / 2.0) - (gameEndBorder.Height / 2)));
            Canvas.SetLeft(rulesMenuBorder, ((canvasUI.Width / 2.0) - (rulesMenuBorder.Width / 2)));
            Canvas.SetTop(rulesMenuBorder, ((canvasUI.Height / 2.0) - (rulesMenuBorder.Height / 2)));
        }
    }
}

