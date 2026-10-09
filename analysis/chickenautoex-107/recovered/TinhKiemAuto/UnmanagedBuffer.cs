using System;
using System.Runtime.InteropServices;

namespace TinhKiemAuto
{
	[Serializable]
	public class UnmanagedBuffer : ErrorBase, IDisposable
	{
		private bool _disposed;

		public IntPtr Pointer { get; private set; }

		public int Size { get; private set; }

		public UnmanagedBuffer(int cbneeded)
		{
			if (cbneeded > 0)
			{
				Pointer = Marshal.AllocHGlobal(cbneeded);
				Size = cbneeded;
			}
			else
			{
				Pointer = IntPtr.Zero;
				Size = 0;
			}
		}

		private bool Alloc(int cb)
		{
			try
			{
				if (cb > Size)
				{
					Pointer = ((Pointer == IntPtr.Zero) ? Marshal.AllocHGlobal(cb) : Marshal.ReAllocHGlobal(Pointer, new IntPtr(cb)));
					Size = cb;
				}
				return true;
			}
			catch (Exception lastError)
			{
				return SetLastError(lastError);
			}
		}

		public bool Commit<T>(T data) where T : struct
		{
			try
			{
				if (Alloc(Marshal.SizeOf(typeof(T))))
				{
					Marshal.StructureToPtr(data, Pointer, fDeleteOld: false);
					return true;
				}
				return false;
			}
			catch (Exception lastError)
			{
				return SetLastError(lastError);
			}
		}

		public bool Commit(byte[] data, int index, int count)
		{
			if (data != null && Alloc(count))
			{
				Marshal.Copy(data, index, Pointer, count);
				return true;
			}
			if (data == null)
			{
				SetLastError(new ArgumentException("Attempting to commit a null reference", "data"));
			}
			return false;
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					Resize(0);
				}
				_disposed = true;
			}
		}

		public byte[] Read(int count)
		{
			try
			{
				if (count > Size || count <= 0)
				{
					throw new ArgumentException("There is either not enough memory allocated to read 'count' bytes, or 'count' is negative (" + count + ")", "count");
				}
				byte[] array = new byte[count];
				Marshal.Copy(Pointer, array, 0, count);
				return array;
			}
			catch (Exception lastError)
			{
				SetLastError(lastError);
				return null;
			}
		}

		public bool Read<TResult>(out TResult data) where TResult : struct
		{
			data = default(TResult);
			try
			{
				if (Size < Marshal.SizeOf(typeof(TResult)))
				{
					throw new InvalidCastException("Not enough unmanaged memory is allocated to contain this structure type.");
				}
				data = (TResult)Marshal.PtrToStructure(Pointer, typeof(TResult));
				return true;
			}
			catch (Exception lastError)
			{
				return SetLastError(lastError);
			}
		}

		public bool Resize(int size)
		{
			if (size < 0)
			{
				return SetLastError(new ArgumentException("Attempting to resize to less than zero bytes of memory", "size"));
			}
			if (size == Size)
			{
				return true;
			}
			if (size > Size)
			{
				return Alloc(size);
			}
			try
			{
				if (size == 0)
				{
					Marshal.FreeHGlobal(Pointer);
					Pointer = IntPtr.Zero;
				}
				else if (size > 0)
				{
					Pointer = Marshal.ReAllocHGlobal(Pointer, new IntPtr(size));
				}
				Size = size;
				return true;
			}
			catch (Exception lastError)
			{
				return SetLastError(lastError);
			}
		}

		public bool SafeDecommit<T>() where T : struct
		{
			try
			{
				if (Size < Marshal.SizeOf(typeof(T)))
				{
					throw new InvalidCastException("Not enough unmanaged memory is allocated to contain this structure type.");
				}
				Marshal.DestroyStructure(Pointer, typeof(T));
				return true;
			}
			catch (Exception lastError)
			{
				return SetLastError(lastError);
			}
		}

		public bool Translate<TSource>(TSource data, out byte[] buffer) where TSource : struct
		{
			buffer = null;
			if (Commit(data))
			{
				buffer = Read(Marshal.SizeOf(typeof(TSource)));
				SafeDecommit<TSource>();
			}
			return buffer != null;
		}

		public bool Translate<TResult>(byte[] buffer, out TResult result) where TResult : struct
		{
			result = default(TResult);
			if (buffer == null)
			{
				return SetLastError(new ArgumentException("Attempted to translate a null reference to a structure.", "buffer"));
			}
			if (Commit(buffer, 0, buffer.Length))
			{
				return Read(out result);
			}
			return false;
		}

		public bool Translate<TSource, TResult>(TSource data, out TResult result) where TSource : struct where TResult : struct
		{
			result = default(TResult);
			if (Commit(data) && Read(out result))
			{
				return SafeDecommit<TSource>();
			}
			return false;
		}
	}
}
