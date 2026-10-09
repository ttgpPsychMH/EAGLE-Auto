using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ProtoBuf.Serializers;
using TinhKiemAuto.AutoControl;
using TinhKiemAuto.Models;

namespace TinhKiemAuto
{
	public sealed class SocketClient : IDisposable
	{
		private int bufferSize = 60000;

		private const int MessageHeaderSize = 4;

		private Socket clientSocket;

		public bool connected;

		private IPEndPoint hostEndPoint;

		private AutoResetEvent autoConnectEvent;

		private AutoResetEvent autoSendEvent;

		private SocketAsyncEventArgs sendEventArgs;

		private SocketAsyncEventArgs receiveEventArgs;

		public SocketClient(IPEndPoint hostEndPoint)
		{
			this.hostEndPoint = hostEndPoint;
			autoConnectEvent = new AutoResetEvent(initialState: false);
			autoSendEvent = new AutoResetEvent(initialState: false);
			clientSocket = new Socket(this.hostEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			sendEventArgs = new SocketAsyncEventArgs();
			sendEventArgs.UserToken = clientSocket;
			sendEventArgs.RemoteEndPoint = this.hostEndPoint;
			sendEventArgs.Completed += OnSend;
			receiveEventArgs = new SocketAsyncEventArgs();
			receiveEventArgs.UserToken = new AsyncUserToken(clientSocket);
			receiveEventArgs.RemoteEndPoint = this.hostEndPoint;
			receiveEventArgs.SetBuffer(new byte[bufferSize], 0, bufferSize);
			receiveEventArgs.Completed += OnReceive;
		}

		public void Connect()
		{
			SocketAsyncEventArgs e = new SocketAsyncEventArgs();
			e.UserToken = clientSocket;
			e.RemoteEndPoint = hostEndPoint;
			e.Completed += OnConnect;
			clientSocket.ConnectAsync(e);
			autoConnectEvent.WaitOne();
			if (e.SocketError == SocketError.Success && !clientSocket.ReceiveAsync(receiveEventArgs))
			{
				ProcessReceive(receiveEventArgs);
			}
		}

		public void SendData(byte[] message)
		{
			Send(BuildMessage(message));
		}

		public static byte[] BuildMessage(byte[] data)
		{
			byte[] array = DataHelper.MAHOA(data, "e9b3390206d8dfc5ffc9b09284c0bbde");
			byte[] bytes = BitConverter.GetBytes(array.Length);
			byte[] array2 = new byte[bytes.Length + array.Length];
			bytes.CopyTo(array2, 0);
			array.CopyTo(array2, bytes.Length);
			return array2;
		}

		public void Disconnect()
		{
			clientSocket.Disconnect(reuseSocket: false);
		}

		public void Send(byte[] message)
		{
			if (message != null)
			{
				sendEventArgs.SetBuffer(message, 0, message.Length);
				clientSocket.SendAsync(sendEventArgs);
				autoSendEvent.WaitOne();
			}
		}

		private void OnConnect(object sender, SocketAsyncEventArgs e)
		{
			autoConnectEvent.Set();
			connected = e.SocketError == SocketError.Success;
			_ = connected;
		}

		private void OnSend(object sender, SocketAsyncEventArgs e)
		{
			autoSendEvent.Set();
		}

		private void OnReceive(object sender, SocketAsyncEventArgs e)
		{
			ProcessReceive(e);
		}

		private void ProcessReceive(SocketAsyncEventArgs e)
		{
			if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
			{
				AsyncUserToken asyncUserToken = e.UserToken as AsyncUserToken;
				ProcessReceivedData(asyncUserToken.DataStartOffset, asyncUserToken.NextReceiveOffset - asyncUserToken.DataStartOffset + e.BytesTransferred, 0, asyncUserToken, e);
				asyncUserToken.NextReceiveOffset += e.BytesTransferred;
				if (asyncUserToken.NextReceiveOffset == e.Buffer.Length)
				{
					asyncUserToken.NextReceiveOffset = 0;
					if (asyncUserToken.DataStartOffset < e.Buffer.Length)
					{
						int num = e.Buffer.Length - asyncUserToken.DataStartOffset;
						Buffer.BlockCopy(e.Buffer, asyncUserToken.DataStartOffset, e.Buffer, 0, num);
						asyncUserToken.NextReceiveOffset = num;
					}
					asyncUserToken.DataStartOffset = 0;
				}
				e.SetBuffer(asyncUserToken.NextReceiveOffset, e.Buffer.Length - asyncUserToken.NextReceiveOffset);
				if (!asyncUserToken.Socket.ReceiveAsync(e))
				{
					ProcessReceive(e);
				}
			}
			else
			{
				ProcessError(e);
			}
		}

		private void ProcessReceivedData(int dataStartOffset, int totalReceivedDataSize, int alreadyProcessedDataSize, AsyncUserToken token, SocketAsyncEventArgs e)
		{
			if (alreadyProcessedDataSize >= totalReceivedDataSize)
			{
				return;
			}
			if (!token.MessageSize.HasValue)
			{
				if (totalReceivedDataSize > 4)
				{
					byte[] array = new byte[4];
					Buffer.BlockCopy(e.Buffer, dataStartOffset, array, 0, 4);
					int value = BitConverter.ToInt32(array, 0);
					token.MessageSize = value;
					token.DataStartOffset = dataStartOffset + 4;
					ProcessReceivedData(token.DataStartOffset, totalReceivedDataSize, alreadyProcessedDataSize + 4, token, e);
				}
				return;
			}
			int value2 = token.MessageSize.Value;
			if (totalReceivedDataSize - alreadyProcessedDataSize >= value2)
			{
				byte[] array2 = new byte[value2];
				Buffer.BlockCopy(e.Buffer, dataStartOffset, array2, 0, value2);
				ProcessMessage(array2);
				token.DataStartOffset = dataStartOffset + value2;
				token.MessageSize = null;
				ProcessReceivedData(token.DataStartOffset, totalReceivedDataSize, alreadyProcessedDataSize + value2, token, e);
			}
		}

		private void ProcessMessage(byte[] messageData)
		{
			byte[] TMPDATA = DataHelper.GIAIMA(messageData, "e9b3390206d8dfc5ffc9b09284c0bbde");
			if (TMPDATA.Length != 0)
			{
				Thread thread = new Thread((ThreadStart)delegate
				{
					DoWork(TMPDATA);
				});
				thread.IsBackground = true;
				thread.Start();
			}
		}

		public void DoWork(byte[] dataBuf)
		{
			try
			{
				PacketDef packetDef = new PacketDef();
				packetDef = DataHelper.BytesToObject<PacketDef>(dataBuf, 0, dataBuf.Length);
				if (packetDef.IDPacket == 1000)
				{
					LoginProsecc(packetDef.data);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, ex.ToString());
			}
		}

		public void LoginProsecc(byte[] dataBuf)
		{
			PacketSend packetSend = DataHelper.BytesToObject<PacketSend>(dataBuf, 0, dataBuf.Length);
			string hardwareID = packetSend.HardwareID;
			Console.Write("NHẬN ĐƯỢC TỪ PHÍA MÁY CHỦ :" + hardwareID);
			foreach (KeyValuePair<string, AutoReport> item in packetSend.DanhSachGame)
			{
				Global.SetInfo(item.Value);
			}
		}

		private void ProcessError(SocketAsyncEventArgs e)
		{
			if (e.UserToken is Socket socket && socket.Connected)
			{
				try
				{
					socket.Shutdown(SocketShutdown.Both);
				}
				catch (Exception)
				{
				}
				finally
				{
					if (socket.Connected)
					{
						socket.Close();
					}
				}
			}
			connected = false;
		}

		public void Dispose()
		{
			autoConnectEvent.Close();
			if (clientSocket.Connected)
			{
				clientSocket.Close();
			}
		}
	}
}
