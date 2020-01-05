using RAGE;
using RAGE.Elements;

namespace RageServer
{
    class Browser
    {
        private static string url = "http://localhost/CSRageOffensive/index.html";
        public static bool isOpened = false;
        private static int blureTime = 500;
        private static RAGE.Ui.HtmlWindow Cef;
        private static RAGE.Ui.HtmlWindow CefHUD;
        public static void open(string page)
        {
            if (page == "buyMenuCT")
            {
                url = "http://localhost/CSRageOffensive/buyMenuCT.html";
                prepair(true, true);
            }
            else if (page == "buyMenuT")
            {
                url = "http://localhost/CSRageOffensive/buyMenuT.html";
                prepair(true, true);
            }
            else
            {
                url = "http://localhost/CSRageOffensive/index.html";
                prepair(true, false);
            }
        }
        public static void close()
        {
            prepair(false, false);
        }
        public static void timer(object roundsLeft)
        {
            url = "http://localhost/CSRageOffensive/timer.html";
            CefHUD = new RAGE.Ui.HtmlWindow(url);
            var round = roundsLeft;
            CefHUD.ExecuteJs($"roundsLeft('{round}')");
        }
        public static void cash(object money)
        {
            var dollars = money;
            CefHUD.ExecuteJs($"cash('{dollars}')");
        }
        public static void closeHUD()
        {
            CefHUD.Destroy();
        }
        public static void call(string func)
        {
            Cef.ExecuteJs(func);
        }
        private static void prepair(bool isOpen, bool withBlure)
        {
            isOpened = isOpen;
            Player.LocalPlayer.FreezePosition(isOpen);
            RAGE.Ui.Cursor.Visible = isOpen;
            if (isOpen)
            {
                ClientTest.isPlayerInBrowser = true;
                Chat.Activate(false);
                Cef = new RAGE.Ui.HtmlWindow(url);
                if (withBlure) RAGE.Game.Graphics.TransitionToBlurred(blureTime);
            }
            else
            {
                ClientTest.isPlayerInBrowser = false;
                Cef.Destroy();
                Chat.Activate(true);
                RAGE.Game.Graphics.TransitionFromBlurred(blureTime);
            }
        }
    }
}
