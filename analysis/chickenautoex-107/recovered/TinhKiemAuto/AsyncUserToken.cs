using System;
using System.Net.Sockets;

namespace TinhKiemAuto
{
	public sealed class AsyncUserToken : IDisposable
	{
		public Socket Socket { get; private set; }

		public int? MessageSize { get; set; }

		public int DataStartOffset { get; set; }

		public int NextReceiveOffset { get; set; }

		public AsyncUserToken(Socket socket)
		{
			Socket = socket;
		}

		public void Dispose()
		{
			try
			{
				Socket.Shutdown(SocketShutdown.Send);
			}
			catch (Exception)
			{
			}
			try
			{
				Socket.Close();
			}
			catch (Exception)
			{
			}
		}
	}
}
