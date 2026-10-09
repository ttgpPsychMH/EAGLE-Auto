using System.Text;
using System.Xml;

namespace TinhKiemAuto
{
	public class Script
	{
		public XmlNode Node { get; set; }

		public string ID
		{
			get
			{
				try
				{
					return Node.Attributes["ID"].Value;
				}
				catch
				{
				}
				return "";
			}
			set
			{
				try
				{
					Node.Attributes["ID"].Value = value;
				}
				catch
				{
					XmlAttribute xmlAttribute = Scripts.XML.CreateAttribute("ID");
					xmlAttribute.Value = value;
					Node.Attributes.Append(xmlAttribute);
				}
			}
		}

		public string Level
		{
			get
			{
				try
				{
					return Node.Attributes["Level"].Value;
				}
				catch
				{
				}
				return "";
			}
			set
			{
				try
				{
					Node.Attributes["Level"].Value = value;
				}
				catch
				{
					XmlAttribute xmlAttribute = Scripts.XML.CreateAttribute("Level");
					xmlAttribute.Value = value;
					Node.Attributes.Append(xmlAttribute);
				}
			}
		}

		public string InfoEx
		{
			get
			{
				try
				{
					return Node.Attributes["InfoEx"].Value;
				}
				catch
				{
				}
				return "";
			}
			set
			{
				try
				{
					Node.Attributes["InfoEx"].Value = value;
				}
				catch
				{
					XmlAttribute xmlAttribute = Scripts.XML.CreateAttribute("InfoEx");
					xmlAttribute.Value = value;
					Node.Attributes.Append(xmlAttribute);
				}
			}
		}

		public string MD
		{
			get
			{
				try
				{
					return Node.Attributes["MD"].Value;
				}
				catch
				{
				}
				return "";
			}
			set
			{
				try
				{
					Node.Attributes["MD"].Value = value;
				}
				catch
				{
					XmlAttribute xmlAttribute = Scripts.XML.CreateAttribute("MD");
					xmlAttribute.Value = value;
					Node.Attributes.Append(xmlAttribute);
				}
			}
		}

		public string NameEx
		{
			get
			{
				try
				{
					return TINHKIEM.ClearSign(Info.Split(':')[0]).Replace(" ", "");
				}
				catch
				{
					return "";
				}
			}
		}

		public string Name
		{
			get
			{
				try
				{
					return Node.Attributes["Name"].Value;
				}
				catch
				{
				}
				XmlAttribute xmlAttribute = Scripts.XML.CreateAttribute("Name");
				xmlAttribute.Value = "";
				Node.Attributes.Append(xmlAttribute);
				return "";
			}
			set
			{
				try
				{
					Node.Attributes["Name"].Value = value;
				}
				catch
				{
					XmlAttribute xmlAttribute = Scripts.XML.CreateAttribute("Name");
					xmlAttribute.Value = value;
					Node.Attributes.Append(xmlAttribute);
				}
			}
		}

		public string Recv
		{
			get
			{
				try
				{
					return Node.Attributes["Recv"].Value;
				}
				catch
				{
				}
				return "";
			}
			set
			{
				try
				{
					Node.Attributes["Recv"].Value = value;
				}
				catch
				{
					XmlAttribute xmlAttribute = Scripts.XML.CreateAttribute("Recv");
					xmlAttribute.Value = value;
					Node.Attributes.Append(xmlAttribute);
				}
			}
		}

		public NPC RecvNPC
		{
			get
			{
				NPC nPC = new NPC();
				if (Recv.Length == 12)
				{
					nPC.Id = Memory.Hex2Int(Recv.Substring(0, 3));
					nPC.X = Memory.Hex2Int(Recv.Substring(3, 3));
					nPC.Y = Memory.Hex2Int(Recv.Substring(6, 3));
					nPC.Map = Memory.Hex2Int(Recv.Substring(9, 3));
					if (nPC.Map == 0)
					{
						nPC.Map = LACDUONG.Id;
					}
				}
				return nPC;
			}
		}

		public NPC AtkNPC1
		{
			get
			{
				NPC nPC = new NPC();
				if (Do.Length >= 18)
				{
					nPC.MD = Do.Substring(6, 3);
					nPC.X = Memory.Hex2Int(Do.Substring(9, 3));
					nPC.Y = Memory.Hex2Int(Do.Substring(12, 3));
					nPC.Map = Memory.Hex2Int(Do.Substring(15, 3));
					if (nPC.Map == 0)
					{
						nPC.Map = LACDUONG.Id;
					}
				}
				return nPC;
			}
		}

		public NPC AtkNPC2
		{
			get
			{
				NPC nPC = new NPC();
				if (Do.Length >= 30)
				{
					nPC.MD = Do.Substring(18, 3);
					nPC.X = Memory.Hex2Int(Do.Substring(21, 3));
					nPC.Y = Memory.Hex2Int(Do.Substring(24, 3));
					nPC.Map = Memory.Hex2Int(Do.Substring(27, 3));
					if (nPC.Map == 0)
					{
						nPC.Map = LACDUONG.Id;
					}
				}
				return nPC;
			}
		}

		public NPC AtkNPC3
		{
			get
			{
				NPC nPC = new NPC();
				if (Do.Length >= 42)
				{
					nPC.MD = Do.Substring(30, 3);
					nPC.X = Memory.Hex2Int(Do.Substring(33, 3));
					nPC.Y = Memory.Hex2Int(Do.Substring(36, 3));
					nPC.Map = Memory.Hex2Int(Do.Substring(39, 3));
					if (nPC.Map == 0)
					{
						nPC.Map = LACDUONG.Id;
					}
				}
				return nPC;
			}
		}

		public bool AtkAny
		{
			get
			{
				if (Do.Length >= 43)
				{
					return Do[42] == '1';
				}
				return false;
			}
		}

		public string IsCollect
		{
			get
			{
				if (Do.Length >= 44)
				{
					return Do[43].ToString();
				}
				return "0";
			}
			set
			{
				if (Do.Length >= 44)
				{
					StringBuilder stringBuilder = new StringBuilder(Do);
					stringBuilder[43] = value.ToString()[0];
					Do = stringBuilder.ToString();
				}
			}
		}

		public bool IsComplete
		{
			get
			{
				if (Do.Length >= 45)
				{
					return Do[44] == '1';
				}
				return false;
			}
			set
			{
				if (Do.Length >= 45)
				{
					StringBuilder stringBuilder = new StringBuilder(Do);
					if (value)
					{
						stringBuilder[44] = '1';
					}
					else
					{
						stringBuilder[44] = '0';
					}
					Do = stringBuilder.ToString();
				}
			}
		}

		public bool Completed
		{
			get
			{
				if (Do.Length >= 46)
				{
					return Do[45] == '1';
				}
				return false;
			}
			set
			{
				if (Do.Length >= 46)
				{
					StringBuilder stringBuilder = new StringBuilder(Do);
					if (value)
					{
						stringBuilder[45] = '1';
					}
					else
					{
						stringBuilder[45] = '0';
					}
					Do = stringBuilder.ToString();
				}
			}
		}

		public bool IsUseItem
		{
			get
			{
				if (Do.Length >= 47)
				{
					return Do[46] == '1';
				}
				return false;
			}
			set
			{
				if (Do.Length >= 47)
				{
					StringBuilder stringBuilder = new StringBuilder(Do);
					if (value)
					{
						stringBuilder[46] = '1';
					}
					else
					{
						stringBuilder[46] = '0';
					}
					Do = stringBuilder.ToString();
				}
			}
		}

		public int CompleteNoi
		{
			get
			{
				if (Do.Length >= 48)
				{
					return TINHKIEM.ParseInt(Do[47].ToString()) - 1;
				}
				return -1;
			}
			set
			{
				if (Do.Length >= 48)
				{
					StringBuilder stringBuilder = new StringBuilder(Do);
					stringBuilder[47] = value.ToString()[0];
					Do = stringBuilder.ToString();
				}
			}
		}

		public int CompleteNgoai
		{
			get
			{
				if (Do.Length >= 49)
				{
					return TINHKIEM.ParseInt(Do[48].ToString()) - 1;
				}
				return -1;
			}
			set
			{
				if (Do.Length >= 49)
				{
					StringBuilder stringBuilder = new StringBuilder(Do);
					stringBuilder[48] = value.ToString()[0];
					Do = stringBuilder.ToString();
				}
			}
		}

		public bool IsPick
		{
			get
			{
				if (Do.Length >= 50)
				{
					return Do[49] == '1';
				}
				return false;
			}
			set
			{
				if (Do.Length >= 50)
				{
					StringBuilder stringBuilder = new StringBuilder(Do);
					if (value)
					{
						stringBuilder[49] = '1';
					}
					else
					{
						stringBuilder[49] = '0';
					}
					Do = stringBuilder.ToString();
				}
			}
		}

		public bool IsThuThap
		{
			get
			{
				if (Do.Length >= 51)
				{
					return Do[50] == '1';
				}
				return false;
			}
			set
			{
				if (Do.Length >= 51)
				{
					StringBuilder stringBuilder = new StringBuilder(Do);
					if (value)
					{
						stringBuilder[50] = '1';
					}
					else
					{
						stringBuilder[50] = '0';
					}
					Do = stringBuilder.ToString();
				}
			}
		}

		public string Send
		{
			get
			{
				try
				{
					return Node.Attributes["Send"].Value;
				}
				catch
				{
				}
				return "";
			}
			set
			{
				try
				{
					Node.Attributes["Send"].Value = value;
				}
				catch
				{
					XmlAttribute xmlAttribute = Scripts.XML.CreateAttribute("Send");
					xmlAttribute.Value = value;
					Node.Attributes.Append(xmlAttribute);
				}
			}
		}

		public NPC SendNPC
		{
			get
			{
				NPC nPC = new NPC();
				if (Send.Length == 12)
				{
					nPC.Id = Memory.Hex2Int(Send.Substring(0, 3));
					nPC.X = Memory.Hex2Int(Send.Substring(3, 3));
					nPC.Y = Memory.Hex2Int(Send.Substring(6, 3));
					nPC.Map = Memory.Hex2Int(Send.Substring(9, 3));
					if (nPC.Map == 0)
					{
						nPC.Map = LACDUONG.Id;
					}
				}
				return nPC;
			}
		}

		public string SendClickMD
		{
			get
			{
				if (Do.Length >= 6)
				{
					return Do.Substring(3, 3);
				}
				return "000";
			}
		}

		public string RecvClickMD
		{
			get
			{
				if (Do.Length >= 3)
				{
					return Do.Substring(0, 3);
				}
				return "000";
			}
		}

		public string Do
		{
			get
			{
				try
				{
					return Node.Attributes["Do"].Value;
				}
				catch
				{
				}
				return "";
			}
			set
			{
				try
				{
					Node.Attributes["Do"].Value = value;
				}
				catch
				{
					XmlAttribute xmlAttribute = Scripts.XML.CreateAttribute("Do");
					xmlAttribute.Value = value;
					Node.Attributes.Append(xmlAttribute);
				}
			}
		}

		public string Info
		{
			get
			{
				try
				{
					return Node.Attributes["Info"].Value;
				}
				catch
				{
				}
				return "";
			}
			set
			{
				try
				{
					Node.Attributes["Info"].Value = value;
				}
				catch
				{
					XmlAttribute xmlAttribute = Scripts.XML.CreateAttribute("Info");
					xmlAttribute.Value = value;
					Node.Attributes.Append(xmlAttribute);
				}
			}
		}

		public Script()
		{
			Node = Scripts.XML.CreateElement("Script");
		}

		public Script(XmlNode node)
		{
			Node = node;
		}

		public bool IsClickExacly(QuestFrame dialog)
		{
			if (!(SendClickMD == dialog.MD) && !(RecvClickMD == dialog.MD))
			{
				if (!Name.Contains(dialog.Name))
				{
					return TINHKIEM.VietLien(Name) == TINHKIEM.VietLien(dialog.Name);
				}
				return true;
			}
			return true;
		}
	}
}
