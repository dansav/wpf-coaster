# wpf-coaster

WPF window placement utilities - automatically persist and restore window size, position, and state across application sessions.

## Why "Coaster"?

A coaster is something you put under your glass to keep it in place. Similarly, this library keeps your WPF windows in their place! 🍺

## Compatibility

* .NET Framework 4.6+
* .NET 8.0+

## Content

* **Window Placement Persistence**: Base class (`WindowWithPositionPersistence`) that automatically saves and restores window size, position, and state (maximized/minimized) to the registry

## Installation

```powershell
Install-Package DanielsWpfCoaster
```

Or via .NET CLI:

```bash
dotnet add package DanielsWpfCoaster
```

## Usage

Simply derive your window from `WindowWithPositionPersistence` instead of `Window`:

```csharp
public partial class MainWindow : WindowWithPositionPersistence
{
    public MainWindow()
    {
        InitializeComponent();
    }
}
```

The window's size, position, and state will automatically be persisted when the window closes and restored when it opens again.

## Note

This library previously included MVVM utilities (ICommand, INotifyPropertyChanged implementations, messaging, etc.), but those have been removed as the excellent [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) now provides superior implementations of these patterns. This library now focuses solely on window placement functionality.
