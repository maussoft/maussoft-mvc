using System.Configuration;
using System.Reflection;

namespace Maussoft.Mvc.CsTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			// https://andyp.dev/posts/retrieve-app-settings-values-by-static-in-asp-net-core-3
			new WebServer<Session>(ConfigurationManager.AppSettings).Run(Assembly.GetExecutingAssembly());
		}
	}
}
