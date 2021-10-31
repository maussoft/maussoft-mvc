using System.Reflection;

namespace Maussoft.Mvc.CsTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			new WebServer<Session>("appsettings.json").Run(Assembly.GetExecutingAssembly());
		}
	}
}
