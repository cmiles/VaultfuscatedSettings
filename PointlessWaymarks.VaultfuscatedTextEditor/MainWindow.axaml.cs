using System.ComponentModel;
using Avalonia.Controls;
using PointlessWaymarks.AvaloniaToolkit.AppToast;
using PointlessWaymarks.AvaloniaToolkit.StatusLayer;
using PointlessWaymarks.VaultfuscatedTextEditor.Controls;

namespace PointlessWaymarks.VaultfuscatedTextEditor;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private ObfuscatedDocumentContext? _documentContext;

    private StatusLayerContext _statusContext;

    public MainWindow()
    {
        InitializeComponent();

        StatusContext = new StatusLayerContext(new AppToastContext([]));

        DataContext = this;

        StatusContext.RunFireAndForgetBlockingTask(Load);
    }

    public ObfuscatedDocumentContext? DocumentContext
    {
        get => _documentContext;
        set
        {
            if (_documentContext != value)
            {
                _documentContext = value;
                OnPropertyChanged(nameof(DocumentContext));
            }
        }
    }

    public StatusLayerContext StatusContext
    {
        get => _statusContext;
        set
        {
            if (_statusContext != value)
            {
                _statusContext = value;
                OnPropertyChanged(nameof(StatusContext));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private async Task Load()
    {
        DocumentContext = await ObfuscatedDocumentContext.CreateInstance();
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}