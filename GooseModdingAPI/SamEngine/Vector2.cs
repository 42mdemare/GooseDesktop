using System;

namespace SamEngine
{
	public struct Vector2
	{
		public float x;

		public float y;

		public static readonly Vector2 zero = new Vector2(0f, 0f);

		public Vector2(float _x, float _y)
		{
			x = _x;
			y = _y;
		}

		public static Vector2 operator +(Vector2 a, Vector2 b)
		{
			return new Vector2(a.x + b.x, a.y + b.y);
		}

		public static Vector2 operator -(Vector2 a, Vector2 b)
		{
			return new Vector2(a.x - b.x, a.y - b.y);
		}

		public static Vector2 operator -(Vector2 a)
		{
			return a * -1f;
		}

		public static Vector2 operator *(Vector2 a, Vector2 b)
		{
			return new Vector2(a.x * b.x, a.y * b.y);
		}

		public static Vector2 operator *(Vector2 a, float b)
		{
			return new Vector2(a.x * b, a.y * b);
		}

		public static Vector2 operator /(Vector2 a, float b)
		{
			return new Vector2(a.x / b, a.y / b);
		}

		public static Vector2 GetFromAngleDegrees(float angle)
		{
			return new Vector2((float)Math.Cos(angle * ((float)Math.PI / 180f)), (float)Math.Sin(angle * ((float)Math.PI / 180f)));
		}

		public static float Distance(Vector2 a, Vector2 b)
		{
			Vector2 vector = new Vector2(a.x - b.x, a.y - b.y);
			return (float)Math.Sqrt(vector.x * vector.x + vector.y * vector.y);
		}

		public static Vector2 Lerp(Vector2 a, Vector2 b, float p)
		{
			return new Vector2(SamMath.Lerp(a.x, b.x, p), SamMath.Lerp(a.y, b.y, p));
		}

		public static float Dot(Vector2 a, Vector2 b)
		{
			return a.x * b.x + a.y * b.y;
		}

		public static Vector2 Normalize(Vector2 a)
		{
			if (a.x == 0f && a.y == 0f)
			{
				return zero;
			}
			float num = (float)Math.Sqrt(a.x * a.x + a.y * a.y);
			return new Vector2(a.x / num, a.y / num);
		}

		public static float Magnitude(Vector2 a)
		{
			return (float)Math.Sqrt(a.x * a.x + a.y * a.y);
		}

		public static Vector2 ClampMagnitude(Vector2 a, float l)
		{
			if (Magnitude(a) > l)
			{
				a = Normalize(a) * l;
			}
			return a;
		}
	}
}
