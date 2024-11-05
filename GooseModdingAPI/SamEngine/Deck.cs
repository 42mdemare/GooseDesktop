namespace SamEngine
{
	public class Deck
	{
		public int[] indices;

		private int i;

		public Deck(int Length)
		{
			indices = new int[Length];
			Reshuffle();
		}

		public void Reshuffle()
		{
			for (int i = 0; i < indices.Length; i++)
			{
				indices[i] = i;
				int num = (int)SamMath.RandomRange(0f, i);
				int num2 = indices[i];
				indices[i] = indices[num];
				indices[num] = num2;
			}
		}

		public int Next()
		{
			int result = indices[i];
			i++;
			if (i >= indices.Length)
			{
				Reshuffle();
				i = 0;
			}
			return result;
		}
	}
}
