using System.Windows.Forms;

namespace GooseDesktop
{
	public class BufferedPanel : Panel
	{
		public BufferedPanel()
		{
			DoubleBuffered = true;
		}
	}
}
