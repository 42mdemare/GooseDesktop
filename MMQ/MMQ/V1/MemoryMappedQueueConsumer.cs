using System;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Threading;

namespace MMQ.V1
{
	internal sealed class MemoryMappedQueueConsumer : MemoryMappedQueueAccessor, IMemoryMappedQueueConsumer, IDisposable
	{
		private static readonly TimeSpan DefaultTimeout;

		private readonly MemoryMappedViewAccessor _accessor;

		private readonly string _queueName;

		private readonly MemoryMappedFile _file;

		static MemoryMappedQueueConsumer()
		{
			DefaultTimeout = TimeSpan.FromSeconds(10.0);
		}

		public MemoryMappedQueueConsumer(string queueName, MemoryMappedFile file, MemoryMappedViewAccessor accessor)
		{
			_queueName = queueName;
			_file = file;
			_accessor = accessor;
		}

		public byte[] Dequeue()
		{
			return Dequeue(DefaultTimeout);
		}

		public byte[] Dequeue(TimeSpan timeout)
		{
			if (!TryDequeue(out var message, timeout))
			{
				throw new TimeoutException();
			}
			return message;
		}

		public bool TryDequeue(out byte[] message)
		{
			return TryDequeue(out message, DefaultTimeout);
		}

		public bool TryDequeue(out byte[] message, TimeSpan timeout)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < int.MaxValue; i++)
			{
				if (stopwatch.Elapsed >= timeout)
				{
					break;
				}
				using (QueueLock queueLock = new QueueLock(_accessor, _queueName))
				{
					if (queueLock.Acquire(1) && queueLock.AvailableReadLength > 0)
					{
						message = queueLock.ReadMessage();
						return true;
					}
				}
				int millisecondsTimeout = ((i >= 10) ? ((i < 100) ? 1 : ((i >= 1000) ? 100 : 10)) : 0);
				Thread.Sleep(millisecondsTimeout);
			}
			message = null;
			return false;
		}

		public override void Dispose()
		{
			_accessor.Dispose();
			_file.Dispose();
		}
	}
}
