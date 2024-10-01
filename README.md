# Introduction 
Provides classes for fetching data from CatalystOne API and a small program to demonstrate how to use the classes. A license from CatalystOne is required for this to work.

# Getting Started
The credentils used to connect to CatalystOne is stored in Catalyst_ClientId.txt and Catalyst_ClientSecret.txt which are located in the CredentialsDir as specified in appsettings.json.
There is a setting useCache, which if true tries to read data from disk instead of from the API. This is useful for testing and development. The program always writes data read from CatalystOne to disk.

# Build and Test
Open the solution in Visual Studio and run the project.

# Additional Information
The classes CatalystEmployyes.cs and CatalystOrgs.cs contains fields that are customized for a specific installation of CatalystOne and will probaly not be present in other installations.
All fields not present in the data from CatalystOne are set to null. 
