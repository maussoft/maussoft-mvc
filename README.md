# Maussoft.Mvc

Simple C# web framework for .NET 5

### Build package

You can run the following:

    dotnet pack -p:Configuration=Release
    
This will produce a `Maussoft.Mvc` nuget package file in the `bin/Release` directory.

### TODO

- add 404 for nonmatching route 
- add debug toolbar with
  - routing debugger (remove router debug output)
  - API client debugger (API call pane)
  - MySQL connection debugging (query pane)
