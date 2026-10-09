using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace TinhKiemAuto
{
	internal static class ProtocolProviderFactory
	{
		private static Hashtable protocolHandlers = new Hashtable();

		[CompilerGenerated]
		public static event EventHandler<ResolvingProtocolProviderEventArgs> ResolvingProtocolProvider;

		public static void RegisterProtocolHandler(string prefix, Type protocolProvider)
		{
			protocolHandlers[prefix] = protocolProvider;
		}

		public static IProtocolProvider CreateProvider(string uri, Downloader downloader)
		{
			IProtocolProvider protocolProvider = InternalGetProvider(uri);
			if (downloader != null)
			{
				protocolProvider.Initialize(downloader);
			}
			return protocolProvider;
		}

		public static IProtocolProvider GetProvider(string uri)
		{
			return InternalGetProvider(uri);
		}

		public static Type GetProviderType(string uri)
		{
			int num = uri.IndexOf("://");
			if (num > 0)
			{
				string key = uri.Substring(0, num);
				return protocolHandlers[key] as Type;
			}
			return null;
		}

		public static IProtocolProvider CreateProvider(Type providerType, Downloader downloader)
		{
			IProtocolProvider protocolProvider = CreateFromType(providerType);
			if (ProtocolProviderFactory.ResolvingProtocolProvider != null)
			{
				ResolvingProtocolProviderEventArgs e = new ResolvingProtocolProviderEventArgs(protocolProvider, null);
				ProtocolProviderFactory.ResolvingProtocolProvider(null, e);
				protocolProvider = e.ProtocolProvider;
			}
			if (downloader != null)
			{
				protocolProvider.Initialize(downloader);
			}
			return protocolProvider;
		}

		private static IProtocolProvider InternalGetProvider(string uri)
		{
			IProtocolProvider protocolProvider = CreateFromType(GetProviderType(uri));
			if (ProtocolProviderFactory.ResolvingProtocolProvider != null)
			{
				ResolvingProtocolProviderEventArgs e = new ResolvingProtocolProviderEventArgs(protocolProvider, uri);
				ProtocolProviderFactory.ResolvingProtocolProvider(null, e);
				protocolProvider = e.ProtocolProvider;
			}
			return protocolProvider;
		}

		private static IProtocolProvider CreateFromType(Type type)
		{
			return (IProtocolProvider)Activator.CreateInstance(type);
		}
	}
}
