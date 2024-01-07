using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using ReadyOrNotOpenMic.Data;

namespace ReadyOrNotOpenMic.UI
{
    [ObservableObject]
    public sealed partial class HomePage : Page
    {
        public Config Cfg => App.Config;
        public App App => (App)Application.Current;

        [ObservableProperty] private bool isWaitingForKeyPushToTalk;
        [ObservableProperty] private bool isWaitingForKeyToggleOpenMic;

        public HomePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (IsWaitingForKeyPushToTalk)
            {
                Cfg.Settings.PushToTalkKey = e.Key;
                IsWaitingForKeyPushToTalk = false;
                return;
            }

            if (IsWaitingForKeyToggleOpenMic)
            {
                Cfg.Settings.ToggleOpenMicKey = e.Key;
                IsWaitingForKeyToggleOpenMic = false;
                return;
            }
        }

        [RelayCommand]
        private void BindPushToTalk()
        {
            IsWaitingForKeyPushToTalk = true;
        }

        [RelayCommand]
        private void BindToggleOpenMic()
        {
            IsWaitingForKeyToggleOpenMic = true;
        }
    }
}
