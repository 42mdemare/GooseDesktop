using GooseShared;

namespace GooseDesktop.Refactor.GooseTasks.Tasks
{
	internal class DemoTask : GooseTaskInfo
	{
		public class DemoTaskData : GooseTaskData
		{
			public float thisIsAFloat;

			public float gooseXPositionOnTaskStart;
		}

		public const string TaskID = "DemoTask";

		private const float ThisFloatDefaultValue = 4.53f;

		public DemoTask()
		{
			canBePickedRandomly = false;
			shortName = "Demo task - don't use?";
			description = "This is a demo task that does nothing useful!";
			taskID = "DemoTask";
		}

		public override GooseTaskData GetNewTaskData(GooseEntity goose)
		{
			return new DemoTaskData
			{
				thisIsAFloat = 4.53f,
				gooseXPositionOnTaskStart = goose.position.x
			};
		}

		public override void RunTask(GooseEntity goose)
		{
			((DemoTaskData)goose.currentTaskData).thisIsAFloat += 1f;
		}
	}
}
