using GTANetworkAPI;
using System;

namespace RageServer
{
    public class ServerTest : Script
    {
        public static uint var = 0, 
                           countOfPlayers = 9, 
                           countOfAlivesCT,
                           countOfAlivesT,
                           roundsLeft = 0,
                           winCT = 0,
                           winT = 0;
        public static bool isRoundStarted = false,
                           goodGame = false;
        /*[Command("veh", "Используйте: /veh model name", GreedyArg = true)]
        public void veh(Client player, string vehicleModel)
        {
            if (!player.GetData("authorized")) return;
            VehicleHash vehicleHash = NAPI.Util.VehicleNameToModel(vehicleModel);
            Vehicle veh = NAPI.Vehicle.CreateVehicle(vehicleHash, NAPI.Entity.GetEntityPosition(player), NAPI.Entity.GetEntityRotation(player), 0, 0);
            NAPI.Player.SetPlayerIntoVehicle(player, veh, -1);
        }
        [Command("skin", "Используйте: /skin model name", GreedyArg = true)]
        public void skin(Client player, string modelName)
        {
            if (!player.GetData("authorized")) return;
            /*PedHash modelHash = NAPI.Util.PedNameToModel(modelName);
            //NAPI.Player.SetPlayerSkin(player, 0x63858A4A);
            NAPI.Player.SetPlayerSkin(player, (UInt32)modelHash);
        }
        [Command("weapon","Используйте: /weapon weapon name", GreedyArg = true)]
        public void weap(Client player, string weaponName)
        {
            if (!player.GetData("authorized")) return;
            WeaponHash weapHash = NAPI.Util.WeaponNameToModel(weaponName);
            NAPI.Player.GivePlayerWeapon(player, weapHash, 100);
        }*/
        [Command("position", "Используйте: /position", GreedyArg = true)]
        public void pos(Client player)
        {
            float x = player.Position.X,
                  y = player.Position.Y,
                  z = player.Position.Z;
            NAPI.Chat.SendChatMessageToPlayer(player, $"x: {x}, y: {y}, z: {z}");
        }
        [ServerEvent(Event.PlayerConnected)]
        public void onPlayerConnect(Client player)
        {
            NAPI.Chat.SendChatMessageToAll("!{FF4500}"+ $"{player.Name}" + "!{FFFFFF} подключился");
            //player.TriggerEvent("cl_TestEvent", "Player is connected");
        }
        [ServerEvent(Event.PlayerDisconnected)]
        public void onPlayerDisconnect(Client player)
        {
            NAPI.Chat.SendChatMessageToAll("!{FF4500}" + $"{player.Name}" + "!{FFFFFF} отключился");
            countOfPlayers--;
        }
        [RemoteEvent("srv_TestEvent")]
        public void onTestEvent(Client player, string message)
        {
            NAPI.Util.ConsoleOutput($"Test message: {message} from player: {player.Name}");
        }
        [RemoteEvent("srv_buyWeapon")]
        public void onBuyWeapon(Client player, string weaponName)
        {
            if (!player.GetData("authorized")) return;
            WeaponHash weapHash = NAPI.Util.WeaponNameToModel(weaponName);
            NAPI.Player.GivePlayerWeapon(player, weapHash, 90);
        }
        [RemoteEvent("srv_authorized")]
        public void authorized(Client player)
        {
            player.SetData("authorized", true);
            player.Position = new Vector3(-519.337f, 5341.695f, 74.10365f);
            Random rnd = new Random();
            Int32[] skins = {-1920001264, -1275859404};
            int index = rnd.Next(skins.Length);
            player.SetSkin((PedHash)skins[index]);
            WeaponHash weapHash = NAPI.Util.WeaponNameToModel("carbinerifle");
            NAPI.Player.GivePlayerWeapon(player, weapHash, 10);
            NAPI.ClientEvent.TriggerClientEvent(player, "cl_mainMenuCamera");
        }
        [RemoteEvent("srv_playerSide")]
        public void playerSide(Client player, string side)
        {
            player.SetData("isPlayerSideCT", side=="CT" ? true : false);
            if (side == "CT")
            {
                var skin = -1920001264;
                player.SetSkin((PedHash)skin);
                player.Position = new Vector3(-519.337f, 5341.695f, 74.10365f);
            }
            else
            {
                var skin = -1275859404;
                player.SetSkin((PedHash)skin);
                //NAPI.Player.SetPlayerSkin()
                player.Position = new Vector3(-527.0718f, 5292.934f, 74.23135f);
            }
            NAPI.Player.RemoveAllPlayerWeapons(player);
            countOfPlayers++;
        }
        [ServerEvent(Event.ChatMessage)]
        public void chatMsg(Client player, string msg)
        {
            if (player.GetData("isPlayerSideCT"))
                NAPI.Chat.SendChatMessageToAll("!{#0000FF}" + $"(All) {player.Name}: {msg}");
            else NAPI.Chat.SendChatMessageToAll("!{#FFCC00}" + $"(All) {player.Name}: {msg}");
        }
        [RemoteEvent("srv_sendClientMsg")]
        public void scm(Client player, string msg)
        {
            NAPI.Chat.SendChatMessageToPlayer(player, "!{#FF4500}" + $" {msg}");
        }
        [ServerEvent(Event.Update)]
        public void update()
        {
            if (countOfPlayers == 10 && !isRoundStarted && roundsLeft < 30)
            {
                isRoundStarted = true;
                startRound(roundsLeft);
            }
            /*if (var == 35)
            {
                DateTime date1 = DateTime.Now;
                Vector3 labelPos = new Vector3(200.0f,200.0f,0.0f);
                NAPI.TextLabel.CreateTextLabel($"{date1.Second}", labelPos, );
                NAPI.ClientEvent.TriggerClientEventForAll("cl_timer");//date1.Second);
                NAPI.Util.ConsoleOutput($"{date1.Second}");
                var = 0;
            }*/
        }
        [ServerEvent(Event.ResourceStart)]
        public void resourceStart()
        {
            NAPI.Server.SetGlobalServerChat(false);
            NAPI.Server.SetAutoRespawnAfterDeath(false);
            Vector3 plantAPos = new Vector3(-503.7411, 5314.64, 80.23986);
            Vector3 plantADir = new Vector3(0, 0, 0);
            Vector3 plantARot = new Vector3(0, 0, 0);
            Color plantAColor = new Color(255, 0, 0, 155);
            NAPI.Marker.CreateMarker(27, plantAPos, plantADir, plantARot, 1.0f, plantAColor);
            Vector3 plantBPos = new Vector3(-551.1024, 5331.008, 75.07009);
            Vector3 plantBDir = new Vector3(0, 0, 0);
            Vector3 plantBRot = new Vector3(0, 0, 0);
            Color plantBColor = new Color(255, 0, 0, 155);
            NAPI.Marker.CreateMarker(27, plantBPos, plantBDir, plantBRot, 1.0f, plantBColor);
        }
        [RemoteEvent("srv_startRound")]
        public void startRound(uint gameRoundsLeft)
        {
            var round = gameRoundsLeft;
            NAPI.ClientEvent.TriggerClientEventForAll("cl_roundStart", round);
            countOfAlivesT = 5;
            countOfAlivesCT = 5;
        }
        [RemoteEvent("srv_stopRound")]
        public void stopRound(Client player, string side)
        {
            isRoundStarted = false;
            if (side == "T")
            {
                player.Position = new Vector3(-527.0718f, 5292.934f, 74.23135f);
                if (player.Health <= 0) countOfAlivesT--;
            }
            else if (side == "CT")
            {
                player.Position = new Vector3(-519.337f, 5341.695f, 74.10365f);
                if (player.Health <= 0) countOfAlivesCT--;
            }
            if (countOfAlivesT > countOfAlivesCT)
            {
                NAPI.Chat.SendChatMessageToAll("!{#FF45AA} Terrorists win");
                winT++;
                if (side == "T") NAPI.ClientEvent.TriggerClientEvent(player, "cl_roundMoney", 2500);
                else NAPI.ClientEvent.TriggerClientEvent(player, "cl_roundMoney", 1300);
            }
            else
            {
                NAPI.Chat.SendChatMessageToAll("!{#FF45AA} Counter-Terrorists win");
                winCT++;
                if (side == "CT") NAPI.ClientEvent.TriggerClientEvent(player, "cl_roundMoney", 2500);
                else NAPI.ClientEvent.TriggerClientEvent(player, "cl_roundMoney", 1300);
            }
            if (winCT == 16 || winT == 16)
            {
                goodGame = true;
                if (winT != 16) NAPI.Chat.SendChatMessageToAll("!{#FF45AA} Counter-Terrorists win this game");
                else NAPI.Chat.SendChatMessageToAll("!{#FF45AA} Terrorists win this game");
            }
            if (roundsLeft < 30 && !goodGame) roundsLeft++;
            else
            {
                winT = 0;
                winCT = 0;
                goodGame = true;
                countOfPlayers = 0;
                player.Health = 100;
            }
        }
    }
}
