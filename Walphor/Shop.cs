﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Windows.Input;
using System.Reflection;

namespace Walphor
{
    internal class Shop
    {
        public Canvas canvasShop;
        private Grid gridGame;
        private UIManager uIManager;
        private GameEngine engine;
        private bool isVisible = false;
        private Border promptBorder;
        private TextBlock promptText;
        private TextBlock yesOption;
        private TextBlock noOption;
        private Image exitButtonImage;
        private Card cardToReplace = null;
        public static bool isSelectingCards = false;
        public bool[] occupiedPositions = new bool[3];

        public Character player_Shop;
        public Shop(Grid gridGame, Character character, UIManager uIManager, GameEngine gameEngine)
        {
            this.gridGame = gridGame;
            this.player_Shop = character;
            this.uIManager = uIManager;
            engine = gameEngine;
            InitializeShop();
        }
        private void InitializeShop()
        {
            canvasShop = new Canvas
            {
                Name = "ShopCanvas",
                Width = 1280,
                Height = 720,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Visibility = Visibility.Hidden
            };
            exitButtonImage = new Image
            {
                Width = 50,
                Height = 50,
                Source = new BitmapImage(new Uri("pack://application:,,,/Data/Button/Exit_button.png")),
                Visibility = Visibility.Collapsed,
            };



            gridGame.Children.Add(canvasShop);
            Panel.SetZIndex(canvasShop, 1);

            AddImage("pack://application:,,,/Data/Shop/Avatar.png", 100, 40, 256, 256);

            AddImage("pack://application:,,,/Data/Shop/Shop_Background.png", 440, 40, 800, 640);

            AddImage("pack://application:,,,/Data/Cards/Demo_card.png", 20, 468, 128, 212);

            AddImage("pack://application:,,,/Data/Cards/Demo_card.png", 156, 468, 128, 212);

            AddImage("pack://application:,,,/Data/Cards/Demo_card.png", 292, 468, 128, 212);

            InitializeCards();

            // Додаємо подiї для змiни кольору Border при наведеннi та натисканнi
            exitButtonImage.MouseEnter += CustomButtonText_MouseEnter;
            exitButtonImage.MouseLeave += CustomButtonText_MouseLeave;
            exitButtonImage.MouseLeftButtonDown += Exit_Button_Click;

            Canvas.SetZIndex(exitButtonImage, 110);
            Canvas.SetLeft(exitButtonImage, 360);
            Canvas.SetTop(exitButtonImage, 320);
            canvasShop.Children.Add(exitButtonImage);
        }
        private void InitializeCards()
        {

            Card card_1 = Card.Create(466, 66, 128, 212, "Rom", 0.2, 2, 5, 10, 10, "pack://application:,,,/Data/Cards/Card_Rom.png", canvasShop,
        "Ром: помiрний захист i атака.");
            card_1.image.MouseLeftButtonDown += OnCardClicked;

            Card card_2 = Card.Create(620, 66, 128, 212, "Tecila", 0.1, 8, 3, 12, 15, "pack://application:,,,/Data/Cards/Card_Tecila_1.png", canvasShop,
                "Текiла: висока атака, низький захист.");
            card_2.image.MouseLeftButtonDown += OnCardClicked;

            Card card_3 = Card.Create(774, 66, 128, 212, "Dragon", 0.3, 3, 5, 8, 12, "pack://application:,,,/Data/Cards/Card_Dragon_1.png", canvasShop,
                "Дракон: високий захист, помiрна атака.");
            card_3.image.MouseLeftButtonDown += OnCardClicked;

            Card card_4 = Card.Create(928, 66, 128, 212, "Dvarf", 0.2, 5, 4, 11, 14, "pack://application:,,,/Data/Cards/Card_Dvarf_1.png", canvasShop,
                "Дварф: збалансований захист i атака.");
            card_4.image.MouseLeftButtonDown += OnCardClicked;

            Card card_5 = Card.Create(1082, 66, 128, 212, "Elfi", 0.1, 6, 2, 13, 16, "pack://application:,,,/Data/Cards/Card_Elfi_1.png", canvasShop,
                "Ельфи: високе здоров'я, помiрна атака.");
            card_5.image.MouseLeftButtonDown += OnCardClicked;

            Card card_6 = Card.Create(466, 368, 128, 212, "Elicir", 0.25, 4, 6, 9, 18, "pack://application:,,,/Data/Cards/Card_Elicir_1.png", canvasShop,
                "Елiксир: високий захист, збалансованi бафи.");
            card_6.image.MouseLeftButtonDown += OnCardClicked;

            Card card_7 = Card.Create(620, 368, 128, 212, "Shadow", 0.2, 7, 3, 10, 20, "pack://application:,,,/Data/Cards/Card_Shadow_1.png", canvasShop,
                "Тiнь: висока атака, помiрний захист.");
            card_7.image.MouseLeftButtonDown += OnCardClicked;

            Card card_8 = Card.Create(774, 368, 128, 212, "Night", 0.1, 4, 7, 11, 22, "pack://application:,,,/Data/Cards/Card_Night_1.png", canvasShop,
                "Нiч: високий алгоритм, помiрне здоров'я.");
            card_8.image.MouseLeftButtonDown += OnCardClicked;

            Card card_9 = Card.Create(928, 368, 128, 212, "KrovavaiaMery", 0.15, 5, 6, 9, 24, "pack://application:,,,/Data/Cards/Card_KrovavaiaMery_1.png", canvasShop,
                "Кривава Мерi: збалансованi характеристики, акцент на здоров'я.");
            card_9.image.MouseLeftButtonDown += OnCardClicked;

            Card card_10 = Card.Create(1082, 368, 128, 212, "Winter", 0.2, 3, 8, 10, 26, "pack://application:,,,/Data/Cards/Card_Winter_1.png", canvasShop,
                "Зима: високий алгоритм, збалансований захист i атака.");
            card_10.image.MouseLeftButtonDown += OnCardClicked;
        }
        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            engine.ApplysScene();
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
        public void OnCardClicked(object sender, RoutedEventArgs e)
        {

            Image clickedImage = sender as Image;
            if (clickedImage != null)
            {
                Card clickedCard = clickedImage.Tag as Card;
                if (player_Shop.Money >= clickedCard.Cost)
                {
                    if (clickedCard != null && Character.Character_Cards.Count < 3)
                    {
                        int index = Array.IndexOf(occupiedPositions, false);
                        player_Shop.Money -= clickedCard.Cost;
                        uIManager.UpdateMoney(player_Shop.Money);
                        double newX = 20 + 136 * index;
                        double newY = 468;
                        string newName = "Player_" + clickedCard.Name + (index + 1).ToString();
                        Card clone = (Card)clickedCard.Clone(canvasShop, newX, newY, newName, index );
                        Character.Character_Cards.Add(clone);
                        occupiedPositions[index] = true;
                    }
                    else
                    {
                        cardToReplace = clickedCard;
                        CreatePrompt(canvasShop);
                        ShowPrompt();
                    }
                }
                else return;
            }
        }

        public void YesOption_Click(object sender, MouseButtonEventArgs e)
        {
            isSelectingCards = true;
            ActivateCardReplacementMode(sender);
            HidePrompt();
            player_Shop.Money -= cardToReplace.Cost;
            foreach (Card card in Character.Character_Cards)
            {
                card.image.Opacity = 0.5;
                card.image.MouseLeftButtonDown += CardReplaceClick;
            }
        }
        private void ActivateCardReplacementMode(object sender)
        {
            Image img = sender as Image;
            if (img == null) return;
            Card clickedCard = img.Tag as Card;
            if (clickedCard == null) return;

            foreach (Card card in Character.Character_Cards)
            {
                if (clickedCard.IsClone)
                {
                    card.image.Opacity = 0.5;
                    card.image.MouseLeftButtonDown += CardReplaceClick;
                }
            }
        }
        private void CardReplaceClick(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            if (img == null) return;
            Card clickedCard = img.Tag as Card;
            if (clickedCard == null) return;

            if (Character.Character_Cards.Contains(clickedCard))
            {
                int index = Character.Character_Cards.IndexOf(clickedCard);

                canvasShop.Children.Remove(clickedCard.image);
                string newName = cardToReplace.Name + (index + 1);
                Card clone = (Card)cardToReplace.Clone(canvasShop, Canvas.GetLeft(clickedCard.image), Canvas.GetTop(clickedCard.image), newName, index);

                Character.Character_Cards[index] = clone;

                DeactivateCardReplacementMode();
                isSelectingCards = false;
            }
        }
        private void DeactivateCardReplacementMode()
        {
            foreach (Card card in Character.Character_Cards)
            {
                if (card != null)
                {
                    card.image.Opacity = 1.0;
                    card.image.MouseLeftButtonDown -= CardReplaceClick;
                }
            }
        }
        public void AddImage(string imagePath, double x, double y, double width, double height)
        {
            Image image = new Image();

            BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
            image.Source = bitmapImage;

            Canvas.SetLeft(image, x);
            Canvas.SetTop(image, y);
            image.Width = width;
            image.Height = height;
            canvasShop.Children.Add(image);
        }
        public void ToggleVisibility()
        {
            Debug.WriteLine("Money Shop" + player_Shop.Money);
            var animation = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(0.2),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            if (!isVisible)
            {
                canvasShop.Visibility = Visibility.Visible;
                animation.From = 0;
                exitButtonImage.Visibility = Visibility.Visible;
                uIManager.ToggleBlurEffect(true);
                animation.To = 1;
            }
            else
            {
                animation.From = 1;
                animation.To = 0;
                animation.Completed += (s, e) => canvasShop.Visibility = Visibility.Hidden;
                uIManager.ToggleBlurEffect(false);
            }

            canvasShop.BeginAnimation(Canvas.OpacityProperty, animation);
            isVisible = !isVisible;

        }
        private void CreatePrompt(Canvas canvasShop)
        {
            promptText = new TextBlock
            {
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                Text = "Список карток повний. Хочете замiнити iснуючу картку?",
                FontSize = 14,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            yesOption = new TextBlock
            {
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                Text = "Так",
                FontSize = 14,
                Foreground = Brushes.Green,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10),
                Cursor = Cursors.Hand,
            };
            yesOption.MouseLeftButtonDown += YesOption_Click;

            noOption = new TextBlock
            {
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                Text = "Нi",
                FontSize = 14,
                Foreground = Brushes.Red,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10),
                Cursor = Cursors.Hand,
            };
            noOption.MouseLeftButtonDown += NoOption_Click;

            StackPanel panel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            panel.Children.Add(promptText);
            panel.Children.Add(yesOption);
            panel.Children.Add(noOption);

            promptBorder = new Border
            {
                Background = new SolidColorBrush(Colors.Black) { Opacity = 1 },
                Width = 300,
                Height = 150,
                Child = panel,
                Visibility = Visibility.Hidden
            };

            Canvas.SetLeft(promptBorder, (canvasShop.Width - promptBorder.Width) / 2);
            Canvas.SetTop(promptBorder, (canvasShop.Height - promptBorder.Height) / 2);
            Canvas.SetZIndex(promptBorder, 5);
            canvasShop.Children.Add(promptBorder);
        }

        private void ShowPrompt()
        {
            promptBorder.Visibility = Visibility.Visible;
        }

        private void HidePrompt()
        {
            promptBorder.Visibility = Visibility.Hidden;
        }


        public void NoOption_Click(object sender, MouseButtonEventArgs e)
        {
            HidePrompt();
        }

    }
}
