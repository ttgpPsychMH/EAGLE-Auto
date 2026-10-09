using System.Xml;

namespace TinhKiemAuto
{
	internal class Settings
	{
		private static SettingEx instance;

		public static string xml;

		public static SettingEx Instance
		{
			get
			{
				if (instance == null)
				{
					Load();
					instance = new SettingEx();
				}
				return instance;
			}
		}

		public static XmlDocument XML { get; set; }

		public static XmlNode Root => XML.SelectSingleNode("Settings");

		public static void Load()
		{
			XML = new XmlDocument();
			try
			{
				XML.LoadXml(xml);
			}
			catch
			{
			}
			if (Root == null)
			{
				XML.InsertBefore(XML.CreateXmlDeclaration("1.0", "UTF-8", null), XML.DocumentElement);
				XML.AppendChild(XML.CreateNode(XmlNodeType.Element, "Settings", ""));
				Save();
			}
		}

		public static void Save()
		{
		}

		public static string Read(string name)
		{
			XmlNode xmlNode = Root.SelectSingleNode(name);
			if (xmlNode != null)
			{
				return xmlNode.InnerText;
			}
			return "";
		}

		public static void Write(string name, string value)
		{
			XmlNode xmlNode = Root.SelectSingleNode(name);
			if (xmlNode == null)
			{
				xmlNode = XML.CreateElement(name);
			}
			xmlNode.InnerText = value;
			Root.AppendChild(xmlNode);
			Save();
		}
	}
}
