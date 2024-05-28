# Vaultfuscated Settings

It is a wonderful thing when you have the programming skills to sit down and put together a simple program to create some functionality or solve a problem - maybe a quick console app to download the weather data from your home weather station...

One challenge if you frequently write these types of small personal programs is where and how to store settings - especially if you are keeping things simple and don't have a server/database/cloud to access, and especially if the program is intended to be automated.

In some cases with highly sensitive data there is simply no perfect solution and the truth is that it is a bad idea to try to save sensitive information and fully automate a program...

But in other cases obfuscating the settings so that they are very difficult to read by anyone without full access to your personal user account is good enough!

This program tries to help you setup on the commandline and read/write a settings file that is obfuscated using a key stored in a local credential vault - some important details:
  - Data in memory IS NOT protected in any way - no secure strings or other mechanisms are used
  - The key used to obfuscate the settings is stored in a local credential vault - this is secure as long as your user account is secure, but is easily and immediately accessible to anyone with access to your user account.
  - Settings files are obfuscated using the key stored in the vault - 'casual' access to the file (viewing it with notepad for example) will not reveal anything and outside of the context of your user account the file is very difficult to de-obfuscate.
  - The settings files are AES encrypted, but with so many potential security problems it seems more appropriate/honest/safe to refer to them as obfuscated.

This library doesn't represent anything particularly difficult to code, new or novel, but I haven't yet found a way to do this without repeating too much boilerplate code in each project... This library is designed to help with the easy creation and use of obfuscated settings files intended for use inside a single user account.
