using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Runtime.CompilerServices;

namespace GooseDesktop.Properties
{
	// Token: 0x0200000D RID: 13
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x0600005B RID: 91 RVA: 0x00004B65 File Offset: 0x00002D65
		internal Resources()
		{
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600005C RID: 92 RVA: 0x00004B6D File Offset: 0x00002D6D
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new ResourceManager("GooseDesktop.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600005D RID: 93 RVA: 0x00004B99 File Offset: 0x00002D99
		// (set) Token: 0x0600005E RID: 94 RVA: 0x00004BA0 File Offset: 0x00002DA0
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600005F RID: 95 RVA: 0x00004BA8 File Offset: 0x00002DA8
		internal static UnmanagedMemoryStream Pat1
		{
			get
			{
				return Resources.ResourceManager.GetStream("Pat1", Resources.resourceCulture);
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000060 RID: 96 RVA: 0x00004BBE File Offset: 0x00002DBE
		internal static UnmanagedMemoryStream Pat2
		{
			get
			{
				return Resources.ResourceManager.GetStream("Pat2", Resources.resourceCulture);
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000061 RID: 97 RVA: 0x00004BD4 File Offset: 0x00002DD4
		internal static UnmanagedMemoryStream Pat3
		{
			get
			{
				return Resources.ResourceManager.GetStream("Pat3", Resources.resourceCulture);
			}
		}

		// Token: 0x0400005C RID: 92
		private static ResourceManager resourceMan;

		// Token: 0x0400005D RID: 93
		private static CultureInfo resourceCulture;
	}
}
