using Metalama.Patterns.Observability;
using PointlessWaymarks.AvaloniaToolkit.AppToast;
using PointlessWaymarks.AvaloniaToolkit.ProgramUpdateMessage;
using PointlessWaymarks.AvaloniaToolkit.StatusLayer;
using PointlessWaymarks.CommonTools;
using PointlessWaymarks.VaultfuscatedTextEditor.Controls;
using Serilog;

namespace PointlessWaymarks.VaultfuscatedTextEditor;

[Observable]
public partial class MainWindowContext
{
    public MainWindowContext()
    {
        var versionInfo =
            ProgramInfoTools.StandardAppInformationString(AppContext.BaseDirectory,
                "Pointless Waymarks Vaultfuscated Editor Beta");

        InfoTitle = versionInfo.humanTitleString;

        var currentDateVersion = versionInfo.dateVersion;

        StatusContext = new StatusLayerContext(new AppToastContext([]));

        UpdateMessageContext = new ProgramUpdateMessageContext();

        StatusContext.RunFireAndForgetBlockingTask(async () => { await CheckForProgramUpdate(currentDateVersion); });

        StatusContext.RunFireAndForgetBlockingTask(Load);
    }

    public ProgramUpdateMessageContext UpdateMessageContext { get; set; }

    public string InfoTitle { get; set; }

    public ObfuscatedDocumentContext? DocumentContext { get; set; }

    public StatusLayerContext StatusContext { get; set; }

    public async Task CheckForProgramUpdate(string currentDateVersion)
    {
        var settings = VaultfuscatedTextEditorGuiSettingTools.ReadSettings();

        Log.Information(
            $"Program Update Check - Current Version {currentDateVersion}, Installer Directory {settings.ProgramUpdateDirectory}");

        if (string.IsNullOrEmpty(currentDateVersion)) return;

        var (dateString, setupFile) = ProgramInfoTools.LatestInstaller(
            settings.ProgramUpdateDirectory,
            "PointlessWaymarksVaultfuscatedTextEditorSetup");

        Log.Information(
            $"Program Update Check - Current Version {currentDateVersion}, Installer Directory {settings.ProgramUpdateDirectory}, Installer Date Found {dateString ?? string.Empty}, Setup File Found {setupFile?.FullName ?? string.Empty}");

        if (string.IsNullOrWhiteSpace(dateString) || setupFile is not { Exists: true }) return;

        if (string.Compare(currentDateVersion, dateString, StringComparison.OrdinalIgnoreCase) >= 0) return;

        await UpdateMessageContext.LoadData(currentDateVersion, dateString, setupFile);
    }

    private async Task Load()
    {
        DocumentContext = await ObfuscatedDocumentContext.CreateInstance();
    }
}