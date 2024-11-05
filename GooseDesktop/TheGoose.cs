using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using SamEngine;

namespace GooseDesktop
{
	internal static class TheGoose
	{
		private enum SpeedTiers
		{
			Walk,
			Run,
			Charge
		}

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

		private struct Task_Wander
		{
			private const float MinPauseTime = 1f;

			private const float MaxPauseTime = 2f;

			public const float GoodEnoughDistance = 20f;

			public float wanderingStartTime;

			public float wanderingDuration;

			public float pauseStartTime;

			public float pauseDuration;

			public static float GetRandomPauseDuration()
			{
				return 1f + (float)SamMath.Rand.NextDouble() * 1f;
			}

			public static float GetRandomWanderDuration()
			{
				if (Time.time < 1f)
				{
					return GooseConfig.settings.FirstWanderTimeSeconds;
				}
				return SamMath.RandomRange(GooseConfig.settings.MinWanderingTimeSeconds, GooseConfig.settings.MaxWanderingTimeSeconds);
			}

			public static float GetRandomWalkTime()
			{
				return SamMath.RandomRange(1f, 6f);
			}
		}

		private struct Task_NabMouse
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

		private struct Task_CollectWindow
		{
			public enum Stage
			{
				WalkingOffscreen,
				WaitingToBringWindowBack,
				DraggingWindowBack
			}

			public enum ScreenDirection
			{
				Left,
				Top,
				Right
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

		private class MovableForm : Form
		{
			public MovableForm()
			{
				base.StartPosition = FormStartPosition.Manual;
				base.Width = 400;
				base.Height = 400;
				BackColor = Color.DimGray;
				base.Icon = null;
				base.ShowIcon = false;
				SetWindowResizableThreadsafe(canResize: false);
			}

			public void SetWindowPositionThreadsafe(Point p)
			{
				if (base.InvokeRequired)
				{
					BeginInvoke((MethodInvoker)delegate
					{
						base.Location = p;
						base.TopMost = true;
					});
				}
				else
				{
					base.Location = p;
					base.TopMost = true;
				}
			}

			public void SetWindowResizableThreadsafe(bool canResize)
			{
				if (base.InvokeRequired)
				{
					BeginInvoke((MethodInvoker)delegate
					{
						base.FormBorderStyle = ((!canResize) ? FormBorderStyle.FixedSingle : FormBorderStyle.Sizable);
						MovableForm movableForm = this;
						bool maximizeBox2 = (base.MinimizeBox = canResize);
						movableForm.MaximizeBox = maximizeBox2;
					});
				}
				else
				{
					base.FormBorderStyle = ((!canResize) ? FormBorderStyle.FixedSingle : FormBorderStyle.Sizable);
					bool maximizeBox = (base.MinimizeBox = canResize);
					base.MaximizeBox = maximizeBox;
				}
			}
		}

		private class SimpleImageForm : MovableForm
		{
			private static readonly string memesRootFolder = Program.GetPathToFileInAssembly("Assets/Images/Memes/");

			private Image[] localImages;

			private Deck localImageDeck;

			private static string[] imageURLs = new string[5] { "https://preview.redd.it/dsfjv8aev0p31.png?width=960&crop=smart&auto=webp&s=1d58948acc5c6dd60df1092c1bd2a59a509069fd", "https://i.redd.it/4ojv59zvglp31.jpg", "https://i.redd.it/4bamd6lnso241.jpg", "https://i.redd.it/5i5et9p1vsp31.jpg", "https://i.redd.it/j2f1i9djx5p31.jpg" };

			private static Deck imageURLDeck = new Deck(imageURLs.Length);

			public SimpleImageForm()
			{
				List<Image> list = new List<Image>();
				try
				{
					string[] files = Directory.GetFiles(memesRootFolder);
					for (int i = 0; i < files.Length; i++)
					{
						Image image = Image.FromFile(files[i]);
						if (image != null)
						{
							list.Add(image);
						}
					}
				}
				catch
				{
				}
				localImages = list.ToArray();
				localImageDeck = new Deck(localImages.Length);
				PictureBox pictureBox = new PictureBox
				{
					Dock = DockStyle.Fill
				};
				try
				{
					pictureBox.Image = localImages[localImageDeck.Next()];
				}
				catch
				{
					pictureBox.LoadAsync(imageURLs[imageURLDeck.Next()]);
				}
				pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
				base.Controls.Add(pictureBox);
			}
		}

		private class SimpleTextForm : MovableForm
		{
			private static string[] possiblePhrases = new string[6] { "am goose hjonk", "good work", "nsfdafdsaafsdjl\r\nasdas       sorry\r\nhard to type withh feet", "i cause problems on purpose", "\"peace was never an option\"\r\n   -the goose (me)", "\r\n\r\n  >o) \r\n    (_>" };

			private static Deck textIndices = new Deck(possiblePhrases.Length);

			public SimpleTextForm()
			{
				base.Width = 200;
				base.Height = 150;
				Text = "Goose \"Not-epad\"";
				TextBox textBox = new TextBox();
				textBox.Multiline = true;
				textBox.AcceptsReturn = true;
				textBox.Text = possiblePhrases[textIndices.Next()];
				textBox.Location = new Point(0, 0);
				textBox.Width = base.ClientSize.Width;
				textBox.Height = base.ClientSize.Height - 5;
				textBox.Select(textBox.Text.Length, 0);
				textBox.Font = new Font(textBox.Font.FontFamily, 10f, FontStyle.Regular);
				base.Controls.Add(textBox);
				string text = Environment.SystemDirectory + "\\notepad.exe";
				if (File.Exists(text))
				{
					try
					{
						base.Icon = Icon.ExtractAssociatedIcon(text);
						base.ShowIcon = true;
					}
					catch
					{
					}
				}
			}

			private void ExitWindow(object sender, EventArgs args)
			{
				Close();
			}
		}

		private struct Task_TrackMud
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

		private struct Rig
		{
			public const int UnderBodyRadius = 15;

			public const int UnderBodyLength = 7;

			public const int UnderBodyElevation = 9;

			public Vector2 underbodyCenter;

			public const int BodyRadius = 22;

			public const int BodyLength = 11;

			public const int BodyElevation = 14;

			public Vector2 bodyCenter;

			public const int NeccRadius = 13;

			public const int NeccHeight1 = 20;

			public const int NeccExtendForward1 = 3;

			public const int NeccHeight2 = 10;

			public const int NeccExtendForward2 = 16;

			public float neckLerpPercent;

			public Vector2 neckCenter;

			public Vector2 neckBase;

			public Vector2 neckHeadPoint;

			public const int HeadRadius1 = 15;

			public const int HeadLength1 = 3;

			public const int HeadRadius2 = 10;

			public const int HeadLength2 = 5;

			public Vector2 head1EndPoint;

			public Vector2 head2EndPoint;

			public const int EyeRadius = 2;

			public const int EyeElevation = 3;

			public const float IPD = 5f;

			public const float EyesForward = 5f;
		}

		private static Vector2 position = new Vector2(300f, 300f);

		private static Vector2 velocity = new Vector2(0f, 0f);

		private static float direction = 90f;

		private static Vector2 targetDirection;

		private static bool overrideExtendNeck;

		private const GooseTask FirstUX_FirstTask = GooseTask.TrackMud;

		private const GooseTask FirstUX_SecondTask = GooseTask.CollectWindow_Meme;

		private static Vector2 targetPos = new Vector2(300f, 300f);

		private static float targetDir = 90f;

		private static float currentSpeed = 80f;

		private static float currentAcceleration = 1300f;

		private static float stepTime = 0.2f;

		private const float WalkSpeed = 80f;

		private const float RunSpeed = 200f;

		private const float ChargeSpeed = 400f;

		private const float turnSpeed = 120f;

		private const float AccelerationNormal = 1300f;

		private const float AccelerationCharged = 2300f;

		private const float StopRadius = -10f;

		private const float StepTimeNormal = 0.2f;

		private const float StepTimeCharged = 0.1f;

		private static float trackMudEndTime = -1f;

		private const float DurationToTrackMud = 15f;

		private static Pen DrawingPen;

		private static Bitmap shadowBitmap;

		private static TextureBrush shadowBrush;

		private static Pen shadowPen;

		private static SolidBrush brushGooseWhite;

		private static SolidBrush brushGooseOrange;

		private static SolidBrush brushGooseOutline;

		private static FootMark[] footMarks = new FootMark[64];

		private static int footMarkIndex = 0;

		private static bool lastFrameMouseButtonPressed = false;

		private static GooseTask currentTask;

		private static Task_Wander taskWanderInfo;

		private static Task_NabMouse taskNabMouseInfo;

		private static Rectangle tmpRect = default(Rectangle);

		private static Size tmpSize = default(Size);

		private static Task_CollectWindow taskCollectWindowInfo;

		private static Task_TrackMud taskTrackMudInfo;

		private static GooseTask[] gooseTaskWeightedList = new GooseTask[8]
		{
			GooseTask.TrackMud,
			GooseTask.TrackMud,
			GooseTask.CollectWindow_Meme,
			GooseTask.CollectWindow_Meme,
			GooseTask.CollectWindow_Notepad,
			GooseTask.NabMouse,
			GooseTask.NabMouse,
			GooseTask.NabMouse
		};

		private static Deck taskPickerDeck = new Deck(gooseTaskWeightedList.Length);

		private static Vector2 lFootPos;

		private static Vector2 rFootPos;

		private static float lFootMoveTimeStart = -1f;

		private static float rFootMoveTimeStart = -1f;

		private static Vector2 lFootMoveOrigin;

		private static Vector2 rFootMoveOrigin;

		private static Vector2 lFootMoveDir;

		private static Vector2 rFootMoveDir;

		private const float wantStepAtDistance = 5f;

		private const int feetDistanceApart = 6;

		private const float overshootFraction = 0.4f;

		private static Rig gooseRig;

		public static void Init()
		{
			position = new Vector2(-20f, 120f);
			targetPos = new Vector2(100f, 150f);
			if (!GooseConfig.settings.AttackRandomly)
			{
				int num = Array.IndexOf(taskPickerDeck.indices, Array.IndexOf(gooseTaskWeightedList, GooseTask.CollectWindow_Meme));
				int num2 = taskPickerDeck.indices[0];
				taskPickerDeck.indices[0] = taskPickerDeck.indices[num];
				taskPickerDeck.indices[num] = num2;
			}
			lFootPos = GetFootHome(rightFoot: false);
			rFootPos = GetFootHome(rightFoot: true);
			shadowBitmap = new Bitmap(2, 2);
			shadowBitmap.SetPixel(0, 0, Color.Transparent);
			shadowBitmap.SetPixel(1, 1, Color.Transparent);
			shadowBitmap.SetPixel(1, 0, Color.Transparent);
			shadowBitmap.SetPixel(0, 1, Color.DarkGray);
			shadowBrush = new TextureBrush(shadowBitmap);
			shadowPen = new Pen(shadowBrush);
			LineCap lineCap3 = (shadowPen.StartCap = (shadowPen.EndCap = LineCap.Round));
			DrawingPen = new Pen(Brushes.White);
			lineCap3 = (DrawingPen.EndCap = (DrawingPen.StartCap = LineCap.Round));
			if (GooseConfig.settings.UseCustomColors)
			{
				brushGooseWhite = new SolidBrush(ColorTranslator.FromHtml(GooseConfig.settings.GooseDefaultWhite));
				brushGooseOrange = new SolidBrush(ColorTranslator.FromHtml(GooseConfig.settings.GooseDefaultOrange));
				brushGooseOutline = new SolidBrush(ColorTranslator.FromHtml(GooseConfig.settings.GooseDefaultOutline));
			}
			else
			{
				brushGooseWhite = Brushes.White as SolidBrush;
				brushGooseOrange = Brushes.Orange as SolidBrush;
				brushGooseOutline = Brushes.LightGray as SolidBrush;
			}
			SetTask(GooseTask.Wander);
		}

		private static void SetSpeed(SpeedTiers tier)
		{
			switch (tier)
			{
			case SpeedTiers.Walk:
				currentSpeed = 80f;
				currentAcceleration = 1300f;
				stepTime = 0.2f;
				break;
			case SpeedTiers.Run:
				currentSpeed = 200f;
				currentAcceleration = 1300f;
				stepTime = 0.2f;
				break;
			case SpeedTiers.Charge:
				currentSpeed = 400f;
				currentAcceleration = 2300f;
				stepTime = 0.1f;
				break;
			}
		}

		public static void Tick()
		{
			Cursor.Clip = Rectangle.Empty;
			if (GooseConfig.settings.Task_CanAttackMouse && currentTask != GooseTask.NabMouse && (Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left && !lastFrameMouseButtonPressed && Vector2.Distance(position + new Vector2(0f, 14f), new Vector2(Cursor.Position.X, Cursor.Position.Y)) < 30f)
			{
				SetTask(GooseTask.NabMouse);
			}
			lastFrameMouseButtonPressed = (Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left;
			targetDirection = Vector2.Normalize(targetPos - position);
			overrideExtendNeck = false;
			RunAI();
			Vector2 vector = Vector2.Lerp(Vector2.GetFromAngleDegrees(direction), targetDirection, (float)(1.0 - Math.Exp(-0.25)));
			direction = (float)Math.Atan2(vector.y, vector.x) * (180f / (float)Math.PI);
			if (Vector2.Magnitude(velocity) > currentSpeed)
			{
				velocity = Vector2.Normalize(velocity) * currentSpeed;
			}
			velocity += Vector2.Normalize(targetPos - position) * currentAcceleration * 0.008333334f;
			position += velocity * 0.008333334f;
			SolveFeet();
			Vector2.Magnitude(velocity);
			int num = ((overrideExtendNeck | (currentSpeed >= 200f)) ? 1 : 0);
			gooseRig.neckLerpPercent = SamMath.Lerp(gooseRig.neckLerpPercent, num, 0.075f);
		}

		public static void InjectTask(int taskNum)
		{
			SetTask((GooseTask)taskNum);
		}

		private static void RunWander()
		{
			if (!IPCManager.ConnectedToTwitch && Time.time - taskWanderInfo.wanderingStartTime > taskWanderInfo.wanderingDuration)
			{
				ChooseNextTask();
			}
			else if (taskWanderInfo.pauseStartTime > 0f)
			{
				if (Time.time - taskWanderInfo.pauseStartTime > taskWanderInfo.pauseDuration)
				{
					taskWanderInfo.pauseStartTime = -1f;
					float num = Task_Wander.GetRandomWalkTime() * currentSpeed;
					targetPos = new Vector2(SamMath.RandomRange(0f, Program.mainForm.Width), SamMath.RandomRange(0f, Program.mainForm.Height));
					if (Vector2.Distance(position, targetPos) > num)
					{
						targetPos = position + Vector2.Normalize(targetPos - position) * num;
					}
				}
				else
				{
					velocity = Vector2.zero;
				}
			}
			else if (Vector2.Distance(position, targetPos) < 20f)
			{
				taskWanderInfo.pauseStartTime = Time.time;
				taskWanderInfo.pauseDuration = Task_Wander.GetRandomPauseDuration();
			}
		}

		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow(IntPtr hWnd);

		private static void RunNabMouse()
		{
			Vector2 vector = new Vector2(Cursor.Position.X, Cursor.Position.Y);
			Vector2 head2EndPoint = gooseRig.head2EndPoint;
			if (taskNabMouseInfo.currentStage == Task_NabMouse.Stage.SeekingMouse)
			{
				SetSpeed(SpeedTiers.Charge);
				targetPos = vector - (gooseRig.head2EndPoint - position);
				if (Vector2.Distance(head2EndPoint, vector) < 15f)
				{
					taskNabMouseInfo.originalVectorToMouse = vector - head2EndPoint;
					taskNabMouseInfo.grabbedOriginalTime = Time.time;
					taskNabMouseInfo.dragToPoint = position;
					while (Vector2.Distance(taskNabMouseInfo.dragToPoint, position) / 400f < 1.2f)
					{
						taskNabMouseInfo.dragToPoint = new Vector2((float)SamMath.Rand.NextDouble() * (float)Program.mainForm.Width, (float)SamMath.Rand.NextDouble() * (float)Program.mainForm.Height);
					}
					targetPos = taskNabMouseInfo.dragToPoint;
					SetForegroundWindow(Program.mainForm.Handle);
					Sound.CHOMP();
					taskNabMouseInfo.currentStage = Task_NabMouse.Stage.DraggingMouseAway;
				}
				if (Time.time > taskNabMouseInfo.chaseStartTime + 9f)
				{
					taskNabMouseInfo.currentStage = Task_NabMouse.Stage.Decelerating;
				}
			}
			if (taskNabMouseInfo.currentStage == Task_NabMouse.Stage.DraggingMouseAway)
			{
				if (Vector2.Distance(position, targetPos) < 30f)
				{
					Cursor.Clip = Rectangle.Empty;
					taskNabMouseInfo.currentStage = Task_NabMouse.Stage.Decelerating;
				}
				else
				{
					float p = Math.Min((Time.time - taskNabMouseInfo.grabbedOriginalTime) / 0.06f, 1f);
					Vector2 vector2 = Vector2.Lerp(taskNabMouseInfo.originalVectorToMouse, Task_NabMouse.StruggleRange, p);
					Vector2 vector3 = default(Vector2);
					vector3.x = ((vector2.x < 0f) ? (head2EndPoint.x + vector2.x) : head2EndPoint.x);
					vector3.y = ((vector2.y < 0f) ? (head2EndPoint.y + vector2.y) : head2EndPoint.y);
					tmpRect.Location = ToIntPoint(vector3);
					tmpSize.Width = Math.Abs((int)vector2.x);
					tmpSize.Height = Math.Abs((int)vector2.y);
					tmpRect.Size = tmpSize;
					Cursor.Clip = tmpRect;
				}
			}
			if (taskNabMouseInfo.currentStage == Task_NabMouse.Stage.Decelerating)
			{
				targetPos = position + Vector2.Normalize(velocity) * 5f;
				velocity -= Vector2.Normalize(velocity) * currentAcceleration * 2f * 0.008333334f;
				if (Vector2.Magnitude(velocity) < 80f)
				{
					SetTask(GooseTask.Wander);
				}
			}
		}

		private static void RunCollectWindow()
		{
			switch (taskCollectWindowInfo.stage)
			{
			case Task_CollectWindow.Stage.WalkingOffscreen:
				if (Vector2.Distance(position, targetPos) < 5f)
				{
					taskCollectWindowInfo.secsToWait = Task_CollectWindow.GetWaitTime();
					taskCollectWindowInfo.waitStartTime = Time.time;
					taskCollectWindowInfo.stage = Task_CollectWindow.Stage.WaitingToBringWindowBack;
				}
				break;
			case Task_CollectWindow.Stage.WaitingToBringWindowBack:
				if (Time.time - taskCollectWindowInfo.waitStartTime > taskCollectWindowInfo.secsToWait)
				{
					taskCollectWindowInfo.mainForm.FormClosing += CollectMemeTask_CancelEarly;
					new Thread((ThreadStart)delegate
					{
						taskCollectWindowInfo.mainForm.ShowDialog();
					}).Start();
					switch (taskCollectWindowInfo.screenDirection)
					{
					case Task_CollectWindow.ScreenDirection.Left:
						targetPos.y = SamMath.Lerp(position.y, Program.mainForm.Height / 2, SamMath.RandomRange(0.2f, 0.3f));
						targetPos.x = (float)taskCollectWindowInfo.mainForm.Width + SamMath.RandomRange(15f, 20f);
						break;
					case Task_CollectWindow.ScreenDirection.Top:
						targetPos.y = (float)taskCollectWindowInfo.mainForm.Height + SamMath.RandomRange(80f, 100f);
						targetPos.x = SamMath.Lerp(position.x, Program.mainForm.Width / 2, SamMath.RandomRange(0.2f, 0.3f));
						break;
					case Task_CollectWindow.ScreenDirection.Right:
						targetPos.y = SamMath.Lerp(position.y, Program.mainForm.Height / 2, SamMath.RandomRange(0.2f, 0.3f));
						targetPos.x = (float)Program.mainForm.Width - ((float)taskCollectWindowInfo.mainForm.Width + SamMath.RandomRange(20f, 30f));
						break;
					}
					targetPos.x = SamMath.Clamp(targetPos.x, taskCollectWindowInfo.mainForm.Width + 55, Program.mainForm.Width - (taskCollectWindowInfo.mainForm.Width + 55));
					targetPos.y = SamMath.Clamp(targetPos.y, taskCollectWindowInfo.mainForm.Height + 80, Program.mainForm.Height);
					taskCollectWindowInfo.stage = Task_CollectWindow.Stage.DraggingWindowBack;
				}
				break;
			case Task_CollectWindow.Stage.DraggingWindowBack:
				if (Vector2.Distance(position, targetPos) < 5f)
				{
					targetPos = position + Vector2.GetFromAngleDegrees(direction + 180f) * 40f;
					SetTask(GooseTask.Wander);
				}
				else
				{
					overrideExtendNeck = true;
					targetDirection = position - targetPos;
					taskCollectWindowInfo.mainForm.SetWindowPositionThreadsafe(ToIntPoint(gooseRig.head2EndPoint - taskCollectWindowInfo.windowOffsetToBeak));
				}
				break;
			}
		}

		private static void CollectMemeTask_CancelEarly(object sender, FormClosingEventArgs args)
		{
			if (GooseConfig.settings.Task_CanAttackMouse)
			{
				SetTask(GooseTask.NabMouse);
			}
		}

		private static void RunTrackMud()
		{
			switch (taskTrackMudInfo.stage)
			{
			case Task_TrackMud.Stage.DecideToRun:
				SetTargetOffscreen();
				SetSpeed(SpeedTiers.Run);
				taskTrackMudInfo.stage = Task_TrackMud.Stage.RunningOffscreen;
				break;
			case Task_TrackMud.Stage.RunningOffscreen:
				if (Vector2.Distance(position, targetPos) < 5f)
				{
					targetPos = new Vector2(SamMath.RandomRange(0f, Program.mainForm.Width), SamMath.RandomRange(0f, Program.mainForm.Height));
					taskTrackMudInfo.nextDirChangeTime = Time.time + Task_TrackMud.GetDirChangeInterval();
					taskTrackMudInfo.timeToStopRunning = Time.time + 2f;
					trackMudEndTime = Time.time + 15f;
					taskTrackMudInfo.stage = Task_TrackMud.Stage.RunningWandering;
					Sound.PlayMudSquith();
				}
				break;
			case Task_TrackMud.Stage.RunningWandering:
				if (Vector2.Distance(position, targetPos) < 5f || Time.time > taskTrackMudInfo.nextDirChangeTime)
				{
					targetPos = new Vector2(SamMath.RandomRange(0f, Program.mainForm.Width), SamMath.RandomRange(0f, Program.mainForm.Height));
					taskTrackMudInfo.nextDirChangeTime = Time.time + Task_TrackMud.GetDirChangeInterval();
				}
				if (Time.time > taskTrackMudInfo.timeToStopRunning)
				{
					targetPos = position + new Vector2(30f, 3f);
					targetPos.x = SamMath.Clamp(targetPos.x, 55f, Program.mainForm.Width - 55);
					targetPos.y = SamMath.Clamp(targetPos.y, 80f, Program.mainForm.Height - 80);
					SetTask(GooseTask.Wander, honck: false);
				}
				break;
			}
		}

		private static void ChooseNextTask()
		{
			if (!GooseConfig.settings.AttackRandomly && Time.time < GooseConfig.settings.FirstWanderTimeSeconds + 1f)
			{
				SetTask(GooseTask.TrackMud);
				return;
			}
			float num = 8f;
			GooseTask gooseTask = gooseTaskWeightedList[taskPickerDeck.Next()];
			while (!GooseConfig.settings.AttackRandomly && gooseTask == GooseTask.NabMouse)
			{
				gooseTask = gooseTaskWeightedList[taskPickerDeck.Next()];
			}
			SetTask(gooseTask);
		}

		private static void SetTask(GooseTask task)
		{
			SetTask(task, honck: true);
		}

		private static void SetTask(GooseTask task, bool honck)
		{
			if (honck)
			{
				Sound.HONCC();
			}
			currentTask = task;
			switch (task)
			{
			case GooseTask.Wander:
				SetSpeed(SpeedTiers.Walk);
				taskWanderInfo = default(Task_Wander);
				taskWanderInfo.pauseStartTime = -1f;
				taskWanderInfo.wanderingStartTime = Time.time;
				taskWanderInfo.wanderingDuration = Task_Wander.GetRandomWanderDuration();
				break;
			case GooseTask.NabMouse:
				taskNabMouseInfo = default(Task_NabMouse);
				taskNabMouseInfo.chaseStartTime = Time.time;
				break;
			case GooseTask.CollectWindow_Meme:
				taskCollectWindowInfo = default(Task_CollectWindow);
				taskCollectWindowInfo.mainForm = new SimpleImageForm();
				SetTask(GooseTask.CollectWindow_DONOTSET, honck: false);
				break;
			case GooseTask.CollectWindow_Notepad:
				taskCollectWindowInfo = default(Task_CollectWindow);
				taskCollectWindowInfo.mainForm = new SimpleTextForm();
				SetTask(GooseTask.CollectWindow_DONOTSET, honck: false);
				break;
			case GooseTask.CollectWindow_DONOTSET:
				taskCollectWindowInfo.screenDirection = SetTargetOffscreen();
				switch (taskCollectWindowInfo.screenDirection)
				{
				case Task_CollectWindow.ScreenDirection.Left:
					taskCollectWindowInfo.windowOffsetToBeak = new Vector2(taskCollectWindowInfo.mainForm.Width, taskCollectWindowInfo.mainForm.Height / 2);
					break;
				case Task_CollectWindow.ScreenDirection.Right:
					taskCollectWindowInfo.windowOffsetToBeak = new Vector2(0f, taskCollectWindowInfo.mainForm.Height / 2);
					break;
				case Task_CollectWindow.ScreenDirection.Top:
					taskCollectWindowInfo.windowOffsetToBeak = new Vector2(taskCollectWindowInfo.mainForm.Width / 2, taskCollectWindowInfo.mainForm.Height);
					break;
				}
				break;
			case GooseTask.TrackMud:
				taskTrackMudInfo = default(Task_TrackMud);
				break;
			}
		}

		private static void RunAI()
		{
			switch (currentTask)
			{
			case GooseTask.Wander:
				RunWander();
				break;
			case GooseTask.NabMouse:
				RunNabMouse();
				break;
			case GooseTask.CollectWindow_DONOTSET:
				RunCollectWindow();
				break;
			case GooseTask.TrackMud:
				RunTrackMud();
				break;
			case GooseTask.CollectWindow_Meme:
			case GooseTask.CollectWindow_Notepad:
				break;
			}
		}

		private static Task_CollectWindow.ScreenDirection SetTargetOffscreen(bool canExitTop = false)
		{
			int num = (int)position.x;
			Task_CollectWindow.ScreenDirection result = Task_CollectWindow.ScreenDirection.Left;
			targetPos = new Vector2(-50f, SamMath.Lerp(position.y, Program.mainForm.Height / 2, 0.4f));
			if (num > Program.mainForm.Width / 2)
			{
				num = Program.mainForm.Width - (int)position.x;
				result = Task_CollectWindow.ScreenDirection.Right;
				targetPos = new Vector2(Program.mainForm.Width + 50, SamMath.Lerp(position.y, Program.mainForm.Height / 2, 0.4f));
			}
			if (canExitTop && (float)num > position.y)
			{
				result = Task_CollectWindow.ScreenDirection.Top;
				targetPos = new Vector2(SamMath.Lerp(position.x, Program.mainForm.Width / 2, 0.4f), -50f);
			}
			return result;
		}

		private static void SolveFeet()
		{
			Vector2.GetFromAngleDegrees(direction);
			Vector2.GetFromAngleDegrees(direction + 90f);
			Vector2 footHome = GetFootHome(rightFoot: false);
			Vector2 footHome2 = GetFootHome(rightFoot: true);
			if (lFootMoveTimeStart < 0f && rFootMoveTimeStart < 0f)
			{
				if (Vector2.Distance(lFootPos, footHome) > 5f)
				{
					lFootMoveOrigin = lFootPos;
					lFootMoveDir = Vector2.Normalize(footHome - lFootPos);
					lFootMoveTimeStart = Time.time;
				}
				else if (Vector2.Distance(rFootPos, footHome2) > 5f)
				{
					rFootMoveOrigin = rFootPos;
					rFootMoveDir = Vector2.Normalize(footHome2 - rFootPos);
					rFootMoveTimeStart = Time.time;
				}
			}
			else if (lFootMoveTimeStart > 0f)
			{
				Vector2 b = footHome + lFootMoveDir * 0.4f * 5f;
				if (Time.time > lFootMoveTimeStart + stepTime)
				{
					lFootPos = b;
					lFootMoveTimeStart = -1f;
					Sound.PlayPat();
					if (Time.time < trackMudEndTime)
					{
						AddFootMark(lFootPos);
					}
				}
				else
				{
					float p = (Time.time - lFootMoveTimeStart) / stepTime;
					lFootPos = Vector2.Lerp(lFootMoveOrigin, b, Easings.CubicEaseInOut(p));
				}
			}
			else
			{
				if (!(rFootMoveTimeStart > 0f))
				{
					return;
				}
				Vector2 b2 = footHome2 + rFootMoveDir * 0.4f * 5f;
				if (Time.time > rFootMoveTimeStart + stepTime)
				{
					rFootPos = b2;
					rFootMoveTimeStart = -1f;
					Sound.PlayPat();
					if (Time.time < trackMudEndTime)
					{
						AddFootMark(rFootPos);
					}
				}
				else
				{
					float p2 = (Time.time - rFootMoveTimeStart) / stepTime;
					rFootPos = Vector2.Lerp(rFootMoveOrigin, b2, Easings.CubicEaseInOut(p2));
				}
			}
		}

		private static Vector2 GetFootHome(bool rightFoot)
		{
			float num = (rightFoot ? 1 : 0);
			Vector2 vector = Vector2.GetFromAngleDegrees(direction + 90f) * num;
			return position + vector * 6f;
		}

		private static void AddFootMark(Vector2 markPos)
		{
			footMarks[footMarkIndex].time = Time.time;
			footMarks[footMarkIndex].position = markPos;
			footMarkIndex++;
			if (footMarkIndex >= footMarks.Length)
			{
				footMarkIndex = 0;
			}
		}

		public static void UpdateRig()
		{
			float num = direction;
			int num2 = (int)position.x;
			int num3 = (int)position.y;
			Vector2 vector = new Vector2(num2, num3);
			Vector2 vector2 = new Vector2(1.3f, 0.4f);
			Vector2 fromAngleDegrees = Vector2.GetFromAngleDegrees(num);
			_ = fromAngleDegrees * vector2;
			_ = Vector2.GetFromAngleDegrees(num + 90f) * vector2;
			Vector2 vector3 = new Vector2(0f, -1f);
			gooseRig.underbodyCenter = vector + vector3 * 9f;
			gooseRig.bodyCenter = vector + vector3 * 14f;
			int num4 = (int)SamMath.Lerp(20f, 10f, gooseRig.neckLerpPercent);
			int num5 = (int)SamMath.Lerp(3f, 16f, gooseRig.neckLerpPercent);
			gooseRig.neckCenter = vector + vector3 * (14 + num4);
			gooseRig.neckBase = gooseRig.bodyCenter + fromAngleDegrees * 15f;
			gooseRig.neckHeadPoint = gooseRig.neckBase + fromAngleDegrees * num5 + vector3 * num4;
			gooseRig.head1EndPoint = gooseRig.neckHeadPoint + fromAngleDegrees * 3f - vector3 * 1f;
			gooseRig.head2EndPoint = gooseRig.head1EndPoint + fromAngleDegrees * 5f;
		}

		public static void Render(Graphics g)
		{
			for (int i = 0; i < footMarks.Length; i++)
			{
				if (footMarks[i].time != 0f)
				{
					float num = footMarks[i].time + 8.5f;
					float p = SamMath.Clamp(Time.time - num, 0f, 1f) / 1f;
					float num2 = SamMath.Lerp(3f, 0f, p);
					FillCircleFromCenter(g, Brushes.SaddleBrown, footMarks[i].position, (int)num2);
				}
			}
			UpdateRig();
			float num3 = direction;
			int num4 = (int)position.x;
			int num5 = (int)position.y;
			Vector2 vector = new Vector2(num4, num5);
			Vector2 vector2 = new Vector2(1.3f, 0.4f);
			Vector2 fromAngleDegrees = Vector2.GetFromAngleDegrees(num3);
			_ = fromAngleDegrees * vector2;
			Vector2 fromAngleDegrees2 = Vector2.GetFromAngleDegrees(num3 + 90f);
			_ = fromAngleDegrees2 * vector2;
			Vector2 vector3 = new Vector2(0f, -1f);
			int num6 = 2;
			DrawingPen.Brush = brushGooseWhite;
			FillCircleFromCenter(g, brushGooseOrange, lFootPos, 4);
			FillCircleFromCenter(g, brushGooseOrange, rFootPos, 4);
			FillEllipseFromCenter(g, shadowBrush, (int)vector.x, (int)vector.y, 20, 15);
			DrawingPen.Color = brushGooseOutline.Color;
			DrawingPen.Width = 22 + num6;
			g.DrawLine(DrawingPen, ToIntPoint(gooseRig.bodyCenter + fromAngleDegrees * 11f), ToIntPoint(gooseRig.bodyCenter - fromAngleDegrees * 11f));
			DrawingPen.Width = 13 + num6;
			g.DrawLine(DrawingPen, ToIntPoint(gooseRig.neckBase), ToIntPoint(gooseRig.neckHeadPoint));
			DrawingPen.Width = 15 + num6;
			g.DrawLine(DrawingPen, ToIntPoint(gooseRig.neckHeadPoint), ToIntPoint(gooseRig.head1EndPoint));
			DrawingPen.Width = 10 + num6;
			g.DrawLine(DrawingPen, ToIntPoint(gooseRig.head1EndPoint), ToIntPoint(gooseRig.head2EndPoint));
			DrawingPen.Color = brushGooseOutline.Color;
			DrawingPen.Width = 15f;
			g.DrawLine(DrawingPen, ToIntPoint(gooseRig.underbodyCenter + fromAngleDegrees * 7f), ToIntPoint(gooseRig.underbodyCenter - fromAngleDegrees * 7f));
			DrawingPen.Color = brushGooseWhite.Color;
			DrawingPen.Width = 22f;
			g.DrawLine(DrawingPen, ToIntPoint(gooseRig.bodyCenter + fromAngleDegrees * 11f), ToIntPoint(gooseRig.bodyCenter - fromAngleDegrees * 11f));
			DrawingPen.Width = 13f;
			g.DrawLine(DrawingPen, ToIntPoint(gooseRig.neckBase), ToIntPoint(gooseRig.neckHeadPoint));
			DrawingPen.Width = 15f;
			g.DrawLine(DrawingPen, ToIntPoint(gooseRig.neckHeadPoint), ToIntPoint(gooseRig.head1EndPoint));
			DrawingPen.Width = 10f;
			g.DrawLine(DrawingPen, ToIntPoint(gooseRig.head1EndPoint), ToIntPoint(gooseRig.head2EndPoint));
			int num7 = 9;
			int num8 = 3;
			DrawingPen.Width = num7;
			DrawingPen.Brush = brushGooseOrange;
			Vector2 vector4 = gooseRig.head2EndPoint + fromAngleDegrees * num8;
			g.DrawLine(DrawingPen, ToIntPoint(gooseRig.head2EndPoint), ToIntPoint(vector4));
			Vector2 pos = gooseRig.neckHeadPoint + vector3 * 3f + -fromAngleDegrees2 * vector2 * 5f + fromAngleDegrees * 5f;
			Vector2 pos2 = gooseRig.neckHeadPoint + vector3 * 3f + fromAngleDegrees2 * vector2 * 5f + fromAngleDegrees * 5f;
			FillCircleFromCenter(g, Brushes.Black, pos, 2);
			FillCircleFromCenter(g, Brushes.Black, pos2, 2);
		}

		public static void FillCircleFromCenter(Graphics g, Brush brush, Vector2 pos, int radius)
		{
			FillEllipseFromCenter(g, brush, (int)pos.x, (int)pos.y, radius, radius);
		}

		public static void FillCircleFromCenter(Graphics g, Brush brush, int x, int y, int radius)
		{
			FillEllipseFromCenter(g, brush, x, y, radius, radius);
		}

		public static void FillEllipseFromCenter(Graphics g, Brush brush, int x, int y, int xRadius, int yRadius)
		{
			g.FillEllipse(brush, x - xRadius, y - yRadius, xRadius * 2, yRadius * 2);
		}

		public static void FillEllipseFromCenter(Graphics g, Brush brush, Vector2 position, Vector2 xyRadius)
		{
			g.FillEllipse(brush, position.x - xyRadius.x, position.y - xyRadius.y, xyRadius.x * 2f, xyRadius.y * 2f);
		}

		private static Point ToIntPoint(Vector2 vector)
		{
			return new Point((int)vector.x, (int)vector.y);
		}
	}
}
