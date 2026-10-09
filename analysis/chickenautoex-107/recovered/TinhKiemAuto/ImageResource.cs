using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace TinhKiemAuto
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	public class ImageResource
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static ResourceManager ResourceManager
		{
			get
			{
				if (resourceMan == null)
				{
					resourceMan = new ResourceManager("TinhKiemAuto.ImageResource", typeof(ImageResource).Assembly);
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static CultureInfo Culture
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

		public static Bitmap captcha => (Bitmap)ResourceManager.GetObject("captcha", resourceCulture);

		public static string Fix2D => ResourceManager.GetString("Fix2D", resourceCulture);

		public static string Fix3D => ResourceManager.GetString("Fix3D", resourceCulture);

		public static string FixDG => ResourceManager.GetString("FixDG", resourceCulture);

		public static Bitmap loading => (Bitmap)ResourceManager.GetObject("loading", resourceCulture);

		public static string Lua => ResourceManager.GetString("Lua", resourceCulture);

		public static string LuaEx => ResourceManager.GetString("LuaEx", resourceCulture);

		public static Bitmap refresh => (Bitmap)ResourceManager.GetObject("refresh", resourceCulture);

		public static Bitmap sound => (Bitmap)ResourceManager.GetObject("sound", resourceCulture);

		internal ImageResource()
		{
		}
	}
}
