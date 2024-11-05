using System;

namespace MMQ
{
	public interface IMemoryMappedQueueProducer : IDisposable
	{
		void Enqueue(byte[] message);

		bool TryEnqueue(byte[] message, TimeSpan timeout);
	}
}
