using Avalonia.Platform.Storage;
using PointlessWaymarks.AvaloniaToolkit;
using PointlessWaymarks.AvaloniaToolkit.StatusLayer;
using PointlessWaymarks.CommonTools;
using PointlessWaymarks.LlamaAspects;

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

    [BlockingCommand]
    public async Task LoadWithKey()
    {
        await UiThreadSwitcher.ResumeBackgroundAsync();

        var key = await StatusContext.ShowStringEntry("Key Enter",
            "Enter the Key that will be used to encrypt/decrypt the file", string.Empty);

        if (!key.Item1) return;

        if (string.IsNullOrWhiteSpace(key.Item2))
            StatusContext.ToastError("The key can not be blank or all whitespace.");

        await UiThreadSwitcher.ResumeForegroundAsync();

        //This can also be applied for SaveFilePicker.
        var files = await MainWindow.Instance.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Pick File",
            //You can add either custom or from the built-in file types. See "Defining custom file types" on how to create a custom one.
            FileTypeFilter = new[] { FilePickerFileTypes.All },
            AllowMultiple = false
        });

        if (files.Count == 0) StatusContext.ToastError("No file selected...");
        ;

        await using var stream = await files[0].OpenReadAsync();
        using var reader = new StreamReader(stream);
        // Read all text to a string variable
        var fileContents = await reader.ReadToEndAsync();

        FileText = fileContents.Decrypt(key.Item2);
        Key = () => key.Item2;
        LoadedFileName = files[0].TryGetLocalPath();
    }

    [BlockingCommand]
    public async Task SaveFile()
    {
        if (string.IsNullOrWhiteSpace(LoadedFileName))
        {
            StatusContext.ToastError("Use Save As...");
            return;
        }

        await File.WriteAllTextAsync(LoadedFileName, FileText.Encrypt(Key()));
    }

    public static async Task<ObfuscatedDocumentContext> CreateInstance()
    {
        await UiThreadSwitcher.ResumeBackgroundAsync();

        var factoryStatusLayer = await StatusLayerContext.CreateInstance();

        return new ObfuscatedDocumentContext(factoryStatusLayer);
    }
}