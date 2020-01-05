using System;
using System.Collections.Generic;
using RAGE;
using RAGE.Elements;
using RAGE.Game;
using RageServer.Inputs;

namespace RageServer
{
    public class ClientTest : Events.Script
    {
        public static bool isPlayerAuthorized = false, 
                           isPlayerInBrowser = false,
                           isPlayerSideCT = false,
                           isPlayerInChat = false;
        int cam, money;
        //private static uint var = 0;
        public ClientTest() // Constructor
        {
            Key.bind(KeyCodes.B, null, onBuyMenu);
            Key.bind(KeyCodes.T, null, onChat);
            Key.bind(KeyCodes.Enter, null, onChatExit);
            Browser.open("");
            Chat.Colors = true;
            //Events.Tick += onPlayerUpdate;
            Events.OnPlayerChat += onPlayerChat;
            Events.Add("cl_TestEvent", onTestEvent);
            Events.Add("cef_exit", onCefExit);
            Events.Add("cef_buyWeapon", onCefBuyWeapon);
            Events.Add("cef_authorized", onCefAuthorized);
            Events.Add("cef_play", onPlayerSide);
            Events.Add("cef_roundFinished", onRoundFinished);
            Events.Add("cl_roundStart", onRoundStart);
            Events.Add("cl_goodGame", goodGame);
            Events.Add("cl_mainMenuCamera", onMainMenuCamera);
            Events.Add("cl_roundMoney", onRoundMoney);
            Nametags.Enabled = false;
        }
        private void onRoundMoney(object[] args)
        {
            int cash = (int)args[0];
            money += cash;
        }
        private void onMainMenuCamera(object[] args)
        {
            //Cam.DestroyAllCams(true);
            Vector3 forwardPosition = RAGE.Elements.Player.LocalPlayer.Position + RAGE.Elements.Player.LocalPlayer.GetForwardVector();
            cam = Cam.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", forwardPosition.X, forwardPosition.Y, forwardPosition.Z + 0.6f, 0, 0, 0, 70f, false, 2);
            Cam.PointCamAtEntity(cam, RAGE.Elements.Player.LocalPlayer.Handle, 0, 0, 0.25f, true);
            Cam.SetCamActive(cam, true);
            Cam.RenderScriptCams(true, false, 0, false, false, 0);
        }
        private void goodGame(object[] args)
        {
            isPlayerAuthorized = false;
            isPlayerInBrowser = false;
            isPlayerSideCT = false;
            isPlayerInChat = false;
            Browser.open("");
        }
        private void onRoundStart(object[] args)
        {
            if (!isPlayerAuthorized) return;
            var round = args[0];
            Browser.timer(round);
            Browser.cash(money);
            Ui.DisplayAreaName(true);
            Ui.DisplayCash(false);
            Ui.DisplayRadar(false);
            Ui.SetHudColour(18, 114, 204, 114, 255);
            Ui.SetPauseMenuActive(false);
        }
        private void onRoundFinished(object[] args)
        {
            Browser.closeHUD();
            if (isPlayerSideCT) Events.CallRemote("srv_stopRound", "CT");
            else Events.CallRemote("srv_stopRound", "T");
        }
        /*private void onRoundTimer(object[] args)
        {
            //string time = (string)args[0];
            Graphics.DrawRect(0.5f, 0.5f, 0.5f, 0.5f, 255, 0, 0, 255, 0);
            Chat.Output("OK!");
        }
        private void onPlayerUpdate(List<Events.TickNametagData> nametags)
        {
           var++;
           if (var == 30)
           {
               Events.CallRemote("srv_sendClientMsg", "Rabotaet");
               RAGE.Game.Graphics.DrawRect(0.5f, 0.5f, 0.2f, 0.2f, 255, 0, 0, 0, 5);
               var = 0;
           }
        }*/
        private void onChatExit()
        {
            if (!isPlayerAuthorized) return;
            if (isPlayerInChat) isPlayerInChat = false;
        }
        private void onChat()
        {
            if (!isPlayerAuthorized) return;
            if (!isPlayerInChat) isPlayerInChat = true;
        }
        private void onPlayerSide(object[] args)
        {
            string side = (string)args[0];
            isPlayerSideCT = side == "T" ? false : true;
            Cam.SetCamActive(cam, false);
            //Cam.DestroyAllCams(true);
            Cam.RenderScriptCams(false, false, 0, false, false, 0);
            Cam.DestroyCam(cam, true);
            money = 800;
            if (side == "T") Events.CallRemote("srv_playerSide", "T");
            else Events.CallRemote("srv_playerSide", "CT");
        }
        private void onCefAuthorized(object[] args)
        {
            isPlayerAuthorized = true;
            Ui.SetPauseMenuActive(false);
            Events.CallRemote("srv_authorized");
        }
        private void onCefBuyWeapon(object[] args)
        {
            string weaponName = (string)args[0];
            bool good = false;
            switch (weaponName)
            {
                case "carbinerifle":
                {
                    if (money >= 3100)
                    {
                        money -= 3100;
                        good = true;
                    }
                    break;
                }
                case "assaultrifle":
                {
                    if (money >= 2700)
                    {
                        money -= 2700;
                        good = true;
                    }
                    break;
                }
            }
            if (good)
            {
                Events.CallRemote("srv_buyWeapon", weaponName);
                Browser.cash(money);
            }
            //else textmessage
        }
        public static void onBuyMenu()
        {
            if (!isPlayerAuthorized) return;
            else if (!isPlayerInBrowser && !isPlayerInChat)
            {
                if (isPlayerSideCT) Browser.open("buyMenuCT");
                else Browser.open("buyMenuT");
            }
            else return;
        }
        private void onCefExit(object[] args)
        {
            Browser.close();
        }
        private void onTestEvent(object[] args)
        {
            string message = (string)args[0];
            Chat.Output(message);
        }
        private void onPlayerChat(string text, Events.CancelEventArgs cancel)
        {
            if (!isPlayerAuthorized || isPlayerInBrowser) return;
        }
    }
}
