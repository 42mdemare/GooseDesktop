using System.IO.MemoryMappedFiles;

namespace TwitchDesktopShared
{
	internal static class Helpers
	{
		public static bool MessageQueueExists(string name)
		{
			try
			{
				MemoryMappedFile.OpenExisting("TwitchMessages");
			}
			catch
			{
				return false;
			}
			return true;
		}
	}
}
