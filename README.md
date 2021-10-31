# maussoft-mvc

High speed web framework built in C# that runs on .NET 5

### Build single executable

    dotnet publish /p:Configuration=Release /p:PublishSingleFile=true /p:RuntimeIdentifier=linux-x64

### TODO

- config in json format
- add 404 for nonmatching route (with debug info) and remove router debug output
- add debug toolbar with API client (call pane) and MySQL connection debugging (query pane)

