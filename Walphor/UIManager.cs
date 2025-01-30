using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using Google.Protobuf.WellKnownTypes;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Data.SQLite;
using System.IO;
using System.Data.Common;
using System.Windows.Input;
using System.Windows.Media.Effects;

namespace Walphor
{
    internal class UIManager
    {
        internal UICreator uiCreator;
        private Grid gridGame;
        private Character character;
        private GameEngine engine;
        private Menu menu;
        private SoundManager soundManager;
        private string Dbpath;
        private Canvas canvasGame;

        public UIManager(Grid gridGame, Character character, int Level_Npc, GameEngine engine, Menu menu, string dbpath, SoundManager soundManager, Canvas GameCanvas ,UICreator uICreator)
        {
            this.gridGame = gridGame;
            this.menu = menu;
            this.character = character;
            this.engine = engine;
            this.Dbpath = dbpath;
            this.soundManager = soundManager;
            this.canvasGame = GameCanvas;

            this.uiCreator = uICreator;

            character.OnMoneyChanged += UpdateMoney;
            character.OnLevelChanged += UpdateLevel;

            uiCreator.canvasUI.Loaded += CanvasUI_Loaded;
        }

        private void CanvasUI_Loaded(object sender, RoutedEventArgs e)
        {
            // Встановлюємо розміри Canvas відповідно до розмірів Grid після завантаження
            uiCreator.canvasUI.Width = gridGame.ActualWidth;
            uiCreator.canvasUI.Height = gridGame.ActualHeight;

            uiCreator.InitializeProfile(this);
            uiCreator.InitializeSettings(this);
            uiCreator.InitializeStatistics(Dbpath, this);
            uiCreator.InitializeGameEndMenu(this);
            uiCreator.InitializeRulesMenu(this);
            this.gridGame.SizeChanged += GridGame_SizeChanged;

            uiCreator.volumeSlider.Value = soundManager.GetMasterVolume();
        }
        private void GridGame_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Оновлюємо розміри Canvas відповідно до розмірів Grid
            uiCreator.canvasUI.Width = gridGame.ActualWidth;
            uiCreator.canvasUI.Height = gridGame.ActualHeight;

            uiCreator.UpdateLayout();
        }
        public void ShowRulesMenu()
        {
            uiCreator.rulesMenuBorder.Visibility = Visibility.Visible;
            menu.ToggleVisibility();
            engine.inMenu = false;
        }
        internal void CloseRulesMenuButton_Click(object sender, RoutedEventArgs e)
        {
            soundManager.PlayClickSound(engine.GetSoundPath("klik.mp3"));
            uiCreator.rulesMenuBorder.Visibility = Visibility.Hidden;
            menu.ToggleVisibility();
            engine.inRules = false;
            engine.inMenu = true;
        }
        public void SetMenu(Menu menu, UICreator uiCreator)
        {
            this.menu = menu;
            this.uiCreator = uiCreator;
        }
        public void StatisticsVisible()
        {
            engine.StopTimer();
            uiCreator.statistics_ScrollViewer.Visibility = Visibility.Visible;
            uiCreator.exitButtonImage.Visibility = Visibility.Visible;
            uiCreator.statistics_BorderName.Visibility = Visibility.Visible;
            engine.inStatistic = true;
        }
        public void StatisticsHidden()
        {
            uiCreator.statistics_ScrollViewer.Visibility = Visibility.Hidden;
            engine.inStatistic = false;
            uiCreator.exitButtonImage.Visibility = Visibility.Collapsed;
            uiCreator.statistics_BorderName.Visibility = Visibility.Collapsed;
            menu.ToggleVisibility();
            engine.inMenu = true;
        }
        public void ShowGameEndMenu(string message)
        {
            uiCreator.gameEndTextBlock.Text = message;
            uiCreator.gameEndBorder.Visibility = Visibility.Visible;
            engine.EndGame();
            engine.StopTimer();
            UpdateStatistics(GameEngine.GetDatabasePath());
        }
        internal void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            soundManager.PlayClickSound(engine.GetSoundPath("klik.mp3"));
            uiCreator.gameEndBorder.Visibility = Visibility.Hidden;
            Application.Current.Shutdown();
        }
        internal void ReplaseButton_Click(object sender, RoutedEventArgs e)
        {
            soundManager.PlayClickSound(engine.GetSoundPath("klik.mp3"));
            uiCreator.gameEndBorder.Visibility = Visibility.Hidden;
            uiCreator.taskBorder.Visibility = Visibility.Visible;
            MoveImage("Coin", 100, 20);
            ToggleBlurEffect(false);

            OpacityObject(uiCreator.levelTextBlock, 1);
            OpacityImage("Level", 1);
            OpacityObject(uiCreator.taskTextBlock, 1);
            
            engine.LoadGame(character.Name);
        }
        public void UpdateStatistics(string dbpath)
        {
            uiCreator.statistics_StackPanel.Children.Clear();
            string dbPath = $"Data Source={dbpath};Version=3;";
            using (var conn = new SQLiteConnection(dbPath))
            {
                conn.Open();
                // Сортуємо дані за часом у порядку спадання
                string sql = "SELECT id, name, level, money, time FROM players ORDER BY time DESC";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Border statistics_Border = new Border
                            {
                                Background = new SolidColorBrush(Color.FromRgb(43, 45, 66)),
                                BorderThickness = new Thickness(2),
                                Margin = new Thickness(5),
                                Padding = new Thickness(10),
                                CornerRadius = new CornerRadius(10)
                            };

                            StackPanel rowStackPanel = new StackPanel
                            {
                                Orientation = Orientation.Horizontal
                            };

                            rowStackPanel.Children.Add(uiCreator.CreateTextBlock(reader["name"].ToString()));
                            rowStackPanel.Children.Add(uiCreator.CreateTextBlock(reader["level"].ToString()));
                            rowStackPanel.Children.Add(uiCreator.CreateTextBlock(reader["money"].ToString()));
                            rowStackPanel.Children.Add(uiCreator.CreateTextBlock(reader["time"].ToString()));

                            statistics_Border.Child = rowStackPanel;
                            uiCreator.statistics_StackPanel.Children.Add(statistics_Border);
                        }
                    }
                }
            }
        }
        internal void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            StatisticsHidden();
            soundManager.PlayClickSound(engine.GetSoundPath("klik.mp3"));
        }
        internal void CustomButtonText_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Image Exit_button)
            {
                Exit_button.Source = new BitmapImage(new Uri("pack://application:,,,/Data/Button/Exit_button_2.png"));
            }
        }
        internal void CustomButtonText_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Image Exit_button)
            {
                Exit_button.Source = new BitmapImage(new Uri("pack://application:,,,/Data/Button/Exit_button.png"));
            }
        }
        internal void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            soundManager.PlayClickSound(engine.GetSoundPath("klik.mp3"));
            character.Name = uiCreator.profile_TextBox.Text;
            string Password = uiCreator.profile_pas_TextBox.Password;

            if (string.IsNullOrEmpty(character.Name))
            {
                ProfileWarning("Введiть iм'я!");
                uiCreator.ProfileButton.Visibility = Visibility.Hidden;
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                ProfileWarning("Введiть пароль!");
                uiCreator.ProfileButton.Visibility = Visibility.Hidden;
                return;
            }

            string dbPath = $"Data Source={GameEngine.GetDatabasePath()};Version=3;";
            using (var conn = new SQLiteConnection(dbPath))
            {
                conn.Open();

                string sqlCheckNameAndPassword = "SELECT COUNT(*) FROM players WHERE name = @name AND password = @password";
                using (var cmdCheckNameAndPassword = new SQLiteCommand(sqlCheckNameAndPassword, conn))
                {
                    cmdCheckNameAndPassword.Parameters.AddWithValue("@name", character.Name);
                    cmdCheckNameAndPassword.Parameters.AddWithValue("@password", Password);
                    long countNameAndPassword = (long)cmdCheckNameAndPassword.ExecuteScalar();

                    if (countNameAndPassword == 0)
                    {
                        string sqlCheckName = "SELECT COUNT(*) FROM players WHERE name = @name";
                        using (var cmdCheckName = new SQLiteCommand(sqlCheckName, conn))
                        {
                            cmdCheckName.Parameters.AddWithValue("@name", character.Name);
                            long countName = (long)cmdCheckName.ExecuteScalar();

                            if (countName > 0)
                            {
                                ProfileWarning("пароль невірний!");
                                uiCreator.ProfileButton.Visibility = Visibility.Visible;
                                uiCreator.ProfileButton.Visibility = Visibility.Hidden;
                                return;
                            }
                        }
                    }
                }
            }
            uiCreator.taskTextBlock.Text = "Завдання: поговорити з Jery";
            engine.StartTimer();
            int gameTimeInSeconds = 1;
            engine.InsertOrUpdatePlayerData(character.Name, Password, gameTimeInSeconds);
            engine.inProfile = false;
            engine.inMenu = true;
            engine.LoadGame(character.Name);
            engine.Help();
            menu.ToggleVisibility();
            uiCreator.ProfileBorder.Visibility = Visibility.Hidden;
            uiCreator.ProfileButton.Visibility = Visibility.Hidden;
            
        }
        internal void warningButton_Click(object sender, RoutedEventArgs e)
        {
            soundManager.PlayClickSound(engine.GetSoundPath("klik.mp3"));
            uiCreator.profileWarningBorder.Visibility = Visibility.Collapsed;
            uiCreator.ProfileButton.Visibility = Visibility.Visible;
        }
        private void ProfileWarning(string war)
        {
            uiCreator.ProfileWarning(war, this);
        }
        internal void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            soundManager.SetMasterVolume((float)e.NewValue);
        }
        internal void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            soundManager.PlayClickSound(engine.GetSoundPath("klik.mp3"));
            uiCreator.settingsBorder.Visibility = Visibility.Hidden;
            menu.ToggleVisibility();
            engine.inSettings = false;
            engine.inMenu = true;
        }
        public void ShowSettings()
        {
            uiCreator.settingsBorder.Visibility = Visibility.Visible;
            menu.ToggleVisibility();
            engine.inMenu = false;
        }
        public void UpdateRectangleWidth(string name, double newWidth, double maxWidth)
        {
            if (newWidth < 0) newWidth = 0;
            if (uiCreator.rectangles.TryGetValue(name, out Rectangle rectangle))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    rectangle.Width = newWidth;
                });
            }
            else
            {
                Debug.WriteLine($"Rectangle {name} not found!");
            }
        }
        public void UpdateRectangleWidth_NPC(string name, double Width, double maxWidth)
        {
            if (Width < 0) Width = 0;
            if (uiCreator.rectangles.TryGetValue(name, out Rectangle rectangle))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    double currentLeft = Canvas.GetLeft(rectangle);
                    double shift = (rectangle.Width - Width);
                    rectangle.Width = Width;
                    Canvas.SetLeft(rectangle, Canvas.GetLeft(rectangle) + shift);
                });
            }
            else
            {
                Debug.WriteLine($"Rectangle {name} not found!");
            }
        }
        public void UpdateRectangleHeight(string name, double Height, double maxHeight)
        {
            if (Height < 0) Height = 0;
            if (uiCreator.rectangles.TryGetValue(name, out Rectangle rectangle))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    double currentTop = Canvas.GetTop(rectangle);
                    double shift = (rectangle.Height - Height);
                    rectangle.Height = Height;
                    Canvas.SetTop(rectangle, Canvas.GetTop(rectangle) + shift);
                });
            }
            else
            {
                Debug.WriteLine($"Rectangle {name} not found!");
            }
        }
        public void UpdateMoney(double money)
        {
            uiCreator.moneyTextBlock.Text = money.ToString();
        }
        public void UpdateLevel(int level)
        {
            uiCreator.levelTextBlock.Text = level.ToString();
            uiCreator.level_NPCTextBlock.Text = level.ToString();
        }
        public void UpdateHelp(string helptext, string help, bool visible)
        {
            uiCreator.help_TextBlock.Text = helptext + " " + help;
            if (visible) uiCreator.help_TextBlock.Visibility = Visibility.Visible;
            else uiCreator.help_TextBlock.Visibility = Visibility.Collapsed;
        }
        public void OpacityImage(string name, double opacity)
        {
            if (uiCreator.images.TryGetValue(name, out Image image))
            {
                image.Opacity = opacity;
            }
        }
        public void OpacityObject(UIElement element, double opacity)
        {
            element.Opacity = opacity;
        }
        public void OpacityRectangle(string name, double opacity)
        {
            if (uiCreator.rectangles.TryGetValue(name, out Rectangle rectangle))
            {
                rectangle.Opacity = opacity;
                Debug.WriteLine($"Setting opacity for {name} to {opacity}");
            }
            else
            {
                Debug.WriteLine($"Rectangle {name} not found!");
            }
        }
        public void MoveImage(string name, double newX, double newY)
        {
            if (uiCreator.images.TryGetValue(name, out Image image))
            {
                Canvas.SetLeft(image, newX);
                Canvas.SetTop(image, newY);
                MoveObjectOnCanvas(uiCreator.moneyTextBlock, newX + 60, newY + 4);
            }
        }
        private void MoveObjectOnCanvas(UIElement element, double newX, double newY)
        {
            Canvas.SetLeft(element, newX);
            Canvas.SetTop(element, newY);
        }
        private string[] imagePaths = new string[]
        {
            "pack://application:,,,/Data/Transition/1.png",
            "pack://application:,,,/Data/Transition/2.png",
            "pack://application:,,,/Data/Transition/3.png",
            "pack://application:,,,/Data/Transition/4.png",
            "pack://application:,,,/Data/Transition/5.png",
            "pack://application:,,,/Data/Transition/6.png"
        };
        public void StartTransitionAnimation()
        {
            if (!uiCreator.images.TryGetValue("animationImage", out Image animationImage))
            {
                animationImage = new Image
                {
                    Width = uiCreator.canvasUI.Width,
                    Visibility = Visibility.Visible
                };
                uiCreator.canvasUI.Children.Add(animationImage);
                Canvas.SetZIndex(animationImage, 10);
                Canvas.SetLeft(animationImage, 0);
                Canvas.SetTop(animationImage, 0);
                uiCreator.images["animationImage"] = animationImage;
            }
            else
            {
                animationImage.Visibility = Visibility.Visible;
            }

            int currentFrame = 0;
            bool goingUp = true;

            var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(0.05) };
            timer.Tick += async (sender, args) =>
            {
                animationImage.Source = new BitmapImage(new Uri(imagePaths[currentFrame], UriKind.RelativeOrAbsolute));

                if (goingUp)
                {
                    if (currentFrame < imagePaths.Length - 1)
                        currentFrame++;
                    else
                    {
                        goingUp = false;
                        timer.Stop();
                        await Task.Delay(500);
                        timer.Start();
                    }
                }
                else
                {
                    if (currentFrame > 0)
                        currentFrame--;
                    else
                    {
                        timer.Stop();
                        animationImage.Visibility = Visibility.Hidden;
                    }
                }
            };

            timer.Start();
        }
        public void ToggleBlurEffect(bool enable)
        {
            if (enable)
            {
                BlurEffect blurEffect = new BlurEffect
                {
                    Radius = 25
                };
                canvasGame.Effect = blurEffect;
            }
            else
            {
                canvasGame.Effect = null;
            }
        }
    }
}