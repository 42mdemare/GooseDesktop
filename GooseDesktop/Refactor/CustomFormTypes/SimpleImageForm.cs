using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GooseShared;
using SamEngine;

namespace GooseDesktop.Refactor.CustomFormTypes
{
	internal class SimpleImageForm : MovableForm
	{
		private static readonly string memesRootFolder = Program.GetPathToFileInAssembly("Assets/Images/Memes/");

		private static string[] imageURLs = new string[5] { "https://preview.redd.it/dsfjv8aev0p31.png?width=960&crop=smart&auto=webp&s=1d58948acc5c6dd60df1092c1bd2a59a509069fd", "https://i.redd.it/4ojv59zvglp31.jpg", "https://i.redd.it/4bamd6lnso241.jpg", "https://i.redd.it/5i5et9p1vsp31.jpg", "https://i.redd.it/j2f1i9djx5p31.jpg" };

		private static Deck imageURLDeck = new Deck(imageURLs.Length);

		public SimpleImageForm(GooseEntity ownerGoose)
			: base(ownerGoose)
		{
			PictureBox pictureBox = new PictureBox
			{
				Dock = DockStyle.Fill
			};
			try
			{
				string[] files = Directory.GetFiles(memesRootFolder);
				pictureBox.Image = Image.FromFile(files[(int)SamMath.RandomRange(0f, files.Length)]);
			}
			catch
			{
				MessageBox.Show("COULD NOT FIND THE DANG IMAGE MEME");
				pictureBox.LoadAsync(imageURLs[imageURLDeck.Next()]);
			}
			pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
			base.Controls.Add(pictureBox);
		}
	}
}
