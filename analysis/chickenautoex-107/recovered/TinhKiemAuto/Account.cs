using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Xml;
using TinhKiemAuto.Models;

namespace TinhKiemAuto
{
	public class Account
	{
		public bool IsReadCaptCha;

		private BackgroundWorker worker = new BackgroundWorker();

		public static XmlDocument XML = new XmlDocument();

		public bool IsSelectRole;

		public int IsSelect = 5;

		public int CharacterNum = 1;

		public XmlNode Node;

		public Game game;

		public bool IsGetAn;

		public bool Entered;

		public int SelectCount;

		public int LoginMessageTime;

		public string Status = string.Empty;

		public string Img = string.Empty;

		public string ImgHash = string.Empty;

		public bool IsBHD;

		public bool IsTrong;

		public bool IsNhanMam;

		public bool IsTrungAc;

		public bool IsDua;

		public string User
		{
			get
			{
				try
				{
					XmlAttribute xmlAttribute = Node.Attributes["User"];
					if (xmlAttribute == null)
					{
						xmlAttribute = XML.CreateAttribute("User");
						Node.Attributes.Append(xmlAttribute);
					}
					if (Node.SelectSingleNode("User") != null)
					{
						xmlAttribute.Value = Node.SelectSingleNode("User").InnerText;
						Node.RemoveChild(Node.SelectSingleNode("User"));
					}
					return xmlAttribute.Value;
				}
				catch
				{
					return "";
				}
			}
			set
			{
				try
				{
					XmlAttribute xmlAttribute = Node.Attributes["User"];
					if (xmlAttribute == null)
					{
						xmlAttribute = XML.CreateAttribute("User");
						Node.Attributes.Append(xmlAttribute);
					}
					xmlAttribute.Value = value;
				}
				catch
				{
				}
			}
		}

		public string Pass
		{
			get
			{
				try
				{
					XmlAttribute xmlAttribute = Node.Attributes["Pass"];
					if (xmlAttribute == null)
					{
						xmlAttribute = XML.CreateAttribute("Pass");
						Node.Attributes.Append(xmlAttribute);
					}
					if (Node.SelectSingleNode("Pass") != null)
					{
						xmlAttribute.Value = Node.SelectSingleNode("Pass").InnerText;
						Node.RemoveChild(Node.SelectSingleNode("Pass"));
					}
					return xmlAttribute.Value;
				}
				catch
				{
					return "";
				}
			}
			set
			{
				try
				{
					XmlAttribute xmlAttribute = Node.Attributes["Pass"];
					if (xmlAttribute == null)
					{
						xmlAttribute = XML.CreateAttribute("Pass");
						Node.Attributes.Append(xmlAttribute);
					}
					xmlAttribute.Value = value;
				}
				catch
				{
				}
			}
		}

		public string NPH
		{
			get
			{
				try
				{
					XmlAttribute xmlAttribute = Node.Attributes["NPH"];
					if (xmlAttribute == null)
					{
						xmlAttribute = XML.CreateAttribute("NPH");
						Node.Attributes.Append(xmlAttribute);
					}
					if (Node.SelectSingleNode("NPH") != null)
					{
						xmlAttribute.Value = Node.SelectSingleNode("NPH").InnerText;
						Node.RemoveChild(Node.SelectSingleNode("NPH"));
					}
					return xmlAttribute.Value;
				}
				catch
				{
					return "";
				}
			}
			set
			{
				try
				{
					XmlAttribute xmlAttribute = Node.Attributes["NPH"];
					if (xmlAttribute == null)
					{
						xmlAttribute = XML.CreateAttribute("NPH");
						Node.Attributes.Append(xmlAttribute);
					}
					xmlAttribute.Value = value;
				}
				catch
				{
				}
			}
		}

		public string Server
		{
			get
			{
				try
				{
					XmlAttribute xmlAttribute = Node.Attributes["Server"];
					if (xmlAttribute == null)
					{
						xmlAttribute = XML.CreateAttribute("Server");
						Node.Attributes.Append(xmlAttribute);
					}
					if (Node.SelectSingleNode("Server") != null)
					{
						xmlAttribute.Value = Node.SelectSingleNode("Server").InnerText;
						Node.RemoveChild(Node.SelectSingleNode("Server"));
					}
					return xmlAttribute.Value;
				}
				catch
				{
					return "";
				}
			}
			set
			{
				try
				{
					XmlAttribute xmlAttribute = Node.Attributes["Server"];
					if (xmlAttribute == null)
					{
						xmlAttribute = XML.CreateAttribute("Server");
						Node.Attributes.Append(xmlAttribute);
					}
					xmlAttribute.Value = value;
				}
				catch
				{
				}
			}
		}

		public string Tail
		{
			get
			{
				try
				{
					XmlAttribute xmlAttribute = Node.Attributes["Tail"];
					if (xmlAttribute == null)
					{
						xmlAttribute = XML.CreateAttribute("Tail");
						Node.Attributes.Append(xmlAttribute);
					}
					if (Node.SelectSingleNode("Tail") != null)
					{
						xmlAttribute.Value = Node.SelectSingleNode("Tail").InnerText;
						Node.RemoveChild(Node.SelectSingleNode("Tail"));
					}
					return xmlAttribute.Value;
				}
				catch
				{
					return "";
				}
			}
			set
			{
				try
				{
					XmlAttribute xmlAttribute = Node.Attributes["Tail"];
					if (xmlAttribute == null)
					{
						xmlAttribute = XML.CreateAttribute("Tail");
						Node.Attributes.Append(xmlAttribute);
					}
					xmlAttribute.Value = value;
				}
				catch
				{
				}
			}
		}

		public string Name
		{
			get
			{
				try
				{
					XmlAttribute xmlAttribute = Node.Attributes["Name"];
					if (xmlAttribute == null)
					{
						xmlAttribute = XML.CreateAttribute("Name");
						Node.Attributes.Append(xmlAttribute);
					}
					if (Node.SelectSingleNode("Name") != null)
					{
						xmlAttribute.Value = Node.SelectSingleNode("Name").InnerText;
						Node.RemoveChild(Node.SelectSingleNode("Name"));
					}
					return xmlAttribute.Value;
				}
				catch
				{
					return "";
				}
			}
			set
			{
				try
				{
					XmlAttribute xmlAttribute = Node.Attributes["Name"];
					if (xmlAttribute == null)
					{
						xmlAttribute = XML.CreateAttribute("Name");
						Node.Attributes.Append(xmlAttribute);
					}
					xmlAttribute.Value = value;
				}
				catch
				{
				}
			}
		}

		public string Ids
		{
			get
			{
				try
				{
					XmlAttribute xmlAttribute = Node.Attributes["Ids"];
					if (xmlAttribute == null)
					{
						xmlAttribute = XML.CreateAttribute("Ids");
						Node.Attributes.Append(xmlAttribute);
					}
					if (Node.SelectSingleNode("Ids") != null)
					{
						xmlAttribute.Value = Node.SelectSingleNode("Ids").InnerText;
						Node.RemoveChild(Node.SelectSingleNode("Ids"));
					}
					return xmlAttribute.Value;
				}
				catch
				{
					return "";
				}
			}
			set
			{
				try
				{
					XmlAttribute xmlAttribute = Node.Attributes["Ids"];
					if (xmlAttribute == null)
					{
						xmlAttribute = XML.CreateAttribute("Ids");
						Node.Attributes.Append(xmlAttribute);
					}
					xmlAttribute.Value = value;
				}
				catch
				{
				}
			}
		}

		public string Menpai
		{
			get
			{
				try
				{
					XmlAttribute xmlAttribute = Node.Attributes["Menpai"];
					if (xmlAttribute == null)
					{
						xmlAttribute = XML.CreateAttribute("Menpai");
						Node.Attributes.Append(xmlAttribute);
					}
					if (Node.SelectSingleNode("Menpai") != null)
					{
						xmlAttribute.Value = Node.SelectSingleNode("Menpai").InnerText;
						Node.RemoveChild(Node.SelectSingleNode("Menpai"));
					}
					return xmlAttribute.Value;
				}
				catch
				{
					return "";
				}
			}
			set
			{
				try
				{
					XmlAttribute xmlAttribute = Node.Attributes["Menpai"];
					if (xmlAttribute == null)
					{
						xmlAttribute = XML.CreateAttribute("Menpai");
						Node.Attributes.Append(xmlAttribute);
					}
					xmlAttribute.Value = value;
				}
				catch
				{
				}
			}
		}

		public string Lvl
		{
			get
			{
				try
				{
					XmlAttribute xmlAttribute = Node.Attributes["Lvl"];
					if (xmlAttribute == null)
					{
						xmlAttribute = XML.CreateAttribute("Lvl");
						Node.Attributes.Append(xmlAttribute);
					}
					if (Node.SelectSingleNode("Lvl") != null)
					{
						xmlAttribute.Value = Node.SelectSingleNode("Lvl").InnerText;
						Node.RemoveChild(Node.SelectSingleNode("Lvl"));
					}
					return xmlAttribute.Value;
				}
				catch
				{
					return "";
				}
			}
			set
			{
				try
				{
					XmlAttribute xmlAttribute = Node.Attributes["Lvl"];
					if (xmlAttribute == null)
					{
						xmlAttribute = XML.CreateAttribute("Lvl");
						Node.Attributes.Append(xmlAttribute);
					}
					xmlAttribute.Value = value;
				}
				catch
				{
				}
			}
		}

		public Stopwatch LogonTime { get; set; }

		public Stopwatch IsNextLogin { get; set; }

		public bool IsForceOpen { get; set; }

		public Stopwatch SelectAccTime { get; set; }

		public int ServerIndex
		{
			get
			{
				switch (Server)
				{
				case "Nhất Kiếm":
					return 0;
				case "Nhị Kiếm":
					return 1;
				case "Tam Kiếm":
					return 2;
				case "Tứ Kiếm":
					return 3;
				case "Tiếu Ngạo":
					return 4;
				case "Tái Chiến":
					return 5;
				case "Long Kiếm":
					return 6;
				case "Du Kiếm":
					return 7;
				case "Song Kiếm":
					return 8;
				case "Ảnh Kiếm":
					return 9;
				default:
					return -1;
				}
			}
		}

		public int TailIndex
		{
			get
			{
				switch (Server)
				{
				case "Nhất Kiếm":
					return 1;
				case "Nhị Kiếm":
					return 2;
				case "Tam Kiếm":
					return 3;
				case "Tứ Kiếm":
					return 4;
				case "Tiếu Ngạo":
					return 6;
				case "Tái Chiến":
					return 5;
				case "Long Kiếm":
					return 7;
				case "Du Kiếm":
					return 8;
				case "Song Kiếm":
					return 9;
				case "Ảnh Kiếm":
					return -1;
				default:
					return -1;
				}
			}
		}

		public bool IsSave { get; set; }

		public string LoginIndex
		{
			get
			{
				if (Node.SelectSingleNode("LoginIndex") == null)
				{
					return "1";
				}
				return Node.SelectSingleNode("LoginIndex").InnerText;
			}
			set
			{
				XmlNode xmlNode = Node.SelectSingleNode("LoginIndex");
				if (xmlNode == null)
				{
					xmlNode = XML.CreateElement("LoginIndex");
					xmlNode.InnerText = "1";
					Node.AppendChild(xmlNode);
				}
				xmlNode.InnerText = value;
				Save();
			}
		}

		public bool IsSaveName { get; set; }

		public string Path
		{
			get
			{
				try
				{
					return LoadFile.LoadFileWithDecrypt(Global.DataPath + "\\ExecutePath.dat");
				}
				catch
				{
					return "";
				}
			}
		}

		public bool IsCaptcha
		{
			get
			{
				if (game != null && game.TLBB.IsSelectCharacter)
				{
					return game.TLBB.IsTextCaptcha;
				}
				return false;
			}
		}

		public Stopwatch OpenGameTime { get; set; }

		public bool Online
		{
			get
			{
				if (game != null)
				{
					return game.TLBB.Online;
				}
				return false;
			}
		}

		private int BaseImg { get; set; }

		public Bitmap KetQua { get; set; }

		public Account(string user, string pass, string NPH, string server, string tail)
		{
			XmlElement xmlElement = XML.CreateElement("Account");
			XML.SelectSingleNode("/*").AppendChild(xmlElement);
			Node = xmlElement;
			User = user;
			Pass = pass;
			Tail = tail;
			this.NPH = NPH;
			Server = server;
			Save();
		}

		public Account(XmlElement node)
		{
			Node = node;
		}

		public void Answer(string txt)
		{
			if (game != null)
			{
				game.LuaDoOneLineString("DataPool:SendLoginCode(" + txt + ")");
			}
		}

		public void ReadCaptcha1()
		{
			IsReadCaptCha = false;
			worker.DoWork += delegate
			{
				try
				{
					Img = string.Empty;
					if (game == null)
					{
						KetQua = null;
					}
					else
					{
						Bitmap bitmap = new Bitmap(128, 36);
						int num;
						if (BaseImg != 0)
						{
							num = BaseImg + 4;
						}
						else
						{
							int moduleAddress = game.Memory.GetModuleAddress("UI_CEGUI.dll");
							num = game.Memory.Read(new int[4]
							{
								moduleAddress + game.Address.Captcha[0],
								game.Address.Captcha[1],
								game.Address.Captcha[2],
								game.Address.Captcha[3]
							});
						}
						int num2 = game.Memory.Read2Byte(num);
						if (num2 != 40960 && num2 != 41215 && game.Address.GameType != 1)
						{
							if (BaseImg == 0)
							{
								BaseImg = game.Memory.Scan("## 00 00 00 ?? A0 ?? A0 ?? A0 ?? A0 ?? A0 ?? A0 ?? A0 ?? A0", 0, 0, 0);
							}
							num = BaseImg + 4;
							num2 = game.Memory.Read2Byte(num);
						}
						if (num2 == 40960 || num2 == 41215)
						{
							string text = "";
							for (int i = 0; i < 9216; i += 2)
							{
								num2 = game.Memory.Read2Byte(num + i);
								if (num2 == 40960)
								{
									bitmap.SetPixel(i / 2 % 128, i / 2 / 128, Color.Black);
									text += "0";
								}
								else
								{
									bitmap.SetPixel(i / 2 % 128, i / 2 / 128, Color.White);
									text += "1";
								}
								if (text.Length == 8)
								{
									Img += Convert.ToInt32(text, 2).ToString("X2");
									text = "";
								}
							}
							bitmap = new Bitmap(bitmap, new Size(160, 45));
						}
						KetQua = bitmap;
						IsReadCaptCha = true;
					}
				}
				catch (Exception ex)
				{
					Console.Write(ex.ToString());
					KetQua = null;
				}
			};
			worker.RunWorkerAsync();
		}

		public static void Load()
		{
			try
			{
				XML.LoadXml(LoadFile.LoadFileWithDecrypt(Global.DataPath + "\\Account.dat"));
			}
			catch
			{
				if (XML.SelectSingleNode("/*") == null)
				{
					XML.InsertBefore(XML.CreateXmlDeclaration("1.0", "UTF-8", null), XML.DocumentElement);
					XML.AppendChild(XML.CreateNode(XmlNodeType.Element, "Accounts", ""));
					LoadFile.WriteFileWithEncrypt(XML.OuterXml, Global.DataPath + "\\Account.dat");
				}
			}
		}

		public static void Save()
		{
			foreach (XmlElement item in XML.SelectSingleNode("Accounts").SelectNodes("Account"))
			{
				if (item.InnerXml.Trim() == string.Empty)
				{
					item.IsEmpty = true;
				}
			}
			LoadFile.WriteFileWithEncrypt(XML.OuterXml, Global.DataPath + "\\Account.dat");
		}

		public static List<Account> Enum()
		{
			List<Account> list = new List<Account>();
			foreach (XmlElement item2 in XML.SelectSingleNode("Accounts").SelectNodes("Account"))
			{
				try
				{
					Account item = new Account(item2);
					list.Add(item);
				}
				catch
				{
				}
			}
			return list;
		}
	}
}
