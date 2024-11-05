using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace GooseDesktop.Properties
{
	// Token: 0x0200000E RID: 14
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
	internal sealed partial class Settings : ApplicationSettingsBase
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000062 RID: 98 RVA: 0x00004BEA File Offset: 0x00002DEA
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		// Token: 0x0400005E RID: 94
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}
