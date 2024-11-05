using System.Collections.Generic;
using System.Windows.Forms;
using GooseShared;
using SamEngine;

namespace GooseDesktop.Refactor.GooseTasks
{
	public static class GooseTaskDatabase
	{
		private static List<GooseTaskInfo> tasksDatabase = new List<GooseTaskInfo>();

		private static Dictionary<string, int> idToIndex = new Dictionary<string, int>();

		private static List<int> randomlyPickableTaskIndices = new List<int>();

		private static Deck taskDeck;

		public static void RegisterTask(GooseTaskInfo task)
		{
			if (taskDeck == null)
			{
				if (task.taskID == "")
				{
					MessageBox.Show("ERROR: Attempting to register a task with no ID set. \nTask description: " + task.description, "ERROR: Task registration failure.", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				tasksDatabase.Add(task);
				int num = tasksDatabase.IndexOf(task);
				idToIndex.Add(task.taskID, num);
				if (task.canBePickedRandomly)
				{
					randomlyPickableTaskIndices.Add(num);
				}
			}
			else
			{
				MessageBox.Show("ERROR: Attempting to register a task after the database is locked. \nTask description: " + task.description, "ERROR: Task registration failure.", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		public static void LockDatabase()
		{
			taskDeck = new Deck(randomlyPickableTaskIndices.Count);
		}

		public static GooseTaskInfo GetTask(int index)
		{
			return tasksDatabase[index];
		}

		public static int GetTaskIndexByID(string id)
		{
			if (!idToIndex.ContainsKey(id))
			{
				MessageBox.Show("Trying to retrieve task of ID \"" + id + "\" - but it doesn't exist in the database!");
				return -1;
			}
			return idToIndex[id];
		}

		public static int GetNextRandomTask()
		{
			if (taskDeck.indices.Length != 0)
			{
				return randomlyPickableTaskIndices[taskDeck.Next()];
			}
			return GetTaskIndexByID("Wander");
		}

		public static string[] GetAllLoadedTaskIDs()
		{
			string[] array = new string[tasksDatabase.Count];
			for (int i = 0; i < tasksDatabase.Count; i++)
			{
				array[i] = tasksDatabase[i].taskID;
			}
			return array;
		}

		public static string GetRandomTaskID()
		{
			return GetTask(GetNextRandomTask()).taskID;
		}
	}
}
