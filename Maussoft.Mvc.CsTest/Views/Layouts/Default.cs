/**
 * WARNING: Generated file, do not edit, changes will be lost!
 **/

using System;

namespace Maussoft.Mvc.CsTest.Views.Layouts
{
	public class Default: Maussoft.Mvc.View
	{
		public override void Header()
		{
			WriteLine();
			WriteLine(@"<!DOCTYPE html>");
			WriteLine(@"<html>");
			WriteLine(@"  <head>");
			WriteLine(@"    <title>Hello {0}</title>",  Context.Data ["Name"] );
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