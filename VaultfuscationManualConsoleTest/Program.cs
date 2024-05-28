using System.Reflection;
using Microsoft.Extensions.Logging;
using Tools;
using VaultfuscationManualConsoleTest;

var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
var testSettingsFile = Path.Combine(executingDirectory, $"TestSettings-{DateTime.Now.Ticks}.json");

using var factory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = factory.CreateLogger<ObfuscatedSettingsConsoleSetup<TestSettings>>();

Console.WriteLine("Vaultfuscation Manual Console Test - this is an interactive/manual test");
Console.WriteLine("  that will prompt you to enter some values from a fake settings file that");
Console.WriteLine("  is meant to be representative of the type of information this setup is");
Console.WriteLine("  intended to store.");
Console.WriteLine("");
Console.WriteLine("PLEASE remember that the VaultfuscatedSettings library is IN NO WAY INTENDED");
Console.WriteLine("  TO ENCRYPT OR FULLY SECURE ANY DATA - strings in memory are not secure and");
Console.WriteLine("  the key that is used to de-obfuscate a settings file is store in a");
Console.WriteLine("  LOCAL Credentials vault -> any attacker or person with full access to your");
Console.WriteLine("  user account will automatically have ALL of the pieces they need to access");
Console.WriteLine("  a settings file created with this library!!! In some cases this is valuable");
Console.WriteLine("  and meaningful protection - in some cases it is completely useless and");
Console.WriteLine("  a dangerously bad choice!");
Console.WriteLine("");
Console.WriteLine($"Executing Directory: {executingDirectory}");
Console.WriteLine($"Test Settings File: {testSettingsFile}");
Console.WriteLine("");
Console.WriteLine("Part 1 - New File Setup:");

var settingFileReadAndSetup = new ObfuscatedSettingsConsoleSetup<TestSettings>(logger)
{
    SettingsFile = testSettingsFile,
    SettingsFileIdentifier = TestSettings.SettingsTypeIdentifier,
    VaultServiceIdentifier = "http://vaultfuscationmanualconsoletest.test",
    SettingsFileProperties =
    [
        new SettingsFileProperty<TestSettings>
        {
            PropertyDisplayName = "Email",
            PropertyEntryHelp =
                "The email to use to login to the API.",
            HideEnteredValue = false,
            PropertyIsValid =
                ObfuscatedSettingsHelpers.PropertyIsValidIfNotNullOrWhiteSpace<TestSettings>(x => x.Email),
            UserEntryIsValid = ObfuscatedSettingsHelpers.UserEntryIsValidIfNotNullOrWhiteSpace(),
            SetValue = (settings, userEntry) => settings.Email = userEntry.Trim()
        },
        new SettingsFileProperty<TestSettings>
        {
            PropertyDisplayName = "Password",
            PropertyEntryHelp =
                "The password to use to login to the API.",
            HideEnteredValue = true,
            PropertyIsValid =
                ObfuscatedSettingsHelpers.PropertyIsValidIfNotNullOrWhiteSpace<TestSettings>(x => x.Password),
            UserEntryIsValid = ObfuscatedSettingsHelpers.UserEntryIsValidIfNotNullOrWhiteSpace(),
            SetValue = (settings, userEntry) => settings.Password = userEntry.Trim()
        },
        new SettingsFileProperty<TestSettings>
        {
            PropertyDisplayName = "Backup Directory",
            PropertyEntryHelp =
                "The backup directory will be used to save the backup data - it is best to dedicate a directory just to this data to avoid conflicts with other data.",
            HideEnteredValue = false,
            PropertyIsValid =
                ObfuscatedSettingsHelpers.PropertyIsValidIfNotNullOrWhiteSpace<TestSettings>(x => x.BackupDirectory),
            UserEntryIsValid = ObfuscatedSettingsHelpers.UserEntryIsValidIfNotNullOrWhiteSpace(),
            SetValue = (settings, userEntry) => settings.BackupDirectory = userEntry.Trim()
        },
        new SettingsFileProperty<TestSettings>
        {
            PropertyDisplayName = "Days Back",
            PropertyEntryHelp =
                "The number of days back to check for backups.",
            HideEnteredValue = false,
            PropertyIsValid = ObfuscatedSettingsHelpers.PropertyIsValidIfPositiveInt<TestSettings>(x => x.NumberOfDays),
            UserEntryIsValid = ObfuscatedSettingsHelpers.UserEntryIsValidIfInt(),
            SetValue = (settings, userEntry) => settings.NumberOfDays = int.Parse(userEntry)
        }
    ]
};

var settingsSetupResultPart1 = await settingFileReadAndSetup.Setup();

if (!settingsSetupResultPart1.isValid)
{
    Console.WriteLine($"Part 1: FAILED - setup returned isValid: {settingsSetupResultPart1.isValid}");
    Console.WriteLine($"  {settingsSetupResultPart1.settings}");
    return;
}

Console.WriteLine($"Part 1: SUCCESS - setup returned isValid: {settingsSetupResultPart1.isValid}.");
Console.WriteLine($"  {settingsSetupResultPart1.settings}");

Console.WriteLine("");
Console.WriteLine("");

Console.WriteLine("Part 2: Read the created file - if you are prompted to enter");
Console.WriteLine("  ANYTHING this test has gone wrong - with the file setup in Part 1");
Console.WriteLine("  you should now be able to get/read the settings without being");
Console.WriteLine("  prompted for anything and you should see the expected settings");
Console.WriteLine("  values...");

Console.WriteLine("");

var settingsSetupResult2 = await settingFileReadAndSetup.Setup();

if (!settingsSetupResult2.isValid)
{
    Console.WriteLine($"Part 2: FAILED - setup returned isValid: {settingsSetupResult2.isValid}");
    Console.WriteLine($"  {settingsSetupResult2.settings}");
    return;
}

Console.WriteLine($"Part 2: SUCCESS - setup returned isValid: {settingsSetupResult2.isValid}.");
Console.WriteLine($"  {settingsSetupResult2.settings}");