using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.IO;

namespace ReadyOrNotOpenMic.UI
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/Logo.ico"));

            if (MicaController.IsSupported())
            {
                SystemBackdrop = new MicaBackdrop();
            }
        }
    }
}
