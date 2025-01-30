using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Walphor
{
    internal class SaveGame
    {
        public string PlayerName { get; set; }
        public int PlayerLevel { get; set; }
        public double PlayerMoney { get; set; }
        public bool[] OccupiedPositions { get; set; }
        public List<GameObjectData> GameObjectDataList { get; set; }
        public List<CardData> CharacterCardsData { get; set; }
        public float MasterVolume { get; set; }
        public SaveGame()
        {
            GameObjectDataList = new List<GameObjectData>();
        }

        public void Save(List<NPC> npcs, List<ItemManager> itemsHome, List<ItemManager> itemsTree, List<ItemManager> mapItems, List<Wall> walls, List<Wall> wallsslow, Character player, bool[] occupiedPositions, float masterVolume)
        {
            PlayerName = player.Name;
            PlayerLevel = player.Level;
            PlayerMoney = player.Money;
            OccupiedPositions = occupiedPositions;
            MasterVolume = masterVolume;

            // Serialize NPCs
            foreach (var npc in npcs)
            {
                GameObjectDataList.Add(new GameObjectData
                {
                    X = npc.X,
                    Y = npc.Y,
                    Width = npc.Width,
                    Height = npc.Height,
                    ImageSource = npc.ImageSource,
                    Type = "NPC",
                    Name = npc.Name
                });
            }

            // Serialize Home Items
            foreach (var item in itemsHome)
            {
                GameObjectDataList.Add(new GameObjectData
                {
                    Id = item.Id,
                    X = item.X,
                    Y = item.Y,
                    Width = item.Width,
                    Height = item.Height,
                    ImageSource = item.ImageSource,
                    Type = "HomeItem"
                });
            }

            // Serialize Tree Items
            foreach (var item in itemsTree)
            {
                GameObjectDataList.Add(new GameObjectData
                {
                    Id = item.Id,
                    X = item.X,
                    Y = item.Y,
                    Width = item.Width,
                    Height = item.Height,
                    ImageSource = item.ImageSource,
                    Type = "TreeItem"
                });
            }

            // Serialize Map Items
            foreach (var item in mapItems)
            {
                GameObjectDataList.Add(new GameObjectData
                {
                    Id = item.Id,
                    X = item.X,
                    Y = item.Y,
                    Width = item.Width,
                    Height = item.Height,
                    ImageSource = item.ImageSource,
                    Type = "MapItem"
                });
            }

            // Serialize Walls
            foreach (var wall in walls)
            {
                GameObjectDataList.Add(new GameObjectData
                {
                    Id = wall.Id,
                    X = wall.X,
                    Y = wall.Y,
                    Width = wall.Width,
                    Height = wall.Height,
                    Type = "Wall"
                });
            }

            // Serialize Slow Walls
            foreach (var wall in wallsslow)
            {
                GameObjectDataList.Add(new GameObjectData
                {
                    Id = wall.Id,
                    X = wall.X,
                    Y = wall.Y,
                    Width = wall.Width,
                    Height = wall.Height,
                    Type = "WallSlow"
                });
            }

            // Serialize Character Cards
            CharacterCardsData = new List<CardData>();
            foreach (var card in Character.Character_Cards)
            {
                CharacterCardsData.Add(new CardData
                {
                    X = card.X,
                    Y = card.Y,
                    Width = card.Width,
                    Height = card.Height,
                    Name = card.Name,
                    Def = card.Def,
                    Atck = card.Atck,
                    Algo = card.Algo,
                    Health = card.Health,
                    Cost = card.Cost,
                    ImageSource = card.ImageSource,
                    InfoCard = card.InfoCard,
                    Index = card.Index
                });
            }

            // Визначення шляху до папки проекту з використанням вiдносного шляху
            string executableLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string projectDirectory = Path.GetFullPath(Path.Combine(executableLocation, @"..\..\"));

            string saveDirectory = Path.Combine(projectDirectory, "Save");
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            int fileIndex = 1;
            string filePath;
            do
            {
                filePath = Path.Combine(saveDirectory, $"{player.Name}_{fileIndex}.json");
                fileIndex++;
            } while (File.Exists(filePath));

            File.WriteAllText(filePath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static SaveGame Load(string playerName)
        {
            string executableLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string projectDirectory = Path.GetFullPath(Path.Combine(executableLocation, @"..\..\"));
            string saveDirectory = Path.Combine(projectDirectory, "Save");

            // Find the latest save file for the player
            var files = Directory.GetFiles(saveDirectory, $"{playerName}_*.json")
                                 .Select(f => new { FilePath = f, FileName = Path.GetFileNameWithoutExtension(f) })
                                 .OrderByDescending(f => int.Parse(f.FileName.Split('_').Last()))
                                 .ToList();

            if (files.Any())
            {
                string latestFilePath = files.First().FilePath;
                SaveGame loadedGame = JsonConvert.DeserializeObject<SaveGame>(File.ReadAllText(latestFilePath));

                // Відновлення стану occupiedPositions
                if (loadedGame.OccupiedPositions == null)
                {
                    loadedGame.OccupiedPositions = new bool[3];
                }

                return loadedGame;
            }
            return null;
        }
    }
    public class CardData
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Name { get; set; }
        public double Def { get; set; }
        public double Atck { get; set; }
        public double Algo { get; set; }
        public double Health { get; set; }
        public double Cost { get; set; }
        public string ImageSource { get; set; }
        public string InfoCard { get; set; }
        public int Index { get; set; }
    }
    internal class GameObjectData
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string ImageSource { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public double? Money { get; set; }
        public int? Level { get; set; }
    }

}
