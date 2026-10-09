using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace TinhKiemAuto
{
	internal class Downloader
	{
		private string localFile;

		private int requestedSegmentCount;

		private ResourceLocation resourceLocation;

		private List<ResourceLocation> mirrors;

		private List<Segment> segments;

		private Thread mainThread;

		private List<Thread> threads;

		private RemoteFileInfo remoteFileInfo;

		private DownloaderState state;

		private DateTime createdDateTime;

		private Exception lastError;

		private Dictionary<string, object> extentedProperties = new Dictionary<string, object>();

		private IProtocolProvider defaultDownloadProvider;

		private ISegmentCalculator segmentCalculator;

		private IMirrorSelector mirrorSelector;

		private string statusMessage;

		public Dictionary<string, object> ExtendedProperties => extentedProperties;

		public ResourceLocation ResourceLocation => resourceLocation;

		public List<ResourceLocation> Mirrors => mirrors;

		public long FileSize
		{
			get
			{
				if (remoteFileInfo == null)
				{
					return 0L;
				}
				return remoteFileInfo.FileSize;
			}
		}

		public DateTime CreatedDateTime => createdDateTime;

		public int RequestedSegments => requestedSegmentCount;

		public string LocalFile => localFile;

		public int Percent => (int)Progress;

		public double Progress
		{
			get
			{
				int count = segments.Count;
				if (count > 0)
				{
					double num = 0.0;
					for (int i = 0; i < count; i++)
					{
						num += segments[i].Progress;
					}
					return num / (double)count;
				}
				return 0.0;
			}
		}

		public double Rate
		{
			get
			{
				double num = 0.0;
				for (int i = 0; i < segments.Count; i++)
				{
					num += segments[i].Rate;
				}
				return num;
			}
		}

		public long Transfered
		{
			get
			{
				long num = 0L;
				for (int i = 0; i < segments.Count; i++)
				{
					num += segments[i].Transfered;
				}
				return num;
			}
		}

		public TimeSpan Left
		{
			get
			{
				if (Rate == 0.0)
				{
					return TimeSpan.MaxValue;
				}
				double num = 0.0;
				for (int i = 0; i < segments.Count; i++)
				{
					num += (double)segments[i].MissingTransfer;
				}
				return TimeSpan.FromSeconds(num / Rate);
			}
		}

		public List<Segment> Segments => segments;

		public Exception LastError
		{
			get
			{
				return lastError;
			}
			set
			{
				lastError = value;
			}
		}

		public DownloaderState State => state;

		public RemoteFileInfo RemoteFileInfo => remoteFileInfo;

		public string StatusMessage
		{
			get
			{
				return statusMessage;
			}
			set
			{
				statusMessage = value;
			}
		}

		public ISegmentCalculator SegmentCalculator
		{
			get
			{
				return segmentCalculator;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				segmentCalculator = value;
			}
		}

		public IMirrorSelector MirrorSelector
		{
			get
			{
				return mirrorSelector;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				mirrorSelector = value;
				mirrorSelector.Init(this);
			}
		}

		[CompilerGenerated]
		public event EventHandler Ending;

		[CompilerGenerated]
		public event EventHandler InfoReceived;

		[CompilerGenerated]
		public event EventHandler StateChanged;

		[CompilerGenerated]
		public event EventHandler<SegmentEventArgs> RestartingSegment;

		[CompilerGenerated]
		public event EventHandler<SegmentEventArgs> SegmentStoped;

		[CompilerGenerated]
		public event EventHandler<SegmentEventArgs> SegmentStarting;

		[CompilerGenerated]
		public event EventHandler<SegmentEventArgs> SegmentStarted;

		[CompilerGenerated]
		public event EventHandler<SegmentEventArgs> SegmentFailed;

		private Downloader(ResourceLocation rl, ResourceLocation[] mirrors, string localFile)
		{
			threads = new List<Thread>();
			resourceLocation = rl;
			if (mirrors == null)
			{
				this.mirrors = new List<ResourceLocation>();
			}
			else
			{
				this.mirrors = new List<ResourceLocation>(mirrors);
			}
			this.localFile = localFile;
			extentedProperties = new Dictionary<string, object>();
			defaultDownloadProvider = rl.BindProtocolProviderInstance(this);
			segmentCalculator = new MinSizeSegmentCalculator();
			MirrorSelector = new SequentialMirrorSelector();
		}

		public Downloader(ResourceLocation rl, ResourceLocation[] mirrors, string localFile, int segmentCount)
			: this(rl, mirrors, localFile)
		{
			SetState(DownloaderState.NeedToPrepare);
			createdDateTime = DateTime.Now;
			requestedSegmentCount = segmentCount;
			segments = new List<Segment>();
		}

		public Downloader(ResourceLocation rl, ResourceLocation[] mirrors, string localFile, List<Segment> segments, RemoteFileInfo remoteInfo, int requestedSegmentCount, DateTime createdDateTime)
			: this(rl, mirrors, localFile)
		{
			if (segments.Count > 0)
			{
				SetState(DownloaderState.Prepared);
			}
			else
			{
				SetState(DownloaderState.NeedToPrepare);
			}
			this.createdDateTime = createdDateTime;
			remoteFileInfo = remoteInfo;
			this.requestedSegmentCount = requestedSegmentCount;
			this.segments = segments;
		}

		public bool IsWorking()
		{
			DownloaderState downloaderState = State;
			if (downloaderState != DownloaderState.Preparing && downloaderState != DownloaderState.WaitingForReconnect)
			{
				return downloaderState == DownloaderState.Working;
			}
			return true;
		}

		private void SetState(DownloaderState value)
		{
			state = value;
			OnStateChanged();
		}

		private void StartToPrepare()
		{
			mainThread = new Thread(StartDownloadThreadProc);
			mainThread.IsBackground = true;
			mainThread.Start(requestedSegmentCount);
		}

		private void StartPrepared()
		{
			mainThread = new Thread(RestartDownload);
			mainThread.Start();
		}

		protected virtual void OnRestartingSegment(Segment segment)
		{
			if (this.RestartingSegment != null)
			{
				this.RestartingSegment(this, new SegmentEventArgs(this, segment));
			}
		}

		protected virtual void OnSegmentStoped(Segment segment)
		{
			if (this.SegmentStoped != null)
			{
				this.SegmentStoped(this, new SegmentEventArgs(this, segment));
			}
		}

		protected virtual void OnSegmentFailed(Segment segment)
		{
			if (this.SegmentFailed != null)
			{
				this.SegmentFailed(this, new SegmentEventArgs(this, segment));
			}
		}

		protected virtual void OnSegmentStarting(Segment segment)
		{
			if (this.SegmentStarting != null)
			{
				this.SegmentStarting(this, new SegmentEventArgs(this, segment));
			}
		}

		protected virtual void OnSegmentStarted(Segment segment)
		{
			if (this.SegmentStarted != null)
			{
				this.SegmentStarted(this, new SegmentEventArgs(this, segment));
			}
		}

		protected virtual void OnStateChanged()
		{
			if (this.StateChanged != null)
			{
				this.StateChanged(this, EventArgs.Empty);
			}
		}

		protected virtual void OnEnding()
		{
			if (this.Ending != null)
			{
				this.Ending(this, EventArgs.Empty);
			}
		}

		protected virtual void OnInfoReceived()
		{
			if (this.InfoReceived != null)
			{
				this.InfoReceived(this, EventArgs.Empty);
			}
		}

		public IDisposable LockSegments()
		{
			return new ObjectLocker(segments);
		}

		public void WaitForConclusion()
		{
			if (!IsWorking() && mainThread != null && mainThread.IsAlive)
			{
				mainThread.Join(TimeSpan.FromSeconds(1.0));
			}
			while (IsWorking())
			{
				Thread.Sleep(100);
			}
		}

		public void Pause()
		{
			if (state == DownloaderState.Preparing || state == DownloaderState.WaitingForReconnect)
			{
				Segments.Clear();
				mainThread.Abort();
				mainThread = null;
				SetState(DownloaderState.NeedToPrepare);
			}
			else if (state == DownloaderState.Working)
			{
				SetState(DownloaderState.Pausing);
				while (!AllWorkersStopped(5))
				{
				}
				lock (threads)
				{
					threads.Clear();
				}
				mainThread.Abort();
				mainThread = null;
				if (RemoteFileInfo != null && !RemoteFileInfo.AcceptRanges)
				{
					Segments[0].StartPosition = 0L;
				}
				SetState(DownloaderState.Paused);
			}
		}

		public void Start()
		{
			if (state == DownloaderState.NeedToPrepare)
			{
				SetState(DownloaderState.Preparing);
				StartToPrepare();
			}
			else if (state != DownloaderState.Preparing && state != DownloaderState.Pausing && state != DownloaderState.Working && state != DownloaderState.WaitingForReconnect)
			{
				SetState(DownloaderState.Preparing);
				StartPrepared();
			}
		}

		private void AllocLocalFile()
		{
			FileInfo fileInfo = new FileInfo(LocalFile);
			if (!Directory.Exists(fileInfo.DirectoryName))
			{
				Directory.CreateDirectory(fileInfo.DirectoryName);
			}
			if (fileInfo.Exists)
			{
				int num = 1;
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(LocalFile);
				string extension = Path.GetExtension(LocalFile);
				string path;
				do
				{
					path = PathHelper.GetWithBackslash(fileInfo.DirectoryName) + fileNameWithoutExtension + $"({num++})" + extension;
				}
				while (File.Exists(path));
				localFile = path;
			}
			using (FileStream fileStream = new FileStream(LocalFile, FileMode.Create, FileAccess.Write))
			{
				fileStream.SetLength(Math.Max(FileSize, 0L));
			}
		}

		private void StartDownloadThreadProc(object objSegmentCount)
		{
			SetState(DownloaderState.Preparing);
			int segmentCount = Math.Min((int)objSegmentCount, DownloadSettings.MaxSegments);
			Stream stream = null;
			int num = 0;
			while (true)
			{
				lastError = null;
				if (state != DownloaderState.Pausing)
				{
					SetState(DownloaderState.Preparing);
					num++;
					try
					{
						remoteFileInfo = defaultDownloadProvider.GetFileInfo(ResourceLocation, out stream);
					}
					catch (ThreadAbortException)
					{
						SetState(DownloaderState.NeedToPrepare);
						return;
					}
					catch (Exception ex2)
					{
						lastError = ex2;
						if (num < DownloadSettings.MaxRetries)
						{
							SetState(DownloaderState.WaitingForReconnect);
							Thread.Sleep(TimeSpan.FromSeconds(DownloadSettings.RetryDelay));
							continue;
						}
						SetState(DownloaderState.NeedToPrepare);
						return;
					}
					break;
				}
				SetState(DownloaderState.NeedToPrepare);
				return;
			}
			try
			{
				lastError = null;
				StartSegments(segmentCount, stream);
			}
			catch (ThreadAbortException)
			{
				throw;
			}
			catch (Exception ex4)
			{
				lastError = ex4;
				SetState(DownloaderState.EndedWithError);
			}
		}

		private void StartSegments(int segmentCount, Stream inputStream)
		{
			OnInfoReceived();
			AllocLocalFile();
			CalculatedSegment[] array = (remoteFileInfo.AcceptRanges ? SegmentCalculator.GetSegments(segmentCount, remoteFileInfo) : new CalculatedSegment[1]
			{
				new CalculatedSegment(0L, remoteFileInfo.FileSize)
			});
			lock (threads)
			{
				threads.Clear();
			}
			lock (segments)
			{
				segments.Clear();
			}
			for (int i = 0; i < array.Length; i++)
			{
				Segment segment = new Segment();
				if (i == 0)
				{
					segment.InputStream = inputStream;
				}
				segment.Index = i;
				segment.InitialStartPosition = array[i].StartPosition;
				segment.StartPosition = array[i].StartPosition;
				segment.EndPosition = array[i].EndPosition;
				segments.Add(segment);
			}
			RunSegments();
		}

		private void RestartDownload()
		{
			int num = 0;
			RemoteFileInfo fileInfo;
			Stream stream;
			try
			{
				while (true)
				{
					lastError = null;
					SetState(DownloaderState.Preparing);
					num++;
					try
					{
						fileInfo = defaultDownloadProvider.GetFileInfo(ResourceLocation, out stream);
					}
					catch (Exception ex)
					{
						lastError = ex;
						if (num >= DownloadSettings.MaxRetries)
						{
							return;
						}
						SetState(DownloaderState.WaitingForReconnect);
						Thread.Sleep(TimeSpan.FromSeconds(DownloadSettings.RetryDelay));
						continue;
					}
					break;
				}
			}
			finally
			{
				SetState(DownloaderState.Prepared);
			}
			try
			{
				if (!fileInfo.AcceptRanges || fileInfo.LastModified > RemoteFileInfo.LastModified || fileInfo.FileSize != RemoteFileInfo.FileSize)
				{
					remoteFileInfo = fileInfo;
					StartSegments(RequestedSegments, stream);
				}
				else
				{
					stream?.Dispose();
					RunSegments();
				}
			}
			catch (ThreadAbortException)
			{
				throw;
			}
			catch (Exception ex3)
			{
				lastError = ex3;
				SetState(DownloaderState.EndedWithError);
			}
		}

		private void RunSegments()
		{
			SetState(DownloaderState.Working);
			using (FileStream outputStream = new FileStream(LocalFile, FileMode.Open, FileAccess.Write))
			{
				for (int i = 0; i < Segments.Count; i++)
				{
					Segments[i].OutputStream = outputStream;
					StartSegment(Segments[i]);
				}
				while (!AllWorkersStopped(1000) || RestartFailedSegments())
				{
				}
			}
			for (int j = 0; j < Segments.Count; j++)
			{
				if (Segments[j].State == SegmentState.Error)
				{
					SetState(DownloaderState.EndedWithError);
					return;
				}
			}
			if (State != DownloaderState.Pausing)
			{
				OnEnding();
			}
			SetState(DownloaderState.Ended);
		}

		private bool RestartFailedSegments()
		{
			bool result = false;
			double num = 0.0;
			for (int i = 0; i < Segments.Count; i++)
			{
				if (Segments[i].State == SegmentState.Error && Segments[i].LastErrorDateTime != DateTime.MinValue && (DownloadSettings.MaxRetries == 0 || Segments[i].CurrentTry < DownloadSettings.MaxRetries))
				{
					result = true;
					TimeSpan timeSpan = DateTime.Now - Segments[i].LastErrorDateTime;
					if (timeSpan.TotalSeconds >= (double)DownloadSettings.RetryDelay)
					{
						Segments[i].CurrentTry++;
						StartSegment(Segments[i]);
						OnRestartingSegment(Segments[i]);
					}
					else
					{
						num = Math.Max(num, (double)(DownloadSettings.RetryDelay * 1000) - timeSpan.TotalMilliseconds);
					}
				}
			}
			Thread.Sleep((int)num);
			return result;
		}

		private void StartSegment(Segment newSegment)
		{
			Thread thread = new Thread(SegmentThreadProc);
			thread.IsBackground = true;
			thread.Start(newSegment);
			lock (threads)
			{
				threads.Add(thread);
			}
		}

		private bool AllWorkersStopped(int timeOut)
		{
			bool flag = true;
			Thread[] array;
			lock (threads)
			{
				array = threads.ToArray();
			}
			Thread[] array2 = array;
			foreach (Thread thread in array2)
			{
				bool flag2 = thread.Join(timeOut);
				flag = flag && flag2;
				if (flag2)
				{
					lock (threads)
					{
						threads.Remove(thread);
					}
				}
			}
			return flag;
		}

		private void SegmentThreadProc(object objSegment)
		{
			Segment segment = (Segment)objSegment;
			segment.LastError = null;
			try
			{
				if (segment.EndPosition > 0 && segment.StartPosition >= segment.EndPosition)
				{
					segment.State = SegmentState.Finished;
					OnSegmentStoped(segment);
					return;
				}
				int num = 8192;
				byte[] buffer = new byte[num];
				segment.State = SegmentState.Connecting;
				OnSegmentStarting(segment);
				if (segment.InputStream == null)
				{
					ResourceLocation nextResourceLocation = MirrorSelector.GetNextResourceLocation();
					IProtocolProvider protocolProvider = nextResourceLocation.BindProtocolProviderInstance(this);
					while (nextResourceLocation != ResourceLocation)
					{
						Stream stream;
						RemoteFileInfo fileInfo = protocolProvider.GetFileInfo(nextResourceLocation, out stream);
						stream?.Dispose();
						if (fileInfo.FileSize == remoteFileInfo.FileSize && fileInfo.AcceptRanges == remoteFileInfo.AcceptRanges)
						{
							break;
						}
						lock (mirrors)
						{
							mirrors.Remove(nextResourceLocation);
						}
						nextResourceLocation = MirrorSelector.GetNextResourceLocation();
						protocolProvider = nextResourceLocation.BindProtocolProviderInstance(this);
					}
					segment.InputStream = protocolProvider.CreateStream(nextResourceLocation, segment.StartPosition, segment.EndPosition);
					segment.CurrentURL = nextResourceLocation.URL;
				}
				else
				{
					segment.CurrentURL = resourceLocation.URL;
				}
				using (segment.InputStream)
				{
					OnSegmentStarted(segment);
					segment.State = SegmentState.Downloading;
					segment.CurrentTry = 0;
					long num2;
					do
					{
						num2 = segment.InputStream.Read(buffer, 0, num);
						if (segment.EndPosition > 0 && segment.StartPosition + num2 > segment.EndPosition)
						{
							num2 = segment.EndPosition - segment.StartPosition;
							if (num2 <= 0)
							{
								segment.StartPosition = segment.EndPosition;
								break;
							}
						}
						lock (segment.OutputStream)
						{
							segment.OutputStream.Position = segment.StartPosition;
							segment.OutputStream.Write(buffer, 0, (int)num2);
						}
						segment.IncreaseStartPosition(num2);
						if (segment.EndPosition <= 0 || segment.StartPosition < segment.EndPosition)
						{
							if (state == DownloaderState.Pausing)
							{
								segment.State = SegmentState.Paused;
								break;
							}
							continue;
						}
						segment.StartPosition = segment.EndPosition;
						break;
					}
					while (num2 > 0);
					if (segment.State == SegmentState.Downloading)
					{
						segment.State = SegmentState.Finished;
						AddNewSegmentIfNeeded();
					}
				}
				OnSegmentStoped(segment);
			}
			catch (Exception ex)
			{
				segment.State = SegmentState.Error;
				segment.LastError = ex;
				OnSegmentFailed(segment);
			}
			finally
			{
				segment.InputStream = null;
			}
		}

		private void AddNewSegmentIfNeeded()
		{
			lock (segments)
			{
				for (int i = 0; i < segments.Count; i++)
				{
					Segment segment = segments[i];
					if (segment.State == SegmentState.Downloading && segment.Left.TotalSeconds > (double)DownloadSettings.MinSegmentLeftToStartNewSegment && segment.MissingTransfer / 2 >= DownloadSettings.MinSegmentSize)
					{
						long num = segment.MissingTransfer / 2;
						Segment segment2 = new Segment();
						segment2.Index = segments.Count;
						segment2.StartPosition = segment.StartPosition + num;
						segment2.InitialStartPosition = segment2.StartPosition;
						segment2.EndPosition = segment.EndPosition;
						segment2.OutputStream = segment.OutputStream;
						segment.EndPosition -= num;
						segments.Add(segment2);
						StartSegment(segment2);
						break;
					}
				}
			}
		}
	}
}
