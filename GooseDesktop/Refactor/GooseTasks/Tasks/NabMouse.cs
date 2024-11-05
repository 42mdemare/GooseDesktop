using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GooseShared;
using SamEngine;

namespace GooseDesktop.Refactor.GooseTasks.Tasks
{
	internal class NabMouse : GooseTaskInfo
	{
		public class NabMouseData : GooseTaskData
		{
			public enum Stage
			{
				SeekingMouse,
				DraggingMouseAway,
				Decelerating
			}

			public Stage currentStage;

			public Vector2 dragToPoint;

			public float grabbedOriginalTime;

			public float chaseStartTime;

			public Vector2 originalVectorToMouse;

			public const float MouseGrabDistance = 15f;

			public const float MouseSuccTime = 0.06f;

			public const float MouseDropDistance = 30f;

			public const float MinRunTime = 2f;

			public const float MaxRunTime = 4f;

			public const float GiveUpTime = 9f;

			public static readonly Vector2 StruggleRange = new Vector2(3f, 3f);
		}

		public const string TaskID = "NabMouse";

		private const float ThisFloatDefaultValue = 4.53f;

		private static Rectangle tmpRect;

		private static Size tmpSize;

		public NabMouse()
		{
			canBePickedRandomly = GooseConfig.settings.AttackRandomly;
			shortName = "Steal the mouse";
			description = "Make the goose try and steal your mouse!";
			taskID = "NabMouse";
		}

		public override GooseTaskData GetNewTaskData(GooseEntity goose)
		{
			return new NabMouseData
			{
				chaseStartTime = Time.time
			};
		}

		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow(IntPtr hWnd);

		public override void RunTask(GooseEntity g)
		{
			NabMouseData nabMouseData = (NabMouseData)g.currentTaskData;
			Vector2 vector = new Vector2(Cursor.Position.X, Cursor.Position.Y);
			Vector2 head2EndPoint = g.rig.head2EndPoint;
			if (nabMouseData.currentStage == NabMouseData.Stage.SeekingMouse)
			{
				GooseFunctions.SetSpeed(g, GooseEntity.SpeedTiers.Charge);
				g.targetPos = vector - (g.rig.head2EndPoint - g.position);
				if (Vector2.Distance(head2EndPoint, vector) < 15f)
				{
					nabMouseData.originalVectorToMouse = vector - head2EndPoint;
					nabMouseData.grabbedOriginalTime = Time.time;
					nabMouseData.dragToPoint = g.position;
					while (Vector2.Distance(nabMouseData.dragToPoint, g.position) / g.parameters.ChargeSpeed < 1.2f)
					{
						nabMouseData.dragToPoint = new Vector2((float)SamMath.Rand.NextDouble() * (float)Program.mainForm.Width, (float)SamMath.Rand.NextDouble() * (float)Program.mainForm.Height);
					}
					g.targetPos = nabMouseData.dragToPoint;
					SetForegroundWindow(Program.mainForm.Handle);
					Sound.CHOMP();
					nabMouseData.currentStage = NabMouseData.Stage.DraggingMouseAway;
				}
				if (Time.time > nabMouseData.chaseStartTime + 9f)
				{
					nabMouseData.currentStage = NabMouseData.Stage.Decelerating;
				}
			}
			if (nabMouseData.currentStage == NabMouseData.Stage.DraggingMouseAway)
			{
				if (Vector2.Distance(g.position, g.targetPos) < 30f)
				{
					Cursor.Clip = Rectangle.Empty;
					nabMouseData.currentStage = NabMouseData.Stage.Decelerating;
				}
				else
				{
					float p = Math.Min((Time.time - nabMouseData.grabbedOriginalTime) / 0.06f, 1f);
					Vector2 vector2 = Vector2.Lerp(nabMouseData.originalVectorToMouse, NabMouseData.StruggleRange, p);
					Vector2 vector3 = default(Vector2);
					vector3.x = ((vector2.x < 0f) ? (head2EndPoint.x + vector2.x) : head2EndPoint.x);
					vector3.y = ((vector2.y < 0f) ? (head2EndPoint.y + vector2.y) : head2EndPoint.y);
					tmpRect.Location = RenderFuncs.ToIntPoint(vector3);
					tmpSize.Width = Math.Abs((int)vector2.x);
					tmpSize.Height = Math.Abs((int)vector2.y);
					tmpRect.Size = tmpSize;
					Cursor.Clip = tmpRect;
				}
			}
			if (nabMouseData.currentStage == NabMouseData.Stage.Decelerating)
			{
				g.targetPos = g.position + Vector2.Normalize(g.velocity) * 5f;
				g.velocity -= Vector2.Normalize(g.velocity) * g.currentAcceleration * 2f * 0.008333334f;
				if (Vector2.Magnitude(g.velocity) < 80f)
				{
					GooseFunctions.SetTaskByID(g, "Wander");
				}
			}
		}
	}
}
