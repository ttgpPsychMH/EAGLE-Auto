using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace TinhKiemAuto
{
	internal class Debug : Form
	{
		private Game game;

		private static List<Poster> listAtk = new List<Poster>();

		private bool IsAtk;

		private const int WS_BORDER = 8388608;

		private const int WS_DLGFRAME = 4194304;

		private const int WS_CAPTION = 12582912;

		private const int WS_SYSMENU = 524288;

		private const int WS_THICKFRAME = 262144;

		private const int WS_MINIMIZE = 536870912;

		private const int WS_MAXIMIZEBOX = 65536;

		private const int GWL_STYLE = -16;

		private const int GWL_EXSTYLE = -20;

		private const int WS_EX_DLGMODALFRAME = 1;

		private const int SWP_NOMOVE = 2;

		private const int SWP_NOSIZE = 1;

		private const int SWP_FRAMECHANGED = 32;

		private const uint MF_BYPOSITION = 1024u;

		private const uint MF_REMOVE = 4096u;

		public static bool NotRead = false;

		public static int offset = 112;

		private IContainer components;

		private TabControl tabControl1;

		private TabPage tabPage1;

		private TabPage tabPage2;

		private Button btnReadGameObject;

		private ListView listViewGameObject;

		private ColumnHeader columnHeader1;

		private ColumnHeader columnHeader2;

		private Timer tmrRefresh;

		private Button btnReadSkill;

		private ListView listViewSkill;

		private ColumnHeader columnHeader3;

		private ColumnHeader columnHeader4;

		private TabPage tabPage3;

		private Button btnDoString;

		private TextBox txtDoString;

		private TabPage tabPage4;

		private Button btnGameControl;

		private TextBox txtGameControl;

		private ListView listViewGameControl;

		private ColumnHeader columnHeader5;

		private ColumnHeader columnHeader6;

		private Button btnGameObjectParty;

		private ColumnHeader columnHeader7;

		private Button btnCompress;

		private Button btnToString;

		private TabPage tabPage5;

		private TextBox txtTask;

		private Button btnReadTask;

		private ListView listViewTask;

		private ColumnHeader columnHeader13;

		private ColumnHeader columnHeader14;

		private TabPage tabPage6;

		private TextBox txtDialog;

		private Button btnReadDialog;

		private ListView listViewDialog;

		private ColumnHeader columnHeader15;

		private ColumnHeader columnHeader16;

		private TabPage tabPage7;

		private TextBox txtPacket;

		private Button btnReadPacket;

		private ListView listViewPacket;

		private ColumnHeader columnHeader8;

		private ColumnHeader columnHeader9;

		private Button btnGameObjectMonter;

		private TabPage tabPage8;

		private TabPage tabPage9;

		private TextBox txtShop;

		private Button btnReadShop;

		private ListView listViewShop;

		private ColumnHeader columnHeader10;

		private ColumnHeader columnHeader11;

		private Button btnResetTime;

		private TabPage tabPage10;

		private TextBox txtTLBB;

		private Button btn20m;

		private Button btnSendPacket;

		private TextBox txtSendPacket;

		private ColumnHeader columnHeader12;

		private TabPage tabPage11;

		private SplitContainer splitContainer1;

		private TextBox txtCode;

		private TextBox txtNewCode;

		private Button btnPushDebugMessage;

		private TextBox txtPushDebugMessage;

		private ColumnHeader columnHeader17;

		private TabPage tabPage12;

		private SplitContainer splitContainer2;

		private TextBox txtUnicode;

		private TextBox txtVISCII;

		private Button btnSendCaptcha;

		private NumericUpDown nudAtkCount;

		private Button btnAtk;

		private TextBox txtUrlAtk;

		private Button btnTest;

		private Button button1;

		private TextBox textBox1;

		private Button btnPhone;

		private TextBox txtPhone;

		private Button button2;

		private TabPage tabPage13;

		private ListView listViewScript;

		private ColumnHeader columnHeader18;

		private ColumnHeader columnHeader19;

		private ColumnHeader columnHeader20;

		private ColumnHeader columnHeader21;

		private ColumnHeader columnHeader22;

		private ContextMenuStrip menuScript;

		private ToolStripMenuItem menuAddScript;

		private ToolStripMenuItem menuEditScript;

		private ToolStripMenuItem menuDeleteScript;

		private ColumnHeader columnHeader23;

		private Button button3;

		private ContextMenuStrip menuGameObject;

		private ToolStripMenuItem talkToolStripMenuItem;

		private ToolStripMenuItem pickToolStripMenuItem;

		private ToolStripMenuItem collectToolStripMenuItem;

		private ToolStripMenuItem instanceToolStripMenuItem;

		private ToolStripMenuItem refreshToolStripMenuItem;

		private SplitContainer splitContainer3;

		private ColumnHeader columnHeader24;

		private ColumnHeader columnHeader25;

		private CheckBox chkNotRead;

		private ToolStripMenuItem menuPickObject;

		private ColumnHeader columnHeader26;

		private TabPage tabPage14;

		private PropertyGrid probGame;

		private Button btnrRemoveHex;

		private TabPage tabPage15;

		private PropertyGrid pgTLBB;

		private Button button4;

		private Button button5;

		private NumericUpDown numericUpDown1;

		private Button button6;

		private Button button7;

		private SplitContainer splitContainer4;

		private PropertyGrid pgGameObject;

		private Timer tmrTest;

		private Button button8;

		private Button button9;

		private Button button10;

		private TextBox txtidskill;

		private TextBox txtnameskill;

		private Button button11;

		private Button button12;

		private TextBox txtbufff;

		private TabPage tabPage16;

		private Button button13;

		private Button button14;

		private Button button15;

		private Button button16;

		private Button button17;

		public Debug(Game game)
		{
			this.game = game;
			InitializeComponent();
			Text = game.TLBB.Name;
			probGame.SelectedObject = game.Process;
		}

		private void btnReadGameObject_Click(object sender, EventArgs e)
		{
			Text = (DateTime.Now.Ticks / 10000000).ToString();
			listViewGameObject.Items.Clear();
			float num = 9999f;
			foreach (GameObject item in game.Objects.All)
			{
				float distance = TINHKIEM.GetDistance(item.X, item.Y, game.CharX, game.CharY);
				if (distance < num && item.Name != game.TLBB.Name)
				{
					num = distance;
					_ = item.Name;
				}
				ListViewItem listViewItem = new ListViewItem(item.Id.ToString());
				listViewItem.SubItems.Add(item.Name + " | " + item.RoundX + "," + item.RoundY);
				listViewItem.SubItems.Add(TINHKIEM.GetDistance(game.CharX, game.CharY, item.X, item.Y).ToString());
				listViewItem.Tag = item;
				listViewGameObject.Items.Add(listViewItem);
			}
			listViewGameObject.Columns[0].Text = listViewGameObject.Items.Count.ToString();
		}

		private void listViewGameObject_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewGameObject.SelectedItems.Count != 0)
			{
				GameObject selectedObject = listViewGameObject.SelectedItems[0].Tag as GameObject;
				pgGameObject.SelectedObject = selectedObject;
			}
		}

		private void tmrRefresh_Tick(object sender, EventArgs e)
		{
			txtTLBB.Text = game.TLBB.ToString();
		}

		private void btnReadSkill_Click(object sender, EventArgs e)
		{
			listViewSkill.Items.Clear();
			foreach (Skill skill in game.Skills)
			{
				ListViewItem value = new ListViewItem(new string[3]
				{
					skill.PacketId.ToString(),
					skill.Name,
					skill.DelayOffset.ToString("X8")
				});
				listViewSkill.Items.Add(value);
			}
			listViewSkill.Columns[0].Text = listViewSkill.Items.Count.ToString();
		}

		private void btnDoString_Click(object sender, EventArgs e)
		{
			game.LuaDoString(txtDoString.Text);
		}

		private void btnGameControl_Click(object sender, EventArgs e)
		{
			listViewGameControl.Items.Clear();
			foreach (GameControl item in GameControl.Enum(game))
			{
				ListViewItem listViewItem = new ListViewItem(item.Id.ToString());
				listViewItem.SubItems.Add(item.Name);
				listViewItem.SubItems.Add(item.PacketId.ToString());
				listViewItem.Tag = item;
				listViewGameControl.Items.Add(listViewItem);
			}
		}

		private void btnGameObjectParty_Click(object sender, EventArgs e)
		{
			listViewGameObject.Items.Clear();
			foreach (GameObject item in game.Objects.Party)
			{
				ListViewItem listViewItem = new ListViewItem(item.Id.ToString());
				listViewItem.SubItems.Add(item.Name);
				listViewItem.Tag = item;
				listViewGameObject.Items.Add(listViewItem);
			}
		}

		private void btnCompress_Click(object sender, EventArgs e)
		{
			MessageBox.Show(game.TrangThaiXayDung);
		}

		private void btnToString_Click(object sender, EventArgs e)
		{
			game.LuaDoOneLineString(txtDoString.Text);
			game.LuaToString();
			string text = game.LuaString();
			MessageBox.Show(text);
			if (text != "")
			{
				Clipboard.SetText(text);
			}
		}

		public static string Int2Hex(int val)
		{
			return val.ToString("X8").Substring(5, 3);
		}

		private void btnReadTask_Click(object sender, EventArgs e)
		{
			listViewTask.Items.Clear();
			foreach (Task item in Task.Enum(game))
			{
				if (!(item.Name.Trim() == ""))
				{
					ListViewItem listViewItem = new ListViewItem(new string[2]
					{
						item.Id.ToString("X8"),
						item.Name
					});
					listViewItem.Tag = item;
					listViewTask.Items.Add(listViewItem);
				}
			}
			listViewTask.Columns[0].Text = "Id [" + listViewTask.Items.Count + "]";
		}

		private void listViewTask_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewTask.SelectedItems.Count > 0)
			{
				Task task = (Task)listViewTask.SelectedItems[0].Tag;
				txtTask.Text = task.ToString();
			}
		}

		private void btnReadDialog_Click(object sender, EventArgs e)
		{
			string text = "@";
			listViewDialog.Items.Clear();
			List<QuestFrame> list = QuestFrame.Enum(game);
			Text = QuestFrame.Enum(game).Count.ToString();
			foreach (QuestFrame item in list)
			{
				ListViewItem listViewItem = new ListViewItem(new string[1] { item.Name });
				listViewItem.Tag = item;
				text += item.Name;
				listViewDialog.Items.Add(listViewItem);
			}
			listViewDialog.Columns[0].Text = "Id [" + listViewDialog.Items.Count + "]";
			Clipboard.SetText(text);
		}

		private void listViewDialog_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewDialog.SelectedItems.Count != 0)
			{
				QuestFrame questFrame = listViewDialog.SelectedItems[0].Tag as QuestFrame;
				txtDialog.Text = questFrame.ToString();
			}
		}

		private void txtDoString_KeyDown(object sender, KeyEventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			if (e.Control && e.KeyCode == Keys.A)
			{
				textBox.SelectAll();
			}
		}

		private void btnReadPacket_Click(object sender, EventArgs e)
		{
			listViewPacket.Items.Clear();
			foreach (PacketItem item in PacketItem.Enum(game))
			{
				ListViewItem listViewItem = new ListViewItem(new string[2]
				{
					item.PacketId.ToString(),
					item.Name
				});
				listViewItem.Tag = item;
				listViewPacket.Items.Add(listViewItem);
			}
		}

		private void listViewPacket_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewPacket.SelectedItems.Count > 0)
			{
				PacketItem packetItem = (PacketItem)listViewPacket.SelectedItems[0].Tag;
				txtPacket.Text = packetItem.Info;
			}
		}

		private void btnGameObjectMonter_Click(object sender, EventArgs e)
		{
			listViewGameObject.Items.Clear();
			foreach (GameObject item in game.Objects.Monter)
			{
				ListViewItem listViewItem = new ListViewItem(item.Id.ToString());
				listViewItem.SubItems.Add(item.Name);
				listViewItem.Tag = item;
				listViewGameObject.Items.Add(listViewItem);
			}
		}

		private void btnReadShop_Click(object sender, EventArgs e)
		{
			listViewShop.Items.Clear();
			foreach (Shop item in Shop.Enum(game))
			{
				ListViewItem listViewItem = new ListViewItem(new string[3]
				{
					item.DefineId.ToString(),
					item.Name,
					item.Class.ToString("X8")
				});
				listViewItem.Tag = item;
				listViewShop.Items.Add(listViewItem);
			}
		}

		private void listViewGameControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewGameControl.SelectedItems.Count != 0)
			{
				GameControl gameControl = listViewGameControl.SelectedItems[0].Tag as GameControl;
				txtGameControl.Text = gameControl.ToString();
			}
		}

		private void btnResetTime_Click(object sender, EventArgs e)
		{
			game.ResetTime();
		}

		private void btn20m_Click(object sender, EventArgs e)
		{
			listViewGameObject.Items.Clear();
			foreach (GameObject item in game.Objects.NearMonter20m)
			{
				ListViewItem listViewItem = new ListViewItem(item.Id.ToString());
				listViewItem.SubItems.Add(item.Name);
				listViewItem.Tag = item;
				listViewGameObject.Items.Add(listViewItem);
			}
		}

		private void btnSendPacket_Click(object sender, EventArgs e)
		{
			game.SendPacket(txtSendPacket.Text);
		}

		private void txtCode_TextChanged(object sender, EventArgs e)
		{
			string text = txtCode.Text;
			text = text.Replace('\t', ' ');
			text = text.Replace('\r', ' ');
			text = text.Replace('\n', ' ');
			text = text.Replace("  ", " ");
			text = text.Replace("  ", " ");
			text = text.Replace("  ", " ");
			text = text.Replace("  ", " ");
			text = text.Replace("  ", " ");
			txtNewCode.Text = text;
		}

		private void txtCode_KeyDown(object sender, KeyEventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			if (e.Control && e.KeyCode == Keys.A)
			{
				textBox.SelectAll();
			}
		}

		private void txtNewCode_KeyDown(object sender, KeyEventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			if (e.Control && e.KeyCode == Keys.A)
			{
				textBox.SelectAll();
			}
		}

		private void txtSendPacket_TextChanged(object sender, EventArgs e)
		{
		}

		private void listViewShop_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewShop.SelectedItems.Count > 0)
			{
				Shop shop = (Shop)listViewShop.SelectedItems[0].Tag;
				txtShop.Text = shop.ToString();
			}
		}

		private void listViewShop_DoubleClick(object sender, EventArgs e)
		{
			if (listViewShop.SelectedItems.Count > 0)
			{
				Shop shop = (Shop)listViewShop.SelectedItems[0].Tag;
				game.Buy(shop.Index);
			}
		}

		private void btnPushDebugMessage_Click(object sender, EventArgs e)
		{
			game.LuaDoUnicodeString("PushDebugMessage(\"" + txtPushDebugMessage.Text + "\");");
		}

		private void txtUnicode_TextChanged(object sender, EventArgs e)
		{
		}

		private void txtVISCII_TextChanged(object sender, EventArgs e)
		{
			txtUnicode.Text = Memory.VISCII2Unicode(txtVISCII.Text);
		}

		private void txtUnicode_KeyDown(object sender, KeyEventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			if (e.Control && e.KeyCode == Keys.A)
			{
				textBox.SelectAll();
			}
		}

		private void txtVISCII_KeyDown(object sender, KeyEventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			if (e.Control && e.KeyCode == Keys.A)
			{
				textBox.SelectAll();
			}
		}

		private void btnSendCaptcha_Click(object sender, EventArgs e)
		{
			FileStream fileStream = new FileStream("d:\\captcha.jpg", FileMode.Open, FileAccess.Read);
			byte[] array = new byte[fileStream.Length];
			fileStream.Read(array, 0, array.Length);
			fileStream.Close();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("file", new FormUpload.FileParameter(array, "captcha.jpg", "image/jpeg"));
			dictionary.Add("key", "[REDACTED]" /* analysis redaction */);
			dictionary.Add("numeric", "1");
			dictionary.Add("min_len", "4");
			dictionary.Add("max_len", "4");
			dictionary.Add("submit", "download and get the ID");
			string userAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36";
			HttpWebResponse httpWebResponse = FormUpload.MultipartFormDataPost("http://2captcha.com/in.php", userAgent, dictionary);
			string text = new StreamReader(httpWebResponse.GetResponseStream()).ReadToEnd();
			httpWebResponse.Close();
			MessageBox.Show(text);
		}

		private void btnAtk_Click(object sender, EventArgs e)
		{
			if (btnAtk.Text == "Atk")
			{
				btnAtk.Text = "Stop";
			}
			else
			{
				btnAtk.Text = "Atk";
			}
			IsAtk = !IsAtk;
			Atk();
		}

		private void Atk()
		{
			while ((decimal)listAtk.Count < nudAtkCount.Value && IsAtk)
			{
				Poster poster = new Poster();
				listAtk.Add(poster);
				poster.Url = txtUrlAtk.Text;
				poster.Control = this;
				poster.Completed += poster_Completed;
				poster.Get();
			}
		}

		private void poster_Completed(object sender, EventArgs e)
		{
			Poster item = sender as Poster;
			listAtk.Remove(item);
			Atk();
		}

		[DllImport("user32.dll")]
		private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		[DllImport("USER32.DLL")]
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		private void btnTest_Click(object sender, EventArgs e)
		{
			game.LUA.TogleMissionOutline();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Process process = game.Process;
			for (int i = 0; i < process.Modules.Count; i++)
			{
				if (process.Modules[i].FileName.ToLower().Contains("static"))
				{
					MessageBox.Show(process.Modules[i].ModuleMemorySize.ToString("X8"));
				}
			}
		}

		private void Debug_Load(object sender, EventArgs e)
		{
			pgTLBB.SelectedObject = game.TLBB;
		}

		private void btnPhone_Click(object sender, EventArgs e)
		{
			MessageBox.Show(TINHKIEM.IsPhoneNumber(txtPhone.Text).ToString());
		}

		private void button2_Click(object sender, EventArgs e)
		{
			_ = game.RecvDat;
			txtDoString.Text = game.RecvDat;
		}

		private void btnLoadScripts_Click(object sender, EventArgs e)
		{
			listViewScript.Items.Clear();
			foreach (Script item in Scripts.Load())
			{
				ListViewItem listViewItem = new ListViewItem(item.ID);
				listViewItem.SubItems.Add("");
				listViewItem.SubItems.Add("");
				listViewItem.SubItems.Add("");
				listViewItem.SubItems.Add("");
				listViewItem.SubItems.Add("");
				listViewItem.Tag = item;
				listViewItem.SubItems[1].Text = item.MD;
				listViewItem.SubItems[2].Text = item.Recv;
				listViewItem.SubItems[3].Text = item.Send;
				listViewItem.SubItems[4].Text = item.Do;
				listViewItem.SubItems[5].Text = item.Info;
				listViewScript.Items.Add(listViewItem);
			}
		}

		private void menuEditScript_Click(object sender, EventArgs e)
		{
		}

		private void button3_Click(object sender, EventArgs e)
		{
			game.LuaDoString(ImageResource.LuaEx);
			game.LuaToString();
			string text = game.LuaStringEx();
			MessageBox.Show(text);
			if (text != "")
			{
				Clipboard.SetText(text);
			}
			List<Script> list = Scripts.Load();
			if (MessageBox.Show(this, "Bạn có muốn thêm script", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				text = text.Replace("=", ":");
				text = text.Replace("   ", "");
				text = text.Replace("  ", "");
				text.Trim(';');
				string[] array = text.Split(';');
				foreach (string text2 in array)
				{
					if (text2.Trim() == "")
					{
						continue;
					}
					try
					{
						Stopwatch.StartNew();
						int num = TINHKIEM.ParseInt(text2.Substring(0, text2.IndexOf(' ')));
						string text3 = text2.Substring(text2.IndexOf(' ') + 1);
						string text4 = TINHKIEM.Hasher.MD5(text3);
						foreach (Script item in list)
						{
							item.MD.Contains(text4);
						}
						string text5 = Regex.Replace(text3, ".*{_INFOAIM", "");
						int val = TINHKIEM.ParseInt(text5.Split(',')[0]);
						int val2 = TINHKIEM.ParseInt(text5.Split(',')[1]);
						int val3 = 0;
						try
						{
							val3 = TINHKIEM.ParseInt(text5.Split(',')[2]);
						}
						catch
						{
						}
						Scripts.Add(new Script
						{
							MD = text4,
							Recv = Int2Hex(0) + Int2Hex(val) + Int2Hex(val2) + Int2Hex(val3),
							Send = Int2Hex(0) + Int2Hex(val) + Int2Hex(val2) + Int2Hex(val3),
							Level = num.ToString(),
							Info = text3
						});
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message + ex.StackTrace);
					}
				}
			}
			Scripts.Save();
		}

		private void menuDeleteScript_Click(object sender, EventArgs e)
		{
			if (listViewScript.SelectedItems.Count == 0 || MessageBox.Show(this, "Bạn có muốn xóa thông tin những nhiệm vụ đã chọn", "", MessageBoxButtons.YesNo) != DialogResult.Yes)
			{
				return;
			}
			foreach (ListViewItem selectedItem in listViewScript.SelectedItems)
			{
				try
				{
					XmlNode node = ((Script)selectedItem.Tag).Node;
					Scripts.XML.SelectSingleNode("/*").RemoveChild(node);
					selectedItem.Remove();
				}
				catch
				{
				}
			}
			Scripts.Save();
			Scripts.Load();
		}

		private void talkToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (listViewGameObject.SelectedItems.Count > 0)
			{
				GameObject gameObject = listViewGameObject.SelectedItems[0].Tag as GameObject;
				game.Talk(gameObject.Id);
			}
		}

		private void pickToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (listViewGameObject.SelectedItems.Count > 0)
			{
				GameObject gameObject = listViewGameObject.SelectedItems[0].Tag as GameObject;
				game.PickItem(gameObject.Id);
			}
		}

		private void collectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (listViewGameObject.SelectedItems.Count > 0)
			{
				GameObject gameObject = listViewGameObject.SelectedItems[0].Tag as GameObject;
				game.UseSkill(3, gameObject.Id);
			}
		}

		private void listViewScript_DragDrop(object sender, DragEventArgs e)
		{
			XmlNode xmlNode = Scripts.XML.SelectSingleNode("/*");
			xmlNode.RemoveAll();
			IEnumerator enumerator = listViewScript.Items.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Script script = ((ListViewItem)enumerator.Current).Tag as Script;
				xmlNode.AppendChild(script.Node);
			}
			Scripts.Save();
		}

		private void instanceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (listViewGameObject.SelectedItems.Count > 0)
			{
				GameObject gameObject = listViewGameObject.SelectedItems[0].Tag as GameObject;
				Clipboard.SetText("public static NPC " + TINHKIEM.ClearSign(gameObject.Name).Replace(" ", "") + "  = new NPC()\r\n{\r\n\tId = " + gameObject.Id + ",\r\n\tX = " + gameObject.RoundX + ",\r\n\tY = " + gameObject.RoundY + ",\r\n\tMap = Id,\r\n\tINFOAIM = \"#G" + game.TLBB.MapName + "#R" + gameObject.Name + "#{_INFOAIM" + gameObject.RoundX + "," + gameObject.RoundY + "," + game.TLBB.MapId + "," + gameObject.Name + "}\"\r\n};");
			}
		}

		private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			listViewScript.Items.Clear();
			foreach (Script item in Scripts.Load())
			{
				ListViewItem listViewItem = new ListViewItem(item.ID);
				listViewItem.SubItems.Add("");
				listViewItem.SubItems.Add("");
				listViewItem.SubItems.Add("");
				listViewItem.SubItems.Add("");
				listViewItem.SubItems.Add("");
				listViewItem.SubItems.Add("");
				listViewItem.SubItems.Add("");
				listViewItem.Tag = item;
				listViewItem.SubItems[1].Text = item.MD;
				listViewItem.SubItems[2].Text = item.Recv;
				listViewItem.SubItems[3].Text = item.Send;
				listViewItem.SubItems[4].Text = item.Do;
				listViewItem.SubItems[5].Text = item.Info;
				listViewItem.SubItems[6].Text = item.Name;
				listViewItem.SubItems[7].Text = item.Level;
				listViewScript.Items.Add(listViewItem);
			}
		}

		private void listViewScript_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		private void Editer_Edited(object sender, EventArgs e)
		{
			if (listViewScript.SelectedItems.Count != 0)
			{
				ListViewItem listViewItem = listViewScript.SelectedItems[0];
				Script script = listViewScript.SelectedItems[0].Tag as Script;
				listViewItem.Text = script.ID;
				listViewItem.SubItems[1].Text = script.MD;
				listViewItem.SubItems[2].Text = script.Recv;
				listViewItem.SubItems[3].Text = script.Send;
				listViewItem.SubItems[4].Text = script.Do;
				listViewItem.SubItems[5].Text = script.Info;
				listViewItem.SubItems[6].Text = script.Name;
				MessageBox.Show("Saved");
			}
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			NotRead = chkNotRead.Checked;
		}

		private void menuPickObject_Click(object sender, EventArgs e)
		{
			if (listViewGameObject.SelectedItems.Count > 0)
			{
				GameObject gameObject = listViewGameObject.SelectedItems[0].Tag as GameObject;
				game.PickObject(gameObject);
			}
		}

		private void btnrRemoveHex_Click(object sender, EventArgs e)
		{
			txtVISCII.Text = ConverterEx.Hex2String(txtUnicode.Text);
			txtVISCII.Text = ConverterEx.CleanJarVar(txtVISCII.Text);
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		private void button4_Click(object sender, EventArgs e)
		{
			txtVISCII.Text = ConverterEx.Unicode2VISCII(txtUnicode.Text);
		}

		private void button5_Click(object sender, EventArgs e)
		{
			listViewPacket.Items.Clear();
			foreach (PacketItem item in PacketItem.EnumTrangBi(game))
			{
				ListViewItem listViewItem = new ListViewItem(new string[2]
				{
					item.PacketId.ToString(),
					item.Name
				});
				listViewItem.Tag = item;
				listViewPacket.Items.Add(listViewItem);
			}
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			offset = (int)numericUpDown1.Value;
		}

		private void button6_Click(object sender, EventArgs e)
		{
			foreach (Modules item in ProcessManager.CollectModules(game.Process))
			{
				txtDoString.Text = txtDoString.Text + item.ModuleName + "\r\n";
			}
		}

		private void button7_Click(object sender, EventArgs e)
		{
			listViewPacket.Items.Clear();
			foreach (Bank item in new Bank(game).Enum())
			{
				ListViewItem listViewItem = new ListViewItem(new string[2]
				{
					item.PacketId.ToString(),
					item.Name
				});
				listViewItem.Tag = item;
				listViewPacket.Items.Add(listViewItem);
			}
		}

		private void tmrTest_Tick(object sender, EventArgs e)
		{
		}

		private void button9_Click(object sender, EventArgs e)
		{
			FrmMain.CurGame.GetBestTarget();
			if (FrmMain.CurGame.BestTarget != null)
			{
				FrmMain.CurGame.UseSkill(int.Parse(txtidskill.Text), FrmMain.CurGame.BestTarget.Id);
			}
		}

		private void listViewSkill_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewSkill.SelectedItems.Count != 0)
			{
				ListViewItem listViewItem = listViewSkill.SelectedItems[0];
				txtidskill.Text = listViewItem.Text;
				txtnameskill.Text = listViewItem.SubItems[1].Text;
			}
		}

		private void button8_Click(object sender, EventArgs e)
		{
			FrmMain.CurGame.GetBestTarget();
		}

		private void button10_Click(object sender, EventArgs e)
		{
			FrmMain.CurGame.UseSkill(int.Parse(txtidskill.Text));
		}

		private void button11_Click(object sender, EventArgs e)
		{
			FrmMain.CurGame.UseSkill(int.Parse(txtidskill.Text), FrmMain.CurGame.Objects.Self.Id);
		}

		private void button12_Click(object sender, EventArgs e)
		{
			txtbufff.Text = FrmMain.CurGame.Objects.Self.BuffToString;
		}

		private void button13_Click(object sender, EventArgs e)
		{
			MessageBox.Show(game.TLBB.GetMonPhaiName());
		}

		private void button14_Click(object sender, EventArgs e)
		{
			MessageBox.Show(game.TLBB.Rage.ToString() ?? "");
		}

		private void button15_Click(object sender, EventArgs e)
		{
			FrmMain.CurGame.KinhCong();
		}

		private void button16_Click(object sender, EventArgs e)
		{
			FrmMain.CurGame.GiamDinh();
		}

		private void button17_Click(object sender, EventArgs e)
		{
			FrmMain.CurGame.DragTo42();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TinhKiemAuto.Debug));
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.splitContainer4 = new System.Windows.Forms.SplitContainer();
			this.listViewGameObject = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader17 = new System.Windows.Forms.ColumnHeader();
			this.menuGameObject = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.talkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.collectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.instanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuPickObject = new System.Windows.Forms.ToolStripMenuItem();
			this.pgGameObject = new System.Windows.Forms.PropertyGrid();
			this.btn20m = new System.Windows.Forms.Button();
			this.btnGameObjectMonter = new System.Windows.Forms.Button();
			this.btnGameObjectParty = new System.Windows.Forms.Button();
			this.btnReadGameObject = new System.Windows.Forms.Button();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.button15 = new System.Windows.Forms.Button();
			this.button14 = new System.Windows.Forms.Button();
			this.txtbufff = new System.Windows.Forms.TextBox();
			this.button12 = new System.Windows.Forms.Button();
			this.button11 = new System.Windows.Forms.Button();
			this.txtidskill = new System.Windows.Forms.TextBox();
			this.txtnameskill = new System.Windows.Forms.TextBox();
			this.button10 = new System.Windows.Forms.Button();
			this.button9 = new System.Windows.Forms.Button();
			this.button8 = new System.Windows.Forms.Button();
			this.btnReadSkill = new System.Windows.Forms.Button();
			this.listViewSkill = new System.Windows.Forms.ListView();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader26 = new System.Windows.Forms.ColumnHeader();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.button6 = new System.Windows.Forms.Button();
			this.chkNotRead = new System.Windows.Forms.CheckBox();
			this.button3 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.btnPhone = new System.Windows.Forms.Button();
			this.txtPhone = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.btnTest = new System.Windows.Forms.Button();
			this.nudAtkCount = new System.Windows.Forms.NumericUpDown();
			this.btnAtk = new System.Windows.Forms.Button();
			this.txtUrlAtk = new System.Windows.Forms.TextBox();
			this.btnSendCaptcha = new System.Windows.Forms.Button();
			this.btnPushDebugMessage = new System.Windows.Forms.Button();
			this.txtPushDebugMessage = new System.Windows.Forms.TextBox();
			this.btnSendPacket = new System.Windows.Forms.Button();
			this.txtSendPacket = new System.Windows.Forms.TextBox();
			this.btnResetTime = new System.Windows.Forms.Button();
			this.btnToString = new System.Windows.Forms.Button();
			this.btnCompress = new System.Windows.Forms.Button();
			this.btnDoString = new System.Windows.Forms.Button();
			this.txtDoString = new System.Windows.Forms.TextBox();
			this.tabPage4 = new System.Windows.Forms.TabPage();
			this.btnGameControl = new System.Windows.Forms.Button();
			this.txtGameControl = new System.Windows.Forms.TextBox();
			this.listViewGameControl = new System.Windows.Forms.ListView();
			this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
			this.tabPage5 = new System.Windows.Forms.TabPage();
			this.txtTask = new System.Windows.Forms.TextBox();
			this.btnReadTask = new System.Windows.Forms.Button();
			this.listViewTask = new System.Windows.Forms.ListView();
			this.columnHeader13 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader14 = new System.Windows.Forms.ColumnHeader();
			this.tabPage6 = new System.Windows.Forms.TabPage();
			this.button16 = new System.Windows.Forms.Button();
			this.txtDialog = new System.Windows.Forms.TextBox();
			this.btnReadDialog = new System.Windows.Forms.Button();
			this.listViewDialog = new System.Windows.Forms.ListView();
			this.columnHeader15 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader16 = new System.Windows.Forms.ColumnHeader();
			this.tabPage7 = new System.Windows.Forms.TabPage();
			this.button17 = new System.Windows.Forms.Button();
			this.button7 = new System.Windows.Forms.Button();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.button5 = new System.Windows.Forms.Button();
			this.txtPacket = new System.Windows.Forms.TextBox();
			this.btnReadPacket = new System.Windows.Forms.Button();
			this.listViewPacket = new System.Windows.Forms.ListView();
			this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader9 = new System.Windows.Forms.ColumnHeader();
			this.tabPage8 = new System.Windows.Forms.TabPage();
			this.tabPage9 = new System.Windows.Forms.TabPage();
			this.txtShop = new System.Windows.Forms.TextBox();
			this.btnReadShop = new System.Windows.Forms.Button();
			this.listViewShop = new System.Windows.Forms.ListView();
			this.columnHeader10 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader11 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader12 = new System.Windows.Forms.ColumnHeader();
			this.tabPage10 = new System.Windows.Forms.TabPage();
			this.txtTLBB = new System.Windows.Forms.TextBox();
			this.tabPage11 = new System.Windows.Forms.TabPage();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.txtCode = new System.Windows.Forms.TextBox();
			this.txtNewCode = new System.Windows.Forms.TextBox();
			this.tabPage12 = new System.Windows.Forms.TabPage();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.button4 = new System.Windows.Forms.Button();
			this.btnrRemoveHex = new System.Windows.Forms.Button();
			this.txtUnicode = new System.Windows.Forms.TextBox();
			this.txtVISCII = new System.Windows.Forms.TextBox();
			this.tabPage13 = new System.Windows.Forms.TabPage();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.listViewScript = new System.Windows.Forms.ListView();
			this.columnHeader18 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader19 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader20 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader21 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader22 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader23 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader24 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader25 = new System.Windows.Forms.ColumnHeader();
			this.menuScript = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuAddScript = new System.Windows.Forms.ToolStripMenuItem();
			this.menuEditScript = new System.Windows.Forms.ToolStripMenuItem();
			this.menuDeleteScript = new System.Windows.Forms.ToolStripMenuItem();
			this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tabPage14 = new System.Windows.Forms.TabPage();
			this.probGame = new System.Windows.Forms.PropertyGrid();
			this.tabPage15 = new System.Windows.Forms.TabPage();
			this.pgTLBB = new System.Windows.Forms.PropertyGrid();
			this.tabPage16 = new System.Windows.Forms.TabPage();
			this.button13 = new System.Windows.Forms.Button();
			this.tmrRefresh = new System.Windows.Forms.Timer(this.components);
			this.tmrTest = new System.Windows.Forms.Timer(this.components);
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.splitContainer4.Panel1.SuspendLayout();
			this.splitContainer4.Panel2.SuspendLayout();
			this.splitContainer4.SuspendLayout();
			this.menuGameObject.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.nudAtkCount).BeginInit();
			this.tabPage4.SuspendLayout();
			this.tabPage5.SuspendLayout();
			this.tabPage6.SuspendLayout();
			this.tabPage7.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.numericUpDown1).BeginInit();
			this.tabPage9.SuspendLayout();
			this.tabPage10.SuspendLayout();
			this.tabPage11.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tabPage12.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.tabPage13.SuspendLayout();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			this.menuScript.SuspendLayout();
			this.tabPage14.SuspendLayout();
			this.tabPage15.SuspendLayout();
			this.tabPage16.SuspendLayout();
			base.SuspendLayout();
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Controls.Add(this.tabPage4);
			this.tabControl1.Controls.Add(this.tabPage5);
			this.tabControl1.Controls.Add(this.tabPage6);
			this.tabControl1.Controls.Add(this.tabPage7);
			this.tabControl1.Controls.Add(this.tabPage8);
			this.tabControl1.Controls.Add(this.tabPage9);
			this.tabControl1.Controls.Add(this.tabPage10);
			this.tabControl1.Controls.Add(this.tabPage11);
			this.tabControl1.Controls.Add(this.tabPage12);
			this.tabControl1.Controls.Add(this.tabPage13);
			this.tabControl1.Controls.Add(this.tabPage14);
			this.tabControl1.Controls.Add(this.tabPage15);
			this.tabControl1.Controls.Add(this.tabPage16);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(906, 571);
			this.tabControl1.TabIndex = 0;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(tabControl1_SelectedIndexChanged);
			this.tabPage1.Controls.Add(this.splitContainer4);
			this.tabPage1.Controls.Add(this.btn20m);
			this.tabPage1.Controls.Add(this.btnGameObjectMonter);
			this.tabPage1.Controls.Add(this.btnGameObjectParty);
			this.tabPage1.Controls.Add(this.btnReadGameObject);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(898, 545);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "GameObject";
			this.tabPage1.UseVisualStyleBackColor = true;
			this.splitContainer4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.splitContainer4.Location = new System.Drawing.Point(3, 3);
			this.splitContainer4.Name = "splitContainer4";
			this.splitContainer4.Panel1.Controls.Add(this.listViewGameObject);
			this.splitContainer4.Panel2.Controls.Add(this.pgGameObject);
			this.splitContainer4.Size = new System.Drawing.Size(892, 505);
			this.splitContainer4.SplitterDistance = 451;
			this.splitContainer4.TabIndex = 10;
			this.listViewGameObject.AllowColumnReorder = true;
			this.listViewGameObject.AllowDrop = true;
			this.listViewGameObject.Columns.AddRange(new System.Windows.Forms.ColumnHeader[3] { this.columnHeader1, this.columnHeader2, this.columnHeader17 });
			this.listViewGameObject.ContextMenuStrip = this.menuGameObject;
			this.listViewGameObject.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewGameObject.FullRowSelect = true;
			this.listViewGameObject.GridLines = true;
			this.listViewGameObject.HideSelection = false;
			this.listViewGameObject.Location = new System.Drawing.Point(0, 0);
			this.listViewGameObject.Name = "listViewGameObject";
			this.listViewGameObject.Size = new System.Drawing.Size(451, 505);
			this.listViewGameObject.TabIndex = 0;
			this.listViewGameObject.UseCompatibleStateImageBehavior = false;
			this.listViewGameObject.View = System.Windows.Forms.View.Details;
			this.listViewGameObject.SelectedIndexChanged += new System.EventHandler(listViewGameObject_SelectedIndexChanged);
			this.columnHeader1.Text = "Id";
			this.columnHeader2.Text = "Name";
			this.columnHeader2.Width = 166;
			this.columnHeader17.Text = "Khoảng Cách";
			this.columnHeader17.Width = 85;
			this.menuGameObject.Items.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.talkToolStripMenuItem, this.pickToolStripMenuItem, this.collectToolStripMenuItem, this.instanceToolStripMenuItem, this.menuPickObject });
			this.menuGameObject.Name = "menuGameObject";
			this.menuGameObject.Size = new System.Drawing.Size(135, 114);
			this.talkToolStripMenuItem.Name = "talkToolStripMenuItem";
			this.talkToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
			this.talkToolStripMenuItem.Text = "Talk";
			this.talkToolStripMenuItem.Click += new System.EventHandler(talkToolStripMenuItem_Click);
			this.pickToolStripMenuItem.Name = "pickToolStripMenuItem";
			this.pickToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
			this.pickToolStripMenuItem.Text = "Pick";
			this.pickToolStripMenuItem.Click += new System.EventHandler(pickToolStripMenuItem_Click);
			this.collectToolStripMenuItem.Name = "collectToolStripMenuItem";
			this.collectToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
			this.collectToolStripMenuItem.Text = "Collect";
			this.collectToolStripMenuItem.Click += new System.EventHandler(collectToolStripMenuItem_Click);
			this.instanceToolStripMenuItem.Name = "instanceToolStripMenuItem";
			this.instanceToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
			this.instanceToolStripMenuItem.Text = "Instance";
			this.instanceToolStripMenuItem.Click += new System.EventHandler(instanceToolStripMenuItem_Click);
			this.menuPickObject.Name = "menuPickObject";
			this.menuPickObject.Size = new System.Drawing.Size(134, 22);
			this.menuPickObject.Text = "Pick Object";
			this.menuPickObject.Click += new System.EventHandler(menuPickObject_Click);
			this.pgGameObject.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgGameObject.HelpVisible = false;
			this.pgGameObject.LineColor = System.Drawing.SystemColors.ControlDark;
			this.pgGameObject.Location = new System.Drawing.Point(0, 0);
			this.pgGameObject.Name = "pgGameObject";
			this.pgGameObject.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
			this.pgGameObject.Size = new System.Drawing.Size(437, 505);
			this.pgGameObject.TabIndex = 0;
			this.btn20m.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btn20m.Location = new System.Drawing.Point(89, 516);
			this.btn20m.Name = "btn20m";
			this.btn20m.Size = new System.Drawing.Size(75, 23);
			this.btn20m.TabIndex = 9;
			this.btn20m.Text = "20m";
			this.btn20m.UseVisualStyleBackColor = true;
			this.btn20m.Click += new System.EventHandler(btn20m_Click);
			this.btnGameObjectMonter.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnGameObjectMonter.Location = new System.Drawing.Point(170, 516);
			this.btnGameObjectMonter.Name = "btnGameObjectMonter";
			this.btnGameObjectMonter.Size = new System.Drawing.Size(75, 23);
			this.btnGameObjectMonter.TabIndex = 8;
			this.btnGameObjectMonter.Text = "Monter";
			this.btnGameObjectMonter.UseVisualStyleBackColor = true;
			this.btnGameObjectMonter.Click += new System.EventHandler(btnGameObjectMonter_Click);
			this.btnGameObjectParty.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnGameObjectParty.Location = new System.Drawing.Point(251, 516);
			this.btnGameObjectParty.Name = "btnGameObjectParty";
			this.btnGameObjectParty.Size = new System.Drawing.Size(75, 23);
			this.btnGameObjectParty.TabIndex = 7;
			this.btnGameObjectParty.Text = "Party";
			this.btnGameObjectParty.UseVisualStyleBackColor = true;
			this.btnGameObjectParty.Click += new System.EventHandler(btnGameObjectParty_Click);
			this.btnReadGameObject.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnReadGameObject.Location = new System.Drawing.Point(8, 514);
			this.btnReadGameObject.Name = "btnReadGameObject";
			this.btnReadGameObject.Size = new System.Drawing.Size(75, 23);
			this.btnReadGameObject.TabIndex = 2;
			this.btnReadGameObject.Text = "Read";
			this.btnReadGameObject.UseVisualStyleBackColor = true;
			this.btnReadGameObject.Click += new System.EventHandler(btnReadGameObject_Click);
			this.tabPage2.Controls.Add(this.button15);
			this.tabPage2.Controls.Add(this.button14);
			this.tabPage2.Controls.Add(this.txtbufff);
			this.tabPage2.Controls.Add(this.button12);
			this.tabPage2.Controls.Add(this.button11);
			this.tabPage2.Controls.Add(this.txtidskill);
			this.tabPage2.Controls.Add(this.txtnameskill);
			this.tabPage2.Controls.Add(this.button10);
			this.tabPage2.Controls.Add(this.button9);
			this.tabPage2.Controls.Add(this.button8);
			this.tabPage2.Controls.Add(this.btnReadSkill);
			this.tabPage2.Controls.Add(this.listViewSkill);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(898, 545);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Skill";
			this.tabPage2.UseVisualStyleBackColor = true;
			this.button15.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.button15.Location = new System.Drawing.Point(414, 177);
			this.button15.Name = "button15";
			this.button15.Size = new System.Drawing.Size(133, 23);
			this.button15.TabIndex = 13;
			this.button15.Text = "KHIN CÔNG";
			this.button15.UseVisualStyleBackColor = true;
			this.button15.Click += new System.EventHandler(button15_Click);
			this.button14.Location = new System.Drawing.Point(646, 386);
			this.button14.Name = "button14";
			this.button14.Size = new System.Drawing.Size(151, 23);
			this.button14.TabIndex = 12;
			this.button14.Text = "CHECK RANGER";
			this.button14.UseVisualStyleBackColor = true;
			this.button14.Click += new System.EventHandler(button14_Click);
			this.txtbufff.Location = new System.Drawing.Point(569, 180);
			this.txtbufff.Name = "txtbufff";
			this.txtbufff.Size = new System.Drawing.Size(311, 20);
			this.txtbufff.TabIndex = 11;
			this.button12.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.button12.Location = new System.Drawing.Point(664, 221);
			this.button12.Name = "button12";
			this.button12.Size = new System.Drawing.Size(133, 23);
			this.button12.TabIndex = 10;
			this.button12.Text = "READ BUFFF";
			this.button12.UseVisualStyleBackColor = true;
			this.button12.Click += new System.EventHandler(button12_Click);
			this.button11.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.button11.Location = new System.Drawing.Point(432, 350);
			this.button11.Name = "button11";
			this.button11.Size = new System.Drawing.Size(133, 23);
			this.button11.TabIndex = 9;
			this.button11.Text = "Buff BẢN THÂN";
			this.button11.UseVisualStyleBackColor = true;
			this.button11.Click += new System.EventHandler(button11_Click);
			this.txtidskill.Location = new System.Drawing.Point(446, 44);
			this.txtidskill.Name = "txtidskill";
			this.txtidskill.Size = new System.Drawing.Size(311, 20);
			this.txtidskill.TabIndex = 8;
			this.txtnameskill.Location = new System.Drawing.Point(446, 18);
			this.txtnameskill.Name = "txtnameskill";
			this.txtnameskill.Size = new System.Drawing.Size(311, 20);
			this.txtnameskill.TabIndex = 7;
			this.txtnameskill.Text = "34";
			this.button10.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.button10.Location = new System.Drawing.Point(432, 297);
			this.button10.Name = "button10";
			this.button10.Size = new System.Drawing.Size(133, 23);
			this.button10.TabIndex = 6;
			this.button10.Text = "DoSkill NoTaget";
			this.button10.UseVisualStyleBackColor = true;
			this.button10.Click += new System.EventHandler(button10_Click);
			this.button9.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.button9.Location = new System.Drawing.Point(432, 247);
			this.button9.Name = "button9";
			this.button9.Size = new System.Drawing.Size(133, 23);
			this.button9.TabIndex = 5;
			this.button9.Text = "DoSkill Taget";
			this.button9.UseVisualStyleBackColor = true;
			this.button9.Click += new System.EventHandler(button9_Click);
			this.button8.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.button8.Location = new System.Drawing.Point(432, 122);
			this.button8.Name = "button8";
			this.button8.Size = new System.Drawing.Size(133, 23);
			this.button8.TabIndex = 4;
			this.button8.Text = "AutoTaget";
			this.button8.UseVisualStyleBackColor = true;
			this.button8.Click += new System.EventHandler(button8_Click);
			this.btnReadSkill.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnReadSkill.Location = new System.Drawing.Point(456, 435);
			this.btnReadSkill.Name = "btnReadSkill";
			this.btnReadSkill.Size = new System.Drawing.Size(75, 23);
			this.btnReadSkill.TabIndex = 3;
			this.btnReadSkill.Text = "Read";
			this.btnReadSkill.UseVisualStyleBackColor = true;
			this.btnReadSkill.Click += new System.EventHandler(btnReadSkill_Click);
			this.listViewSkill.AllowColumnReorder = true;
			this.listViewSkill.AllowDrop = true;
			this.listViewSkill.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.listViewSkill.Columns.AddRange(new System.Windows.Forms.ColumnHeader[3] { this.columnHeader3, this.columnHeader4, this.columnHeader26 });
			this.listViewSkill.FullRowSelect = true;
			this.listViewSkill.GridLines = true;
			this.listViewSkill.HideSelection = false;
			this.listViewSkill.Location = new System.Drawing.Point(6, 6);
			this.listViewSkill.Name = "listViewSkill";
			this.listViewSkill.Size = new System.Drawing.Size(371, 472);
			this.listViewSkill.TabIndex = 1;
			this.listViewSkill.UseCompatibleStateImageBehavior = false;
			this.listViewSkill.View = System.Windows.Forms.View.Details;
			this.listViewSkill.SelectedIndexChanged += new System.EventHandler(listViewSkill_SelectedIndexChanged);
			this.columnHeader3.Text = "Id";
			this.columnHeader4.Text = "Name";
			this.columnHeader4.Width = 178;
			this.tabPage3.Controls.Add(this.button6);
			this.tabPage3.Controls.Add(this.chkNotRead);
			this.tabPage3.Controls.Add(this.button3);
			this.tabPage3.Controls.Add(this.button2);
			this.tabPage3.Controls.Add(this.btnPhone);
			this.tabPage3.Controls.Add(this.txtPhone);
			this.tabPage3.Controls.Add(this.button1);
			this.tabPage3.Controls.Add(this.textBox1);
			this.tabPage3.Controls.Add(this.btnTest);
			this.tabPage3.Controls.Add(this.nudAtkCount);
			this.tabPage3.Controls.Add(this.btnAtk);
			this.tabPage3.Controls.Add(this.txtUrlAtk);
			this.tabPage3.Controls.Add(this.btnSendCaptcha);
			this.tabPage3.Controls.Add(this.btnPushDebugMessage);
			this.tabPage3.Controls.Add(this.txtPushDebugMessage);
			this.tabPage3.Controls.Add(this.btnSendPacket);
			this.tabPage3.Controls.Add(this.txtSendPacket);
			this.tabPage3.Controls.Add(this.btnResetTime);
			this.tabPage3.Controls.Add(this.btnToString);
			this.tabPage3.Controls.Add(this.btnCompress);
			this.tabPage3.Controls.Add(this.btnDoString);
			this.tabPage3.Controls.Add(this.txtDoString);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(898, 545);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "DoString";
			this.tabPage3.UseVisualStyleBackColor = true;
			this.button6.Location = new System.Drawing.Point(382, 219);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(75, 23);
			this.button6.TabIndex = 21;
			this.button6.Text = "Modules";
			this.button6.UseVisualStyleBackColor = true;
			this.button6.Click += new System.EventHandler(button6_Click);
			this.chkNotRead.AutoSize = true;
			this.chkNotRead.Location = new System.Drawing.Point(8, 461);
			this.chkNotRead.Name = "chkNotRead";
			this.chkNotRead.Size = new System.Drawing.Size(69, 17);
			this.chkNotRead.TabIndex = 20;
			this.chkNotRead.Text = "NotRead";
			this.chkNotRead.UseVisualStyleBackColor = true;
			this.chkNotRead.CheckedChanged += new System.EventHandler(checkBox1_CheckedChanged);
			this.button3.Location = new System.Drawing.Point(463, 219);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(75, 23);
			this.button3.TabIndex = 19;
			this.button3.Text = "LoadScript";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(button3_Click);
			this.button2.Location = new System.Drawing.Point(544, 219);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 18;
			this.button2.Text = "Read Recv";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(button2_Click);
			this.btnPhone.Location = new System.Drawing.Point(613, 413);
			this.btnPhone.Name = "btnPhone";
			this.btnPhone.Size = new System.Drawing.Size(167, 23);
			this.btnPhone.TabIndex = 17;
			this.btnPhone.Text = "IsPhone";
			this.btnPhone.UseVisualStyleBackColor = true;
			this.btnPhone.Click += new System.EventHandler(btnPhone_Click);
			this.txtPhone.Location = new System.Drawing.Point(7, 415);
			this.txtPhone.Name = "txtPhone";
			this.txtPhone.Size = new System.Drawing.Size(600, 20);
			this.txtPhone.TabIndex = 16;
			this.button1.Location = new System.Drawing.Point(614, 384);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(167, 23);
			this.button1.TabIndex = 15;
			this.button1.Text = "Search";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(button1_Click);
			this.textBox1.Location = new System.Drawing.Point(8, 386);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(600, 20);
			this.textBox1.TabIndex = 14;
			this.btnTest.Location = new System.Drawing.Point(625, 457);
			this.btnTest.Name = "btnTest";
			this.btnTest.Size = new System.Drawing.Size(155, 23);
			this.btnTest.TabIndex = 13;
			this.btnTest.Text = "Test";
			this.btnTest.UseVisualStyleBackColor = true;
			this.btnTest.Click += new System.EventHandler(btnTest_Click);
			this.nudAtkCount.Location = new System.Drawing.Point(540, 358);
			this.nudAtkCount.Name = "nudAtkCount";
			this.nudAtkCount.Size = new System.Drawing.Size(68, 20);
			this.nudAtkCount.TabIndex = 12;
			this.btnAtk.Location = new System.Drawing.Point(614, 355);
			this.btnAtk.Name = "btnAtk";
			this.btnAtk.Size = new System.Drawing.Size(167, 23);
			this.btnAtk.TabIndex = 11;
			this.btnAtk.Text = "Atk";
			this.btnAtk.UseVisualStyleBackColor = true;
			this.btnAtk.Click += new System.EventHandler(btnAtk_Click);
			this.txtUrlAtk.Location = new System.Drawing.Point(8, 357);
			this.txtUrlAtk.Name = "txtUrlAtk";
			this.txtUrlAtk.Size = new System.Drawing.Size(526, 20);
			this.txtUrlAtk.TabIndex = 10;
			this.btnSendCaptcha.Location = new System.Drawing.Point(452, 457);
			this.btnSendCaptcha.Name = "btnSendCaptcha";
			this.btnSendCaptcha.Size = new System.Drawing.Size(156, 23);
			this.btnSendCaptcha.TabIndex = 9;
			this.btnSendCaptcha.Text = "Send Captcha";
			this.btnSendCaptcha.UseVisualStyleBackColor = true;
			this.btnSendCaptcha.Click += new System.EventHandler(btnSendCaptcha_Click);
			this.btnPushDebugMessage.Location = new System.Drawing.Point(614, 326);
			this.btnPushDebugMessage.Name = "btnPushDebugMessage";
			this.btnPushDebugMessage.Size = new System.Drawing.Size(167, 23);
			this.btnPushDebugMessage.TabIndex = 8;
			this.btnPushDebugMessage.Text = "PushDebugMessage";
			this.btnPushDebugMessage.UseVisualStyleBackColor = true;
			this.btnPushDebugMessage.Click += new System.EventHandler(btnPushDebugMessage_Click);
			this.txtPushDebugMessage.Location = new System.Drawing.Point(8, 328);
			this.txtPushDebugMessage.Name = "txtPushDebugMessage";
			this.txtPushDebugMessage.Size = new System.Drawing.Size(600, 20);
			this.txtPushDebugMessage.TabIndex = 7;
			this.btnSendPacket.Location = new System.Drawing.Point(614, 288);
			this.btnSendPacket.Name = "btnSendPacket";
			this.btnSendPacket.Size = new System.Drawing.Size(167, 23);
			this.btnSendPacket.TabIndex = 6;
			this.btnSendPacket.Text = "Send Packet";
			this.btnSendPacket.UseVisualStyleBackColor = true;
			this.btnSendPacket.Click += new System.EventHandler(btnSendPacket_Click);
			this.txtSendPacket.Location = new System.Drawing.Point(8, 290);
			this.txtSendPacket.Name = "txtSendPacket";
			this.txtSendPacket.Size = new System.Drawing.Size(600, 20);
			this.txtSendPacket.TabIndex = 5;
			this.txtSendPacket.TextChanged += new System.EventHandler(txtSendPacket_TextChanged);
			this.btnResetTime.Location = new System.Drawing.Point(625, 248);
			this.btnResetTime.Name = "btnResetTime";
			this.btnResetTime.Size = new System.Drawing.Size(75, 23);
			this.btnResetTime.TabIndex = 4;
			this.btnResetTime.Text = "Reset Time";
			this.btnResetTime.UseVisualStyleBackColor = true;
			this.btnResetTime.Click += new System.EventHandler(btnResetTime_Click);
			this.btnToString.Location = new System.Drawing.Point(625, 219);
			this.btnToString.Name = "btnToString";
			this.btnToString.Size = new System.Drawing.Size(75, 23);
			this.btnToString.TabIndex = 3;
			this.btnToString.Text = "To String";
			this.btnToString.UseVisualStyleBackColor = true;
			this.btnToString.Click += new System.EventHandler(btnToString_Click);
			this.btnCompress.Location = new System.Drawing.Point(706, 248);
			this.btnCompress.Name = "btnCompress";
			this.btnCompress.Size = new System.Drawing.Size(75, 23);
			this.btnCompress.TabIndex = 2;
			this.btnCompress.Text = "Compress";
			this.btnCompress.UseVisualStyleBackColor = true;
			this.btnCompress.Click += new System.EventHandler(btnCompress_Click);
			this.btnDoString.Location = new System.Drawing.Point(706, 219);
			this.btnDoString.Name = "btnDoString";
			this.btnDoString.Size = new System.Drawing.Size(75, 23);
			this.btnDoString.TabIndex = 1;
			this.btnDoString.Text = "Do";
			this.btnDoString.UseVisualStyleBackColor = true;
			this.btnDoString.Click += new System.EventHandler(btnDoString_Click);
			this.txtDoString.Location = new System.Drawing.Point(8, 6);
			this.txtDoString.Multiline = true;
			this.txtDoString.Name = "txtDoString";
			this.txtDoString.Size = new System.Drawing.Size(773, 207);
			this.txtDoString.TabIndex = 0;
			this.txtDoString.KeyDown += new System.Windows.Forms.KeyEventHandler(txtDoString_KeyDown);
			this.tabPage4.Controls.Add(this.btnGameControl);
			this.tabPage4.Controls.Add(this.txtGameControl);
			this.tabPage4.Controls.Add(this.listViewGameControl);
			this.tabPage4.Location = new System.Drawing.Point(4, 22);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage4.Size = new System.Drawing.Size(898, 545);
			this.tabPage4.TabIndex = 3;
			this.tabPage4.Text = "GameControl";
			this.tabPage4.UseVisualStyleBackColor = true;
			this.btnGameControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			this.btnGameControl.Location = new System.Drawing.Point(817, 7);
			this.btnGameControl.Name = "btnGameControl";
			this.btnGameControl.Size = new System.Drawing.Size(75, 23);
			this.btnGameControl.TabIndex = 5;
			this.btnGameControl.Text = "Read";
			this.btnGameControl.UseVisualStyleBackColor = true;
			this.btnGameControl.Click += new System.EventHandler(btnGameControl_Click);
			this.txtGameControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.txtGameControl.Location = new System.Drawing.Point(385, 7);
			this.txtGameControl.Multiline = true;
			this.txtGameControl.Name = "txtGameControl";
			this.txtGameControl.Size = new System.Drawing.Size(396, 443);
			this.txtGameControl.TabIndex = 4;
			this.listViewGameControl.AllowColumnReorder = true;
			this.listViewGameControl.AllowDrop = true;
			this.listViewGameControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.listViewGameControl.Columns.AddRange(new System.Windows.Forms.ColumnHeader[3] { this.columnHeader5, this.columnHeader6, this.columnHeader7 });
			this.listViewGameControl.FullRowSelect = true;
			this.listViewGameControl.GridLines = true;
			this.listViewGameControl.HideSelection = false;
			this.listViewGameControl.Location = new System.Drawing.Point(8, 7);
			this.listViewGameControl.Name = "listViewGameControl";
			this.listViewGameControl.Size = new System.Drawing.Size(371, 472);
			this.listViewGameControl.TabIndex = 3;
			this.listViewGameControl.UseCompatibleStateImageBehavior = false;
			this.listViewGameControl.View = System.Windows.Forms.View.Details;
			this.listViewGameControl.SelectedIndexChanged += new System.EventHandler(listViewGameControl_SelectedIndexChanged);
			this.columnHeader5.Text = "Id";
			this.columnHeader6.Text = "Name";
			this.columnHeader6.Width = 194;
			this.columnHeader7.Text = "PacketId";
			this.tabPage5.Controls.Add(this.txtTask);
			this.tabPage5.Controls.Add(this.btnReadTask);
			this.tabPage5.Controls.Add(this.listViewTask);
			this.tabPage5.Location = new System.Drawing.Point(4, 22);
			this.tabPage5.Name = "tabPage5";
			this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage5.Size = new System.Drawing.Size(898, 545);
			this.tabPage5.TabIndex = 4;
			this.tabPage5.Text = "Task";
			this.tabPage5.UseVisualStyleBackColor = true;
			this.txtTask.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.txtTask.Location = new System.Drawing.Point(286, 35);
			this.txtTask.Multiline = true;
			this.txtTask.Name = "txtTask";
			this.txtTask.Size = new System.Drawing.Size(495, 445);
			this.txtTask.TabIndex = 10;
			this.btnReadTask.Location = new System.Drawing.Point(8, 6);
			this.btnReadTask.Name = "btnReadTask";
			this.btnReadTask.Size = new System.Drawing.Size(75, 23);
			this.btnReadTask.TabIndex = 9;
			this.btnReadTask.Text = "Read";
			this.btnReadTask.UseVisualStyleBackColor = true;
			this.btnReadTask.Click += new System.EventHandler(btnReadTask_Click);
			this.listViewTask.AllowColumnReorder = true;
			this.listViewTask.AllowDrop = true;
			this.listViewTask.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.listViewTask.Columns.AddRange(new System.Windows.Forms.ColumnHeader[2] { this.columnHeader13, this.columnHeader14 });
			this.listViewTask.FullRowSelect = true;
			this.listViewTask.GridLines = true;
			this.listViewTask.HideSelection = false;
			this.listViewTask.Location = new System.Drawing.Point(8, 35);
			this.listViewTask.Name = "listViewTask";
			this.listViewTask.Size = new System.Drawing.Size(272, 445);
			this.listViewTask.TabIndex = 8;
			this.listViewTask.UseCompatibleStateImageBehavior = false;
			this.listViewTask.View = System.Windows.Forms.View.Details;
			this.listViewTask.SelectedIndexChanged += new System.EventHandler(listViewTask_SelectedIndexChanged);
			this.columnHeader13.Text = "Id";
			this.columnHeader13.Width = 57;
			this.columnHeader14.Text = "Name";
			this.columnHeader14.Width = 170;
			this.tabPage6.Controls.Add(this.button16);
			this.tabPage6.Controls.Add(this.txtDialog);
			this.tabPage6.Controls.Add(this.btnReadDialog);
			this.tabPage6.Controls.Add(this.listViewDialog);
			this.tabPage6.Location = new System.Drawing.Point(4, 22);
			this.tabPage6.Name = "tabPage6";
			this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage6.Size = new System.Drawing.Size(898, 545);
			this.tabPage6.TabIndex = 5;
			this.tabPage6.Text = "Dialog";
			this.tabPage6.UseVisualStyleBackColor = true;
			this.button16.Location = new System.Drawing.Point(118, 6);
			this.button16.Name = "button16";
			this.button16.Size = new System.Drawing.Size(75, 23);
			this.button16.TabIndex = 11;
			this.button16.Text = "GIAMDINH";
			this.button16.UseVisualStyleBackColor = true;
			this.button16.Click += new System.EventHandler(button16_Click);
			this.txtDialog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.txtDialog.Location = new System.Drawing.Point(273, 32);
			this.txtDialog.Multiline = true;
			this.txtDialog.Name = "txtDialog";
			this.txtDialog.Size = new System.Drawing.Size(508, 446);
			this.txtDialog.TabIndex = 10;
			this.btnReadDialog.Location = new System.Drawing.Point(8, 3);
			this.btnReadDialog.Name = "btnReadDialog";
			this.btnReadDialog.Size = new System.Drawing.Size(75, 23);
			this.btnReadDialog.TabIndex = 9;
			this.btnReadDialog.Text = "Read";
			this.btnReadDialog.UseVisualStyleBackColor = true;
			this.btnReadDialog.Click += new System.EventHandler(btnReadDialog_Click);
			this.listViewDialog.AllowColumnReorder = true;
			this.listViewDialog.AllowDrop = true;
			this.listViewDialog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.listViewDialog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[2] { this.columnHeader15, this.columnHeader16 });
			this.listViewDialog.FullRowSelect = true;
			this.listViewDialog.GridLines = true;
			this.listViewDialog.HideSelection = false;
			this.listViewDialog.Location = new System.Drawing.Point(8, 32);
			this.listViewDialog.Name = "listViewDialog";
			this.listViewDialog.Size = new System.Drawing.Size(259, 446);
			this.listViewDialog.TabIndex = 8;
			this.listViewDialog.UseCompatibleStateImageBehavior = false;
			this.listViewDialog.View = System.Windows.Forms.View.Details;
			this.listViewDialog.SelectedIndexChanged += new System.EventHandler(listViewDialog_SelectedIndexChanged);
			this.columnHeader15.Text = "Id";
			this.columnHeader15.Width = 57;
			this.columnHeader16.Text = "Name";
			this.columnHeader16.Width = 170;
			this.tabPage7.Controls.Add(this.button17);
			this.tabPage7.Controls.Add(this.button7);
			this.tabPage7.Controls.Add(this.numericUpDown1);
			this.tabPage7.Controls.Add(this.button5);
			this.tabPage7.Controls.Add(this.txtPacket);
			this.tabPage7.Controls.Add(this.btnReadPacket);
			this.tabPage7.Controls.Add(this.listViewPacket);
			this.tabPage7.Location = new System.Drawing.Point(4, 22);
			this.tabPage7.Name = "tabPage7";
			this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage7.Size = new System.Drawing.Size(898, 545);
			this.tabPage7.TabIndex = 6;
			this.tabPage7.Text = "Packet";
			this.tabPage7.UseVisualStyleBackColor = true;
			this.button17.Location = new System.Drawing.Point(287, 7);
			this.button17.Name = "button17";
			this.button17.Size = new System.Drawing.Size(75, 23);
			this.button17.TabIndex = 11;
			this.button17.Text = "button17";
			this.button17.UseVisualStyleBackColor = true;
			this.button17.Click += new System.EventHandler(button17_Click);
			this.button7.Location = new System.Drawing.Point(170, 6);
			this.button7.Name = "button7";
			this.button7.Size = new System.Drawing.Size(75, 23);
			this.button7.TabIndex = 10;
			this.button7.Text = "Bank";
			this.button7.UseVisualStyleBackColor = true;
			this.button7.Click += new System.EventHandler(button7_Click);
			this.numericUpDown1.Location = new System.Drawing.Point(809, 179);
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(67, 20);
			this.numericUpDown1.TabIndex = 9;
			this.numericUpDown1.ValueChanged += new System.EventHandler(numericUpDown1_ValueChanged);
			this.button5.Location = new System.Drawing.Point(89, 6);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(75, 23);
			this.button5.TabIndex = 8;
			this.button5.Text = "TrangBi";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(button5_Click);
			this.txtPacket.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.txtPacket.Location = new System.Drawing.Point(273, 35);
			this.txtPacket.Multiline = true;
			this.txtPacket.Name = "txtPacket";
			this.txtPacket.Size = new System.Drawing.Size(508, 443);
			this.txtPacket.TabIndex = 7;
			this.btnReadPacket.Location = new System.Drawing.Point(8, 6);
			this.btnReadPacket.Name = "btnReadPacket";
			this.btnReadPacket.Size = new System.Drawing.Size(75, 23);
			this.btnReadPacket.TabIndex = 6;
			this.btnReadPacket.Text = "Read";
			this.btnReadPacket.UseVisualStyleBackColor = true;
			this.btnReadPacket.Click += new System.EventHandler(btnReadPacket_Click);
			this.listViewPacket.AllowColumnReorder = true;
			this.listViewPacket.AllowDrop = true;
			this.listViewPacket.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.listViewPacket.Columns.AddRange(new System.Windows.Forms.ColumnHeader[2] { this.columnHeader8, this.columnHeader9 });
			this.listViewPacket.FullRowSelect = true;
			this.listViewPacket.GridLines = true;
			this.listViewPacket.HideSelection = false;
			this.listViewPacket.Location = new System.Drawing.Point(8, 35);
			this.listViewPacket.Name = "listViewPacket";
			this.listViewPacket.Size = new System.Drawing.Size(259, 443);
			this.listViewPacket.TabIndex = 5;
			this.listViewPacket.UseCompatibleStateImageBehavior = false;
			this.listViewPacket.View = System.Windows.Forms.View.Details;
			this.listViewPacket.SelectedIndexChanged += new System.EventHandler(listViewPacket_SelectedIndexChanged);
			this.columnHeader8.Text = "Id";
			this.columnHeader8.Width = 57;
			this.columnHeader9.Text = "Name";
			this.columnHeader9.Width = 170;
			this.tabPage8.Location = new System.Drawing.Point(4, 22);
			this.tabPage8.Name = "tabPage8";
			this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage8.Size = new System.Drawing.Size(898, 545);
			this.tabPage8.TabIndex = 7;
			this.tabPage8.Text = "Converter";
			this.tabPage8.UseVisualStyleBackColor = true;
			this.tabPage9.Controls.Add(this.txtShop);
			this.tabPage9.Controls.Add(this.btnReadShop);
			this.tabPage9.Controls.Add(this.listViewShop);
			this.tabPage9.Location = new System.Drawing.Point(4, 22);
			this.tabPage9.Name = "tabPage9";
			this.tabPage9.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage9.Size = new System.Drawing.Size(898, 545);
			this.tabPage9.TabIndex = 8;
			this.tabPage9.Text = "Shop";
			this.tabPage9.UseVisualStyleBackColor = true;
			this.txtShop.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.txtShop.Location = new System.Drawing.Point(407, 36);
			this.txtShop.Multiline = true;
			this.txtShop.Name = "txtShop";
			this.txtShop.Size = new System.Drawing.Size(374, 443);
			this.txtShop.TabIndex = 10;
			this.btnReadShop.Location = new System.Drawing.Point(8, 7);
			this.btnReadShop.Name = "btnReadShop";
			this.btnReadShop.Size = new System.Drawing.Size(75, 23);
			this.btnReadShop.TabIndex = 9;
			this.btnReadShop.Text = "Read";
			this.btnReadShop.UseVisualStyleBackColor = true;
			this.btnReadShop.Click += new System.EventHandler(btnReadShop_Click);
			this.listViewShop.AllowColumnReorder = true;
			this.listViewShop.AllowDrop = true;
			this.listViewShop.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.listViewShop.Columns.AddRange(new System.Windows.Forms.ColumnHeader[3] { this.columnHeader10, this.columnHeader11, this.columnHeader12 });
			this.listViewShop.FullRowSelect = true;
			this.listViewShop.GridLines = true;
			this.listViewShop.HideSelection = false;
			this.listViewShop.Location = new System.Drawing.Point(8, 36);
			this.listViewShop.Name = "listViewShop";
			this.listViewShop.Size = new System.Drawing.Size(393, 443);
			this.listViewShop.TabIndex = 8;
			this.listViewShop.UseCompatibleStateImageBehavior = false;
			this.listViewShop.View = System.Windows.Forms.View.Details;
			this.listViewShop.SelectedIndexChanged += new System.EventHandler(listViewShop_SelectedIndexChanged);
			this.listViewShop.DoubleClick += new System.EventHandler(listViewShop_DoubleClick);
			this.columnHeader10.Text = "Id";
			this.columnHeader10.Width = 57;
			this.columnHeader11.Text = "Name";
			this.columnHeader11.Width = 170;
			this.columnHeader12.Text = "Class";
			this.columnHeader12.Width = 95;
			this.tabPage10.Controls.Add(this.txtTLBB);
			this.tabPage10.Location = new System.Drawing.Point(4, 22);
			this.tabPage10.Name = "tabPage10";
			this.tabPage10.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage10.Size = new System.Drawing.Size(898, 545);
			this.tabPage10.TabIndex = 9;
			this.tabPage10.Text = "TLBB";
			this.tabPage10.UseVisualStyleBackColor = true;
			this.txtTLBB.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtTLBB.Location = new System.Drawing.Point(3, 3);
			this.txtTLBB.Multiline = true;
			this.txtTLBB.Name = "txtTLBB";
			this.txtTLBB.Size = new System.Drawing.Size(892, 539);
			this.txtTLBB.TabIndex = 11;
			this.tabPage11.Controls.Add(this.splitContainer1);
			this.tabPage11.Location = new System.Drawing.Point(4, 22);
			this.tabPage11.Name = "tabPage11";
			this.tabPage11.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage11.Size = new System.Drawing.Size(898, 545);
			this.tabPage11.TabIndex = 10;
			this.tabPage11.Text = "Convert";
			this.tabPage11.UseVisualStyleBackColor = true;
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(3, 3);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.splitContainer1.Panel1.Controls.Add(this.txtCode);
			this.splitContainer1.Panel2.Controls.Add(this.txtNewCode);
			this.splitContainer1.Size = new System.Drawing.Size(892, 539);
			this.splitContainer1.SplitterDistance = 257;
			this.splitContainer1.TabIndex = 1;
			this.txtCode.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtCode.Location = new System.Drawing.Point(0, 0);
			this.txtCode.Multiline = true;
			this.txtCode.Name = "txtCode";
			this.txtCode.Size = new System.Drawing.Size(892, 257);
			this.txtCode.TabIndex = 0;
			this.txtCode.TextChanged += new System.EventHandler(txtCode_TextChanged);
			this.txtCode.KeyDown += new System.Windows.Forms.KeyEventHandler(txtCode_KeyDown);
			this.txtNewCode.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtNewCode.Location = new System.Drawing.Point(0, 0);
			this.txtNewCode.Multiline = true;
			this.txtNewCode.Name = "txtNewCode";
			this.txtNewCode.Size = new System.Drawing.Size(892, 278);
			this.txtNewCode.TabIndex = 1;
			this.txtNewCode.KeyDown += new System.Windows.Forms.KeyEventHandler(txtNewCode_KeyDown);
			this.tabPage12.Controls.Add(this.splitContainer2);
			this.tabPage12.Location = new System.Drawing.Point(4, 22);
			this.tabPage12.Name = "tabPage12";
			this.tabPage12.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage12.Size = new System.Drawing.Size(898, 545);
			this.tabPage12.TabIndex = 11;
			this.tabPage12.Text = "Unicode2VISCII";
			this.tabPage12.UseVisualStyleBackColor = true;
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(3, 3);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.splitContainer2.Panel1.Controls.Add(this.button4);
			this.splitContainer2.Panel1.Controls.Add(this.btnrRemoveHex);
			this.splitContainer2.Panel1.Controls.Add(this.txtUnicode);
			this.splitContainer2.Panel2.Controls.Add(this.txtVISCII);
			this.splitContainer2.Size = new System.Drawing.Size(892, 539);
			this.splitContainer2.SplitterDistance = 257;
			this.splitContainer2.TabIndex = 2;
			this.button4.Location = new System.Drawing.Point(623, 229);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(129, 23);
			this.button4.TabIndex = 2;
			this.button4.Text = "Unicode2VSICII";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(button4_Click);
			this.btnrRemoveHex.Location = new System.Drawing.Point(758, 229);
			this.btnrRemoveHex.Name = "btnrRemoveHex";
			this.btnrRemoveHex.Size = new System.Drawing.Size(129, 23);
			this.btnrRemoveHex.TabIndex = 1;
			this.btnrRemoveHex.Text = "Remove Hex Character";
			this.btnrRemoveHex.UseVisualStyleBackColor = true;
			this.btnrRemoveHex.Click += new System.EventHandler(btnrRemoveHex_Click);
			this.txtUnicode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.txtUnicode.Location = new System.Drawing.Point(0, 0);
			this.txtUnicode.MaxLength = 9999999;
			this.txtUnicode.Multiline = true;
			this.txtUnicode.Name = "txtUnicode";
			this.txtUnicode.Size = new System.Drawing.Size(892, 223);
			this.txtUnicode.TabIndex = 0;
			this.txtUnicode.TextChanged += new System.EventHandler(txtUnicode_TextChanged);
			this.txtUnicode.KeyDown += new System.Windows.Forms.KeyEventHandler(txtUnicode_KeyDown);
			this.txtVISCII.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtVISCII.Location = new System.Drawing.Point(0, 0);
			this.txtVISCII.MaxLength = 9999999;
			this.txtVISCII.Multiline = true;
			this.txtVISCII.Name = "txtVISCII";
			this.txtVISCII.Size = new System.Drawing.Size(892, 278);
			this.txtVISCII.TabIndex = 1;
			this.txtVISCII.TextChanged += new System.EventHandler(txtVISCII_TextChanged);
			this.txtVISCII.KeyDown += new System.Windows.Forms.KeyEventHandler(txtVISCII_KeyDown);
			this.tabPage13.Controls.Add(this.splitContainer3);
			this.tabPage13.Location = new System.Drawing.Point(4, 22);
			this.tabPage13.Name = "tabPage13";
			this.tabPage13.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage13.Size = new System.Drawing.Size(898, 545);
			this.tabPage13.TabIndex = 12;
			this.tabPage13.Text = "Script";
			this.tabPage13.UseVisualStyleBackColor = true;
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.Location = new System.Drawing.Point(3, 3);
			this.splitContainer3.Name = "splitContainer3";
			this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.splitContainer3.Panel1.Controls.Add(this.listViewScript);
			this.splitContainer3.Size = new System.Drawing.Size(892, 539);
			this.splitContainer3.SplitterDistance = 232;
			this.splitContainer3.TabIndex = 46;
			this.listViewScript.AllowColumnReorder = true;
			this.listViewScript.AllowDrop = true;
			this.listViewScript.Columns.AddRange(new System.Windows.Forms.ColumnHeader[8] { this.columnHeader18, this.columnHeader19, this.columnHeader20, this.columnHeader21, this.columnHeader22, this.columnHeader23, this.columnHeader24, this.columnHeader25 });
			this.listViewScript.ContextMenuStrip = this.menuScript;
			this.listViewScript.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewScript.FullRowSelect = true;
			this.listViewScript.GridLines = true;
			this.listViewScript.HideSelection = false;
			this.listViewScript.Location = new System.Drawing.Point(0, 0);
			this.listViewScript.Name = "listViewScript";
			this.listViewScript.Size = new System.Drawing.Size(892, 232);
			this.listViewScript.TabIndex = 45;
			this.listViewScript.UseCompatibleStateImageBehavior = false;
			this.listViewScript.View = System.Windows.Forms.View.Details;
			this.listViewScript.SelectedIndexChanged += new System.EventHandler(listViewScript_SelectedIndexChanged);
			this.listViewScript.DragDrop += new System.Windows.Forms.DragEventHandler(listViewScript_DragDrop);
			this.columnHeader18.Text = "ID";
			this.columnHeader18.Width = 69;
			this.columnHeader19.Text = "MD";
			this.columnHeader19.Width = 80;
			this.columnHeader20.Text = "Recv";
			this.columnHeader20.Width = 70;
			this.columnHeader21.Text = "Send";
			this.columnHeader21.Width = 71;
			this.columnHeader22.Text = "Do";
			this.columnHeader22.Width = 96;
			this.columnHeader23.Text = "Info";
			this.columnHeader23.Width = 118;
			this.columnHeader24.Text = "Name";
			this.columnHeader24.Width = 166;
			this.columnHeader25.Text = "Level";
			this.menuScript.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.menuAddScript, this.menuEditScript, this.menuDeleteScript, this.refreshToolStripMenuItem });
			this.menuScript.Name = "menuScript";
			this.menuScript.Size = new System.Drawing.Size(114, 92);
			this.menuAddScript.Name = "menuAddScript";
			this.menuAddScript.Size = new System.Drawing.Size(113, 22);
			this.menuAddScript.Text = "Thêm";
			this.menuEditScript.Name = "menuEditScript";
			this.menuEditScript.Size = new System.Drawing.Size(113, 22);
			this.menuEditScript.Text = "Sửa";
			this.menuEditScript.Click += new System.EventHandler(menuEditScript_Click);
			this.menuDeleteScript.Name = "menuDeleteScript";
			this.menuDeleteScript.Size = new System.Drawing.Size(113, 22);
			this.menuDeleteScript.Text = "Xóa";
			this.menuDeleteScript.Click += new System.EventHandler(menuDeleteScript_Click);
			this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
			this.refreshToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
			this.refreshToolStripMenuItem.Text = "Refresh";
			this.refreshToolStripMenuItem.Click += new System.EventHandler(refreshToolStripMenuItem_Click);
			this.tabPage14.Controls.Add(this.probGame);
			this.tabPage14.Location = new System.Drawing.Point(4, 22);
			this.tabPage14.Name = "tabPage14";
			this.tabPage14.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage14.Size = new System.Drawing.Size(898, 545);
			this.tabPage14.TabIndex = 13;
			this.tabPage14.Text = "Game";
			this.tabPage14.UseVisualStyleBackColor = true;
			this.probGame.Dock = System.Windows.Forms.DockStyle.Fill;
			this.probGame.LineColor = System.Drawing.SystemColors.ControlDark;
			this.probGame.Location = new System.Drawing.Point(3, 3);
			this.probGame.Name = "probGame";
			this.probGame.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
			this.probGame.Size = new System.Drawing.Size(892, 539);
			this.probGame.TabIndex = 0;
			this.tabPage15.Controls.Add(this.pgTLBB);
			this.tabPage15.Location = new System.Drawing.Point(4, 22);
			this.tabPage15.Name = "tabPage15";
			this.tabPage15.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage15.Size = new System.Drawing.Size(898, 545);
			this.tabPage15.TabIndex = 14;
			this.tabPage15.Text = "tabPage15";
			this.tabPage15.UseVisualStyleBackColor = true;
			this.pgTLBB.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgTLBB.LineColor = System.Drawing.SystemColors.ControlDark;
			this.pgTLBB.Location = new System.Drawing.Point(3, 3);
			this.pgTLBB.Name = "pgTLBB";
			this.pgTLBB.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
			this.pgTLBB.Size = new System.Drawing.Size(892, 539);
			this.pgTLBB.TabIndex = 1;
			this.tabPage16.Controls.Add(this.button13);
			this.tabPage16.Location = new System.Drawing.Point(4, 22);
			this.tabPage16.Name = "tabPage16";
			this.tabPage16.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage16.Size = new System.Drawing.Size(898, 545);
			this.tabPage16.TabIndex = 15;
			this.tabPage16.Text = "tabPage16";
			this.tabPage16.UseVisualStyleBackColor = true;
			this.button13.Location = new System.Drawing.Point(182, 322);
			this.button13.Name = "button13";
			this.button13.Size = new System.Drawing.Size(75, 23);
			this.button13.TabIndex = 0;
			this.button13.Text = "button13";
			this.button13.UseVisualStyleBackColor = true;
			this.button13.Click += new System.EventHandler(button13_Click);
			this.tmrRefresh.Enabled = true;
			this.tmrRefresh.Tick += new System.EventHandler(tmrRefresh_Tick);
			this.tmrTest.Enabled = true;
			this.tmrTest.Interval = 150;
			this.tmrTest.Tick += new System.EventHandler(tmrTest_Tick);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(906, 571);
			base.Controls.Add(this.tabControl1);
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.Name = "Debug";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Debug";
			base.Load += new System.EventHandler(Debug_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.splitContainer4.Panel1.ResumeLayout(false);
			this.splitContainer4.Panel2.ResumeLayout(false);
			this.splitContainer4.ResumeLayout(false);
			this.menuGameObject.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.nudAtkCount).EndInit();
			this.tabPage4.ResumeLayout(false);
			this.tabPage4.PerformLayout();
			this.tabPage5.ResumeLayout(false);
			this.tabPage5.PerformLayout();
			this.tabPage6.ResumeLayout(false);
			this.tabPage6.PerformLayout();
			this.tabPage7.ResumeLayout(false);
			this.tabPage7.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.numericUpDown1).EndInit();
			this.tabPage9.ResumeLayout(false);
			this.tabPage9.PerformLayout();
			this.tabPage10.ResumeLayout(false);
			this.tabPage10.PerformLayout();
			this.tabPage11.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			this.splitContainer1.ResumeLayout(false);
			this.tabPage12.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel1.PerformLayout();
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.Panel2.PerformLayout();
			this.splitContainer2.ResumeLayout(false);
			this.tabPage13.ResumeLayout(false);
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.ResumeLayout(false);
			this.menuScript.ResumeLayout(false);
			this.tabPage14.ResumeLayout(false);
			this.tabPage15.ResumeLayout(false);
			this.tabPage16.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
