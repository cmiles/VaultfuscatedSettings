using Avalonia;
using Avalonia.Controls;

namespace PointlessWaymarks.VaultfuscatedTextEditor;

public partial class MainWindow : Window
{
    public static readonly StyledProperty<MainWindowContext?> WindowContextProperty =
        AvaloniaProperty.Register<MainWindow, MainWindowContext?>(nameof(WindowContext), null);

    public MainWindow()
    {
        InitializeComponent();

        Instance = this;

        DataContext = new MainWindowContext();
    }

    public static MainWindow? Instance { get; private set; }

    public MainWindowContext? WindowContext
    {
        get => GetValue(WindowContextProperty);
        set => SetValue(WindowContextProperty, value);
    }
}