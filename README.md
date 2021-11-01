# maussoft-mvc

High speed web framework built in C# that runs on .NET 5

### Build single executable

You can run the following in the `Maussoft.Mvc.CsTest` directory:

    dotnet publish \
        /p:Configuration=Release \
        /p:PublishSingleFile=true \
        /p:RuntimeIdentifier=linux-x64 \
        /p:DebugType=None \
        /p:DebugSymbols=false
    
You end up with two files in `bin/Release/net5.0/linux-x64/publish`:

- appsettings.json
- Maussoft.Mvc.CsTest

One is the config file, the other one the executable.
 
### TODO

- add 404 for nonmatching route 
- add debug toolbar with
  - routing debugger (remove router debug output)
  - API client debugger (API call pane)
  - MySQL connection debugging (query pane)
