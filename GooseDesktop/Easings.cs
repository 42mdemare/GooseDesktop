using System;

public static class Easings
{
	public enum Functions
	{
		Linear,
		QuadraticEaseIn,
		QuadraticEaseOut,
		QuadraticEaseInOut,
		CubicEaseIn,
		CubicEaseOut,
		CubicEaseInOut,
		QuarticEaseIn,
		QuarticEaseOut,
		QuarticEaseInOut,
		QuinticEaseIn,
		QuinticEaseOut,
		QuinticEaseInOut,
		SineEaseIn,
		SineEaseOut,
		SineEaseInOut,
		CircularEaseIn,
		CircularEaseOut,
		CircularEaseInOut,
		ExponentialEaseIn,
		ExponentialEaseOut,
		ExponentialEaseInOut,
		ElasticEaseIn,
		ElasticEaseOut,
		ElasticEaseInOut,
		BackEaseIn,
		BackEaseOut,
		BackEaseInOut,
		BounceEaseIn,
		BounceEaseOut,
		BounceEaseInOut
	}

	private const float PI = (float)Math.PI;

	private const float HALFPI = (float)Math.PI / 2f;

	public static float Interpolate(float p, Functions function)
	{
		return function switch
		{
			Functions.QuadraticEaseOut => QuadraticEaseOut(p), 
			Functions.QuadraticEaseIn => QuadraticEaseIn(p), 
			Functions.QuadraticEaseInOut => QuadraticEaseInOut(p), 
			Functions.CubicEaseIn => CubicEaseIn(p), 
			Functions.CubicEaseOut => CubicEaseOut(p), 
			Functions.CubicEaseInOut => CubicEaseInOut(p), 
			Functions.QuarticEaseIn => QuarticEaseIn(p), 
			Functions.QuarticEaseOut => QuarticEaseOut(p), 
			Functions.QuarticEaseInOut => QuarticEaseInOut(p), 
			Functions.QuinticEaseIn => QuinticEaseIn(p), 
			Functions.QuinticEaseOut => QuinticEaseOut(p), 
			Functions.QuinticEaseInOut => QuinticEaseInOut(p), 
			Functions.SineEaseIn => SineEaseIn(p), 
			Functions.SineEaseOut => SineEaseOut(p), 
			Functions.SineEaseInOut => SineEaseInOut(p), 
			Functions.CircularEaseIn => CircularEaseIn(p), 
			Functions.CircularEaseOut => CircularEaseOut(p), 
			Functions.CircularEaseInOut => CircularEaseInOut(p), 
			Functions.ExponentialEaseIn => ExponentialEaseIn(p), 
			Functions.ExponentialEaseOut => ExponentialEaseOut(p), 
			Functions.ExponentialEaseInOut => ExponentialEaseInOut(p), 
			Functions.ElasticEaseIn => ElasticEaseIn(p), 
			Functions.ElasticEaseOut => ElasticEaseOut(p), 
			Functions.ElasticEaseInOut => ElasticEaseInOut(p), 
			Functions.BackEaseIn => BackEaseIn(p), 
			Functions.BackEaseOut => BackEaseOut(p), 
			Functions.BackEaseInOut => BackEaseInOut(p), 
			Functions.BounceEaseIn => BounceEaseIn(p), 
			Functions.BounceEaseOut => BounceEaseOut(p), 
			Functions.BounceEaseInOut => BounceEaseInOut(p), 
			_ => Linear(p), 
		};
	}

	public static float Linear(float p)
	{
		return p;
	}

	public static float QuadraticEaseIn(float p)
	{
		return p * p;
	}

	public static float QuadraticEaseOut(float p)
	{
		return 0f - p * (p - 2f);
	}

	public static float QuadraticEaseInOut(float p)
	{
		if (p < 0.5f)
		{
			return 2f * p * p;
		}
		return -2f * p * p + 4f * p - 1f;
	}

	public static float CubicEaseIn(float p)
	{
		return p * p * p;
	}

	public static float CubicEaseOut(float p)
	{
		float num = p - 1f;
		return num * num * num + 1f;
	}

	public static float CubicEaseInOut(float p)
	{
		if (p < 0.5f)
		{
			return 4f * p * p * p;
		}
		float num = 2f * p - 2f;
		return 0.5f * num * num * num + 1f;
	}

	public static float QuarticEaseIn(float p)
	{
		return p * p * p * p;
	}

	public static float QuarticEaseOut(float p)
	{
		float num = p - 1f;
		return num * num * num * (1f - p) + 1f;
	}

	public static float QuarticEaseInOut(float p)
	{
		if (p < 0.5f)
		{
			return 8f * p * p * p * p;
		}
		float num = p - 1f;
		return -8f * num * num * num * num + 1f;
	}

	public static float QuinticEaseIn(float p)
	{
		return p * p * p * p * p;
	}

	public static float QuinticEaseOut(float p)
	{
		float num = p - 1f;
		return num * num * num * num * num + 1f;
	}

	public static float QuinticEaseInOut(float p)
	{
		if (p < 0.5f)
		{
			return 16f * p * p * p * p * p;
		}
		float num = 2f * p - 2f;
		return 0.5f * num * num * num * num * num + 1f;
	}

	public static float SineEaseIn(float p)
	{
		return (float)Math.Sin((p - 1f) * ((float)Math.PI / 2f)) + 1f;
	}

	public static float SineEaseOut(float p)
	{
		return (float)Math.Sin(p * ((float)Math.PI / 2f));
	}

	public static float SineEaseInOut(float p)
	{
		return 0.5f * (1f - (float)Math.Cos(p * (float)Math.PI));
	}

	public static float CircularEaseIn(float p)
	{
		return 1f - (float)Math.Sqrt(1f - p * p);
	}

	public static float CircularEaseOut(float p)
	{
		return (float)Math.Sqrt((2f - p) * p);
	}

	public static float CircularEaseInOut(float p)
	{
		if (p < 0.5f)
		{
			return 0.5f * (1f - (float)Math.Sqrt(1f - 4f * (p * p)));
		}
		return 0.5f * ((float)Math.Sqrt((0f - (2f * p - 3f)) * (2f * p - 1f)) + 1f);
	}

	public static float ExponentialEaseIn(float p)
	{
		if (p != 0f)
		{
			return (float)Math.Pow(2.0, 10f * (p - 1f));
		}
		return p;
	}

	public static float ExponentialEaseOut(float p)
	{
		if (p != 1f)
		{
			return 1f - (float)Math.Pow(2.0, -10f * p);
		}
		return p;
	}

	public static float ExponentialEaseInOut(float p)
	{
		if ((double)p == 0.0 || (double)p == 1.0)
		{
			return p;
		}
		if (p < 0.5f)
		{
			return 0.5f * (float)Math.Pow(2.0, 20f * p - 10f);
		}
		return -0.5f * (float)Math.Pow(2.0, -20f * p + 10f) + 1f;
	}

	public static float ElasticEaseIn(float p)
	{
		return (float)Math.Sin(20.4203529f * p) * (float)Math.Pow(2.0, 10f * (p - 1f));
	}

	public static float ElasticEaseOut(float p)
	{
		return (float)Math.Sin(-20.4203529f * (p + 1f)) * (float)Math.Pow(2.0, -10f * p) + 1f;
	}

	public static float ElasticEaseInOut(float p)
	{
		if (p < 0.5f)
		{
			return 0.5f * (float)Math.Sin(20.4203529f * (2f * p)) * (float)Math.Pow(2.0, 10f * (2f * p - 1f));
		}
		return 0.5f * ((float)Math.Sin(-20.4203529f * (2f * p - 1f + 1f)) * (float)Math.Pow(2.0, -10f * (2f * p - 1f)) + 2f);
	}

	public static float BackEaseIn(float p)
	{
		return p * p * p - p * (float)Math.Sin(p * (float)Math.PI);
	}

	public static float BackEaseOut(float p)
	{
		float num = 1f - p;
		return 1f - (num * num * num - num * (float)Math.Sin(num * (float)Math.PI));
	}

	public static float BackEaseInOut(float p)
	{
		if (p < 0.5f)
		{
			float num = 2f * p;
			return 0.5f * (num * num * num - num * (float)Math.Sin(num * (float)Math.PI));
		}
		float num2 = 1f - (2f * p - 1f);
		return 0.5f * (1f - (num2 * num2 * num2 - num2 * (float)Math.Sin(num2 * (float)Math.PI))) + 0.5f;
	}

	public static float BounceEaseIn(float p)
	{
		return 1f - BounceEaseOut(1f - p);
	}

	public static float BounceEaseOut(float p)
	{
		if (p < 0.363636374f)
		{
			return 121f * p * p / 16f;
		}
		if (p < 0.727272749f)
		{
			return 9.075f * p * p - 9.9f * p + 3.4f;
		}
		if (p < 0.9f)
		{
			return 12.0664816f * p * p - 19.635458f * p + 8.898061f;
		}
		return 10.8f * p * p - 20.52f * p + 10.72f;
	}

	public static float BounceEaseInOut(float p)
	{
		if (p < 0.5f)
		{
			return 0.5f * BounceEaseIn(p * 2f);
		}
		return 0.5f * BounceEaseOut(p * 2f - 1f) + 0.5f;
	}
}
