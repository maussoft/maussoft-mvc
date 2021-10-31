using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using Maussoft.Mvc;
using System.Text.Json;

namespace Maussoft.Mvc.CsTest.Controllers
{
	public class Test
	{
		public Test()
		{ 
			// before handler?
		}

		public void Hello(WebContext<Session> context, long id, int test, UInt32 test3, string test4, string name="world")
		{
			List<string> names;

			if (context.Session.Names != null) {
				names = context.Session.Names;
			} else {
				names = new List<string>();
				context.Session.Names = names;
			}

			string rstr = String.Format ("{0}\n{1}\n{2}\n{3}\nhello {4}\nold: {5}", id, test,test3,test4, name, String.Join(",",names.ToArray()));

			names.Add(name);

			context.Data.Name = "World";
			context.Data.Html = rstr;

			context.SendString (rstr);
		}

		public void Index(WebContext<Session> context)
		{
			context.Data.Name = "World";
			context.Data.Email = "test@test.com";
		}

		public void Index2(WebContext<Session> context)
		{
			Index (context);
			context.View = "Test.Index";
		}

	}

}