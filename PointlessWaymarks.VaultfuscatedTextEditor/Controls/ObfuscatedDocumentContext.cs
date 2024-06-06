using PointlessWaymarks.AvaloniaToolkit;
using PointlessWaymarks.AvaloniaToolkit.Aspects;
using PointlessWaymarks.AvaloniaToolkit.StatusLayer;

namespace PointlessWaymarks.VaultfuscatedTextEditor.Controls;

[NotifyPropertyChanged]
[GenerateStatusCommands]
public partial class ObfuscatedDocumentContext
{
    public ObfuscatedDocumentContext(StatusLayerContext statusContext)
    {
        StatusContext = statusContext;

        BuildCommands();
    }

    public string LoadedFileName { get; set; } = string.Empty;
    public string FileText { get; set; } = string.Empty;
    public Func<string> Key { get; set; } = () => string.Empty;
    public StatusLayerContext StatusContext { get; set; }

    [BlockingCommand]
    public async Task Test(IProgress<string> progress)
    {
        await UiThreadSwitcher.ResumeBackgroundAsync();

        StatusContext.Progress("Test Test Test");
        
        await Task.Delay(3000);
    }

    [BlockingCommand]
    public async Task Test02(IProgress<string> progress)
    {
        await UiThreadSwitcher.ResumeBackgroundAsync();
        StatusContext.ToastSuccess("Happy Times?");
    }

    public static async Task<ObfuscatedDocumentContext> CreateInstance()
    {
        await UiThreadSwitcher.ResumeBackgroundAsync();

        var factoryStatusLayer = await StatusLayerContext.CreateInstance();

        return new ObfuscatedDocumentContext(factoryStatusLayer);
    }
}