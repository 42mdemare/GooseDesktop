using GooseDesktop.Refactor.CustomFormTypes;
using GooseShared;

namespace GooseDesktop.Refactor.GooseTasks.Tasks
{
	internal class CollectMeme : RunCollectWindow
	{
		public new const string TaskID = "CollectMeme";

		public CollectMeme()
		{
			canBePickedRandomly = true;
			shortName = "Collect meme";
			description = "Make the goose run offscreen, and collect a meme.";
			taskID = "CollectMeme";
		}

		public override GooseTaskData GetNewTaskData(GooseEntity goose)
		{
			CollectWindowTaskData collectWindowTaskData = new CollectWindowTaskData();
			collectWindowTaskData.mainForm = new SimpleImageForm(goose);
			collectWindowTaskData.mainForm.FormClosing += RunCollectWindow.OnGiftClosed;
			SetupScreenTargetAndBeakOffset(collectWindowTaskData, goose);
			return collectWindowTaskData;
		}
	}
}
