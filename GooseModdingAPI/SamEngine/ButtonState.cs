namespace SamEngine
{
	public struct ButtonState
	{
		public bool Held;

		public bool Clicked;

		public bool Released;

		public void Update(bool heldThisFrame)
		{
			Clicked = heldThisFrame && !Held;
			Released = !heldThisFrame && Held;
			Held = heldThisFrame;
		}
	}
}
