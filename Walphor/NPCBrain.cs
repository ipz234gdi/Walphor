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

        public int ComputeAction(int npcLevel)
        {
            double probabilityOfZero = Math.Min(0.1 * npcLevel, 1.0);
            double probabilityOfOne = (1.0 - probabilityOfZero) / 2.0;
            double probabilityOfTwo = probabilityOfOne;

            // Генеруємо випадкове число вiд 0 до 1
            double randomValue = random.NextDouble();

            if (randomValue < probabilityOfZero)
            {
                return 0;
            }
            else if (randomValue < probabilityOfZero + probabilityOfOne)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
    }
}
