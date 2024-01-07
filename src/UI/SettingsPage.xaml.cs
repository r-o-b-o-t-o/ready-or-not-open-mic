using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using ReadyOrNotOpenMic.Data;
using Windows.System;
using WinRT;

namespace ReadyOrNotOpenMic.UI
{
    public sealed partial class SettingsPage : Page
    {
        public Config Cfg => App.Config;

        public string WinAppSdkRuntimeDetails => App.WinAppSdkRuntimeDetails;
        public string Version
        {
            get
            {
                Version v = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
                return $"{v.Major}.{v.Minor}.{v.Build}";
            }
        }

        public string GitHubRepository => $"https://github.com/{Application.Current.Resources["GitHubRepository"].As<string>()}";
        public string GitHubNewIssue => $"{GitHubRepository}/issues/new";

        public SettingsPage()
        {
            InitializeComponent();
            string theme = App.ThemeSelector.Theme.ToString();
            ComboTheme.SelectedItem = ComboTheme.Items.FirstOrDefault(item => ((ComboBoxItem)item).Tag as string == theme);
        }

        private async void CardRepository_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(GitHubRepository));
        }

        private async void CardNewIssue_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(GitHubNewIssue));
        }

        private void CardSettingsDir_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = Data.Config.GetSaveDirectory() + "\\",
                UseShellExecute = true,
                Verb = "open",
            });
        }

        private void ComboTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string themeName = (ComboTheme.SelectedItem as ComboBoxItem).Tag as string;
            if (!Enum.TryParse(themeName, out ElementTheme theme))
            {
                return;
            }
            App.ThemeSelector.Theme = theme;
        }
    }
}
