using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;

namespace ReadyOrNotOpenMic.UI
{
    [ObservableObject]
    public sealed partial class MainPage : Page
    {
        public static MainPage Instance { get; private set; }

        [ObservableProperty] private NavigationViewItem navigationSelectedItem;
        [ObservableProperty] private bool isBackEnabled;
        [ObservableProperty] private bool isNotificationOpen;
        [ObservableProperty] private string notificationTitle;
        [ObservableProperty] private string notificationBody;

        public Frame MainFrame => FrameContent;
        public FrameworkElement TitleBar => AppTitleBarText;

        public MainPage()
        {
            Instance = this;
            InitializeComponent();

            App.MainWindow.ExtendsContentIntoTitleBar = true;
            App.MainWindow.SetTitleBar(AppTitleBar);
            App.MainWindow.Activated += MainWindow_Activated;
        }

        public void ShowNotification(string title, string body, char icon)
        {
            NotificationTitle = title;
            NotificationBody = body;
            Notification.IconSource = new SymbolIconSource()
            {
                Symbol = (Symbol)icon,
            };
            IsNotificationOpen = true;
        }

        public void ShowErrorNotification(string text)
        {
            ShowNotification("Error", text, '\uEA39');
        }

        public void ShowSuccessNotification(string text)
        {
            ShowNotification("Success", text, '\uE73E');
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TitleBarHelper.UpdateTitleBar(RequestedTheme);
            NavView.PaneDisplayMode = NavigationViewPaneDisplayMode.Auto;
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            string resource = args.WindowActivationState == WindowActivationState.Deactivated ? "WindowCaptionForegroundDisabled" : "WindowCaptionForeground";
            AppTitleBarText.Foreground = (SolidColorBrush)Resources[resource];
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            FrameContent.Navigated += On_Navigated;

            // NavView doesn't load any page by default, so load the first item
            NavView.SelectedItem = NavView.MenuItems[0];
            NavView_Navigate(typeof(HomePage), new EntranceNavigationTransitionInfo());
        }

        private void NavView_Unloaded(object sender, RoutedEventArgs e)
        {
            FrameContent.Navigated -= On_Navigated;
        }

        private void NavView_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
        {
            AppTitleBar.Margin = new Thickness()
            {
                Left = sender.CompactPaneLength * (sender.DisplayMode == NavigationViewDisplayMode.Minimal ? 2 : 1),
                Top = AppTitleBar.Margin.Top,
                Right = AppTitleBar.Margin.Right,
                Bottom = AppTitleBar.Margin.Bottom
            };
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                NavView_Navigate(typeof(SettingsPage), args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null)
            {
                Type navPageType = Type.GetType(args.InvokedItemContainer.Tag.ToString());
                NavView_Navigate(navPageType, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavView_Navigate(Type navPageType, NavigationTransitionInfo transitionInfo)
        {
            // Get the page type before navigation to prevent duplicate
            // entries in the backstack.
            Type preNavPageType = FrameContent.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded
            if (navPageType is not null && !Type.Equals(preNavPageType, navPageType))
            {
                FrameContent.Navigate(navPageType, null, transitionInfo);
            }
        }

        private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            TryGoBack();
        }

        private bool TryGoBack()
        {
            if (!FrameContent.CanGoBack)
            {
                return false;
            }

            // Don't go back if the nav pane is overlayed.
            if (NavView.IsPaneOpen &&
                (NavView.DisplayMode == NavigationViewDisplayMode.Compact || NavView.DisplayMode == NavigationViewDisplayMode.Minimal))
            {
                return false;
            }

            FrameContent.GoBack();
            return true;
        }

        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            NavView.IsBackEnabled = FrameContent.CanGoBack;

            if (FrameContent.SourcePageType == typeof(SettingsPage))
            {
                // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
                NavView.SelectedItem = (NavigationViewItem)NavView.SettingsItem;
            }
            else if (FrameContent.SourcePageType != null)
            {
                // Select the nav view item that corresponds to the page being navigated to.
                NavView.SelectedItem = NavView.MenuItems
                    .OfType<NavigationViewItem>()
                    .FirstOrDefault(i => i.Tag.Equals(FrameContent.SourcePageType.FullName.ToString()));
            }

            NavigationSelectedItem = (NavigationViewItem)NavView.SelectedItem;
        }

        private void Notification_Closed(TeachingTip sender, TeachingTipClosedEventArgs args)
        {
            IsNotificationOpen = false;
        }
    }
}
