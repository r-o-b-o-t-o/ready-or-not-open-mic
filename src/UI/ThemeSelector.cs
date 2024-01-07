using Microsoft.UI.Xaml;
using System;

namespace ReadyOrNotOpenMic.UI
{
    public class ThemeSelector
    {
        public ElementTheme Theme
        {
            get { return theme; }
            set
            {
                theme = value;
                SetRequestedTheme();
                SaveTheme(value);
            }
        }

        private ElementTheme theme;

        public void Initialize()
        {
            Theme = LoadTheme();
        }

        public void SetRequestedTheme()
        {
            if (App.MainWindow.Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = Theme;
                TitleBarHelper.UpdateTitleBar(Theme);
            }
        }

        private static ElementTheme LoadTheme()
        {
            string themeName = App.Config.Settings.Theme;

            if (Enum.TryParse(themeName, out ElementTheme theme))
            {
                return theme;
            }

            return ElementTheme.Default;
        }

        private static void SaveTheme(ElementTheme theme)
        {
            App.Config.Settings.Theme = theme.ToString();
        }
    }
}
