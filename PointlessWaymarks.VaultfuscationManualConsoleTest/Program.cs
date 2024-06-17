using System.Reflection;
using GitCredentialManager;
using Microsoft.Extensions.Logging;
using PointlessWaymarks.CommonTools;
using PointlessWaymarks.VaultfuscationManualConsoleTest;
using PointlessWaymarks.VaultfuscationTools;

using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger<ObfuscatedSettingsConsoleSetup<TestSettings>>();

AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
{
    Console.WriteLine("");
    Console.WriteLine("FAILED!!! Unhandled Exception...");
    Console.WriteLine("");

    logger.LogCritical(eventArgs.ExceptionObject as Exception,
        $"Unhandled Exception {(eventArgs.ExceptionObject as Exception)?.Message ?? ""}");
};

var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
var testSettingsFile = Path.Combine(executingDirectory, $"TestSettings-{DateTime.Now.Ticks}.json");

ConsoleTools.WriteLineWrappedTextBlock("""
                                   Vaultfuscation Manual Console Test - this is an interactive/manual test that will use a Test settings file, prompt you to enter some values and exercise some of the features of the Vaultfuscation library.

                                   An important aspect of this library, both in name and intent, is to be very clear about the security implications of this approach. I believe the level of security offered by storing an encryption key in the local user's Vault/Credential Manager and using that to decrypt a file on disk is a good approach for many cases allowing seamless access for the user and for programs running as the user (included automated programs). But this approach does mean that anyone with full access to the user's account or Vault/Credential Manager has everything they need to decrypt the settings file - far from appropriate in all cases.

                                   The program can write a simple warning to the console:

                                   """);

VaultfuscationMessages.VaultfuscationWarning();

ConsoleTools.WriteLineWrappedTextBlock("""
                                   And can also force a user to press Y to acknowledge a warning:
                                   """);

VaultfuscationMessages.VaultfuscationWarningAndUserAcknowledgement();


ConsoleTools.WriteLineWrappedTextBlock($"Executing Directory: {executingDirectory}");

ConsoleTools.WriteLineWrappedTextBlock($"Test Settings File: {testSettingsFile}");

var vaultService = "http://vaultfuscationmanualconsoletest.test";

ConsoleTools.WriteLineWrappedTextBlock($"Vault Service: {vaultService}");

var preCheckStore = CredentialManager.Create();
var preCheckCredentials =
    preCheckStore.Get(vaultService, new ObfuscatedSettingsConsoleSetup<TestSettings>(logger).VaultAccount);

if (preCheckCredentials is not null)
{
    Console.WriteLine("Setup: Found existing Vault Credentials - Removing");
    preCheckStore.Remove(vaultService, new ObfuscatedSettingsConsoleSetup<TestSettings>(logger).VaultAccount);

    preCheckCredentials =
        preCheckStore.Get(vaultService, new ObfuscatedSettingsConsoleSetup<TestSettings>(logger).VaultAccount);

    if (preCheckCredentials is not null)
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
            ShowCurrentSettingAsDefault = false,
            PropertyIsValid =
                ObfuscatedSettingsHelpers.PropertyIsValidIfNotNullOrWhiteSpace<TestSettings>(x => x.Email),
            UserEntryIsValid = ObfuscatedSettingsHelpers.UserEntryIsValidIfNotNullOrWhiteSpace(),
            GetCurrentStringValue = x => x.Email,
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
            GetCurrentStringValue = x => x.Password,
            SetValue = (settings, userEntry) => settings.Password = userEntry.Trim()
        },
        new SettingsFileProperty<TestSettings>
        {
            PropertyDisplayName = "Backup Directory",
            PropertyEntryHelp =
                "A directory field - this is setup to try to create the directory if it does not exist and is only valid if the directory exists or can be created. If you input a directory that does not exist and the program can create it you will have to manually clean it up - the test program will not auto-delete it...",
            HideEnteredValue = false,
            PropertyIsValid =
                ObfuscatedSettingsHelpers.PropertyIsValidIfDirectoryExistsOrCanBeCreated<TestSettings>(x =>
                    x.BackupDirectory),
            UserEntryIsValid = ObfuscatedSettingsHelpers.UserEntryIsValidIfNotNullOrWhiteSpace(),
            GetCurrentStringValue = x => x.BackupDirectory,
            SetValue = (settings, userEntry) => settings.BackupDirectory = userEntry.Trim()
        },
        new SettingsFileProperty<TestSettings>
        {
            PropertyDisplayName = "Days Back",
            PropertyEntryHelp =
                "Tests a int field in the settings file - inputting something like 'a' will prompt you again for entry since it is not a valid int.",
            ShowCurrentSettingAsDefault = false,
            HideEnteredValue = false,
            PropertyIsValid = ObfuscatedSettingsHelpers.PropertyIsValidIfPositiveInt<TestSettings>(x => x.NumberOfDays),
            UserEntryIsValid = ObfuscatedSettingsHelpers.UserEntryIsValidIfInt(),
            GetCurrentStringValue = x => x.NumberOfDays.ToString(),
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
    $"Vault Key (should match what you entered above - the program will not automatically check for a match...): {settingsFileKey.Password}");
store.Remove(vaultService, settingFileReadAndSetup.VaultAccount);

var settingsFileKeyExpectedDeleted = store.Get(vaultService, settingFileReadAndSetup.VaultAccount);

Console.WriteLine("");
Console.WriteLine(settingsFileKeyExpectedDeleted is not null
    ? "Part 3: FAILED - Delete of Vault Credentials Failed?"
    : "Part 3: SUCCESS - Vault Credentials Deleted");