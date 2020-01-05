using RAGE;
using System;
using System.Collections.Generic;

namespace RageServer.Inputs
{
    class Key : Events.Script
    {
        Key()
        {
            Events.Tick += handler;
        }
        private static KeyModel Released = new KeyModel(KeyCodes.released, null, null);
        private static KeyModel Pressed = Released;
        private static List<KeyModel> InputList = new List<KeyModel>();
        private void handler(List<Events.TickNametagData> nametags)
        {
            if (Pressed.KeyCode == KeyCodes.released) checkPressed();
            else if (!Input.IsDown((int)Pressed.KeyCode))
            {
                if (Pressed.OnRelease != null) Pressed.OnRelease.Invoke();
                Pressed = Released;
            };
        }
        private static void checkPressed()
        {
            InputList.ForEach(i =>
            {
                if (Input.IsDown((int)i.KeyCode))
                {
                    Pressed = i;
                    if (i.OnPress != null) i.OnPress.Invoke();
                    return;
                }
            });
        }
        public static void bind(KeyCodes keyCode, KeyActions onPress, KeyActions onRelease = null)
        {
            if (InputList.Exists(i => i.KeyCode == keyCode))
            {
                KeyModel Input = InputList.Find(i => i.KeyCode == keyCode);
                Input.OnPress += onPress;
                Input.OnRelease += onRelease;
            }
            else InputList.Add(new KeyModel(keyCode, onPress, onRelease));
        }
        public static void unbind(KeyCodes keyCode)
        {
            if (InputList.Exists(i => i.KeyCode == keyCode))
            {
                KeyModel Input = InputList.Find(i => i.KeyCode == keyCode);
                InputList.Remove(Input);
            }
        }
    }
}
