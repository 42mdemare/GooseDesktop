using System;

namespace GooseDesktop
{
	internal static class IPCManager
	{
		public static bool ConnectedToTwitch;

		private const int DequeueTimeoutNanoSeconds = 1000;

		private const double loopMS = 150.0;

		public static void Init()
		{
		}

		public static void ProcessMessages(object sender, EventArgs args)
		{
		}
	}
}
