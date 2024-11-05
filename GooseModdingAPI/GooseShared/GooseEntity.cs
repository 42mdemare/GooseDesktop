using System.Collections.Generic;
using System.Drawing;
using SamEngine;

namespace GooseShared
{
	public class GooseEntity
	{
		public delegate void TickFunction(GooseEntity g);

		public delegate void UpdateRigFunction(Rig rig, Vector2 centerPosition, float direction);

		public delegate void RenderFunction(GooseEntity g, Graphics gfx);

		public enum SpeedTiers
		{
			Walk,
			Run,
			Charge
		}

		public class ParametersTable
		{
			public float WalkSpeed = 80f;

			public float RunSpeed = 200f;

			public float ChargeSpeed = 400f;

			public float AccelerationNormal = 1300f;

			public float AccelerationCharged = 2300f;

			public float StopRadius = -10f;

			public float StepTimeNormal = 0.2f;

			public float StepTimeCharged = 0.1f;

			public float DurationToTrackMud = 15f;
		}

		public TickFunction tick;

		public UpdateRigFunction updateRig;

		public RenderFunction render;

		public ParametersTable parameters;

		public GooseRenderData renderData;

		public Vector2 position = new Vector2(300f, 300f);

		public Vector2 velocity = new Vector2(0f, 0f);

		public float direction = 90f;

		public Vector2 targetDirection;

		public bool extendingNeck;

		public Vector2 targetPos = new Vector2(300f, 300f);

		public float currentSpeed;

		public float currentAcceleration;

		public float stepInterval;

		public bool canDecelerateImmediately = true;

		public float trackMudEndTime = -1f;

		public FootMark[] footMarks = new FootMark[64];

		public int footMarkIndex;

		public Rig rig = new Rig();

		public int currentTask = -1;

		public GooseTaskData currentTaskData;

		public List<int> taskIndexQueue;

		public GooseEntity(TickFunction t, UpdateRigFunction ur, RenderFunction r)
		{
			tick = t;
			updateRig = ur;
			render = r;
		}
	}
}
