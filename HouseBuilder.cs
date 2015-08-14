using System;
using CodeHatch.Blocks.Networking.Events;
using CodeHatch.Blocks.Networking.Events.Local;
using CodeHatch.Common;
using CodeHatch.Engine.Networking;
using CodeHatch.Networking.Events;
using CodeHatch.Networking.Events.Entities;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("HouseBuilder", "Scorpyon", "1.0.0")]
    public class HouseBuilder : ReignOfKingsPlugin
    {
        // Create a standard 8x10 House
        [ChatCommand("buildhouse")]
        private void BuildMyFirstHouse(Player player, string cmd, string[] input)
        {
            if (!player.HasPermission("admin"))
            {
                PrintToChat(player, "For now, only admins can check locations.");
                return;
            }
            if (input.Length < 3)
            {
                PrintToChat(player, "Dont forget to include a width, length and height!");
                return;   
            }
            int width;
            bool widthOk = Int32.TryParse(input[0], out width);
            if (!widthOk)
            {
                PrintToChat(player, "The width was bad!");
                return;
            }
            int length;
            bool lengthOk = Int32.TryParse(input[1], out length);
            if (!lengthOk)
            {
                PrintToChat(player, "The length was bad!");
                return;
            }
            int height;
            bool heightOk = Int32.TryParse(input[2], out height);
            if (!heightOk)
            {
                PrintToChat(player, "The height was bad!");
                return;
            }
            if (width < 4 || length < 4 || height < 3)
            {
                PrintToChat(player, "The width and length MUST be at least 4 blocks and the height must be at least 3!");
                return;
            }

            BuildARoom(player, width, length, height);
        }

        private void BuildARoom(Player player, int width, int length, int height)
        {
            var startPosition = player.Entity.Position;
            // material
            // 0 - (Air)
            // 1 - Cobblestone
            // 2 - Stone
            // 3 - Clay
            // 4 - Dirt/grass
            // 5 - Thatch
            // 6 - Sticks
            // 7 - Wood
            // 8 - Logs
            // 9 - Reinforced Wood
            byte material = 1; // Cobblestone

            // Floor
            CreateNewPlatform(player, startPosition, width, length, material);
            // North Wall
            var wallStartPositon = new Vector3(startPosition.x, startPosition.y, startPosition.z);
            CreateLeftWall(player, wallStartPositon, width, length, height, material);
            ////South Wall
            CreateRightWall(player, wallStartPositon, width, length, height, material);
            //// East Wall
            CreateBackWall(player, wallStartPositon, width, length, height, material);
            //// West Wall
            //wallStartPositon = new Vector3(startPosition.x + (width / 2), startPosition.y, startPosition.z - (length / 2) + 1);
            //CreateLengthWallWithDoor(player, wallStartPositon, width, height, material);
            // Ceiling
            var roofStartPositon = new Vector3(startPosition.x, startPosition.y + height + 2, startPosition.z);
            CreateNewPlatform(player, roofStartPositon, width, length, material);
        }

        private void CreateBackWall(Player player, Vector3 startPosition, int width, int length, int height, byte material)
        {
            Quaternion rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
            byte prefabId = 0;
            Vector3Int playerPos = (Vector3Int)startPosition;
            var teleEvent = new TeleportEvent(player.Entity, playerPos);
            EventManager.CallEvent((BaseEvent)teleEvent);

            int setX = playerPos.x - (width / 2);
            setX = (int)(setX - (double)(setX / 6));
            int setY = playerPos.y;
            setY = (int)(setY - (double)(setY / 6));
            int setZ = playerPos.z - (length / 2);
            setZ = (int)(setZ - (double)(setZ / 6));

            var originZ = setZ;

            for (var i = 0; i < height; i++)
            {
                for (var ii = 0; ii < width; ii++)
                {
                    BuildBlockOnThisSquare(setX, setY, setZ, material, rotation, prefabId);
                    setZ++;
                }
                setZ = originZ;
                setY++;
            }

            // Tele the player over the platform (to prevent getting stuck
            EventManager.CallEvent((BaseEvent)new TeleportEvent(player.Entity, new Vector3(startPosition.x, startPosition.y + 1, startPosition.z)));
        }

        private void CreateLeftWall(Player player, Vector3 startPosition, int width, int length, int height, byte material)
        {
            Quaternion rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
            byte prefabId = 0;
            Vector3Int playerPos = (Vector3Int)startPosition;
            var teleEvent = new TeleportEvent(player.Entity, playerPos);
            EventManager.CallEvent((BaseEvent)teleEvent);

            int setX = playerPos.x - (width / 2);
            setX = (int)(setX - (double)(setX / 6));
            int setY = playerPos.y;
            setY = (int)(setY - (double)(setY / 6));
            int setZ = playerPos.z - (length / 2);
            setZ = (int)(setZ - (double)(setZ / 6));

            BuildWallBlocks(length, height, material, setX, setY, setZ, rotation, prefabId);

            // Tele the player over the platform (to prevent getting stuck
            EventManager.CallEvent((BaseEvent)new TeleportEvent(player.Entity, new Vector3(startPosition.x, startPosition.y + 1, startPosition.z)));
        }

        private static void BuildWallBlocks(int length, int height, byte material, int setX, int setY, int setZ,
            Quaternion rotation, byte prefabId)
        {
            var originX = setX;

            for (var i = 0; i < height; i++)
            {
                for (var ii = 0; ii < length; ii++)
                {
                    BuildBlockOnThisSquare(setX, setY, setZ, material, rotation, prefabId);
                    setX++;
                }
                setX = originX;
                setY++;
            }
        }

        private void CreateRightWall(Player player, Vector3 startPosition, int width, int length, int height, byte material)
        {
            Quaternion rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
            byte prefabId = 0;
            Vector3Int playerPos = (Vector3Int)startPosition;
            var teleEvent = new TeleportEvent(player.Entity, playerPos);
            EventManager.CallEvent((BaseEvent)teleEvent);

            int setX = playerPos.x - (width / 2);
            setX = (int)(setX - (double)(setX / 6));
            int setY = playerPos.y;
            setY = (int)(setY - (double)(setY / 6));
            int setZ = playerPos.z + (length / 2) - 1;
            setZ = (int)(setZ - (double)(setZ / 6));

            BuildWallBlocks(length, height, material, setX, setY, setZ, rotation, prefabId);

            // Tele the player over the platform (to prevent getting stuck
            EventManager.CallEvent((BaseEvent)new TeleportEvent(player.Entity, new Vector3(startPosition.x, startPosition.y + 1, startPosition.z)));

        }


        void CreateNewPlatform(Player player, Vector3 startPosition, int width, int length,byte material)
        {
            Quaternion rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
            byte prefabId = 0;

            Vector3Int playerPos = (Vector3Int)startPosition;
            var teleEvent = new TeleportEvent(player.Entity, playerPos);
            EventManager.CallEvent((BaseEvent)teleEvent);

            int setX = playerPos.x - (width / 2);
            setX = (int)(setX - (double)(setX / 6));
            int setY = playerPos.y;
            setY = (int)(setY - (double)(setY / 6) - 1);
            int setZ = playerPos.z - (length / 2);
            setZ = (int)(setZ - (double)(setZ / 6));

            var originX = setX;

            for (var i = 0; i < width; i++)
            {
                for (var ii = 0; ii < length; ii++)
                {
                    BuildBlockOnThisSquare(setX, setY, setZ, material, rotation, prefabId);
                    setX++;
                }
                setX = originX;
                setZ++;
            }
            // Tele the player over the platform (to prevent getting stuck
            EventManager.CallEvent((BaseEvent)new TeleportEvent(player.Entity, new Vector3(startPosition.x, startPosition.y + 1, startPosition.z)));
        }

        private static void BuildBlockOnThisSquare(int setX, int setY, int setZ, byte material, Quaternion rotation, byte prefabId)
        {
            var newPosition = new Vector3Int(setX, setY, setZ);
            var cubeEvent = new CubePlaceEvent(0, newPosition, material, rotation, prefabId, 0.0f);
            var localCubeEvent = new CubePlaceLocalEvent(cubeEvent, true);
            EventManager.CallEvent((BaseEvent)cubeEvent);
            EventManager.CallEvent((BaseEvent)localCubeEvent);
        }
    }
}
