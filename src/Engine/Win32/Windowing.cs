using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace ReadyOrNotOpenMic.Engine.Win32
{
    public class Windowing
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EnumWindows(EnumThreadWindowsCallback callback, IntPtr extraData);

        private delegate bool EnumThreadWindowsCallback(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [ResourceExposure(ResourceScope.Machine)]
        private static extern int GetWindowThreadProcessId(HandleRef handle, out int processId);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern IntPtr GetWindow(HandleRef hWnd, int uCmd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool IsWindowVisible(HandleRef hWnd);


        private const int GW_OWNER = 4;


        public static IntPtr FindHandle(int pid)
        {
            MainWindowFinder windowFinder = new();
            return windowFinder.FindMainWindow(pid);
        }

        /// <summary>
        /// This class finds the main window of a process. It needs to be
        /// class because we need to store state while searching the set
        /// of windows.
        /// </summary>
        /// https://referencesource.microsoft.com/#System/services/monitoring/system/diagnosticts/ProcessManager.cs,21c55970ba46fbb6
        private class MainWindowFinder
        {
            IntPtr bestHandle;
            int processId;

            public IntPtr FindMainWindow(int processId)
            {
                bestHandle = (IntPtr)0;
                this.processId = processId;

                EnumThreadWindowsCallback callback = new(EnumWindowsCallback);
                EnumWindows(callback, IntPtr.Zero);

                GC.KeepAlive(callback);
                return bestHandle;
            }

            bool IsMainWindow(IntPtr handle)
            {
                if (GetWindow(new HandleRef(this, handle), GW_OWNER) != (IntPtr)0 || !IsWindowVisible(new HandleRef(this, handle)))
                {
                    return false;
                }

                return true;
            }

            bool EnumWindowsCallback(IntPtr handle, IntPtr extraParameter)
            {
                _ = GetWindowThreadProcessId(new HandleRef(this, handle), out int processId);
                if (processId == this.processId && IsMainWindow(handle))
                {
                    bestHandle = handle;
                    return false;
                }
                return true;
            }
        }
    }
}
