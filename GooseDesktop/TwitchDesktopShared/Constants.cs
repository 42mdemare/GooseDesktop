namespace TwitchDesktopShared
{
	internal static class Constants
	{
		public enum GooseTask
		{
			Wander,
			NabMouse,
			CollectWindow_Meme,
			CollectWindow_Notepad,
			CollectWindow_DONOTSET,
			TrackMud,
			Count
		}

		public struct TaskTwitchInfo
		{
			public int code;

			public string ID;

			public string name;

			public string description;

			public string twitchCommandArg;

			public TaskTwitchInfo(int _dbIndex, string taskID, string taskName, string taskDescription, string twitchCommand)
			{
				code = _dbIndex;
				ID = taskID;
				name = taskName;
				description = taskDescription;
				twitchCommandArg = twitchCommand;
			}
		}

		public static class GooseTaskDatabase
		{
			private static TaskTwitchInfo[] twitchTaskDB;

			public static int GetTaskCode(int ledgerIndex)
			{
				return twitchTaskDB[ledgerIndex].code;
			}

			public static string GetTaskName(int ledgerIndex)
			{
				return twitchTaskDB[ledgerIndex].name;
			}

			public static string GetTaskTwitchCommand(int ledgerIndex)
			{
				return twitchTaskDB[ledgerIndex].twitchCommandArg;
			}

			public static int GetLoadedTasksNumber()
			{
				return twitchTaskDB.Length;
			}
		}

		public const int currentVersion = 0;

		public const string MMQName_Twitch = "TwitchMessages";

		public const int MMQ_TwitchSize = 4096;
	}
}
