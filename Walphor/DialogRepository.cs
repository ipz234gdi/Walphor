using System.Collections.Generic;

namespace Walphor
{
    public class DialogRepository
    {
        private Dictionary<string, List<string>> npcDialogues = new Dictionary<string, List<string>>();

        public DialogRepository()
        {
            LoadDialogues();
        }

        private void LoadDialogues()
        {
            npcDialogues.Add("LowSkill", new List<string>
            {
                "...",
                "Ти занадто слабкий для мене.",
                "Повертайся потiм!",
            });
            npcDialogues.Add("MaxSkill", new List<string>
            {
                "...",
                "Ти вже бився зi мною.",
                "Я не збираюсь битися другий раз.",
            });

            npcDialogues.Add("Gery", new List<string>
            {
                "...",
                "Привiт!",
                "Ти новенький тут?",
                "Це досить спокiйне мiсце, але останнiм часом щось дивне вiдбувається у лiсi за селом.",
            });

            npcDialogues.Add("Miranda", new List<string>
            {
                "...",
                "Доброго дня!",
                "Чи не бачили ви випадково мого кота?",
                "Вiн втiк сьогоднi зранку i не повернувся.",
                "Вiн має червону хустинку на шиї.",
                "Ти хочеш зi мною бiй?"
            });

            npcDialogues.Add("Pedro", new List<string>
            {
                "...",
                "Ласкаво просимо до нашого бару!",
                "Якщо тобi потрiбно вiдпочити пiсля довгих пригод або потрiбна iнформацiя, ти зайшов за адресою.",
                "А ще у нас є кiлька особливих напоїв, якi можуть тобi знадобитися.",
            });

            npcDialogues.Add("Pedro_Battle", new List<string>
            {
                "...",
                "Ласкаво просимо до нашого бару!",
            });

            npcDialogues.Add("Lusia", new List<string>
            {
                "...",
                "Якщо ти шукаєш когось, хто може розповiсти тобi про iсторiю цього мiсця, ти маєш поговорити з нашим старiйшиною.",
                "Вiн знає про цi землi все!",
                "Ти хочеш зi мною бiй?"
            });

            npcDialogues.Add("Jery", new List<string>
            {
                "...",
                "Ох, ти прокинувся!",
                "Я знайшов тебе на березi пiсля того, як твiй корабель розбився.",
                "Тут, у нашому селi, є лодка, яка може допомогти тобi повернутися додому.",
                "Але вона не просто так — спочатку тобi потрiбно довести, що ти вартий її.",
                "Всiх переможеш в бою — отримаєш лодку. ",
                "I ще, я раджу тобi заглянути до Педро в бар. Можливо, вiн допоможе тобi пiдготуватись.",
                "Ти занадто слабкий для мене.",
                "Повертайся потiм!",
            });

            npcDialogues.Add("Gery_Battle", new List<string> { "Ти хочеш зi мною бiй? Я готовий, коли завгодно!" });
            npcDialogues.Add("Miranda_Battle", new List<string> { "Ти хочеш зi мною бiй? Я готовий, коли завгодно!" });
            npcDialogues.Add("Lusia_Battle", new List<string> { "Ти хочеш зi мною бiй? Я готовий, коли завгодно!" });
            npcDialogues.Add("Jery_Battle", new List<string> { "Ти хочеш зi мною бiй? Я готовий, коли завгодно!" });
        }

        public List<string> GetDialogue(string npcName)
        {
            return npcDialogues.ContainsKey(npcName) ? npcDialogues[npcName] : new List<string> { "..." };
        }
    }
}