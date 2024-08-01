using System.Text.Json;
using PointlessWaymarks.CommonTools;

namespace PointlessWaymarks.VaultfuscatedTextEditor;

public static class VaultfuscatedTextEditorGuiSettingTools
{
    public static VaultfuscatedTextEditorGuiSettings ReadSettings()
    {
        var settingsFileName = Path.Combine(FileLocationTools.DefaultStorageDirectory().FullName,
            "PwPowerShellRunnerSettings.json");
        var settingsFile = new FileInfo(settingsFileName);

        if (!settingsFile.Exists)
        {
            File.WriteAllText(settingsFile.FullName, JsonSerializer.Serialize(new VaultfuscatedTextEditorGuiSettings()));

            return new VaultfuscatedTextEditorGuiSettings();
        }

        return JsonSerializer.Deserialize<VaultfuscatedTextEditorGuiSettings>(
                   FileAndFolderTools.ReadAllText(settingsFileName)) ??
               new VaultfuscatedTextEditorGuiSettings();
    }

    public static async Task WriteSettings(VaultfuscatedTextEditorGuiSettings settings)
    {
        var settingsFileName = Path.Combine(FileLocationTools.DefaultStorageDirectory().FullName,
            "PwPowerShellRunnerSettings.json");
        var settingsFile = new FileInfo(settingsFileName);

        if (settingsFile.Exists) settingsFile.Delete();

        await using var stream = File.Create(settingsFile.FullName);
        await JsonSerializer.SerializeAsync(stream, settings);
    }
}