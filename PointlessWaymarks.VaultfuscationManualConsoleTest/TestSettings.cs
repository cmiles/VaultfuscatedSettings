using PointlessWaymarks.VaultfuscationTools;

namespace PointlessWaymarks.VaultfuscationManualConsoleTest;

public class TestSettings : ISettingsFileType
{
    public const string SettingsTypeIdentifier = nameof(TestSettings);
    public string BackupDirectory { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int NumberOfDays { get; set; }
    public string Password { get; set; } = string.Empty;
    public string SettingsType { get; set; } = SettingsTypeIdentifier;
    
    public override string ToString()
    {
        return $"Email: {Email}, Password: {Password}, Backup Directory: {BackupDirectory}, Days Back: {NumberOfDays}";
    }
}