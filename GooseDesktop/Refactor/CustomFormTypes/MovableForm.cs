using System.Drawing;
using System.Windows.Forms;
using GooseShared;

namespace GooseDesktop.Refactor.CustomFormTypes
{
	internal class MovableForm : Form
	{
		public readonly GooseEntity ownerGoose;

		public MovableForm(GooseEntity owner)
		{
			ownerGoose = owner;
			base.StartPosition = FormStartPosition.Manual;
			base.Width = 400;
			base.Height = 400;
			BackColor = Color.DimGray;
			base.Icon = null;
			base.ShowIcon = false;
			SetWindowResizableThreadsafe(canResize: false);
		}

		public void SetWindowPositionThreadsafe(Point p)
		{
			if (base.InvokeRequired)
			{
				BeginInvoke((MethodInvoker)delegate
				{
					base.Location = p;
					base.TopMost = true;
				});
			}
			else
			{
				base.Location = p;
				base.TopMost = true;
			}
		}

		public void SetWindowResizableThreadsafe(bool canResize)
		{
			if (base.InvokeRequired)
			{
				BeginInvoke((MethodInvoker)delegate
				{
					base.FormBorderStyle = ((!canResize) ? FormBorderStyle.FixedSingle : FormBorderStyle.Sizable);
					MovableForm movableForm = this;
					bool maximizeBox2 = (base.MinimizeBox = canResize);
					movableForm.MaximizeBox = maximizeBox2;
				});
			}
			else
			{
				base.FormBorderStyle = ((!canResize) ? FormBorderStyle.FixedSingle : FormBorderStyle.Sizable);
				bool maximizeBox = (base.MinimizeBox = canResize);
				base.MaximizeBox = maximizeBox;
			}
		}
	}
}
