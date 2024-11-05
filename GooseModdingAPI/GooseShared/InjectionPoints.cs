using System.Drawing;

namespace GooseShared
{
	public class InjectionPoints
	{
		public delegate void PostModLoadEventHandler();

		public delegate void PreTickEventHandler(GooseEntity goose);

		public delegate void PostTickEventHandler(GooseEntity goose);

		public delegate void PreUpdateRigEventHandler(GooseEntity goose);

		public delegate void PostUpdateRigEventHandler(GooseEntity goose);

		public delegate void PreRenderEventHandler(GooseEntity goose, Graphics g);

		public delegate void PostRenderEventHandler(GooseEntity goose, Graphics g);

		public static event PostModLoadEventHandler PostModsLoaded;

		public static event PreTickEventHandler PreTickEvent;

		public static event PostTickEventHandler PostTickEvent;

		public static event PreUpdateRigEventHandler PreUpdateRigEvent;

		public static event PostUpdateRigEventHandler PostUpdateRigEvent;

		public static event PreRenderEventHandler PreRenderEvent;

		public static event PostRenderEventHandler PostRenderEvent;

		public static void RaisePostModLoad()
		{
			InjectionPoints.PostModsLoaded?.Invoke();
		}

		public static void RaisePreTick(GooseEntity goose)
		{
			InjectionPoints.PreTickEvent?.Invoke(goose);
		}

		public static void RaisePostTick(GooseEntity goose)
		{
			InjectionPoints.PostTickEvent?.Invoke(goose);
		}

		public static void RaisePreUpdateRig(GooseEntity goose)
		{
			InjectionPoints.PreUpdateRigEvent?.Invoke(goose);
		}

		public static void RaisePostUpdateRig(GooseEntity goose)
		{
			InjectionPoints.PostUpdateRigEvent?.Invoke(goose);
		}

		public static void RaisePreRender(GooseEntity goose, Graphics g)
		{
			InjectionPoints.PreRenderEvent?.Invoke(goose, g);
		}

		public static void RaisePostRender(GooseEntity goose, Graphics g)
		{
			InjectionPoints.PostRenderEvent?.Invoke(goose, g);
		}
	}
}
