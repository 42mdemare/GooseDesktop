using GooseShared;
using SamEngine;

namespace GooseDesktop.Refactor.GooseTasks.Tasks
{
	public class Wander : GooseTaskInfo
	{
		public class WanderTaskData : GooseTaskData
		{
			public float wanderingStartTime;

			public float wanderingDuration;

			public float pauseStartTime;

			public float pauseDuration;
		}

		public const string TaskID = "Wander";

		private const float MinPauseTime = 1f;

		private const float MaxPauseTime = 2f;

		public const float GoodEnoughDistance = 20f;

		public Wander()
		{
			canBePickedRandomly = false;
			shortName = "Wandering";
			description = "Just the goose's wandering around, default state.";
			taskID = "Wander";
		}

		public override GooseTaskData GetNewTaskData(GooseEntity goose)
		{
			WanderTaskData wanderTaskData = new WanderTaskData();
			GooseFunctions.SetSpeed(goose, GooseEntity.SpeedTiers.Walk);
			wanderTaskData.pauseStartTime = -1f;
			wanderTaskData.wanderingStartTime = Time.time;
			wanderTaskData.wanderingDuration = GetRandomWanderDuration();
			return wanderTaskData;
		}

		public override void RunTask(GooseEntity goose)
		{
			WanderTaskData wanderTaskData = (WanderTaskData)goose.currentTaskData;
			if (!IPCManager.ConnectedToTwitch && Time.time - wanderTaskData.wanderingStartTime > wanderTaskData.wanderingDuration)
			{
				GooseFunctions.ChooseRandomTask(goose);
			}
			else if (wanderTaskData.pauseStartTime > 0f)
			{
				if (Time.time - wanderTaskData.pauseStartTime > wanderTaskData.pauseDuration)
				{
					wanderTaskData.pauseStartTime = -1f;
					float num = GetRandomWalkTime() * goose.currentSpeed;
					goose.targetPos = new Vector2(SamMath.RandomRange(0f, Program.mainForm.Width), SamMath.RandomRange(0f, Program.mainForm.Height));
					if (Vector2.Distance(goose.position, goose.targetPos) > num)
					{
						goose.targetPos = goose.position + Vector2.Normalize(goose.targetPos - goose.position) * num;
					}
				}
				else
				{
					goose.velocity = Vector2.zero;
				}
			}
			else if (Vector2.Distance(goose.position, goose.targetPos) < 20f)
			{
				wanderTaskData.pauseStartTime = Time.time;
				wanderTaskData.pauseDuration = GetRandomPauseDuration();
			}
		}

		private static float GetRandomPauseDuration()
		{
			return 1f + (float)SamMath.Rand.NextDouble() * 1f;
		}

		private static float GetRandomWanderDuration()
		{
			if (Time.time < 1f)
			{
				return GooseConfig.settings.FirstWanderTimeSeconds;
			}
			return SamMath.RandomRange(GooseConfig.settings.MinWanderingTimeSeconds, GooseConfig.settings.MaxWanderingTimeSeconds);
		}

		private static float GetRandomWalkTime()
		{
			return SamMath.RandomRange(1f, 6f);
		}
	}
}
