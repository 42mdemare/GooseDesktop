using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GooseDesktop.Refactor.GooseTasks;
using GooseShared;
using SamEngine;

namespace GooseDesktop.Refactor
{
	internal static class GooseFunctions
	{
		public static GooseEntity InitGoose()
		{
			GooseEntity gooseEntity = new GooseEntity(TickGoose, UpdateRig, RenderGoose);
			gooseEntity.parameters = new GooseEntity.ParametersTable();
			gooseEntity.position = new Vector2(-20f, 120f);
			gooseEntity.targetPos = new Vector2(100f, 150f);
			SetSpeed(gooseEntity, GooseEntity.SpeedTiers.Walk);
			gooseEntity.rig.feets = new ProceduralFeets();
			gooseEntity.rig.feets.lFootPos = ProceduralFeetFuncs.GetFootHome(gooseEntity.position, gooseEntity.direction, gooseEntity.rig.feets.feetDistanceApart, rightFoot: false);
			gooseEntity.rig.feets.rFootPos = ProceduralFeetFuncs.GetFootHome(gooseEntity.position, gooseEntity.direction, gooseEntity.rig.feets.feetDistanceApart, rightFoot: true);
			gooseEntity.renderData = new GooseRenderData();
			gooseEntity.renderData.shadowBitmap = new Bitmap(2, 2);
			gooseEntity.renderData.shadowBitmap.SetPixel(0, 0, Color.Transparent);
			gooseEntity.renderData.shadowBitmap.SetPixel(1, 1, Color.Transparent);
			gooseEntity.renderData.shadowBitmap.SetPixel(1, 0, Color.Transparent);
			gooseEntity.renderData.shadowBitmap.SetPixel(0, 1, Color.DarkGray);
			gooseEntity.renderData.shadowBrush = new TextureBrush(gooseEntity.renderData.shadowBitmap);
			gooseEntity.renderData.shadowPen = new Pen(gooseEntity.renderData.shadowBrush);
			LineCap lineCap3 = (gooseEntity.renderData.shadowPen.StartCap = (gooseEntity.renderData.shadowPen.EndCap = LineCap.Round));
			gooseEntity.renderData.DrawingPen = new Pen(Brushes.White);
			lineCap3 = (gooseEntity.renderData.DrawingPen.EndCap = (gooseEntity.renderData.DrawingPen.StartCap = LineCap.Round));
			if (GooseConfig.settings.UseCustomColors)
			{
				gooseEntity.renderData.brushGooseWhite = new SolidBrush(ColorTranslator.FromHtml(GooseConfig.settings.GooseDefaultWhite));
				gooseEntity.renderData.brushGooseOrange = new SolidBrush(ColorTranslator.FromHtml(GooseConfig.settings.GooseDefaultOrange));
				gooseEntity.renderData.brushGooseOutline = new SolidBrush(ColorTranslator.FromHtml(GooseConfig.settings.GooseDefaultOutline));
			}
			else
			{
				gooseEntity.renderData.brushGooseWhite = Brushes.White as SolidBrush;
				gooseEntity.renderData.brushGooseOrange = Brushes.Orange as SolidBrush;
				gooseEntity.renderData.brushGooseOutline = Brushes.LightGray as SolidBrush;
			}
			gooseEntity.tick = TickGoose;
			gooseEntity.updateRig = UpdateRig;
			gooseEntity.render = RenderGoose;
			SetTaskByID(gooseEntity, "Wander");
			return gooseEntity;
		}

		public static void TickGoose(GooseEntity g)
		{
			if (GooseConfig.settings.Task_CanAttackMouse && Input.leftMouseButton.Clicked && Vector2.Distance(g.position + new Vector2(0f, 14f), new Vector2(Input.mouseX, Input.mouseY)) < 30f)
			{
				SetTaskByID(g, "NabMouse");
			}
			g.targetDirection = Vector2.Normalize(g.position - g.targetPos);
			g.extendingNeck = false;
			RunAI(g);
			Vector2 vector = Vector2.Lerp(Vector2.GetFromAngleDegrees(g.direction), g.targetDirection, -0.25f);
			g.direction = (float)Math.Atan2(vector.y, vector.x) * (180f / (float)Math.PI);
			g.velocity = Vector2.ClampMagnitude(g.velocity, g.currentSpeed);
			if (g.canDecelerateImmediately && Vector2.Distance(g.position, g.targetPos) < g.parameters.StopRadius)
			{
				g.velocity = Vector2.Lerp(g.velocity, Vector2.zero, Vector2.Distance(g.position, g.targetPos) / g.parameters.StopRadius);
			}
			else
			{
				g.velocity += Vector2.Normalize(g.targetPos - g.position) * g.currentAcceleration * 0.008333334f;
			}
			g.position += g.velocity * 0.008333334f;
			ProceduralFeetFuncs.SolveFeets(g.rig.feets, g.position, g.direction, g.stepInterval, g.rig.feets.feetDistanceApart, Time.time < g.trackMudEndTime, g.footMarks, ref g.footMarkIndex);
			Vector2.Magnitude(g.velocity);
			int num = ((g.extendingNeck | (g.currentSpeed >= g.parameters.RunSpeed)) ? 1 : 0);
			g.rig.neckLerpPercent = SamMath.Lerp(g.rig.neckLerpPercent, num, 0.075f);
		}

		public static void RenderGoose(GooseEntity g, Graphics gfx)
		{
			for (int i = 0; i < g.footMarks.Length; i++)
			{
				if (g.footMarks[i].time != 0f)
				{
					float num = g.footMarks[i].time + 8.5f;
					float p = SamMath.Clamp(Time.time - num, 0f, 1f) / 1f;
					float num2 = SamMath.Lerp(3f, 0f, p);
					RenderFuncs.FillCircleFromCenter(gfx, Brushes.SaddleBrown, g.footMarks[i].position, (int)num2);
				}
			}
			float direction = g.direction;
			int num3 = (int)g.position.x;
			int num4 = (int)g.position.y;
			Vector2 vector = new Vector2(num3, num4);
			Vector2 vector2 = new Vector2(1.3f, 0.4f);
			Vector2 fromAngleDegrees = Vector2.GetFromAngleDegrees(direction);
			_ = fromAngleDegrees * vector2;
			Vector2 fromAngleDegrees2 = Vector2.GetFromAngleDegrees(direction + 90f);
			_ = fromAngleDegrees2 * vector2;
			Vector2 vector3 = new Vector2(0f, -1f);
			int num5 = 2;
			g.renderData.DrawingPen.Brush = g.renderData.brushGooseWhite;
			RenderFuncs.FillCircleFromCenter(gfx, g.renderData.brushGooseOrange, g.rig.feets.lFootPos, 4);
			RenderFuncs.FillCircleFromCenter(gfx, g.renderData.brushGooseOrange, g.rig.feets.rFootPos, 4);
			RenderFuncs.FillEllipseFromCenter(gfx, g.renderData.shadowBrush, (int)vector.x, (int)vector.y, 20, 15);
			g.renderData.DrawingPen.Color = g.renderData.brushGooseOutline.Color;
			g.renderData.DrawingPen.Width = 22 + num5;
			gfx.DrawLine(g.renderData.DrawingPen, RenderFuncs.ToIntPoint(g.rig.bodyCenter + fromAngleDegrees * 11f), RenderFuncs.ToIntPoint(g.rig.bodyCenter - fromAngleDegrees * 11f));
			g.renderData.DrawingPen.Width = 13 + num5;
			gfx.DrawLine(g.renderData.DrawingPen, RenderFuncs.ToIntPoint(g.rig.neckBase), RenderFuncs.ToIntPoint(g.rig.neckHeadPoint));
			g.renderData.DrawingPen.Width = 15 + num5;
			gfx.DrawLine(g.renderData.DrawingPen, RenderFuncs.ToIntPoint(g.rig.neckHeadPoint), RenderFuncs.ToIntPoint(g.rig.head1EndPoint));
			g.renderData.DrawingPen.Width = 10 + num5;
			gfx.DrawLine(g.renderData.DrawingPen, RenderFuncs.ToIntPoint(g.rig.head1EndPoint), RenderFuncs.ToIntPoint(g.rig.head2EndPoint));
			g.renderData.DrawingPen.Color = g.renderData.brushGooseOutline.Color;
			g.renderData.DrawingPen.Width = 15f;
			gfx.DrawLine(g.renderData.DrawingPen, RenderFuncs.ToIntPoint(g.rig.underbodyCenter + fromAngleDegrees * 7f), RenderFuncs.ToIntPoint(g.rig.underbodyCenter - fromAngleDegrees * 7f));
			g.renderData.DrawingPen.Color = g.renderData.brushGooseWhite.Color;
			g.renderData.DrawingPen.Width = 22f;
			gfx.DrawLine(g.renderData.DrawingPen, RenderFuncs.ToIntPoint(g.rig.bodyCenter + fromAngleDegrees * 11f), RenderFuncs.ToIntPoint(g.rig.bodyCenter - fromAngleDegrees * 11f));
			g.renderData.DrawingPen.Width = 13f;
			gfx.DrawLine(g.renderData.DrawingPen, RenderFuncs.ToIntPoint(g.rig.neckBase), RenderFuncs.ToIntPoint(g.rig.neckHeadPoint));
			g.renderData.DrawingPen.Width = 15f;
			gfx.DrawLine(g.renderData.DrawingPen, RenderFuncs.ToIntPoint(g.rig.neckHeadPoint), RenderFuncs.ToIntPoint(g.rig.head1EndPoint));
			g.renderData.DrawingPen.Width = 10f;
			gfx.DrawLine(g.renderData.DrawingPen, RenderFuncs.ToIntPoint(g.rig.head1EndPoint), RenderFuncs.ToIntPoint(g.rig.head2EndPoint));
			int num6 = 9;
			int num7 = 3;
			g.renderData.DrawingPen.Width = num6;
			g.renderData.DrawingPen.Brush = g.renderData.brushGooseOrange;
			Vector2 vector4 = g.rig.head2EndPoint + fromAngleDegrees * num7;
			gfx.DrawLine(g.renderData.DrawingPen, RenderFuncs.ToIntPoint(g.rig.head2EndPoint), RenderFuncs.ToIntPoint(vector4));
			Vector2 pos = g.rig.neckHeadPoint + vector3 * 3f + -fromAngleDegrees2 * vector2 * 5f + fromAngleDegrees * 5f;
			Vector2 pos2 = g.rig.neckHeadPoint + vector3 * 3f + fromAngleDegrees2 * vector2 * 5f + fromAngleDegrees * 5f;
			RenderFuncs.FillCircleFromCenter(gfx, Brushes.Black, pos, 2);
			RenderFuncs.FillCircleFromCenter(gfx, Brushes.Black, pos2, 2);
		}

		public static void UpdateRig(Rig rig, Vector2 position, float direction)
		{
			int num = (int)position.x;
			int num2 = (int)position.y;
			Vector2 vector = new Vector2(num, num2);
			Vector2 vector2 = new Vector2(1.3f, 0.4f);
			Vector2 fromAngleDegrees = Vector2.GetFromAngleDegrees(direction);
			_ = fromAngleDegrees * vector2;
			_ = Vector2.GetFromAngleDegrees(direction + 90f) * vector2;
			Vector2 vector3 = new Vector2(0f, -1f);
			rig.underbodyCenter = vector + vector3 * 9f;
			rig.bodyCenter = vector + vector3 * 14f;
			int num3 = (int)SamMath.Lerp(20f, 10f, rig.neckLerpPercent);
			int num4 = (int)SamMath.Lerp(3f, 16f, rig.neckLerpPercent);
			rig.neckCenter = vector + vector3 * (14 + num3);
			rig.neckBase = rig.bodyCenter + fromAngleDegrees * 15f;
			rig.neckHeadPoint = rig.neckBase + fromAngleDegrees * num4 + vector3 * num3;
			rig.head1EndPoint = rig.neckHeadPoint + fromAngleDegrees * 3f - vector3 * 1f;
			rig.head2EndPoint = rig.head1EndPoint + fromAngleDegrees * 5f;
		}

		public static void RunAI(GooseEntity g)
		{
			GooseTaskDatabase.GetTask(g.currentTask).RunTask(g);
		}

		public static void SetTaskByID(GooseEntity g, string id, bool honck = true)
		{
			if (honck)
			{
				Sound.HONCC();
			}
			int taskIndexByID = GooseTaskDatabase.GetTaskIndexByID(id);
			if (taskIndexByID == -1)
			{
				MessageBox.Show("Cannot set task by an ID that's not registered in the database.", "Set Task Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			g.currentTask = taskIndexByID;
			g.currentTaskData = GooseTaskDatabase.GetTask(g.currentTask).GetNewTaskData(g);
		}

		public static void ChooseRandomTask(GooseEntity g)
		{
			g.currentTask = GooseTaskDatabase.GetNextRandomTask();
			g.currentTaskData = GooseTaskDatabase.GetTask(g.currentTask).GetNewTaskData(g);
		}

		public static void SetTaskDefault(GooseEntity g)
		{
			SetTaskByID(g, "Wander", honck: false);
		}

		public static void SetSpeed(GooseEntity g, GooseEntity.SpeedTiers tier)
		{
			switch (tier)
			{
			case GooseEntity.SpeedTiers.Walk:
				g.currentSpeed = g.parameters.WalkSpeed;
				g.currentAcceleration = g.parameters.AccelerationNormal;
				g.stepInterval = g.parameters.StepTimeNormal;
				break;
			case GooseEntity.SpeedTiers.Run:
				g.currentSpeed = g.parameters.RunSpeed;
				g.currentAcceleration = g.parameters.AccelerationNormal;
				g.stepInterval = g.parameters.StepTimeNormal;
				break;
			case GooseEntity.SpeedTiers.Charge:
				g.currentSpeed = g.parameters.ChargeSpeed;
				g.currentAcceleration = g.parameters.AccelerationCharged;
				g.stepInterval = g.parameters.StepTimeCharged;
				break;
			}
		}

		public static ScreenDirection SetTargetOffscreen(GooseEntity g, bool canExitTop = false)
		{
			int num = (int)g.position.x;
			ScreenDirection result = ScreenDirection.Left;
			g.targetPos = new Vector2(-50f, SamMath.Lerp(g.position.y, Program.mainForm.Height / 2, 0.4f));
			if (num > Program.mainForm.Width / 2)
			{
				num = Program.mainForm.Width - (int)g.position.x;
				result = ScreenDirection.Right;
				g.targetPos = new Vector2(Program.mainForm.Width + 50, SamMath.Lerp(g.position.y, Program.mainForm.Height / 2, 0.4f));
			}
			if (canExitTop && (float)num > g.position.y)
			{
				result = ScreenDirection.Top;
				g.targetPos = new Vector2(SamMath.Lerp(g.position.x, Program.mainForm.Width / 2, 0.4f), -50f);
			}
			return result;
		}

		public static void AddFootMark(Vector2 markPos, GooseShared.FootMark[] footMarks, ref int footMarkIndex)
		{
			footMarks[footMarkIndex].time = Time.time;
			footMarks[footMarkIndex].position = markPos;
			footMarkIndex++;
			if (footMarkIndex >= footMarks.Length)
			{
				footMarkIndex = 0;
			}
		}

		public static bool IsGooseAtTarget(GooseEntity g, float distanceToTrigger)
		{
			return Vector2.Distance(g.position, g.targetPos) < distanceToTrigger;
		}

		public static float GetDistanceToTarget(GooseEntity g)
		{
			return Vector2.Distance(g.position, g.targetPos);
		}
	}
}
