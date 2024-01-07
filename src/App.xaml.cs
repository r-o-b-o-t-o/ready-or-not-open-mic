using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using ReadyOrNotOpenMic.Data;
using ReadyOrNotOpenMic.Engine;
using ReadyOrNotOpenMic.Engine.Win32;
using ReadyOrNotOpenMic.UI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Winook;
using WASDK = Microsoft.WindowsAppSDK;

namespace ReadyOrNotOpenMic
{
    [ObservableObject]
    public partial class App : Application
    {
        public static MainWindow MainWindow { get; private set; }
        public static ThemeSelector ThemeSelector { get; private set; }
        public static Config Config { get; private set; }

        public static string WinAppSdkDetails => $"Windows App SDK {WASDK.Release.Major}.{WASDK.Release.Minor}.{WASDK.Release.Patch}";

        public static string WinAppSdkRuntimeDetails
        {
            get
            {
                string details = WinAppSdkDetails;
                details += ", Windows App Runtime " + WASDK.Runtime.Version.DotQuadString;
                return details;
            }
        }

        [ObservableProperty]
        private bool micKeyPushed;
        [ObservableProperty]
        private bool hookInstalled;

        private KeyboardHook toggleHook;
        private Timer hookTimer;
        private IntPtr gameWindow;

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            Config = Config.Load();

            ThemeSelector = new ThemeSelector();
            MainWindow = new MainWindow();

            ThemeSelector.Initialize();
            MainWindow.Content = new MainPage();
            MainWindow.Activate();
            ThemeSelector.SetRequestedTheme();

            hookTimer = new Timer()
            {
                Interval = 3000,
                AutoReset = true,
            };
            hookTimer.Elapsed += (s, e) => OnHookTimerElapsed();
            hookTimer.Start();
        }

        private void OnHookTimerElapsed()
        {
            if (HookInstalled)
            {
                return;
            }

            Process p = GetGameProcess();
            if (p == null)
            {
                return;
            }

            gameWindow = Windowing.FindHandle(p.Id);
            if (gameWindow == IntPtr.Zero)
            {
                return;
            }

            MainWindow.DispatcherQueue.EnqueueAsync(async () =>
            {
                toggleHook = new KeyboardHook(p.Id);
                toggleHook.MessageReceived += KeyboardHook_MessageReceived;
                await toggleHook.InstallAsync();
                HookInstalled = true;

                p.Exited += (s, e) =>
                {
                    HookInstalled = false;
                    MicKeyPushed = false;
                };
            });
        }

        private static Process GetGameProcess()
        {
            return Process.GetProcessesByName("ReadyOrNot-Win64-Shipping").FirstOrDefault();
        }

        private void KeyboardHook_MessageReceived(object sender, KeyboardMessageEventArgs e)
        {
            if (e.KeyValue == (ushort)Config.Settings.ToggleOpenMicKey && e.Direction == KeyDirection.Down)
            {
                MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    if (!MicKeyPushed)
                    {
                        Keyboard.SendKeyDown(gameWindow, Config.Settings.PushToTalkKey);
                    }
                    else
                    {
                        Keyboard.SendKeyUp(gameWindow, Config.Settings.PushToTalkKey);
                    }
                    MicKeyPushed = !MicKeyPushed;
                });
            }
        }
    }
}
