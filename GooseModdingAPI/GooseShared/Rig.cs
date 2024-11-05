using SamEngine;

namespace GooseShared
{
	public class Rig
	{
		public ProceduralFeets feets;

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
}
