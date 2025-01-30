using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Diagnostics;
using System.Windows.Threading;

namespace Walphor
{
    internal class NPC : GameObject
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public double MaxHealth { get; set; }
        public double Health
        {
            get => _health;
            set
            {
                double newHealth = Math.Min(value, MaxHealth);
                if (_health != newHealth)
                {
                    _health = newHealth;
                    Debug.WriteLine($"Health NPC: {_health}");
                    OnHealthChanged_NPC?.Invoke(_health);
                }
            }
        }
        private double _health;
        public event Action<double> OnHealthChanged_NPC;

        public double MaxDef { get; set; }
        public double Def
        {
            get => _def;
            set
            {
                double newDef = Math.Min(value, MaxDef);
                if (_def != newDef)
                {
                    _def = newDef;
                    Debug.WriteLine($"Defense NPC: {_def}");
                    OnDefChanged_NPC?.Invoke(_def);
                }
            }
        }
        private double _def;
        public event Action<double> OnDefChanged_NPC;
        public double Atck { get; set; }

        private int currentFrame = 0;
        private string currentDirection = "down_idle";
        private List<string> names = new List<string> { "Pedro", "Gary", "Lusia", "Jery", "Miranda" };
        private DispatcherTimer animationTimer;

        public bool HasInteracted { get; set; } = false;
        private NPC(double x, double y, double width, double height, string imageSource, string name, int level, double health, double def, double atck, Canvas canvas)
        : base(x, y, width, height, imageSource, canvas)
        {
            Name = name;
            Level = level;
            MaxHealth = health;
            MaxDef = def;
            Health = health;
            Def = def;
            Atck = atck;
            ZPosition(1);

            animationTimer = new DispatcherTimer();
            animationTimer.Interval = TimeSpan.FromMilliseconds(250);
            animationTimer.Tick += new EventHandler(UpdateAnimation);
            animationTimer.Start();
        }
        public static NPC Create(double x, double y, double width, double height, string imageSource, string name, int level, double health, double def, double atck, Canvas canvas)
        {
            return new NPC(x, y, width, height, imageSource, name, level, health, def, atck, canvas);
        }

        private void UpdateAnimation(object sender, EventArgs e)
        {
            currentFrame = (currentFrame + 1) % 4; // Кожне напрямлення має 4 кадри
            string imageSource = $"pack://application:,,,/Data/NPC/{currentDirection}_{currentFrame + 1}_{Name}.png";
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

        public void SetSize(double width, double height)
        {
            Width = width;
            Height = height;
            image.Width = width;
            image.Height = height;
        }
        public void Move(double deltaX, double deltaY)
        {
            SetPosition(X + deltaX, Y + deltaY);
        }
        public override GameObject Clone(Canvas newCanvas, double X, double Y)
        {
            return new NPC(X, Y, Width, Height, ImageSource, Name, Level, Health, Def, Atck, newCanvas);
        }
    }
}
