using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        public bool Sent;

        public Dictionary<string, string> Post;
        public CookieCollection Cookies;
        public dynamic Data;

        public string SessionIdentifier;
        public TSession Session;

        private HttpListenerContext context;
        private string sessionSavePath;

        private FileStream sessionStream;

        public WebContext(HttpListenerContext context, string sessionSavePath)
        {
            this.context = context;
            this.sessionSavePath = sessionSavePath;

            Method = this.context.Request.HttpMethod;
            Url = this.context.Request.RawUrl;
            Post = new Dictionary<string, string>();
            ReadPostData();

            Data = new System.Dynamic.ExpandoObject();
            Sent = false;
        }

        private String CreateSessionIdentifier()
        {
            Byte[] data = RandomNumberGenerator.GetBytes(18);
            return Convert.ToBase64String(data).Replace("/", "_").Replace("+", "-");
        }

        public void StartSession()
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

            this.sessionStream = WaitForFile(this.sessionSavePath + SessionIdentifier, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

            if (this.sessionStream.Length == 0)
            {
                Session = new TSession();
            }
            else
            {
                byte[] bytes = new byte[this.sessionStream.Length];
                this.sessionStream.Read(bytes, 0, bytes.Length);
                Session = JsonSerializer.Deserialize<TSession>(bytes);
            }
        }

        public void WriteSession()
        {
            this.sessionStream.SetLength(0);
            var bytes = JsonSerializer.SerializeToUtf8Bytes<TSession>(Session);
            this.sessionStream.Write(bytes, 0, bytes.Length);
        }

        public void CloseSession()
        {
            this.sessionStream.Close();
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

        public void SendString(string output, int StatusCode = 200)
        {
            if (!Sent && StatusCode != 200) this.context.Response.StatusCode = StatusCode;
            byte[] buf = System.Text.Encoding.UTF8.GetBytes(output);
            this.context.Response.ContentLength64 = buf.Length;
            this.context.Response.OutputStream.Write(buf, 0, buf.Length);
            Sent = true;
        }

    }
}

