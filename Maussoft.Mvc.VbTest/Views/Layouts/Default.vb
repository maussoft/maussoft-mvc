'
' WARNING: Generated file, do not edit, changes will be lost!
'


Namespace Views.Layouts
	Public Class [Default]
		Inherits Global.Maussoft.Mvc.View
		
		Public Overrides Sub Header()
			WriteLine()
			WriteLine("<!DOCTYPE html>")
			WriteLine("<html>")
			WriteLine("  <head>")
			WriteLine("    <title>Hello {0}</title>",  Context.Data ("Name") )
			WriteLine("  </head>")
			WriteLine("  <body>")
			Write("    ")
		End Sub

		Public Overrides Sub Footer()
			WriteLine()
			WriteLine("  </body>")
			Write("</html>")
		End Sub
	End Class
End Namespace