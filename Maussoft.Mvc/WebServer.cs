using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Reflection;
using System.Text.Json;
using System.Collections.Generic;

//http://codehosting.net/blog/BlogEngine/post/Simple-C-Web-Server.aspx

namespace Maussoft.Mvc
{
	public class WebServer<TSession> where TSession : new()
	{
		private readonly HttpListener _listener = new HttpListener();

		private readonly string _listen = null;
		private readonly string[] _viewNamespaces = null;
		private readonly string[] _controllerNamespaces = null;
		private readonly string _sessionSavePath = null;
		private readonly int _sessionTimeout;

		public WebServer(string appSettingsJsonFilename)
		{
			string value;
			Dictionary<string, string> settings;

			using (StreamReader r = new StreamReader(appSettingsJsonFilename))
			{
				settings = JsonSerializer.Deserialize<Dictionary<string, string>>(r.ReadToEnd());
			}

			_listen = settings["Maussoft.Mvc.ListenUrl"];
			if (_listen == null) {
				_listen = "http://localhost:9000";
			}

			value = settings["Maussoft.Mvc.ViewNamespaces"];
			if (value == null) {
				System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
				MethodBase method = stackTrace.GetFrame(1).GetMethod();
				_viewNamespaces = new string[]{method.DeclaringType.Namespace+".Views"};
			} else {
				_viewNamespaces = value.Split (',');
			}

			value = settings["Maussoft.Mvc.ControllerNamespaces"];
			if (value == null) {
				System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
				MethodBase method = stackTrace.GetFrame(1).GetMethod();
				_controllerNamespaces = new string[]{method.DeclaringType.Namespace+".Controllers"};
			} else {
				_controllerNamespaces = value.Split (',');
			}

			_sessionSavePath = settings["Maussoft.Mvc.SessionSavePath"];
			if (_sessionSavePath == null) {
				_sessionSavePath = Path.Combine (System.IO.Path.GetTempPath (), "maussoftmvc");
			}

			value = settings["Maussoft.Mvc.SessionTimeout"];
			if (value == null || !int.TryParse(value, out _sessionTimeout)) {
				_sessionTimeout = 3600;
			}

			string m = "Maussoft.Mvc Server\nlisten: {0}\nviewNamespaces: {1}\ncontrollerNamespaces: {2}\nsessionSavePath: {3}\nsessionTimeout: {4}";
			Console.WriteLine(m, _listen, String.Join(", ",_viewNamespaces), String.Join(", ",_controllerNamespaces), _sessionSavePath, _sessionTimeout);

			_listener.Prefixes.Add(_listen.TrimEnd('/')+'/');
			_listener.Start();
		}

		public void Run(Assembly assembly)
		{
			ThreadPool.QueueUserWorkItem((o) =>
				{
					try
					{
						while (_listener.IsListening)
						{
							ThreadPool.QueueUserWorkItem((c) =>
								{
									HttpListenerContext context = c as HttpListenerContext;
									WebContext<TSession> webctx = null;
									Boolean found = false;
									try
									{
										if (!StaticServer.Serve(assembly, context)) {
											webctx = new WebContext<TSession>(context,_sessionSavePath);
											Console.WriteLine(webctx.Url); // access log
											webctx.StartSession();
											found = (new ActionRouter<TSession>(_controllerNamespaces)).Route(webctx);
											if (!found) webctx.View = "Error.NotFound";
											found = new ViewRouter<TSession>(_viewNamespaces).Route(webctx);
											if (!webctx.Sent) webctx.SendString("NotFound",404);
											webctx.WriteSession();
										}
									}
									catch (Exception e) // application log
									{
										Console.WriteLine(e.ToString());
										if (webctx!=null) webctx.SendString("<pre>"+e.ToString()+"</pre>",500);
									} 
									finally
									{
										context.Response.OutputStream.Close();
										if (webctx!=null) webctx.CloseSession();
									}
								}, _listener.GetContext());
						}
					}
					catch (Exception e) // application log
					{
						Console.WriteLine(e.ToString());
					} 
				});
			for(;true;) { 
				System.Threading.Thread.Sleep (10000);
				CleanUpSessionFiles ();
			}
		}

		private void CleanUpSessionFiles()
		{
			string dir = Path.GetDirectoryName (_sessionSavePath);
			string file = Path.GetFileName (_sessionSavePath);
			foreach(FileInfo f in new DirectoryInfo(dir).GetFiles(file+"*")) {
				if (f.LastWriteTime < DateTime.Now.AddSeconds (-1*_sessionTimeout)) {
					f.Delete ();
				}
			}
		}

		public void Stop()
		{
			_listener.Stop();
			_listener.Close();
		}
	}


}
