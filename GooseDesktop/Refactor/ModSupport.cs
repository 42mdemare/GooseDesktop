using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using GooseDesktop.Refactor.GooseTasks;
using GooseShared;

namespace GooseDesktop.Refactor
{
	internal static class ModSupport
	{
		private static List<IMod> modEntryPoints = new List<IMod>();

		private static Dictionary<IMod, string> modPaths = new Dictionary<IMod, string>();

		public static void LoadMods()
		{
			Type modEntryType = typeof(IMod);
			Type taskType = typeof(GooseTaskInfo);
			string pathToFileInAssembly = Program.GetPathToFileInAssembly("Assets/Mods/");
			if (!Directory.Exists(pathToFileInAssembly))
			{
				MessageBox.Show("Error: Could not find 'Assets/Mods' directory. Continuing without loading mods.", "Could not find mods folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			string[] directories = Directory.GetDirectories(pathToFileInAssembly);
			for (int i = 0; i < directories.Length; i++)
			{
				string[] files = Directory.GetFiles(directories[i]);
				foreach (string text in files)
				{
					if (!text.EndsWith(".dll"))
					{
						continue;
					}
					Assembly assembly = Assembly.UnsafeLoadFrom(Path.GetFullPath(text));
					while (true)
					{
						try
						{
							Type[] array = (from p in assembly.GetTypes()
								where modEntryType.IsAssignableFrom(p) && p.IsClass
								select p).ToArray();
							for (int k = 0; k < array.Length; k++)
							{
								IMod mod = (IMod)Activator.CreateInstance(array[k]);
								modEntryPoints.Add(mod);
								modPaths.Add(mod, directories[i]);
							}
							array = (from p in assembly.GetTypes()
								where p.IsSubclassOf(taskType) && p.IsClass
								select p).ToArray();
							for (int k = 0; k < array.Length; k++)
							{
								GooseTaskDatabase.RegisterTask((GooseTaskInfo)Activator.CreateInstance(array[k]));
							}
						}
						catch
						{
							string text2 = Path.GetFileName(Path.GetDirectoryName(text)) + "/" + Path.GetFileName(text);
							switch (MessageBox.Show("Could not load mod \"" + text2 + "\"\n\nIt may be for a different version of the goose.", "Couldn't Load Mod", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Hand))
							{
							case DialogResult.Retry:
								break;
							case DialogResult.Abort:
								Environment.Exit(1);
								goto end_IL_016a;
							default:
								goto end_IL_016a;
							}
							continue;
							end_IL_016a:;
						}
						break;
					}
				}
			}
			API.GooseFunctionPointers gooseFunctionPointers = new API.GooseFunctionPointers();
			gooseFunctionPointers.setSpeed = GooseFunctions.SetSpeed;
			gooseFunctionPointers.setTargetOffscreen = GooseFunctions.SetTargetOffscreen;
			gooseFunctionPointers.setCurrentTaskByID = GooseFunctions.SetTaskByID;
			gooseFunctionPointers.chooseRandomTask = GooseFunctions.ChooseRandomTask;
			gooseFunctionPointers.setTaskRoaming = GooseFunctions.SetTaskDefault;
			gooseFunctionPointers.isGooseAtTarget = GooseFunctions.IsGooseAtTarget;
			gooseFunctionPointers.getDistanceToTarget = GooseFunctions.GetDistanceToTarget;
			gooseFunctionPointers.playHonckSound = Sound.HONCC;
			API.ModHelperFunctions modHelperFunctions = new API.ModHelperFunctions();
			modHelperFunctions.getModDirectory = GetModDirectory;
			API.TaskDatabaseQueryFunctions taskDatabase = new API.TaskDatabaseQueryFunctions
			{
				getTaskIndexByID = GooseTaskDatabase.GetTaskIndexByID,
				getAllLoadedTaskIDs = GooseTaskDatabase.GetAllLoadedTaskIDs,
				getRandomTaskID = GooseTaskDatabase.GetRandomTaskID
			};
			API.Goose = gooseFunctionPointers;
			API.Helper = modHelperFunctions;
			API.TaskDatabase = taskDatabase;
			foreach (IMod modEntryPoint in modEntryPoints)
			{
				modEntryPoint.Init();
			}
			InjectionPoints.RaisePostModLoad();
		}

		public static string GetModDirectory(IMod mod)
		{
			if (!modPaths.ContainsKey(mod))
			{
				return "No such mod found, at least not loaded at initialization.";
			}
			return modPaths[mod];
		}
	}
}
