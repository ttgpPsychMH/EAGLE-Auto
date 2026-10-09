using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace TinhKiemAuto.Properties
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (resourceMan == null)
				{
					resourceMan = new ResourceManager("TinhKiemAuto.Properties.Resources", typeof(Resources).Assembly);
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		internal static Bitmap Cancel_50px => (Bitmap)ResourceManager.GetObject("Cancel_50px", resourceCulture);

		internal static Bitmap List_50px => (Bitmap)ResourceManager.GetObject("List_50px", resourceCulture);

		internal static Bitmap Ok_50px => (Bitmap)ResourceManager.GetObject("Ok_50px", resourceCulture);

		internal static Bitmap Settings_48px => (Bitmap)ResourceManager.GetObject("Settings_48px", resourceCulture);

		internal Resources()
		{
		}
	}
}
