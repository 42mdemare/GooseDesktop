using System;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Threading;

namespace MMQ.V1
{
	internal sealed class MemoryMappedQueueProducer : MemoryMappedQueueAccessor, IMemoryMappedQueueProducer, IDisposable
	{
		private readonly MemoryMappedViewAccessor _accessor;

		private readonly MemoryMappedFile _file;

		private readonly string _queueName;

		private static readonly TimeSpan Timeout;

		static MemoryMappedQueueProducer()
		{
			Timeout = TimeSpan.FromSeconds(10.0);
		}

		public MemoryMappedQueueProducer(string queueName, MemoryMappedFile file, MemoryMappedViewAccessor accessor)
		{
			_queueName = queueName;
			_file = file;
			_accessor = accessor;
		}

		public void Enqueue(byte[] message)
		{
			if (!TryEnqueue(message, Timeout))
			{
				throw new TimeoutException();
			}
		}

		public bool TryEnqueue(byte[] message, TimeSpan timeout)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < int.MaxValue; i++)
			{
				if (stopwatch.Elapsed >= timeout)
				{
					return false;
				}
				using (QueueLock queueLock = new QueueLock(_accessor, _queueName))
				{
					if (queueLock.Acquire(1) && queueLock.AvailableWriteLength >= message.Length)
					{
						queueLock.WriteMessage(message);
						return true;
					}
				}
				int millisecondsTimeout = ((i >= 10) ? ((i < 100) ? 1 : ((i >= 1000) ? 100 : 10)) : 0);
				Thread.Sleep(millisecondsTimeout);
			}
			return false;
		}

		public override void Dispose()
		{
			_accessor.Dispose();
			_file.Dispose();
		}
	}
}
