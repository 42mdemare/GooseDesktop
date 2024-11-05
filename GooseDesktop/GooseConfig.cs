using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace GooseDesktop
{
	public static class GooseConfig
	{
		public class ConfigSettings
		{
			public int Version_DoNotEdit = 1;

			public bool EnableMods;

			public bool SilenceSounds;

			public bool Task_CanAttackMouse = true;

			public bool AttackRandomly;

			public bool UseCustomColors;

			public string GooseDefaultWhite = "#ffffff";

			public string GooseDefaultOrange = "#ffa500";

			public string GooseDefaultOutline = "#d3d3d3";

			public float MinWanderingTimeSeconds = 20f;

			public float MaxWanderingTimeSeconds = 40f;

			public float FirstWanderTimeSeconds = 20f;

			public static ConfigSettings ReadFileIntoConfig(string configGivenPath)
			{
				ConfigSettings configSettings = new ConfigSettings();
				if (!File.Exists(configGivenPath))
				{
					MessageBox.Show("Can't find config.ini file! Creating a new one with default values");
					WriteConfigToFile(configGivenPath, configSettings);
					return configSettings;
				}
				try
				{
					using StreamReader streamReader = new StreamReader(configGivenPath);
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					string text;
					while ((text = streamReader.ReadLine()) != null)
					{
						string[] array = text.Split('=');
						if (array.Length == 2)
						{
							dictionary.Add(array[0], array[1]);
						}
					}
					int result = -1;
					int.TryParse(dictionary["Version_DoNotEdit"], out result);
					if (result != 1)
					{
						MessageBox.Show("config.ini is for the wrong version! Creating a new one with default values!");
						File.Delete(configGivenPath);
						WriteConfigToFile(configGivenPath, configSettings);
						return configSettings;
					}
					foreach (KeyValuePair<string, string> item in dictionary)
					{
						FieldInfo field = typeof(ConfigSettings).GetField(item.Key);
						try
						{
							field.SetValue(configSettings, Convert.ChangeType(item.Value, field.FieldType));
						}
						catch
						{
							MessageBox.Show("Loading config error: field " + field.Name + "'s value is not valid. Setting it to the default value.");
						}
					}
					return configSettings;
				}
				catch
				{
					MessageBox.Show("config.ini corrupt! Creating a new one!");
					File.Delete(configGivenPath);
					WriteConfigToFile(configGivenPath, configSettings);
					return configSettings;
				}
			}

			public static void WriteConfigToFile(string path, ConfigSettings f)
			{
				using StreamWriter streamWriter = File.CreateText(path);
				streamWriter.Write(GenerateTextFromSettings(f));
			}

			public static string GenerateTextFromSettings(ConfigSettings f)
			{
				StringBuilder stringBuilder = new StringBuilder();
				FieldInfo[] fields = typeof(ConfigSettings).GetFields();
				foreach (FieldInfo fieldInfo in fields)
				{
					stringBuilder.Append($"{fieldInfo.Name}={fieldInfo.GetValue(f).ToString()}\n");
				}
				return stringBuilder.ToString();
			}
		}

		private static string filePath = Program.GetPathToFileInAssembly("config.ini");

		public const int GOOSE_CONFIG_VERSION = 1;

		public static ConfigSettings settings = null;

		public static void LoadConfig()
		{
			settings = ConfigSettings.ReadFileIntoConfig(filePath);
		}
	}
}
