/**
 * WARNING: Generated file, do not edit, changes will be lost!
 **/

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Maussoft.Mvc.CsTest.Views.Test
{
	public class Index: Layouts.Default
	{
		public override void Content()
		{
			WriteLine();
			Context.Data ["title"] = Context.Data ["Name"]; WriteLine();
			WriteLine(@"Email: <input type=""text"" value=""{0}""></input>",  Context.Data["Email"]);
			Write(@"Test: <input type=""text"" value=""{0}""></input>",  "<?/!@#$$%^&*()_+-={}[]|\\:\"';,.>");
		}
	}
}