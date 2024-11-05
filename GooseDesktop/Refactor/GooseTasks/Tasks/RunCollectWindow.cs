using System.Threading;
using System.Windows.Forms;
using GooseDesktop.Refactor.CustomFormTypes;
using GooseShared;
using SamEngine;

namespace GooseDesktop.Refactor.GooseTasks.Tasks
{
	internal class RunCollectWindow : GooseTaskInfo
	{
		public class CollectWindowTaskData : GooseTaskData
		{
			public enum Stage
			{
				WalkingOffscreen,
				WaitingToBringWindowBack,
				DraggingWindowBack
			}

			public MovableForm mainForm;

			public Stage stage;

			public float secsToWait;

			public float waitStartTime;

			public ScreenDirection screenDirection;

			public Vector2 windowOffsetToBeak;

			public static float GetWaitTime()
			{
				return SamMath.RandomRange(2f, 3.5f);
			}
		}

		public const string TaskID = "";

		private const float ThisFloatDefaultValue = 4.53f;

		public RunCollectWindow()
		{
			canBePickedRandomly = false;
			shortName = "Internal task. Don't use.";
			description = "This is an internal 'task' that handles all cases of collecting windows. Cannot be called on its own.";
			taskID = "";
		}

		public override GooseTaskData GetNewTaskData(GooseEntity goose)
		{
			MessageBox.Show("ERROR: BASE WINDOW TASK DATA GETTING CALLED WITHOUT ANY OTHER CONTEXT.");
			return null;
		}

		public void SetupScreenTargetAndBeakOffset(CollectWindowTaskData data, GooseEntity goose)
		{
			data.screenDirection = GooseFunctions.SetTargetOffscreen(goose);
			switch (data.screenDirection)
			{
			case ScreenDirection.Left:
				data.windowOffsetToBeak = new Vector2(data.mainForm.Width, data.mainForm.Height / 2);
				break;
			case ScreenDirection.Right:
				data.windowOffsetToBeak = new Vector2(0f, data.mainForm.Height / 2);
				break;
			case ScreenDirection.Top:
				data.windowOffsetToBeak = new Vector2(data.mainForm.Width / 2, data.mainForm.Height);
				break;
			}
		}

		public override void RunTask(GooseEntity goose)
		{
			if (goose.currentTaskData == null)
			{
				MessageBox.Show("Cannot run CollectWindow task without specifying a window type.", "CollectWindow Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				GooseFunctions.SetTaskDefault(goose);
				return;
			}
			CollectWindowTaskData taskData = (CollectWindowTaskData)goose.currentTaskData;
			switch (taskData.stage)
			{
			case CollectWindowTaskData.Stage.WalkingOffscreen:
				if (Vector2.Distance(goose.position, goose.targetPos) < 5f)
				{
					taskData.secsToWait = CollectWindowTaskData.GetWaitTime();
					taskData.waitStartTime = Time.time;
					taskData.stage = CollectWindowTaskData.Stage.WaitingToBringWindowBack;
				}
				break;
			case CollectWindowTaskData.Stage.WaitingToBringWindowBack:
				if (Time.time - taskData.waitStartTime > taskData.secsToWait)
				{
					taskData.mainForm.FormClosing += OnGiftClosed;
					new Thread((ThreadStart)delegate
					{
						taskData.mainForm.ShowDialog();
					}).Start();
					switch (taskData.screenDirection)
					{
					case ScreenDirection.Left:
						goose.targetPos.y = SamMath.Lerp(goose.position.y, Program.mainForm.Height / 2, SamMath.RandomRange(0.2f, 0.3f));
						goose.targetPos.x = (float)taskData.mainForm.Width + SamMath.RandomRange(15f, 20f);
						break;
					case ScreenDirection.Top:
						goose.targetPos.y = (float)taskData.mainForm.Height + SamMath.RandomRange(80f, 100f);
						goose.targetPos.x = SamMath.Lerp(goose.position.x, Program.mainForm.Width / 2, SamMath.RandomRange(0.2f, 0.3f));
						break;
					case ScreenDirection.Right:
						goose.targetPos.y = SamMath.Lerp(goose.position.y, Program.mainForm.Height / 2, SamMath.RandomRange(0.2f, 0.3f));
						goose.targetPos.x = (float)Program.mainForm.Width - ((float)taskData.mainForm.Width + SamMath.RandomRange(20f, 30f));
						break;
					}
					goose.targetPos.x = SamMath.Clamp(goose.targetPos.x, taskData.mainForm.Width + 55, Program.mainForm.Width - (taskData.mainForm.Width + 55));
					goose.targetPos.y = SamMath.Clamp(goose.targetPos.y, taskData.mainForm.Height + 80, Program.mainForm.Height);
					taskData.stage = CollectWindowTaskData.Stage.DraggingWindowBack;
				}
				break;
			case CollectWindowTaskData.Stage.DraggingWindowBack:
				if (Vector2.Distance(goose.position, goose.targetPos) < 5f)
				{
					goose.targetPos = goose.position + Vector2.GetFromAngleDegrees(goose.direction + 180f) * 40f;
					GooseFunctions.SetTaskByID(goose, "Wander");
				}
				else
				{
					goose.extendingNeck = true;
					goose.targetDirection = goose.targetPos - goose.position;
					taskData.mainForm.SetWindowPositionThreadsafe(RenderFuncs.ToIntPoint(goose.rig.head2EndPoint - taskData.windowOffsetToBeak));
				}
				break;
			}
		}

		public static void OnGiftClosed(object sender, FormClosingEventArgs args)
		{
			GooseEntity ownerGoose = ((MovableForm)sender).ownerGoose;
			if (GooseConfig.settings.Task_CanAttackMouse)
			{
				GooseFunctions.SetTaskByID(ownerGoose, "NabMouse");
			}
			else if (ownerGoose.currentTaskData is CollectWindowTaskData && ((CollectWindowTaskData)ownerGoose.currentTaskData).mainForm == (MovableForm)sender)
			{
				GooseFunctions.SetTaskByID(ownerGoose, "Wander");
			}
		}
	}
}
