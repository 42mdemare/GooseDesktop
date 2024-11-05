using System;
using System.IO.MemoryMappedFiles;
using System.Threading;

namespace MMQ.V1
{
	internal abstract class MemoryMappedQueueAccessor : IDisposable
	{
		private interface IQueueLock : IDisposable
		{
		}

		internal sealed class QueueLock : IQueueLock, IDisposable
		{
			private const int ReadPointerOffset = 0;

			private const int WritePointerOffset = 4;

			private const int DataOffset = 8;

			private const int MessageStartCookie = 45054;

			private const int MessageEndCookie = 49181;

			private readonly MemoryMappedViewAccessor _accessor;

			private readonly int _length;

			private readonly int _dataLength;

			private readonly Mutex _mutex;

			private bool _acquired;

			private int ReadPointer
			{
				get
				{
					return _accessor.ReadInt32(0L);
				}
				set
				{
					_accessor.Write(0L, value);
				}
			}

			private int WritePointer
			{
				get
				{
					return _accessor.ReadInt32(4L);
				}
				set
				{
					_accessor.Write(4L, value);
				}
			}

			public int AvailableWriteLength
			{
				get
				{
					int readPointer = ReadPointer;
					int writePointer = WritePointer;
					if (readPointer < writePointer)
					{
						return _length - writePointer + readPointer;
					}
					if (writePointer == readPointer)
					{
						return _length;
					}
					return readPointer - writePointer;
				}
			}

			public int AvailableReadLength
			{
				get
				{
					int readPointer = ReadPointer;
					int writePointer = WritePointer;
					if (readPointer < writePointer)
					{
						return writePointer - readPointer;
					}
					if (readPointer == writePointer)
					{
						return 0;
					}
					return _length - readPointer + writePointer;
				}
			}

			public QueueLock(MemoryMappedViewAccessor accessor, string queueName)
			{
				_accessor = accessor;
				_length = (int)accessor.Capacity;
				_dataLength = _length - 8;
				_mutex = new Mutex(initiallyOwned: false, $"{queueName}.Mutex");
			}

			public bool Acquire(int millisecondsTimeout)
			{
				return _acquired = _mutex.WaitOne(millisecondsTimeout);
			}

			public byte[] ReadMessage()
			{
				int readPointer = ReadPointer;
				Read(ref readPointer);
				int length = Read(ref readPointer);
				byte[] result = ReadArray(ref readPointer, length);
				Read(ref readPointer, align: true);
				ReadPointer = readPointer;
				return result;
			}

			private int Read(ref int readPointer, bool align = false)
			{
				int result = _accessor.ReadInt32(8 + readPointer);
				readPointer = (readPointer + 4) % _dataLength;
				if (align)
				{
					int num = 4 - readPointer % 4;
					readPointer += num;
				}
				return result;
			}

			private byte[] ReadArray(ref int readPointer, int length)
			{
				byte[] array = new byte[length];
				int num = _dataLength - readPointer;
				if (num < length)
				{
					_accessor.ReadArray(8 + readPointer, array, 0, num);
					_accessor.ReadArray(8L, array, num, length - num);
					readPointer = 8 + length - num;
				}
				else
				{
					_accessor.ReadArray(8 + readPointer, array, 0, length);
					readPointer += length;
				}
				return array;
			}

			public void WriteMessage(byte[] data)
			{
				int writePointer = WritePointer;
				Write(ref writePointer, 45054);
				Write(ref writePointer, data.Length);
				Write(ref writePointer, data);
				Write(ref writePointer, 49181, align: true);
				WritePointer = writePointer;
			}

			private void Write(ref int writePointer, int value, bool align = false)
			{
				_accessor.Write(8 + writePointer, value);
				writePointer = (writePointer + 4) % _dataLength;
				if (align)
				{
					int num = 4 - writePointer % 4;
					writePointer += num;
				}
			}

			private void Write(ref int writePointer, byte[] data)
			{
				int num = _dataLength - writePointer;
				if (num < data.Length)
				{
					_accessor.WriteArray(8 + writePointer, data, 0, num);
					_accessor.WriteArray(8L, data, num, data.Length - num);
					writePointer = 8 + data.Length - num;
				}
				else
				{
					_accessor.WriteArray(8 + writePointer, data, 0, data.Length);
					writePointer += data.Length;
				}
			}

			public void Dispose()
			{
				if (_acquired)
				{
					_mutex.ReleaseMutex();
				}
				_mutex?.Dispose();
			}
		}

		public abstract void Dispose();
	}
}
