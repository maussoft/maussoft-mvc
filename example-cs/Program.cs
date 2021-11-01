using Maussoft.Mvc;

namespace Acme.Example
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			new WebServer<Session>("appsettings.json").Run();
		}
	}
}
