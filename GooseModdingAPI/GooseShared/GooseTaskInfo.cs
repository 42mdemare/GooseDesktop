namespace GooseShared
{
	public abstract class GooseTaskInfo
	{
		public bool canBePickedRandomly = true;

		public string shortName = "Human-readable name here";

		public string description = "The person who programmed this forgot to give this task a description!";

		public string taskID = "";

		public abstract GooseTaskData GetNewTaskData(GooseEntity s);

		public abstract void RunTask(GooseEntity s);
	}
}
