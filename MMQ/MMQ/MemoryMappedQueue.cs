using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using MMQ.V1;

namespace MMQ
{
	public static class MemoryMappedQueue
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct Header
		{
			public const long MagicCookieValue = 3735944941L;

			public const int Size = 24;

			public long Cookie1;

			public int FileLength;

			public uint Version;

			public long Cookie2;

			public bool Verify()
			{
				if (Cookie1 != 3735944941u)
				{
					return false;
				}
				if (FileLength <= 24)
				{
					return false;
				}
				if (Cookie2 != 3735944941u)
				{
					return false;
				}
				return true;
			}
		}

		public unsafe static IMemoryMappedQueue Create(string name, int bufferSize = 65535)
		{
			MemoryMappedFile memoryMappedFile = null;
			try
			{
				memoryMappedFile = MemoryMappedFile.CreateNew(name, bufferSize);
				Header header = default(Header);
				header.Cookie1 = 3735944941L;
				header.Version = 1u;
				header.FileLength = bufferSize;
				header.Cookie2 = 3735944941L;
				Header structure = header;
				using (MemoryMappedViewAccessor memoryMappedViewAccessor = memoryMappedFile.CreateViewAccessor())
				{
					byte* pointer = null;
					memoryMappedViewAccessor.SafeMemoryMappedViewHandle.AcquirePointer(ref pointer);
					for (int i = 0; i < bufferSize; i++)
					{
						pointer[i] = 0;
					}
					memoryMappedViewAccessor.Write(0L, ref structure);
				}
				return new MMQ.V1.MemoryMappedQueue(name, memoryMappedFile);
			}
			catch (Exception)
			{
				memoryMappedFile?.Dispose();
				throw;
			}
		}

		public static IMemoryMappedQueueProducer CreateProducer(string name)
		{
			MemoryMappedFile memoryMappedFile = null;
			try
			{
				memoryMappedFile = MemoryMappedFile.OpenExisting(name);
				return CreateFactory(name, memoryMappedFile).CreateProducer();
			}
			catch (Exception)
			{
				memoryMappedFile?.Dispose();
				throw;
			}
		}

		public static IMemoryMappedQueueConsumer CreateConsumer(string name)
		{
			MemoryMappedFile memoryMappedFile = null;
			try
			{
				memoryMappedFile = MemoryMappedFile.OpenExisting(name);
				return CreateFactory(name, memoryMappedFile).CreateConsumer();
			}
			catch (Exception)
			{
				memoryMappedFile?.Dispose();
				throw;
			}
		}

		private static IMemoryMappedQueueFactory CreateFactory(string name, MemoryMappedFile file)
		{
			int num = 24;
			using MemoryMappedViewAccessor memoryMappedViewAccessor = file.CreateViewAccessor(0L, num);
			memoryMappedViewAccessor.Read<Header>(0L, out var structure);
			if (!structure.Verify())
			{
				throw new InvalidOperationException($"The given name '{name}' does not point to a valid memory mapped queue. This may be because the name is wrong, the queue's memory has been overwritten by another application or due to a bug in this library.");
			}
			int length = structure.FileLength - num;
			uint version = structure.Version;
			if (version == 1)
			{
				return new MemoryMappedQueueFactory(name, file, num, length);
			}
			throw new NotSupportedException($"MMQ of version '{structure.Version}' is not supported!");
		}
	}
}
