# Maussoft.Mvc

Simple C# web framework for .NET 5

For example applications using this package, look at:

- [Example in C#](https://github.com/maussoft/mvc-example-cs)
- [Example in VB](https://github.com/maussoft/mvc-example-vb)

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
