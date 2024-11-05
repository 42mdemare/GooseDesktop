using GooseShared;
using SamEngine;

namespace GooseDesktop.Refactor.GooseTasks.Tasks
{
	internal class TrackMud : GooseTaskInfo
	{
		public class TrackMudTaskData : GooseTaskData
		{
			public enum Stage
			{
				DecideToRun,
				RunningOffscreen,
				RunningWandering
			}

			public const float DurationToRunAmok = 2f;

			public float nextDirChangeTime;

			public float timeToStopRunning;

			public Stage stage;

			public static float GetDirChangeInterval()
			{
				return 100f;
			}
		}

		public const string TaskID = "TrackMud";

		public TrackMud()
		{
			canBePickedRandomly = true;
			shortName = "Tracking mud";
			description = "The goose runs off the screen, and runs back on leaving MUDDY FOOTPRINTS!";
			taskID = "TrackMud";
		}

		public override GooseTaskData GetNewTaskData(GooseEntity goose)
		{
			return new TrackMudTaskData();
		}

		public override void RunTask(GooseEntity g)
		{
			TrackMudTaskData trackMudTaskData = (TrackMudTaskData)g.currentTaskData;
			switch (trackMudTaskData.stage)
			{
			case TrackMudTaskData.Stage.DecideToRun:
				GooseFunctions.SetTargetOffscreen(g);
				GooseFunctions.SetSpeed(g, GooseEntity.SpeedTiers.Run);
				trackMudTaskData.stage = TrackMudTaskData.Stage.RunningOffscreen;
				break;
			case TrackMudTaskData.Stage.RunningOffscreen:
				if (Vector2.Distance(g.position, g.targetPos) < 5f)
				{
					g.targetPos = new Vector2(SamMath.RandomRange(0f, Program.mainForm.Width), SamMath.RandomRange(0f, Program.mainForm.Height));
					trackMudTaskData.nextDirChangeTime = Time.time + TrackMudTaskData.GetDirChangeInterval();
					trackMudTaskData.timeToStopRunning = Time.time + 2f;
					g.trackMudEndTime = Time.time + g.parameters.DurationToTrackMud;
					trackMudTaskData.stage = TrackMudTaskData.Stage.RunningWandering;
					Sound.PlayMudSquith();
				}
				break;
			case TrackMudTaskData.Stage.RunningWandering:
				if (Vector2.Distance(g.position, g.targetPos) < 5f || Time.time > trackMudTaskData.nextDirChangeTime)
				{
					g.targetPos = new Vector2(SamMath.RandomRange(0f, Program.mainForm.Width), SamMath.RandomRange(0f, Program.mainForm.Height));
					trackMudTaskData.nextDirChangeTime = Time.time + TrackMudTaskData.GetDirChangeInterval();
				}
				if (Time.time > trackMudTaskData.timeToStopRunning)
				{
					g.targetPos = g.position + new Vector2(30f, 3f);
					g.targetPos.x = SamMath.Clamp(g.targetPos.x, 55f, Program.mainForm.Width - 55);
					g.targetPos.y = SamMath.Clamp(g.targetPos.y, 80f, Program.mainForm.Height - 80);
					GooseFunctions.SetTaskByID(g, "Wander", honck: false);
				}
				break;
			}
		}
	}
}
