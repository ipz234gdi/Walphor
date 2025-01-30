using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Shapes;

namespace Walphor
{
    internal class DialogManager
    {
        private Canvas canvasDialog;
        private GameEngine gameEngine;
        private Grid gridGame;
        private UIManager UIManager;
        private SoundManager soundManager;
        private UICreator uiCreator;
        private DialogRepository dialogRepository;

        private TextBlock dialogTextBlock;
        private Border dialogBorder;
        private TextBlock dialogNameTextBlock;
        private Border dialogNameBorder;
        private TextBlock dialogButton_Battle_TextBlock;
        private Border dialogButton_Battle_Border;
        private TextBlock dialogButton_NoBattle_TextBlock;
        private Border dialogButton_NoBattle_Border;


        private int currentDialogueIndex = 0;
        private NPC curentNpc;
        private Character character;
        private string currentNPCName = "";
        private string dialogKey = "";
        private bool pedro = false;

        public DialogManager(Grid gridGame, Character character, GameEngine gameEngine, SoundManager soundManager, UIManager uIManager)
        {
            this.gridGame = gridGame;
            this.character = character;
            this.gameEngine = gameEngine;
            InitializeDialogUI();
            this.soundManager = soundManager;
            this.UIManager = uIManager;
            this.uiCreator = uIManager.uiCreator;
            this.dialogRepository = new DialogRepository();
        }

        private void InitializeDialogUI()
        {
            canvasDialog = new Canvas
            {
                Name = "canvasDialog",
                Width = 1280,
                Height = 720,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Visibility = Visibility.Visible
            };

            gridGame.Children.Add(canvasDialog);
            Panel.SetZIndex(canvasDialog, 20);

            var(borderButton_Battle, textBlockButton_Battle) = CreateDialogComponent(260, 44, 40 + 1200 - 260, 720 - 344, "pack://application:,,,/Data/Dialog/Dialog_B.png", 26, Brushes.Black, new Thickness(10, 5, 5, 5), false);
            canvasDialog.Children.Add(borderButton_Battle);
            this.dialogButton_Battle_TextBlock = textBlockButton_Battle;
            this.dialogButton_Battle_Border = borderButton_Battle;

            var (borderButton_NoBattle, textBlockButton_NoBattle) = CreateDialogComponent(260, 44, 40 + 1200 - 260, 720 - 344 - 50, "pack://application:,,,/Data/Dialog/Dialog_B.png", 26, Brushes.Black, new Thickness(10, 5, 5, 5), false);
            canvasDialog.Children.Add(borderButton_NoBattle);
            this.dialogButton_NoBattle_TextBlock = textBlockButton_NoBattle;
            this.dialogButton_NoBattle_Border = borderButton_NoBattle;


            var (borderName, textBlockName) = CreateDialogComponent(260, 44, 40, 720 - 288, "pack://application:,,,/Data/Dialog/Name_Dialog.png", 26, Brushes.Black, new Thickness(10, 5, 5, 5), false);
            canvasDialog.Children.Add(borderName);
            this.dialogNameTextBlock = textBlockName;
            this.dialogNameBorder = borderName;

            var (borderDialog, textBlockDialog) = CreateDialogComponent(1200, 200, 40, 720 - 240, "pack://application:,,,/Data/Dialog/Dialog.png", 32, Brushes.Black, new Thickness(10), false);
            canvasDialog.Children.Add(borderDialog);
            this.dialogTextBlock = textBlockDialog;
            this.dialogBorder = borderDialog;

            borderButton_Battle.MouseLeftButtonDown += ClickButton_Battle;
            borderButton_NoBattle.MouseLeftButtonDown += ClickButton_NoBattle;
            gridGame.KeyDown += Canvas_KeyDown;

        }

        private void ClickButton_Battle(object sender, MouseButtonEventArgs e)
        {
            dialogBorder.Visibility = Visibility.Hidden;
            dialogNameBorder.Visibility = Visibility.Hidden;
            dialogButton_Battle_Border.Visibility = Visibility.Hidden;
            dialogButton_NoBattle_Border.Visibility = Visibility.Hidden;
            GameEngine.inDialog = false;
            soundManager.PlayClickSound(gameEngine.GetSoundPath("klik.mp3"));
            gameEngine.OpenScene();

        }
        private void ClickButton_NoBattle(object sender, MouseButtonEventArgs e)
        {
            dialogBorder.Visibility = Visibility.Hidden;
            dialogNameBorder.Visibility = Visibility.Hidden;
            dialogButton_Battle_Border.Visibility = Visibility.Hidden;
            dialogButton_NoBattle_Border.Visibility = Visibility.Hidden;
            GameEngine.inDialog = false;
            soundManager.PlayClickSound(gameEngine.GetSoundPath("klik.mp3"));
            gameEngine.ToggleCursorVisibility(false);
        }
        private (Border, TextBlock) CreateDialogComponent(double width, double height, double left, double top, string imagePath, double fontSize, SolidColorBrush textColor, Thickness padding, bool visible)
        {
            TextBlock textBlock = new TextBlock
            {
                FontSize = fontSize,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#F77 Minecraft"),
                RenderTransform = new TranslateTransform(0, 4),
                Foreground = textColor,
                TextWrapping = TextWrapping.Wrap,
                Padding = padding
            };

            Border border = new Border
            {
                Width = width,
                Height = height,
                Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute)),
                    Stretch = Stretch.Fill
                },
                Child = textBlock,
                Visibility = visible ? Visibility.Visible : Visibility.Hidden
            };

            RenderOptions.SetBitmapScalingMode(border, BitmapScalingMode.NearestNeighbor);
            Canvas.SetLeft(border, left);
            Canvas.SetTop(border, top);

            return (border, textBlock);
        }
        private void Canvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (dialogBorder.Visibility == Visibility.Hidden && dialogNameBorder.Visibility == Visibility.Hidden) return;

                currentDialogueIndex++;

                string dialogueText = GetNextDialogueText();

                if (!string.IsNullOrEmpty(dialogueText))
                {
                    dialogTextBlock.Text = dialogueText;
                    dialogNameTextBlock.Text = currentNPCName.ToString();

                    soundManager.StopWalkingSound();
                    soundManager.StopDialogueSound();
                    soundManager.PlayDialogueSound(gameEngine.GetSoundPath("Voicy.mp3"));
                }
                else
                {
                    ShowBattleOptions();

                    soundManager.StopWalkingSound();
                }
            }
        }
        private string GetNextDialogueText()
        {
            if (curentNpc.Name == "Jery") uiCreator.taskTextBlock.Text = "Завдання: поговорити з Pedro";
            List<string> dialogues = dialogRepository.GetDialogue(dialogKey);
            
            if (currentDialogueIndex < dialogues.Count)
            {
                return dialogues[currentDialogueIndex];
            }

            return null;
        }

        private void ShowBattleOptions()
        {
            if (curentNpc.Name != "Pedro")
            {
                if (curentNpc.Level == character.Level) dialogButton_Battle_Border.Visibility = Visibility.Visible;
                dialogButton_NoBattle_Border.Visibility = Visibility.Visible;

                if (curentNpc.Level == character.Level) dialogButton_Battle_TextBlock.Text = "Бій";
                dialogButton_NoBattle_TextBlock.Text = "Покинути нпс";
            }
            else
            {
                dialogButton_Battle_Border.Visibility = Visibility.Visible;
                dialogButton_NoBattle_Border.Visibility = Visibility.Visible;
                dialogButton_Battle_TextBlock.Text = "В Бар";
                dialogButton_NoBattle_TextBlock.Text = "Покинути нпс";
            }

            soundManager.StopWalkingSound();
            soundManager.PlayWalkingSound(gameEngine.GetSoundPath("Voicy.mp3"));
        }
        public void StartDialog(NPC activenPC)
        {
            if (activenPC == null) return;
            
            curentNpc = activenPC;
            currentNPCName = activenPC.Name;
            currentDialogueIndex = 0;

            dialogKey = activenPC.HasInteracted ? activenPC.Name + "_Battle" : activenPC.Name;

            List<string> dialogues = dialogRepository.GetDialogue(dialogKey);

            if (dialogues.Count > 0)
            {
                dialogNameTextBlock.Text = currentNPCName.ToString();
                dialogTextBlock.Text = dialogues[currentDialogueIndex];
                dialogBorder.Visibility = Visibility.Visible;
                dialogNameBorder.Visibility = Visibility.Visible;
            }

            activenPC.HasInteracted = true;
        }

    }
}
