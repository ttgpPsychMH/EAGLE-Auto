using System;

namespace TinhKiemAuto
{
	internal class ResolvingProtocolProviderEventArgs : EventArgs
	{
		private IProtocolProvider provider;

		private string url;

		public string URL => url;

		public IProtocolProvider ProtocolProvider
		{
			get
			{
				return provider;
			}
			set
			{
				provider = value;
			}
		}

		public ResolvingProtocolProviderEventArgs(IProtocolProvider provider, string url)
		{
			this.url = url;
			this.provider = provider;
		}
	}
}
