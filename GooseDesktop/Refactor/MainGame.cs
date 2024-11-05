using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GooseDesktop.Refactor.GooseTasks;
using GooseDesktop.Refactor.GooseTasks.Tasks;
using GooseShared;
using SamEngine;

namespace GooseDesktop.Refactor
{
	internal class MainGame
	{
		public static GooseEntity goose;

		public static void Init()
		{
			string pathToFileInAssembly = Program.GetPathToFileInAssembly("Assets/Images/Memes/");
			try
			{
				Directory.GetFiles(pathToFileInAssembly);
			}
			catch
			{
				MessageBox.Show("Warning: Some assets expected at the path: \n\n'" + pathToFileInAssembly + "' \n\ncannot be found. \n\nYour .exe should ideally be next to an Assets folder and config, all bundled together!\n\nPlease make sure you extracted the zip file, with the whole folder together, to a known location like Documents or Desktop- and we didn't end up somewhere random.\n\nGoose will still work, but he won't be able to use custom memes or any of that fanciness.\nHold ESC for several seconds to quit.");
			}
			GooseConfig.LoadConfig();
			GooseTaskDatabase.RegisterTask(new Wander());
			GooseTaskDatabase.RegisterTask(new TrackMud());
			GooseTaskDatabase.RegisterTask(new CollectMeme());
			GooseTaskDatabase.RegisterTask(new CollectNotepad());
			GooseTaskDatabase.RegisterTask(new NabMouse());
			if (GooseConfig.settings.EnableMods && MessageBox.Show("Mods are not created by the maker of Desktop Goose, and *can* contain malicious code that might harm your computer. This is a legalese thing, and it's not guaranteed, but the Desktop Goose creator cannot be held liable if this occurs. Do you still wish to enable mods?", "Mod Enabler Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
			{
				ModSupport.LoadMods();
			}
			GooseTaskDatabase.LockDatabase();
			Sound.Init();
			goose = GooseFunctions.InitGoose();
		}

		public static void TickGame(Graphics gfx)
		{
			Time.TickTime();
			OSFunctions.ClearCursorClip();
			OSFunctions.RefreshInput();
			InjectionPoints.RaisePreTick(goose);
			goose.tick(goose);
			InjectionPoints.RaisePostTick(goose);
			InjectionPoints.RaisePreUpdateRig(goose);
			goose.updateRig(goose.rig, goose.position, goose.direction);
			InjectionPoints.RaisePostUpdateRig(goose);
			InjectionPoints.RaisePreRender(goose, gfx);
			goose.render(goose, gfx);
			InjectionPoints.RaisePostRender(goose, gfx);
			EscToQuitOverlay.UpdateAndDraw(gfx);
		}
	}
}
