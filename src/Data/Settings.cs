using CommunityToolkit.Mvvm.ComponentModel;
using Windows.System;

namespace ReadyOrNotOpenMic.Data
{
    public partial class Settings : ObservableObject
    {
        [ObservableProperty] private string theme = "Default";
        [ObservableProperty] private VirtualKey pushToTalkKey;
        [ObservableProperty] private VirtualKey toggleOpenMicKey;
    }
}
