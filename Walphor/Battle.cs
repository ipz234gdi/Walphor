using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Linq;
//using static System.Net.Mime.MediaTypeNames;

namespace Walphor
{
    internal class Battle
    {
        
        private NPCBrain npcBrain = new NPCBrain();
        private Random rand = new Random();
        private GameEngine gameEngine;
        private SoundManager soundManager;
        public Canvas canvasBattle;
        private Grid gridGame;
        private Shop shop;
        private bool isVisible = false;
        private TextBlock timerDisplay;
        private TextBlock turnDisplay;
        private List<TextBlock> bonusStatBlocks = new List<TextBlock>();
        private Dictionary<string, Image> images = new Dictionary<string, Image>();
        private bool playerisDefending = false;
        private bool npcisDefending = false;
        double Running = 15;
        double Defing = 50;
        private bool inic = false;
        private Character player_battle;
        private Character player_battle_org;
        private NPC npc_battle;

        private DispatcherTimer turnTimer;
        private int timeLeft = 15;
        bool isPlayerTurn = true;

        private double Bonus_Atck = 0;
        private double Bonus_Def = 0;
        private double Bonus_Run = 0;

        private ButtonManager button1;
        private ButtonManager button2;
        private ButtonManager button3;

        private UIManager uiManager;
        public Battle(Grid gridGame, Character character, NPC npc, UIManager uIManager, GameEngine gameEngine, Shop shop, SoundManager soundManager)
        {
            this.gridGame = gridGame;
            this.player_battle_org = character;
            player_battle = character;
            this.npc_battle = npc;
            this.gameEngine = gameEngine;
            this.uiManager = uIManager;
            this.shop = shop;


            InitializeBatle();
            this.soundManager = soundManager;
        }
        private void SetupTurnTimer()
        {
            turnTimer = new DispatcherTimer();
            turnTimer.Interval = TimeSpan.FromSeconds(1);
            turnTimer.Tick += TurnTimer_Tick;
        }

        private void InitializeBatle()
        {
            canvasBattle = new Canvas
            {
                Name = "BattleCanvas",
                Width = 1280,
                Height = 720,
                Visibility = Visibility.Hidden
            };


            gridGame.Children.Add(canvasBattle);

            Panel.SetZIndex(canvasBattle, 1);

            AddImage("pack://application:,,,/Data/Battle_background.png", 0, 0, 1280, 720);

            button1 = ButtonManager.Create(20, 480, 204, 68, "pack://application:,,,/Data/Button/Attack.png", canvasBattle);
            button1.Tag = "Attack";
            button1.Click += Button1_Click;
            AddTextNextToButton("Attack", 20 + 204 + 10, 492, player_battle.Atck, "", canvasBattle);

            button2 = ButtonManager.Create(20, 560, 204, 68, "pack://application:,,,/Data/Button/Defens.png", canvasBattle);
            button2.Tag = "Defend";
            button2.Click += Button2_Click;
            AddTextNextToButton("Defend", 20 + 204 + 10, 572, Defing, "%", canvasBattle);

            button3 = ButtonManager.Create(20, 640, 204, 68, "pack://application:,,,/Data/Button/Run_away.png", canvasBattle);
            button3.Tag = "Run_away";
            button3.Click += Button3_Click;
            AddTextNextToButton("Run", 20 + 204 + 10, 652, Running, "%", canvasBattle);

            AddImage("pack://application:,,,/Data/Cards/Demo_card.png", 600, 488, 128, 212);
            AddImage("pack://application:,,,/Data/Cards/Demo_card.png", 740, 488, 128, 212);
            AddImage("pack://application:,,,/Data/Cards/Demo_card.png", 880, 488, 128, 212);

            timerDisplay = new TextBlock
            {
                FontSize = 24,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                Foreground = new SolidColorBrush(Colors.White),
                Text = "15",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top
            };
            Canvas.SetTop(timerDisplay, 10);
            Canvas.SetLeft(timerDisplay, (canvasBattle.Width / 2) - 16);
            canvasBattle.Children.Add(timerDisplay);

            turnDisplay = new TextBlock
            {
                FontSize = 24,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                Foreground = Brushes.White,
                Text = "<     ",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
            };
            Canvas.SetTop(turnDisplay, 10);
            Canvas.SetLeft(turnDisplay, (canvasBattle.Width / 2) - 44);
            canvasBattle.Children.Add(turnDisplay);

            SetupTurnTimer();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            if (!isPlayerTurn) return;
            soundManager.PlayClickSound(gameEngine.GetSoundPath("klik.mp3"));

            button1.SetImageSource("pack://application:,,,/Data/Button/Attack_1.png");
            Debug.WriteLine("tetetetettete");
            Task.Delay(200).ContinueWith(t =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    button1.SetImageSource("pack://application:,,,/Data/Button/Attack.png");
                });
            });

            if (npcisDefending && rand.NextDouble() < (Defing - (player_battle.Algo / 2) / 100))
            {
                Debug.WriteLine("Оборона нпс успiшна!");
                StartTransitionAnimation_Def("NPC");
                npc_battle.Def -= player_battle.Atck + Bonus_Atck + (player_battle.Algo / 2);
                if (npc_battle.Def < 0) npc_battle.Health -= player_battle.Atck + Bonus_Atck + (player_battle.Algo / 2);
            }
            else
            {
                npc_battle.Health -= player_battle.Atck + Bonus_Atck + (player_battle.Algo / 2);
                StartTransitionAnimation("NPC");
            }
            npcisDefending = false;
            ToggleTurn();
        }
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            if (!isPlayerTurn) return;

            button2.SetImageSource("pack://application:,,,/Data/Button/Defens_1.png");
            Task.Delay(200).ContinueWith(t =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    button2.SetImageSource("pack://application:,,,/Data/Button/Defens.png");
                });
            });

            soundManager.PlayClickSound(gameEngine.GetSoundPath("klik.mp3"));
            playerisDefending = true;
            ToggleTurn();
        }
        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            if (!isPlayerTurn) return;
            soundManager.PlayClickSound(gameEngine.GetSoundPath("klik.mp3"));

            button3.SetImageSource("pack://application:,,,/Data/Button/Run_away_1.png");
            Task.Delay(200).ContinueWith(t =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    button3.SetImageSource("pack://application:,,,/Data/Button/Run_away.png");
                });
            });

            if (rand.NextDouble() < ((Running + Bonus_Run) / 100))
            {

                EndBattle();
            }
            else
            {
                ToggleTurn();
            }

        }
        
        private string[] imagePaths = new string[]
        {
            "pack://application:,,,/Data/Battle/animation/F_1.png",
            "pack://application:,,,/Data/Battle/animation/F_2.png",
            "pack://application:,,,/Data/Battle/animation/F_3.png",
            "pack://application:,,,/Data/Battle/animation/F_4.png",
            "pack://application:,,,/Data/Battle/animation/F_5.png",
            "pack://application:,,,/Data/Battle/animation/F_6.png",
            "pack://application:,,,/Data/Battle/animation/F_7.png",
            "pack://application:,,,/Data/Battle/animation/F_8.png"
        };
        private string[] imagePaths_def = new string[]
        {
            "pack://application:,,,/Data/Battle/animation_Def/FD_1.png",
            "pack://application:,,,/Data/Battle/animation_Def/FD_2.png",
            "pack://application:,,,/Data/Battle/animation_Def/FD_3.png",
            "pack://application:,,,/Data/Battle/animation_Def/FD_4.png",
            "pack://application:,,,/Data/Battle/animation_Def/FD_5.png",
            "pack://application:,,,/Data/Battle/animation_Def/FD_6.png",
            "pack://application:,,,/Data/Battle/animation_Def/FD_7.png",
            "pack://application:,,,/Data/Battle/animation_Def/FD_8.png"
        };
        public void StartTransitionAnimation(string active)
        {
            if (!images.TryGetValue("animationImage", out Image animationImage))
            {
                animationImage = new Image
                {
                    Width = 200,
                    Height = 100,
                    Visibility = Visibility.Visible
                };
                if (active == "NPC")
                {
                    animationImage.RenderTransform = new ScaleTransform(-1, 1, animationImage.Width / 2, animationImage.Height / 2);
                }
                else if (active == "PLAYER")
                {
                    animationImage.RenderTransform = new ScaleTransform(0, 0, animationImage.Width / 2, animationImage.Height / 2);
                }

                canvasBattle.Children.Add(animationImage);
                Canvas.SetZIndex(animationImage, 10);
                if (active == "NPC")
                {
                    Canvas.SetLeft(animationImage, (canvasBattle.Width / 2.0) - 75);
                    Canvas.SetTop(animationImage, (canvasBattle.Height / 2.0) - 50);
                }
                else if (active == "PLAYER")
                {
                    Canvas.SetLeft(animationImage, (canvasBattle.Width / 2.0) - 125);
                    Canvas.SetTop(animationImage, (canvasBattle.Height / 2.0) - 50);
                }

                images["animationImage"] = animationImage;
            }
            else
            {
                if (active == "NPC")
                {
                    animationImage.RenderTransform = new ScaleTransform(-1, 1, animationImage.Width / 2, animationImage.Height / 2);
                }
                else if (active == "PLAYER")
                {
                    animationImage.RenderTransform = new ScaleTransform(1, 1, animationImage.Width / 2, animationImage.Height / 2);
                }
                if (active == "NPC")
                {
                    Canvas.SetLeft(animationImage, (canvasBattle.Width / 2.0) - 75);
                    Canvas.SetTop(animationImage, (canvasBattle.Height / 2.0) - 50);
                }
                else if (active == "PLAYER")
                {
                    Canvas.SetLeft(animationImage, (canvasBattle.Width / 2.0) - 125);
                    Canvas.SetTop(animationImage, (canvasBattle.Height / 2.0) - 50);
                }
                animationImage.Visibility = Visibility.Visible;
            }
            int currentFrame = 0;
            int curentPos = 30;
            var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(0.05) };
            timer.Start();
            timer.Tick += async (sender, args) =>
            {
                if (npc_battle != null)
                {
                    if (currentFrame < imagePaths.Length)
                    {
                        animationImage.Source = new BitmapImage(new Uri(imagePaths[currentFrame], UriKind.RelativeOrAbsolute));
                        currentFrame++;
                        if (active == "NPC")
                        {
                            if (npc_battle.X >= 665 && curentPos <= 50)
                            {
                                npc_battle.SetPosition((canvasBattle.Width / 2.0) + 25 + curentPos, (canvasBattle.Height / 2.0) - 50);
                                curentPos += 10;
                            }
                            else
                            {
                                npc_battle.SetPosition((canvasBattle.Width / 2.0) + 125 - curentPos, (canvasBattle.Height / 2.0) - 50);
                                curentPos += 10;
                            }
                        }
                        else if (active == "PLAYER")
                        {
                            if (player_battle.X <= 565 && curentPos <= 50)
                            {
                                player_battle.SetPosition((canvasBattle.Width / 2.0) - 75 - curentPos, (canvasBattle.Height / 2.0) - 50);
                                curentPos += 10;
                            }
                            else
                            {
                                player_battle.SetPosition((canvasBattle.Width / 2.0) - 175 + curentPos, (canvasBattle.Height / 2.0) - 50);
                                curentPos += 10;
                            }
                        }

                    }
                    else
                    {

                        timer.Stop();
                        animationImage.Visibility = Visibility.Hidden;
                        currentFrame = 0;
                        animationImage.Source = new BitmapImage(new Uri(imagePaths[currentFrame], UriKind.RelativeOrAbsolute));
                    }
                }
            };
        }
        public void StartTransitionAnimation_Def(string active)
        {
            if (!images.TryGetValue("animationImage", out Image animationImage))
            {
                animationImage = new Image
                {
                    Width = 200,
                    Height = 100,
                    Visibility = Visibility.Visible
                };
                if (active == "NPC")
                {
                    animationImage.RenderTransform = new ScaleTransform(-1, 1, animationImage.Width / 2, animationImage.Height / 2);
                }
                else if (active == "PLAYER")
                {
                    animationImage.RenderTransform = new ScaleTransform(0, 0, animationImage.Width / 2, animationImage.Height / 2);
                }

                canvasBattle.Children.Add(animationImage);
                Canvas.SetZIndex(animationImage, 10);
                if (active == "NPC")
                {
                    Canvas.SetLeft(animationImage, (canvasBattle.Width / 2.0) - 75);
                    Canvas.SetTop(animationImage, (canvasBattle.Height / 2.0) - 50);
                }
                else if (active == "PLAYER")
                {
                    Canvas.SetLeft(animationImage, (canvasBattle.Width / 2.0) - 125);
                    Canvas.SetTop(animationImage, (canvasBattle.Height / 2.0) - 50);
                }

                images["animationImage"] = animationImage;
            }
            else
            {
                if (active == "NPC")
                {
                    animationImage.RenderTransform = new ScaleTransform(-1, 1, animationImage.Width / 2, animationImage.Height / 2);
                }
                else if (active == "PLAYER")
                {
                    animationImage.RenderTransform = new ScaleTransform(1, 1, animationImage.Width / 2, animationImage.Height / 2);
                }
                if (active == "NPC")
                {
                    Canvas.SetLeft(animationImage, (canvasBattle.Width / 2.0) - 75);
                    Canvas.SetTop(animationImage, (canvasBattle.Height / 2.0) - 50);
                }
                else if (active == "PLAYER")
                {
                    Canvas.SetLeft(animationImage, (canvasBattle.Width / 2.0) - 125);
                    Canvas.SetTop(animationImage, (canvasBattle.Height / 2.0) - 50);
                }
                animationImage.Visibility = Visibility.Visible;
            }
            int currentFrame = 0;
            int curentPos = 30;
            var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(0.05) };
            timer.Start();
            timer.Tick += async (sender, args) =>
            {
                if (npc_battle != null)
                {
                    if (currentFrame < imagePaths.Length)
                    {
                        animationImage.Source = new BitmapImage(new Uri(imagePaths_def[currentFrame], UriKind.RelativeOrAbsolute));
                        currentFrame++;
                        if (active == "NPC")
                        {
                            if (npc_battle.X >= 665 && curentPos <= 50)
                            {
                                npc_battle.SetPosition((canvasBattle.Width / 2.0) + 25 + curentPos, (canvasBattle.Height / 2.0) - 50);
                                curentPos += 10;
                            }
                            else
                            {
                                npc_battle.SetPosition((canvasBattle.Width / 2.0) + 125 - curentPos, (canvasBattle.Height / 2.0) - 50);
                                curentPos += 10;
                            }
                        }
                        else if (active == "PLAYER")
                        {
                            if (player_battle.X <= 565 && curentPos <= 50)
                            {
                                player_battle.SetPosition((canvasBattle.Width / 2.0) - 75 - curentPos, (canvasBattle.Height / 2.0) - 50);
                                curentPos += 10;
                            }
                            else
                            {
                                player_battle.SetPosition((canvasBattle.Width / 2.0) - 175 + curentPos, (canvasBattle.Height / 2.0) - 50);
                                curentPos += 10;
                            }
                        }

                    }
                    else
                    {

                        timer.Stop();
                        animationImage.Visibility = Visibility.Hidden;
                        currentFrame = 0;
                        animationImage.Source = new BitmapImage(new Uri(imagePaths_def[currentFrame], UriKind.RelativeOrAbsolute));
                    }
                }
            };
        }
        private void TurnTimer_Tick(object sender, EventArgs e)
        {
            timeLeft--;
            if (timeLeft <= 0)
            {
                ToggleTurn();
                timeLeft = 15;
            }
            timerDisplay.Text = "" + timeLeft.ToString();
        }
        private void ToggleTurn()
        {
            isPlayerTurn = !isPlayerTurn;
            turnDisplay.Text = "" + (isPlayerTurn ? "<" : "           >");
            if (!isPlayerTurn)
                NPCAction();
            timeLeft = 15;
        }
        private async void NPCAction()
        {
            if (!isPlayerTurn && npc_battle != null)
            {

                Random rand = new Random();
                int delay = rand.Next(1000, 5001); // Випадкова затримка від 1 до 5 секунд
                await Task.Delay(delay);

                int action = npcBrain.ComputeAction(npc_battle.Level);
                switch (action)
                {
                    case 0:
                        if (playerisDefending && rand.NextDouble() < (Defing / 100))
                        {
                            player_battle.Def -= npc_battle.Atck;
                            Debug.WriteLine("Оборона гравця успiшна!");
                            StartTransitionAnimation_Def("PLAYER");
                            if (player_battle.Def < 0) player_battle.Health -= npc_battle.Atck;
                        }
                        else
                        {
                            Debug.WriteLine("НПС атакує!");
                            StartTransitionAnimation("PLAYER");
                            player_battle.Health -= npc_battle.Atck + (npc_battle.Level * 3);

                        }
                        playerisDefending = false;
                        break;
                    case 1:
                        npcisDefending = true;
                        Debug.WriteLine("оборона");
                        break;
                    case 2:

                        Debug.WriteLine("бонус");
                        npc_battle.Health += 20;
                        break;
                }
                ToggleTurn();
            }
        }
        private void UpdateHealthBar(double newHealth)
        {
            double maxWidth = 294;
            double healthPercentage = newHealth / player_battle.MaxHealth;

            if (newHealth <= 0)
            { 
                EndBattle();
                uiManager.ShowGameEndMenu("Ви програли!");
            }

            uiManager.UpdateRectangleWidth("HP", maxWidth * healthPercentage, maxWidth);
        }
        private void UpdateHealthBar_NPC(double newHealth)
        {
            double maxWidth = 294;
            double healthPercentage_NPC = newHealth / npc_battle.MaxHealth;

            if (newHealth <= 0)
            {
                player_battle_org.Level += 1;
                player_battle_org.Money += 10;
                uiManager.UpdateMoney(player_battle_org.Money);
                uiManager.UpdateLevel(player_battle_org.Level);
                EndBattle();
            }

            uiManager.UpdateRectangleWidth_NPC("HP_NPC", maxWidth * healthPercentage_NPC, maxWidth);
        }
        private void UpdateDefBar(double newDef)
        {
            double maxWidth = 208;
            double defPercentage = newDef / player_battle.MaxDef;

            uiManager.UpdateRectangleWidth("DEF", maxWidth * defPercentage, maxWidth);
        }
        private void UpdateDefBar_NPC(double newDef)
        {
            double maxWidth = 208;
            double defPercentage_NPC = newDef / npc_battle.MaxDef;

            uiManager.UpdateRectangleWidth_NPC("DEF_NPC", maxWidth * defPercentage_NPC, maxWidth);
        }
        private void UpdateAlgoBar(double newAlgo)
        {
            double maxHeight = 272;
            double algoPercentage = newAlgo / player_battle.MaxAlgo;
            uiManager.UpdateRectangleHeight("BIB", maxHeight * algoPercentage, maxHeight);
        }
        public void OnCardClicked(object sender, RoutedEventArgs e)
        {
            if (!isPlayerTurn) return;
            Image clickedImage = sender as Image;
            if (clickedImage != null)
            {
                Card card = clickedImage.Tag as Card;
                if (card != null && Character.Character_Cards.Contains(card))
                {
                    player_battle.Health += card.Health;
                    Bonus_Atck += card.Atck;

                    if (card.Def > 0 && card.Def < 1)
                    {
                        Bonus_Def += (card.Def * 100);
                    }
                    else player_battle.Def += card.Def;

                    Bonus_Run += card.Def;

                    player_battle.Algo += card.Algo;

                    Character.Character_Cards.Remove(card);

                    shop.occupiedPositions[card.Index] = false;
                    Card.ClearByName(card.Name, canvasBattle);
                }
                
            }
            DisplayBonusStats(472, 492, canvasBattle);

            ToggleTurn();
        }
        private void DisplayBonusStats(double x, double y, Canvas canvas)
        {
            foreach (TextBlock oldBlock in bonusStatBlocks)
            {
                if (canvas.Children.Contains(oldBlock))
                {
                    canvas.Children.Remove(oldBlock);
                }
            }
            bonusStatBlocks.Clear();

            string[] bonusTexts = new string[]
            {
                $"+{Bonus_Atck}",
                $"+{Bonus_Def}%",
                $"+{Bonus_Run}%"
            };

            for (int i = 0; i < bonusTexts.Length; i++)
            {
                TextBlock statBlock = new TextBlock
                {
                    Text = bonusTexts[i],
                    FontSize = 32,
                    FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                    Foreground = new SolidColorBrush(Colors.Black),
                    Background = Brushes.Transparent,
                    Width = 124,
                    Height = 88,
                    TextAlignment = TextAlignment.Left
                };

                Canvas.SetLeft(statBlock, x);
                Canvas.SetTop(statBlock, y + (i * 80));
                Canvas.SetZIndex(statBlock, 2);
                canvas.Children.Add(statBlock);
                bonusStatBlocks.Add(statBlock);
            }
        }
        private void DisplayPlayerCards()
        {
            double startX = 600;
            double startY = 488;
            double offset = 140;

            for (int i = 0; i < Character.Character_Cards.Count; i++)
            {
                Card card = Character.Character_Cards[i];
                int index = card.Index;
                string newName = Character.Character_Cards[i].Name;
                Card.MoveCardToCanvas(newName, canvasBattle, startX + index * offset, startY);
                
                card.image.MouseLeftButtonDown += OnCardClicked;
                Debug.WriteLine("Картка перемiщенна!" + index);
            }
        }
        private void ClearNPCs()
        {
            //if (player_battle != null)
            //{
            //    // Вiдписка вiд подiй
            //    player_battle.OnHealthChanged -= UpdateHealthBar;
            //    player_battle.OnDefChanged -= UpdateDefBar;


            //    // Видалення з Canvas
            //    if (canvasBattle.Children.Contains(player_battle.GetImage()))
            //        canvasBattle.Children.Remove(player_battle.GetImage());

            //    player_battle = null;
            //}
            if (npc_battle != null)
            {
                npc_battle.OnHealthChanged_NPC -= UpdateHealthBar_NPC;
                npc_battle.OnDefChanged_NPC -= UpdateDefBar_NPC;

                if (canvasBattle.Children.Contains(npc_battle.GetImage()))
                    canvasBattle.Children.Remove(npc_battle.GetImage());

                npc_battle = null;
            }
        }
        public void EndBattle()
        {
            player_battle.Health = 100;
            player_battle.Def = 100;
            player_battle.Algo = 0;
            
            ClearNPCs();
            Bonus_Atck = 0;
            Bonus_Def = 0;
            Bonus_Run = 0;

            DisplayBonusStats(472, 492, canvasBattle);

            for (int i = 0; i < Character.Character_Cards.Count; i++)
            {
                Card card = Character.Character_Cards[i];
                int index = card.Index;
                double newX = 20 + 136 * index;
                double newY = 468;
                string newName = Character.Character_Cards[i].Name;
                Card.MoveCardToCanvas(newName, shop.canvasShop, newX, newY);
                
                card.image.MouseLeftButtonDown += OnCardClicked;

            }
            
            turnTimer.Stop();
            gameEngine.ToggleCursorVisibility(false);
            ToggleTurn();
            playerisDefending = false;
            npcisDefending = false;
            gameEngine.ApplysScene();
        }
        private void AddTextNextToButton(string text, double x, double y, double num, string text2, Canvas canvas)
        {
            TextBlock playerStatsTextBlock = new TextBlock
            {
                Text = text + "  " + num.ToString() + text2,
                FontSize = 32,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                Foreground = new SolidColorBrush(Colors.Black),
                Background = Brushes.Transparent,
                Width = 230,
                Height = 68
            };

            Canvas.SetLeft(playerStatsTextBlock, x);
            Canvas.SetTop(playerStatsTextBlock, y);
            Canvas.SetZIndex(playerStatsTextBlock, 2);
            canvas.Children.Add(playerStatsTextBlock);
        }
        public void AddImage(string imagePath, double x, double y, double width, double height)
        {
            Image image = new Image
            {
                Width = width,
                Height = height,
                Source = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute))
            };

            BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
            image.Source = bitmapImage;

            Canvas.SetLeft(image, x);
            Canvas.SetTop(image, y);

            canvasBattle.Children.Add(image);
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
                canvasBattle.Visibility = Visibility.Visible;
                animation.From = 0;
                animation.To = 1;
            }
            else
            {
                animation.From = 1;
                animation.To = 0;
                animation.Completed += (s, e) => canvasBattle.Visibility = Visibility.Hidden;
            }

            canvasBattle.BeginAnimation(Canvas.OpacityProperty, animation);
            isVisible = !isVisible;

        }
        public void BattleScene(Character player, NPC npc)
        {
            if (inic == false)
            {
                this.player_battle = (Character)player.Clone(canvasBattle, player.X, player.Y);
                Debug.WriteLine("RRRRRRRRRRRRRRRRR222222222222");
                inic = true;
            }
            npc_battle = (NPC)npc.Clone(canvasBattle, npc.X, npc.Y);

            player_battle.SetPosition((canvasBattle.Width / 2.0) - 75, (canvasBattle.Height / 2.0) - 50);
            npc_battle.SetPosition((canvasBattle.Width / 2.0) + 25, (canvasBattle.Height / 2.0) - 50);

            npc_battle.Health = 99;
            npc_battle.Def = 99;

            player_battle.OnHealthChanged += UpdateHealthBar;
            player_battle.OnDefChanged += UpdateDefBar;
            player_battle.OnAlgoChanged += UpdateAlgoBar;
            npc_battle.OnHealthChanged_NPC += UpdateHealthBar_NPC;
            npc_battle.OnDefChanged_NPC += UpdateDefBar_NPC;

            
            timeLeft = 15;
            turnTimer.Start();

            player_battle.Health = 100;
            player_battle.Def = 100;



            npc_battle.Health = 100;
            npc_battle.Def = 100;
            npc_battle.Level = npc.Level;

            DisplayPlayerCards();
        }
    }
}
