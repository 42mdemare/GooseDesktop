using System;

namespace MMQ
{
	public interface IMemoryMappedQueueConsumer : IDisposable
	{
		byte[] Dequeue();

		byte[] Dequeue(TimeSpan timeout);

		bool TryDequeue(out byte[] message);

		bool TryDequeue(out byte[] message, TimeSpan timeout);
	}
}
