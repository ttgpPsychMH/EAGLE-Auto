using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace TinhKiemAuto
{
	[Serializable]
	public class MemoryIterator : ErrorBase, IDisposable
	{
		private MemoryStream _base;

		private bool _disposed;

		private UnmanagedBuffer _ubuffer;

		public MemoryIterator(byte[] iterable)
		{
			if (iterable == null)
			{
				throw new ArgumentException("Unable to iterate a null reference", "iterable");
			}
			_base = new MemoryStream(iterable, 0, iterable.Length, writable: true);
			_ubuffer = new UnmanagedBuffer(256);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					_ubuffer.Dispose();
					_base.Dispose();
				}
				_disposed = true;
			}
		}

		protected byte[] GetUnderlyingData()
		{
			return _base.ToArray();
		}

		public bool Read<TResult>(out TResult result) where TResult : struct
		{
			return Read(0L, SeekOrigin.Current, out result);
		}

		public bool Read(long offset, SeekOrigin origin, byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", "Parameter cannot be null");
			}
			try
			{
				_base.Seek(offset, origin);
				_base.Read(buffer, 0, buffer.Length);
			}
			catch (Exception lastError)
			{
				SetLastError(lastError);
				buffer = null;
			}
			return buffer != null;
		}

		public bool Read<TResult>(long offset, SeekOrigin origin, out TResult result) where TResult : struct
		{
			result = default(TResult);
			try
			{
				_base.Seek(offset, origin);
				byte[] array = new byte[Marshal.SizeOf(typeof(TResult))];
				_base.Read(array, 0, array.Length);
				if (!_ubuffer.Translate(array, out result))
				{
					throw _ubuffer.GetLastError();
				}
				return true;
			}
			catch (Exception lastError)
			{
				return base.SetLastError(lastError);
			}
		}

		public bool ReadString(long offset, SeekOrigin origin, out string lpBuffer, int len = -1, Encoding stringEncoding = null)
		{
			lpBuffer = null;
			byte[] array = new byte[(len > 0) ? len : 64];
			if (stringEncoding == null)
			{
				stringEncoding = Encoding.ASCII;
			}
			try
			{
				_base.Seek(offset, origin);
				StringBuilder stringBuilder = new StringBuilder((len > 0) ? len : 260);
				int num = -1;
				int num2 = 0;
				int num3;
				while (num == -1 && (num3 = _base.Read(array, 0, array.Length)) > 0)
				{
					stringBuilder.Append(stringEncoding.GetString(array));
					num = stringBuilder.ToString().IndexOf('\0', num2);
					num2 += num3;
					if (len > 0 && num2 >= len)
					{
						break;
					}
				}
				if (num > -1)
				{
					lpBuffer = stringBuilder.ToString().Substring(0, num);
				}
				else if (num2 >= len && len > 0)
				{
					lpBuffer = stringBuilder.ToString().Substring(0, len);
				}
				return lpBuffer != null;
			}
			catch (Exception lastError)
			{
				return SetLastError(lastError);
			}
		}

		public long Seek(long offset, SeekOrigin origin)
		{
			return _base.Seek(offset, origin);
		}

		public bool Write(long offset, SeekOrigin origin, byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("Parameter 'data' cannot be null");
			}
			try
			{
				_base.Seek(offset, origin);
				_base.Write(data, 0, data.Length);
				return true;
			}
			catch (Exception lastError)
			{
				return SetLastError(lastError);
			}
		}

		public bool Write<TSource>(long offset, SeekOrigin origin, TSource data) where TSource : struct
		{
			try
			{
				_base.Seek(offset, origin);
				byte[] buffer = null;
				if (!_ubuffer.Translate(data, out buffer))
				{
					throw _ubuffer.GetLastError();
				}
				_base.Write(buffer, 0, buffer.Length);
				return true;
			}
			catch (Exception lastError)
			{
				return SetLastError(lastError);
			}
		}
	}
}
