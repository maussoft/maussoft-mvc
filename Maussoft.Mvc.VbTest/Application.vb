Imports System.Configuration

Public Class Application
	Public Shared Sub Main()
		Dim w as New WebServer(Of Session)(ConfigurationManager.AppSettings)
		w.Run()
	End Sub
End Class
