using System.Configuration;

namespace Maussoft.Mvc.CsTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			new WebServer<Session>(ConfigurationManager.AppSettings).Run();
		}
	}
}
