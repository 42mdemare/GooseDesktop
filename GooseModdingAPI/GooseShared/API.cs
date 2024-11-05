namespace GooseShared
{
	public static class API
	{
		public class GooseFunctionPointers
		{
			public delegate void SetSpeedFunction(GooseEntity g, GooseEntity.SpeedTiers tier);

			public delegate ScreenDirection SetTargetOffscreenFunction(GooseEntity g, bool canExitTop = false);

			public delegate bool IsGooseAtTargetFunction(GooseEntity g, float distanceToTrigger);

			public delegate float GetDistanceToTargetFunction(GooseEntity g);

			public delegate void SetCurrentTaskByIDFunction(GooseEntity g, string id, bool honk = true);

			public delegate void ChooseRandomTaskFunction(GooseEntity g);

			public delegate void SetTaskToRoaming(GooseEntity g);

			public delegate void PlayHonckSoundFunction();

			public SetSpeedFunction setSpeed;

			public SetTargetOffscreenFunction setTargetOffscreen;

			public IsGooseAtTargetFunction isGooseAtTarget;

			public GetDistanceToTargetFunction getDistanceToTarget;

			public SetCurrentTaskByIDFunction setCurrentTaskByID;

			public ChooseRandomTaskFunction chooseRandomTask;

			public SetTaskToRoaming setTaskRoaming;

			public PlayHonckSoundFunction playHonckSound;
		}

		public class ModHelperFunctions
		{
			public delegate string GetModDirectoryFunction(IMod mod);

			public GetModDirectoryFunction getModDirectory;
		}

		public class TaskDatabaseQueryFunctions
		{
			public delegate int GetTaskIndexByIDFunction(string id);

			public delegate string[] GetAllLoadedTaskIDsFunction();

			public delegate string GetNextRandomTaskFunction();

			public GetTaskIndexByIDFunction getTaskIndexByID;

			public GetAllLoadedTaskIDsFunction getAllLoadedTaskIDs;

			public GetNextRandomTaskFunction getRandomTaskID;
		}

		public static GooseFunctionPointers Goose;

		public static ModHelperFunctions Helper;

		public static TaskDatabaseQueryFunctions TaskDatabase;
	}
}
