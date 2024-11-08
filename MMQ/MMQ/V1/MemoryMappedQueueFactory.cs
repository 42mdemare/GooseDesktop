using System;
using System.IO.MemoryMappedFiles;

namespace MMQ.V1
{
	internal sealed class MemoryMappedQueueFactory : IMemoryMappedQueueFactory
	{
		private readonly string _queueName;

		private readonly MemoryMappedFile _file;

		private readonly int _start;

		private readonly int _length;

		public MemoryMappedQueueFactory(string queueName, MemoryMappedFile file, int start, int length)
		{
			_queueName = queueName;
			_file = file;
			_start = start;
			_length = length;
		}

		public IMemoryMappedQueueProducer CreateProducer()
		{
			MemoryMappedViewAccessor memoryMappedViewAccessor = null;
			try
			{
				memoryMappedViewAccessor = _file.CreateViewAccessor(_start, _length);
				return new MemoryMappedQueueProducer(_queueName, _file, memoryMappedViewAccessor);
			}
			catch (Exception)
			{
				memoryMappedViewAccessor?.Dispose();
				throw;
			}
		}

		public IMemoryMappedQueueConsumer CreateConsumer()
		{
			MemoryMappedViewAccessor memoryMappedViewAccessor = null;
			try
			{
				memoryMappedViewAccessor = _file.CreateViewAccessor(_start, _length);
				return new MemoryMappedQueueConsumer(_queueName, _file, memoryMappedViewAccessor);
			}
			catch (Exception)
			{
				memoryMappedViewAccessor?.Dispose();
				throw;
			}
		}
	}
}
