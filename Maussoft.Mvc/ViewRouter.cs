using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Maussoft.Mvc;

//http://dotnetslackers.com/Community/blogs/haissam/archive/2007/07/25/Call-a-function-using-Reflection.aspx

namespace Maussoft.Mvc
{
	public class ViewRouter
	{
		string[] namespaces;

		public ViewRouter(string[] namespaces)
		{
			this.namespaces = namespaces;
		}

		private Boolean Invoke(WebContext context, Type routedClass)
		{
			View view = Activator.CreateInstance(routedClass) as View;
			if (view == null) {
				Console.WriteLine ("ViewRouter: object {0} could not be created.", routedClass.FullName);
				return false;
			}
			Console.WriteLine ("ViewRouter: object {0} was created.", routedClass.FullName);

			context.SendString (view.Render (context));
			Console.WriteLine ("ViewRouter: invoked {0}.Render()", routedClass.FullName);

			return true;
		}

		private Boolean Match(WebContext context, string prefix, string className)
		{
			Type routedClass = null;
			Assembly assembly = Assembly.GetEntryAssembly();

			Console.WriteLine ("ViewRouter: try {0}.Render()", className);

			routedClass = assembly.GetType(prefix+'.'+className);
			if (routedClass == null) {
				Console.WriteLine ("ViewRouter: class {0} does not exist.", prefix+'.'+className);
				return false;
			}
			Console.WriteLine ("ViewRouter: class {0} found.", prefix+'.'+className);

			return this.Invoke(context, routedClass);
		}

		public void Route(WebContext context)
		{
			if (context.Sent) return;
			if (context.View == null) return; //Route to 404?

			string[] parts = context.View.Split ('.');
			string className = null;

			foreach (string prefix in this.namespaces) {
				for (int i = parts.Length-1; i >= 0; i--) {

					className = (String.Join (".", parts, 0, i) + '.' + parts[parts.Length-1]).Trim('.');

					if (this.Match (context, prefix, className)) {
						return;
					}

				}
			}
		}
		
	}
}