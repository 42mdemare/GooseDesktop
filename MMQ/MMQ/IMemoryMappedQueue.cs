using System;

namespace MMQ
{
	public interface IMemoryMappedQueue : IDisposable
	{
		IMemoryMappedQueueProducer CreateProducer();

		IMemoryMappedQueueConsumer CreateConsumer();
	}
}
