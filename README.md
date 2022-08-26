# Maussoft.Mvc

Simple C# web framework for .NET 6

For example applications using this package, look at:

- [Example in C#](https://github.com/maussoft/mvc-example-cs)
- [Example in VB.net](https://github.com/maussoft/mvc-example-vb)

### Build package

You can run the following:

    dotnet pack -p:Configuration=Release
    
This will produce a `Maussoft.Mvc` [NuGet package](https://www.nuget.org/packages/Maussoft.Mvc/0.9.2) file in the `bin/Release` directory.

### TODO

- add prettier 404 for nonmatching route 
- add debug toolbar with
  - routing debugger (remove router debug output)
  - API client debugger (API call pane)
  - MySQL connection debugging (query pane)
- ~~read-only sessions (idempotency)~~
- database access disabled for view
- csrf token support on post requests
- named parameters on get requests
