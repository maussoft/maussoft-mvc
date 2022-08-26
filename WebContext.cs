using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Security.Cryptography;
using System.Text.Json;

namespace Maussoft.Mvc
{
    public class WebContext<TSession> where TSession : new()
    {
        public string Method;
        public string Url;
        public string Controller;
        public string Action;
        public string View;
        public string RedirectUrl;
        public bool Sent;

        public Dictionary<string, string> Post;
        public CookieCollection Cookies;
        public dynamic Data;

        public string SessionIdentifier;
        public TSession Session;
        private int sessionHashCode;

        private HttpListenerContext context;
        private string sessionSavePath;

        private FileStream sessionStream;

        public WebContext(HttpListenerContext context, string sessionSavePath)
        {
            this.context = context;
            this.sessionSavePath = sessionSavePath;

            this.Method = this.context.Request.HttpMethod;
            this.Url = this.context.Request.RawUrl;
            this.Post = new Dictionary<string, string>();
            this.ReadPostData();

            this.Data = new ViewData();
            this.RedirectUrl = null;
            this.Sent = false;
        }

        private String CreateSessionIdentifier()
        {
            Byte[] data = RandomNumberGenerator.GetBytes(18);
            return Convert.ToBase64String(data).Replace("/", "_").Replace("+", "-");
        }

        public void StartSession(Boolean readOnly)
        {
            Cookie cookie = this.context.Request.Cookies["Maussoft.Mvc"];
            if (cookie == null)
            {
                SessionIdentifier = CreateSessionIdentifier();
                cookie = new Cookie("Maussoft.Mvc", SessionIdentifier);
                this.context.Response.AppendCookie(cookie);
            }
            else
            {
                SessionIdentifier = cookie.Value;
            }

            this.sessionStream = WaitForFile(this.sessionSavePath + SessionIdentifier, FileMode.OpenOrCreate, readOnly ? FileAccess.Read : FileAccess.ReadWrite, FileShare.Read);

            if (this.sessionStream.Length == 0)
            {
                Session = new TSession();
                var bytes = JsonSerializer.SerializeToUtf8Bytes<TSession>(Session);
                sessionHashCode = ComputeHash(bytes);
            }
            else
            {
                byte[] bytes = new byte[this.sessionStream.Length];
                this.sessionStream.Read(bytes, 0, bytes.Length);
                Session = JsonSerializer.Deserialize<TSession>(bytes);
                sessionHashCode = ComputeHash(bytes);
            }

            if (readOnly)
            {
                this.sessionStream.Close();
                this.sessionStream = null;
            }

        }

        private static int ComputeHash(params byte[] data)
        {
            var hash = new HashCode();
            hash.AddBytes(data);
            return hash.ToHashCode();
        }

        public void WriteSession()
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes<TSession>(Session);

            var newSessionHashCode = ComputeHash(bytes);

            if (newSessionHashCode != this.sessionHashCode)
            {
                if (this.sessionStream != null)
                {
                    this.sessionStream.SetLength(0);
                    this.sessionStream.Write(bytes, 0, bytes.Length);
                    this.sessionHashCode = newSessionHashCode;
                }
                else
                {
                    throw new Exception("A '" + Method + "' on '" + Controller + "." + Action + "' shouldn't write to the session in the Controller");
                }
            }

            if (this.sessionStream != null)
            {
                this.sessionStream.Close();
            }
        }

        public void FinalizeSession()
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes<TSession>(Session);

            var newSessionHashCode = ComputeHash(bytes);

            if (newSessionHashCode != this.sessionHashCode)
            {
                throw new Exception("A '" + Method + "' on '" + Controller + "." + Action + "' shouldn't write to the session in the View '" + View + "'");
            }
        }

        FileStream WaitForFile(string fullPath, FileMode mode, FileAccess access, FileShare share)
        {
            for (int numTries = 0; numTries < 3000; numTries++)
            {
                try
                {
                    FileStream fs = new FileStream(fullPath, mode, access, share);

                    fs.ReadByte();
                    fs.Seek(0, SeekOrigin.Begin);

                    return fs;
                }
                catch (IOException)
                {
                    Thread.Sleep(100);
                }
            }

            return null;
        }

        public void Redirect(string url)
        {
            this.RedirectUrl = url;
        }

        private void ReadPostData()
        {
            HttpListenerRequest request = this.context.Request;
            if (request.HasEntityBody)
            {
                StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding);
                NameValueCollection rawParams = HttpUtility.ParseQueryString(reader.ReadToEnd());

                foreach (string key in rawParams.AllKeys)
                {
                    string value = rawParams[key];
                    Post.Add(key, value);
                }
            }

        }

        internal void SendString(string output, string mimeType = "text/html", int StatusCode = 200)
        {
            if (Sent)
            {
                throw new Exception("Output has already been sent");
            }
            if (RedirectUrl != null)
            {
                this.context.Response.StatusCode = 302;
                this.context.Response.AddHeader("Location", RedirectUrl);
                return;
            }
            if (StatusCode != 200) this.context.Response.StatusCode = StatusCode;
            byte[] buf = System.Text.Encoding.UTF8.GetBytes(output);
            this.context.Response.ContentLength64 = buf.Length;
            this.context.Response.OutputStream.Write(buf, 0, buf.Length);
            Sent = true;
        }

    }
}

