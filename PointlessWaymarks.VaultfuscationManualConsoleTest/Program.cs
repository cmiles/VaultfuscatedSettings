using System.Reflection;
using GitCredentialManager;
using Microsoft.Extensions.Logging;
using PointlessWaymarks.VaultfuscationManualConsoleTest;
using PointlessWaymarks.VaultfuscationTools;

using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger<ObfuscatedSettingsConsoleSetup<TestSettings>>();

AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
{
    Console.WriteLine("");
    Console.WriteLine("FAILED!!! Unhandled Exception...");
    Console.WriteLine("");

    logger.LogCritical(eventArgs.ExceptionObject as Exception,
        $"Unhandled Exception {(eventArgs.ExceptionObject as Exception)?.Message ?? ""}");
    return;
};

var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
var testSettingsFile = Path.Combine(executingDirectory, $"TestSettings-{DateTime.Now.Ticks}.json");

Console.WriteLine("");
Console.WriteLine("");
Console.WriteLine("Vaultfuscation Manual Console Test - this is an interactive/manual test");
Console.WriteLine("  that will prompt you to enter some values for a test settings file."); 
Console.WriteLine("");
Console.WriteLine("The VaultfuscatedSettings library is IN NO WAY INTENDED TO ENCRYPT OR");
Console.WriteLine("  FULLY SECURE ANY DATA - strings in memory are not secure and the");
Console.WriteLine("  key that is used to de-obfuscate a settings file is stored in a");
Console.WriteLine("  LOCAL Credentials Vault.");
Console.WriteLine("");
Console.WriteLine("Any attacker or person with full access to your user account");
Console.WriteLine("  will automatically have ALL of the pieces they need to access");
Console.WriteLine("  a settings file created with this library!!! In some cases this is valuable");
Console.WriteLine("  and meaningful protection - in some cases it is completely useless and");
Console.WriteLine("  a dangerously bad choice!");
Console.WriteLine("");
Console.WriteLine($"Executing Directory: {executingDirectory}");
Console.WriteLine("");
Console.WriteLine($"Test Settings File: {testSettingsFile}");
Console.WriteLine("");
Console.WriteLine("");


var vaultService = "http://vaultfuscationmanualconsoletest.test";

var preCheckStore = CredentialManager.Create();
var preCheckCredentials = preCheckStore.Get(vaultService, new ObfuscatedSettingsConsoleSetup<TestSettings>(logger).VaultAccount);

if (preCheckCredentials is not null)
{
    Console.WriteLine("Setup: Found existing Vault Credentials - Removing");
    preCheckStore.Remove(vaultService, new ObfuscatedSettingsConsoleSetup<TestSettings>(logger).VaultAccount);

    preCheckCredentials = preCheckStore.Get(vaultService, new ObfuscatedSettingsConsoleSetup<TestSettings>(logger).VaultAccount);

    if(preCheckCredentials is not null)
    {
        Console.WriteLine("Setup: FAILED - Could Not Remove Existing Vault Credentials");
        return;
    }
}

Console.WriteLine("");
Console.WriteLine("Part 1 - New File Setup:");
Console.WriteLine("");

var settingFileReadAndSetup = new ObfuscatedSettingsConsoleSetup<TestSettings>(logger)
{
    SettingsFile = testSettingsFile,
    SettingsFileIdentifier = TestSettings.SettingsTypeIdentifier,
    VaultServiceIdentifier = vaultService,
    SettingsFileProperties =
    [
        new SettingsFileProperty<TestSettings>
        {
            PropertyDisplayName = "Email",
            PropertyEntryHelp =
                "The Email for ... - this email field doesn't try to validate for a valid email, just rejects blank.",
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
                "The Password for ... - blank is not valid.",
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
                "A directory field - this is setup to try to create the directory if it does not exist and is only valid if the directory exists or can be created. If you input a directory that does not exist and the program can create it you will have to manually clean it up - the test program will not auto-delete it...",
            HideEnteredValue = false,
            PropertyIsValid =
                ObfuscatedSettingsHelpers.PropertyIsValidIfDirectoryExistsOrCanBeCreated<TestSettings>(x => x.BackupDirectory),
            UserEntryIsValid = ObfuscatedSettingsHelpers.UserEntryIsValidIfNotNullOrWhiteSpace(),
            SetValue = (settings, userEntry) => settings.BackupDirectory = userEntry.Trim()
        },
        new SettingsFileProperty<TestSettings>
        {
            PropertyDisplayName = "Days Back",
            PropertyEntryHelp =
                "Tests a int field in the settings file - inputting something like 'a' will prompt you again for entry since it is not a valid int.",
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
    Console.WriteLine("");
    Console.WriteLine($"Part 1: FAILED - setup returned isValid: {settingsSetupResultPart1.isValid}");
    Console.WriteLine($"  {settingsSetupResultPart1.settings}");
    return;
}

Console.WriteLine("");
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
    Console.WriteLine("");
    Console.WriteLine($"Part 2: FAILED - setup returned isValid: {settingsSetupResult2.isValid}");
    Console.WriteLine($"  {settingsSetupResult2.settings}");
    return;
}

Console.WriteLine($"Part 2: SUCCESS - setup returned isValid: {settingsSetupResult2.isValid}.");
Console.WriteLine($"  {settingsSetupResult2.settings}");

Console.WriteLine("");

Console.WriteLine("Part 3: Check Vault for Key and Delete");

var store = CredentialManager.Create();
var settingsFileKey = store.Get(vaultService, settingFileReadAndSetup.VaultAccount);

if (settingsFileKey is null)
{
    Console.WriteLine("Part 3: FAILED - Could Not Get Expected Vault Credentials");
    return;
}

Console.WriteLine("");
Console.WriteLine(
    $"Vault Key (should match what you entered above - the program will not automatically check for a match...): {settingsFileKey!.Password}");
store.Remove(vaultService, settingFileReadAndSetup.VaultAccount);

var settingsFileKeyExpectedDeleted = store.Get(vaultService, settingFileReadAndSetup.VaultAccount);

Console.WriteLine("");
Console.WriteLine(settingsFileKeyExpectedDeleted is not null
    ? "Part 3: FAILED - Delete of Vault Credentials Failed?"
    : "Part 3: SUCCESS - Vault Credentials Deleted");