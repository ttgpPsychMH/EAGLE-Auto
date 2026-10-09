using System;
using System.IO;

namespace TinhKiemAuto
{
	internal class Segment
	{
		private long startPosition;

		private int index;

		private string currentURL;

		private long initialStartPosition;

		private long endPosition;

		private Stream outputStream;

		private Stream inputStream;

		private Exception lastError;

		private SegmentState state;

		private bool started;

		private DateTime lastReception = DateTime.MinValue;

		private DateTime lastErrorDateTime = DateTime.MinValue;

		private double rate;

		private long start;

		private TimeSpan left = TimeSpan.Zero;

		private int currentTry;

		public int CurrentTry
		{
			get
			{
				return currentTry;
			}
			set
			{
				currentTry = value;
			}
		}

		public SegmentState State
		{
			get
			{
				return state;
			}
			set
			{
				state = value;
				switch (state)
				{
				case SegmentState.Connecting:
				case SegmentState.Paused:
				case SegmentState.Finished:
				case SegmentState.Error:
					rate = 0.0;
					left = TimeSpan.Zero;
					break;
				case SegmentState.Downloading:
					BeginWork();
					break;
				}
			}
		}

		public DateTime LastErrorDateTime => lastErrorDateTime;

		public Exception LastError
		{
			get
			{
				return lastError;
			}
			set
			{
				if (value != null)
				{
					lastErrorDateTime = DateTime.Now;
				}
				else
				{
					lastErrorDateTime = DateTime.MinValue;
				}
				lastError = value;
			}
		}

		public int Index
		{
			get
			{
				return index;
			}
			set
			{
				index = value;
			}
		}

		public long InitialStartPosition
		{
			get
			{
				return initialStartPosition;
			}
			set
			{
				initialStartPosition = value;
			}
		}

		public long StartPosition
		{
			get
			{
				return startPosition;
			}
			set
			{
				startPosition = value;
			}
		}

		public long Transfered => StartPosition - InitialStartPosition;

		public long TotalToTransfer
		{
			get
			{
				if (EndPosition > 0)
				{
					return EndPosition - InitialStartPosition;
				}
				return 0L;
			}
		}

		public long MissingTransfer
		{
			get
			{
				if (EndPosition > 0)
				{
					return EndPosition - StartPosition;
				}
				return 0L;
			}
		}

		public double Progress
		{
			get
			{
				if (EndPosition > 0)
				{
					return (double)Transfered / (double)TotalToTransfer * 100.0;
				}
				return 0.0;
			}
		}

		public long EndPosition
		{
			get
			{
				return endPosition;
			}
			set
			{
				endPosition = value;
			}
		}

		public Stream OutputStream
		{
			get
			{
				return outputStream;
			}
			set
			{
				outputStream = value;
			}
		}

		public Stream InputStream
		{
			get
			{
				return inputStream;
			}
			set
			{
				inputStream = value;
			}
		}

		public string CurrentURL
		{
			get
			{
				return currentURL;
			}
			set
			{
				currentURL = value;
			}
		}

		public double Rate
		{
			get
			{
				if (State == SegmentState.Downloading)
				{
					IncreaseStartPosition(0L);
					return rate;
				}
				return 0.0;
			}
		}

		public TimeSpan Left => left;

		public void BeginWork()
		{
			start = startPosition;
			lastReception = DateTime.Now;
			started = true;
		}

		public void IncreaseStartPosition(long size)
		{
			lock (this)
			{
				DateTime now = DateTime.Now;
				startPosition += size;
				if (started)
				{
					TimeSpan timeSpan = now - lastReception;
					if (timeSpan.TotalSeconds != 0.0)
					{
						rate = (double)(startPosition - start) / timeSpan.TotalSeconds;
						if (rate > 0.0)
						{
							left = TimeSpan.FromSeconds((double)MissingTransfer / rate);
						}
						else
						{
							left = TimeSpan.MaxValue;
						}
					}
				}
				else
				{
					start = startPosition;
					lastReception = now;
					started = true;
				}
			}
		}
	}
}
