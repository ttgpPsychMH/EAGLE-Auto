using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TinhKiemAuto
{
	[Serializable]
	internal class ResourceLocation
	{
		private string url;

		private bool authenticate;

		private string login;

		private string password;

		private Type protocolProviderType;

		private IProtocolProvider provider;

		public string URL
		{
			get
			{
				return url;
			}
			set
			{
				url = value;
				BindProtocolProviderType();
			}
		}

		public bool Authenticate
		{
			get
			{
				return authenticate;
			}
			set
			{
				authenticate = value;
			}
		}

		public string Login
		{
			get
			{
				return login;
			}
			set
			{
				login = value;
			}
		}

		public string Password
		{
			get
			{
				return password;
			}
			set
			{
				password = value;
			}
		}

		public string ProtocolProviderType
		{
			get
			{
				if (protocolProviderType == null)
				{
					return null;
				}
				return protocolProviderType.AssemblyQualifiedName;
			}
			set
			{
				if (value == null)
				{
					BindProtocolProviderType();
				}
				else
				{
					protocolProviderType = Type.GetType(value);
				}
			}
		}

		public static ResourceLocation FromURL(string url)
		{
			return new ResourceLocation
			{
				URL = url
			};
		}

		public static ResourceLocation[] FromURLArray(string[] urls)
		{
			List<ResourceLocation> list = new List<ResourceLocation>();
			for (int i = 0; i < urls.Length; i++)
			{
				if (IsURL(urls[i]))
				{
					list.Add(FromURL(urls[i]));
				}
			}
			return list.ToArray();
		}

		public static ResourceLocation FromURL(string url, bool authenticate, string login, string password)
		{
			return new ResourceLocation
			{
				URL = url,
				Authenticate = authenticate,
				Login = login,
				Password = password
			};
		}

		public IProtocolProvider GetProtocolProvider(Downloader downloader)
		{
			return BindProtocolProviderInstance(downloader);
		}

		public void BindProtocolProviderType()
		{
			provider = null;
			if (!string.IsNullOrEmpty(URL))
			{
				protocolProviderType = ProtocolProviderFactory.GetProviderType(URL);
			}
		}

		public IProtocolProvider BindProtocolProviderInstance(Downloader downloader)
		{
			if (protocolProviderType == null)
			{
				BindProtocolProviderType();
			}
			if (provider == null)
			{
				provider = ProtocolProviderFactory.CreateProvider(protocolProviderType, downloader);
			}
			return provider;
		}

		public ResourceLocation Clone()
		{
			return (ResourceLocation)MemberwiseClone();
		}

		public override string ToString()
		{
			return URL;
		}

		public static bool IsURL(string url)
		{
			return Regex.Match(url, "(?<Protocol>\\w+):\\/\\/(?<Domain>[\\w.]+\\/?)\\S*").ToString() != string.Empty;
		}
	}
}
