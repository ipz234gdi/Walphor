using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Threading;
using MySql.Data.MySqlClient;
using System.Data.SQLite;
using System.IO;
using Path = System.IO.Path;
using System.Reflection;
using System.Timers;

namespace Walphor
{
    /// <summary>
    /// Interaction logic for GameEngine.xaml
    /// </summary>
    public partial class GameEngine : Window
    {
        public int GAMEFPS = 0;
        string connectionString = "server=localhost;uid=admin;password=0192837465Den;database=SaveGame;";
        private Character player;
        private ButtonManager wallButton;
        private Map map;
        private SoundManager soundManager;
        private bool _moveUp;
        private bool _moveDown;
        private bool _moveLeft;
        private bool _moveRight;
        public double Acceleration = 4.0;
        public double MaxSpeed = 4.0;
        private const double Friction = 0.2;
        
        private NPC activeNPC;
        public bool inBattle = false;
        public bool inMenu = false;
        public bool inProfile = true;
        public bool inStatistic = false;
        public bool inShop = false;
        public bool inFishing = false;
        public bool inSettings = false;
        public bool inRules = false;
        private bool fishingCol = false;
        public static bool inDialog = false;
        public bool help = true;

        private CollisionManager collisionManager;

        private UIManager uiManager;
        private Menu gameMenu;
        private Battle battle;
        private Shop shop;
        private Fishing fishing;
        private DialogManager dialogManager;
        private ItemManager activeItem;

        private List<NPC> listOfNPCs = new List<NPC>();
        private List<Wall> walls = new List<Wall>();
        private List<Wall> wallsslow = new List<Wall>();
        private List<ItemManager> listOfItems_tree = new List<ItemManager>();
        private List<ItemManager> listOfItems_home = new List<ItemManager>();
        private List<ItemManager> mapOfItems = new List<ItemManager>();

        private List<ItemManager> listOfItems = new List<ItemManager>();

        MySqlConnection conn;
        public string BDPath;
        private DispatcherTimer gameTimer;
        private TimeSpan gameTime;

        private UICreator uiCreator;
        public GameEngine()
        {
            InitializeComponent();
            CreateDatabase();

            soundManager = new SoundManager();

            BDPath = GetDatabasePath();

            map = new Map(canvasGame);

            InitializeGame();

            this.uiCreator = new UICreator();

            uiCreator.InitializeUI(gridGame, player.Money, player.Level, 1);

            uiManager = new UIManager(gridGame, player, 1, this, gameMenu, BDPath, soundManager, canvasGame, uiCreator);

            gameMenu = new Menu(gridGame, canvasGame, this, uiManager, player, soundManager);

            uiManager.SetMenu(gameMenu, uiCreator);
            gameMenu.SetUIManager(uiManager, uiCreator);

            shop = new Shop(gridGame, player, uiManager, this);

            collisionManager = new CollisionManager(canvasGame, wallButton, uiManager, this);

            uiManager.MoveImage("Coin", -2000, 20);
            uiManager.OpacityObject(uiCreator.levelTextBlock, 0);
            uiManager.OpacityImage("Level", 0);
            uiManager.OpacityObject(uiCreator.taskTextBlock, 0);
            uiCreator.taskBorder.Visibility = Visibility.Hidden;
            uiManager.OpacityImage("HP", 0);
            uiManager.OpacityImage("bibbing", 0);
            uiManager.OpacityImage("Defense", 0);
            uiManager.OpacityImage("HP_NPC", 0);
            uiManager.OpacityImage("Defense_NPC", 0);
            uiManager.OpacityRectangle("DEF_NPC", 0);
            uiManager.OpacityRectangle("HP_NPC", 0);
            uiManager.OpacityRectangle("DEF", 0);
            uiManager.OpacityRectangle("HP", 0);
            uiManager.OpacityRectangle("BIB", 0);
            uiManager.OpacityObject(uiCreator.level_NPCTextBlock, 0);
            uiManager.OpacityImage("Level_NPC", 0);

            battle = new Battle(gridGame, player, activeNPC, uiManager, this, shop, soundManager);
            fishing = new Fishing(gridGame, player, uiManager, this);

            dialogManager = new DialogManager(gridGame, player, this, soundManager, uiManager);

            ToggleCursorVisibility(true);

            conn = new MySqlConnection(connectionString);

            soundManager.PlayMainMusic(GetSoundPath("mine.mp3"));


        }
        public string GetSoundPath(string music)
        {
            // Визначення шляху до папки проекту з використанням вiдносного шляху
            string executableLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string projectDirectory = Path.GetFullPath(Path.Combine(executableLocation, @"..\..\"));
            string databaseDirectory = Path.Combine(projectDirectory, "Sound");

            // Створення папки, якщо вона не iснує
            if (!Directory.Exists(databaseDirectory))
            {
                Directory.CreateDirectory(databaseDirectory);
            }
            string databasePath = System.IO.Path.Combine(databaseDirectory, music);
            return databasePath;
        }
        //-------------------GENERAL_DB-----------------------
        //----------------------------------------------------
        public void InsertOrUpdatePlayerData(string playerName, string password, double gameTime)
        {
            string dbPath = $"Data Source={GetDatabasePath()};Version=3;";
            using (var conn = new SQLiteConnection(dbPath))
            {
                conn.Open();

                // Перевiрка, чи iснує гравець з таким iменем та паролем
                string sqlCheck = "SELECT id FROM players WHERE name = @name AND password = @password";
                using (var cmdCheck = new SQLiteCommand(sqlCheck, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@name", playerName);
                    cmdCheck.Parameters.AddWithValue("@password", password);
                    var id = cmdCheck.ExecuteScalar();

                    if (id != null)
                    {
                        string sqlUpdate = "UPDATE players SET level = @level, money = @money WHERE id = @id";
                        using (var cmdUpdate = new SQLiteCommand(sqlUpdate, conn))
                        {
                            cmdUpdate.Parameters.AddWithValue("@level", player.Level);
                            cmdUpdate.Parameters.AddWithValue("@money", player.Money);
                            cmdUpdate.Parameters.AddWithValue("@id", id);
                            cmdUpdate.ExecuteNonQuery();
                        }
                        InitializeTimer();
                        LoadGameTime();
                    }
                    else
                    {
                        // Якщо гравець не iснує, створюємо новий запис
                        string sqlInsert = "INSERT INTO players (name, password, time, level, money) VALUES (@name, @password, @time, @level, @money)";
                        using (var cmdInsert = new SQLiteCommand(sqlInsert, conn))
                        {
                            cmdInsert.Parameters.AddWithValue("@name", playerName);
                            cmdInsert.Parameters.AddWithValue("@password", password);
                            cmdInsert.Parameters.AddWithValue("@time", gameTime);
                            cmdInsert.Parameters.AddWithValue("@level", player.Level);
                            cmdInsert.Parameters.AddWithValue("@money", player.Money);
                            cmdInsert.ExecuteNonQuery();
                        }
                        InitializeTimer();
                    }
                }
            }
        }
        public static string GetDatabasePath()
        {
            // Визначення шляху до папки проекту з використанням вiдносного шляху
            string executableLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string projectDirectory = Path.GetFullPath(Path.Combine(executableLocation, @"..\..\"));
            string databaseDirectory = Path.Combine(projectDirectory, "DataBase");

            // Створення папки, якщо вона не iснує
            if (!Directory.Exists(databaseDirectory))
            {
                Directory.CreateDirectory(databaseDirectory);
            }
            string databasePath = System.IO.Path.Combine(databaseDirectory, "Profiles.sqlite");
            return databasePath;
        }
        public static void CreateDatabase()
        {

            string dbPath = $"Data Source={GetDatabasePath()};Version=3;";
            if (!File.Exists(GetDatabasePath()))
            {
                SQLiteConnection.CreateFile(GetDatabasePath());
            }

            using (var conn = new SQLiteConnection(dbPath))
            {
                conn.Open();

                string sql = @"
                CREATE TABLE IF NOT EXISTS players (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL UNIQUE,
                    password TEXT NOT NULL,
                    time DOUBLE NOT NULL,
                    level DOUBLE NOT NULL,
                    money INTEGER NOT NULL
                );";

                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
            }
        }
        private void InitializeGame()
        {
            map = new Map(canvasGame);
            // Iнiцiалiзацiя об'єктiв з їх початковими координатами
            ItemManager mapItem = ItemManager.Create(-4000, -5000, 8192, 8192, "pack://application:,,,/Data/Map_6.png", canvasGame, 1);
            listOfItems.Add(mapItem);
            mapOfItems.Add(mapItem);

            ItemManager home_1 = ItemManager.Create(-320, -2284, 836, 456, "pack://application:,,,/Data/item/Home_1.png", canvasGame, 2);
            listOfItems_home.Add(home_1);
            listOfItems.Add(home_1);

            ItemManager home_2 = ItemManager.Create(1050, -1650, 316, 556, "pack://application:,,,/Data/item/Home_2.png", canvasGame, 3);
            listOfItems_home.Add(home_2);
            listOfItems.Add(home_2);

            ItemManager home_3 = ItemManager.Create(-1250, -1600, 316, 556, "pack://application:,,,/Data/item/Home_3.png", canvasGame, 4);
            listOfItems_home.Add(home_3);
            listOfItems.Add(home_3);

            ItemManager home_4 = ItemManager.Create(-400, -750, 316, 556, "pack://application:,,,/Data/item/Home_4.png", canvasGame, 5);
            listOfItems_home.Add(home_4);
            listOfItems.Add(home_4);

            ItemManager tree_1 = ItemManager.Create(400, -400, 368, 396, "pack://application:,,,/Data/item/tree.png", canvasGame, 6);
            listOfItems_tree.Add(tree_1);
            listOfItems.Add(tree_1);

            ItemManager tree_2 = ItemManager.Create(700, -800, 368, 396, "pack://application:,,,/Data/item/tree.png", canvasGame, 7);
            listOfItems_home.Add(tree_2);
            listOfItems.Add(tree_2);

            ItemManager tree_3 = ItemManager.Create(1400, -600, 368, 396, "pack://application:,,,/Data/item/tree.png", canvasGame, 8);
            listOfItems_tree.Add(tree_3);
            listOfItems.Add(tree_3);

            ItemManager tree_4 = ItemManager.Create(1500, -1600, 368, 396, "pack://application:,,,/Data/item/tree.png", canvasGame, 9);
            listOfItems_tree.Add(tree_4);
            listOfItems.Add(tree_4);

            ItemManager tree_5 = ItemManager.Create(1200, -2400, 368, 396, "pack://application:,,,/Data/item/tree.png", canvasGame, 10);
            listOfItems_tree.Add(tree_5);
            listOfItems.Add(tree_5);

            ItemManager tree_6 = ItemManager.Create(600, -2200, 368, 396, "pack://application:,,,/Data/item/tree.png", canvasGame, 11);
            listOfItems_tree.Add(tree_6);
            listOfItems.Add(tree_6);

            ItemManager tree_7 = ItemManager.Create(-800, -2600, 368, 396, "pack://application:,,,/Data/item/tree.png", canvasGame, 12);
            listOfItems_tree.Add(tree_7);
            listOfItems.Add(tree_7);

            ItemManager tree_8 = ItemManager.Create(-1600, -2400, 368, 396, "pack://application:,,,/Data/item/tree.png", canvasGame, 13);
            listOfItems_tree.Add(tree_8);
            listOfItems.Add(tree_8);

            ItemManager tree_9 = ItemManager.Create(-1700, -1900, 368, 396, "pack://application:,,,/Data/item/tree.png", canvasGame, 14);
            listOfItems_tree.Add(tree_9);
            listOfItems.Add(tree_9);

            ItemManager tree_10 = ItemManager.Create(-900, -500, 368, 396, "pack://application:,,,/Data/item/tree.png", canvasGame, 15);
            listOfItems_tree.Add(tree_10);
            listOfItems.Add(tree_10);

            ItemManager tree_11 = ItemManager.Create(-1600, -400, 368, 396, "pack://application:,,,/Data/item/tree.png", canvasGame, 16);
            listOfItems_tree.Add(tree_11);
            listOfItems.Add(tree_11);

            ItemManager tree_12 = ItemManager.Create(-1800, -1000, 368, 396, "pack://application:,,,/Data/item/tree.png", canvasGame, 17);
            listOfItems_tree.Add(tree_12);
            listOfItems.Add(tree_12);

            Wall wall_down = new Wall(-2300, 680, 4700, 100, canvasGame, 1);
            wall_down.SetColor(Colors.Blue);
            walls.Add(wall_down);

            Wall wall_right = new Wall(2350, -3000, 100, 4700, canvasGame, 2);
            wall_right.SetColor(Colors.Blue);
            walls.Add(wall_right);

            Wall wall_up = new Wall(-2300, -3050, 4700, 100, canvasGame, 3);
            wall_up.SetColor(Colors.Blue);
            walls.Add(wall_up);

            Wall wall_left = new Wall(-2400, -3000, 100, 4700, canvasGame, 4);
            wall_left.SetColor(Colors.Blue);
            walls.Add(wall_left);

            Wall wall_slow = new Wall(170, -850, 1680, 1000, canvasGame, 5);
            wall_slow.SetColor(Colors.Red);
            wallsslow.Add(wall_slow);
            Wall wall_slow2 = new Wall(620, -1250, 250, 500, canvasGame, 6);
            wall_slow2.SetColor(Colors.Red);
            wallsslow.Add(wall_slow2);
            Wall wall_slow3 = new Wall(1470, -2450, 370, 2000, canvasGame, 7);
            wall_slow3.SetColor(Colors.Red);
            wallsslow.Add(wall_slow3);
            Wall wall_slow4 = new Wall(1020, -1650, 500, 680, canvasGame, 8);
            wall_slow4.SetColor(Colors.Red);
            wallsslow.Add(wall_slow4);
            Wall wall_slow5 = new Wall(-1800, -2450, 3400, 700, canvasGame, 9);
            wall_slow5.SetColor(Colors.Red);
            wallsslow.Add(wall_slow5);
            Wall wall_slow6 = new Wall(620, -1870, 250, 500, canvasGame, 10);
            wall_slow6.SetColor(Colors.Red);
            wallsslow.Add(wall_slow6);
            Wall wall_slow7 = new Wall(-700, -1870, 250, 500, canvasGame, 11);
            wall_slow7.SetColor(Colors.Red);
            wallsslow.Add(wall_slow7);
            Wall wall_slow8 = new Wall(-1800, -2450, 520, 2400, canvasGame, 12);
            wall_slow8.SetColor(Colors.Red);
            wallsslow.Add(wall_slow8);
            Wall wall_slow9 = new Wall(-1320, -1620, 500, 680, canvasGame, 13);
            wall_slow9.SetColor(Colors.Red);
            wallsslow.Add(wall_slow9);
            Wall wall_slow10 = new Wall(-700, -1220, 250, 500, canvasGame, 14);
            wall_slow10.SetColor(Colors.Red);
            wallsslow.Add(wall_slow10);
            Wall wall_slow11 = new Wall(-1800, -850, 1800, 1000, canvasGame, 15);
            wall_slow11.SetColor(Colors.Red);
            wallsslow.Add(wall_slow11);

            // Створюємо гравця
            player = Character.Create(1280 / 2.0 - 20, 720 / 2.0 - 45, 51, 96, 1, 100, 52, 12, 0, 0, "", "pack://application:,,,/Data/Character/down_idle_1.png", canvasGame);

            // Створюємо NPC
            NPC npc = NPC.Create(950, -1400, 51, 96, "pack://application:,,,/Data/NPC/down_idle_1_Gery.png", "Gery", 1, 100, 100, 12, canvasGame);
            listOfNPCs.Add(npc);

            // Створюємо NPC2
            NPC npc_2 = NPC.Create(-900, -1400, 51, 96, "pack://application:,,,/Data/NPC/down_idle_1_Miranda.png", "Miranda", 2, 100, 100, 16, canvasGame);
            listOfNPCs.Add(npc_2);

            // Створюємо NPC3
            NPC Shopper = NPC.Create(0, -1800, 51, 96, "pack://application:,,,/Data/NPC/down_idle_1_Pedro.png", "Pedro", 1, 100, 100, 18, canvasGame);
            listOfNPCs.Add(Shopper);

            // Створюємо NPC4
            NPC npc_3 = NPC.Create(-50, -700, 51, 96, "pack://application:,,,/Data/NPC/down_idle_1_Lusia.png", "Lusia", 3, 100, 100, 20, canvasGame);
            listOfNPCs.Add(npc_3);

            // Створюємо NPC5
            NPC npc_4 = NPC.Create(400, 200, 51, 96, "pack://application:,,,/Data/NPC/down_idle_1_Jery.png", "Jery", 4, 100, 100, 22, canvasGame);
            listOfNPCs.Add(npc_4);

            // Додаємо обробник клавiш для керування гравцем
            this.PreviewKeyUp += GameEngine_PreviewKeyUp;
            this.PreviewKeyDown += GameEngine_PreviewKeyDown;
            this.PreviewKeyDown += OpenSettings_Click;
            CompositionTarget.Rendering += GameLoop;
            
        }

        //-------------------Save_Game-----------------------
        //---------------------------------------------------
        public void SaveCurrentGame()
        {
            SaveGame saveGame = new SaveGame();
            saveGame.Save(listOfNPCs, listOfItems_home, listOfItems_tree, mapOfItems, walls, wallsslow, player, shop.occupiedPositions, soundManager.GetMasterVolume());
        }
        public void LoadGame(string playerName)
        {
            SaveGame saveGame = SaveGame.Load(playerName);
            if (saveGame != null)
            {
                // Відновлення даних гравця
                player.Level = saveGame.PlayerLevel;
                player.Money = saveGame.PlayerMoney;

                // Відновлення стану occupiedPositions
                if (saveGame.OccupiedPositions != null)
                {
                    shop.occupiedPositions = saveGame.OccupiedPositions;
                }
                else
                {
                    shop.occupiedPositions = new bool[3];
                }

                // Відновлення карток персонажа
                Character.Character_Cards.Clear();
                foreach (var cardData in saveGame.CharacterCardsData)
                {
                    var card = Card.Create(cardData.X, cardData.Y, cardData.Width, cardData.Height, cardData.Name, cardData.Def, cardData.Atck, cardData.Algo, cardData.Health, cardData.Cost, cardData.ImageSource, shop.canvasShop, cardData.InfoCard);
                    card.Index = cardData.Index;
                    Character.Character_Cards.Add(card);
                }

                // Відновлення інших об'єктів
                foreach (var data in saveGame.GameObjectDataList)
                {
                    switch (data.Type)
                    {
                        case "NPC":
                            var npc = listOfNPCs.FirstOrDefault(n => n.Name == data.Name);
                            if (npc != null)
                            {
                                npc.SetPosition(data.X, data.Y);
                                npc.ChangeSize(data.Width, data.Height);
                            }
                            break;
                        case "HomeItem":
                            var homeItem = listOfItems_home.FirstOrDefault(i => i.Id == data.Id);
                            if (homeItem != null)
                            {
                                homeItem.SetPosition(data.X, data.Y);
                                homeItem.SetSize(data.Width, data.Height);
                            }
                            break;
                        case "TreeItem":
                            var treeItem = listOfItems_tree.FirstOrDefault(i => i.Id == data.Id);
                            if (treeItem != null)
                            {
                                treeItem.SetPosition(data.X, data.Y);
                                treeItem.SetSize(data.Width, data.Height);
                            }
                            break;
                        case "MapItem":
                            var mapItem = mapOfItems.FirstOrDefault(i => i.Id == data.Id);
                            if (mapItem != null)
                            {
                                mapItem.SetPosition(data.X, data.Y);
                                mapItem.SetSize(data.Width, data.Height);
                            }
                            break;
                        case "Wall":
                            var wall = walls.FirstOrDefault(w => w.Id == data.Id);
                            if (wall != null)
                            {
                                wall.SetPosition(data.X, data.Y);
                            }
                            break;
                        case "WallSlow":
                            var wallSlow = wallsslow.FirstOrDefault(w => w.Id == data.Id);
                            if (wallSlow != null)
                            {
                                wallSlow.SetPosition(data.X, data.Y);
                            }
                            break;
                    }
                }
                soundManager.LoadMasterVolume(saveGame.MasterVolume);
                uiCreator.volumeSlider.Value = soundManager.GetMasterVolume();
            }
        }

        //-------------------TIMER_DB------------------------
        //---------------------------------------------------
        private void InitializeTimer()
        {
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromSeconds(1);
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Stop();
        }
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            
            gameTime = gameTime.Add(TimeSpan.FromSeconds(1));
            Debug.WriteLine("Game Time: " + gameTime.ToString());
        }
        public void StartTimer()
        {
            gameTimer?.Start();
        }

        public void StopTimer()
        {
            gameTimer?.Stop();
            SaveGameTime();
        }
        private void LoadGameTime()
        {
            gameTime = GetGameTimeFromDatabase();
        }

        private void SaveGameTime()
        {
            SaveGameTimeToDatabase(gameTime);
        }
        private TimeSpan GetGameTimeFromDatabase()
        {
            string dbPath = $"Data Source={GetDatabasePath()};Version=3;";
            TimeSpan gameTime = TimeSpan.Zero;

            using (var conn = new SQLiteConnection(dbPath))
            {
                conn.Open();
                string sql = "SELECT time FROM players WHERE name = @name";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", player.Name);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        gameTime = TimeSpan.FromSeconds(Convert.ToDouble(result));
                    }
                }
            }

            return gameTime;
        }
        private void SaveGameTimeToDatabase(TimeSpan time)
        {
            string dbPath = $"Data Source={GetDatabasePath()};Version=3;";

            using (var conn = new SQLiteConnection(dbPath))
            {
                conn.Open();
                string sql = "UPDATE players SET time = @time, level = @level, money = @money WHERE name = @name";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@time", time.TotalSeconds);
                    cmd.Parameters.AddWithValue("@level", player.Level);
                    cmd.Parameters.AddWithValue("@money", player.Money);
                    cmd.Parameters.AddWithValue("@name", player.Name);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        //-------------------GENERAL_LOOP-----------------------
        //------------------------------------------------------
        private void GameLoop(object sender, EventArgs e)
        {
            var collisionDirections = collisionManager.CheckCollisions(player, listOfNPCs, walls, listOfItems_home, listOfItems_tree);

            activeNPC = collisionManager.ButtonColision(player, listOfNPCs, canvasGame);

            activeItem = collisionManager.ItemColision(player, listOfItems, canvasGame);

            fishingCol = collisionManager.CheckProximity(player, walls);

            MaxSpeed = collisionManager.MoveSlow(player, wallsslow);
            Acceleration = collisionManager.MoveSlow(player, wallsslow);

            ZPosition();

            Move(collisionDirections);

            if (player.Level >= 5)
            {
                uiManager.ShowGameEndMenu("You Win!"); // Відображаємо меню завершення гри при досягненні 5 рівня
                return;
            }

            if (GAMEFPS <= 60) GAMEFPS += 1;
            else GAMEFPS = 0;

            if(player.Level == 2) uiCreator.taskTextBlock.Text = "Завдання: поговорити з Miranda";
            if (player.Level == 3) uiCreator.taskTextBlock.Text = "Завдання: поговорити з Lusia";
            if (player.Level == 4) uiCreator.taskTextBlock.Text = "Завдання: поговорити з Jery";

            
        }
        public void EndGame()
        {
            // Зупиняємо таймер
            StopTimer();

            // Скидаємо час гри
            gameTime = TimeSpan.Zero;
        }
        public void ToggleCursorVisibility(bool visible)
        {
            if (visible == true)
            {
                // Якщо курсор зараз схований, вiдобразити його
                this.Cursor = Cursors.Arrow; // або Cursors.Default
            }
            if (visible == false)
            {
                // Якщо курсор видимий, сховати його
                this.Cursor = Cursors.None;
            }
        }

        private void ZPosition()
        {
            if (activeItem != null)
            {
                if (player.Y - 290 < activeItem.Y)
                {
                    activeItem.ZPosition(2);
                    player.ZPosition(1);
                }
                else
                {
                    player.ZPosition(2);
                    activeItem.ZPosition(1);
                }
            }

            if (activeNPC != null)
            {
                if (player.Y > activeNPC.Y)
                {
                    activeNPC.ZPosition(1);
                    player.ZPosition(2);
                }
                else
                {
                    player.ZPosition(1);
                    activeNPC.ZPosition(2);
                }
            }
        }
        public async void Help()
        {
            uiManager.UpdateHelp("Перемiщення:", "(W.A.S.D)", true);
            await Task.Delay(6000);
            help = false;
        }


        //-------------------MOVE-----------------------
        //----------------------------------------------
        private bool isMoving;
        private void Move((bool up, bool down, bool left, bool right) collisions)
        {
            Vector tempVelocity = new Vector(0, 0);

            // Перевiрка дiагональних напрямкiв спочатку
            if (_moveUp && _moveRight && !collisions.down && !collisions.left)
            {
                tempVelocity.Y -= Acceleration;
                tempVelocity.X += Acceleration;
                player.ChangeDirection("down_right");
                isMoving = true;
            }
            else if (_moveUp && _moveLeft && !collisions.down && !collisions.right)
            {
                tempVelocity.Y -= Acceleration;
                tempVelocity.X -= Acceleration;
                player.ChangeDirection("down_left");
                isMoving = true;
            }
            else if (_moveDown && _moveRight && !collisions.up && !collisions.left)
            {
                tempVelocity.Y += Acceleration;
                tempVelocity.X += Acceleration;
                player.ChangeDirection("up_right");
                isMoving = true;
            }
            else if (_moveDown && _moveLeft && !collisions.up && !collisions.right)
            {
                tempVelocity.Y += Acceleration;
                tempVelocity.X -= Acceleration;
                player.ChangeDirection("up_left");
                isMoving = true;
            }
            // Потiм перевiрка простих напрямкiв
            else if (_moveUp && !collisions.down)
            {
                tempVelocity.Y -= Acceleration;
                player.ChangeDirection("down");
                isMoving = true;
            }
            else if (_moveDown && !collisions.up)
            {
                tempVelocity.Y += Acceleration;
                player.ChangeDirection("up");
                isMoving = true;
            }
            else if (_moveLeft && !collisions.right)
            {
                tempVelocity.X -= Acceleration;
                player.ChangeDirection("left");
                isMoving = true;
            }
            else if (_moveRight && !collisions.left)
            {
                tempVelocity.X += Acceleration;
                player.ChangeDirection("right");
                isMoving = true;
            }
            else 
            {
                player.ChangeDirection("down_idle");
                isMoving = false;
            } 

            if (tempVelocity.Length > MaxSpeed)
                tempVelocity = tempVelocity * (MaxSpeed / tempVelocity.Length);

            ApplyFriction(ref tempVelocity);

            // Фактичне перемiщення
            map.Move(tempVelocity.X, tempVelocity.Y);
            foreach (var npc in listOfNPCs) npc.Move(tempVelocity.X, tempVelocity.Y);
            foreach (var item in listOfItems) item.Move(tempVelocity.X, tempVelocity.Y);
            foreach (var walls in walls) walls.Move(tempVelocity.X, tempVelocity.Y);
            foreach (var walls in wallsslow) walls.Move(tempVelocity.X, tempVelocity.Y);
            player.Move(0, 0);

            HandleStepSound();
        }
        private bool wasMoving;
        private void HandleStepSound()
        {
            string walkingSoundPath;

            if (Acceleration < 4)
            {
                walkingSoundPath = GetSoundPath("walking.mp3");
            }
            else
            {
                walkingSoundPath = GetSoundPath("sand.mp3");
            }

            if (isMoving)
            {
                if (!wasMoving || (walkingSoundPath != soundManager.CurrentWalkingSoundPath))
                {
                    soundManager.StopWalkingSound();  // Stop the current walking sound
                    soundManager.PlayWalkingSound(walkingSoundPath);  // Play the new walking sound
                }
            }
            else
            {
                if (wasMoving)
                {
                    soundManager.StopWalkingSound();
                }
            }

            wasMoving = isMoving;
        }
        private void ApplyFriction(ref Vector velocity)
        {
            if (velocity.X != 0) velocity.X -= Friction;
            if (velocity.Y != 0) velocity.Y -= Friction;
        }
        private void GameEngine_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (inFishing) return;
            if (inMenu || inProfile) return;


            if (e.Key == Key.E && activeNPC != null && inBattle == false && inShop == false && inFishing == false && inDialog == false)
            {
                OpenDialog();
            }
            if (e.Key == Key.E && inFishing == false && fishingCol == true && inBattle == false)
            {
                fishing.ToggleVisibility();
            }
            if (e.Key == Key.Q && (inBattle == true || inShop == true))
            {
                ApplysScene();
            }
            if (inBattle || inShop || inFishing || inDialog) return;
            if (e.Key == Key.W)
            {
                _moveDown = true;
            }
            if (e.Key == Key.S)
            {
                _moveUp = true;
            }
            if (e.Key == Key.A)
            {
                _moveRight = true;
            }
            if (e.Key == Key.D)
            {
                _moveLeft = true;
            }
        }
        private void GameEngine_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
            {

                _moveDown = false;
            }
            if (e.Key == Key.S)
            {

                _moveUp = false;
            }
            if (e.Key == Key.A)
            {
                _moveRight = false;
            }
            if (e.Key == Key.D)
            {
                _moveLeft = false;
            }
        }


        //-------------------OPEN_SCENE-----------------------
        //----------------------------------------------------
        private void OpenSettings_Click(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && inProfile == false && inStatistic == false && inFishing == false && inBattle == false && inShop == false && inSettings == false && inRules == false)
            {
                ToggleCursorVisibility(true);
                gameMenu.ToggleVisibility();
                if (!inMenu) { inMenu = true; uiManager.ToggleBlurEffect(true); }
                else { 
                    inMenu = false; 
                    uiManager.ToggleBlurEffect(false);

                    uiCreator.taskBorder.Visibility = Visibility.Visible;
                    uiManager.MoveImage("Coin", 100, 20);
                    uiManager.ToggleBlurEffect(false);
                    uiManager.OpacityObject(uiCreator.taskTextBlock, 1);
                    uiManager.OpacityObject(uiCreator.levelTextBlock, 1);
                    uiManager.OpacityImage("Level", 1);
                }
            }
        }
        private void EnterBattleMode()
        {
            battle.ToggleVisibility();
            inBattle = true;
            battle.BattleScene(player, activeNPC);
            Debug.WriteLine("RRRRRRRRRRRRRRRRR1111111111");
        }
        public async void OpenScene()
        {
            if (activeNPC.Name == "Pedro")
            {
                // Вiдкриваємо магазин
                uiManager.StartTransitionAnimation();
                await Task.Delay(600);
                inShop = true;
                uiCreator.taskBorder.Visibility = Visibility.Collapsed;
                uiManager.MoveImage("Coin", 140, 300);
                uiManager.OpacityObject(uiCreator.levelTextBlock, 0);
                uiManager.OpacityImage("Level", 0);
                uiManager.OpacityImage("HP", 0);
                uiManager.OpacityImage("bibbing", 0);
                uiManager.OpacityImage("Defense", 0);
                uiManager.OpacityImage("HP_NPC", 0);
                uiManager.OpacityImage("Defense_NPC", 0);
                uiManager.OpacityRectangle("DEF_NPC", 0);
                uiManager.OpacityRectangle("HP_NPC", 0);
                uiManager.OpacityRectangle("DEF", 0);
                uiManager.OpacityRectangle("HP", 0);
                uiManager.OpacityRectangle("BIB", 0);
                uiManager.OpacityObject(uiCreator.level_NPCTextBlock, 0);
                uiManager.OpacityImage("Level_NPC", 0);
                shop.ToggleVisibility();
                Debug.WriteLine("Money do - " + player.Money);
            }
            else
            {
                // Входження в режим бою
                uiManager.StartTransitionAnimation();
                await Task.Delay(600);
                ToggleCursorVisibility(true);
                inBattle = true;
                uiCreator.taskBorder.Visibility = Visibility.Collapsed;
                uiManager.MoveImage("Coin", 370, 20);
                uiManager.OpacityImage("HP", 1);
                uiManager.OpacityImage("bibbing", 1);
                uiManager.OpacityImage("Defense", 1);
                uiManager.OpacityImage("HP_NPC", 1);
                uiManager.OpacityImage("Defense_NPC", 1);
                uiManager.OpacityRectangle("DEF_NPC", 1);
                uiManager.OpacityRectangle("HP_NPC", 1);
                uiManager.OpacityRectangle("DEF", 1);
                uiManager.OpacityRectangle("HP", 1);
                uiManager.OpacityRectangle("BIB", 1);
                uiManager.OpacityObject(uiCreator.level_NPCTextBlock, 1);
                uiManager.OpacityImage("Level_NPC", 1);
                EnterBattleMode();
            }
        }


        public async void ApplysScene()
        {
            inBattle = false;
            inShop = false;
            if (activeNPC.Name == "Pedro")
            {
                ToggleCursorVisibility(false);
                uiManager.StartTransitionAnimation();
                await Task.Delay(600);
                shop.ToggleVisibility();
                uiCreator.taskBorder.Visibility = Visibility.Visible;
                uiManager.MoveImage("Coin", 100, 20);
                uiManager.OpacityObject(uiCreator.levelTextBlock, 1);
                uiManager.OpacityImage("Level", 1);
                uiManager.OpacityImage("HP", 0);
                uiManager.OpacityImage("bibbing", 0);
                uiManager.OpacityImage("Defense", 0);
                uiManager.OpacityImage("HP_NPC", 0);
                uiManager.OpacityImage("Defense_NPC", 0);
                uiManager.OpacityRectangle("DEF_NPC", 0);
                uiManager.OpacityRectangle("HP_NPC", 0);
                uiManager.OpacityRectangle("DEF", 0);
                uiManager.OpacityRectangle("HP", 0);
                uiManager.OpacityRectangle("BIB", 0);
            }
            else
            {
                uiManager.StartTransitionAnimation();
                await Task.Delay(600);
                ToggleCursorVisibility(true);
                battle.ToggleVisibility();
                uiCreator.taskBorder.Visibility = Visibility.Visible;
                uiManager.MoveImage("Coin", 100, 20);
                uiManager.OpacityImage("HP", 0);
                uiManager.OpacityImage("bibbing", 0);
                uiManager.OpacityImage("Defense", 0);
                uiManager.OpacityImage("HP_NPC", 0);
                uiManager.OpacityImage("Defense_NPC", 0);
                uiManager.OpacityRectangle("DEF_NPC", 0);
                uiManager.OpacityRectangle("HP_NPC", 0);
                uiManager.OpacityRectangle("DEF", 0);
                uiManager.OpacityRectangle("HP", 0);
                uiManager.OpacityRectangle("BIB", 0);
                uiManager.OpacityObject(uiCreator.level_NPCTextBlock, 0);
                uiManager.OpacityImage("Level_NPC", 0);
            }
        }

        private void OpenDialog()
        {
            inDialog = true;
            ToggleCursorVisibility(true);
            dialogManager.StartDialog(activeNPC);
        }

        

    }
}
