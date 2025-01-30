using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Walphor
{
    public class NPCBrain
    {
        private Random random;

        public NPCBrain()
        {
            random = new Random();
        }

        public int ComputeAction(int npcLevel, double npcAggressiveness, double npcHealth)
        {
            npcAggressiveness = MathExtensions.Clamp(npcAggressiveness, 0.0, 1.0);
            npcHealth = MathExtensions.Clamp(npcHealth, 0.0, 1.0);

            double attackProbability = Math.Min(0.1 * npcLevel + npcAggressiveness * 0.5, 1.0);
            double defenseProbability = Math.Max(0.3 - npcHealth * 0.2, 0.1);
            double specialMoveProbability = Math.Max(0.3 - attackProbability, 0.2);

            // Нормалізація ймовірностей
            double sum = attackProbability + defenseProbability + specialMoveProbability;
            attackProbability /= sum;
            defenseProbability /= sum;
            specialMoveProbability /= sum;

            // Генеруємо випадкове число від 0 до 1
            double randomValue = random.NextDouble();

            if (randomValue < defenseProbability)
            {
                return 0; // Захист
            }
            else if (randomValue < defenseProbability + attackProbability)
            {
                return 1; // Атака
            }
            else
            {
                return 2; // Спеціальна дія
            }
        }
        
        public static class MathExtensions
        {
            public static double Clamp(double value, double min, double max)
            {
                return Math.Max(min, Math.Min(max, value));
            }
        }
    }
}
