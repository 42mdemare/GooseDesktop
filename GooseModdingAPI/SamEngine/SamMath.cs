using System;

namespace SamEngine
{
	public static class SamMath
	{
		public const float Deg2Rad = (float)Math.PI / 180f;

		public const float Rad2Deg = 180f / (float)Math.PI;

		public static Random Rand = new Random();

		public static float RandomRange(float min, float max)
		{
			return min + (float)Rand.NextDouble() * (max - min);
		}

		public static float Lerp(float a, float b, float p)
		{
			return a * (1f - p) + b * p;
		}

		public static float Clamp(float a, float min, float max)
		{
			return Math.Min(Math.Max(a, min), max);
		}
	}
}
