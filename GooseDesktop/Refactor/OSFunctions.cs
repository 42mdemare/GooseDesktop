using System.Drawing;
using System.Windows.Forms;
using SamEngine;

namespace GooseDesktop.Refactor
{
	public static class OSFunctions
	{
		public static void RefreshInput()
		{
			Input.leftMouseButton.Update((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left);
			Input.mouseX = Cursor.Position.X;
			Input.mouseY = Cursor.Position.Y;
		}

		public static void ClearCursorClip()
		{
			Cursor.Clip = Rectangle.Empty;
		}
	}
}
