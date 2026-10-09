using System;

namespace TinhKiemAuto
{
	[Serializable]
	public abstract class ErrorBase
	{
		protected Exception _lasterror;

		public virtual void ClearErrors()
		{
			_lasterror = null;
		}

		public virtual Exception GetLastError()
		{
			return _lasterror;
		}

		protected virtual bool SetLastError(Exception e)
		{
			_lasterror = e;
			return false;
		}

		protected virtual bool SetLastError(string message)
		{
			return SetLastError(new Exception(message));
		}
	}
}
