# Vaultfuscated Settings

It is a wonderful thing when you have the programming skills to sit down and put together a simple program to create some functionality or solve a problem - maybe a quick console app to download the weather data from your home weather station...

For me a challenge with these small, useful, programs when you want to fully automate them is where to store program settings when the program is meant to be a stand alone program run on a personal computer with no server/database/cloud infrastructure? 
In some cases with highly sensitive data the is simply no perfect solution and sometimes the truth is that it is a bad idea to try to fully automate a program...

But in other cases obfuscating the settings so that they are very difficult to read by anyone without full access to your personal user account is good enough!

This program tries to help you setup on the commandline and read/write a settings file that is obfuscated using a key stored in a local credential vault:
  - Data in memory IS NOT protected - no secure strings or other mechanisms are used
  - The key used to obfuscate the settings is stored in a local credential vault - this is secure as long as your user account is secure, but is easily and immediately accessible to anyone with access to your user account.
  - Settings files are obfuscated using the key stored in the vault - 'casual' access to the file (viewing it with notepad for example) will not reveal anything and the file outside of the context of your user account would be very difficult to de-obfuscate.
  - The settings files are AES encrypted but because there are so many potential problems with the security of these files it seems more appropriate/honest in this case to refer to them as obfuscated.

This library doesn't represent anything particularly difficult to code, new or novel - but I haven't yet found a way to do this without repeating too much boilerplate code in each project... This library was designed to help what is for me a frequent use case - easy creation of obfuscated settings files for used by programs, sometimes automated, running under my personal account. 
