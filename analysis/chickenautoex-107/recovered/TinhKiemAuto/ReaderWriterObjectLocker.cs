using System;
using System.Threading;

namespace TinhKiemAuto
{
	internal class ReaderWriterObjectLocker
	{
		private class BaseReleaser
		{
			protected ReaderWriterObjectLocker locker;

			public BaseReleaser(ReaderWriterObjectLocker locker)
			{
				this.locker = locker;
			}
		}

		private class ReaderReleaser : BaseReleaser, IDisposable
		{
			public ReaderReleaser(ReaderWriterObjectLocker locker)
				: base(locker)
			{
			}

			public void Dispose()
			{
				locker.locker.ReleaseReaderLock();
			}
		}

		private class WriterReleaser : BaseReleaser, IDisposable
		{
			public WriterReleaser(ReaderWriterObjectLocker locker)
				: base(locker)
			{
			}

			public void Dispose()
			{
				locker.locker.ReleaseWriterLock();
			}
		}

		private ReaderWriterLock locker;

		private IDisposable writerReleaser;

		private IDisposable readerReleaser;

		public ReaderWriterObjectLocker()
		{
			locker = new ReaderWriterLock();
			writerReleaser = new WriterReleaser(this);
			readerReleaser = new ReaderReleaser(this);
		}

		public IDisposable LockForRead()
		{
			locker.AcquireReaderLock(-1);
			return readerReleaser;
		}

		public IDisposable LockForWrite()
		{
			locker.AcquireWriterLock(-1);
			return writerReleaser;
		}
	}
}
