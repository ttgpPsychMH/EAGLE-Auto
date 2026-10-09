using System.Collections.Generic;
using System.Xml;
using TinhKiemAuto.Models;

namespace TinhKiemAuto
{
	internal class Scripts
	{
		public static XmlDocument XML = new XmlDocument();

		public static string Path => Global.DataPath + "\\20.dat";

		public static List<Script> All { get; set; }

		public static List<Script> Load()
		{
			try
			{
				XML.LoadXml(LoadFile.LoadFileWithDecrypt(Path));
			}
			catch
			{
				if (XML.SelectSingleNode("/*") == null)
				{
					XML.InsertBefore(XML.CreateXmlDeclaration("1.0", "UTF-8", null), XML.DocumentElement);
					XML.AppendChild(XML.CreateNode(XmlNodeType.Element, "Scripts", ""));
					XML.Save(Path);
				}
			}
			List<Script> list = new List<Script>();
			foreach (XmlNode item in XML.SelectSingleNode("Scripts").SelectNodes("Script"))
			{
				list.Add(new Script
				{
					Node = item
				});
			}
			All = list;
			return list;
		}

		public static Script Get(string id)
		{
			foreach (Script item in All)
			{
				if (item.ID == id || item.MD.Contains(id))
				{
					return item;
				}
			}
			return null;
		}

		public static Script GetByName(string id)
		{
			foreach (Script item in All)
			{
				if (item.Name.Contains(id) || TINHKIEM.VietLien(item.Name) == TINHKIEM.VietLien(id))
				{
					return item;
				}
			}
			return null;
		}

		public static string Add(Script script)
		{
			foreach (XmlNode item in XML.SelectSingleNode("Scripts").SelectNodes("Script"))
			{
				Script script2 = new Script();
				script2.Node = item;
				if (script2.MD.Contains(script.MD) || (script2.ID == script.ID && script.ID != ""))
				{
					return "Script Đã Tồn Tại";
				}
			}
			XML.SelectSingleNode("/*").AppendChild(script.Node);
			Save();
			return "";
		}

		public static void Save()
		{
			LoadFile.WriteFileWithEncrypt(XML.OuterXml, Path);
		}

		public static void Remove(Script script)
		{
			XML.SelectSingleNode("/*").RemoveChild(script.Node);
			LoadFile.WriteFileWithEncrypt(XML.OuterXml, Path);
		}
	}
}
