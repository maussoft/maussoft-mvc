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
        private readonly HttpListener listener = new HttpListener();

        private readonly string listen = null;
        private readonly Assembly assembly = null;
        private readonly string viewNamespace = null;
        private readonly string controllerNamespace = null;
        private readonly string sessionSavePath = null;
        private readonly int sessionTimeout;

        public WebServer(string appSettingsJsonFilename)
        {
            string value;
            Dictionary<string, string> settings;

            using (StreamReader r = new StreamReader(appSettingsJsonFilename))
            {
                settings = JsonSerializer.Deserialize<Dictionary<string, string>>(r.ReadToEnd());
            }

            this.assembly = Assembly.GetEntryAssembly();

            this.listen = settings["Maussoft.Mvc.ListenUrl"];
            if (this.listen == null)
            {
                this.listen = "http://localhost:9000";
            }

            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
            MethodBase method = stackTrace.GetFrame(1).GetMethod();
            this.viewNamespace = method.DeclaringType.Namespace + ".Views";
            this.controllerNamespace = method.DeclaringType.Namespace + ".Controllers";

            this.sessionSavePath = settings["Maussoft.Mvc.SessionSavePath"];
            if (this.sessionSavePath == null)
            {
                this.sessionSavePath = Path.Combine(System.IO.Path.GetTempPath(), "maussoftmvc_");
            }

            value = settings["Maussoft.Mvc.SessionTimeout"];
            if (value == null || !int.TryParse(value, out this.sessionTimeout))
            {
                this.sessionTimeout = 3600;
            }

            string m = "Maussoft.Mvc Server\nlisten: {0}\nviewNamespace: {1}\ncontrollerNamespace: {2}\nsessionSavePath: {3}\nsessionTimeout: {4}";
            Console.WriteLine(m, this.listen, this.viewNamespace, this.controllerNamespace, this.sessionSavePath, this.sessionTimeout);

            this.listener.Prefixes.Add(this.listen.TrimEnd('/') + '/');
            this.listener.Start();
        }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
                {
                    try
                    {
                        while (this.listener.IsListening)
                        {
                            ThreadPool.QueueUserWorkItem((c) =>
                                {
                                    HttpListenerContext context = c as HttpListenerContext;
                                    WebContext<TSession> webctx = null;
                                    Boolean actionResult = false;
                                    string viewResult = null;
                                    try
                                    {
                                        if (!StaticServer.Serve(this.assembly, context))
                                        {
                                            webctx = new WebContext<TSession>(context, this.sessionSavePath);
                                            Console.WriteLine(webctx.Method + " " + webctx.Url); // access log
                                            webctx.StartSession(webctx.Method == "GET");
                                            actionResult = (new ActionRouter<TSession>(this.controllerNamespace)).Route(webctx);
                                            webctx.WriteSession();
                                            if (!actionResult) webctx.View = "Error.NotFound";
                                            if (webctx.RedirectUrl == null)
                                            {
                                                viewResult = new ViewRouter<TSession>(this.viewNamespace).Route(webctx);
                                                webctx.FinalizeSession();
                                                if (viewResult == null) webctx.SendString("NotFound", "text/plain", 404);
                                                else webctx.SendString(viewResult);
                                            }
                                            else
                                            {
                                                webctx.SendString(viewResult);
                                            }
                                        }
                                    }
                                    catch (Exception e) // application log
                                    {
                                        Console.WriteLine(e.ToString());
                                        if (webctx != null) webctx.SendString("<pre>" + e.ToString() + "</pre>", "text/html", 500);
                                    }
                                    finally
                                    {
                                        context.Response.OutputStream.Close();
                                    }
                                }, this.listener.GetContext());
                        }
                    }
                    catch (Exception e) // application log
                    {
                        Console.WriteLine(e.ToString());
                    }
                });
            for (; true;)
            {
                System.Threading.Thread.Sleep(10000);
                CleanUpSessionFiles();
            }
        }

        private void CleanUpSessionFiles()
        {
            string dir = Path.GetDirectoryName(this.sessionSavePath);
            string file = Path.GetFileName(this.sessionSavePath);
            foreach (FileInfo f in new DirectoryInfo(dir).GetFiles(file + "*"))
            {
                if (f.LastWriteTime < DateTime.Now.AddSeconds(-1 * this.sessionTimeout))
                {
                    f.Delete();
                }
            }
        }

        public void Stop()
        {
            this.listener.Stop();
            this.listener.Close();
        }
    }


}
