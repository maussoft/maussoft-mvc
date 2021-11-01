# maussoft-mvc

High speed web framework built in C# that runs on .NET 5

### Build single executable

You can run the following in the `example-cs` directory:

    dotnet publish \
        /p:Configuration=Release \
        /p:PublishSingleFile=true \
        /p:RuntimeIdentifier=linux-x64 \
        /p:DebugType=None \
        /p:DebugSymbols=false
    
You end up with `appsettings.json` (config file) and `Acme.Example` (executable) in the `bin/Release/net5.0/linux-x64/publish` directory.
