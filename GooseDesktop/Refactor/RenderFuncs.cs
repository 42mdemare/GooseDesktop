using System.Drawing;
using SamEngine;

namespace GooseDesktop.Refactor
{
	public static class RenderFuncs
	{
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

		public static Point ToIntPoint(Vector2 vector)
		{
			return new Point((int)vector.x, (int)vector.y);
		}
	}
}
