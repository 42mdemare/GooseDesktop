using System.Drawing;
using System.Windows.Forms;
using SamEngine;

namespace GooseDesktop.Refactor
{
	internal class EscToQuitOverlay
	{
		private static float curQuitAlpha = 0f;

		private static Font showCurQuitFont = new Font("Arial", 12f, FontStyle.Bold);

		public static void UpdateAndDraw(Graphics g)
		{
			if (Program.GetAsyncKeyState(Keys.Escape) != 0)
			{
				curQuitAlpha += 0.00216666679f;
			}
			else
			{
				curQuitAlpha -= 0.0166666675f;
			}
			curQuitAlpha = SamMath.Clamp(curQuitAlpha, 0f, 1f);
			if (curQuitAlpha > 0.05f)
			{
				float num = (curQuitAlpha - 0.1f) / 0.9f;
				int num2 = (int)SamMath.Lerp(-15f, 10f, Easings.ExponentialEaseOut(num * 2f));
				SizeF sizeF = g.MeasureString("Continuez à maintenir la touche ESC pour expulser l'oie", showCurQuitFont, int.MaxValue);
				g.FillRectangle(Brushes.LightBlue, new Rectangle(5, num2 - 5, (int)sizeF.Width + 10, (int)sizeF.Height + 10));
				g.FillRectangle(Brushes.LightPink, new Rectangle(5, num2 - 5, (int)SamMath.Lerp(0f, sizeF.Width + 10f, num), (int)sizeF.Height + 10));
				SolidBrush solidBrush = new SolidBrush(Color.FromArgb(255, (int)(256f * curQuitAlpha), (int)(256f * curQuitAlpha), (int)(256f * curQuitAlpha)));
				g.DrawString("Continuez à maintenir la touche ESC pour expulser l'oie", showCurQuitFont, solidBrush, 10f, num2);
				solidBrush.Dispose();
			}
			if (curQuitAlpha > 0.99f)
			{
				OSFunctions.ClearCursorClip();
				Application.Exit();
			}
		}
	}
}
