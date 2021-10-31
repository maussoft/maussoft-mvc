using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Collections.Generic;

namespace Maussoft.Mvc
{
	public class StaticServer
	{
		public static Dictionary<String, String> MimeTypes = new Dictionary<String, String>
		{
			// images
			{".bmp", "image/bmp"},
			{".gif", "image/gif"},
			{".jpeg", "image/jpeg"},
			{".jpg", "image/jpeg"},
			{".png", "image/png"},
			{".tif", "image/tiff"},
			{".tiff", "image/tiff"},
			// office
			{".doc", "application/msword"},
			{".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
			{".pdf", "application/pdf"},
			{".ppt", "application/vnd.ms-powerpoint"},
			{".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
			{".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
			{".xls", "application/vnd.ms-excel"},
			// data
			{".csv", "text/csv"},
			{".xml", "text/xml"},
			{".txt", "text/plain"},
			{".zip", "application/zip"},
			// audio
			{".ogg", "application/ogg"},
			{".mp3", "audio/mpeg"},
			{".wma", "audio/x-ms-wma"},
			{".wav", "audio/x-wav"},
			{".wmv", "audio/x-ms-wmv"},
			// video
			{".swf", "application/x-shockwave-flash"},
			{".avi", "video/avi"},
			{".mp4", "video/mp4"},
			{".mpeg", "video/mpeg"},
			{".mpg", "video/mpeg"},
			{".qt", "video/quicktime"},
			// html
			{".html", "text/html"}
		};

		public static string GetContentType(string filename)
		{
			string fileExtension = Path.GetExtension (filename);
			return MimeTypes.ContainsKey(fileExtension) ? MimeTypes[fileExtension] : null;
		}

		public static bool Serve(Assembly assembly, HttpListenerContext context)
		{
			string filename = context.Request.Url.LocalPath;
			filename = Path.Combine("Content:",filename.Substring(1));
			//filename = Path.Combine(Directory.GetCurrentDirectory(),filename);

			using (Stream stream = assembly.GetManifestResourceStream(filename))
			{
				if (stream == null) {
					Console.WriteLine("not found "+filename);
					return false;
				}

				context.Response.Headers.Add("Cache-Control", "max-age=600, public, no-transform");
				context.Response.ContentType = GetContentType (filename);
				
				byte[] buffer = new byte[8192];
				int bytes;
				while ((bytes = stream.Read(buffer, 0, buffer.Length)) > 0) {
					context.Response.OutputStream.Write(buffer, 0, bytes);
				}
				return true;
			}
		}
	}
}