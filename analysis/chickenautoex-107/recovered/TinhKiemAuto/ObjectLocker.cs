using System;
using System.Threading;

namespace TinhKiemAuto
{
	internal class ObjectLocker : IDisposable
	{
		private object obj;

		public ObjectLocker(object obj)
		{
			this.obj = obj;
			Monitor.Enter(this.obj);
		}

		public void Dispose()
		{
			Monitor.Exit(obj);
		}
	}
}
