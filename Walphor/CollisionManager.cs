using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;

namespace Walphor
{
    internal class CollisionManager
    {
        private Canvas canvas;
        private UIManager uiManager;
        private GameEngine engine;

        private ButtonManager wallButton;

        public CollisionManager(Canvas canvas, ButtonManager wallButton, UIManager uIManager, GameEngine gameEngine)
        {
            this.canvas = canvas;
            this.wallButton = wallButton;
            this.uiManager = uIManager;
            engine = gameEngine;
        }
        public double MoveSlow(Character player, List<Wall> wallsslow)
        {
            bool isSlowed = false;
            double slowedSpeed = 4;
            Rect playerRect = new Rect(player.X, player.Y + 60, player.Width, player.Height - 60);

            foreach (var wall in wallsslow)
            {
                Rect wallRect = new Rect(wall.X, wall.Y, wall.Width, wall.Height);
                //Debug.WriteLine($"Wall Rect: {wallRect}, Player Rect: {playerRect}");
                if (playerRect.IntersectsWith(wallRect))
                {
                    isSlowed = true;
                    //Debug.WriteLine("true");
                    break;
                }
            }
            if (isSlowed)
            {
                slowedSpeed = 1.0;
            }

            //Debug.WriteLine($"isSlowed: {isSlowed}, Speed: {slowedSpeed}");
            return slowedSpeed;
        }
        public (bool up, bool down, bool left, bool right) CheckCollisions(Character player, List<NPC> listOfNPCs, List<Wall> listOfWalls, List<ItemManager> listOfItems_home, List<ItemManager> listOfitems_tree)
        {
            var collisions = (up: false, down: false, left: false, right: false);
            Rect playerRect = new Rect(player.X, player.Y + 60, player.Width, player.Height - 60);

            foreach (var npc in listOfNPCs)
            {
                Rect npcRect = new Rect(npc.X, npc.Y + 60, npc.Width, npc.Height - 60);

                if (playerRect.IntersectsWith(npcRect))
                {
                    Rect intersection = Rect.Intersect(playerRect, npcRect);

                    // Визначення "сили" зiткнення по кожному напрямку
                    double leftRightForce = intersection.Width;
                    double upDownForce = intersection.Height;

                    // Визначення, в якому напрямку зiткнення сильнiше
                    if (leftRightForce < upDownForce)
                    {
                        // Зiткнення сильнiше горизонтально
                        if (player.X < npc.X) collisions.right = true;
                        else collisions.left = true;
                    }
                    else
                    {
                        // Зiткнення сильнiше вертикально
                        if (player.Y + 60 < npc.Y + 60) collisions.down = true;
                        else collisions.up = true;
                    }
                    break;
                }
            }

            foreach (var wall in listOfWalls)
            {
                Rect wallRect = new Rect(wall.X, wall.Y, wall.Width, wall.Height);
                if (playerRect.IntersectsWith(wallRect))
                {
                    Rect intersection = Rect.Intersect(playerRect, wallRect);

                    double leftRightForce = intersection.Width;
                    double upDownForce = intersection.Height;

                    if (leftRightForce < upDownForce)
                    {
                        if (player.X < wall.X) collisions.right = true;
                        else collisions.left = true;
                    }
                    else
                    {
                        if (player.Y < wall.Y) collisions.down = true;
                        else collisions.up = true;
                    }
                }
            }

            foreach (var item in listOfItems_home)
            {
                Rect itemRect = new Rect(item.X - 4, item.Y + 60, item.Width + 8, item.Height - 60);
                if (playerRect.IntersectsWith(itemRect))
                {
                    Rect intersection = Rect.Intersect(playerRect, itemRect);

                    double leftRightForce = intersection.Width;
                    double upDownForce = intersection.Height;

                    if (leftRightForce < upDownForce)
                    {
                        if (player.X < item.X - 4) collisions.right = true;
                        else collisions.left = true;
                    }
                    else
                    {
                        if (player.Y + 60 < item.Y + 60) collisions.down = true;
                        else collisions.up = true;
                    }
                }
            }

            foreach (var item in listOfitems_tree)
            {
                Rect treeRect = new Rect(item.X + 150, item.Y + 320, item.Width - 270, item.Height - 340);
                if (playerRect.IntersectsWith(treeRect))
                {
                    Rect intersection = Rect.Intersect(playerRect, treeRect);

                    double leftRightForce = intersection.Width;
                    double upDownForce = intersection.Height;

                    if (leftRightForce < upDownForce)
                    {
                        if (player.X < item.X + 150) collisions.right = true;
                        else collisions.left = true;
                    }
                    else
                    {
                        if (player.Y + 60 < item.Y + 320) collisions.down = true;
                        else collisions.up = true;
                    }
                }
            }

            return collisions;
        }

        public NPC ButtonColision(Character player, List<NPC> listOfNPCs, Canvas canvasGame)
        {
            double r = 100;
            NPC nPC = null;

            foreach (var npc in listOfNPCs)
            {
                double dx = player.X - npc.X;
                double dy = player.Y - npc.Y;
                double distance = Math.Sqrt(dx * dx + dy * dy);
                if (distance <= r)
                {
                    if(engine.inShop == false)
                    {
                        uiManager.UpdateHelp("Взаэмодiя з нпс:", "( E )", true);
                    }
                    nPC = npc;
                }
            }
            if (nPC == null && engine.help == false || engine.inShop == true || engine.inBattle == true)
            {
                uiManager.UpdateHelp("", "", false);
            }
            return nPC;
        }
        public ItemManager ItemColision(Character player, List<ItemManager> listOfItems, Canvas canvasGame)
        {
            double r = 1000;
            double closestDistance = double.MaxValue;
            ItemManager closestItem = null;

            foreach (var item in listOfItems)
            {
                double dx = (player.X) - (item.X / 1.0);
                double dy = (player.Y - 60) - (item.Y / 1.0);
                double distance = Math.Sqrt(dx * dx + dy * dy);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestItem = item;
                }
            }
            return closestDistance <= r ? closestItem : null;
        }
        public bool CheckProximity(Character player, List<Wall> walls)
        {
            Rect playerRect = new Rect(player.X, player.Y, player.Width, player.Height);
            foreach (var wall in walls)
            {
                Rect wallRect = new Rect(wall.X - 50, wall.Y - 50, wall.Width + 100, wall.Height + 100);
                if (playerRect.IntersectsWith(wallRect))
                {
                    uiManager.UpdateHelp("Рибалка:", "( E )", true);
                    return true;
                }
            }
            return false;
        }
    }
}
