using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using GooseDesktop.Refactor;

namespace GooseDesktop
{
	internal static class Program
	{
		public struct NativeMessage
		{
			public IntPtr Handle;

			public uint Message;

			public IntPtr WParameter;

			public IntPtr LParameter;

			public uint Time;

			public Point Location;
		}

		public const int GWL_EXSTYLE = -20;

		private const int WS_EX_LAYERED = 524288;

		private const int WS_EX_TRANSPARENT = 32;

		private const int LWA_ALPHA = 2;

		private const int LWA_COLORKEY = 1;

		private static IntPtr OriginalWindowStyle;

		private static IntPtr PassthruWindowStyle;

		private static BufferedPanel canvas;

		public static Color ColorKey = Color.Coral;

		public static Form mainForm;

		[DllImport("user32.dll", SetLastError = true)]
		private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll")]
		private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

		[DllImport("user32.dll")]
		private static extern int PeekMessage(out NativeMessage message, IntPtr window, uint filterMin, uint filterMax, uint remove);

		[DllImport("user32.dll")]
		public static extern short GetAsyncKeyState(Keys vKey);

		[STAThread]
		private static void Main()
		{
			mainForm = new Form();
			mainForm.BackColor = ColorKey;
			mainForm.FormBorderStyle = FormBorderStyle.None;
			mainForm.Size = Screen.PrimaryScreen.WorkingArea.Size;
			mainForm.StartPosition = FormStartPosition.Manual;
			mainForm.Location = new Point(0, 0);
			mainForm.TopMost = true;
			mainForm.AllowTransparency = true;
			mainForm.BackColor = ColorKey;
			mainForm.TransparencyKey = ColorKey;
			mainForm.ShowIcon = false;
			mainForm.ShowInTaskbar = false;
			OriginalWindowStyle = (IntPtr)GetWindowLong(mainForm.Handle, -20);
			PassthruWindowStyle = (IntPtr)(GetWindowLong(mainForm.Handle, -20) | 0x80000 | 0x20);
			SetWindowPassthru(passthrough: true);
			canvas = new BufferedPanel();
			canvas.Dock = DockStyle.Fill;
			canvas.BackColor = Color.Transparent;
			canvas.BringToFront();
			canvas.Paint += Render;
			mainForm.Controls.Add(canvas);
			IPCManager.Init();
			GooseDesktop.Refactor.MainGame.Init();
			Application.Idle += HandleApplicationIdle;
			Application.EnableVisualStyles();
			Application.Run(mainForm);
		}

		private static void SetWindowPassthru(bool passthrough)
		{
			if (passthrough)
			{
				SetWindowLong(mainForm.Handle, -20, PassthruWindowStyle);
			}
			else
			{
				SetWindowLong(mainForm.Handle, -20, OriginalWindowStyle);
			}
		}

		public static string GetPathToFileInAssembly(string relativePath)
		{
			return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), relativePath);
		}

		private static bool IsApplicationIdle()
		{
			NativeMessage message;
			return PeekMessage(out message, IntPtr.Zero, 0u, 0u, 0u) == 0;
		}

		private static void HandleApplicationIdle(object sender, EventArgs e)
		{
			while (IsApplicationIdle())
			{
				mainForm.TopMost = true;
				canvas.BringToFront();
				canvas.Invalidate();
				Thread.Sleep(8);
			}
		}

		private static void Render(object sender, PaintEventArgs e)
		{
			GooseDesktop.Refactor.MainGame.TickGame(e.Graphics);
		}

		public static void OpenSubform(Form f)
		{
			mainForm.IsMdiContainer = true;
			f.MdiParent = mainForm;
			f.Show();
		}
	}
}
