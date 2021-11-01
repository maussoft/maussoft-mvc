/**
 * WARNING: Generated file, do not edit, changes will be lost!
 **/

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Acme.Example.Views.Test
{
	public class Index<TSession>: Layouts.Default<TSession> where TSession : new()
	{
		public override void Content()
		{
			WriteLine();
			Context.Data.Title = Context.Data.Name; WriteLine();
			WriteLine(@"Email: <input type=""text"" value=""{0}""></input>",  Context.Data.Email );
			Write(@"Test: <input type=""text"" value=""{0}""></input>",  "<?/!@#$$%^&*()_+-={}[]|\\:\"';,.>");
		}
	}
}