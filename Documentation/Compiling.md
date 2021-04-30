# Compiling the interpreter

## Setup

1. You need `dotnet` to compile the interpreter. Head over to [the `dotnet` website](https://dotnet.microsoft.com/download) and get the SDK and the runtime for .NET 5.0.
2. Reboot your computer to apply the changes and update your environment variables.
3. Clone the repository or download the source code.
4. Open PowerShell and navigate to the directory where you downloaded the code. In this directory, there should be files like `Giosue` and `Giosue.ConsoleApp`.

## Compiling

1. To compile the interpreter, issue the following command: `dotnet build --configuration Release`

## Running the interpreter

1. To run the REPL, issue this command: `dotnet run --configuration Release --project .\Giosue.ConsoleApp\`
   1. This may be slow; `dotnet` builds the project before running. To prevent this, add the `--no-build` flag: `dotnet run --no-build --configuration Release --project .\Giosue.ConsoleApp\`
2. To run a source code file, issue this command: `dotnet run --configuration Release --project .\Giosue.ConsoleApp\ path\to\your\source\code\file.gsu`
   1. Adding the `--no-build` flag may have a slight performance improvement: `dotnet run --no-build --configuration Release --project .\Giosue.ConsoleApp\ path\to\your\source\code\file.gsu`
