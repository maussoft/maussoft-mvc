using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Text;
using System.Web;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization.Formatters.Binary;

namespace Maussoft.Mvc
{
	public class WebContext
	{  
		public string Url;
		public string Controller;
		public string Action;
		public string View;
		public bool Sent;

		public Dictionary<string, string> Post;
		public CookieCollection Cookies;

		public Dictionary<string, object> Data;

		public string SessionIdentifier;
		public Dictionary<string, object> Session;

		private HttpListenerContext _context;
		private string _sessionSavePath;

		private FileStream _sessionStream;

		public WebContext(HttpListenerContext context, string sessionSavePath)
		{
			_context = context;
			_sessionSavePath = sessionSavePath;

			Url = _context.Request.RawUrl;
			Post = new Dictionary<string, string>();
			ReadPostData ();

			Data = new Dictionary<string, object>();
		}

		public void StartSession()
		{
			Cookie cookie = _context.Request.Cookies ["Maussoft.Mvc"];
			if (cookie == null) {
				SessionIdentifier = System.Guid.NewGuid ().ToString();
				cookie = new Cookie ("Maussoft.Mvc", SessionIdentifier);
				_context.Response.AppendCookie (cookie);
			} else {
				SessionIdentifier = cookie.Value;
			}

			_sessionStream = WaitForFile (_sessionSavePath + SessionIdentifier, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

			if (_sessionStream.Length == 0) {
				Session = new Dictionary<string,object> ();
			} else {
				BinaryFormatter bin = new BinaryFormatter ();
				Session = (Dictionary<string,object>)bin.Deserialize (_sessionStream);
			}
		}

		public void WriteSession()
		{
			_sessionStream.SetLength (0);
			BinaryFormatter bin = new BinaryFormatter();
			bin.Serialize(_sessionStream, Session);
		}

		public void CloseSession()
		{
			_sessionStream.Close ();
		}

		FileStream WaitForFile (string fullPath, FileMode mode, FileAccess access, FileShare share)
		{
			for (int numTries = 0; numTries < 3000; numTries++) {
				try {
					FileStream fs = new FileStream (fullPath, mode, access, share);

					fs.ReadByte ();
					fs.Seek (0, SeekOrigin.Begin);

					return fs;
				}
				catch (IOException) {
					Thread.Sleep (100);
				}
			}

			return null;
		}

		private void ReadPostData()
		{
			HttpListenerRequest request = _context.Request;
			if (request.HasEntityBody)
			{
				StreamReader reader = new StreamReader (request.InputStream, request.ContentEncoding);
				NameValueCollection rawParams = HttpUtility.ParseQueryString (reader.ReadToEnd());

				foreach(string key in rawParams.AllKeys)
				{
					string value = rawParams[key];
					Post.Add(key, value);
				}
			}

		}

		public void SendString(string output,int StatusCode=200)
		{  
			if (!Sent && StatusCode!=200) _context.Response.StatusCode = StatusCode;
			byte[] buf = System.Text.Encoding.UTF8.GetBytes(output);
			_context.Response.ContentLength64 = buf.Length;
			_context.Response.OutputStream.Write(buf, 0, buf.Length);
			Sent = true;
		} 

	}
}

