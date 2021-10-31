/**
 * WARNING: Generated file, do not edit, changes will be lost!
 **/

using System;

namespace Maussoft.Mvc.CsTest.Views.Layouts
{
	public class Default<TSession>: Maussoft.Mvc.View<TSession> where TSession : new()
	{
		public override void Header()
		{
			WriteLine();
			WriteLine(@"<!DOCTYPE html>");
			WriteLine(@"<html>");
			WriteLine(@"  <head>");
			WriteLine(@"    <title>Hello {0}</title>",  Context.Data.Name );
			WriteLine(@"  </head>");
			WriteLine(@"  <body>");
			Write(@"    ");
		}

		public override void Footer()
		{
			WriteLine();
			WriteLine(@"  </body>");
			Write(@"</html>");
		}
	}
}