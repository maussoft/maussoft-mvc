Imports System.Configuration

Imports Maussoft.Mvc

Public Class Application
	Public Shared Sub Main()
		Dim w as New WebServer(ConfigurationManager.AppSettings)
		w.Run()
	End Sub
End Class

