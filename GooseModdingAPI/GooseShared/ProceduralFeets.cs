using SamEngine;

namespace GooseShared
{
	public class ProceduralFeets
	{
		public Vector2 lFootPos;

		public Vector2 rFootPos;

		public float lFootMoveTimeStart = -1f;

		public float rFootMoveTimeStart = -1f;

		public Vector2 lFootMoveOrigin;

		public Vector2 rFootMoveOrigin;

		public Vector2 lFootMoveDir;

		public Vector2 rFootMoveDir;

		public int feetDistanceApart = 6;

		public const float wantStepAtDistance = 5f;

		public const float overshootFraction = 0.4f;
	}
}
