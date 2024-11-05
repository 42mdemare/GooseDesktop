using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SamEngine;

namespace GooseDesktop
{
	public static class MainGame
	{
		private static float curQuitAlpha = 0f;

		private static Font showCurQuitFont = new Font("Arial", 12f, FontStyle.Bold);

		public static void Init()
		{
			string pathToFileInAssembly = Program.GetPathToFileInAssembly("Assets/Images/Memes/");
			try
			{
				Directory.GetFiles(pathToFileInAssembly);
			}
			catch
			{
				MessageBox.Show("Warning: Some assets expected at the path: \n\n'" + pathToFileInAssembly + "' \n\ncannot be found. \n\nYour .exe should ideally be next to an Assets folder and config, all bundled together!\n\nPlease make sure you extracted the zip file, with the whole folder together, to a known location like Documents or Desktop- and we didn't end up somewhere random.\n\nGoose will still work, but he won't be able to use custom memes or any of that fanciness.\nHold ESC for several seconds to quit.");
			}
			GooseConfig.LoadConfig();
			Sound.Init();
			TheGoose.Init();
		}

        public static void Update(Graphics g)
		{
			Time.TickTime();
			if (Program.GetAsyncKeyState(Keys.Escape) != 0)
			{
				curQuitAlpha += 0.00216666679f;
            }
			else
			{
				curQuitAlpha -= 0.0166666675f;
			}
			curQuitAlpha = SamMath.Clamp(curQuitAlpha, 0f, 1f);
			if (curQuitAlpha > 0.2f)
			{
				float num = (curQuitAlpha - 0.2f) / 0.8f;
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
				Application.Exit();
			}
			TheGoose.Tick();
			TheGoose.Render(g);
		}
	}
}
