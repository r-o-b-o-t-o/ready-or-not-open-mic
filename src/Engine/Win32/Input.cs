using System;
using System.Runtime.InteropServices;
using Windows.System;

namespace ReadyOrNotOpenMic.Engine.Win32
{
    public class Input
    {
        [DllImport("USER32.dll")]
        private static extern short GetKeyState(int nVirtKey);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        private const uint WM_KEYDOWN = 0x100;
        private const uint WM_KEYUP = 0x101;
        private const uint WM_CHAR = 0x102;

        public const int VK_LBUTTON = 0x01;
        public const int VK_RBUTTON = 0x02;
        public const int VK_MBUTTON = 0x04;
        public const int VK_XBUTTON1 = 0x05;
        public const int VK_XBUTTON2 = 0x06;

        public static bool IsButtonDown(int button)
        {
            return (GetKeyState(button) & 0x8000) != 0;
        }

        public static void SendKeyDown(IntPtr hWnd, VirtualKey key)
        {
            uint scanCode = (uint)key;
            uint repeatCount = 0;
            uint extended = 0;
            uint context = 0;
            uint previousState = 0;
            uint transition = 0;

            uint lParamDown;
            lParamDown = repeatCount
                | (scanCode << 16)
                | (extended << 24)
                | (context << 29)
                | (previousState << 30)
                | (transition << 31);
            PostMessage(hWnd, WM_KEYDOWN, (IntPtr)key, unchecked((IntPtr)(int)lParamDown));
        }

        public static void SendKeyUp(IntPtr hWnd, VirtualKey key)
        {
            uint scanCode = (uint)key;
            uint repeatCount = 0;
            uint extended = 0;
            uint context = 0;
            uint previousState = 1;
            uint transition = 1;

            uint lParamUp;
            lParamUp = repeatCount
                | (scanCode << 16)
                | (extended << 24)
                | (context << 29)
                | (previousState << 30)
                | (transition << 31);
            PostMessage(hWnd, WM_KEYUP, (IntPtr)key, unchecked((IntPtr)(int)lParamUp));
        }

        public static void SendCharacter(IntPtr hWnd, char ch)
        {
            PostMessage(hWnd, WM_CHAR, (IntPtr)ch, (IntPtr)0);
        }
    }
}
