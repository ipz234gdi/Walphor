using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Windows.Threading;

namespace Walphor
{
    internal class Character : GameObject
    {
        public double Health
        {
            get => _health;
            set
            {
                double newHealth = Math.Min(value, MaxHealth);
                if (_health != newHealth)
                {
                    _health = newHealth;
                    Debug.WriteLine($"Health: {_health}");
                    OnHealthChanged?.Invoke(_health);
                }
            }
        }
        public double _health;
        public event Action<double> OnHealthChanged;

        public double MaxHealth { get; set; }
        public int Level
        {
            get => _level;
            set
            {
                if (_level != value)
                {
                    _level = value;
                    Debug.WriteLine($"Level: {_level}");
                    OnLevelChanged?.Invoke(_level);
                }
            }
        }
        private int _level = 0;
        public event Action<int> OnLevelChanged;
        public double Def
        {
            get => _def;
            set
            {
                double newDef = Math.Min(value, MaxDef);
                if (_def != newDef)
                {
                    _def = newDef;
                    Debug.WriteLine($"Defense: {_def}");
                    OnDefChanged?.Invoke(_def);
                }
            }
        }
        public double _def;
        public event Action<double> OnDefChanged;
        public double MaxAlgo { get; set; }
        public double Algo
        {
            get => _algo;
            set
            {
                double newAlgo = Math.Min(value, MaxAlgo);
                if (_algo != newAlgo)
                {
                    _algo = newAlgo;
                    Debug.WriteLine($"Algo: {_algo}");
                    OnAlgoChanged?.Invoke(_algo);
                }
            }
        }
        public double _algo;
        public event Action<double> OnAlgoChanged;
        public double MaxDef { get; set; }
        public double Atck { get; set; }

        private double _money;
        public double Money
        {
            get => _money;
            set
            {
                if (_money != value)
                {
                    _money = value;
                    Debug.WriteLine($"Money: {_money}");
                    OnMoneyChanged?.Invoke(_money);
                }
            }
        }
        public event Action<double> OnMoneyChanged;
        public string Name { get; set; }
        public static List<Card> Character_Cards = new List<Card>();

        private int currentFrame = 0;
        private string currentDirection = "down_idle";
        private DispatcherTimer animationTimer;

        public Character(double x, double y, double width, double height, int level, double health, double def, double atck, double algo, double money, string name, string imageSource, Canvas canvas) : base(x, y, width, height, imageSource, canvas)
        {
            MaxHealth = health;
            MaxDef = def;
            MaxAlgo = 100;
            Money = money;
            Health = health;
            Algo = algo;
            Level = level;
            Def = def;
            Atck = atck;
            Name = name;

            animationTimer = new DispatcherTimer();
            animationTimer.Interval = TimeSpan.FromMilliseconds(200); // Час мiж кадрами анiмацiї
            animationTimer.Tick += new EventHandler(UpdateAnimation);
            animationTimer.Start();
        }
        public static Character Create(double x, double y, double width, double height, int level, double health, double def, double atck, double algo, double money, string name, string imageSource, Canvas canvas)
        {
            return new Character(x, y, width, height, level, health, def, atck, algo, money, name, imageSource, canvas);
        }

        private void UpdateAnimation(object sender, EventArgs e)
        {
            currentFrame = (currentFrame + 1) % 4; // Кожне напрямлення має 4 кадри
            string imageSource = $"pack://application:,,,/Data/Character/{currentDirection}_{currentFrame + 1}.png";
            SetImageSource(imageSource);
        }

        public void ChangeDirection(string direction)
        {
            if (currentDirection != direction)
            {
                currentDirection = direction;
                currentFrame = 0; // Скидання кадру на перший у новому напрямку
            }
        }

        public void Move(double deltaX, double deltaY)
        {
            SetPosition(X - deltaX, Y - deltaY);

        }
        public override Point GetPosition()
        {
            return base.GetPosition();
        }
        public override GameObject Clone(Canvas newCanvas, double X, double Y)
        {
            return new Character(X, Y, Width, Height, Level, Health, Def, Atck, Algo, Money, Name, ImageSource, newCanvas);
        }

    }
}
