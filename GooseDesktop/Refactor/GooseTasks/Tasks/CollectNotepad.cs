using GooseDesktop.Refactor.CustomFormTypes;
using GooseShared;

namespace GooseDesktop.Refactor.GooseTasks.Tasks
{
	internal class CollectNotepad : RunCollectWindow
	{
		public new const string TaskID = "CollectNotepad";

		public CollectNotepad()
		{
			canBePickedRandomly = true;
			shortName = "Collect Not-epad window";
			description = "Make the goose run offscreen, and collect a \"Goose Not-epad\" document.";
			taskID = "CollectNotepad";
		}

		public override GooseTaskData GetNewTaskData(GooseEntity goose)
		{
			CollectWindowTaskData collectWindowTaskData = new CollectWindowTaskData();
			collectWindowTaskData.mainForm = new SimpleTextForm(goose);
			SetupScreenTargetAndBeakOffset(collectWindowTaskData, goose);
			return collectWindowTaskData;
		}
	}
}
