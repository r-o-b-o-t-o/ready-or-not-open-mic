using System;
using ReadyOrNotOpenMic.Engine.Win32;
using Windows.System;

namespace ReadyOrNotOpenMic.Engine
{
    public class Keyboard
    {
        public static void SendText(IntPtr window, string text)
        {
            foreach (char ch in text)
            {
                SendCharacter(window, ch);
            }
        }

        public static void SendCharacter(IntPtr window, char ch)
        {
            Input.SendCharacter(window, ch);
        }

        public static void SendKeyDown(IntPtr hWnd, VirtualKey key)
        {
            Input.SendKeyDown(hWnd, key);
        }

        public static void SendKeyUp(IntPtr hWnd, VirtualKey key)
        {
            Input.SendKeyUp(hWnd, key);
        }

        public static void SendKey(IntPtr hWnd, VirtualKey key)
        {
            SendKeyDown(hWnd, key);
            SendKeyUp(hWnd, key);
        }
    }
}
