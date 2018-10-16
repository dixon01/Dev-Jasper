
# Nuget Package Creation

https://docs.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package-using-the-dotnet-cli

1. create the project

dotnet new classlib

2. Add nuget package metadata to the project file

<PackageId>testnugetupload</PackageId>
<Version>1.0.2</Version>
<Authors>Natraj</Authors>
<Company>Effysoft</Company>

3. run the dotnet pack

4. Automatically generate package on build add to project file 

<GeneratePackageOnBuild>true</GeneratePackageOnBuild>

5. Get the Api key

oy2g36rcjqiurszyfzkq4c7qkcxhodee5hemp74fudbvny

Save the key inside of the project

nuget setApiKey <key>

6. Publish with dotnet nuget push
Change to the folder containing the .nupkg file.

Run the following command, specifying your package name and replacing the key value with your API key:

## Did not work 

dotnet nuget push weriotexreo.1.0.2.nupkg -k oy2g36rcjqiurszyfzkq4c7qkcxhodee5hemp74fudbvny -src https://api.nuget.org/v3/index.json

## Worked !!!

nuget push weriotexreo.1.0.2.nupkg oy2g36rcjqiurszyfzkq4c7qkcxhodee5hemp74fudbvny -src https://api.nuget.org/v3/index.json

## Output:

Apples-MacBook-Pro:debug apple$ nuget push weriotexreo.1.0.2.nupkg oy2g36rcjqiurszyfzkq4c7qkcxhodee5hemp74fudbvny -src https://api.nuget.org/v3/index.json
Pushing weriotexreo.1.0.2.nupkg to 'https://www.nuget.org/api/v2/package'...
  PUT https://www.nuget.org/api/v2/package/
  Created https://www.nuget.org/api/v2/package/ 1479ms
Your package was pushed.


