using System;
using System.IO;
using System.Net;
using System.Net.Configuration;
using System.Net.Security;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows.Forms;

namespace TinhKiemAuto
{
	internal class Poster
	{
		private delegate void CallBack();

		public string Url = string.Empty;

		public string Data = string.Empty;

		public string Referer = string.Empty;

		public string HTML = string.Empty;

		public string Error = string.Empty;

		public string CookieToString = string.Empty;

		public string Heads = string.Empty;

		public Control Control;

		public CookieContainer Cookie = new CookieContainer();

		public static int ErrorCount;

		private HttpWebRequest request;

		public static string ErrorUrl = "";

		private static string UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:6.0a2) Gecko/20110613 Firefox/6.0a2";

		public bool AutoReconnect { get; set; }

		public bool IsError => Error != string.Empty;

		public string Response
		{
			get
			{
				if (IsError)
				{
					return Error;
				}
				return HTML;
			}
			set
			{
				HTML = value;
			}
		}

		[CompilerGenerated]
		public event EventHandler Completed;

		public void Post()
		{
			ChangeHost();
			Thread thread = new Thread(PostThread);
			thread.IsBackground = true;
			thread.Start();
		}

		private void ChangeHost()
		{
		}

		public void Get()
		{
			ChangeHost();
			Thread thread = new Thread(GetThread);
			thread.IsBackground = true;
			thread.Start();
		}

		public void Abort()
		{
			try
			{
				request.Abort();
			}
			catch
			{
			}
		}

		private HttpWebRequest RequestGet(string url, CookieContainer cookie)
		{
			request = (HttpWebRequest)WebRequest.Create(url);
			request.CookieContainer = cookie;
			request.Method = "GET";
			request.UserAgent = UserAgent;
			request.Headers.Add("Accept-Encoding: *");
			request.Connection = "keepalive";
			request.Proxy = null;
			request.ServicePoint.Expect100Continue = false;
			request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			return request;
		}

		private HttpWebRequest RequestPost(string url, string data, CookieContainer cookie, string referer)
		{
			request = (HttpWebRequest)WebRequest.Create(url);
			try
			{
				request.Referer = referer;
			}
			catch
			{
			}
			request.ServicePoint.Expect100Continue = false;
			request.AllowAutoRedirect = false;
			request.CookieContainer = cookie;
			request.Method = "POST";
			request.UserAgent = UserAgent;
			request.Headers.Add("Accept-Encoding: *");
			request.Accept = "*/*";
			request.Connection = "keepalive";
			request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			request.Proxy = null;
			request.ContentType = "application/x-www-form-urlencoded";
			StreamWriter streamWriter = new StreamWriter(request.GetRequestStream());
			streamWriter.Write(data);
			streamWriter.Close();
			return request;
		}

		private void PostThread()
		{
			Data = Data + "&serial=" + FingerPrint.Serial + "&version=" + Global.Version + "&li=&md=" + Global.SelfMd5 + "&vip=" + Global.IsVIP;
			try
			{
				PostFunc();
			}
			catch (Exception ex)
			{
				if (AutoReconnect)
				{
					while (true)
					{
						try
						{
							PostFunc();
							break;
						}
						catch
						{
							Error = ex.Message;
							ErrorCount++;
							ErrorUrl = Url;
							ChangeHost();
						}
					}
				}
				else
				{
					Error = ex.Message;
					ErrorCount++;
					ErrorUrl = Url;
				}
			}
			OnCompleted();
		}

		private void PostFunc()
		{
			HttpWebRequest httpWebRequest = RequestPost(Url, Data, Cookie, Referer);
			GetResponse(this, httpWebRequest);
		}

		private void GetThread()
		{
			try
			{
				HttpWebRequest httpWebRequest = RequestGet(Url, Cookie);
				GetResponse(this, httpWebRequest);
			}
			catch (Exception ex)
			{
				Error = ex.Message;
				ErrorCount++;
				ErrorUrl = Url;
			}
			OnCompleted();
		}

		private void OnCompleted()
		{
			try
			{
				if (!Control.IsDisposed && this.Completed != null)
				{
					if (Control.InvokeRequired)
					{
						Control.Invoke(new CallBack(OnCompleted));
					}
					else
					{
						this.Completed(this, null);
					}
				}
			}
			catch
			{
			}
		}

		public static void DisableValidate()
		{
			if (!Global.IsFix)
			{
				ServicePointManager.DefaultConnectionLimit = int.MaxValue;
				SetAllowUnsafeHeaderParsing20();
				ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(BypassAllCertificateStuff));
			}
		}

		public static bool SetAllowUnsafeHeaderParsing20()
		{
			Assembly assembly = Assembly.GetAssembly(typeof(SettingsSection));
			if (assembly != null)
			{
				Type type = assembly.GetType("System.Net.Configuration.SettingsSectionInternal");
				if (type != null)
				{
					object obj = type.InvokeMember("Section", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty, null, null, new object[0]);
					if (obj != null)
					{
						FieldInfo field = type.GetField("useUnsafeHeaderParsing", BindingFlags.Instance | BindingFlags.NonPublic);
						if (field != null)
						{
							field.SetValue(obj, true);
							return true;
						}
					}
				}
			}
			return false;
		}

		private static bool BypassAllCertificateStuff(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
		{
			return true;
		}

		private static Poster GetResponse(Poster poster, HttpWebRequest request)
		{
			HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();
			foreach (Cookie cookie in httpWebResponse.Cookies)
			{
				string text = cookie.Domain.TrimStart('.').Replace("www.", "");
				poster.Cookie.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path, text));
				poster.Cookie.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path, "www." + text));
				poster.CookieToString = poster.CookieToString + cookie.Name + "=" + cookie.Value + ";";
			}
			for (int i = 0; i < httpWebResponse.Headers.Count; i++)
			{
				poster.Heads = poster.Heads + httpWebResponse.Headers.Keys[i] + ": " + httpWebResponse.Headers[i] + "\n";
			}
			StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
			poster.HTML = streamReader.ReadToEnd();
			streamReader.Close();
			return poster;
		}
	}
}
