using System;
using System.Configuration;

using Maussoft.Mvc;

namespace Maussoft.Mvc.CsTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			new WebServer(ConfigurationManager.AppSettings).Run();
		}
	}
}
