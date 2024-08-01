using PointlessWaymarks.LlamaAspects;

namespace PointlessWaymarks.VaultfuscatedTextEditor;

[NotifyPropertyChanged]
public partial class VaultfuscatedTextEditorGuiSettings
{
    public string DatabaseFile { get; set; } = string.Empty;
    public string? LastDirectory { get; set; } = string.Empty;
    public string ProgramUpdateDirectory { get; set; } = @"M:\PointlessWaymarksPublications";
}