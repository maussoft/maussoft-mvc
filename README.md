# maussoft-mvc

High speed web framework built in C# that runs on .NET 5

### Build single executable

    dotnet publish /p:Configuration=Release /p:PublishSingleFile=true /p:RuntimeIdentifier=linux-x64

### TODO

- config in json format
- add 404 for nonmatching route 
- add debug toolbar with
  - routing debugger (remove router debug output)
  - API client debugger (API call pane)
  - MySQL connection debugging (query pane)
