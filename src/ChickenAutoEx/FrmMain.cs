using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Threading.Tasks;
using ChickenAutoEx.Startup;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using TinhKiemAuto.AutoControl;
using TinhKiemAuto.Models;
using TinhKiemAuto.Properties;

namespace TinhKiemAuto
{
	public class FrmMain : Form
	{
		public delegate void LogBack(string log);

		private delegate int ChangeWindowMessageFilterDelegate(uint msg, int flag);

		public delegate void CallBack(Game game);

		public class ComboboxItem
		{
			public string Text { get; set; }

			public object Value { get; set; }

			public override string ToString()
			{
				return Text;
			}
		}

		private struct COPYDATASTRUCT
		{
			public IntPtr dwData;

			public int cbData;

			public IntPtr lpData;
		}

		public static int MaxHoaX = 21;

		public static RichTextBox TxtLog;

		public static DateTime startprogram = DateTime.Now;

		public static bool TrimRam = false;

		public static Dictionary<int, Game> dicGame = new Dictionary<int, Game>();

		public static int DiemDanhIndex = -1;

		public static string AllCurGameTrueID = string.Empty;

		public static ListView ListView;

		public static ListViewItem CurItem;

		private static ChangeWindowMessageFilterDelegate ChangeWindowMessageFilter;

		public static List<Account> ListAutoLogin = new List<Account>();

		public static bool IsLoged = false;

		private Thread ThreadMonitor;

		private Stopwatch timeAuto = Stopwatch.StartNew();

		public Memory Memory;

		public static int MaxGame = 0;

		public static Game CurGame;

		public static Dictionary<string, string> Captchas = new Dictionary<string, string>();

		private bool IsPop = true;

		private bool Running;

		public static Dictionary<string, string> Answers = new Dictionary<string, string>();

		public bool IsLoginTab;

		public bool IsCheDoTab;

		private IContainer components;

		private Panel paneltop;

		private Panel panel1;

		private Panel panellistview;

		private ListView ListViewNhanVat;

		private ColumnHeader txtNhanVat;

		private TabControl TablControl;

		private TabPage tabtophop;

		private TabPage tabkynang;

		private TabPage tabvatpham;

		private TabPage tabPage2;

		private TabPage tabtienich;

		private TabPage tabchedo;

		private TabPage tabautologin;

		private MenuStrip menuStrip1;

		private ToolStripMenuItem tùyChọnToolStripMenuItem;

		private ToolStripMenuItem phímTắtToolStripMenuItem;

		private ToolStripMenuItem càiĐườngDẫnGameToolStripMenuItem;

		private Label label1;

		private Label txthp;

		private Label txtmp;

		private Label label3;

		private Label txtpet;

		private Label label4;

		private ToolStripMenuItem chươngTrìnhToolStripMenuItem;

		private ToolStripMenuItem menuactac;

		private ToolStripMenuItem ItemAcBa;

		private Panel panel2;

		private Panel panel3;

		private StatusStrip statusStrip1;

		private ToolStripStatusLabel txttrangthai;

		private Label label7;

		private Label label6;

		private Label label5;

		private Label label2;

		private ToolTip toolTip1;

		private PictureBox butpickall;

		private PictureBox unpickall;

		private PictureBox pictureBox1;

		private PictureBox pictureBox2;

		private PictureBox pictureBox3;

		private PictureBox buttheo;

		private PictureBox pictureBox5;

		private PictureBox pictureBox6;

		private Button buttrieutap;

		private ToolStripMenuItem ẩnAutoToolStripMenuItem;

		private Button button1;

		private GroupBox groupBox1;

		private CheckBox chekcdanhquai;

		private PictureBox butboqua;

		private CheckBox checkradius;

		private NumericUpDown numberdanhquanh;

		private RadioButton radgom;

		private RadioButton rad11;

		private GroupBox groupBox2;

		private ComboBox comdanhsachbando;

		private Label label9;

		private Button butlenbai;

		private Label label10;

		private ComboBox comlenbai;

		private Label label12;

		private Label label11;

		private TextBox txttoadoy;

		private TextBox txttoadox;

		private Button buttrilieu;

		private Button buttimbai;

		private CheckBox checktholinhchau;

		private GroupBox groupBox3;

		private NumericUpDown nudHP;

		private CheckBox checkregenhp;

		private NumericUpDown nudMP;

		private CheckBox checkrengenmp;

		private NumericUpDown numbercongsinhhp;

		private CheckBox checkcongsinh;

		private Label label15;

		private Label label13;

		private Label label14;

		private Label label16;

		private NumericUpDown numhuyettemp;

		private CheckBox checkhuyette;

		private Label label17;

		private NumericUpDown nudNM;

		private CheckBox checkisNM;

		private CheckBox CheckTriLieuComeback;

		private CheckBox CheckAutoHoiSinh;

		private Label label18;

		private NumericUpDown numcanhbaohp;

		private CheckBox checkBox7;

		private RichTextBox txtlogs;

		private TabControl TabKyNangControl;

		private TabPage tabdanhquai;

		private TabPage tabbufffhotro;

		private ListView listViewSkill;

		private Label label19;

		private ComboBox comdanhsachdanhquai;

		private PictureBox butthemskilldanhquai;

		private Label label20;

		private Label label21;

		private PictureBox pictureBox10;

		private Label label22;

		private ComboBox comskillhotro;

		private ListView listviewskillhotro;

		private GroupBox groupBox4;

		private CheckBox checkpickitem;

		private CheckBox checkhuyitem;

		private PictureBox butdanhsachhuy;

		private PictureBox butbanvatpham;

		private CheckBox pickbanvatpham;

		private CheckBox checkautocatkho;

		private CheckBox checkautovutrac;

		private GroupBox groupBox5;

		private Label label23;

		private NumericUpDown numrangerpickitem;

		private CheckBox chekcautox2;

		private PictureBox butitemtuanhoan;

		private CheckBox chekcusingitem;

		private Label label25;

		private Label label24;

		private Label label26;

		private GroupBox groupBox6;

		private CheckBox AutoXuatPhet;

		private ComboBox cboXuatPet;

		private Button button4;

		private CheckBox checkBox9;

		private CheckBox checkBox10;

		private CheckBox CheckthuPet;

		private NumericUpDown numericUpDown3;

		private CheckBox CheckReGenPET;

		private GroupBox groupBox7;

		private PictureBox butdanhsachdongy;

		private CheckBox checkdongytodoi;

		private CheckBox checkdongytoanbo;

		private CheckBox checkautoskillf1;

		private CheckBox checkauouplevel;

		private NumericUpDown numuplevel;

		private GroupBox groupBox8;

		private CheckBox checkgiaochat;

		private RichTextBox txtnoidunggiaochat;

		private Button button5;

		private PictureBox pictureBox11;

		private Label label27;

		private TextBox txtthoigian;

		private Label label28;

		private CheckBox Checkthongbaochatmat;

		private GroupBox groupBox9;

		private NumericUpDown numbankinhtheosau;

		private Label label29;

		private Button button6;

		private TextBox txtmkkho;

		private Label label30;

		private PictureBox pictureBox12;

		private Label label31;

		private GroupBox groupBox10;

		private CheckBox checkBox12;

		private Label label33;

		private NumericUpDown numsonguyenlieu;

		private Label label32;

		private Label label34;

		private NumericUpDown numericUpDown4;

		private Label label35;

		private GroupBox groupBox11;

		private ComboBox comboloai;

		private Label label36;

		private ComboBox comcapdtd;

		private Label label37;

		private GroupBox groupBox12;

		private RadioButton radngoai;

		private RadioButton radnoingoai;

		private RadioButton Radnoi;

		private CheckBox checkBox13;

		private ComboBox comboBox3;

		private Label label38;

		private GroupBox groupBox13;

		private NumericUpDown numsosao;

		private Label label39;

		private Label label44;

		private Label label47;

		private Label label46;

		private Label label45;

		private Label label43;

		private Label label42;

		private NumericUpDown numericUpDown6;

		private Label label41;

		private NumericUpDown numericUpDown5;

		private Label label40;

		private GroupBox groupBox14;

		private Label txttinhthiet;

		private Label label48;

		private Label txttaodo;

		private Label label52;

		private Label txtbingan;

		private Label label51;

		private Label txtvaibong;

		private Label label50;

		private Label txtdahuy;

		private Label label54;

		private Label txtdache;

		private Label label53;

		private Label label49;

		private Label label55;

		private TextBox txtmk;

		private TextBox txttk;

		private Label label56;

		private Label label57;

		private ComboBox ComMayChu;

		private ListView ListViewLogin;

		private Button button7;

		private ColumnHeader tennhanvat;

		private ColumnHeader maychu;

		private ColumnHeader trangthai;

		private ColumnHeader nhanvat;

		private ColumnHeader phai;

		private System.Windows.Forms.Timer timeMonitor;

		private NotifyIcon notifyIcon1;

		private System.Windows.Forms.Timer tmrLogin;

		private ColumnHeader lbltenkynang;

		private ColumnHeader tenkynang;

		private BackgroundWorker AccountLogin;

		private Label label58;

		private Label label59;

		private System.Windows.Forms.Timer tmrRefresh;

		private ToolTip toolTip2;

		private ToolStripMenuItem ItemLauLan;

		private Button butche;

		private System.Windows.Forms.Timer CheDoF5;

		private ToolStripMenuItem mởThêmGameToolStripMenuItem;

		private ToolStripMenuItem tựĐộngToolStripMenuItem;

		private ToolStripMenuItem vôLượngSơnToolStripMenuItem;

		private ToolStripMenuItem kínhHồToolStripMenuItem;

		private ToolStripMenuItem kiếmCácToolStripMenuItem;

		private ToolStripMenuItem tháiHồToolStripMenuItem;

		private ToolStripMenuItem tungSơnToolStripMenuItem;

		private ToolStripMenuItem đônHoàngToolStripMenuItem;

		private ToolStripMenuItem itemTranLongKyCuoc;

		private ToolStripMenuItem ItemThuyLao;

		private ToolStripMenuItem ItemTrungAc;

		private ToolStripMenuItem itemchuacodoi;

		private ToolStripMenuItem dUwngfToolStripMenuItem;

		private ContextMenuStrip contextMenuStrip1;

		private ToolStripMenuItem itemresetauto;

		private ToolStripMenuItem ẩnGameToolStripMenuItem;

		private ToolStripMenuItem hiệnGameToolStripMenuItem;

		private ToolStripMenuItem thiếtLậpAutoToolStripMenuItem;

		private ToolStripMenuItem thôngTinCậpNhậtToolStripMenuItem;

		private System.Windows.Forms.Timer ChacterReport;

		private BackgroundWorker ServerConnect;

		private ToolStripMenuItem thôngTinAUTOToolStripMenuItem;

		public SocketClient _SocketClient { get; set; }

		public static string AllName
		{
			get
			{
				string text = "";
				foreach (KeyValuePair<int, Game> item in dicGame)
				{
					Game value = item.Value;
					if (value.TLBB.Online)
					{
						text = text + value.TLBB.Name + ",";
					}
				}
				return text;
			}
		}

		public static bool AlarmAcBa { get; set; }

		public static bool IsCalender { get; set; }

		public static FrmMain Instance { get; set; }

		public static bool IsFixed { get; set; }

		public static int GameCount { get; set; }

		public static bool IsStop { get; set; }

		private List<Game> AllGame
		{
			get
			{
				List<Game> list = new List<Game>();
				foreach (KeyValuePair<int, Game> item in dicGame)
				{
					list.Add(item.Value);
				}
				return list;
			}
		}

		public static bool IsExit { get; set; }

		private bool Follow { get; set; }

		private Dictionary<int, Stopwatch> FakeGame { get; set; }

		public static bool IsDangCho { get; set; }

		public int LoginCount { get; set; }

		public static int TotalGame => dicGame.Count;

		private bool IsWait { get; set; }

		public static Game Leader
		{
			get
			{
				if (CurGame == null)
				{
					return null;
				}
				if (CurGame.Leader == null)
				{
					return null;
				}
				return CurGame.Leader;
			}
		}

		private List<Game> SelectedGames
		{
			get
			{
				List<Game> list = new List<Game>();
				IEnumerator enumerator = ListViewNhanVat.SelectedItems.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Game item = ((ListViewItem)enumerator.Current).Tag as Game;
					list.Add(item);
				}
				if (list.Count == 0)
				{
					IEnumerator enumerator2 = ListViewNhanVat.Items.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						Game item2 = ((ListViewItem)enumerator2.Current).Tag as Game;
						list.Add(item2);
					}
				}
				return list;
			}
		}

		[CompilerGenerated]
		public event EventHandler AccChanged;

		public FrmMain()
		{
			InitializeComponent();
			InitializeAcBaMenu();
			Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;
			TxtLog = txtlogs;
			ListView = ListViewNhanVat;
			Instance = this;
		}

		public bool VuaBatXong()
		{
			if (GETTIMEHIENTAI(DateTime.Now) - GETTIMEHIENTAI(startprogram) < 180)
			{
				return true;
			}
			return false;
		}

		public static void AddLog(object log)
		{
			if (Instance.InvokeRequired)
			{
				Instance.Invoke(new LogBack(AddLog), log);
			}
			else
			{
				Instance.txtlogs.AppendText(log.ToString() + "\n");
			}
		}

		public Color GetCodeByPercen(int Percen)
		{
			Color lavender = Color.Lavender;
			if (Percen >= 60)
			{
				return Color.LawnGreen;
			}
			if (Percen < 60 && Percen >= 30)
			{
				return Color.YellowGreen;
			}
			return Color.OrangeRed;
		}

		public Game SetForeGame()
		{
			foreach (ListViewItem item in ListView.Items)
			{
				Game game = item.Tag as Game;
				if (game.Handle == Win.GetForegroundWindow())
				{
					CurGame = game;
					CurItem = item;
					DownSetting();
					LoadSkill();
					return game;
				}
			}
			return null;
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ListViewNhanVat.SelectedItems.Count > 0)
			{
				CurGame = (Game)ListViewNhanVat.SelectedItems[0].Tag;
				CurItem = ListViewNhanVat.SelectedItems[0];
				LoadSkill();
			}
			try
			{
				CurGame.SaveSetting();
				DownSetting();
			}
			catch
			{
			}
		}

		private void panel3_Paint(object sender, PaintEventArgs e)
		{
		}

		public static void LoadAccountLogin()
		{
			Account.Load();
			ListAutoLogin = Account.Enum();
		}

		[DllImport("Bin\\EasyHook.dll")]
		public static extern int GetMSG();

		[DllImport("Bin\\EasyHook.dll")]
		public static extern bool SetHook(IntPtr proseccid);

		private void InitializeAcBaMenu()
		{
			var automatic = new ToolStripMenuItem("Tự nhận diện và bắt đầu");
			automatic.Click += (sender, args) => StartAcBaSelection(-1);
			ItemAcBa.DropDownItems.Add(automatic);
			foreach (int school in AcBaEvents.Schools)
			{
				int choice = school;
				var manual = new ToolStripMenuItem("Chọn " + AcBaEvents.SchoolName(choice));
				manual.Click += (sender, args) => StartAcBaSelection(choice);
				ItemAcBa.DropDownItems.Add(manual);
			}
			var stop = new ToolStripMenuItem("Dừng Ác Bá");
			stop.Click += (sender, args) => StartAcBaSelection(-2);
			ItemAcBa.DropDownItems.Add(stop);
		}

		private void StartAcBaSelection(int school)
		{
			if (!RequireDungeonContext("Lỗi Ác Bá", out Game selectedGame, out Game leader)) return;
			if (school == -2) { leader.IsAcBa = false; UpdateDungeonMenu(selectedGame, leader); return; }
			ResetDungeonActions(leader);
			if (school == -1) leader.IsAcBa = true;
			else if (school >= 0) leader.SelectManualAcBaSchool(school);
			UpdateDungeonMenu(selectedGame, leader);
		}

		public bool GetUpdate()
		{
			bool result = false;
			if (new WebClient().DownloadString(Global.UpdateURL) == "True")
			{
				try
				{
					result = true;
					Process process = new Process();
					process.StartInfo.FileName = "AutoUpdate.exe";
					process.StartInfo.Arguments = "";
					process.Start();
					Application.Exit();
				}
				catch
				{
					result = true;
					MessageBox.Show("Không tìm thấy tập tin AutoUpdate.exe\nVui lòng tải lại auto mới nhất trên trang chủ");
				}
			}
			return result;
		}

		private CancellationTokenSource updateCancellation;

		private async void FrmMain_Load(object sender, EventArgs e)
		{
			try
			{
				InitializeStartup();
			}
			catch (Exception ex)
			{
				// Do not include exception messages which may contain account/config data.
				MessageBox.Show("Không hoàn tất khởi tạo ứng dụng. Kiểm tra bộ tệp build và quyền ghi thư mục. Mã lỗi: "
					+ ex.GetType().Name + " (0x" + ex.HResult.ToString("X8", CultureInfo.InvariantCulture) + ").",
					AppBranding.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
				Close();
				return;
			}

			// Public update metadata is advisory; it is not an entitlement check.
			updateCancellation = new CancellationTokenSource();
			FormClosed += CancelStartupUpdate;
			try
			{
				UpdateOptions options;
				UpdateResult result;
				if (!UpdateOptions.TryCreate(ConfigurationManager.AppSettings["UpdateMetadataUrl"] ?? Global.UpdateURL,
					ConfigurationManager.AppSettings["UpdateTimeoutMilliseconds"], out options))
					result = new UpdateResult(UpdateStatus.ConfigurationError, "InvalidOptions");
				else
					result = await new UpdateClient().CheckAsync(options,
						int.Parse(Global.Version, CultureInfo.InvariantCulture), updateCancellation.Token);
				if (!IsDisposed && !Disposing) txtlogs.AppendText(StartupMessages.ForUpdate(result));
			}
			catch (Exception)
			{
				if (!IsDisposed && !Disposing)
					txtlogs.AppendText("Không kiểm tra được cập nhật (UpdateCheckError). Ứng dụng tiếp tục khởi động.\n");
			}
			finally
			{
				FormClosed -= CancelStartupUpdate;
				updateCancellation.Dispose();
				updateCancellation = null;
			}
		}

		private void CancelStartupUpdate(object sender, FormClosedEventArgs e)
		{
			if (updateCancellation != null) updateCancellation.Cancel();
		}

		private void InitializeStartup()
		{
			txtlogs.AppendText("Bật auto :" + DateTime.Now.ToString() + "\n");
			notifyIcon1.Text = AppBranding.WindowTitle;
			notifyIcon1.ContextMenu = new ContextMenu();
			notifyIcon1.ContextMenu.MenuItems.Add(new MenuItem("Hiện Auto", HienAuto));
			notifyIcon1.ContextMenu.MenuItems.Add(new MenuItem("Thoát Auto", Thoat));
			Text = AppBranding.WindowTitle;
			if (!File.Exists(Global.DataPath + "\\20.dat"))
			{
				TINHKIEM.FileInstallMaHoa("TinhKiemAuto", "Scripts.xml", Global.DataPath + "\\20.dat");
			}
			Scripts.Load();
			Global.ExaclyTime = DateTime.Now;
			if (!AccountLogin.IsBusy)
			{
				AccountLogin.RunWorkerAsync();
			}
			if (Global.LanLuot)
			{
				tmrLogin.Interval = 10000;
			}
			if (!File.Exists(Global.DataPath + "\\MapPath.dat"))
			{
				TINHKIEM.FileInstall("TinhKiemAuto", "MapPath.dat", Global.DataPath + "\\MapPath.dat");
			}
			TINHKIEM.FileInstallMaHoa("TinhKiemAuto", "skillList.json", Global.DataPath + "\\SkillList.dat");
			if (!File.Exists(Global.DataPath + "\\16.dat"))
			{
				TINHKIEM.FileInstallMaHoa("TinhKiemAuto", "Screen.txt", Global.DataPath + "\\16.dat");
			}
			if (!File.Exists(Global.DataPath + "\\17.dat"))
			{
				TINHKIEM.FileInstallMaHoa("TinhKiemAuto", "PathList.txt", Global.DataPath + "\\17.dat");
			}
			if (!File.Exists(Global.DataPath + "\\18.dat"))
			{
				TINHKIEM.FileInstallMaHoa("TinhKiemAuto", "sdbai.json", Global.DataPath + "\\18.dat");
			}
			TrainData.LoadData();
			LoadTrainMap();
			if (!File.Exists(Global.DataPath + "\\19.dat"))
			{
				TINHKIEM.FileInstallMaHoa("TinhKiemAuto", "allmap.txt", Global.DataPath + "\\19.dat");
			}
			try
			{
				if (Environment.OSVersion.Version.Major > 5)
				{
					ChangeWindowMessageFilter = (ChangeWindowMessageFilterDelegate)FunctionLoader.LoadFunction<ChangeWindowMessageFilterDelegate>(Environment.SystemDirectory + "\\user32.dll", "ChangeWindowMessageFilter");
					ChangeWindowMessageFilter(74u, 1);
				}
				LoadAccountLogin();
				LoadConfig();
			}
			catch (Exception ex2)
			{
				MessageBox.Show(ex2.ToString());
			}
			Win.Active(this);
		}

		public void LoadTrainMap()
		{
			string[] dsmap = TrainData.dsmap;
			List<BaiTrain> dsbai = TrainData.dsbai;
			if (dsmap == null)
			{
				return;
			}
			try
			{
				string[] array = dsmap;
				foreach (string text in array)
				{
					List<int> list = new List<int>();
					foreach (BaiTrain item in dsbai)
					{
						if (item.MapName == text)
						{
							list.Add(item.Level);
						}
					}
					list.Sort();
					int num = list[0];
					list.Reverse();
					int num2 = list[0];
					ComboboxItem comboboxItem = new ComboboxItem();
					comboboxItem.Text = text + "[" + num + "=>" + num2 + "]";
					comboboxItem.Value = text;
					comdanhsachbando.Items.Add(comboboxItem);
				}
				comdanhsachbando.SelectedIndex = 0;
			}
			catch
			{
			}
		}

		private void LoadConfig()
		{
			LoadSetting();
			if (Global.HookMessage != -1)
			{
				Global.HookMessage = GetMSG();
			}
			timeMonitor.Enabled = true;
			IsLoged = true;
			ThreadMonitor = new Thread(Monitor)
			{
				IsBackground = true
			};
			ThreadMonitor.Start();
			Thread thread = new Thread(Auto);
			thread.IsBackground = true;
			thread.Start();
		}

		public static int Bool2Int(bool value)
		{
			if (value)
			{
				return 1;
			}
			return 0;
		}

		private void menuExit_Click(object sender, EventArgs e)
		{
			foreach (Game item in AllGame)
			{
				if (item.IsHooked && item.RecvAddress != 0)
				{
					item.SaveSetting();
					Memory.WriteProcessMemory(item.Memory.Id, item.RecvAddress, item.bufferRecv, 10, 0);
				}
			}
			IsExit = true;
			SaveSetting();
		}

		public void SaveSetting()
		{
			string value = Bool2Int(Global.FollowKey) + "," + Bool2Int(Global.PickItem) + "," + Bool2Int(Global.ItemFillter) + "," + Global.NoiRadius + "," + Global.NgoaiRadius + "," + Global.PickRadius + "," + Global.ExitHPPercent + "," + Bool2Int(Global.AlarmHP) + "," + Global.AlarmHPPercent + "," + Bool2Int(Global.AutoUpLvl) + "," + Global.AutoUpLvlBelow + "," + Bool2Int(Global.AutoDropItem) + "," + Bool2Int(Global.Mute) + "," + Bool2Int(Global.AutoShutDown) + "," + Bool2Int(Global.UseSkillPet) + "," + Bool2Int(Global.AutoComeBack) + "," + Bool2Int(Global.AlarmPk) + "," + Bool2Int(Global.ExitPk) + "," + Global.BuffHPPercent + "," + Global.BuffMPPercent + "," + Global.BuffNMPercent + "," + Bool2Int(Global.BuffPet) + "," + Bool2Int(Global.IsBoQua) + "," + Bool2Int(Global.AutoResetTime) + "," + Bool2Int(Global.BuffQuanDoan) + "," + Bool2Int(Global.AutoAccept) + "," + Bool2Int(Global.AcceptAll) + "," + Bool2Int(Global.IsXaPhu) + "," + TINHKIEM.Key2Int(Global.BaseSkill) + "," + TINHKIEM.Key2Int(Global.NMSkill) + "," + TINHKIEM.Key2Int(Global.HPKey) + "," + TINHKIEM.Bool2Int(Global.IsHuyDanhQuai) + "," + Global.AntiCaptchaSelf.ToString() + "," + Global.FollowRadius + "," + Global.BHDCount + "," + TINHKIEM.Bool2Int(Global.AutoPk) + "," + Global.MaxBHD + "," + TINHKIEM.Bool2Int(Option.SetSafeTime) + "," + TINHKIEM.Bool2Int(Global.HideBHD) + "," + TINHKIEM.Bool2Int(Option.IsDead) + "," + TINHKIEM.Bool2Int(Game.IsHoldPK) + "," + TINHKIEM.Bool2Int(IsCalender) + "," + TINHKIEM.Bool2Int(value: true) + "," + TINHKIEM.Bool2Int(Option.NotDongMon) + "," + TINHKIEM.Bool2Int(Global.IsXuat) + "," + Game.TrongHoaX + "," + TINHKIEM.Bool2Int(Option.PutBase) + "," + TINHKIEM.Bool2Int(value: true) + "," + TINHKIEM.Bool2Int(value: true) + "," + TINHKIEM.Bool2Int(Global.AutoSellItem) + "," + TINHKIEM.Bool2Int(value: true) + "," + TINHKIEM.Bool2Int(Global.IsVutRac) + "," + TINHKIEM.Bool2Int(AlarmAcBa) + "," + Option.HideTime + "," + TINHKIEM.Bool2Int(Option.IsBank) + "," + TINHKIEM.Bool2Int(Option.AutoPoint) + "," + TINHKIEM.Bool2Int(Option.IsHoTro) + "," + TINHKIEM.Bool2Int(Global.IsHuyThaiHo) + "," + TINHKIEM.Bool2Int(Global.IsHyHuu) + "," + TINHKIEM.Bool2Int(Global.IsTuVaoPhai) + "," + 1 + "," + Option.MapBanDoIndex + "," + Option.MaptriLieuIndex + "," + Global.NumNhiemVuDua;
			Setting.SaveSettingOffline("General.dat", value);
		}

		private void LoadSetting()
		{
			SetHotKey();
			int[] array = Setting.LoadSettingOffline("General.dat");
			Game.TrongHoaX = 166;
			try
			{
				if (array != null && array.Length > 26)
				{
					Global.FollowKey = array[0] == 1;
					Global.PickItem = array[1] == 1;
					Global.ItemFillter = array[2] == 1;
					Global.NoiRadius = array[3];
					Global.NgoaiRadius = array[4];
					numrangerpickitem.Value = (Global.PickRadius = array[5]);
					Global.ExitHPPercent = array[6];
					checkBox7.Checked = (Global.AlarmHP = array[7] == 1);
					numcanhbaohp.Value = (Global.AlarmHPPercent = array[8]);
					checkauouplevel.Checked = (Global.AutoUpLvl = array[9] == 1);
					numuplevel.Value = (Global.AutoUpLvlBelow = array[10]);
					Global.AutoDropItem = array[11] == 1;
					Global.Mute = array[12] == 1;
					Global.AutoShutDown = array[13] == 1;
					checkBox10.Checked = (Global.UseSkillPet = array[14] == 1);
					CheckTriLieuComeback.Checked = (Global.AutoComeBack = array[15] == 1);
					Global.AlarmPk = array[16] == 1;
					Global.ExitPk = array[17] == 1;
					nudHP.Value = (Global.BuffHPPercent = array[18]);
					nudMP.Value = (Global.BuffMPPercent = array[19]);
					nudNM.Value = (Global.BuffNMPercent = array[20]);
					checkBox9.Checked = (Global.BuffPet = array[21] == 1);
					Global.IsBoQua = array[22] == 1;
					Global.AutoResetTime = array[23] == 1;
					Global.BuffQuanDoan = array[24] == 1;
					checkdongytodoi.Checked = (Global.AutoAccept = array[25] == 1);
					checkdongytoanbo.Checked = (Global.AcceptAll = array[26] == 1);
					if (array.Length > 27)
					{
						Global.IsXaPhu = array[27] == 1;
					}
					if (array.Length > 28)
					{
						Global.BaseSkill = TINHKIEM.Int2Key(array[28]);
					}
					if (array.Length > 29)
					{
						Global.NMSkill = TINHKIEM.Int2Key(array[29]);
					}
					if (array.Length > 30)
					{
						Global.HPKey = TINHKIEM.Int2Key(array[30]);
					}
					if (array.Length > 31)
					{
						Global.IsHuyDanhQuai = array[31] == 1;
					}
					if (array.Length > 32)
					{
						Global.AntiCaptchaSelf = array[32] == 1;
					}
					if (array.Length > 33)
					{
						numbankinhtheosau.Value = (Global.FollowRadius = array[33]);
					}
					if (array.Length > 34)
					{
						Global.MaxBHD = array[34];
					}
					if (array.Length > 35)
					{
						Global.AutoPk = array[35] == 1;
					}
					if (array.Length > 38)
					{
						Global.HideBHD = array[38] == 1;
					}
					if (array.Length > 39)
					{
						Option.IsDead = array[39] == 1;
					}
					if (array.Length > 40)
					{
						Game.IsHoldPK = array[40] == 1;
					}
					if (array.Length > 41)
					{
						IsCalender = array[41] == 1;
					}
					_ = array.Length;
					if (array.Length > 43)
					{
						Option.NotDongMon = array[43] == 1;
					}
					if (array.Length > 44)
					{
						Global.IsXuat = array[44] == 1;
					}
					if (array.Length > 45)
					{
						Game.TrongHoaX = array[45];
					}
					if (array.Length > 46)
					{
						Option.PutBase = array[46] == 1;
					}
					_ = array.Length;
					_ = array.Length;
					if (array.Length > 49)
					{
						Global.AutoSellItem = array[49] == 1;
					}
					_ = array.Length;
					if (array.Length > 51)
					{
						Global.IsVutRac = array[51] == 1;
					}
					if (array.Length > 53)
					{
						Option.HideTime = array[53];
					}
					if (array.Length > 54)
					{
						Option.IsBank = array[54] == 1;
					}
					if (array.Length > 59)
					{
						Global.IsTuVaoPhai = array[59] == 1;
						Global.GlIsSetMenPai = array[59] == 1;
					}
					if (array.Length > 60)
					{
						Global.GlSetMenPai = (TINHKIEM.Menpai)array[60];
					}
					if (array.Length > 61)
					{
						Option.MapBanDoIndex = array[61];
					}
					if (array.Length > 62)
					{
						Option.MaptriLieuIndex = array[62];
					}
				}
			}
			catch
			{
			}
		}

		public void Auto()
		{
			while (true)
			{
				if (Global.IsFull != 0)
				{
					Game.TickCount += 3;
					if (Game.TickCount % 3 != 0)
					{
						Game.TickCount = 0;
					}
					int num = 150 - (int)(timeAuto.Elapsed.TotalSeconds * 1000.0);
					if (num > 0)
					{
						Thread.Sleep(num);
					}
					timeAuto = Stopwatch.StartNew();
					Thread.Sleep(130);
					if (Game.TickCount % 36 == 0)
					{
						Game.ListDangThuHoach.Clear();
					}
					if (Game.TickCount % 200 == 0)
					{
						Game.lootPacketIdDua.Clear();
					}
					if (Game.TickCount % 18 == 0)
					{
						Game.LureId.Clear();
						Game.HashToaDo.Clear();
						Game.HashDangBon.Clear();
					}
					try
					{
						foreach (KeyValuePair<int, Game> item in dicGame)
						{
							try
							{
								Game value = item.Value;
								value.Auto();
								if (Game.TickCount % 9 == 0)
								{
									value.BuffPet();
								}
								if (Game.TickCount % 18 == 0)
								{
									value.Objects.Pk.Clear();
								}
							}
							catch
							{
							}
						}
					}
					catch
					{
					}
				}
				else
				{
					Thread.Sleep(1000);
				}
			}
		}

		private void Thoat(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void HienAuto(object sender, EventArgs e)
		{
			Show();
		}

		private void radioButton2_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "Chuyển sang chế độ gom quái", ToolTipIcon.Info);
				CurGame.IsLure = true;
			}
		}

		private void groupBox2_Enter(object sender, EventArgs e)
		{
		}

		private void tabdanhquai_Click(object sender, EventArgs e)
		{
		}

		private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
		}

		private void tabautologin_Click(object sender, EventArgs e)
		{
		}

		private void SetInfo()
		{
			foreach (ListViewItem item in ListViewNhanVat.Items)
			{
				Game game = item.Tag as Game;
				game.TheoDoiCanhBao();
				item.Checked = game.IsAuto;
				item.SubItems[0].Text = game.TLBB.Name + " [" + game.TLBB.MenpaiName + " | " + game.TLBB.Lvl + "]";
				if (game.ON_SCENE_TRANSING)
				{
					item.SubItems[0].Text = "Chuyển Cảnh";
					item.SubItems[0].BackColor = Color.Silver;
				}
				else if (game.TLBB.Online)
				{
					item.SubItems[0].BackColor = SystemColors.Window;
					if (game.TLBB.IsPk)
					{
						item.SubItems[0].BackColor = Color.Red;
					}
					else
					{
						item.SubItems[0].BackColor = SystemColors.Window;
					}
				}
				else
				{
					item.SubItems[0].BackColor = Color.Silver;
				}
				if (game.TLBB.IsLeader)
				{
					item.ForeColor = Color.Green;
				}
				else
				{
					item.ForeColor = SystemColors.WindowText;
				}
			}
			if (CurGame == null || !dicGame.ContainsValue(CurGame))
			{
				if (ListViewNhanVat.Items.Count > 0)
				{
					CurGame = (Game)ListViewNhanVat.Items[0].Tag;
					CurItem = ListViewNhanVat.Items[0];
					DownSetting();
					LoadSkill();
					comlenbai_SelectedIndexChanged(null, null);
				}
				return;
			}
			Random random = new Random();
			txthp.Text = CurGame.TLBB.HPPercent + "%";
			txthp.BackColor = GetCodeByPercen(CurGame.TLBB.HPPercent);
			txtmp.Text = CurGame.TLBB.MPPercent + "%";
			txtmp.BackColor = GetCodeByPercen(CurGame.TLBB.MPPercent);
			AutoXuatPhet.Checked = Global.IsXuat;
			txtpet.Text = CurGame.TLBB.PetHPPercent + "%";
			txtpet.BackColor = GetCodeByPercen(CurGame.TLBB.PetHPPercent);
			txttrangthai.Text = CurGame.TLBB.MapName + " (" + (int)CurGame.CharX + ":" + (int)CurGame.CharY + ") | " + CurGame.Status;
			if (CurGame.LenBaiTrain)
			{
				butlenbai.Text = "Đang lên bãi..";
				butlenbai.ForeColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
			}
			else
			{
				butlenbai.Text = "Lên Bãi";
				butlenbai.ForeColor = Color.Black;
			}
			if (CurGame.AutoTrain)
			{
				buttimbai.Text = "Đang Auto Train..";
				buttimbai.ForeColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
			}
			else
			{
				buttimbai.Text = "Tìm Bãi";
				buttimbai.ForeColor = Color.Black;
			}
			if (CurGame.IsTriLieu)
			{
				buttrilieu.ForeColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
				buttrilieu.Text = "Đang Trị Liệu..";
			}
			else
			{
				buttrilieu.Text = "Trị Liệu";
				buttrilieu.ForeColor = Color.Black;
			}
			chekcdanhquai.Checked = CurGame.IsAttack;
			pickbanvatpham.Checked = CurGame.IsSellItem;
			checkpickitem.Checked = CurGame.IsPickItem;
			if (checkradius.Checked)
			{
				checkradius.Text = "Quanh [" + (int)CurGame.RadiusX + "," + (int)CurGame.RadiusY + "]";
			}
		}

		public int GetTime(DateTime dt)
		{
			return (int)(dt - new DateTime(1970, 1, 1)).TotalSeconds;
		}

		public void Monitor()
		{
			while (true)
			{
				if (Global.IsFull != 0)
				{
					try
					{
						HashSet<Process> hashSet = new HashSet<Process>();
						string[] wndClassNames = Win.WndClassNames;
						for (int i = 0; i < wndClassNames.Length; i++)
						{
							foreach (Process item in Win.GetProcessByClassName(wndClassNames[i]))
							{
								try
								{
									if (Win.GameExeProcessNames.Contains(item.MainModule.ModuleName))
									{
										hashSet.Add(item);
									}
								}
								catch
								{
								}
							}
						}
						GameCount = hashSet.Count;
						foreach (KeyValuePair<int, Game> item2 in dicGame)
						{
							if (item2.Value.Process.HasExited)
							{
								if (CurGame == item2.Value)
								{
									CurGame = null;
								}
								Invoke(new CallBack(DeleteItem), item2.Value);
							}
						}
						foreach (Process current3 in hashSet)
						{
							if (!dicGame.ContainsKey(current3.Id) && ((current3.Responding && current3.MainWindowTitle.ToLower().Contains("3.74.9000")) || (current3.Responding && current3.MainWindowTitle.Contains("Thien Long Bat Bo") && Win.GetHandle(current3.Id, "#32770") == IntPtr.Zero) || (current3.Responding && current3.MainWindowTitle.ToLower().Contains("????")) || (current3.Responding && GetTime(DateTime.Now) - GetTime(current3.StartTime) > 60)))
							{
								bool flag = false;
								if (FakeGame != null && FakeGame.ContainsKey(current3.Id))
								{
									foreach (KeyValuePair<int, Stopwatch> item3 in FakeGame)
									{
										if (item3.Key == current3.Id && item3.Value.Elapsed.TotalSeconds > 240.0)
										{
											flag = true;
										}
									}
								}
								if (!flag)
								{
									try
									{
										string mD = Offset.MD51;
										if ((Offset.MD51 == mD || Offset.MD52 == mD) && (!(Win.GetHandle(current3.Id, Win.WndClassNames) == IntPtr.Zero) || !(Win.GetHandle(current3.Id, "#32770") == IntPtr.Zero)))
										{
											Address instance = Address.GetInstance(mD, Offset.OffList);
											Game game = null;
											if (current3.Responding)
											{
												Invoke((Action)delegate
												{
													game = new Game(current3, instance);
												});
											}
											if (Offset.OffList.ToLower().StartsWith("ffff" + mD.ToLower()))
											{
												game.Is2d = true;
											}
											dicGame.Add(current3.Id, game);
											game.SettingLoaded += game_SettingLoaded;
											game.SkillLoaded += game_SkillLoaded;
											Invoke(new CallBack(AddItem), game);
										}
									}
									catch (Exception)
									{
										if (FakeGame == null)
										{
											FakeGame = new Dictionary<int, Stopwatch>();
										}
										if (!FakeGame.ContainsKey(current3.Id))
										{
											FakeGame.Add(current3.Id, Stopwatch.StartNew());
										}
									}
								}
							}
							if (Win.GetHandle(current3.Id, "#32770") != IntPtr.Zero)
							{
								string mD2 = Offset.MD51;
								Memory = new Memory(current3.Id);
								Address instance2 = Address.GetInstance(mD2, Offset.OffList);
								Memory.Write(instance2.MultiAcc, 2425393296u, 4);
								Memory.Write(instance2.MultiAcc + 4, 2425393296u, 4);
								Thread.Sleep(500);
								Win.PostMessage(Win.GetHandle(current3.Id, "#32770"), 16, 0, 0);
							}
						}
						Thread.Sleep(2000);
					}
					catch
					{
					}
				}
				else
				{
					Thread.Sleep(1000);
				}
			}
		}

		private void game_SkillLoaded(object sender, EventArgs e)
		{
			Invoke(new CallBack(SkillLoaded), sender as Game);
		}

		public void DownSetting()
		{
			if (CurGame == null)
			{
				return;
			}
			txtmkkho.Text = CurGame.Pass2;
			TablControl.TabPages[0].Text = CurGame.TLBB.Name;
			chekcdanhquai.Checked = CurGame.IsAttack;
			if (CurGame.IsLure)
			{
				radgom.Checked = true;
				rad11.Checked = false;
			}
			else
			{
				radgom.Checked = false;
				rad11.Checked = true;
			}
			txtthoigian.Text = ((int)CurGame.TimeGiaoChat).ToString() ?? "";
			checktholinhchau.Checked = CurGame.UsingTholinhChau;
			if (CurGame.LenBaiTrain)
			{
				butlenbai.Text = "Đang lên bãi..";
			}
			else
			{
				butlenbai.Text = "Lên Bãi";
			}
			if (CurGame.AutoTrain)
			{
				buttimbai.Text = "Đang Auto Train..";
			}
			else
			{
				buttimbai.Text = "Tìm Bãi";
			}
			if (CurGame.IsTriLieu)
			{
				buttrilieu.Text = "Đang Trị Liệu..";
			}
			else
			{
				buttrilieu.Text = "Trị Liệu";
			}
			comdanhsachbando.SelectedIndex = CurGame.LenBanDoIndex;
			comlenbai.SelectedIndex = CurGame.LenBaiIndex;
			checkradius.Checked = CurGame.IsRadius;
			if (CurGame.TLBB.IsNoi)
			{
				numberdanhquanh.Value = Global.NoiRadius;
			}
			else
			{
				numberdanhquanh.Value = Global.NgoaiRadius;
			}
			numrangerpickitem.Value = Global.PickRadius;
			txttoadox.Text = CurGame._baitrain.PosX.ToString() ?? "";
			txttoadoy.Text = CurGame._baitrain.PosY.ToString() ?? "";
			CheckthuPet.Checked = CurGame.AutoThuPet;
			CheckReGenPET.Checked = CurGame.IsPet;
			checkregenhp.Checked = CurGame.IsHP;
			checkrengenmp.Checked = CurGame.IsMP;
			checkisNM.Checked = CurGame.IsNM;
			checkcongsinh.Checked = CurGame.CongSinh;
			checkhuyette.Checked = CurGame.HuyetTe;
			CheckAutoHoiSinh.Checked = CurGame.AutoHoiSinh;
			Checkthongbaochatmat.Checked = CurGame.AlarmChat;
			checkhuyitem.Checked = CurGame.IsDropItem;
			pickbanvatpham.Checked = CurGame.IsSellItem;
			checkgiaochat.Checked = CurGame.IsRao;
			checkautocatkho.Checked = CurGame.IsBank;
			chekcautox2.Checked = CurGame.TuAnX2;
			chekcusingitem.Checked = CurGame.AutoEatVatPham;
			txtnoidunggiaochat.Text = CurGame.RaoTxt;
			chekcautox2.Checked = CurGame.TuAnX2;
			checkpickitem.Checked = CurGame.IsPickItem;
			cboXuatPet.Items.Clear();
			ComboboxItem comboboxItem = new ComboboxItem();
			comboboxItem.Text = "Không Xuất";
			comboboxItem.Value = 0;
			cboXuatPet.Items.Add(comboboxItem);
			int num = -1;
			int num2 = 0;
			foreach (KeyValuePair<int, string> item in CurGame.TLBB.DicPet)
			{
				num2++;
				ComboboxItem comboboxItem2 = new ComboboxItem();
				comboboxItem2.Text = item.Value;
				comboboxItem2.Value = item.Key;
				if (item.Key.ToString("X8") == CurGame.PetId)
				{
					num = num2;
				}
				cboXuatPet.Items.Add(comboboxItem2);
			}
			if (num != -1)
			{
				cboXuatPet.SelectedIndex = num;
			}
			if (IsCheDoTab)
			{
				if (CurGame.IsCheDo)
				{
					butche.Text = "Tắt";
				}
				else
				{
					butche.Text = "Chế";
				}
				comboloai.SelectedIndex = CurGame.CheLoai;
				comcapdtd.SelectedIndex = CurGame.CheCap;
				VatLieu vatLieu = new VatLieu();
				vatLieu = CurGame.getsoluong(comboloai.SelectedItem.ToString(), comcapdtd.SelectedIndex + 1);
				txtbingan.Text = vatLieu.BiNgan.ToString() ?? "";
				txttinhthiet.Text = vatLieu.TinhThiet.ToString() ?? "";
				txtvaibong.Text = vatLieu.VaiBong.ToString() ?? "";
				txttaodo.Text = vatLieu.DaTaoDo.ToString() ?? "";
				txtdache.Text = vatLieu.DaChe.ToString() ?? "";
				txtdahuy.Text = vatLieu.DaHuy.ToString() ?? "";
				_ = vatLieu.DaChe;
				numsonguyenlieu.Value = CurGame.SoLuongMua;
				comboloai.SelectedIndex = CurGame.CheLoai;
				comcapdtd.SelectedIndex = CurGame.CheCap;
				numericUpDown4.Value = CurGame.SoLuongChe;
				checkBox12.Checked = !CurGame.IsMuaNguyenLieu;
				checkBox13.Checked = CurGame.HuyNguyenLieu;
				numericUpDown6.Value = CurGame.CheDiem;
				numericUpDown5.Value = CurGame.CheDong;
				numsosao.Value = CurGame.CheSao;
			}
		}

		private void SettingLoaded(Game game)
		{
			if (CurGame == game)
			{
				DownSetting();
			}
			foreach (ListViewItem item in ListViewNhanVat.Items)
			{
				if (item.Tag as Game == game)
				{
					item.Checked = game.IsAuto;
					break;
				}
			}
		}

		private void SkillLoaded(Game game)
		{
			if (CurGame == game)
			{
				LoadSkill();
			}
		}

		private void LoadSkill()
		{
			listviewskillhotro.Items.Clear();
			listViewSkill.Items.Clear();
			comdanhsachdanhquai.Items.Clear();
			comskillhotro.Items.Clear();
			foreach (Skill skill in CurGame.Skills)
			{
				if (skill.Name.Length > 0)
				{
					ComboboxItem comboboxItem = new ComboboxItem();
					comboboxItem.Text = skill.Name;
					comboboxItem.Value = skill;
					if (!Skill.IsBand(skill.PacketId) && !Skill.IsBuffSkill(skill.PacketId))
					{
						comdanhsachdanhquai.Items.Add(comboboxItem);
					}
					else if (!Skill.IsBand(skill.PacketId) && Skill.IsBuffSkill(skill.PacketId))
					{
						comskillhotro.Items.Add(comboboxItem);
					}
					if (skill.Use && listViewSkill.FindItemWithText(skill.Name) == null)
					{
						listViewSkill.Items.Add(skill.Name);
					}
					if (skill.UserBuff && listviewskillhotro.FindItemWithText(skill.Name) == null)
					{
						listviewskillhotro.Items.Add(skill.Name);
					}
				}
				if (comdanhsachdanhquai.Items.Count > 0)
				{
					comdanhsachdanhquai.SelectedIndex = 0;
				}
				if (comskillhotro.Items.Count > 0)
				{
					comskillhotro.SelectedIndex = 0;
				}
			}
		}

		private void listViewSkill_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void game_SettingLoaded(object sender, EventArgs e)
		{
			Invoke(new CallBack(SettingLoaded), sender as Game);
		}

		public void AddItem(Game game)
		{
			ListViewItem listViewItem = new ListViewItem(new string[1] { "ĐăngNhập" })
			{
				UseItemStyleForSubItems = false
			};
			listViewItem.Tag = game;
			game.Item = listViewItem;
			listViewItem.Checked = game.IsAuto;
			listViewItem.SubItems[0].Text = game.TLBB.Name + " [" + game.TLBB.MenpaiName + " | " + game.TLBB.Lvl + "]";
			ListViewNhanVat.Items.Add(listViewItem);
			ListViewNhanVat.Columns[0].Text = "Tổng Nhân Vật [" + ListViewNhanVat.Items.Count + "]";
			GameCount = ListViewNhanVat.Items.Count;
			if (GameCount > MaxGame)
			{
				MaxGame = GameCount;
			}
		}

		public void DeleteItem(Game deleteGame)
		{
			foreach (ListViewItem item in ListViewNhanVat.Items)
			{
				Game game = (Game)item.Tag;
				if (game == deleteGame)
				{
					game.Exit();
					item.Remove();
				}
				if (ListViewNhanVat.Items.Count == 0 && Global.AutoShutDown)
				{
					new ShutDown();
				}
			}
			ListViewNhanVat.Columns[0].Text = "Tổng Nhân Vật [" + ListViewNhanVat.Items.Count + "]";
		}

		private void timeMonitor_Tick(object sender, EventArgs e)
		{
			if (Follow)
			{
				foreach (KeyValuePair<int, Game> item in dicGame)
				{
					Game value = item.Value;
					if (value.TLBB.IsLeader)
					{
						value.TrieuTap();
					}
				}
			}
			if (IsLoginTab)
			{
				foreach (ListViewItem item2 in ListViewLogin.Items)
				{
					Account account = item2.Tag as Account;
					Game game = account.game;
					if (game != null)
					{
						item2.SubItems[3].Text = game.TLBB.Name + " | Thứ " + account.LoginIndex;
						item2.SubItems[4].Text = game.TLBB.MenpaiName;
					}
					if (account.Status == "Online")
					{
						item2.SubItems[2].Text = account.Status;
						continue;
					}
					item2.SubItems[2].Text = account.Status;
					item2.SubItems[3].Text = account.Name + "| Thứ " + account.LoginIndex;
				}
			}
			foreach (KeyValuePair<int, Game> item3 in dicGame)
			{
				Game value2 = item3.Value;
				if (value2 == null)
				{
					continue;
				}
				List<ThongBao> listThongBao = value2.ListThongBao;
				if (listThongBao.Count <= 0)
				{
					continue;
				}
				int num = 0;
				ThongBao[] array = listThongBao.ToArray();
				foreach (ThongBao thongBao in array)
				{
					CanhBao.AddLog(thongBao.tideu, thongBao.noidung, thongBao.Type);
					try
					{
						listThongBao.RemoveAt(num);
						CurGame.ListThongBao.RemoveAt(num);
						num++;
					}
					catch
					{
					}
				}
			}
			try
			{
				SetInfo();
			}
			catch
			{
			}
		}

		private void CheckTriLieuComeback_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				if (!VuaBatXong())
				{
					notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (CheckTriLieuComeback.Checked ? " Bật " : " Tắt ") + "trị liệu và quay lại bãi", ToolTipIcon.Info);
				}
				Global.AutoComeBack = false;
				CurGame.DeadX = 0;
			}
		}

		private void chekcdanhquai_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				if (!VuaBatXong())
				{
					notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (chekcdanhquai.Checked ? " Bật " : " Tắt ") + "đánh quái", ToolTipIcon.Info);
				}
				CurGame.IsAttack = chekcdanhquai.Checked;
			}
		}

		private void rad11_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				if (!VuaBatXong())
				{
					notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "Chuyển sang chế độ đánh từng con", ToolTipIcon.Info);
				}
				CurGame.IsLure = false;
			}
		}

		private void checkradius_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				if (!VuaBatXong())
				{
					notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (checkradius.Checked ? " Bật " : " Tắt ") + "đánh quanh điểm", ToolTipIcon.Info);
				}
				CurGame.IsRadius = checkradius.Checked;
			}
		}

		private void numberdanhquanh_ValueChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				if (CurGame.TLBB.IsNoi)
				{
					Global.NoiRadius = (int)numberdanhquanh.Value;
				}
				else
				{
					Global.NgoaiRadius = (int)numberdanhquanh.Value;
				}
			}
		}

		private void butboqua_Click(object sender, EventArgs e)
		{
			new BoQua().Show();
		}

		private void checktholinhchau_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				if (!VuaBatXong())
				{
					notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (checktholinhchau.Checked ? " Bật " : " Tắt ") + "sử dụng Thổ Linh Châu", ToolTipIcon.Info);
				}
				CurGame.UsingTholinhChau = checktholinhchau.Checked;
			}
		}

		private void nudHP_ValueChanged(object sender, EventArgs e)
		{
			Global.BuffHPPercent = (int)nudHP.Value;
		}

		private void nudMP_ValueChanged(object sender, EventArgs e)
		{
			Global.BuffMPPercent = (int)nudMP.Value;
		}

		private void numbercongsinhhp_ValueChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.CongSinhValue = (int)numbercongsinhhp.Value;
			}
		}

		private void numhuyettemp_ValueChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.HuyetTeValue = (int)numhuyettemp.Value;
			}
		}

		private void nudNM_ValueChanged(object sender, EventArgs e)
		{
			Global.BuffNMPercent = (int)nudNM.Value;
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			Global.AlarmHPPercent = (int)numcanhbaohp.Value;
		}

		private void checkregenhp_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				if (!VuaBatXong())
				{
					notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (checkregenhp.Checked ? " Bật " : " Tắt ") + "tự sử dụng HP", ToolTipIcon.Info);
				}
				CurGame.IsHP = checkregenhp.Checked;
			}
		}

		private void checkrengenmp_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				if (!VuaBatXong())
				{
					notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (checkrengenmp.Checked ? " Bật " : " Tắt ") + "tự sử dụng MP", ToolTipIcon.Info);
				}
				CurGame.IsMP = checkrengenmp.Checked;
			}
		}

		private void checkcongsinh_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				if (!VuaBatXong())
				{
					notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (checkcongsinh.Checked ? " Bật " : " Tắt ") + "cộng sinh PET", ToolTipIcon.Info);
				}
				CurGame.CongSinh = checkcongsinh.Checked;
			}
		}

		private void groupBox3_Enter(object sender, EventArgs e)
		{
		}

		private void checkhuyette_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				if (!VuaBatXong())
				{
					notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (checkhuyette.Checked ? " Bật " : " Tắt ") + "huyết tế PET", ToolTipIcon.Info);
				}
				CurGame.HuyetTe = checkhuyette.Checked;
			}
		}

		private void checkisNM_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.IsNM = checkisNM.Checked;
				if (!VuaBatXong())
				{
					notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (checkhuyette.Checked ? " Bật " : " Tắt ") + "buff Nga My", ToolTipIcon.Info);
				}
			}
		}

		public int GETTIMEHIENTAI(DateTime hientai)
		{
			return (int)(hientai - new DateTime(1970, 1, 1)).TotalSeconds;
		}

		private void CheckAutoHoiSinh_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				if (!VuaBatXong())
				{
					notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (CheckAutoHoiSinh.Checked ? " Bật " : " Tắt ") + "hồi sinh sau khi chết", ToolTipIcon.Info);
				}
				CurGame.AutoHoiSinh = CheckAutoHoiSinh.Checked;
			}
		}

		private void checkBox7_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				if (!VuaBatXong())
				{
					notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (checkBox7.Checked ? " Bật " : " Tắt ") + "cảnh báo HP", ToolTipIcon.Info);
				}
				Global.AlarmHP = checkBox7.Checked;
			}
		}

		private void txttoadox_TextChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame._baitrain.PosX = int.Parse(txttoadox.Text);
			}
		}

		private void txttoadoy_TextChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame._baitrain.PosY = int.Parse(txttoadoy.Text);
			}
		}

		private void butlenbai_Click(object sender, EventArgs e)
		{
			BaiTrain baiTrain = (BaiTrain)(comlenbai.SelectedItem as ComboboxItem).Value;
			if (CurGame != null)
			{
				if (Unity.IsNumeric(txttoadox.Text) && Unity.IsNumeric(txttoadoy.Text) && int.Parse(txttoadoy.Text) > 0 && int.Parse(txttoadoy.Text) > 0)
				{
					List<BaiTrain> dsbai = TrainData.dsbai;
					BaiTrain baiTrain2 = new BaiTrain();
					foreach (BaiTrain item in dsbai)
					{
						if (item.MapName == baiTrain.MapName)
						{
							baiTrain2.Level = 40;
							baiTrain2.MapID = item.MapID;
							baiTrain2.Name = "Bãi tùy chỉnh";
							baiTrain2.PosX = int.Parse(txttoadox.Text);
							baiTrain2.PosY = int.Parse(txttoadoy.Text);
							break;
						}
					}
					CurGame._baitrain = baiTrain2;
					if (CurGame.LenBaiTrain)
					{
						butlenbai.Text = "Lên Bãi";
						CurGame.LenBaiTrain = false;
					}
					else
					{
						butlenbai.Text = "Đang lên..";
						CurGame.LenBaiTrain = true;
						notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + CurGame.TLBB.Name.ToUpper() + "] bắt đầu lên bãi", ToolTipIcon.Info);
					}
				}
				else
				{
					MessageBox.Show("Kiểm tra lại tọa độ");
				}
			}
			else
			{
				MessageBox.Show("Chức năng này cần phải có nhân vật đang auto");
			}
		}

		private void comdanhsachbando_SelectedIndexChanged(object sender, EventArgs e)
		{
			comlenbai.Items.Clear();
			try
			{
				List<BaiTrain> dsbai = TrainData.dsbai;
				string text = (comdanhsachbando.SelectedItem as ComboboxItem).Value.ToString();
				foreach (BaiTrain item in dsbai)
				{
					if (item.MapName == text)
					{
						ComboboxItem comboboxItem = new ComboboxItem();
						comboboxItem.Text = item.Name;
						comboboxItem.Value = item;
						comlenbai.Items.Add(comboboxItem);
					}
				}
				if (CurGame != null && comdanhsachbando.SelectedIndex != -1)
				{
					CurGame.LenBanDoIndex = comdanhsachbando.SelectedIndex;
				}
				comlenbai.SelectedIndex = 0;
			}
			catch
			{
			}
		}

		private void buttrilieu_Click(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				if (CurGame.IsTriLieu)
				{
					buttrilieu.Text = "Trị Liệu";
					CurGame.IsTriLieu = false;
				}
				else
				{
					buttrilieu.Text = "Đang Trị Liệu..";
					CurGame.IsTriLieu = true;
					notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + CurGame.TLBB.Name.ToUpper() + "] đang tiến hành trị liệu", ToolTipIcon.Info);
				}
			}
		}

		private void buttimbai_Click(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				if (CurGame.AutoTrain)
				{
					buttimbai.Text = "Tìm Bãi";
					CurGame.AutoTrain = false;
				}
				else
				{
					buttimbai.Text = "Đang AutoTrain..";
					CurGame.AutoTrain = true;
					notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + CurGame.TLBB.Name.ToUpper() + "] đang tiến hành AUTO Train", ToolTipIcon.Info);
				}
			}
		}

		private void comlenbai_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.LenBaiIndex = comlenbai.SelectedIndex;
				BaiTrain baiTrain = (BaiTrain)(comlenbai.SelectedItem as ComboboxItem).Value;
				txttoadox.Text = baiTrain.PosX.ToString() ?? "";
				txttoadoy.Text = baiTrain.PosY.ToString() ?? "";
			}
		}

		private void thoátToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		[DllImport("user32.dll")]
		private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

		[DllImport("user32.dll")]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		public void SetHotKey()
		{
			RegisterHotKey(base.Handle, 1, 0, Keys.Pause.GetHashCode());
			RegisterHotKey(base.Handle, 6, 6, Keys.C.GetHashCode());
			RegisterHotKey(base.Handle, 13, 2, Keys.Q.GetHashCode());
			RegisterHotKey(base.Handle, 17, 1, Keys.F1.GetHashCode());
			RegisterHotKey(base.Handle, 18, 2, Keys.End.GetHashCode());
			RegisterHotKey(base.Handle, 20, 2, Keys.L.GetHashCode());
			RegisterHotKey(base.Handle, 21, 2, Keys.H.GetHashCode());
			RegisterHotKey(base.Handle, 23, 2, Keys.N.GetHashCode());
			RegisterHotKey(base.Handle, 29, 2, Keys.M.GetHashCode());
			RegisterHotKey(base.Handle, 24, 2, Keys.T.GetHashCode());
			RegisterHotKey(base.Handle, 25, 2, Keys.B.GetHashCode());
			RegisterHotKey(base.Handle, 26, 2, Keys.D.GetHashCode());
			RegisterHotKey(base.Handle, 27, 2, Keys.Delete.GetHashCode());
			RegisterHotKey(base.Handle, 28, 1, Keys.F2.GetHashCode());
			RegisterHotKey(base.Handle, 39, 1, Keys.F3.GetHashCode());
			RegisterHotKey(base.Handle, 32, 2, Keys.O.GetHashCode());
			RegisterHotKey(base.Handle, 33, 2, Keys.G.GetHashCode());
			RegisterHotKey(base.Handle, 34, 2, Keys.R.GetHashCode());
			RegisterHotKey(base.Handle, 35, 2, Keys.I.GetHashCode());
		}

		public void UnSetHoKey()
		{
			UnregisterHotKey(base.Handle, 1);
			UnregisterHotKey(base.Handle, 6);
			UnregisterHotKey(base.Handle, 13);
			UnregisterHotKey(base.Handle, 17);
			UnregisterHotKey(base.Handle, 18);
			UnregisterHotKey(base.Handle, 20);
			UnregisterHotKey(base.Handle, 21);
			UnregisterHotKey(base.Handle, 23);
			UnregisterHotKey(base.Handle, 24);
			UnregisterHotKey(base.Handle, 25);
			UnregisterHotKey(base.Handle, 26);
			UnregisterHotKey(base.Handle, 27);
			UnregisterHotKey(base.Handle, 28);
			UnregisterHotKey(base.Handle, 29);
			UnregisterHotKey(base.Handle, 30);
			UnregisterHotKey(base.Handle, 31);
			UnregisterHotKey(base.Handle, 32);
			UnregisterHotKey(base.Handle, 33);
			UnregisterHotKey(base.Handle, 34);
			UnregisterHotKey(base.Handle, 35);
			UnregisterHotKey(base.Handle, 39);
		}

		private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				if (MessageBox.Show("Bạn có chắn chắn muốn thoát?", "Thoát Auto", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					menuExit_Click(null, null);
					string text = DateTime.Now.ToString("ddMMyy_HHmm");
					LoadFile.WriteFileWithEncrypt(txtlogs.Text, Global.LogPath + "\\Log_" + text + ".dat");
				}
				else
				{
					e.Cancel = true;
				}
			}
			catch
			{
				Application.Exit();
			}
		}

		private void butthemskilldanhquai_Click(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				Skill skill = CurGame.Skills.Find((Skill x) => x.Name == comdanhsachdanhquai.SelectedItem.ToString());
				if (skill != null)
				{
					skill.Use = true;
					listViewSkill.Items.Add(skill.Name);
					CurGame.SaveSkill();
				}
			}
		}

		private void pictureBox10_Click(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				Skill skill = CurGame.Skills.Find((Skill x) => x.Name == comskillhotro.SelectedItem.ToString());
				if (skill != null)
				{
					skill.UserBuff = true;
					listviewskillhotro.Items.Add(skill.Name);
					CurGame.SaveSkillBuff();
				}
			}
		}

		public bool HaveGame(int processId)
		{
			foreach (Account item in ListAutoLogin)
			{
				if (item.game != null && item.game.ProcessId == processId)
				{
					return true;
				}
			}
			return false;
		}

		private void Login(Account account)
		{
			bool flag = false;
			flag = !(account.Status == "Mở Game");
			if (account.game != null)
			{
				return;
			}
			foreach (KeyValuePair<int, Game> item in dicGame)
			{
				try
				{
					if ((item.Value.TLBB.IsSelectServer || item.Value.TLBB.IsNexLogin) && !HaveGame(item.Value.ProcessId) && Process.GetProcessById(item.Value.ProcessId).MainModule.FileName == account.Path)
					{
						if (item.Value.KetQuaSetTitle == 0)
						{
							CanhBao.AddLog("Lỗi Tự Động Đăng Nhập", "Lỗi đăng nhập tài khoản " + account.Name + "\nVui lòng đăng nhập thủ công ", CanhBao.Kieu.Eror);
							account.Status = "";
							item.Value.IsQuit = true;
							account.game.UnHookRecv();
							account.game = null;
							dicGame.Remove(account.game.ProcessId);
						}
						else
						{
							account.IsNextLogin = Stopwatch.StartNew();
							account.Status = "Đăng Nhập";
							item.Value.IsQuit = false;
							account.game = item.Value;
							account.Entered = false;
							account.IsSelectRole = false;
							account.game.BHDCount = 0;
						}
						return;
					}
				}
				catch
				{
				}
			}
			if (!flag || (TotalGame >= Global.MaxBHD && !account.IsForceOpen))
			{
				return;
			}
			if (TienIch.GameCount() > 3)
			{
				IsWait = false;
				account.Status = "";
				account.Entered = false;
				account.IsSelectRole = false;
				CanhBao.Msg("Lỗi Mở Game", "Đã mở quá giới hạn Client cho phép", CanhBao.Kieu.Eror);
				return;
			}
			IsWait = true;
			account.Status = "Mở Game";
			account.OpenGameTime = Stopwatch.StartNew();
			account.Entered = false;
			account.IsSelectRole = false;
			try
			{
				string text = LoadFile.LoadFileWithDecrypt(Global.DataPath + "\\ExecutePath.dat");
				string mD = Offset.MD51.ToLower();
				Process.Start(new ProcessStartInfo
				{
					FileName = text,
					Arguments = ".\\Bin\\Game.exe " + GetCMDBYMD5(mD),
					WorkingDirectory = Path.GetDirectoryName(text)
				});
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
				account.Status = "";
				SettingPath();
			}
		}

		public string GetCMDBYMD5(string MD5)
		{
			return "-fl";
		}

		private void tmrLogin_Tick(object sender, EventArgs e)
		{
			if (IsStop)
			{
				return;
			}
			IsDangCho = false;
			foreach (Account item in ListAutoLogin)
			{
				if (item.Status.Contains("Đang chờ"))
				{
					IsDangCho = true;
					break;
				}
			}
			LoginCount = 0;
			foreach (Account item2 in ListAutoLogin)
			{
				if (item2.game != null)
				{
					LoginCount++;
				}
			}
			foreach (KeyValuePair<int, Game> item3 in dicGame)
			{
				if (item3.Value == null || item3.Value.IsQuit || !item3.Value.TLBB.Online || item3.Value.IsCheckOnline)
				{
					continue;
				}
				item3.Value.IsCheckOnline = true;
				foreach (Account item4 in ListAutoLogin)
				{
					if (item4.Ids.Contains(item3.Value.TLBB.Id))
					{
						item4.game = item3.Value;
						item4.IsSave = true;
					}
				}
			}
			bool flag = false;
			foreach (Account item5 in ListAutoLogin)
			{
				if (item5.game != null && item5.game.IsQuit && !item5.game.TLBB.Online && item5.game.TLBB.IsSelectServer)
				{
					item5.game.IsQuit = false;
					item5.game = null;
					item5.Status = "";
				}
				if (item5.game != null && item5.game.IsQuit)
				{
					continue;
				}
				if (item5.Status == "Thoát")
				{
					item5.game = null;
				}
				if (item5.game == null)
				{
					continue;
				}
				try
				{
					Process.GetProcessById(item5.game.ProcessId);
					if (item5.Status != "Mở Game")
					{
						if (item5.IsCaptcha)
						{
							item5.Status = "Đọc Captcha";
							flag = true;
						}
						else if (item5.Online)
						{
							item5.Status = "Online";
						}
						else if (!item5.game.ON_SCENE_TRANSING)
						{
							item5.Status = "Đăng Nhập";
						}
					}
					if (item5.game.TLBB.Online && !item5.IsSaveName)
					{
						item5.IsSaveName = true;
						item5.Name = item5.game.TLBB.Name;
						item5.Lvl = item5.game.TLBB.Lvl.ToString();
						item5.Menpai = item5.game.TLBB.MenpaiName.ToString();
						item5.Ids = item5.game.TLBB.Id;
						Account.Save();
					}
				}
				catch (Exception)
				{
					if (item5.Status != "xong BHD" && item5.Status != "xong BTD" && item5.Status != "xong DUA")
					{
						if (item5.game.IsBHDByLogin)
						{
							item5.Status = "Đang chờ làm BHD...";
						}
						else if (item5.game.IsBTDByLogin)
						{
							item5.Status = "Đang chờ làm Trừng Ác...";
						}
						else if (item5.game.IsDuaByLogin)
						{
							item5.Status = "Đang chờ làm Q Dưa...";
						}
						else
						{
							item5.Status = "Thoát";
						}
					}
					item5.game = null;
				}
			}
			if (!flag)
			{
				Captchas.Clear();
			}
			IsWait = false;
			foreach (Account item6 in ListAutoLogin)
			{
				if (item6.Status == "xong BHD" || item6.Status == "xong BTD" || item6.Status == "xong DUA" || item6.game != null)
				{
					continue;
				}
				if (item6.Status.Contains("Mở Game"))
				{
					IsWait = true;
					Login(item6);
					if (item6.OpenGameTime == null)
					{
						item6.OpenGameTime = Stopwatch.StartNew();
					}
					if (item6.IsBHD && item6.OpenGameTime.Elapsed.TotalMinutes > 10.0 && TotalGame < Global.MaxBHD)
					{
						item6.OpenGameTime = Stopwatch.StartNew();
						item6.Status = "Đang chờ làm BHD...";
					}
					if (item6.IsTrungAc && item6.OpenGameTime.Elapsed.TotalMinutes > 10.0 && TotalGame < Global.MaxBHD)
					{
						item6.OpenGameTime = Stopwatch.StartNew();
						item6.Status = "Đang chờ làm Trừng Ác...";
					}
					if (item6.IsDua && item6.OpenGameTime.Elapsed.TotalMinutes > 10.0 && TotalGame < Global.MaxBHD)
					{
						item6.OpenGameTime = Stopwatch.StartNew();
						item6.Status = "Đang chờ làm Q Dưa...";
					}
					break;
				}
				if (item6.Status.Contains("Đang chờ"))
				{
					Login(item6);
					break;
				}
			}
		}

		private void AccountLogin_DoWork(object sender, DoWorkEventArgs e)
		{
			Invoke((Action)delegate
			{
				ListViewLogin.Items.Clear();
			});
			Invoke((Action)delegate
			{
				ComMayChu.Items.Clear();
			});
			foreach (Account item in ListAutoLogin)
			{
				ListViewItem listViewItem = new ListViewItem
				{
					UseItemStyleForSubItems = false
				};
				listViewItem.Text = item.User;
				listViewItem.SubItems.Add(item.Server);
				listViewItem.SubItems.Add(item.Status);
				listViewItem.SubItems.Add(item.Name + " | Thứ " + item.LoginIndex);
				listViewItem.SubItems.Add(item.Menpai);
				listViewItem.Tag = item;
				Invoke((Action)delegate
				{
					ListViewLogin.Items.Add(listViewItem);
				});
			}
			foreach (ServerList sv in Unity.GetDanhSach())
			{
				Invoke((Action)delegate
				{
					ComMayChu.Items.Add(sv.ServerName);
				});
			}
			if (ComMayChu.Items.Count > 0)
			{
				Invoke((Action)delegate
				{
					ComMayChu.SelectedIndex = 0;
				});
			}
		}

		private void listviewskillhotro_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Delete || listviewskillhotro.SelectedItems.Count == 0)
			{
				return;
			}
			foreach (ListViewItem listViewItem in listviewskillhotro.SelectedItems)
			{
				if (listViewItem.Text.Trim() != "")
				{
					Skill skill = CurGame.Skills.Find((Skill x) => TINHKIEM.VietLien(x.Name) == TINHKIEM.VietLien(listViewItem.Text.Trim()));
					if (skill != null)
					{
						skill.UserBuff = false;
						CurGame.SaveSkillBuff();
					}
					listViewItem.Remove();
				}
			}
		}

		private void listViewSkill_KeyDown(object sender, KeyEventArgs e)
		{
			if (listViewSkill.SelectedItems.Count == 0)
			{
				return;
			}
			foreach (ListViewItem listViewItem in listViewSkill.SelectedItems)
			{
				if (listViewItem.Text.Trim() != "")
				{
					Skill skill = CurGame.Skills.Find((Skill x) => TINHKIEM.VietLien(x.Name) == TINHKIEM.VietLien(listViewItem.Text.Trim()));
					if (skill != null)
					{
						skill.Use = false;
						CurGame.SaveSkill();
					}
					listViewItem.Remove();
				}
			}
		}

		public bool CheckExit(string username, string Server)
		{
			foreach (Account item in ListAutoLogin)
			{
				if (item.User == username && item.Server == Server)
				{
					return true;
				}
			}
			return false;
		}

		private void button7_Click(object sender, EventArgs e)
		{
			if (txttk.Text.Length == 0 || txtmk.Text.Length == 0)
			{
				MessageBox.Show("Kiểm tra dữ liệu nhập vào", "Thêm tài khoản lỗi", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			else if (!CheckExit(txttk.Text.Trim(), ComMayChu.SelectedItem.ToString()))
			{
				new Account(txttk.Text.Trim(), txtmk.Text.Trim(), "Tình Kiếm", ComMayChu.SelectedItem.ToString(), ComMayChu.SelectedItem.ToString());
				LoadAccountLogin();
				if (!AccountLogin.IsBusy)
				{
					AccountLogin.RunWorkerAsync();
				}
			}
			else
			{
				MessageBox.Show("Tài khoản đã được thêm rồi", "Thêm tài khoản lỗi", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		public void SettingPath()
		{
			MessageBox.Show(this, "Bạn cần phải chọn đường dẫn tới Game.exe\r\nFile Game.exe nằm trong thư mục Bin của TLBB", AppBranding.Name, MessageBoxButtons.OK);
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Game.exe |Game.exe| GameOLD.exe |GameOLD.exe| All files (*.*)|*.*";
			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				try
				{
					LoadFile.WriteFileWithEncrypt(openFileDialog.FileName, Global.DataPath + "\\ExecutePath.dat");
				}
				catch
				{
				}
			}
		}

		private void ListViewLogin_DoubleClick(object sender, EventArgs e)
		{
			if (ListViewLogin.SelectedItems.Count == 0)
			{
				return;
			}
			if (TienIch.GameCount() > 3)
			{
				CanhBao.Msg("Lỗi Mở Game", "Đã mở quá giới hạn Client cho phép", CanhBao.Kieu.Eror);
				return;
			}
			Account account = (Account)ListViewLogin.SelectedItems[0].Tag;
			if (account.game == null || account.Status == "Thoát")
			{
				if (account.Status == "" || account.Status == "Thoát")
				{
					if (account.Path == "")
					{
						SettingPath();
						return;
					}
					account.Status = "Đang chờ...";
					account.IsForceOpen = true;
				}
			}
			else
			{
				account.game.Active();
			}
		}

		private void ListViewLogin_DragDrop(object sender, DragEventArgs e)
		{
			XmlNode xmlNode = Account.XML.SelectSingleNode("/*");
			ListAutoLogin.Clear();
			xmlNode.RemoveAll();
			IEnumerator enumerator = ListViewLogin.Items.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Account account = (Account)((ListViewItem)enumerator.Current).Tag;
				xmlNode.AppendChild(account.Node);
				ListAutoLogin.Add(account);
			}
			Account.Save();
			if (this.AccChanged != null)
			{
				this.AccChanged(this, null);
			}
		}

		private void menuDelete_Click(object sender, EventArgs e)
		{
			if (ListViewLogin.SelectedItems.Count == 0 || MessageBox.Show(this, "Bạn có muốn xóa thông tin những acc đã chọn", AppBranding.Name, MessageBoxButtons.YesNo) != DialogResult.Yes)
			{
				return;
			}
			foreach (ListViewItem selectedItem in ListViewLogin.SelectedItems)
			{
				try
				{
					XmlNode node = ((Account)selectedItem.Tag).Node;
					Account.XML.SelectSingleNode("/*").RemoveChild(node);
					selectedItem.Remove();
				}
				catch
				{
				}
			}
			if (this.AccChanged != null)
			{
				this.AccChanged(this, null);
			}
			Account.Save();
			LoadAccountLogin();
		}

		private void ListViewLogin_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				menuDelete_Click(null, null);
			}
			if (e.KeyCode == Keys.Return)
			{
				if (TienIch.GameCount() > 3)
				{
					CanhBao.Msg("Lỗi Mở Game", "Đã mở quá giới hạn Client cho phép", CanhBao.Kieu.Eror);
					return;
				}
				IEnumerator enumerator = ListViewLogin.SelectedItems.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Account account = (Account)((ListViewItem)enumerator.Current).Tag;
					if (account.Path == "")
					{
						SettingPath();
						return;
					}
					if (account.game == null)
					{
						account.Status = "Đang chờ...";
					}
				}
			}
			if (e.KeyCode == Keys.A && e.Control)
			{
				IEnumerator enumerator2 = ListViewLogin.Items.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					((ListViewItem)enumerator2.Current).Selected = true;
				}
			}
			if (e.KeyCode == Keys.F1)
			{
				IEnumerator enumerator3 = ListViewLogin.SelectedItems.GetEnumerator();
				while (enumerator3.MoveNext())
				{
					((Account)((ListViewItem)enumerator3.Current).Tag).LoginIndex = "1";
				}
			}
			if (e.KeyCode == Keys.F2)
			{
				IEnumerator enumerator4 = ListViewLogin.SelectedItems.GetEnumerator();
				while (enumerator4.MoveNext())
				{
					((Account)((ListViewItem)enumerator4.Current).Tag).LoginIndex = "2";
				}
			}
			if (e.KeyCode == Keys.F3)
			{
				IEnumerator enumerator5 = ListViewLogin.SelectedItems.GetEnumerator();
				while (enumerator5.MoveNext())
				{
					((Account)((ListViewItem)enumerator5.Current).Tag).LoginIndex = "3";
				}
			}
			if (e.KeyCode != Keys.F5)
			{
				return;
			}
			IEnumerator enumerator6 = ListViewLogin.SelectedItems.GetEnumerator();
			while (enumerator6.MoveNext())
			{
				Account account2 = (Account)((ListViewItem)enumerator6.Current).Tag;
				if (account2.Path == "")
				{
					SettingPath();
					break;
				}
				if (account2.game == null)
				{
					account2.Status = "";
				}
			}
		}

		private void Pop()
		{
			if (!base.IsDisposed && Global.AntiCaptcha && IsPop)
			{
				_ = Running;
			}
		}

		private void ValidCaptcha(string hash)
		{
			if (!base.IsDisposed && Global.AntiCaptcha)
			{
				_ = IsPop;
			}
		}

		private void tmrRefresh_Tick(object sender, EventArgs e)
		{
			if (IsStop)
			{
				return;
			}
			if (Global.AntiCaptcha)
			{
				foreach (KeyValuePair<string, string> captcha in Captchas)
				{
					if (!Answers.ContainsKey(captcha.Key))
					{
						Pop();
					}
				}
			}
			foreach (Account item in ListAutoLogin)
			{
				Game game = item.game;
				if (game == null || item.Status == "xong BHD" || item.game.BachHoaDuyenCompleted || item.Status == "xong BTD" || item.game.IsXongTrungAc || item.Status == "xong DUA" || item.game.IsXongTrungAc)
				{
					continue;
				}
				if (item.Online)
				{
					if (!item.IsSave)
					{
						item.IsSave = true;
						if (!item.Ids.Contains(game.TLBB.Id))
						{
							item.Ids += game.TLBB.Id;
						}
					}
					item.Entered = false;
					item.IsSelectRole = false;
					if (Answers.ContainsKey(item.ImgHash) && item.IsGetAn)
					{
						ValidCaptcha(item.ImgHash);
						Answers.Remove(item.ImgHash);
						if (Captchas.ContainsKey(item.ImgHash))
						{
							Captchas.Remove(item.ImgHash);
						}
					}
				}
				if (item.IsDua && game.TLBB.OnlineTimeSec < 60)
				{
					game.IsQDua = true;
					game.IsDuaByLogin = true;
				}
				if (item.IsTrong)
				{
					if (game.TLBB.OnlineTimeSec < 60)
					{
						bool isBonHoa = (game.IsTrongByLogin = true);
						game.IsTrongHoa = (game.IsBonHoa = isBonHoa);
					}
				}
				else
				{
					if (item.IsBHD && game.TLBB.OnlineTimeSec < 60)
					{
						game.IsBachHoaDuyen = true;
						game.IsBHDByLogin = true;
					}
					if (item.IsNhanMam && game.TLBB.OnlineTimeSec < 60)
					{
						game.IsBachHoaDuyen = true;
						game.IsBHDByLogin = true;
						game.IsNhanMam = true;
					}
				}
				if (item.IsTrungAc)
				{
					game.StartTrungAcFromLogin();
				}
				if (item.IsCaptcha && !game.TLBB.IsLogon)
				{
					notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "Vui lòng nhập Captcha cho tài khoản : " + item.User, ToolTipIcon.Info);
				}
				if (!item.Online)
				{
					if (game.TLBB.IsNexLogin && game.KetQuaSetTitle == 1)
					{
						game.LUA.HuoDongRiChengNextClick();
						continue;
					}
					if (game.TLBB.IsSelectServer)
					{
						item.Entered = (item.IsSelectRole = false);
						if (item.ServerIndex == -1)
						{
							continue;
						}
						if (game.Address.GameType == 1)
						{
							if (item.Server == "Thiên Long 15")
							{
								game.LuaDoOneLineString("setmetatable(_G, {__index = LoginSelectServer_Env}); SelectServerEvent_Area_SwitchPage(9); SelectServerEvent_ServerBn_Clicked(" + 1 + ",0); SelectServerEvent_SelectOk();");
							}
							else if (item.Server == "Thiên Long 16")
							{
								game.LuaDoOneLineString("setmetatable(_G, {__index = LoginSelectServer_Env}); SelectServerEvent_Area_SwitchPage(9); SelectServerEvent_ServerBn_Clicked(" + 2 + ",0); SelectServerEvent_SelectOk();");
							}
							else
							{
								game.LuaDoOneLineString("setmetatable(_G, {__index = LoginSelectServer_Env}); SelectServerEvent_Area_SwitchPage(10); SelectServerEvent_ServerBn_Clicked(" + item.ServerIndex + ",0); SelectServerEvent_SelectOk();");
							}
						}
						else
						{
							game.LuaDoOneLineString("setmetatable(_G, {__index = LoginSelectServer_Env}); local index = GameProduceLogin:GetServerAreaCount() - 1; setmetatable(_G, {__index = LoginSelectServer_Env}); SelectServer_SelectAreaServer(index); setmetatable(_G, {__index = LoginSelectServer_Env}); SelectServer_ConfirmSelectLine(" + item.ServerIndex + ");");
						}
						continue;
					}
					if (game.TLBB.IsLogon && !item.Entered)
					{
						item.LogonTime = Stopwatch.StartNew();
						if (item.TailIndex != -1)
						{
							game.LUA.LogOnSelectTail(item.TailIndex);
						}
						string user = item.User;
						foreach (char wParam in user)
						{
							Win.PostMessage(game.Handle, 258, wParam, 0);
						}
						Win.PostMessage(game.Handle, 256, 9, 0);
						Win.PostMessage(game.Handle, 257, 9, 0);
						user = item.Pass;
						foreach (char wParam2 in user)
						{
							Win.PostMessage(game.Handle, 258, wParam2, 0);
						}
						Win.PostMessage(game.Handle, 256, 13, 0);
						Win.PostMessage(game.Handle, 257, 13, 0);
						item.Entered = true;
						item.IsSelectRole = false;
					}
					if (game.TLBB.IsLogon)
					{
						item.SelectAccTime = Stopwatch.StartNew();
						if (item.LogonTime.Elapsed.TotalSeconds < 22.0)
						{
							item.game.PushDebugMessage("Chọn lại máy chủ sau " + (int)(22.0 - item.LogonTime.Elapsed.TotalSeconds) + " giây");
						}
						else
						{
							item.SelectAccTime = Stopwatch.StartNew();
							item.IsSelectRole = false;
							item.Entered = false;
							item.LoginMessageTime = 0;
							game.LUA.LogOn_ExitToSelectServer();
							item.SelectAccTime = null;
						}
					}
					else
					{
						item.LogonTime = Stopwatch.StartNew();
					}
					if (item.Entered && game.TLBB.IsLogon && !game.TLBB.IsSelectServerQuest)
					{
						item.LoginMessageTime++;
						if (item.LoginMessageTime > 10 || item.LoginMessageTime == 0)
						{
							item.LoginMessageTime = 0;
							Win.PostMessage(game.Handle, 256, 13, 0);
							Win.PostMessage(game.Handle, 257, 13, 0);
						}
					}
					if (game.TLBB.IsSelectCharacter && !game.TLBB.IsTextCaptcha && !item.IsSelectRole)
					{
						if (item.SelectAccTime == null)
						{
							item.SelectAccTime = Stopwatch.StartNew();
						}
						if (item.LoginIndex == "1")
						{
							if (item.SelectAccTime.Elapsed.TotalSeconds > 5.0 && item.SelectAccTime.Elapsed.TotalSeconds <= 10.0)
							{
								game.LuaDoString("setmetatable(_G, {__index = LoginSelectServer_Env}); local index = SelectRole_SelectRole1();");
								item.IsSelect = -1;
							}
						}
						else if (item.LoginIndex == "2")
						{
							if (item.SelectAccTime.Elapsed.TotalSeconds > 5.0 && item.SelectAccTime.Elapsed.TotalSeconds <= 10.0)
							{
								game.LuaDoString("setmetatable(_G, {__index = LoginSelectServer_Env}); local index = SelectRole_SelectRole2();");
								item.IsSelect = -1;
							}
						}
						else if (item.LoginIndex == "3" && item.SelectAccTime.Elapsed.TotalSeconds > 5.0 && item.SelectAccTime.Elapsed.TotalSeconds <= 10.0)
						{
							game.LuaDoString("setmetatable(_G, {__index = LoginSelectServer_Env}); local index = SelectRole_SelectRole3();");
							item.IsSelect = -1;
						}
						if (item.SelectAccTime.Elapsed.TotalSeconds > 5.0)
						{
							game.LUA.SelectRoleEnterGame();
							item.Entered = false;
						}
					}
					if (game.TLBB.IsSelectCharacter)
					{
						if (item.SelectAccTime.Elapsed.TotalSeconds >= 60.0)
						{
							item.SelectAccTime = Stopwatch.StartNew();
							item.IsSelectRole = false;
							item.Entered = false;
							item.LoginMessageTime = 0;
							item.game.LuaDoOneLineString("DataPool:SendLoginCode('1222')");
						}
						else
						{
							item.game.PushDebugMessage("Đổi Captcha sau " + (int)(60.0 - item.SelectAccTime.Elapsed.TotalSeconds) + " giây");
						}
					}
					if (game.TLBB.IsTextCaptcha)
					{
						item.IsSelectRole = true;
					}
				}
				else if (game.TLBB.IsSelectServer)
				{
					item.game = null;
					item.Status = "";
					item.Entered = false;
					item.IsSelectRole = false;
				}
			}
		}

		private void TablControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (TablControl.SelectedTab == TablControl.TabPages[6])
			{
				IsLoginTab = true;
			}
			else
			{
				IsLoginTab = false;
			}
			if (TablControl.SelectedTab == TablControl.TabPages[5])
			{
				IsCheDoTab = true;
				if (CurGame != null)
				{
					comboloai.SelectedIndex = CurGame.CheLoai;
					comcapdtd.SelectedIndex = CurGame.CheCap;
					comboBox3.SelectedIndex = 0;
					if (CurGame.IsCheDo)
					{
						butche.Text = "Tắt";
					}
					else
					{
						butche.Text = "Chế";
					}
					VatLieu vatLieu = new VatLieu();
					vatLieu = CurGame.getsoluong(comboloai.SelectedItem.ToString(), comcapdtd.SelectedIndex + 1);
					txtbingan.Text = vatLieu.BiNgan.ToString() ?? "";
					txttinhthiet.Text = vatLieu.TinhThiet.ToString() ?? "";
					txtvaibong.Text = vatLieu.VaiBong.ToString() ?? "";
					txttaodo.Text = vatLieu.DaTaoDo.ToString() ?? "";
					txtdache.Text = vatLieu.DaChe.ToString() ?? "";
					txtdahuy.Text = vatLieu.DaHuy.ToString() ?? "";
					numsosao.Value = CurGame.CheSao;
					numericUpDown6.Value = CurGame.CheDiem;
					numericUpDown5.Value = CurGame.CheDong;
					numsonguyenlieu.Value = CurGame.SoLuongMua;
					numericUpDown4.Value = CurGame.SoLuongChe;
					checkBox12.Checked = !CurGame.IsMuaNguyenLieu;
					checkBox13.Checked = CurGame.HuyNguyenLieu;
				}
			}
			else
			{
				IsCheDoTab = false;
			}
		}

		private void FrmMain_Resize(object sender, EventArgs e)
		{
		}

		private void notifyIcon1_DoubleClick(object sender, EventArgs e)
		{
			Show();
			base.WindowState = FormWindowState.Normal;
			base.TopLevel = true;
		}

		private void ẩnAutoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Hide();
			notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "Auto đang ẩn tại đây.Chuột phải xem danh sách chức năng!", ToolTipIcon.Info);
		}

		private void checkpickitem_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				if (!VuaBatXong())
				{
					notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (checkpickitem.Checked ? " Bật " : " Tắt ") + "nhặt vật phẩm", ToolTipIcon.Info);
				}
				CurGame.IsPickItem = checkpickitem.Checked;
			}
		}

		private void numericUpDown2_ValueChanged(object sender, EventArgs e)
		{
			Global.PickRadius = (int)numrangerpickitem.Value;
		}

		private void checkhuyitem_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.IsDropItem = checkhuyitem.Checked;
				if (!VuaBatXong())
				{
					notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (checkhuyitem.Checked ? " Bật " : " Tắt ") + "hủy vật phẩm", ToolTipIcon.Info);
				}
			}
		}

		private void pickbanvatpham_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.IsSellItem = pickbanvatpham.Checked;
				if (!VuaBatXong())
				{
					notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (pickbanvatpham.Checked ? " Bật " : " Tắt ") + "bán vật phẩm", ToolTipIcon.Info);
				}
			}
		}

		private void checkautocatkho_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.IsBank = checkautocatkho.Checked;
				if (!VuaBatXong())
				{
					notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (checkautocatkho.Checked ? " Bật " : " Tắt ") + "cất đồ vào kho", ToolTipIcon.Info);
				}
			}
		}

		private void checkautovutrac_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				Global.IsVutRac = checkautovutrac.Checked;
				if (!VuaBatXong())
				{
					notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (checkautocatkho.Checked ? " Bật " : " Tắt ") + "vứt rác", ToolTipIcon.Info);
				}
			}
		}

		private void chekcautox2_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.TuAnX2 = chekcautox2.Checked;
				if (!VuaBatXong())
				{
					notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (chekcautox2.Checked ? " Bật " : " Tắt ") + "ăn X2.5", ToolTipIcon.Info);
				}
			}
		}

		private void chekcusingitem_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.AutoEatVatPham = chekcusingitem.Checked;
				if (!VuaBatXong())
				{
					notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (chekcusingitem.Checked ? " Bật " : " Tắt ") + "sử dụng vật phẩm tuần hoàn", ToolTipIcon.Info);
				}
			}
		}

		private void butdanhsachhuy_Click(object sender, EventArgs e)
		{
			new DropItem().Show();
		}

		private void butbanvatpham_Click(object sender, EventArgs e)
		{
			new SellItem().Show();
		}

		private void AutoXuatPhet_CheckedChanged(object sender, EventArgs e)
		{
			Global.IsXuat = AutoXuatPhet.Checked;
		}

		private void checkBox9_CheckedChanged(object sender, EventArgs e)
		{
			Global.BuffPet = checkBox9.Checked;
		}

		private void checkBox10_CheckedChanged(object sender, EventArgs e)
		{
			Global.UseSkillPet = checkBox10.Checked;
		}

		private void CheckthuPet_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.AutoThuPet = CheckthuPet.Checked;
			}
		}

		private void groupBox6_Enter(object sender, EventArgs e)
		{
		}

		private void CheckReGenPET_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.IsPet = CheckReGenPET.Checked;
			}
		}

		private void checkdongytodoi_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				Global.AutoAccept = checkdongytodoi.Checked;
				if (!VuaBatXong())
				{
					CanhBao.Msg("Thiết Lập Thành Công", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (checkdongytodoi.Checked ? " Bật " : " Tắt ") + "đồng ý tổ đội", CanhBao.Kieu.OK);
				}
			}
		}

		private void checkdongytoanbo_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame == null)
			{
				return;
			}
			Global.AcceptAll = checkdongytoanbo.Checked;
			if (!VuaBatXong())
			{
				CanhBao.Msg("Thiết Lập Thành Công", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (checkdongytoanbo.Checked ? " Bật " : " Tắt ") + "đồng ý tất cả", CanhBao.Kieu.OK);
			}
			if (Global.AcceptAll)
			{
				using (Dictionary<int, Game>.Enumerator enumerator = dicGame.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						enumerator.Current.Value.SetTeam(null);
						return;
					}
				}
			}
			using (Dictionary<int, Game>.Enumerator enumerator = dicGame.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					enumerator.Current.Value.SetTeamFromList(Setting.BuffValue);
				}
			}
		}

		private void numbankinhtheosau_ValueChanged(object sender, EventArgs e)
		{
			Global.FollowRadius = (int)numbankinhtheosau.Value;
		}

		private void txtmkkho_TextChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.Pass2 = txtmk.Text;
				CurGame.SavePass2();
			}
		}

		private void pictureBox12_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Game.exe |Game.exe| GameOLD.exe |GameOLD.exe| All files (*.*)|*.*";
			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				try
				{
					LoadFile.WriteFileWithEncrypt(openFileDialog.FileName, Global.DataPath + "\\ExecutePath.dat");
				}
				catch
				{
				}
			}
		}

		private void checkgiaochat_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.IsRao = checkgiaochat.Checked;
				if (!VuaBatXong())
				{
					CanhBao.Msg("Thiết Lập Thành Công", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (checkgiaochat.Checked ? " Bật " : " Tắt ") + "giao chát", CanhBao.Kieu.OK);
				}
			}
		}

		private void checkauouplevel_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				Global.AutoUpLvl = checkauouplevel.Checked;
				if (!VuaBatXong())
				{
					CanhBao.Msg("Thiết Lập Thành Công", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (checkauouplevel.Checked ? " Bật " : " Tắt ") + "tự tăng cấp độ", CanhBao.Kieu.OK);
				}
			}
		}

		private void checkautoskillf1_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				Option.PutBase = checkautoskillf1.Checked;
				if (!VuaBatXong())
				{
					CanhBao.Msg("Thiết Lập Thành Công", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (checkautoskillf1.Checked ? " Bật " : " Tắt ") + "đặt kỹ năng cơ bản vào F1", CanhBao.Kieu.OK);
				}
			}
		}

		private void button6_Click(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.Pass2 = txtmkkho.Text;
				CurGame.SavePass2();
				CurGame.UnlockPass2();
			}
		}

		private void button5_Click(object sender, EventArgs e)
		{
			if (CurGame != null && !CurGame.TLBB.RaoTxt.Contains("INTERFACE") && !CurGame.TLBB.RaoTxt.Contains("mật mã động thái"))
			{
				txtnoidunggiaochat.Text = CurGame.TLBB.RaoTxt;
			}
		}

		private void pictureBox11_Click(object sender, EventArgs e)
		{
			new Chat().Show();
		}

		private void txtthoigian_TextChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				try
				{
					CurGame.TimeGiaoChat = double.Parse(txtthoigian.Text);
				}
				catch
				{
					CurGame.TimeGiaoChat = 180.0;
					txtthoigian.Text = "180";
				}
			}
		}

		private void Checkthongbaochatmat_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.AlarmChat = Checkthongbaochatmat.Checked;
				if (!VuaBatXong())
				{
					CanhBao.Msg("Thiết Lập Thành Công", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (Checkthongbaochatmat.Checked ? " Bật " : " Tắt ") + "thông báo chat mật", CanhBao.Kieu.OK);
				}
			}
		}

		private void cboXuatPet_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cboXuatPet.SelectedIndex == -1 || CurGame == null)
			{
				return;
			}
			try
			{
				Setting.SetValue(CurGame.TLBB.Id + "PET", ((int)(cboXuatPet.SelectedItem as ComboboxItem).Value).ToString("X8"));
				int num = int.Parse((cboXuatPet.SelectedItem as ComboboxItem).Value.ToString());
				CurGame.PetId = num.ToString("X8");
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
		}

		private void numericUpDown3_ValueChanged(object sender, EventArgs e)
		{
			Global.PetLvl = (int)numericUpDown3.Value;
		}

		private void numuplevel_ValueChanged(object sender, EventArgs e)
		{
			Global.AutoUpLvlBelow = (int)numuplevel.Value;
		}

		private void txtnoidunggiaochat_TextChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.RaoTxt = txtnoidunggiaochat.Text;
			}
		}

		private void thoátToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (!RequireDungeonContext("Lỗi Lâu Lan", out Game selectedGame, out Game leader))
			{
				return;
			}
			leader.IsLauLanTamBao = !leader.IsLauLanTamBao;
			UpdateDungeonMenu(selectedGame, leader);
			notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + (selectedGame.TLBB.Name ?? string.Empty).ToUpper() + "]" + (leader.IsLauLanTamBao ? " Bật " : " Tắt ") + "Auto Lâu Lan Tầm Bảo", ToolTipIcon.Info);
		}

		private void càiĐườngDẫnGameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SettingPath();
		}

		private void butitemtuanhoan_Click(object sender, EventArgs e)
		{
			new AutoEatItem().Show();
		}

		private void buttheo_Click(object sender, EventArgs e)
		{
			notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "Bật Theo Sau Key Cho Toàn Bộ AUTO", ToolTipIcon.Info);
			Global.FollowKey = true;
		}

		private void pictureBox3_Click(object sender, EventArgs e)
		{
			notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "Tắt Theo Sau Key Cho Toàn Bộ Auto", ToolTipIcon.Info);
			Global.FollowKey = false;
		}

		private void pictureBox6_Click(object sender, EventArgs e)
		{
			notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "Bật Sử Dụng Skill Cho Toàn Bộ Auto", ToolTipIcon.Info);
			Global.UsingSkill = true;
		}

		private void pictureBox5_Click(object sender, EventArgs e)
		{
			notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "Tắt sử dụng Skill Cho Toàn Bộ Auto", ToolTipIcon.Info);
			Global.UsingSkill = false;
		}

		private void butpickall_Click(object sender, EventArgs e)
		{
			foreach (KeyValuePair<int, Game> item in dicGame)
			{
				Game value = item.Value;
				if (value != null)
				{
					value.IsPickItem = true;
				}
			}
			notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "Bật Nhặt Vật Phẩm Cho Toàn Bộ Auto", ToolTipIcon.Info);
		}

		private void unpickall_Click(object sender, EventArgs e)
		{
			foreach (KeyValuePair<int, Game> item in dicGame)
			{
				Game value = item.Value;
				if (value != null)
				{
					value.IsPickItem = false;
				}
			}
			notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "Tắt Nhặt Vật Phẩm Cho Toàn Bộ Auto", ToolTipIcon.Info);
		}

		private void butdanhsachdongy_Click(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				new Buff(CurGame).Show();
			}
		}

		private void checkBox12_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.IsMuaNguyenLieu = !checkBox12.Checked;
			}
		}

		private void numsonguyenlieu_ValueChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.SoLuongMua = (int)numsonguyenlieu.Value;
			}
		}

		private void numericUpDown4_ValueChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.SoLuongChe = (int)numericUpDown4.Value;
			}
		}

		private void Radnoi_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.CheNoiNgoai = 1;
			}
		}

		private void radnoingoai_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.CheNoiNgoai = 3;
			}
		}

		private void radngoai_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.CheNoiNgoai = 2;
			}
		}

		private void checkBox13_CheckedChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.HuyNguyenLieu = checkBox13.Checked;
			}
		}

		private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.TaskSauCheDO = comboBox3.SelectedItem.ToString();
			}
		}

		private void butche_Click(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.DaChe = 0;
				CurGame.DaHuy = 0;
				CurGame.TocDoChe = 1;
				CurGame.TmpItemBeforeChe = CurGame.Packet.GetListIndex;
				CurGame.IsMuaNguyenLieu = !checkBox12.Checked;
				CurGame.SoLuongMua = (int)numsonguyenlieu.Value;
				CurGame.SoLuongChe = (int)numericUpDown4.Value;
				CurGame.HuyNguyenLieu = checkBox13.Checked;
				CurGame.TaskSauCheDO = comboBox3.SelectedItem.ToString();
				CurGame.TempCount = 0;
				CurGame.CheTen = comboloai.SelectedItem.ToString();
				CurGame.CheCap = comcapdtd.SelectedIndex;
				CurGame.CheLoai = comboloai.SelectedIndex;
				CurGame.IsCheDo = !CurGame.IsCheDo;
				CurGame.CheSao = (int)numsosao.Value;
				CurGame.CheDong = (int)numericUpDown5.Value;
				CurGame.CheDiem = (int)numericUpDown6.Value;
				CurGame.Kiemtranguyenlieu = CurGame.IsCheDo;
				CurGame.CheNoiNgoai = GetCheNoiNgaoi();
				if (CurGame.IsCheDo)
				{
					butche.Text = "Tắt";
				}
				else
				{
					butche.Text = "Chế";
				}
				CanhBao.Msg("Chế Đồ", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (CurGame.IsCheDo ? " Bật " : " Tắt ") + "Chế Đồ", CanhBao.Kieu.OK);
			}
		}

		public int GetCheNoiNgaoi()
		{
			if (Radnoi.Checked)
			{
				return 1;
			}
			if (radnoingoai.Checked)
			{
				return 3;
			}
			if (radngoai.Checked)
			{
				return 2;
			}
			return 3;
		}

		private void numsosao_ValueChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.CheSao = (int)numsosao.Value;
			}
		}

		private void numericUpDown5_ValueChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.CheDong = (int)numericUpDown5.Value;
			}
		}

		private void numericUpDown6_ValueChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.CheDiem = (int)numericUpDown6.Value;
			}
		}

		private void CheDoF5_Tick(object sender, EventArgs e)
		{
			if (CurGame != null && IsCheDoTab)
			{
				if (CurGame.IsCheDo)
				{
					butche.Text = "Tắt";
					comboloai.SelectedIndex = CurGame.CheLoai;
					comcapdtd.SelectedIndex = CurGame.CheCap;
					VatLieu vatLieu = new VatLieu();
					vatLieu = CurGame.getsoluong(comboloai.SelectedItem.ToString(), comcapdtd.SelectedIndex + 1);
					txtbingan.Text = vatLieu.BiNgan.ToString() ?? "";
					txttinhthiet.Text = vatLieu.TinhThiet.ToString() ?? "";
					txtvaibong.Text = vatLieu.VaiBong.ToString() ?? "";
					txttaodo.Text = vatLieu.DaTaoDo.ToString() ?? "";
					txtdache.Text = vatLieu.DaChe.ToString() ?? "";
					txtdahuy.Text = vatLieu.DaHuy.ToString() ?? "";
					_ = vatLieu.DaChe;
					numsonguyenlieu.Value = CurGame.SoLuongMua;
					comboloai.SelectedIndex = CurGame.CheLoai;
					comcapdtd.SelectedIndex = CurGame.CheCap;
					numericUpDown4.Value = CurGame.SoLuongChe;
					checkBox12.Checked = !CurGame.IsMuaNguyenLieu;
					checkBox13.Checked = CurGame.HuyNguyenLieu;
					numericUpDown6.Value = CurGame.CheDiem;
					numericUpDown5.Value = CurGame.CheDong;
					numsosao.Value = CurGame.CheSao;
				}
				else
				{
					butche.Text = "Chế";
				}
			}
		}

		private void pictureBox2_Click(object sender, EventArgs e)
		{
			notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "Bật xuất PET toàn bộ AUTO", ToolTipIcon.Info);
			Global.IsXuat = true;
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{
			notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "Tắt xuất PET toàn bộ AUTO", ToolTipIcon.Info);
			Global.IsXuat = false;
		}

		protected override void WndProc(ref Message m)
		{
			if ((long)m.Msg == 74)
			{
				if (m.LParam == IntPtr.Zero) { base.WndProc(ref m); return; }
				COPYDATASTRUCT cOPYDATASTRUCT = (COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(COPYDATASTRUCT));
				if (cOPYDATASTRUCT.lpData == IntPtr.Zero || cOPYDATASTRUCT.cbData < 4 || cOPYDATASTRUCT.cbData > 65536)
				{ base.WndProc(ref m); return; }
				byte[] array = new byte[cOPYDATASTRUCT.cbData];
				Marshal.Copy(cOPYDATASTRUCT.lpData, array, 0, cOPYDATASTRUCT.cbData);
				int num = -1;
				int num2 = -1;
				for (int i = 0; i < array.Length - 10; i++)
				{
					if (array[i] == 218 && array[i + 1] == 3 && array[i + 6] == 3)
					{
						num = i;
						break;
					}
					if (array[i] == 30 && array[i + 1] == 2 && array[i + 6] == 3)
					{
						num = i;
						break;
					}
					if (array.Length > 25 && array[i] == 218 && array[i + 1] == 3 && array[i + 6] == 4)
					{
						num2 = i;
						break;
					}
				}
				int num3 = BitConverter.ToInt32(array, 0);
				Game game = null;
				foreach (KeyValuePair<int, Game> item in dicGame)
				{
					if (item.Value.ProcessId == num3)
					{
						game = item.Value;
					}
				}
				string systemText;
				if (game != null && AcBaEvents.TryDecodeSystem(array, ConfigurationManager.AppSettings["AcBaSystemEncoding"] ?? "VISCII", out systemText))
					game.ReceiveAcBaNotice(systemText);
				if (num != -1)
				{
					try
					{
						foreach (KeyValuePair<int, Game> item2 in dicGame)
						{
							if (item2.Value.ProcessId != num3)
							{
								continue;
							}
							byte[] array3;
							if (item2.Value.Address.GameType == 1)
							{
								array3 = new byte[array.Length - num - 11];
								for (int k = num + 11; k < array.Length - 1; k++)
								{
									array3[k - (num + 11)] = array[k];
									if (array[k] < 32)
									{
										array3[k - (num + 11)] = 35;
									}
								}
							}
							else
							{
								array3 = new byte[array.Length - num - 8];
								for (int l = num + 8; l < array.Length - 1; l++)
								{
									array3[l - (num + 8)] = array[l];
									if (array[l] < 32)
									{
										array3[l - (num + 8)] = 35;
									}
								}
							}
							string text = DateTime.Now.ToString("HH:mm dd-MM") + "#[" + item2.Value.TLBB.Name + "]:" + ConverterEx.VISCII2Unicode(array3);
							if (text.Split('#').Length > 2)
							{
								string text2 = "";
								int num4 = 2;
								while (num4 < text.Split('#').Length && num4 <= 3)
								{
									text2 += text.Split('#')[num4];
									num4++;
								}
								text = text.Split('#')[0] + "[" + text2 + "] nói thầm  " + text.Split('#')[1];
								if (text2.Length < 3)
								{
									text = "";
								}
							}
							if (text == "")
							{
								return;
							}
							try
							{
								using (SoundPlayer soundPlayer = new SoundPlayer("c:\\Windows\\Media\\tada.wav"))
								{
									soundPlayer.Play();
								}
							}
							catch
							{
							}
							AddLog(text + "\n");
						}
					}
					catch
					{
					}
				}
			}
			if (m.Msg == 6 && m.WParam.ToInt32() == 1 && Control.FromHandle(m.LParam) == null)
			{
				base.WindowState = FormWindowState.Normal;
			}
			if (m.Msg == 786)
			{
				_ = (int)m.LParam;
				_ = (int)m.LParam;
				int num5 = m.WParam.ToInt32();
				if (num5 == 34 && CurGame != null)
				{
					CurGame.IsTriLieu = !CurGame.IsTriLieu;
					CanhBao.Msg("Thiết Lập Thành Công", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (CurGame.IsTriLieu ? " Bật " : " Tắt ") + "trị liệu!", CanhBao.Kieu.OK);
				}
				if (num5 == 18 && CurGame != null)
				{
					if (!CurGame.ishide)
					{
						CurGame.Hide();
						CurGame.ishide = true;
					}
					else
					{
						CurGame.Active();
						CurGame.ishide = false;
					}
					CanhBao.Msg("Thiết Lập Thành Công", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (CurGame.ishide ? " Bật " : " Tắt ") + "ẩn Game!", CanhBao.Kieu.OK);
				}
				if (num5 == 33 && CurGame != null)
				{
					CurGame.AutoTrain = !CurGame.AutoTrain;
					CanhBao.Msg("Thiết Lập Thành Công", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (CurGame.AutoTrain ? " Bật " : " Tắt ") + "Auto Train!", CanhBao.Kieu.OK);
				}
				if (num5 == 26 && CurGame != null)
				{
					Global.FollowKey = !Global.FollowKey;
					CanhBao.Msg("Thiết Lập Thành Công", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (Global.FollowKey ? " Bật " : " Tắt ") + "theo Key!", CanhBao.Kieu.OK);
				}
				if (num5 == 1)
				{
					CurGame.IsAuto = !CurGame.IsAuto;
					CanhBao.Msg("Thiết Lập Thành Công", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (CurGame.IsAuto ? " Bật " : " Tắt ") + "Auto!", CanhBao.Kieu.OK);
				}
				if (num5 == 6)
				{
					CurGame.IsPickItem = !CurGame.IsPickItem;
					CanhBao.Msg("Thiết Lập Thành Công", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (CurGame.IsPickItem ? " Bật " : " Tắt ") + "nhặt vật phẩm", CanhBao.Kieu.OK);
				}
				if (num5 == 17 && CurGame != null)
				{
					TrieuTapNhom();
					CanhBao.Msg("Thiết Lập Thành Công", "Bắt đầu triệu tập nhóm", CanhBao.Kieu.OK);
				}
				if (num5 == 28 && CurGame != null)
				{
					TrieuTap(CurGame);
					CanhBao.Msg("Thiết Lập Thành Công", "Bắt đầu triệu tập về nhân vật :" + CurGame.TLBB.Name, CanhBao.Kieu.OK);
				}
				if (num5 == 21 && CurGame != null)
				{
					CurGame.IsDropItem = !CurGame.IsDropItem;
					CanhBao.Msg("Thiết Lập Thành Công", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (CurGame.IsDropItem ? " Bật " : " Tắt ") + "hủy vật phẩm", CanhBao.Kieu.OK);
				}
				if (num5 == 23 && CurGame != null)
				{
					if (CurGame.IsRide)
					{
						CurGame.DownRide();
						CanhBao.Msg("Thao Tác Thành Công", "Xuống ngựa", CanhBao.Kieu.OK);
					}
					else
					{
						CurGame.Ride();
						CanhBao.Msg("Thao Tác Thành Công", "Lên Ngựa", CanhBao.Kieu.OK);
					}
				}
				if (num5 == 20 && CurGame != null)
				{
					CurGame.IsBank = !CurGame.IsBank;
					CanhBao.Msg("Bắt đầu đi cất đồ", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (CurGame.IsBank ? " Bật " : " Tắt ") + "cất đồ", CanhBao.Kieu.OK);
				}
				if (num5 == 27 && CurGame != null)
				{
					CurGame.Exit();
					CanhBao.Msg("Thành Công", "[" + CurGame.TLBB.Name.ToUpper() + "] thoát game", CanhBao.Kieu.OK);
				}
				if (num5 == 29 && CurGame != null && CurGame.TLBB.Name != "ĐăngNhập")
				{
					if (CurGame.TLBB.IsLeader)
					{
						CanhBao.Msg("Mời Đội", "Mời vào đội của :" + CurGame.TLBB.Name, CanhBao.Kieu.OK);
						MoiDoi(CurGame);
						return;
					}
					CurGame.LUA.PlayerCreateTeamSelf();
				}
				if (num5 == 25 && CurGame != null)
				{
					CurGame.IsSellItem = !CurGame.IsSellItem;
					CanhBao.Msg("Thiết Lập Thành Công", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (CurGame.IsSellItem ? " Bật " : " Tắt ") + "bán vật phẩm", CanhBao.Kieu.OK);
				}
				if (num5 == 35 && CurGame != null)
				{
					if (!CurGame.IsMapNghe())
					{
						CanhBao.Msg("Di Chuyển", "Vui lòng di chuyển tới bản đồ phù hợp", CanhBao.Kieu.Eror);
						return;
					}
					CurGame.IsDuoc = !CurGame.IsDuoc;
					CanhBao.Msg("Thiết Lập Thành Công", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (CurGame.IsDuoc ? " Bật " : " Tắt ") + "hái dược", CanhBao.Kieu.OK);
				}
				if (num5 == 13 && CurGame != null)
				{
					CurGame.UseSkill(22);
					CanhBao.Msg("Hành Động", "Quay về đại lý", CanhBao.Kieu.OK);
				}
				if (num5 == 39)
				{
					foreach (KeyValuePair<int, Game> item3 in dicGame)
					{
						Game value = item3.Value;
						value.IsAuto = !value.IsAuto;
					}
				}
				if (num5 == 24 && CurGame != null)
				{
					CurGame.IsMoBTD = !CurGame.IsMoBTD;
					CanhBao.Msg("Thiết Lập Thành Công", "[" + CurGame.TLBB.Name.ToUpper() + "]" + (CurGame.IsMoBTD ? " Bật " : " Tắt ") + "mở tàng bảo đồ", CanhBao.Kieu.OK);
				}
				if (num5 == 32)
				{
					if (TienIch.GameCount() > 3)
					{
						CanhBao.Msg("Lỗi Mở Game", "Đã mở quá giới hạn Client cho phép", CanhBao.Kieu.Eror);
						return;
					}
					string text3 = LoadFile.LoadFileWithDecrypt(Global.DataPath + "\\ExecutePath.dat");
					try
					{
						Path.GetFileNameWithoutExtension(text3);
						string mD = Offset.MD51.ToLower();
						Process.Start(new ProcessStartInfo
						{
							FileName = text3,
							Arguments = ".\\Bin\\Game.exe " + GetCMDBYMD5(mD),
							WorkingDirectory = Path.GetDirectoryName(text3)
						});
					}
					catch (Exception)
					{
						SettingPath();
						return;
					}
				}
			}
			base.WndProc(ref m);
		}

		private void MoiDoi(Game foreGame)
		{
			if (!foreGame.TLBB.Online)
			{
				return;
			}
			if (!foreGame.TLBB.IsLeader)
			{
				foreGame.LUA.PlayerCreateTeamSelf();
			}
			foreach (KeyValuePair<int, Game> item in dicGame)
			{
				Game value = item.Value;
				if (value != foreGame && value.TLBB.Online && value.Objects.Self != null && value.Objects.Self.PartyId == -1)
				{
					value.LuaDoUnicodeString("Friend:AskTeam(\"" + foreGame.TLBB.Name + "\");");
				}
			}
			foreGame.IsAcceptAll = true;
		}

		private void TrieuTap(Game foreGame)
		{
			if (foreGame == null)
			{
				return;
			}
			foreach (KeyValuePair<int, Game> item in dicGame)
			{
				item.Value.Move((int)foreGame.CharX, (int)foreGame.CharY, foreGame.TLBB.MapId);
			}
		}

		private void TrieuTapNhom()
		{
			foreach (KeyValuePair<int, Game> item in dicGame)
			{
				Game value = item.Value;
				if (!value.TLBB.IsLeader)
				{
					continue;
				}
				foreach (KeyValuePair<int, Game> item2 in dicGame)
				{
					Game value2 = item2.Value;
					if (value2 != value && value2.TLBB.Online && value2.TLBB.KeyId == value.TLBB.Id)
					{
						value2.Move((int)value.CharX, (int)value.CharY, value.TLBB.MapId);
					}
				}
			}
		}

		private void ListViewNhanVat_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			Game game = e.Item.Tag as Game;
			game.IsAuto = e.Item.Checked;
			if (!VuaBatXong())
			{
				notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + game.TLBB.Name.ToUpper() + "]" + (game.IsAuto ? " Bật " : " Tắt ") + "Auto", ToolTipIcon.Info);
			}
			game.SaveSetting();
		}

		private void comcapdtd_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.CheCap = comcapdtd.SelectedIndex;
			}
		}

		private void comboloai_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.CheLoai = comboloai.SelectedIndex;
			}
		}

		private bool TryGetDungeonContext(out Game selectedGame, out Game leader)
		{
			selectedGame = CurGame;
			leader = null;
			if (selectedGame == null || selectedGame.TLBB == null)
			{
				return false;
			}
			leader = selectedGame.Leader;
			if (!ReferenceEquals(CurGame, selectedGame) || leader == null || leader.TLBB == null)
			{
				leader = null;
				return false;
			}
			return true;
		}

		private bool RequireDungeonContext(string title, out Game selectedGame, out Game leader)
		{
			if (TryGetDungeonContext(out selectedGame, out leader))
			{
				return true;
			}
			UpdateDungeonMenu(selectedGame, null);
			CanhBao.Msg(title, "Cần chọn nhân vật đã có tổ đội", CanhBao.Kieu.Eror);
			return false;
		}

		private void UpdateDungeonMenu(Game selectedGame, Game leader)
		{
			bool available = selectedGame != null && selectedGame.TLBB != null && leader != null && leader.TLBB != null;
			menuactac.Enabled = available;
			ItemAcBa.Enabled = available;
			ItemLauLan.Enabled = available;
			itemTranLongKyCuoc.Enabled = available;
			ItemThuyLao.Enabled = available;
			ItemTrungAc.Enabled = selectedGame != null && selectedGame == CurGame && selectedGame.TLBB != null;
			menuactac.Checked = false;
			ItemAcBa.Checked = available && leader.IsAcBa;
			ItemLauLan.Checked = available && leader.IsLauLanTamBao;
			itemTranLongKyCuoc.Checked = available && leader.IsKyCuoc;
			ItemThuyLao.Checked = available && leader.IsThuyLao;
			ItemTrungAc.Checked = selectedGame != null && selectedGame == CurGame && selectedGame.TLBB != null && selectedGame.IsTrungAc;
			UnCheckAllAcTac();
			if (available)
			{
				itemchuacodoi.Text = "Đội Trưởng [ " + (leader.TLBB.Name ?? string.Empty) + "]";
				itemchuacodoi.ForeColor = Color.Green;
				vôLượngSơnToolStripMenuItem.Checked = leader.MapAcTac == MAP.VoLuongSon;
				kínhHồToolStripMenuItem.Checked = leader.MapAcTac == MAP.KinhHo;
				kiếmCácToolStripMenuItem.Checked = leader.MapAcTac == MAP.KiemCac;
				tháiHồToolStripMenuItem.Checked = leader.MapAcTac == MAP.ThaiHo;
				tungSơnToolStripMenuItem.Checked = leader.MapAcTac == MAP.TungSon;
				đônHoàngToolStripMenuItem.Checked = leader.MapAcTac == MAP.DonHoang;
			}
			else
			{
				itemchuacodoi.Text = selectedGame == null ? "Vui lòng chọn nhân vật" : "Chưa nhận diện đội trưởng";
				itemchuacodoi.ForeColor = SystemColors.ControlText;
			}
		}

		private static void ResetDungeonActions(Game leader)
		{
			leader.IsAcBa = false;
			leader.IsTrungAc = false;
			leader.IsLauLanTamBao = false;
			leader.IsKyCuoc = false;
			leader.IsThuyLao = false;
			leader.MapAcTac = 0;
		}

		private void SelectAcTacMap(int map)
		{
			if (!RequireDungeonContext("Lỗi Ác Tặc", out Game selectedGame, out Game leader))
			{
				return;
			}
			foreach (KeyValuePair<int, Game> item in dicGame)
			{
				Game member = item.Value;
				if (member != null && member.TLBB != null && (member == leader || member.TLBB.KeyId == leader.TLBB.Id))
					member.IsTrieuTap = false;
			}
			ResetDungeonActions(leader);
			leader.MapAcTac = map;
			UpdateDungeonMenu(selectedGame, leader);
		}

		private void tựĐộngToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SelectAcTacMap(Game.ChooseAcTacMap());
		}

		private void vôLượngSơnToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SelectAcTacMap(MAP.VoLuongSon);
		}

		private void kínhHồToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SelectAcTacMap(MAP.KinhHo);
		}

		private void kiếmCácToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SelectAcTacMap(MAP.KiemCac);
		}

		private void tháiHồToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SelectAcTacMap(MAP.ThaiHo);
		}

		private void tungSơnToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SelectAcTacMap(MAP.TungSon);
		}

		private void đônHoàngToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SelectAcTacMap(MAP.DonHoang);
		}

		public void CallALLAction()
		{
			if (TryGetDungeonContext(out Game selectedGame, out Game leader))
			{
				ResetDungeonActions(leader);
			}
			UpdateDungeonMenu(selectedGame, leader);
		}

		private void ItemAcBa_Click(object sender, EventArgs e)
		{
			if (!RequireDungeonContext("Lỗi Ác Bá", out Game selectedGame, out Game leader))
			{
				return;
			}
			bool enableAcBa = !leader.IsAcBa;
			ResetDungeonActions(leader);
			leader.IsAcBa = enableAcBa;
			UpdateDungeonMenu(selectedGame, leader);
			notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + (selectedGame.TLBB.Name ?? string.Empty).ToUpper() + "]" + (leader.IsAcBa ? " Bật " : " Tắt ") + "ÁC BÁ", ToolTipIcon.Info);
		}

		private void itemTranLongKyCuoc_Click(object sender, EventArgs e)
		{
			if (!RequireDungeonContext("Lỗi Kỳ Cuộc", out Game selectedGame, out Game leader))
			{
				return;
			}
			leader.IsKyCuoc = !leader.IsKyCuoc;
			UpdateDungeonMenu(selectedGame, leader);
			notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + (selectedGame.TLBB.Name ?? string.Empty).ToUpper() + "]" + (leader.IsKyCuoc ? " Bật " : " Tắt ") + "Auto Trân Long Kỳ Cuộc", ToolTipIcon.Info);
		}

		private void ItemThuyLao_Click(object sender, EventArgs e)
		{
			if (!RequireDungeonContext("Lỗi Thủy Lao", out Game selectedGame, out Game leader))
			{
				return;
			}
			leader.IsThuyLao = false;
			UpdateDungeonMenu(selectedGame, leader);
			CanhBao.Msg("Lỗi Thủy Lao", "Not work", CanhBao.Kieu.Eror);
		}

		private void ItemTrungAc_Click(object sender, EventArgs e)
		{
			Game selectedGame = CurGame;
			if (selectedGame == null || selectedGame.TLBB == null)
			{
				UpdateDungeonMenu(selectedGame, null);
				CanhBao.Msg("Lỗi Trừng Ác", "Cần chọn nhân vật có dữ liệu game", CanhBao.Kieu.Eror);
				return;
			}
			selectedGame.IsTrungAc = !selectedGame.IsTrungAc;
			ItemTrungAc.Checked = selectedGame.IsTrungAc;
			ItemTrungAc.Enabled = true;
			notifyIcon1.ShowBalloonTip(2000, "Thông Báo", "[" + (selectedGame.TLBB.Name ?? string.Empty).ToUpper() + "]" + (selectedGame.IsTrungAc ? " Bật " : " Tắt ") + "Auto Trừng Ác", ToolTipIcon.Info);
		}

		private void chươngTrìnhToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TryGetDungeonContext(out Game selectedGame, out Game leader);
			UpdateDungeonMenu(selectedGame, leader);
		}

		private void phímTắtToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new HotKey().Show();
		}

		private void menuactac_Click(object sender, EventArgs e)
		{
			TryGetDungeonContext(out Game selectedGame, out Game leader);
			UpdateDungeonMenu(selectedGame, leader);
		}

		public void UnCheckAllAcTac()
		{
			tựĐộngToolStripMenuItem.Checked = false;
			vôLượngSơnToolStripMenuItem.Checked = false;
			kínhHồToolStripMenuItem.Checked = false;
			kiếmCácToolStripMenuItem.Checked = false;
			tháiHồToolStripMenuItem.Checked = false;
			tungSơnToolStripMenuItem.Checked = false;
			đônHoàngToolStripMenuItem.Checked = false;
		}

		private void dUwngfToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!RequireDungeonContext("Lỗi Ác Tặc", out Game selectedGame, out Game leader))
			{
				return;
			}
			leader.MapAcTac = 0;
			UpdateDungeonMenu(selectedGame, leader);
			CanhBao.Msg("Hủy Ác Tặc", "Hủy Ác Tặc Thành Công", CanhBao.Kieu.OK);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			foreach (KeyValuePair<int, Game> item in dicGame)
			{
				Game value = item.Value;
				value.IsAuto = !value.IsAuto;
			}
		}

		private void buttrieutap_Click(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				TrieuTapNhom();
				CanhBao.Msg("Thiết Lập Thành Công", "Bắt đầu triệu tập nhóm", CanhBao.Kieu.OK);
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			new Debug(CurGame).Show();
		}

		private void ListViewNhanVat_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right && ListViewNhanVat.FocusedItem.Bounds.Contains(e.Location))
			{
				contextMenuStrip1.Show(Cursor.Position);
			}
		}

		private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
		{
			if (CurGame != null)
			{
				itemresetauto.Text = "Rest Auto [" + CurGame.TLBB.Name + "]";
			}
		}

		private void ẩnGameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.Hide();
			}
		}

		private void hiệnGameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				CurGame.Active();
			}
		}

		private void itemresetauto_Click(object sender, EventArgs e)
		{
			foreach (Game selectedGame in SelectedGames)
			{
				selectedGame.UnHookRecv();
				dicGame.Remove(selectedGame.ProcessId);
				selectedGame.Item.Remove();
			}
		}

		private void mởThêmGameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string text = LoadFile.LoadFileWithDecrypt(Global.DataPath + "\\ExecutePath.dat");
			try
			{
				Path.GetFileNameWithoutExtension(text);
				string mD = Offset.MD51.ToLower();
				Process.Start(new ProcessStartInfo
				{
					FileName = text,
					Arguments = ".\\Bin\\Game.exe " + GetCMDBYMD5(mD),
					WorkingDirectory = Path.GetDirectoryName(text)
				});
			}
			catch (Exception)
			{
				SettingPath();
			}
		}

		private void thiếtLậpAutoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new ThietLapAuto().Show();
		}

		private void thôngTinCậpNhậtToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new ChangeLogs().Show();
		}

		private void button2_Click_1(object sender, EventArgs e)
		{
			string s = "Ðây là bµ gõ Cp1252, nhìn nó s\u00a8 nhß thª này";
			MessageBox.Show(ConverterEx.VISCII2UnicodeEx(Encoding.Default.GetBytes(s)));
		}

		private void button2_Click_2(object sender, EventArgs e)
		{
			string s = ConverterEx.Unicode2VISCII("Ỷ Thiên Đồ Long Kiếm");
			MessageBox.Show(ConverterEx.VISCII2UnicodeEx(Encoding.Default.GetBytes(s)));
		}

		private void button4_Click(object sender, EventArgs e)
		{
			if (CurGame != null)
			{
				if (cboXuatPet.SelectedItem.ToString() == "Không Xuất")
				{
					CurGame.DoAction("PetSkill2_2");
				}
				else
				{
					CurGame.LuaDoOneLineString("XuatPet('" + CurGame.PetId + "')");
				}
			}
		}

		private void button2_Click_3(object sender, EventArgs e)
		{
			CurGame.Quit();
		}

		private void button2_Click_4(object sender, EventArgs e)
		{
			new Debug(CurGame).Show();
		}

		private void button2_Click_5(object sender, EventArgs e)
		{
		}

		private void button2_Click_6(object sender, EventArgs e)
		{
			new Debug(CurGame).Show();
		}

		public void SendData()
		{
			try
			{
				Dictionary<string, AutoReport> dictionary = new Dictionary<string, AutoReport>();
				PacketSend packetSend = new PacketSend();
				packetSend.HardwareID = Class95.String_0;
				KeyValuePair<int, Game>[] array = dicGame.ToArray();
				foreach (KeyValuePair<int, Game> keyValuePair in array)
				{
					AutoReport autoReport = Global.CreateFromGame(keyValuePair.Value);
					if (!dictionary.ContainsKey(autoReport.CharID))
					{
						dictionary.Add(autoReport.CharID, autoReport);
					}
				}
				packetSend.DanhSachGame = dictionary;
				PacketDef packet = new PacketDef();
				packet.IDPacket = 1000;
				packet.data = DataHelper.ObjectToBytes(packetSend);
				Thread thread = new Thread((ThreadStart)delegate
				{
					SendData(DataHelper.ObjectToBytes(packet));
				});
				thread.IsBackground = true;
				thread.Start();
			}
			catch
			{
			}
		}

		private void ChacterReport_Tick(object sender, EventArgs e)
		{
		}

		public static byte[] BuildMessage(byte[] data)
		{
			byte[] array = DataHelper.MAHOA(data, "e9b3390206d8dfc5ffc9b09284c0bbde");
			byte[] bytes = BitConverter.GetBytes(array.Length);
			byte[] array2 = new byte[bytes.Length + array.Length];
			bytes.CopyTo(array2, 0);
			array.CopyTo(array2, bytes.Length);
			return array2;
		}

		public void SendData(byte[] message)
		{
			if (_SocketClient != null && _SocketClient.connected)
			{
				_SocketClient.Send(BuildMessage(message));
			}
			else if (!ServerConnect.IsBusy)
			{
				ServerConnect.RunWorkerAsync();
			}
		}

		private void ServerConnect_DoWork(object sender, DoWorkEventArgs e)
		{
		}

		private void thôngTinAUTOToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, AppBranding.AboutText, AppBranding.AboutTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void groupBox8_Enter(object sender, EventArgs e)
		{
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TinhKiemAuto.FrmMain));
			this.paneltop = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.button1 = new System.Windows.Forms.Button();
			this.buttrieutap = new System.Windows.Forms.Button();
			this.pictureBox5 = new System.Windows.Forms.PictureBox();
			this.pictureBox6 = new System.Windows.Forms.PictureBox();
			this.pictureBox3 = new System.Windows.Forms.PictureBox();
			this.buttheo = new System.Windows.Forms.PictureBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.unpickall = new System.Windows.Forms.PictureBox();
			this.butpickall = new System.Windows.Forms.PictureBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.txtpet = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.txthp = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.txtmp = new System.Windows.Forms.Label();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.tùyChọnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.phímTắtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ẩnAutoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.càiĐườngDẫnGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mởThêmGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.thiếtLậpAutoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.thôngTinCậpNhậtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.thôngTinAUTOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.chươngTrìnhToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuactac = new System.Windows.Forms.ToolStripMenuItem();
			this.tựĐộngToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.vôLượngSơnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.kínhHồToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.kiếmCácToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tháiHồToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tungSơnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.đônHoàngToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.dUwngfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ItemAcBa = new System.Windows.Forms.ToolStripMenuItem();
			this.ItemLauLan = new System.Windows.Forms.ToolStripMenuItem();
			this.itemTranLongKyCuoc = new System.Windows.Forms.ToolStripMenuItem();
			this.ItemThuyLao = new System.Windows.Forms.ToolStripMenuItem();
			this.ItemTrungAc = new System.Windows.Forms.ToolStripMenuItem();
			this.itemchuacodoi = new System.Windows.Forms.ToolStripMenuItem();
			this.panellistview = new System.Windows.Forms.Panel();
			this.ListViewNhanVat = new System.Windows.Forms.ListView();
			this.txtNhanVat = new System.Windows.Forms.ColumnHeader();
			this.panel1 = new System.Windows.Forms.Panel();
			this.txtlogs = new System.Windows.Forms.RichTextBox();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.txttrangthai = new System.Windows.Forms.ToolStripStatusLabel();
			this.TablControl = new System.Windows.Forms.TabControl();
			this.tabtophop = new System.Windows.Forms.TabPage();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label18 = new System.Windows.Forms.Label();
			this.CheckTriLieuComeback = new System.Windows.Forms.CheckBox();
			this.numcanhbaohp = new System.Windows.Forms.NumericUpDown();
			this.CheckAutoHoiSinh = new System.Windows.Forms.CheckBox();
			this.checkBox7 = new System.Windows.Forms.CheckBox();
			this.label17 = new System.Windows.Forms.Label();
			this.nudNM = new System.Windows.Forms.NumericUpDown();
			this.checkisNM = new System.Windows.Forms.CheckBox();
			this.label16 = new System.Windows.Forms.Label();
			this.numhuyettemp = new System.Windows.Forms.NumericUpDown();
			this.checkhuyette = new System.Windows.Forms.CheckBox();
			this.label15 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.numbercongsinhhp = new System.Windows.Forms.NumericUpDown();
			this.checkcongsinh = new System.Windows.Forms.CheckBox();
			this.nudMP = new System.Windows.Forms.NumericUpDown();
			this.checkrengenmp = new System.Windows.Forms.CheckBox();
			this.nudHP = new System.Windows.Forms.NumericUpDown();
			this.checkregenhp = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.checktholinhchau = new System.Windows.Forms.CheckBox();
			this.buttimbai = new System.Windows.Forms.Button();
			this.buttrilieu = new System.Windows.Forms.Button();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.txttoadoy = new System.Windows.Forms.TextBox();
			this.txttoadox = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.comlenbai = new System.Windows.Forms.ComboBox();
			this.butlenbai = new System.Windows.Forms.Button();
			this.label9 = new System.Windows.Forms.Label();
			this.comdanhsachbando = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radgom = new System.Windows.Forms.RadioButton();
			this.rad11 = new System.Windows.Forms.RadioButton();
			this.numberdanhquanh = new System.Windows.Forms.NumericUpDown();
			this.checkradius = new System.Windows.Forms.CheckBox();
			this.butboqua = new System.Windows.Forms.PictureBox();
			this.chekcdanhquai = new System.Windows.Forms.CheckBox();
			this.tabkynang = new System.Windows.Forms.TabPage();
			this.TabKyNangControl = new System.Windows.Forms.TabControl();
			this.tabdanhquai = new System.Windows.Forms.TabPage();
			this.label58 = new System.Windows.Forms.Label();
			this.label20 = new System.Windows.Forms.Label();
			this.butthemskilldanhquai = new System.Windows.Forms.PictureBox();
			this.label19 = new System.Windows.Forms.Label();
			this.comdanhsachdanhquai = new System.Windows.Forms.ComboBox();
			this.listViewSkill = new System.Windows.Forms.ListView();
			this.lbltenkynang = new System.Windows.Forms.ColumnHeader();
			this.tabbufffhotro = new System.Windows.Forms.TabPage();
			this.label59 = new System.Windows.Forms.Label();
			this.label21 = new System.Windows.Forms.Label();
			this.label22 = new System.Windows.Forms.Label();
			this.comskillhotro = new System.Windows.Forms.ComboBox();
			this.pictureBox10 = new System.Windows.Forms.PictureBox();
			this.listviewskillhotro = new System.Windows.Forms.ListView();
			this.tenkynang = new System.Windows.Forms.ColumnHeader();
			this.tabvatpham = new System.Windows.Forms.TabPage();
			this.label26 = new System.Windows.Forms.Label();
			this.label25 = new System.Windows.Forms.Label();
			this.label24 = new System.Windows.Forms.Label();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.butitemtuanhoan = new System.Windows.Forms.PictureBox();
			this.chekcautox2 = new System.Windows.Forms.CheckBox();
			this.checkautovutrac = new System.Windows.Forms.CheckBox();
			this.chekcusingitem = new System.Windows.Forms.CheckBox();
			this.checkautocatkho = new System.Windows.Forms.CheckBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.label23 = new System.Windows.Forms.Label();
			this.numrangerpickitem = new System.Windows.Forms.NumericUpDown();
			this.butbanvatpham = new System.Windows.Forms.PictureBox();
			this.pickbanvatpham = new System.Windows.Forms.CheckBox();
			this.butdanhsachhuy = new System.Windows.Forms.PictureBox();
			this.checkhuyitem = new System.Windows.Forms.CheckBox();
			this.checkpickitem = new System.Windows.Forms.CheckBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.CheckReGenPET = new System.Windows.Forms.CheckBox();
			this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
			this.CheckthuPet = new System.Windows.Forms.CheckBox();
			this.checkBox10 = new System.Windows.Forms.CheckBox();
			this.checkBox9 = new System.Windows.Forms.CheckBox();
			this.button4 = new System.Windows.Forms.Button();
			this.cboXuatPet = new System.Windows.Forms.ComboBox();
			this.AutoXuatPhet = new System.Windows.Forms.CheckBox();
			this.tabtienich = new System.Windows.Forms.TabPage();
			this.groupBox9 = new System.Windows.Forms.GroupBox();
			this.pictureBox12 = new System.Windows.Forms.PictureBox();
			this.button6 = new System.Windows.Forms.Button();
			this.label31 = new System.Windows.Forms.Label();
			this.txtmkkho = new System.Windows.Forms.TextBox();
			this.label30 = new System.Windows.Forms.Label();
			this.numbankinhtheosau = new System.Windows.Forms.NumericUpDown();
			this.label29 = new System.Windows.Forms.Label();
			this.groupBox8 = new System.Windows.Forms.GroupBox();
			this.Checkthongbaochatmat = new System.Windows.Forms.CheckBox();
			this.label28 = new System.Windows.Forms.Label();
			this.txtthoigian = new System.Windows.Forms.TextBox();
			this.label27 = new System.Windows.Forms.Label();
			this.pictureBox11 = new System.Windows.Forms.PictureBox();
			this.button5 = new System.Windows.Forms.Button();
			this.txtnoidunggiaochat = new System.Windows.Forms.RichTextBox();
			this.checkgiaochat = new System.Windows.Forms.CheckBox();
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.numuplevel = new System.Windows.Forms.NumericUpDown();
			this.checkauouplevel = new System.Windows.Forms.CheckBox();
			this.checkautoskillf1 = new System.Windows.Forms.CheckBox();
			this.checkdongytoanbo = new System.Windows.Forms.CheckBox();
			this.butdanhsachdongy = new System.Windows.Forms.PictureBox();
			this.checkdongytodoi = new System.Windows.Forms.CheckBox();
			this.tabchedo = new System.Windows.Forms.TabPage();
			this.butche = new System.Windows.Forms.Button();
			this.label49 = new System.Windows.Forms.Label();
			this.groupBox14 = new System.Windows.Forms.GroupBox();
			this.txtdahuy = new System.Windows.Forms.Label();
			this.label54 = new System.Windows.Forms.Label();
			this.txtdache = new System.Windows.Forms.Label();
			this.label53 = new System.Windows.Forms.Label();
			this.txttaodo = new System.Windows.Forms.Label();
			this.label52 = new System.Windows.Forms.Label();
			this.txtbingan = new System.Windows.Forms.Label();
			this.label51 = new System.Windows.Forms.Label();
			this.txtvaibong = new System.Windows.Forms.Label();
			this.label50 = new System.Windows.Forms.Label();
			this.txttinhthiet = new System.Windows.Forms.Label();
			this.label48 = new System.Windows.Forms.Label();
			this.groupBox11 = new System.Windows.Forms.GroupBox();
			this.comcapdtd = new System.Windows.Forms.ComboBox();
			this.label37 = new System.Windows.Forms.Label();
			this.comboloai = new System.Windows.Forms.ComboBox();
			this.label36 = new System.Windows.Forms.Label();
			this.groupBox10 = new System.Windows.Forms.GroupBox();
			this.groupBox13 = new System.Windows.Forms.GroupBox();
			this.label44 = new System.Windows.Forms.Label();
			this.label47 = new System.Windows.Forms.Label();
			this.label46 = new System.Windows.Forms.Label();
			this.label45 = new System.Windows.Forms.Label();
			this.label43 = new System.Windows.Forms.Label();
			this.label42 = new System.Windows.Forms.Label();
			this.numericUpDown6 = new System.Windows.Forms.NumericUpDown();
			this.label41 = new System.Windows.Forms.Label();
			this.numericUpDown5 = new System.Windows.Forms.NumericUpDown();
			this.label40 = new System.Windows.Forms.Label();
			this.numsosao = new System.Windows.Forms.NumericUpDown();
			this.label39 = new System.Windows.Forms.Label();
			this.comboBox3 = new System.Windows.Forms.ComboBox();
			this.label38 = new System.Windows.Forms.Label();
			this.checkBox13 = new System.Windows.Forms.CheckBox();
			this.groupBox12 = new System.Windows.Forms.GroupBox();
			this.radngoai = new System.Windows.Forms.RadioButton();
			this.radnoingoai = new System.Windows.Forms.RadioButton();
			this.Radnoi = new System.Windows.Forms.RadioButton();
			this.label34 = new System.Windows.Forms.Label();
			this.numericUpDown4 = new System.Windows.Forms.NumericUpDown();
			this.label35 = new System.Windows.Forms.Label();
			this.label33 = new System.Windows.Forms.Label();
			this.numsonguyenlieu = new System.Windows.Forms.NumericUpDown();
			this.label32 = new System.Windows.Forms.Label();
			this.checkBox12 = new System.Windows.Forms.CheckBox();
			this.tabautologin = new System.Windows.Forms.TabPage();
			this.button7 = new System.Windows.Forms.Button();
			this.ListViewLogin = new System.Windows.Forms.ListView();
			this.tennhanvat = new System.Windows.Forms.ColumnHeader();
			this.maychu = new System.Windows.Forms.ColumnHeader();
			this.trangthai = new System.Windows.Forms.ColumnHeader();
			this.nhanvat = new System.Windows.Forms.ColumnHeader();
			this.phai = new System.Windows.Forms.ColumnHeader();
			this.label57 = new System.Windows.Forms.Label();
			this.ComMayChu = new System.Windows.Forms.ComboBox();
			this.label56 = new System.Windows.Forms.Label();
			this.label55 = new System.Windows.Forms.Label();
			this.txtmk = new System.Windows.Forms.TextBox();
			this.txttk = new System.Windows.Forms.TextBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.timeMonitor = new System.Windows.Forms.Timer(this.components);
			this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
			this.tmrLogin = new System.Windows.Forms.Timer(this.components);
			this.AccountLogin = new System.ComponentModel.BackgroundWorker();
			this.tmrRefresh = new System.Windows.Forms.Timer(this.components);
			this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
			this.CheDoF5 = new System.Windows.Forms.Timer(this.components);
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.itemresetauto = new System.Windows.Forms.ToolStripMenuItem();
			this.ẩnGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.hiệnGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ChacterReport = new System.Windows.Forms.Timer(this.components);
			this.ServerConnect = new System.ComponentModel.BackgroundWorker();
			this.paneltop.SuspendLayout();
			this.panel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.pictureBox5).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.pictureBox6).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.pictureBox3).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.buttheo).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.pictureBox2).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.unpickall).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.butpickall).BeginInit();
			this.panel2.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.panellistview.SuspendLayout();
			this.panel1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.TablControl.SuspendLayout();
			this.tabtophop.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.numcanhbaohp).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.nudNM).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.numhuyettemp).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.numbercongsinhhp).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.nudMP).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.nudHP).BeginInit();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.numberdanhquanh).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.butboqua).BeginInit();
			this.tabkynang.SuspendLayout();
			this.TabKyNangControl.SuspendLayout();
			this.tabdanhquai.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.butthemskilldanhquai).BeginInit();
			this.tabbufffhotro.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.pictureBox10).BeginInit();
			this.tabvatpham.SuspendLayout();
			this.groupBox5.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.butitemtuanhoan).BeginInit();
			this.groupBox4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.numrangerpickitem).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.butbanvatpham).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.butdanhsachhuy).BeginInit();
			this.tabPage2.SuspendLayout();
			this.groupBox6.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.numericUpDown3).BeginInit();
			this.tabtienich.SuspendLayout();
			this.groupBox9.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.pictureBox12).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.numbankinhtheosau).BeginInit();
			this.groupBox8.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.pictureBox11).BeginInit();
			this.groupBox7.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.numuplevel).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.butdanhsachdongy).BeginInit();
			this.tabchedo.SuspendLayout();
			this.groupBox14.SuspendLayout();
			this.groupBox11.SuspendLayout();
			this.groupBox10.SuspendLayout();
			this.groupBox13.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.numericUpDown6).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.numericUpDown5).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.numsosao).BeginInit();
			this.groupBox12.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.numericUpDown4).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.numsonguyenlieu).BeginInit();
			this.tabautologin.SuspendLayout();
			this.contextMenuStrip1.SuspendLayout();
			base.SuspendLayout();
			this.paneltop.Controls.Add(this.panel3);
			this.paneltop.Controls.Add(this.panel2);
			this.paneltop.Controls.Add(this.menuStrip1);
			this.paneltop.Controls.Add(this.panellistview);
			this.paneltop.Dock = System.Windows.Forms.DockStyle.Top;
			this.paneltop.Location = new System.Drawing.Point(0, 0);
			this.paneltop.Name = "paneltop";
			this.paneltop.Size = new System.Drawing.Size(382, 168);
			this.paneltop.TabIndex = 0;
			this.panel3.BackColor = System.Drawing.Color.PaleGoldenrod;
			this.panel3.Controls.Add(this.button1);
			this.panel3.Controls.Add(this.buttrieutap);
			this.panel3.Controls.Add(this.pictureBox5);
			this.panel3.Controls.Add(this.pictureBox6);
			this.panel3.Controls.Add(this.pictureBox3);
			this.panel3.Controls.Add(this.buttheo);
			this.panel3.Controls.Add(this.pictureBox1);
			this.panel3.Controls.Add(this.pictureBox2);
			this.panel3.Controls.Add(this.unpickall);
			this.panel3.Controls.Add(this.butpickall);
			this.panel3.Controls.Add(this.label7);
			this.panel3.Controls.Add(this.label6);
			this.panel3.Controls.Add(this.label5);
			this.panel3.Controls.Add(this.label2);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(168, 54);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(214, 114);
			this.panel3.TabIndex = 12;
			this.toolTip1.SetToolTip(this.panel3, "Nhóm thiết lập dùng chung.\r\nCó tác dụng kích hoạt toàn bộ các nhân vật đang AUTO\r\n");
			this.panel3.Paint += new System.Windows.Forms.PaintEventHandler(panel3_Paint);
			this.button1.BackColor = System.Drawing.Color.Red;
			this.button1.Location = new System.Drawing.Point(123, 38);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(82, 23);
			this.button1.TabIndex = 15;
			this.button1.Text = "Ngừng/Chạy";
			this.toolTip1.SetToolTip(this.button1, "Ngừng AUTO cho toàn bộ nhân vật");
			this.button1.UseVisualStyleBackColor = false;
			this.button1.Click += new System.EventHandler(button1_Click);
			this.buttrieutap.BackColor = System.Drawing.Color.Orange;
			this.buttrieutap.Location = new System.Drawing.Point(123, 9);
			this.buttrieutap.Name = "buttrieutap";
			this.buttrieutap.Size = new System.Drawing.Size(82, 23);
			this.buttrieutap.TabIndex = 14;
			this.buttrieutap.Text = "Triệu Tập";
			this.toolTip1.SetToolTip(this.buttrieutap, "Triệu tập toàn bộ thành viên chạy ra KEY");
			this.buttrieutap.UseVisualStyleBackColor = false;
			this.buttrieutap.Click += new System.EventHandler(buttrieutap_Click);
			this.pictureBox5.Cursor = System.Windows.Forms.Cursors.Hand;
			this.pictureBox5.Image = TinhKiemAuto.Properties.Resources.Cancel_50px;
			this.pictureBox5.Location = new System.Drawing.Point(89, 85);
			this.pictureBox5.Name = "pictureBox5";
			this.pictureBox5.Size = new System.Drawing.Size(18, 18);
			this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox5.TabIndex = 13;
			this.pictureBox5.TabStop = false;
			this.toolTip1.SetToolTip(this.pictureBox5, "Tắt sử dụng Skill cho toàn bộ AUTO");
			this.pictureBox5.Click += new System.EventHandler(pictureBox5_Click);
			this.pictureBox6.Cursor = System.Windows.Forms.Cursors.Hand;
			this.pictureBox6.Image = TinhKiemAuto.Properties.Resources.Ok_50px;
			this.pictureBox6.Location = new System.Drawing.Point(57, 85);
			this.pictureBox6.Name = "pictureBox6";
			this.pictureBox6.Size = new System.Drawing.Size(18, 18);
			this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox6.TabIndex = 12;
			this.pictureBox6.TabStop = false;
			this.toolTip1.SetToolTip(this.pictureBox6, "Bật sử dụng Skill đánh quái cho cả AUTO");
			this.pictureBox6.Click += new System.EventHandler(pictureBox6_Click);
			this.pictureBox3.Cursor = System.Windows.Forms.Cursors.Hand;
			this.pictureBox3.Image = TinhKiemAuto.Properties.Resources.Cancel_50px;
			this.pictureBox3.Location = new System.Drawing.Point(89, 61);
			this.pictureBox3.Name = "pictureBox3";
			this.pictureBox3.Size = new System.Drawing.Size(18, 18);
			this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox3.TabIndex = 11;
			this.pictureBox3.TabStop = false;
			this.toolTip1.SetToolTip(this.pictureBox3, "Tắt theo Key cả AUTO");
			this.pictureBox3.Click += new System.EventHandler(pictureBox3_Click);
			this.buttheo.Cursor = System.Windows.Forms.Cursors.Hand;
			this.buttheo.Image = TinhKiemAuto.Properties.Resources.Ok_50px;
			this.buttheo.Location = new System.Drawing.Point(57, 61);
			this.buttheo.Name = "buttheo";
			this.buttheo.Size = new System.Drawing.Size(18, 18);
			this.buttheo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.buttheo.TabIndex = 10;
			this.buttheo.TabStop = false;
			this.toolTip1.SetToolTip(this.buttheo, "Bật theo Key cho cả AUTO");
			this.buttheo.Click += new System.EventHandler(buttheo_Click);
			this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
			this.pictureBox1.Image = TinhKiemAuto.Properties.Resources.Cancel_50px;
			this.pictureBox1.Location = new System.Drawing.Point(89, 35);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(18, 18);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 9;
			this.pictureBox1.TabStop = false;
			this.toolTip1.SetToolTip(this.pictureBox1, "Tắt tự xuất PET cho cả AUTO");
			this.pictureBox1.Click += new System.EventHandler(pictureBox1_Click);
			this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Hand;
			this.pictureBox2.Image = TinhKiemAuto.Properties.Resources.Ok_50px;
			this.pictureBox2.Location = new System.Drawing.Point(57, 35);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(18, 18);
			this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox2.TabIndex = 8;
			this.pictureBox2.TabStop = false;
			this.toolTip1.SetToolTip(this.pictureBox2, "Bật Xuất Pet cho toàn bộ AUTO\r\n");
			this.pictureBox2.Click += new System.EventHandler(pictureBox2_Click);
			this.unpickall.Cursor = System.Windows.Forms.Cursors.Hand;
			this.unpickall.Image = TinhKiemAuto.Properties.Resources.Cancel_50px;
			this.unpickall.Location = new System.Drawing.Point(89, 9);
			this.unpickall.Name = "unpickall";
			this.unpickall.Size = new System.Drawing.Size(18, 18);
			this.unpickall.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.unpickall.TabIndex = 7;
			this.unpickall.TabStop = false;
			this.toolTip1.SetToolTip(this.unpickall, "Tắt Nhặt Vật Phẩm toàn bộ AUTO\r\n");
			this.unpickall.Click += new System.EventHandler(unpickall_Click);
			this.butpickall.Cursor = System.Windows.Forms.Cursors.Hand;
			this.butpickall.Image = TinhKiemAuto.Properties.Resources.Ok_50px;
			this.butpickall.Location = new System.Drawing.Point(57, 9);
			this.butpickall.Name = "butpickall";
			this.butpickall.Size = new System.Drawing.Size(18, 18);
			this.butpickall.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.butpickall.TabIndex = 6;
			this.butpickall.TabStop = false;
			this.toolTip1.SetToolTip(this.butpickall, "Bật Nhặt Vật Phẩm toàn bộ AUTO\r\n");
			this.butpickall.Click += new System.EventHandler(butpickall_Click);
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(15, 89);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(31, 13);
			this.label7.TabIndex = 3;
			this.label7.Text = "Skills";
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(15, 64);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(32, 13);
			this.label6.TabIndex = 2;
			this.label6.Text = "Theo";
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(15, 38);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(26, 13);
			this.label5.TabIndex = 1;
			this.label5.Text = "Pet ";
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(14, 14);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(33, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Nhặt ";
			this.panel2.BackColor = System.Drawing.Color.Lavender;
			this.panel2.Controls.Add(this.txtpet);
			this.panel2.Controls.Add(this.label1);
			this.panel2.Controls.Add(this.txthp);
			this.panel2.Controls.Add(this.label3);
			this.panel2.Controls.Add(this.label4);
			this.panel2.Controls.Add(this.txtmp);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(168, 24);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(214, 30);
			this.panel2.TabIndex = 11;
			this.txtpet.AutoSize = true;
			this.txtpet.BackColor = System.Drawing.Color.DodgerBlue;
			this.txtpet.Location = new System.Drawing.Point(180, 8);
			this.txtpet.Name = "txtpet";
			this.txtpet.Size = new System.Drawing.Size(27, 13);
			this.txtpet.TabIndex = 8;
			this.txtpet.Text = "10%";
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(28, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "HP :";
			this.txthp.AutoSize = true;
			this.txthp.BackColor = System.Drawing.Color.LawnGreen;
			this.txthp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.txthp.Location = new System.Drawing.Point(41, 8);
			this.txthp.Name = "txthp";
			this.txthp.Size = new System.Drawing.Size(27, 13);
			this.txthp.TabIndex = 4;
			this.txthp.Text = "10%";
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(73, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(29, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "MP :";
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(146, 8);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(34, 13);
			this.label4.TabIndex = 7;
			this.label4.Text = "PET :";
			this.txtmp.AutoSize = true;
			this.txtmp.BackColor = System.Drawing.Color.OrangeRed;
			this.txtmp.Location = new System.Drawing.Point(107, 8);
			this.txtmp.Name = "txtmp";
			this.txtmp.Size = new System.Drawing.Size(27, 13);
			this.txtmp.TabIndex = 6;
			this.txtmp.Text = "10%";
			this.menuStrip1.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.tùyChọnToolStripMenuItem, this.chươngTrìnhToolStripMenuItem });
			this.menuStrip1.Location = new System.Drawing.Point(168, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
			this.menuStrip1.Size = new System.Drawing.Size(214, 24);
			this.menuStrip1.TabIndex = 2;
			this.menuStrip1.Text = "menuStrip1";
			this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(menuStrip1_ItemClicked);
			this.tùyChọnToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[7] { this.phímTắtToolStripMenuItem, this.ẩnAutoToolStripMenuItem, this.càiĐườngDẫnGameToolStripMenuItem, this.mởThêmGameToolStripMenuItem, this.thiếtLậpAutoToolStripMenuItem, this.thôngTinCậpNhậtToolStripMenuItem, this.thôngTinAUTOToolStripMenuItem });
			this.tùyChọnToolStripMenuItem.Name = "tùyChọnToolStripMenuItem";
			this.tùyChọnToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
			this.tùyChọnToolStripMenuItem.Text = "Tiện Ích";
			this.phímTắtToolStripMenuItem.Name = "phímTắtToolStripMenuItem";
			this.phímTắtToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
			this.phímTắtToolStripMenuItem.Text = "Hệ Thống Phím Tắt";
			this.phímTắtToolStripMenuItem.Click += new System.EventHandler(phímTắtToolStripMenuItem_Click);
			this.ẩnAutoToolStripMenuItem.Name = "ẩnAutoToolStripMenuItem";
			this.ẩnAutoToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
			this.ẩnAutoToolStripMenuItem.Text = "Ẩn Auto";
			this.ẩnAutoToolStripMenuItem.Click += new System.EventHandler(ẩnAutoToolStripMenuItem_Click);
			this.càiĐườngDẫnGameToolStripMenuItem.Name = "càiĐườngDẫnGameToolStripMenuItem";
			this.càiĐườngDẫnGameToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
			this.càiĐườngDẫnGameToolStripMenuItem.Text = "Cài Đường Dẫn Game";
			this.càiĐườngDẫnGameToolStripMenuItem.Click += new System.EventHandler(càiĐườngDẫnGameToolStripMenuItem_Click);
			this.mởThêmGameToolStripMenuItem.Name = "mởThêmGameToolStripMenuItem";
			this.mởThêmGameToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
			this.mởThêmGameToolStripMenuItem.Text = "Mở Thêm Game [CTRL + O]";
			this.mởThêmGameToolStripMenuItem.Click += new System.EventHandler(mởThêmGameToolStripMenuItem_Click);
			this.thiếtLậpAutoToolStripMenuItem.Name = "thiếtLậpAutoToolStripMenuItem";
			this.thiếtLậpAutoToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
			this.thiếtLậpAutoToolStripMenuItem.Text = "Thiết Lập Auto";
			this.thiếtLậpAutoToolStripMenuItem.Click += new System.EventHandler(thiếtLậpAutoToolStripMenuItem_Click);
			this.thôngTinCậpNhậtToolStripMenuItem.Name = "thôngTinCậpNhậtToolStripMenuItem";
			this.thôngTinCậpNhậtToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
			this.thôngTinCậpNhậtToolStripMenuItem.Text = "Thông Tin Cập Nhật";
			this.thôngTinCậpNhậtToolStripMenuItem.Click += new System.EventHandler(thôngTinCậpNhậtToolStripMenuItem_Click);
			this.thôngTinAUTOToolStripMenuItem.Name = "thôngTinAUTOToolStripMenuItem";
			this.thôngTinAUTOToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
			this.thôngTinAUTOToolStripMenuItem.Text = AppBranding.AboutTitle;
			this.thôngTinAUTOToolStripMenuItem.Click += new System.EventHandler(thôngTinAUTOToolStripMenuItem_Click);
			this.chươngTrìnhToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[7] { this.menuactac, this.ItemAcBa, this.ItemLauLan, this.itemTranLongKyCuoc, this.ItemThuyLao, this.ItemTrungAc, this.itemchuacodoi });
			this.chươngTrìnhToolStripMenuItem.Name = "chươngTrìnhToolStripMenuItem";
			this.chươngTrìnhToolStripMenuItem.Size = new System.Drawing.Size(127, 20);
			this.chươngTrìnhToolStripMenuItem.Text = "Nhiệm Vụ - Phụ Bản";
			this.chươngTrìnhToolStripMenuItem.DropDownOpening += new System.EventHandler(chươngTrìnhToolStripMenuItem_Click);
			this.menuactac.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[8] { this.tựĐộngToolStripMenuItem, this.vôLượngSơnToolStripMenuItem, this.kínhHồToolStripMenuItem, this.kiếmCácToolStripMenuItem, this.tháiHồToolStripMenuItem, this.tungSơnToolStripMenuItem, this.đônHoàngToolStripMenuItem, this.dUwngfToolStripMenuItem });
			this.menuactac.Name = "menuactac";
			this.menuactac.Size = new System.Drawing.Size(172, 22);
			this.menuactac.Text = "Ác Tặc";
			this.menuactac.Click += new System.EventHandler(menuactac_Click);
			this.tựĐộngToolStripMenuItem.CheckOnClick = true;
			this.tựĐộngToolStripMenuItem.Name = "tựĐộngToolStripMenuItem";
			this.tựĐộngToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.tựĐộngToolStripMenuItem.Text = "Tự Động";
			this.tựĐộngToolStripMenuItem.Click += new System.EventHandler(tựĐộngToolStripMenuItem_Click);
			this.vôLượngSơnToolStripMenuItem.CheckOnClick = true;
			this.vôLượngSơnToolStripMenuItem.Name = "vôLượngSơnToolStripMenuItem";
			this.vôLượngSơnToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.vôLượngSơnToolStripMenuItem.Text = "Vô Lượng Sơn";
			this.vôLượngSơnToolStripMenuItem.Click += new System.EventHandler(vôLượngSơnToolStripMenuItem_Click);
			this.kínhHồToolStripMenuItem.CheckOnClick = true;
			this.kínhHồToolStripMenuItem.Name = "kínhHồToolStripMenuItem";
			this.kínhHồToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.kínhHồToolStripMenuItem.Text = "Kính Hồ";
			this.kínhHồToolStripMenuItem.Click += new System.EventHandler(kínhHồToolStripMenuItem_Click);
			this.kiếmCácToolStripMenuItem.CheckOnClick = true;
			this.kiếmCácToolStripMenuItem.Name = "kiếmCácToolStripMenuItem";
			this.kiếmCácToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.kiếmCácToolStripMenuItem.Text = "Kiếm Các";
			this.kiếmCácToolStripMenuItem.Click += new System.EventHandler(kiếmCácToolStripMenuItem_Click);
			this.tháiHồToolStripMenuItem.CheckOnClick = true;
			this.tháiHồToolStripMenuItem.Name = "tháiHồToolStripMenuItem";
			this.tháiHồToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.tháiHồToolStripMenuItem.Text = "Thái Hồ";
			this.tháiHồToolStripMenuItem.Click += new System.EventHandler(tháiHồToolStripMenuItem_Click);
			this.tungSơnToolStripMenuItem.CheckOnClick = true;
			this.tungSơnToolStripMenuItem.Name = "tungSơnToolStripMenuItem";
			this.tungSơnToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.tungSơnToolStripMenuItem.Text = "Tung Sơn";
			this.tungSơnToolStripMenuItem.Click += new System.EventHandler(tungSơnToolStripMenuItem_Click);
			this.đônHoàngToolStripMenuItem.CheckOnClick = true;
			this.đônHoàngToolStripMenuItem.Name = "đônHoàngToolStripMenuItem";
			this.đônHoàngToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.đônHoàngToolStripMenuItem.Text = "Đôn Hoàng";
			this.đônHoàngToolStripMenuItem.Click += new System.EventHandler(đônHoàngToolStripMenuItem_Click);
			this.dUwngfToolStripMenuItem.Name = "dUwngfToolStripMenuItem";
			this.dUwngfToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.dUwngfToolStripMenuItem.Text = "[Dừng Ác Tặc]";
			this.dUwngfToolStripMenuItem.Click += new System.EventHandler(dUwngfToolStripMenuItem_Click);
			this.ItemAcBa.CheckOnClick = false;
			this.ItemAcBa.Name = "ItemAcBa";
			this.ItemAcBa.Size = new System.Drawing.Size(172, 22);
			this.ItemAcBa.Text = "Ác Bá";
			this.ItemLauLan.CheckOnClick = true;
			this.ItemLauLan.Name = "ItemLauLan";
			this.ItemLauLan.Size = new System.Drawing.Size(172, 22);
			this.ItemLauLan.Text = "Lâu Lan Tầm Bảo";
			this.ItemLauLan.Click += new System.EventHandler(thoátToolStripMenuItem1_Click);
			this.itemTranLongKyCuoc.CheckOnClick = true;
			this.itemTranLongKyCuoc.Name = "itemTranLongKyCuoc";
			this.itemTranLongKyCuoc.Size = new System.Drawing.Size(172, 22);
			this.itemTranLongKyCuoc.Text = "Trân Long Kỳ Cuộc";
			this.itemTranLongKyCuoc.Click += new System.EventHandler(itemTranLongKyCuoc_Click);
			this.ItemThuyLao.CheckOnClick = true;
			this.ItemThuyLao.Enabled = false;
			this.ItemThuyLao.Name = "ItemThuyLao";
			this.ItemThuyLao.Size = new System.Drawing.Size(172, 22);
			this.ItemThuyLao.Text = "Thủy Lao";
			this.ItemThuyLao.Click += new System.EventHandler(ItemThuyLao_Click);
			this.ItemTrungAc.CheckOnClick = true;
			this.ItemTrungAc.Enabled = false;
			this.ItemTrungAc.Name = "ItemTrungAc";
			this.ItemTrungAc.Size = new System.Drawing.Size(172, 22);
			this.ItemTrungAc.Text = "Trừng Ác";
			this.ItemTrungAc.Click += new System.EventHandler(ItemTrungAc_Click);
			this.itemchuacodoi.Name = "itemchuacodoi";
			this.itemchuacodoi.Size = new System.Drawing.Size(172, 22);
			this.itemchuacodoi.Text = "Chưa Có Đội";
			this.panellistview.Controls.Add(this.ListViewNhanVat);
			this.panellistview.Dock = System.Windows.Forms.DockStyle.Left;
			this.panellistview.Location = new System.Drawing.Point(0, 0);
			this.panellistview.Name = "panellistview";
			this.panellistview.Size = new System.Drawing.Size(168, 168);
			this.panellistview.TabIndex = 0;
			this.ListViewNhanVat.BackColor = System.Drawing.SystemColors.InactiveBorder;
			this.ListViewNhanVat.CheckBoxes = true;
			this.ListViewNhanVat.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1] { this.txtNhanVat });
			this.ListViewNhanVat.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ListViewNhanVat.HideSelection = false;
			this.ListViewNhanVat.Location = new System.Drawing.Point(0, 0);
			this.ListViewNhanVat.Name = "ListViewNhanVat";
			this.ListViewNhanVat.Size = new System.Drawing.Size(168, 168);
			this.ListViewNhanVat.TabIndex = 0;
			this.ListViewNhanVat.UseCompatibleStateImageBehavior = false;
			this.ListViewNhanVat.View = System.Windows.Forms.View.Details;
			this.ListViewNhanVat.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(ListViewNhanVat_ItemChecked);
			this.ListViewNhanVat.SelectedIndexChanged += new System.EventHandler(listView1_SelectedIndexChanged);
			this.ListViewNhanVat.MouseClick += new System.Windows.Forms.MouseEventHandler(ListViewNhanVat_MouseClick);
			this.txtNhanVat.Text = "Tên Nhân Vật";
			this.txtNhanVat.Width = 162;
			this.panel1.Controls.Add(this.txtlogs);
			this.panel1.Controls.Add(this.statusStrip1);
			this.panel1.Controls.Add(this.TablControl);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 168);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(382, 431);
			this.panel1.TabIndex = 1;
			this.txtlogs.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
			this.txtlogs.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.txtlogs.Location = new System.Drawing.Point(0, 311);
			this.txtlogs.Name = "txtlogs";
			this.txtlogs.ReadOnly = true;
			this.txtlogs.Size = new System.Drawing.Size(382, 98);
			this.txtlogs.TabIndex = 1;
			this.txtlogs.Text = "";
			this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.txttrangthai });
			this.statusStrip1.Location = new System.Drawing.Point(0, 409);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(382, 22);
			this.statusStrip1.TabIndex = 0;
			this.statusStrip1.Text = "statusStrip1";
			this.txttrangthai.Name = "txttrangthai";
			this.txttrangthai.Size = new System.Drawing.Size(67, 17);
			this.txttrangthai.Text = "Trạng Thái :";
			this.TablControl.Controls.Add(this.tabtophop);
			this.TablControl.Controls.Add(this.tabkynang);
			this.TablControl.Controls.Add(this.tabvatpham);
			this.TablControl.Controls.Add(this.tabPage2);
			this.TablControl.Controls.Add(this.tabtienich);
			this.TablControl.Controls.Add(this.tabchedo);
			this.TablControl.Controls.Add(this.tabautologin);
			this.TablControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TablControl.ItemSize = new System.Drawing.Size(60, 25);
			this.TablControl.Location = new System.Drawing.Point(0, 0);
			this.TablControl.Name = "TablControl";
			this.TablControl.SelectedIndex = 0;
			this.TablControl.Size = new System.Drawing.Size(382, 431);
			this.TablControl.TabIndex = 0;
			this.TablControl.SelectedIndexChanged += new System.EventHandler(TablControl_SelectedIndexChanged);
			this.tabtophop.Controls.Add(this.groupBox3);
			this.tabtophop.Controls.Add(this.groupBox2);
			this.tabtophop.Controls.Add(this.groupBox1);
			this.tabtophop.Location = new System.Drawing.Point(4, 29);
			this.tabtophop.Name = "tabtophop";
			this.tabtophop.Padding = new System.Windows.Forms.Padding(3);
			this.tabtophop.Size = new System.Drawing.Size(374, 398);
			this.tabtophop.TabIndex = 0;
			this.tabtophop.Text = "Tổng Hợp";
			this.tabtophop.UseVisualStyleBackColor = true;
			this.groupBox3.Controls.Add(this.label18);
			this.groupBox3.Controls.Add(this.CheckTriLieuComeback);
			this.groupBox3.Controls.Add(this.numcanhbaohp);
			this.groupBox3.Controls.Add(this.CheckAutoHoiSinh);
			this.groupBox3.Controls.Add(this.checkBox7);
			this.groupBox3.Controls.Add(this.label17);
			this.groupBox3.Controls.Add(this.nudNM);
			this.groupBox3.Controls.Add(this.checkisNM);
			this.groupBox3.Controls.Add(this.label16);
			this.groupBox3.Controls.Add(this.numhuyettemp);
			this.groupBox3.Controls.Add(this.checkhuyette);
			this.groupBox3.Controls.Add(this.label15);
			this.groupBox3.Controls.Add(this.label13);
			this.groupBox3.Controls.Add(this.label14);
			this.groupBox3.Controls.Add(this.numbercongsinhhp);
			this.groupBox3.Controls.Add(this.checkcongsinh);
			this.groupBox3.Controls.Add(this.nudMP);
			this.groupBox3.Controls.Add(this.checkrengenmp);
			this.groupBox3.Controls.Add(this.nudHP);
			this.groupBox3.Controls.Add(this.checkregenhp);
			this.groupBox3.Location = new System.Drawing.Point(11, 176);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(358, 106);
			this.groupBox3.TabIndex = 2;
			this.groupBox3.TabStop = false;
			this.groupBox3.Enter += new System.EventHandler(groupBox3_Enter);
			this.label18.AutoSize = true;
			this.label18.Location = new System.Drawing.Point(296, 81);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(13, 13);
			this.label18.TabIndex = 27;
			this.label18.Text = "<";
			this.CheckTriLieuComeback.AutoSize = true;
			this.CheckTriLieuComeback.BackColor = System.Drawing.Color.DarkGray;
			this.CheckTriLieuComeback.Enabled = false;
			this.CheckTriLieuComeback.Location = new System.Drawing.Point(199, 57);
			this.CheckTriLieuComeback.Name = "CheckTriLieuComeback";
			this.CheckTriLieuComeback.Size = new System.Drawing.Size(138, 17);
			this.CheckTriLieuComeback.TabIndex = 26;
			this.CheckTriLieuComeback.Text = "Tự Trị Liệu Và Quay Lại";
			this.toolTip1.SetToolTip(this.CheckTriLieuComeback, "Khi tích vào đây nhân vật bị chết sẽ tự \r\nvề trị liệu và quay lại vị trí vừa chết\r\n");
			this.CheckTriLieuComeback.UseVisualStyleBackColor = false;
			this.CheckTriLieuComeback.CheckedChanged += new System.EventHandler(CheckTriLieuComeback_CheckedChanged);
			this.numcanhbaohp.BackColor = System.Drawing.Color.DarkGray;
			this.numcanhbaohp.Location = new System.Drawing.Point(313, 79);
			this.numcanhbaohp.Name = "numcanhbaohp";
			this.numcanhbaohp.Size = new System.Drawing.Size(39, 20);
			this.numcanhbaohp.TabIndex = 26;
			this.toolTip1.SetToolTip(this.numcanhbaohp, "Cảnh Báo HP\r\n-Khi Hp của bạn xuống thấp hơn mức\r\nthiết lập Auto sẽ đưa ra cảnh báo bằng\r\nÂm Thanh\r\n");
			this.numcanhbaohp.Value = new decimal(new int[4] { 20, 0, 0, 0 });
			this.numcanhbaohp.ValueChanged += new System.EventHandler(numericUpDown1_ValueChanged);
			this.CheckAutoHoiSinh.AutoSize = true;
			this.CheckAutoHoiSinh.Location = new System.Drawing.Point(199, 34);
			this.CheckAutoHoiSinh.Name = "CheckAutoHoiSinh";
			this.CheckAutoHoiSinh.Size = new System.Drawing.Size(125, 17);
			this.CheckAutoHoiSinh.TabIndex = 25;
			this.CheckAutoHoiSinh.Text = "Tự Hồi Sinh Khi Chết";
			this.toolTip1.SetToolTip(this.CheckAutoHoiSinh, "Khi click vào đây khi nhân vật chết\r\nsẽ tự hồi sinh");
			this.CheckAutoHoiSinh.UseVisualStyleBackColor = true;
			this.CheckAutoHoiSinh.CheckedChanged += new System.EventHandler(CheckAutoHoiSinh_CheckedChanged);
			this.checkBox7.AutoSize = true;
			this.checkBox7.BackColor = System.Drawing.Color.DarkGray;
			this.checkBox7.Location = new System.Drawing.Point(199, 80);
			this.checkBox7.Name = "checkBox7";
			this.checkBox7.Size = new System.Drawing.Size(91, 17);
			this.checkBox7.TabIndex = 25;
			this.checkBox7.Text = "Cảnh Báo HP";
			this.toolTip1.SetToolTip(this.checkBox7, "Cảnh Báo HP\r\n-Khi Hp của bạn xuống thấp hơn mức\r\nthiết lập Auto sẽ đưa ra cảnh báo bằng\r\nÂm Thanh");
			this.checkBox7.UseVisualStyleBackColor = false;
			this.checkBox7.CheckedChanged += new System.EventHandler(checkBox7_CheckedChanged);
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point(300, 13);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(13, 13);
			this.label17.TabIndex = 24;
			this.label17.Text = "<";
			this.nudNM.BackColor = System.Drawing.Color.DarkGray;
			this.nudNM.Location = new System.Drawing.Point(313, 11);
			this.nudNM.Name = "nudNM";
			this.nudNM.Size = new System.Drawing.Size(42, 20);
			this.nudNM.TabIndex = 23;
			this.toolTip1.SetToolTip(this.nudNM, "Chức năng chỉ sử dụng cho NM\r\n-Auto sẽ tự sử dụng skill Thanh Tâm Phổ Chiếu\r\nkhi máu của bản thân dưới mức cho phép.\r\n-Nếu ở phụ bản NM sẽ buff cả cho đồng đội của mình\r\n\r\n");
			this.nudNM.Value = new decimal(new int[4] { 60, 0, 0, 0 });
			this.nudNM.ValueChanged += new System.EventHandler(nudNM_ValueChanged);
			this.checkisNM.AutoSize = true;
			this.checkisNM.Location = new System.Drawing.Point(199, 13);
			this.checkisNM.Name = "checkisNM";
			this.checkisNM.Size = new System.Drawing.Size(83, 17);
			this.checkisNM.TabIndex = 22;
			this.checkisNM.Text = "Buff NM HP";
			this.toolTip1.SetToolTip(this.checkisNM, "Chức năng chỉ sử dụng cho NM\r\n-Auto sẽ tự sử dụng skill Thanh Tâm Phổ Chiếu\r\nkhi máu của bản thân dưới mức cho phép.\r\n-Nếu ở phụ bản NM sẽ buff cả cho đồng đội của mình\r\n");
			this.checkisNM.UseVisualStyleBackColor = true;
			this.checkisNM.CheckedChanged += new System.EventHandler(checkisNM_CheckedChanged);
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(104, 84);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(13, 13);
			this.label16.TabIndex = 21;
			this.label16.Text = "<";
			this.numhuyettemp.Location = new System.Drawing.Point(119, 82);
			this.numhuyettemp.Name = "numhuyettemp";
			this.numhuyettemp.Size = new System.Drawing.Size(46, 20);
			this.numhuyettemp.TabIndex = 20;
			this.toolTip1.SetToolTip(this.numhuyettemp, "Tự huyết tế.\r\nNếu PET của bạn hỗ trợ Huyết Tế.\r\nAuto sẽ tự huyết tế khi MP < mức cho phép");
			this.numhuyettemp.Value = new decimal(new int[4] { 40, 0, 0, 0 });
			this.numhuyettemp.ValueChanged += new System.EventHandler(numhuyettemp_ValueChanged);
			this.checkhuyette.AutoSize = true;
			this.checkhuyette.Location = new System.Drawing.Point(11, 83);
			this.checkhuyette.Name = "checkhuyette";
			this.checkhuyette.Size = new System.Drawing.Size(89, 17);
			this.checkhuyette.TabIndex = 19;
			this.checkhuyette.Text = "Huyết Tế MP";
			this.toolTip1.SetToolTip(this.checkhuyette, "Tự huyết tế.\r\nNếu PET của bạn hỗ trợ Huyết Tế.\r\nAuto sẽ tự huyết tế khi MP < mức cho phép\r\n");
			this.checkhuyette.UseVisualStyleBackColor = true;
			this.checkhuyette.CheckedChanged += new System.EventHandler(checkhuyette_CheckedChanged);
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(104, 61);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(13, 13);
			this.label15.TabIndex = 18;
			this.label15.Text = "<";
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(104, 36);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(13, 13);
			this.label13.TabIndex = 17;
			this.label13.Text = "<";
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(104, 13);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(13, 13);
			this.label14.TabIndex = 16;
			this.label14.Text = "<";
			this.numbercongsinhhp.Location = new System.Drawing.Point(119, 59);
			this.numbercongsinhhp.Name = "numbercongsinhhp";
			this.numbercongsinhhp.Size = new System.Drawing.Size(46, 20);
			this.numbercongsinhhp.TabIndex = 14;
			this.toolTip1.SetToolTip(this.numbercongsinhhp, "Tự cộng sinh.\r\nNếu PET của bạn hỗ trợ cộng Sinh.\r\nAuto sẽ tự cộng sinh khi HP < mức cho phép\r\n");
			this.numbercongsinhhp.Value = new decimal(new int[4] { 40, 0, 0, 0 });
			this.numbercongsinhhp.ValueChanged += new System.EventHandler(numbercongsinhhp_ValueChanged);
			this.checkcongsinh.AutoSize = true;
			this.checkcongsinh.Location = new System.Drawing.Point(11, 60);
			this.checkcongsinh.Name = "checkcongsinh";
			this.checkcongsinh.Size = new System.Drawing.Size(93, 17);
			this.checkcongsinh.TabIndex = 13;
			this.checkcongsinh.Text = "Cộng Sinh HP";
			this.toolTip1.SetToolTip(this.checkcongsinh, "Tự cộng sinh.\r\nNếu PET của bạn hỗ trợ cộng Sinh.\r\nAuto sẽ tự cộng sinh khi HP < mức cho phép");
			this.checkcongsinh.UseVisualStyleBackColor = true;
			this.checkcongsinh.CheckedChanged += new System.EventHandler(checkcongsinh_CheckedChanged);
			this.nudMP.BackColor = System.Drawing.Color.DarkGray;
			this.nudMP.Location = new System.Drawing.Point(119, 34);
			this.nudMP.Name = "nudMP";
			this.nudMP.Size = new System.Drawing.Size(46, 20);
			this.nudMP.TabIndex = 12;
			this.toolTip1.SetToolTip(this.nudMP, "Tự dùng mana ăn liền\r\n-Auto sẽ từ tìm mana để sử dụng.\r\n-Vật phẩm nào xếp trước sẽ ưu tiên dùng trước.\r\n\r\n");
			this.nudMP.Value = new decimal(new int[4] { 50, 0, 0, 0 });
			this.nudMP.ValueChanged += new System.EventHandler(nudMP_ValueChanged);
			this.checkrengenmp.AutoSize = true;
			this.checkrengenmp.Location = new System.Drawing.Point(11, 35);
			this.checkrengenmp.Name = "checkrengenmp";
			this.checkrengenmp.Size = new System.Drawing.Size(92, 17);
			this.checkrengenmp.TabIndex = 11;
			this.checkrengenmp.Text = "Phục Hồi MP ";
			this.toolTip1.SetToolTip(this.checkrengenmp, "Tự dùng mana ăn liền\r\n-Auto sẽ từ tìm mana để sử dụng.\r\n-Vật phẩm nào xếp trước sẽ ưu tiên dùng trước.\r\n");
			this.checkrengenmp.UseVisualStyleBackColor = true;
			this.checkrengenmp.CheckedChanged += new System.EventHandler(checkrengenmp_CheckedChanged);
			this.nudHP.BackColor = System.Drawing.Color.DarkGray;
			this.nudHP.Location = new System.Drawing.Point(120, 11);
			this.nudHP.Name = "nudHP";
			this.nudHP.Size = new System.Drawing.Size(45, 20);
			this.nudHP.TabIndex = 10;
			this.toolTip1.SetToolTip(this.nudHP, "Tự dùng máu ăn liền\r\n-Auto sẽ từ tìm máu để sử dụng.\r\n-Vật phẩm nào xếp trước sẽ ưu tiên dùng trước.\r\n");
			this.nudHP.Value = new decimal(new int[4] { 50, 0, 0, 0 });
			this.nudHP.ValueChanged += new System.EventHandler(nudHP_ValueChanged);
			this.checkregenhp.AutoSize = true;
			this.checkregenhp.Location = new System.Drawing.Point(11, 12);
			this.checkregenhp.Name = "checkregenhp";
			this.checkregenhp.Size = new System.Drawing.Size(91, 17);
			this.checkregenhp.TabIndex = 0;
			this.checkregenhp.Text = "Phục Hồi HP ";
			this.toolTip1.SetToolTip(this.checkregenhp, "Tự dùng máu ăn liền\r\n-Auto sẽ từ tìm máu để sử dụng.\r\n-Vật phẩm nào xếp trước sẽ ưu tiên dùng trước.");
			this.checkregenhp.UseVisualStyleBackColor = true;
			this.checkregenhp.CheckedChanged += new System.EventHandler(checkregenhp_CheckedChanged);
			this.groupBox2.Controls.Add(this.checktholinhchau);
			this.groupBox2.Controls.Add(this.buttimbai);
			this.groupBox2.Controls.Add(this.buttrilieu);
			this.groupBox2.Controls.Add(this.label12);
			this.groupBox2.Controls.Add(this.label11);
			this.groupBox2.Controls.Add(this.txttoadoy);
			this.groupBox2.Controls.Add(this.txttoadox);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.comlenbai);
			this.groupBox2.Controls.Add(this.butlenbai);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.comdanhsachbando);
			this.groupBox2.Location = new System.Drawing.Point(11, 60);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(358, 117);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Enter += new System.EventHandler(groupBox2_Enter);
			this.checktholinhchau.AutoSize = true;
			this.checktholinhchau.Location = new System.Drawing.Point(11, 95);
			this.checktholinhchau.Name = "checktholinhchau";
			this.checktholinhchau.Size = new System.Drawing.Size(213, 17);
			this.checktholinhchau.TabIndex = 25;
			this.checktholinhchau.Text = "Sử dụng thổ linh châu để lên bãi nhanh";
			this.toolTip1.SetToolTip(this.checktholinhchau, "Khi bạn tích vào đây.\r\nNếu trong túi đồ của bạn có Thổ Linh Châu giống \r\nbãi mà Auto định lên.\r\nAuto sẽ tự sử dụng.");
			this.checktholinhchau.UseVisualStyleBackColor = true;
			this.checktholinhchau.CheckedChanged += new System.EventHandler(checktholinhchau_CheckedChanged);
			this.buttimbai.BackColor = System.Drawing.Color.DimGray;
			this.buttimbai.Location = new System.Drawing.Point(273, 74);
			this.buttimbai.Name = "buttimbai";
			this.buttimbai.Size = new System.Drawing.Size(78, 23);
			this.buttimbai.TabIndex = 24;
			this.buttimbai.Text = "Tìm Bãi";
			this.toolTip1.SetToolTip(this.buttimbai, "Bấm vào đây auto sẽ tự tìm \r\nbãi train phù hợp với cấp độ\r\nđể Train");
			this.buttimbai.UseVisualStyleBackColor = false;
			this.buttimbai.Click += new System.EventHandler(buttimbai_Click);
			this.buttrilieu.BackColor = System.Drawing.Color.Olive;
			this.buttrilieu.Location = new System.Drawing.Point(273, 43);
			this.buttrilieu.Name = "buttrilieu";
			this.buttrilieu.Size = new System.Drawing.Size(78, 23);
			this.buttrilieu.TabIndex = 23;
			this.buttrilieu.Text = "Trị Liệu";
			this.toolTip1.SetToolTip(this.buttrilieu, "Bấm vào đây nhân vật sẽ tự chạy về\r\nthành để trị liệu");
			this.buttrilieu.UseVisualStyleBackColor = false;
			this.buttrilieu.Click += new System.EventHandler(buttrilieu_Click);
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(138, 72);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(19, 13);
			this.label12.TabIndex = 22;
			this.label12.Text = "Y |";
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(78, 72);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(19, 13);
			this.label11.TabIndex = 21;
			this.label11.Text = "X |";
			this.txttoadoy.Location = new System.Drawing.Point(159, 69);
			this.txttoadoy.Name = "txttoadoy";
			this.txttoadoy.Size = new System.Drawing.Size(37, 20);
			this.txttoadoy.TabIndex = 20;
			this.txttoadoy.Text = "0";
			this.toolTip1.SetToolTip(this.txttoadoy, "Nhập tọa độ Y của bãi");
			this.txttoadoy.TextChanged += new System.EventHandler(txttoadoy_TextChanged);
			this.txttoadox.Location = new System.Drawing.Point(98, 69);
			this.txttoadox.Name = "txttoadox";
			this.txttoadox.Size = new System.Drawing.Size(37, 20);
			this.txttoadox.TabIndex = 19;
			this.txttoadox.Text = "0";
			this.toolTip1.SetToolTip(this.txttoadox, "Nhập tọa độ X của bãi");
			this.txttoadox.TextChanged += new System.EventHandler(txttoadox_TextChanged);
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(15, 45);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(79, 13);
			this.label10.TabIndex = 18;
			this.label10.Text = "Danh Sách Bãi";
			this.comlenbai.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comlenbai.FormattingEnabled = true;
			this.comlenbai.Location = new System.Drawing.Point(98, 41);
			this.comlenbai.Name = "comlenbai";
			this.comlenbai.Size = new System.Drawing.Size(138, 21);
			this.comlenbai.TabIndex = 17;
			this.toolTip1.SetToolTip(this.comlenbai, "Chọn vị trí bãi muốn lên train");
			this.comlenbai.SelectedIndexChanged += new System.EventHandler(comlenbai_SelectedIndexChanged);
			this.butlenbai.BackColor = System.Drawing.Color.Tomato;
			this.butlenbai.Location = new System.Drawing.Point(274, 13);
			this.butlenbai.Name = "butlenbai";
			this.butlenbai.Size = new System.Drawing.Size(78, 23);
			this.butlenbai.TabIndex = 16;
			this.butlenbai.Text = "Lên Bãi";
			this.toolTip1.SetToolTip(this.butlenbai, "Bấm vào đây để lên bãi hoặc ngứng lên bãi");
			this.butlenbai.UseVisualStyleBackColor = false;
			this.butlenbai.Click += new System.EventHandler(butlenbai_Click);
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(15, 18);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(71, 13);
			this.label9.TabIndex = 4;
			this.label9.Text = "Chọn Bản Đồ";
			this.comdanhsachbando.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comdanhsachbando.FormattingEnabled = true;
			this.comdanhsachbando.Location = new System.Drawing.Point(98, 14);
			this.comdanhsachbando.Name = "comdanhsachbando";
			this.comdanhsachbando.Size = new System.Drawing.Size(164, 21);
			this.comdanhsachbando.TabIndex = 0;
			this.toolTip1.SetToolTip(this.comdanhsachbando, "Chọn bản đồ muốn lên bãi");
			this.comdanhsachbando.SelectedIndexChanged += new System.EventHandler(comdanhsachbando_SelectedIndexChanged);
			this.groupBox1.BackColor = System.Drawing.Color.Transparent;
			this.groupBox1.Controls.Add(this.radgom);
			this.groupBox1.Controls.Add(this.rad11);
			this.groupBox1.Controls.Add(this.numberdanhquanh);
			this.groupBox1.Controls.Add(this.checkradius);
			this.groupBox1.Controls.Add(this.butboqua);
			this.groupBox1.Controls.Add(this.chekcdanhquai);
			this.groupBox1.Location = new System.Drawing.Point(10, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(358, 54);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.radgom.AutoSize = true;
			this.radgom.Location = new System.Drawing.Point(63, 31);
			this.radgom.Name = "radgom";
			this.radgom.Size = new System.Drawing.Size(47, 17);
			this.radgom.TabIndex = 11;
			this.radgom.Text = "Gom";
			this.toolTip1.SetToolTip(this.radgom, "Chế độ Gom quái.\r\nAuto sẽ đánh mỗi con 1 hit rồi chuyển sang con khác.");
			this.radgom.UseVisualStyleBackColor = true;
			this.radgom.CheckedChanged += new System.EventHandler(radioButton2_CheckedChanged);
			this.rad11.AutoSize = true;
			this.rad11.Checked = true;
			this.rad11.Location = new System.Drawing.Point(17, 32);
			this.rad11.Name = "rad11";
			this.rad11.Size = new System.Drawing.Size(40, 17);
			this.rad11.TabIndex = 10;
			this.rad11.TabStop = true;
			this.rad11.Text = "1:1";
			this.toolTip1.SetToolTip(this.rad11, "Chế độ đánh từng con.\r\nAuto sẽ đánh chết quái rồi mới chuyển con khác");
			this.rad11.UseVisualStyleBackColor = true;
			this.rad11.CheckedChanged += new System.EventHandler(rad11_CheckedChanged);
			this.numberdanhquanh.BackColor = System.Drawing.Color.DarkGray;
			this.numberdanhquanh.Location = new System.Drawing.Point(307, 10);
			this.numberdanhquanh.Name = "numberdanhquanh";
			this.numberdanhquanh.Size = new System.Drawing.Size(45, 20);
			this.numberdanhquanh.TabIndex = 9;
			this.numberdanhquanh.ValueChanged += new System.EventHandler(numberdanhquanh_ValueChanged);
			this.checkradius.AutoSize = true;
			this.checkradius.Location = new System.Drawing.Point(193, 12);
			this.checkradius.Name = "checkradius";
			this.checkradius.Size = new System.Drawing.Size(113, 17);
			this.checkradius.TabIndex = 8;
			this.checkradius.Text = "Đánh Quanh (X,Y)";
			this.toolTip1.SetToolTip(this.checkradius, "Tích vào đây để bật tắt đánh theo bán kính.");
			this.checkradius.UseVisualStyleBackColor = true;
			this.checkradius.CheckedChanged += new System.EventHandler(checkradius_CheckedChanged);
			this.butboqua.BackColor = System.Drawing.Color.DarkGray;
			this.butboqua.Cursor = System.Windows.Forms.Cursors.Hand;
			this.butboqua.Image = TinhKiemAuto.Properties.Resources.List_50px;
			this.butboqua.Location = new System.Drawing.Point(95, 11);
			this.butboqua.Name = "butboqua";
			this.butboqua.Size = new System.Drawing.Size(18, 18);
			this.butboqua.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.butboqua.TabIndex = 7;
			this.butboqua.TabStop = false;
			this.toolTip1.SetToolTip(this.butboqua, "Danh sách quái sẽ bỏ qua.\r\nNhững quái vật có tên trong danh sách này\r\nAuto sẽ bỏ qua không đánh.\r\n");
			this.butboqua.Click += new System.EventHandler(butboqua_Click);
			this.chekcdanhquai.AutoSize = true;
			this.chekcdanhquai.Location = new System.Drawing.Point(17, 13);
			this.chekcdanhquai.Name = "chekcdanhquai";
			this.chekcdanhquai.Size = new System.Drawing.Size(77, 17);
			this.chekcdanhquai.TabIndex = 0;
			this.chekcdanhquai.Text = "Đánh Quái";
			this.toolTip1.SetToolTip(this.chekcdanhquai, "Tích vào đây để bật tắt đánh quái.");
			this.chekcdanhquai.UseVisualStyleBackColor = true;
			this.chekcdanhquai.CheckedChanged += new System.EventHandler(chekcdanhquai_CheckedChanged);
			this.tabkynang.Controls.Add(this.TabKyNangControl);
			this.tabkynang.Location = new System.Drawing.Point(4, 29);
			this.tabkynang.Name = "tabkynang";
			this.tabkynang.Padding = new System.Windows.Forms.Padding(3);
			this.tabkynang.Size = new System.Drawing.Size(374, 398);
			this.tabkynang.TabIndex = 1;
			this.tabkynang.Text = "Kỹ Năng";
			this.tabkynang.UseVisualStyleBackColor = true;
			this.TabKyNangControl.Alignment = System.Windows.Forms.TabAlignment.Bottom;
			this.TabKyNangControl.Controls.Add(this.tabdanhquai);
			this.TabKyNangControl.Controls.Add(this.tabbufffhotro);
			this.TabKyNangControl.Dock = System.Windows.Forms.DockStyle.Top;
			this.TabKyNangControl.ItemSize = new System.Drawing.Size(58, 25);
			this.TabKyNangControl.Location = new System.Drawing.Point(3, 3);
			this.TabKyNangControl.Name = "TabKyNangControl";
			this.TabKyNangControl.SelectedIndex = 0;
			this.TabKyNangControl.Size = new System.Drawing.Size(368, 275);
			this.TabKyNangControl.TabIndex = 0;
			this.tabdanhquai.Controls.Add(this.label58);
			this.tabdanhquai.Controls.Add(this.label20);
			this.tabdanhquai.Controls.Add(this.butthemskilldanhquai);
			this.tabdanhquai.Controls.Add(this.label19);
			this.tabdanhquai.Controls.Add(this.comdanhsachdanhquai);
			this.tabdanhquai.Controls.Add(this.listViewSkill);
			this.tabdanhquai.Location = new System.Drawing.Point(4, 4);
			this.tabdanhquai.Name = "tabdanhquai";
			this.tabdanhquai.Padding = new System.Windows.Forms.Padding(3);
			this.tabdanhquai.Size = new System.Drawing.Size(360, 242);
			this.tabdanhquai.TabIndex = 0;
			this.tabdanhquai.Text = "Đánh Quái";
			this.tabdanhquai.UseVisualStyleBackColor = true;
			this.tabdanhquai.Click += new System.EventHandler(tabdanhquai_Click);
			this.label58.AutoSize = true;
			this.label58.ForeColor = System.Drawing.Color.Red;
			this.label58.Location = new System.Drawing.Point(13, 216);
			this.label58.Name = "label58";
			this.label58.Size = new System.Drawing.Size(332, 13);
			this.label58.TabIndex = 11;
			this.label58.Text = "Nhấn Delete trên bàn phím để xóa skill bạn muốn loại bỏ khỏi AUTO";
			this.label20.AutoSize = true;
			this.label20.Location = new System.Drawing.Point(10, 71);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(176, 13);
			this.label20.TabIndex = 10;
			this.label20.Text = "Kỹ Năng Đang Sử Dụng Đánh Quái";
			this.butthemskilldanhquai.Cursor = System.Windows.Forms.Cursors.Hand;
			this.butthemskilldanhquai.Image = TinhKiemAuto.Properties.Resources.Ok_50px;
			this.butthemskilldanhquai.Location = new System.Drawing.Point(276, 32);
			this.butthemskilldanhquai.Name = "butthemskilldanhquai";
			this.butthemskilldanhquai.Size = new System.Drawing.Size(30, 30);
			this.butthemskilldanhquai.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.butthemskilldanhquai.TabIndex = 8;
			this.butthemskilldanhquai.TabStop = false;
			this.toolTip1.SetToolTip(this.butthemskilldanhquai, "Thêm Kỹ Năng");
			this.butthemskilldanhquai.Click += new System.EventHandler(butthemskilldanhquai_Click);
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(13, 18);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(149, 13);
			this.label19.TabIndex = 2;
			this.label19.Text = "Chọn Kỹ Năng Từ Danh Sách";
			this.comdanhsachdanhquai.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comdanhsachdanhquai.FormattingEnabled = true;
			this.comdanhsachdanhquai.Location = new System.Drawing.Point(13, 37);
			this.comdanhsachdanhquai.Name = "comdanhsachdanhquai";
			this.comdanhsachdanhquai.Size = new System.Drawing.Size(249, 21);
			this.comdanhsachdanhquai.TabIndex = 1;
			this.listViewSkill.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1] { this.lbltenkynang });
			this.listViewSkill.HideSelection = false;
			this.listViewSkill.Location = new System.Drawing.Point(13, 87);
			this.listViewSkill.Name = "listViewSkill";
			this.listViewSkill.Size = new System.Drawing.Size(309, 116);
			this.listViewSkill.TabIndex = 0;
			this.listViewSkill.UseCompatibleStateImageBehavior = false;
			this.listViewSkill.View = System.Windows.Forms.View.Details;
			this.listViewSkill.KeyDown += new System.Windows.Forms.KeyEventHandler(listViewSkill_KeyDown);
			this.lbltenkynang.Text = "Tên Kỹ Năng";
			this.lbltenkynang.Width = 300;
			this.tabbufffhotro.Controls.Add(this.label59);
			this.tabbufffhotro.Controls.Add(this.label21);
			this.tabbufffhotro.Controls.Add(this.label22);
			this.tabbufffhotro.Controls.Add(this.comskillhotro);
			this.tabbufffhotro.Controls.Add(this.pictureBox10);
			this.tabbufffhotro.Controls.Add(this.listviewskillhotro);
			this.tabbufffhotro.Location = new System.Drawing.Point(4, 4);
			this.tabbufffhotro.Name = "tabbufffhotro";
			this.tabbufffhotro.Padding = new System.Windows.Forms.Padding(3);
			this.tabbufffhotro.Size = new System.Drawing.Size(360, 242);
			this.tabbufffhotro.TabIndex = 1;
			this.tabbufffhotro.Text = "Hỗ Trợ";
			this.tabbufffhotro.UseVisualStyleBackColor = true;
			this.label59.AutoSize = true;
			this.label59.ForeColor = System.Drawing.Color.Red;
			this.label59.Location = new System.Drawing.Point(13, 216);
			this.label59.Name = "label59";
			this.label59.Size = new System.Drawing.Size(332, 13);
			this.label59.TabIndex = 17;
			this.label59.Text = "Nhấn Delete trên bàn phím để xóa skill bạn muốn loại bỏ khỏi AUTO";
			this.label21.AutoSize = true;
			this.label21.Location = new System.Drawing.Point(10, 71);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(170, 13);
			this.label21.TabIndex = 16;
			this.label21.Text = "Danh Sách Kỹ Năng Buff Hiện Tại";
			this.label22.AutoSize = true;
			this.label22.Location = new System.Drawing.Point(13, 18);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(149, 13);
			this.label22.TabIndex = 13;
			this.label22.Text = "Chọn Kỹ Năng Từ Danh Sách";
			this.comskillhotro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comskillhotro.FormattingEnabled = true;
			this.comskillhotro.Location = new System.Drawing.Point(13, 37);
			this.comskillhotro.Name = "comskillhotro";
			this.comskillhotro.Size = new System.Drawing.Size(249, 21);
			this.comskillhotro.TabIndex = 12;
			this.pictureBox10.Cursor = System.Windows.Forms.Cursors.Hand;
			this.pictureBox10.Image = TinhKiemAuto.Properties.Resources.Ok_50px;
			this.pictureBox10.Location = new System.Drawing.Point(276, 32);
			this.pictureBox10.Name = "pictureBox10";
			this.pictureBox10.Size = new System.Drawing.Size(30, 30);
			this.pictureBox10.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox10.TabIndex = 14;
			this.pictureBox10.TabStop = false;
			this.toolTip1.SetToolTip(this.pictureBox10, "Thêm Kỹ Năng");
			this.pictureBox10.Click += new System.EventHandler(pictureBox10_Click);
			this.listviewskillhotro.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1] { this.tenkynang });
			this.listviewskillhotro.HideSelection = false;
			this.listviewskillhotro.Location = new System.Drawing.Point(13, 87);
			this.listviewskillhotro.Name = "listviewskillhotro";
			this.listviewskillhotro.Size = new System.Drawing.Size(309, 116);
			this.listviewskillhotro.TabIndex = 11;
			this.listviewskillhotro.UseCompatibleStateImageBehavior = false;
			this.listviewskillhotro.View = System.Windows.Forms.View.Details;
			this.listviewskillhotro.KeyDown += new System.Windows.Forms.KeyEventHandler(listviewskillhotro_KeyDown);
			this.tenkynang.Text = "Tên Kỹ Năng";
			this.tenkynang.Width = 300;
			this.tabvatpham.Controls.Add(this.label26);
			this.tabvatpham.Controls.Add(this.label25);
			this.tabvatpham.Controls.Add(this.label24);
			this.tabvatpham.Controls.Add(this.groupBox5);
			this.tabvatpham.Controls.Add(this.groupBox4);
			this.tabvatpham.Location = new System.Drawing.Point(4, 29);
			this.tabvatpham.Name = "tabvatpham";
			this.tabvatpham.Padding = new System.Windows.Forms.Padding(3);
			this.tabvatpham.Size = new System.Drawing.Size(374, 398);
			this.tabvatpham.TabIndex = 2;
			this.tabvatpham.Text = "Vật Phẩm";
			this.tabvatpham.UseVisualStyleBackColor = true;
			this.label26.AutoSize = true;
			this.label26.ForeColor = System.Drawing.Color.Red;
			this.label26.Location = new System.Drawing.Point(10, 256);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(292, 13);
			this.label26.TabIndex = 4;
			this.label26.Text = "Auto sẽ không hủy VP có Điêu Văn,Tinh Thông,Khảm Ngọc";
			this.label25.AutoSize = true;
			this.label25.ForeColor = System.Drawing.Color.Red;
			this.label25.Location = new System.Drawing.Point(10, 234);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(249, 13);
			this.label25.TabIndex = 3;
			this.label25.Text = "Danh sách hủy đồ càng nhiều thì AUTO càng LAG";
			this.label24.AutoSize = true;
			this.label24.ForeColor = System.Drawing.Color.Red;
			this.label24.Location = new System.Drawing.Point(10, 212);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(287, 13);
			this.label24.TabIndex = 2;
			this.label24.Text = "Cẩn trọng khi sử dụng chức năng BÁN + HỦY đồ theo loại ";
			this.groupBox5.Controls.Add(this.butitemtuanhoan);
			this.groupBox5.Controls.Add(this.chekcautox2);
			this.groupBox5.Controls.Add(this.checkautovutrac);
			this.groupBox5.Controls.Add(this.chekcusingitem);
			this.groupBox5.Controls.Add(this.checkautocatkho);
			this.groupBox5.Location = new System.Drawing.Point(10, 99);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(356, 106);
			this.groupBox5.TabIndex = 1;
			this.groupBox5.TabStop = false;
			this.butitemtuanhoan.BackColor = System.Drawing.Color.DarkGray;
			this.butitemtuanhoan.Cursor = System.Windows.Forms.Cursors.Hand;
			this.butitemtuanhoan.Image = TinhKiemAuto.Properties.Resources.List_50px;
			this.butitemtuanhoan.Location = new System.Drawing.Point(132, 80);
			this.butitemtuanhoan.Name = "butitemtuanhoan";
			this.butitemtuanhoan.Size = new System.Drawing.Size(18, 18);
			this.butitemtuanhoan.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.butitemtuanhoan.TabIndex = 15;
			this.butitemtuanhoan.TabStop = false;
			this.toolTip1.SetToolTip(this.butitemtuanhoan, "Danh sách quái sẽ bỏ qua.\r\nNhững quái vật có tên trong danh sách này\r\nAuto sẽ bỏ qua không đánh.\r\n");
			this.butitemtuanhoan.Click += new System.EventHandler(butitemtuanhoan_Click);
			this.chekcautox2.AutoSize = true;
			this.chekcautox2.Location = new System.Drawing.Point(10, 58);
			this.chekcautox2.Name = "chekcautox2";
			this.chekcautox2.Size = new System.Drawing.Size(80, 17);
			this.chekcautox2.TabIndex = 13;
			this.chekcautox2.Text = "Tự Ăn X2.5";
			this.toolTip1.SetToolTip(this.chekcautox2, "Tích vào đây AUTO sẽ tự tìm X2.5\r\nở trong túi đồ để sử dụng.");
			this.chekcautox2.UseVisualStyleBackColor = true;
			this.chekcautox2.CheckedChanged += new System.EventHandler(chekcautox2_CheckedChanged);
			this.checkautovutrac.AutoSize = true;
			this.checkautovutrac.BackColor = System.Drawing.Color.DarkGray;
			this.checkautovutrac.Location = new System.Drawing.Point(10, 35);
			this.checkautovutrac.Name = "checkautovutrac";
			this.checkautovutrac.Size = new System.Drawing.Size(65, 17);
			this.checkautovutrac.TabIndex = 12;
			this.checkautovutrac.Text = "Vứt Rác";
			this.toolTip1.SetToolTip(this.checkautovutrac, "Tích vào đây Auto sẽ tự vứt rác.\r\n- Các nguyên liệu rác.\r\n- Các trang bị 1 sao ,2 sao\r\n- Auto sẽ không vứt đồ đã TINH THÔNG,ĐIÊU VĂN,KHẢM NGỌC");
			this.checkautovutrac.UseVisualStyleBackColor = false;
			this.checkautovutrac.CheckedChanged += new System.EventHandler(checkautovutrac_CheckedChanged);
			this.chekcusingitem.AutoSize = true;
			this.chekcusingitem.Location = new System.Drawing.Point(10, 81);
			this.chekcusingitem.Name = "chekcusingitem";
			this.chekcusingitem.Size = new System.Drawing.Size(117, 17);
			this.chekcusingitem.TabIndex = 14;
			this.chekcusingitem.Text = "Sử Dụng Vật Phẩm";
			this.toolTip1.SetToolTip(this.chekcusingitem, "Tích vào đây Auto sẽ tự động sử dụng vật phẩm\r\ntuần hoàn theo sanh sách được thiết lập");
			this.chekcusingitem.UseVisualStyleBackColor = true;
			this.chekcusingitem.CheckedChanged += new System.EventHandler(chekcusingitem_CheckedChanged);
			this.checkautocatkho.AutoSize = true;
			this.checkautocatkho.Location = new System.Drawing.Point(10, 14);
			this.checkautocatkho.Name = "checkautocatkho";
			this.checkautocatkho.Size = new System.Drawing.Size(86, 17);
			this.checkautocatkho.TabIndex = 11;
			this.checkautocatkho.Text = "Cất Vào Kho";
			this.toolTip1.SetToolTip(this.checkautocatkho, "Tích vào đây nhân vật sẽ bắt đầu chạy đi cất đồ vào kho");
			this.checkautocatkho.UseVisualStyleBackColor = true;
			this.checkautocatkho.CheckedChanged += new System.EventHandler(checkautocatkho_CheckedChanged);
			this.groupBox4.Controls.Add(this.label23);
			this.groupBox4.Controls.Add(this.numrangerpickitem);
			this.groupBox4.Controls.Add(this.butbanvatpham);
			this.groupBox4.Controls.Add(this.pickbanvatpham);
			this.groupBox4.Controls.Add(this.butdanhsachhuy);
			this.groupBox4.Controls.Add(this.checkhuyitem);
			this.groupBox4.Controls.Add(this.checkpickitem);
			this.groupBox4.Location = new System.Drawing.Point(10, 6);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(356, 87);
			this.groupBox4.TabIndex = 0;
			this.groupBox4.TabStop = false;
			this.label23.AutoSize = true;
			this.label23.Location = new System.Drawing.Point(218, 16);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(78, 13);
			this.label23.TabIndex = 12;
			this.label23.Text = "Bán Kính Nhặt";
			this.numrangerpickitem.BackColor = System.Drawing.Color.DarkGray;
			this.numrangerpickitem.Location = new System.Drawing.Point(305, 13);
			this.numrangerpickitem.Name = "numrangerpickitem";
			this.numrangerpickitem.Size = new System.Drawing.Size(45, 20);
			this.numrangerpickitem.TabIndex = 11;
			this.toolTip1.SetToolTip(this.numrangerpickitem, "Thiết lập khoảng cách nhặt đồ.\r\nBán kích càng to nhân vật sẽ di chuyển càng xa để nhặt đồ");
			this.numrangerpickitem.ValueChanged += new System.EventHandler(numericUpDown2_ValueChanged);
			this.butbanvatpham.BackColor = System.Drawing.Color.DarkGray;
			this.butbanvatpham.Cursor = System.Windows.Forms.Cursors.Hand;
			this.butbanvatpham.Image = TinhKiemAuto.Properties.Resources.List_50px;
			this.butbanvatpham.Location = new System.Drawing.Point(110, 56);
			this.butbanvatpham.Name = "butbanvatpham";
			this.butbanvatpham.Size = new System.Drawing.Size(18, 18);
			this.butbanvatpham.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.butbanvatpham.TabIndex = 10;
			this.butbanvatpham.TabStop = false;
			this.toolTip1.SetToolTip(this.butbanvatpham, "Danh sách vật phẩm Auto sẽ bán\r\nNhấn vào đây để chỉnh sửa danh sách\r\n");
			this.butbanvatpham.Click += new System.EventHandler(butbanvatpham_Click);
			this.pickbanvatpham.AutoSize = true;
			this.pickbanvatpham.Location = new System.Drawing.Point(10, 59);
			this.pickbanvatpham.Name = "pickbanvatpham";
			this.pickbanvatpham.Size = new System.Drawing.Size(94, 17);
			this.pickbanvatpham.TabIndex = 9;
			this.pickbanvatpham.Text = "Bán Vật Phẩm";
			this.toolTip1.SetToolTip(this.pickbanvatpham, "Tích vào đây Auto sẽ tự bán vật phẩm theo danh sách\r\n- Auto sẽ không bán VP đã TINH THÔNG,ĐIÊU VĂN,KHẢM NGỌC\r\n\r\n");
			this.pickbanvatpham.UseVisualStyleBackColor = true;
			this.pickbanvatpham.CheckedChanged += new System.EventHandler(pickbanvatpham_CheckedChanged);
			this.butdanhsachhuy.BackColor = System.Drawing.Color.DarkGray;
			this.butdanhsachhuy.Cursor = System.Windows.Forms.Cursors.Hand;
			this.butdanhsachhuy.Image = TinhKiemAuto.Properties.Resources.List_50px;
			this.butdanhsachhuy.Location = new System.Drawing.Point(110, 32);
			this.butdanhsachhuy.Name = "butdanhsachhuy";
			this.butdanhsachhuy.Size = new System.Drawing.Size(18, 18);
			this.butdanhsachhuy.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.butdanhsachhuy.TabIndex = 8;
			this.butdanhsachhuy.TabStop = false;
			this.toolTip1.SetToolTip(this.butdanhsachhuy, "Danh sách vật phẩm Auto Sẽ tự hủy\r\nNhấn vào đây để chỉnh sửa danh sách");
			this.butdanhsachhuy.Click += new System.EventHandler(butdanhsachhuy_Click);
			this.checkhuyitem.AutoSize = true;
			this.checkhuyitem.Location = new System.Drawing.Point(10, 37);
			this.checkhuyitem.Name = "checkhuyitem";
			this.checkhuyitem.Size = new System.Drawing.Size(94, 17);
			this.checkhuyitem.TabIndex = 2;
			this.checkhuyitem.Text = "Hủy Vật Phẩm";
			this.toolTip1.SetToolTip(this.checkhuyitem, "Tích vào đây Auto sẽ tự hủy vật phẩm theo danh sách\r\n- Auto sẽ không hủy VP đã TINH THÔNG,ĐIÊU VĂN,KHẢM NGỌC\r\n");
			this.checkhuyitem.UseVisualStyleBackColor = true;
			this.checkhuyitem.CheckedChanged += new System.EventHandler(checkhuyitem_CheckedChanged);
			this.checkpickitem.AutoSize = true;
			this.checkpickitem.Location = new System.Drawing.Point(10, 16);
			this.checkpickitem.Name = "checkpickitem";
			this.checkpickitem.Size = new System.Drawing.Size(98, 17);
			this.checkpickitem.TabIndex = 1;
			this.checkpickitem.Text = "Nhặt Vật Phẩm";
			this.toolTip1.SetToolTip(this.checkpickitem, "Tích vào đây nhân vật sẽ nhặt vật phẩm");
			this.checkpickitem.UseVisualStyleBackColor = true;
			this.checkpickitem.CheckedChanged += new System.EventHandler(checkpickitem_CheckedChanged);
			this.tabPage2.Controls.Add(this.groupBox6);
			this.tabPage2.Location = new System.Drawing.Point(4, 29);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(374, 398);
			this.tabPage2.TabIndex = 3;
			this.tabPage2.Text = "Pet";
			this.tabPage2.UseVisualStyleBackColor = true;
			this.groupBox6.Controls.Add(this.CheckReGenPET);
			this.groupBox6.Controls.Add(this.numericUpDown3);
			this.groupBox6.Controls.Add(this.CheckthuPet);
			this.groupBox6.Controls.Add(this.checkBox10);
			this.groupBox6.Controls.Add(this.checkBox9);
			this.groupBox6.Controls.Add(this.button4);
			this.groupBox6.Controls.Add(this.cboXuatPet);
			this.groupBox6.Controls.Add(this.AutoXuatPhet);
			this.groupBox6.Location = new System.Drawing.Point(6, 6);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(363, 136);
			this.groupBox6.TabIndex = 0;
			this.groupBox6.TabStop = false;
			this.groupBox6.Enter += new System.EventHandler(groupBox6_Enter);
			this.CheckReGenPET.AutoSize = true;
			this.CheckReGenPET.Location = new System.Drawing.Point(15, 111);
			this.CheckReGenPET.Name = "CheckReGenPET";
			this.CheckReGenPET.Size = new System.Drawing.Size(99, 17);
			this.CheckReGenPET.TabIndex = 20;
			this.CheckReGenPET.Text = "Chăm Sóc PET";
			this.toolTip1.SetToolTip(this.CheckReGenPET, "Tích vào đây AUTO sẽ tự cho PET ăn");
			this.CheckReGenPET.UseVisualStyleBackColor = true;
			this.CheckReGenPET.CheckedChanged += new System.EventHandler(CheckReGenPET_CheckedChanged);
			this.numericUpDown3.BackColor = System.Drawing.Color.DarkGray;
			this.numericUpDown3.Location = new System.Drawing.Point(161, 86);
			this.numericUpDown3.Maximum = new decimal(new int[4] { 125, 0, 0, 0 });
			this.numericUpDown3.Name = "numericUpDown3";
			this.numericUpDown3.Size = new System.Drawing.Size(54, 20);
			this.numericUpDown3.TabIndex = 19;
			this.numericUpDown3.ValueChanged += new System.EventHandler(numericUpDown3_ValueChanged);
			this.CheckthuPet.AutoSize = true;
			this.CheckthuPet.Location = new System.Drawing.Point(15, 88);
			this.CheckthuPet.Name = "CheckthuPet";
			this.CheckthuPet.Size = new System.Drawing.Size(140, 17);
			this.CheckthuPet.TabIndex = 18;
			this.CheckthuPet.Text = "Tự Thu Pet Khi Đạt Cấp";
			this.toolTip1.SetToolTip(this.CheckthuPet, "Tự Thu Pet Khi Đạt Cấp");
			this.CheckthuPet.UseVisualStyleBackColor = true;
			this.CheckthuPet.CheckedChanged += new System.EventHandler(CheckthuPet_CheckedChanged);
			this.checkBox10.AutoSize = true;
			this.checkBox10.BackColor = System.Drawing.Color.DarkGray;
			this.checkBox10.Location = new System.Drawing.Point(15, 65);
			this.checkBox10.Name = "checkBox10";
			this.checkBox10.Size = new System.Drawing.Size(85, 17);
			this.checkBox10.TabIndex = 17;
			this.checkBox10.Text = "Tự Skill PET";
			this.toolTip1.SetToolTip(this.checkBox10, "Tích vào đây AUTO sẽ tự sử dụng skill cho PET");
			this.checkBox10.UseVisualStyleBackColor = false;
			this.checkBox10.CheckedChanged += new System.EventHandler(checkBox10_CheckedChanged);
			this.checkBox9.AutoSize = true;
			this.checkBox9.BackColor = System.Drawing.Color.DarkGray;
			this.checkBox9.Location = new System.Drawing.Point(15, 42);
			this.checkBox9.Name = "checkBox9";
			this.checkBox9.Size = new System.Drawing.Size(80, 17);
			this.checkBox9.TabIndex = 16;
			this.checkBox9.Text = "Tự Buff Pet";
			this.toolTip1.SetToolTip(this.checkBox9, "Tích vào đây auto sẽ tự buff PET");
			this.checkBox9.UseVisualStyleBackColor = false;
			this.checkBox9.CheckedChanged += new System.EventHandler(checkBox9_CheckedChanged);
			this.button4.BackColor = System.Drawing.Color.Orange;
			this.button4.Location = new System.Drawing.Point(287, 13);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(56, 23);
			this.button4.TabIndex = 15;
			this.button4.Text = "Gọi";
			this.toolTip1.SetToolTip(this.button4, "Nhấn vào đây để gọi PET");
			this.button4.UseVisualStyleBackColor = false;
			this.button4.Click += new System.EventHandler(button4_Click);
			this.cboXuatPet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboXuatPet.FormattingEnabled = true;
			this.cboXuatPet.Location = new System.Drawing.Point(109, 15);
			this.cboXuatPet.Name = "cboXuatPet";
			this.cboXuatPet.Size = new System.Drawing.Size(164, 21);
			this.cboXuatPet.TabIndex = 3;
			this.toolTip1.SetToolTip(this.cboXuatPet, "Chọn PET trong danh sách");
			this.cboXuatPet.SelectedIndexChanged += new System.EventHandler(cboXuatPet_SelectedIndexChanged);
			this.AutoXuatPhet.AutoSize = true;
			this.AutoXuatPhet.BackColor = System.Drawing.Color.DarkGray;
			this.AutoXuatPhet.Location = new System.Drawing.Point(15, 19);
			this.AutoXuatPhet.Name = "AutoXuatPhet";
			this.AutoXuatPhet.Size = new System.Drawing.Size(88, 17);
			this.AutoXuatPhet.TabIndex = 2;
			this.AutoXuatPhet.Text = "Tự Xuất PET";
			this.toolTip1.SetToolTip(this.AutoXuatPhet, "Tích vào đây AUTO sẽ tự xuất PET đã được thiết lập\r\n");
			this.AutoXuatPhet.UseVisualStyleBackColor = false;
			this.AutoXuatPhet.CheckedChanged += new System.EventHandler(AutoXuatPhet_CheckedChanged);
			this.tabtienich.Controls.Add(this.groupBox9);
			this.tabtienich.Controls.Add(this.groupBox8);
			this.tabtienich.Controls.Add(this.groupBox7);
			this.tabtienich.Location = new System.Drawing.Point(4, 29);
			this.tabtienich.Name = "tabtienich";
			this.tabtienich.Padding = new System.Windows.Forms.Padding(3);
			this.tabtienich.Size = new System.Drawing.Size(374, 398);
			this.tabtienich.TabIndex = 4;
			this.tabtienich.Text = "Tiện Ích";
			this.tabtienich.UseVisualStyleBackColor = true;
			this.groupBox9.Controls.Add(this.pictureBox12);
			this.groupBox9.Controls.Add(this.button6);
			this.groupBox9.Controls.Add(this.label31);
			this.groupBox9.Controls.Add(this.txtmkkho);
			this.groupBox9.Controls.Add(this.label30);
			this.groupBox9.Controls.Add(this.numbankinhtheosau);
			this.groupBox9.Controls.Add(this.label29);
			this.groupBox9.Location = new System.Drawing.Point(6, 214);
			this.groupBox9.Name = "groupBox9";
			this.groupBox9.Size = new System.Drawing.Size(360, 64);
			this.groupBox9.TabIndex = 2;
			this.groupBox9.TabStop = false;
			this.pictureBox12.Cursor = System.Windows.Forms.Cursors.Hand;
			this.pictureBox12.Image = TinhKiemAuto.Properties.Resources.Settings_48px;
			this.pictureBox12.Location = new System.Drawing.Point(278, 13);
			this.pictureBox12.Name = "pictureBox12";
			this.pictureBox12.Size = new System.Drawing.Size(18, 18);
			this.pictureBox12.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox12.TabIndex = 18;
			this.pictureBox12.TabStop = false;
			this.toolTip1.SetToolTip(this.pictureBox12, "Click vào đây để thiết lập lại đường dẫn Game");
			this.pictureBox12.Click += new System.EventHandler(pictureBox12_Click);
			this.button6.BackColor = System.Drawing.Color.Red;
			this.button6.Location = new System.Drawing.Point(262, 35);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(41, 23);
			this.button6.TabIndex = 17;
			this.button6.Text = "Mở";
			this.toolTip1.SetToolTip(this.button6, "Nhấn vào đây để mở khóa kho");
			this.button6.UseVisualStyleBackColor = false;
			this.button6.Click += new System.EventHandler(button6_Click);
			this.label31.AutoSize = true;
			this.label31.Location = new System.Drawing.Point(177, 16);
			this.label31.Name = "label31";
			this.label31.Size = new System.Drawing.Size(99, 13);
			this.label31.TabIndex = 15;
			this.label31.Text = "Đường Dẫn Game :";
			this.txtmkkho.Location = new System.Drawing.Point(115, 37);
			this.txtmkkho.Name = "txtmkkho";
			this.txtmkkho.Size = new System.Drawing.Size(141, 20);
			this.txtmkkho.TabIndex = 15;
			this.txtmkkho.TextChanged += new System.EventHandler(txtmkkho_TextChanged);
			this.label30.AutoSize = true;
			this.label30.Location = new System.Drawing.Point(7, 40);
			this.label30.Name = "label30";
			this.label30.Size = new System.Drawing.Size(75, 13);
			this.label30.TabIndex = 14;
			this.label30.Text = "Mật Khẩu Kho";
			this.numbankinhtheosau.BackColor = System.Drawing.Color.DarkGray;
			this.numbankinhtheosau.Location = new System.Drawing.Point(115, 13);
			this.numbankinhtheosau.Name = "numbankinhtheosau";
			this.numbankinhtheosau.Size = new System.Drawing.Size(34, 20);
			this.numbankinhtheosau.TabIndex = 13;
			this.numbankinhtheosau.Value = new decimal(new int[4] { 50, 0, 0, 0 });
			this.numbankinhtheosau.ValueChanged += new System.EventHandler(numbankinhtheosau_ValueChanged);
			this.label29.AutoSize = true;
			this.label29.Location = new System.Drawing.Point(7, 16);
			this.label29.Name = "label29";
			this.label29.Size = new System.Drawing.Size(102, 13);
			this.label29.TabIndex = 0;
			this.label29.Text = "Bán Kính Theo Sau";
			this.groupBox8.Controls.Add(this.Checkthongbaochatmat);
			this.groupBox8.Controls.Add(this.label28);
			this.groupBox8.Controls.Add(this.txtthoigian);
			this.groupBox8.Controls.Add(this.label27);
			this.groupBox8.Controls.Add(this.pictureBox11);
			this.groupBox8.Controls.Add(this.button5);
			this.groupBox8.Controls.Add(this.txtnoidunggiaochat);
			this.groupBox8.Controls.Add(this.checkgiaochat);
			this.groupBox8.Location = new System.Drawing.Point(6, 99);
			this.groupBox8.Name = "groupBox8";
			this.groupBox8.Size = new System.Drawing.Size(360, 111);
			this.groupBox8.TabIndex = 1;
			this.groupBox8.TabStop = false;
			this.groupBox8.Enter += new System.EventHandler(groupBox8_Enter);
			this.Checkthongbaochatmat.AutoSize = true;
			this.Checkthongbaochatmat.Location = new System.Drawing.Point(7, 92);
			this.Checkthongbaochatmat.Name = "Checkthongbaochatmat";
			this.Checkthongbaochatmat.Size = new System.Drawing.Size(159, 17);
			this.Checkthongbaochatmat.TabIndex = 21;
			this.Checkthongbaochatmat.Text = "Thông Báo Khi Có Chat Mật";
			this.toolTip1.SetToolTip(this.Checkthongbaochatmat, "Tích vào đây để nhận thông báo khi có CHAT \r\nmật");
			this.Checkthongbaochatmat.UseVisualStyleBackColor = true;
			this.Checkthongbaochatmat.CheckedChanged += new System.EventHandler(Checkthongbaochatmat_CheckedChanged);
			this.label28.AutoSize = true;
			this.label28.Location = new System.Drawing.Point(124, 18);
			this.label28.Name = "label28";
			this.label28.Size = new System.Drawing.Size(28, 13);
			this.label28.TabIndex = 20;
			this.label28.Text = "Giây";
			this.txtthoigian.BackColor = System.Drawing.SystemColors.InactiveCaption;
			this.txtthoigian.Location = new System.Drawing.Point(86, 13);
			this.txtthoigian.Name = "txtthoigian";
			this.txtthoigian.Size = new System.Drawing.Size(37, 20);
			this.txtthoigian.TabIndex = 19;
			this.txtthoigian.Text = "300";
			this.toolTip1.SetToolTip(this.txtthoigian, "Thiết lập thời gian giao chát.\r\nCứ sau X giây Auto sẽ giao chát 1 lần trên các kênh \r\nmà bạn đã thiết lập");
			this.txtthoigian.TextChanged += new System.EventHandler(txtthoigian_TextChanged);
			this.label27.AutoSize = true;
			this.label27.Location = new System.Drawing.Point(192, 18);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(80, 13);
			this.label27.TabIndex = 18;
			this.label27.Text = "Thiết Lập Kênh";
			this.pictureBox11.Cursor = System.Windows.Forms.Cursors.Hand;
			this.pictureBox11.Image = TinhKiemAuto.Properties.Resources.Settings_48px;
			this.pictureBox11.Location = new System.Drawing.Point(278, 14);
			this.pictureBox11.Name = "pictureBox11";
			this.pictureBox11.Size = new System.Drawing.Size(18, 18);
			this.pictureBox11.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox11.TabIndex = 17;
			this.pictureBox11.TabStop = false;
			this.toolTip1.SetToolTip(this.pictureBox11, "Click vào đây để thiết lập danh sách \r\ncác kênh bạn sẽ giao chát");
			this.pictureBox11.Click += new System.EventHandler(pictureBox11_Click);
			this.button5.BackColor = System.Drawing.Color.Red;
			this.button5.Location = new System.Drawing.Point(308, 13);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(41, 23);
			this.button5.TabIndex = 16;
			this.button5.Text = "Lấy";
			this.toolTip1.SetToolTip(this.button5, "Click vào đây để lấy nội dung bạn vừa giao\r\nChat trong bảng CHAT");
			this.button5.UseVisualStyleBackColor = false;
			this.button5.Click += new System.EventHandler(button5_Click);
			this.txtnoidunggiaochat.BackColor = System.Drawing.SystemColors.Info;
			this.txtnoidunggiaochat.Location = new System.Drawing.Point(7, 36);
			this.txtnoidunggiaochat.Name = "txtnoidunggiaochat";
			this.txtnoidunggiaochat.Size = new System.Drawing.Size(347, 54);
			this.txtnoidunggiaochat.TabIndex = 5;
			this.txtnoidunggiaochat.Text = "";
			this.txtnoidunggiaochat.TextChanged += new System.EventHandler(txtnoidunggiaochat_TextChanged);
			this.checkgiaochat.AutoSize = true;
			this.checkgiaochat.Location = new System.Drawing.Point(7, 17);
			this.checkgiaochat.Name = "checkgiaochat";
			this.checkgiaochat.Size = new System.Drawing.Size(71, 17);
			this.checkgiaochat.TabIndex = 4;
			this.checkgiaochat.Text = "Rao Chát";
			this.toolTip1.SetToolTip(this.checkgiaochat, "Tích vào đây để kích hoạt giao chát\r\n");
			this.checkgiaochat.UseVisualStyleBackColor = true;
			this.checkgiaochat.CheckedChanged += new System.EventHandler(checkgiaochat_CheckedChanged);
			this.groupBox7.Controls.Add(this.numuplevel);
			this.groupBox7.Controls.Add(this.checkauouplevel);
			this.groupBox7.Controls.Add(this.checkautoskillf1);
			this.groupBox7.Controls.Add(this.checkdongytoanbo);
			this.groupBox7.Controls.Add(this.butdanhsachdongy);
			this.groupBox7.Controls.Add(this.checkdongytodoi);
			this.groupBox7.Location = new System.Drawing.Point(6, 6);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(362, 90);
			this.groupBox7.TabIndex = 0;
			this.groupBox7.TabStop = false;
			this.numuplevel.BackColor = System.Drawing.Color.DarkGray;
			this.numuplevel.Location = new System.Drawing.Point(165, 56);
			this.numuplevel.Maximum = new decimal(new int[4] { 200, 0, 0, 0 });
			this.numuplevel.Name = "numuplevel";
			this.numuplevel.Size = new System.Drawing.Size(46, 20);
			this.numuplevel.TabIndex = 12;
			this.numuplevel.Value = new decimal(new int[4] { 50, 0, 0, 0 });
			this.numuplevel.ValueChanged += new System.EventHandler(numuplevel_ValueChanged);
			this.checkauouplevel.AutoSize = true;
			this.checkauouplevel.BackColor = System.Drawing.Color.DarkGray;
			this.checkauouplevel.Location = new System.Drawing.Point(7, 59);
			this.checkauouplevel.Name = "checkauouplevel";
			this.checkauouplevel.Size = new System.Drawing.Size(152, 17);
			this.checkauouplevel.TabIndex = 11;
			this.checkauouplevel.Text = "Tự Thăng Cấp Nhân Vật <";
			this.toolTip1.SetToolTip(this.checkauouplevel, "Tích vào đây để tự đặt skill vào F1");
			this.checkauouplevel.UseVisualStyleBackColor = false;
			this.checkauouplevel.CheckedChanged += new System.EventHandler(checkauouplevel_CheckedChanged);
			this.checkautoskillf1.AutoSize = true;
			this.checkautoskillf1.BackColor = System.Drawing.Color.DarkGray;
			this.checkautoskillf1.Location = new System.Drawing.Point(7, 36);
			this.checkautoskillf1.Name = "checkautoskillf1";
			this.checkautoskillf1.Size = new System.Drawing.Size(102, 17);
			this.checkautoskillf1.TabIndex = 10;
			this.checkautoskillf1.Text = "Đặt Skill Vào F1";
			this.toolTip1.SetToolTip(this.checkautoskillf1, "Tích vào đây để tự đặt skill vào F1");
			this.checkautoskillf1.UseVisualStyleBackColor = false;
			this.checkautoskillf1.CheckedChanged += new System.EventHandler(checkautoskillf1_CheckedChanged);
			this.checkdongytoanbo.AutoSize = true;
			this.checkdongytoanbo.BackColor = System.Drawing.Color.DarkGray;
			this.checkdongytoanbo.Location = new System.Drawing.Point(129, 13);
			this.checkdongytoanbo.Name = "checkdongytoanbo";
			this.checkdongytoanbo.Size = new System.Drawing.Size(67, 17);
			this.checkdongytoanbo.TabIndex = 9;
			this.checkdongytoanbo.Text = "Toàn Bộ";
			this.toolTip1.SetToolTip(this.checkdongytoanbo, "Tích vào đây để đồng ý toàn bộ \r\ncác lời mời tổ đội");
			this.checkdongytoanbo.UseVisualStyleBackColor = false;
			this.checkdongytoanbo.CheckedChanged += new System.EventHandler(checkdongytoanbo_CheckedChanged);
			this.butdanhsachdongy.Cursor = System.Windows.Forms.Cursors.Hand;
			this.butdanhsachdongy.Image = TinhKiemAuto.Properties.Resources.List_50px;
			this.butdanhsachdongy.Location = new System.Drawing.Point(105, 12);
			this.butdanhsachdongy.Name = "butdanhsachdongy";
			this.butdanhsachdongy.Size = new System.Drawing.Size(18, 18);
			this.butdanhsachdongy.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.butdanhsachdongy.TabIndex = 8;
			this.butdanhsachdongy.TabStop = false;
			this.toolTip1.SetToolTip(this.butdanhsachdongy, "Danh sách quái sẽ bỏ qua.\r\nNhững quái vật có tên trong danh sách này\r\nAuto sẽ bỏ qua không đánh.\r\n");
			this.butdanhsachdongy.Click += new System.EventHandler(butdanhsachdongy_Click);
			this.checkdongytodoi.AutoSize = true;
			this.checkdongytodoi.BackColor = System.Drawing.Color.DarkGray;
			this.checkdongytodoi.Location = new System.Drawing.Point(7, 14);
			this.checkdongytodoi.Name = "checkdongytodoi";
			this.checkdongytodoi.Size = new System.Drawing.Size(97, 17);
			this.checkdongytodoi.TabIndex = 3;
			this.checkdongytodoi.Text = "Đồng Ý Tổ Đội";
			this.toolTip1.SetToolTip(this.checkdongytodoi, "Tích vào đây AUTO sẽ tự xuất PET đã được thiết lập\r\n");
			this.checkdongytodoi.UseVisualStyleBackColor = false;
			this.checkdongytodoi.CheckedChanged += new System.EventHandler(checkdongytodoi_CheckedChanged);
			this.tabchedo.Controls.Add(this.butche);
			this.tabchedo.Controls.Add(this.label49);
			this.tabchedo.Controls.Add(this.groupBox14);
			this.tabchedo.Controls.Add(this.groupBox11);
			this.tabchedo.Controls.Add(this.groupBox10);
			this.tabchedo.Location = new System.Drawing.Point(4, 29);
			this.tabchedo.Name = "tabchedo";
			this.tabchedo.Padding = new System.Windows.Forms.Padding(3);
			this.tabchedo.Size = new System.Drawing.Size(374, 398);
			this.tabchedo.TabIndex = 5;
			this.tabchedo.Text = "Chế Đồ";
			this.tabchedo.UseVisualStyleBackColor = true;
			this.butche.Location = new System.Drawing.Point(284, 257);
			this.butche.Name = "butche";
			this.butche.Size = new System.Drawing.Size(75, 23);
			this.butche.TabIndex = 6;
			this.butche.Text = "Chế";
			this.butche.UseVisualStyleBackColor = true;
			this.butche.Click += new System.EventHandler(butche_Click);
			this.label49.AutoSize = true;
			this.label49.ForeColor = System.Drawing.Color.Red;
			this.label49.Location = new System.Drawing.Point(8, 261);
			this.label49.Name = "label49";
			this.label49.Size = new System.Drawing.Size(248, 13);
			this.label49.TabIndex = 5;
			this.label49.Text = "Chú Ý : Khi đang chế không tự ý vứt đồ ra khỏi túi.";
			this.groupBox14.Controls.Add(this.txtdahuy);
			this.groupBox14.Controls.Add(this.label54);
			this.groupBox14.Controls.Add(this.txtdache);
			this.groupBox14.Controls.Add(this.label53);
			this.groupBox14.Controls.Add(this.txttaodo);
			this.groupBox14.Controls.Add(this.label52);
			this.groupBox14.Controls.Add(this.txtbingan);
			this.groupBox14.Controls.Add(this.label51);
			this.groupBox14.Controls.Add(this.txtvaibong);
			this.groupBox14.Controls.Add(this.label50);
			this.groupBox14.Controls.Add(this.txttinhthiet);
			this.groupBox14.Controls.Add(this.label48);
			this.groupBox14.Location = new System.Drawing.Point(223, 92);
			this.groupBox14.Name = "groupBox14";
			this.groupBox14.Size = new System.Drawing.Size(143, 166);
			this.groupBox14.TabIndex = 4;
			this.groupBox14.TabStop = false;
			this.txtdahuy.AutoSize = true;
			this.txtdahuy.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.txtdahuy.ForeColor = System.Drawing.Color.Red;
			this.txtdahuy.Location = new System.Drawing.Point(87, 137);
			this.txtdahuy.Name = "txtdahuy";
			this.txtdahuy.Size = new System.Drawing.Size(14, 15);
			this.txtdahuy.TabIndex = 16;
			this.txtdahuy.Text = "0";
			this.label54.AutoSize = true;
			this.label54.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label54.ForeColor = System.Drawing.Color.Red;
			this.label54.Location = new System.Drawing.Point(20, 137);
			this.label54.Name = "label54";
			this.label54.Size = new System.Drawing.Size(47, 15);
			this.label54.TabIndex = 15;
			this.label54.Text = "Đã Hủy";
			this.txtdache.AutoSize = true;
			this.txtdache.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.txtdache.ForeColor = System.Drawing.Color.Blue;
			this.txtdache.Location = new System.Drawing.Point(87, 111);
			this.txtdache.Name = "txtdache";
			this.txtdache.Size = new System.Drawing.Size(14, 15);
			this.txtdache.TabIndex = 14;
			this.txtdache.Text = "0";
			this.label53.AutoSize = true;
			this.label53.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label53.ForeColor = System.Drawing.Color.Blue;
			this.label53.Location = new System.Drawing.Point(20, 111);
			this.label53.Name = "label53";
			this.label53.Size = new System.Drawing.Size(54, 15);
			this.label53.TabIndex = 13;
			this.label53.Text = "Đã Chế :";
			this.txttaodo.AutoSize = true;
			this.txttaodo.Location = new System.Drawing.Point(87, 90);
			this.txttaodo.Name = "txttaodo";
			this.txttaodo.Size = new System.Drawing.Size(13, 13);
			this.txttaodo.TabIndex = 12;
			this.txttaodo.Text = "0";
			this.label52.AutoSize = true;
			this.label52.Location = new System.Drawing.Point(20, 90);
			this.label52.Name = "label52";
			this.label52.Size = new System.Drawing.Size(49, 13);
			this.label52.TabIndex = 11;
			this.label52.Text = "Tạo Đồ :";
			this.txtbingan.AutoSize = true;
			this.txtbingan.Location = new System.Drawing.Point(87, 68);
			this.txtbingan.Name = "txtbingan";
			this.txtbingan.Size = new System.Drawing.Size(13, 13);
			this.txtbingan.TabIndex = 10;
			this.txtbingan.Text = "0";
			this.label51.AutoSize = true;
			this.label51.Location = new System.Drawing.Point(20, 68);
			this.label51.Name = "label51";
			this.label51.Size = new System.Drawing.Size(53, 13);
			this.label51.TabIndex = 9;
			this.label51.Text = "Bí Ngân :";
			this.txtvaibong.AutoSize = true;
			this.txtvaibong.Location = new System.Drawing.Point(87, 44);
			this.txtvaibong.Name = "txtvaibong";
			this.txtvaibong.Size = new System.Drawing.Size(13, 13);
			this.txtvaibong.TabIndex = 8;
			this.txtvaibong.Text = "0";
			this.label50.AutoSize = true;
			this.label50.Location = new System.Drawing.Point(20, 44);
			this.label50.Name = "label50";
			this.label50.Size = new System.Drawing.Size(56, 13);
			this.label50.TabIndex = 7;
			this.label50.Text = "Vải Bông :";
			this.txttinhthiet.AutoSize = true;
			this.txttinhthiet.Location = new System.Drawing.Point(87, 22);
			this.txttinhthiet.Name = "txttinhthiet";
			this.txttinhthiet.Size = new System.Drawing.Size(13, 13);
			this.txttinhthiet.TabIndex = 6;
			this.txttinhthiet.Text = "0";
			this.label48.AutoSize = true;
			this.label48.Location = new System.Drawing.Point(15, 22);
			this.label48.Name = "label48";
			this.label48.Size = new System.Drawing.Size(61, 13);
			this.label48.TabIndex = 5;
			this.label48.Text = "Tinh Thiết :";
			this.groupBox11.Controls.Add(this.comcapdtd);
			this.groupBox11.Controls.Add(this.label37);
			this.groupBox11.Controls.Add(this.comboloai);
			this.groupBox11.Controls.Add(this.label36);
			this.groupBox11.Location = new System.Drawing.Point(223, 6);
			this.groupBox11.Name = "groupBox11";
			this.groupBox11.Size = new System.Drawing.Size(143, 80);
			this.groupBox11.TabIndex = 3;
			this.groupBox11.TabStop = false;
			this.comcapdtd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comcapdtd.FormattingEnabled = true;
			this.comcapdtd.Items.AddRange(new object[10] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" });
			this.comcapdtd.Location = new System.Drawing.Point(40, 43);
			this.comcapdtd.Name = "comcapdtd";
			this.comcapdtd.Size = new System.Drawing.Size(97, 21);
			this.comcapdtd.TabIndex = 6;
			this.toolTip1.SetToolTip(this.comcapdtd, "Chọn bản đồ muốn lên bãi");
			this.comcapdtd.SelectedIndexChanged += new System.EventHandler(comcapdtd_SelectedIndexChanged);
			this.label37.AutoSize = true;
			this.label37.Location = new System.Drawing.Point(7, 48);
			this.label37.Name = "label37";
			this.label37.Size = new System.Drawing.Size(30, 13);
			this.label37.TabIndex = 5;
			this.label37.Text = "ĐTĐ";
			this.label37.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.comboloai.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboloai.FormattingEnabled = true;
			this.comboloai.Items.AddRange(new object[16]
			{
				"Đao búa", "Thương tần", "Đơn đoản", "Song đoản", "Phiến", "Khuyên", "Mão", "Y phục", "Hộ thủ", "Hài",
				"Hộ uyển", "Hộ kiên", "Yêu đái", "Hạng liên", "Giới chỉ", "Hộ phù"
			});
			this.comboloai.Location = new System.Drawing.Point(40, 15);
			this.comboloai.Name = "comboloai";
			this.comboloai.Size = new System.Drawing.Size(97, 21);
			this.comboloai.TabIndex = 4;
			this.toolTip1.SetToolTip(this.comboloai, "Chọn bản đồ muốn lên bãi");
			this.comboloai.SelectedIndexChanged += new System.EventHandler(comboloai_SelectedIndexChanged);
			this.label36.AutoSize = true;
			this.label36.Location = new System.Drawing.Point(7, 20);
			this.label36.Name = "label36";
			this.label36.Size = new System.Drawing.Size(27, 13);
			this.label36.TabIndex = 3;
			this.label36.Text = "Loại";
			this.groupBox10.Controls.Add(this.groupBox13);
			this.groupBox10.Controls.Add(this.comboBox3);
			this.groupBox10.Controls.Add(this.label38);
			this.groupBox10.Controls.Add(this.checkBox13);
			this.groupBox10.Controls.Add(this.groupBox12);
			this.groupBox10.Controls.Add(this.label34);
			this.groupBox10.Controls.Add(this.numericUpDown4);
			this.groupBox10.Controls.Add(this.label35);
			this.groupBox10.Controls.Add(this.label33);
			this.groupBox10.Controls.Add(this.numsonguyenlieu);
			this.groupBox10.Controls.Add(this.label32);
			this.groupBox10.Controls.Add(this.checkBox12);
			this.groupBox10.Location = new System.Drawing.Point(8, 6);
			this.groupBox10.Name = "groupBox10";
			this.groupBox10.Size = new System.Drawing.Size(209, 252);
			this.groupBox10.TabIndex = 2;
			this.groupBox10.TabStop = false;
			this.groupBox13.Controls.Add(this.label44);
			this.groupBox13.Controls.Add(this.label47);
			this.groupBox13.Controls.Add(this.label46);
			this.groupBox13.Controls.Add(this.label45);
			this.groupBox13.Controls.Add(this.label43);
			this.groupBox13.Controls.Add(this.label42);
			this.groupBox13.Controls.Add(this.numericUpDown6);
			this.groupBox13.Controls.Add(this.label41);
			this.groupBox13.Controls.Add(this.numericUpDown5);
			this.groupBox13.Controls.Add(this.label40);
			this.groupBox13.Controls.Add(this.numsosao);
			this.groupBox13.Controls.Add(this.label39);
			this.groupBox13.Location = new System.Drawing.Point(0, 161);
			this.groupBox13.Name = "groupBox13";
			this.groupBox13.Size = new System.Drawing.Size(209, 91);
			this.groupBox13.TabIndex = 11;
			this.groupBox13.TabStop = false;
			this.label44.AutoSize = true;
			this.label44.Location = new System.Drawing.Point(76, 67);
			this.label44.Name = "label44";
			this.label44.Size = new System.Drawing.Size(13, 13);
			this.label44.TabIndex = 31;
			this.label44.Text = "<";
			this.label47.AutoSize = true;
			this.label47.Location = new System.Drawing.Point(148, 67);
			this.label47.Name = "label47";
			this.label47.Size = new System.Drawing.Size(31, 13);
			this.label47.TabIndex = 30;
			this.label47.Text = "Điểm";
			this.label46.AutoSize = true;
			this.label46.Location = new System.Drawing.Point(146, 41);
			this.label46.Name = "label46";
			this.label46.Size = new System.Drawing.Size(33, 13);
			this.label46.TabIndex = 29;
			this.label46.Text = "Dòng";
			this.label45.AutoSize = true;
			this.label45.Location = new System.Drawing.Point(149, 15);
			this.label45.Name = "label45";
			this.label45.Size = new System.Drawing.Size(26, 13);
			this.label45.TabIndex = 28;
			this.label45.Text = "Sao";
			this.label43.AutoSize = true;
			this.label43.Location = new System.Drawing.Point(76, 41);
			this.label43.Name = "label43";
			this.label43.Size = new System.Drawing.Size(13, 13);
			this.label43.TabIndex = 26;
			this.label43.Text = "<";
			this.label42.AutoSize = true;
			this.label42.Location = new System.Drawing.Point(76, 15);
			this.label42.Name = "label42";
			this.label42.Size = new System.Drawing.Size(13, 13);
			this.label42.TabIndex = 25;
			this.label42.Text = "<";
			this.numericUpDown6.Location = new System.Drawing.Point(95, 65);
			this.numericUpDown6.Maximum = new decimal(new int[4] { 300, 0, 0, 0 });
			this.numericUpDown6.Name = "numericUpDown6";
			this.numericUpDown6.Size = new System.Drawing.Size(45, 20);
			this.numericUpDown6.TabIndex = 9;
			this.numericUpDown6.ValueChanged += new System.EventHandler(numericUpDown6_ValueChanged);
			this.label41.AutoSize = true;
			this.label41.Location = new System.Drawing.Point(8, 69);
			this.label41.Name = "label41";
			this.label41.Size = new System.Drawing.Size(47, 13);
			this.label41.TabIndex = 8;
			this.label41.Text = "Thể Lực";
			this.numericUpDown5.Location = new System.Drawing.Point(95, 39);
			this.numericUpDown5.Maximum = new decimal(new int[4] { 300, 0, 0, 0 });
			this.numericUpDown5.Name = "numericUpDown5";
			this.numericUpDown5.Size = new System.Drawing.Size(45, 20);
			this.numericUpDown5.TabIndex = 7;
			this.numericUpDown5.ValueChanged += new System.EventHandler(numericUpDown5_ValueChanged);
			this.label40.AutoSize = true;
			this.label40.Location = new System.Drawing.Point(8, 43);
			this.label40.Name = "label40";
			this.label40.Size = new System.Drawing.Size(49, 13);
			this.label40.TabIndex = 6;
			this.label40.Text = "Số Dòng";
			this.numsosao.Location = new System.Drawing.Point(95, 13);
			this.numsosao.Maximum = new decimal(new int[4] { 300, 0, 0, 0 });
			this.numsosao.Name = "numsosao";
			this.numsosao.Size = new System.Drawing.Size(45, 20);
			this.numsosao.TabIndex = 5;
			this.numsosao.ValueChanged += new System.EventHandler(numsosao_ValueChanged);
			this.label39.AutoSize = true;
			this.label39.Location = new System.Drawing.Point(8, 17);
			this.label39.Name = "label39";
			this.label39.Size = new System.Drawing.Size(42, 13);
			this.label39.TabIndex = 4;
			this.label39.Text = "Số Sao";
			this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox3.FormattingEnabled = true;
			this.comboBox3.Items.AddRange(new object[3] { "Ngồi Chơi", "Đi Train", "Tắt Máy" });
			this.comboBox3.Location = new System.Drawing.Point(68, 134);
			this.comboBox3.Name = "comboBox3";
			this.comboBox3.Size = new System.Drawing.Size(113, 21);
			this.comboBox3.TabIndex = 10;
			this.toolTip1.SetToolTip(this.comboBox3, "Chọn bản đồ muốn lên bãi");
			this.comboBox3.SelectedIndexChanged += new System.EventHandler(comboBox3_SelectedIndexChanged);
			this.label38.AutoSize = true;
			this.label38.Location = new System.Drawing.Point(8, 139);
			this.label38.Name = "label38";
			this.label38.Size = new System.Drawing.Size(54, 13);
			this.label38.TabIndex = 9;
			this.label38.Text = "Chế Xong";
			this.checkBox13.AutoSize = true;
			this.checkBox13.Location = new System.Drawing.Point(3, 111);
			this.checkBox13.Name = "checkBox13";
			this.checkBox13.Size = new System.Drawing.Size(133, 17);
			this.checkBox13.TabIndex = 8;
			this.checkBox13.Text = "Vứt Nguyên Liệu Thừa";
			this.toolTip1.SetToolTip(this.checkBox13, "Tích vào đây để AUTO tự vứt nguyên liệu thừa\r\nMỗi lần thực hiện nhận nguyên liệu mới");
			this.checkBox13.UseVisualStyleBackColor = true;
			this.checkBox13.CheckedChanged += new System.EventHandler(checkBox13_CheckedChanged);
			this.groupBox12.Controls.Add(this.radngoai);
			this.groupBox12.Controls.Add(this.radnoingoai);
			this.groupBox12.Controls.Add(this.Radnoi);
			this.groupBox12.Location = new System.Drawing.Point(0, 64);
			this.groupBox12.Name = "groupBox12";
			this.groupBox12.Size = new System.Drawing.Size(209, 42);
			this.groupBox12.TabIndex = 4;
			this.groupBox12.TabStop = false;
			this.radngoai.AutoSize = true;
			this.radngoai.Location = new System.Drawing.Point(145, 13);
			this.radngoai.Name = "radngoai";
			this.radngoai.Size = new System.Drawing.Size(53, 17);
			this.radngoai.TabIndex = 2;
			this.radngoai.Text = "Ngoại";
			this.radngoai.UseVisualStyleBackColor = true;
			this.radngoai.CheckedChanged += new System.EventHandler(radngoai_CheckedChanged);
			this.radnoingoai.AutoSize = true;
			this.radnoingoai.Checked = true;
			this.radnoingoai.Location = new System.Drawing.Point(56, 13);
			this.radnoingoai.Name = "radnoingoai";
			this.radnoingoai.Size = new System.Drawing.Size(81, 17);
			this.radnoingoai.TabIndex = 1;
			this.radnoingoai.TabStop = true;
			this.radnoingoai.Text = "Nội + Ngoại";
			this.radnoingoai.UseVisualStyleBackColor = true;
			this.radnoingoai.CheckedChanged += new System.EventHandler(radnoingoai_CheckedChanged);
			this.Radnoi.AutoSize = true;
			this.Radnoi.Location = new System.Drawing.Point(9, 13);
			this.Radnoi.Name = "Radnoi";
			this.Radnoi.Size = new System.Drawing.Size(41, 17);
			this.Radnoi.TabIndex = 0;
			this.Radnoi.Text = "Nội";
			this.Radnoi.UseVisualStyleBackColor = true;
			this.Radnoi.CheckedChanged += new System.EventHandler(Radnoi_CheckedChanged);
			this.label34.AutoSize = true;
			this.label34.Location = new System.Drawing.Point(144, 46);
			this.label34.Name = "label34";
			this.label34.Size = new System.Drawing.Size(53, 13);
			this.label34.TabIndex = 7;
			this.label34.Text = "Vật Phẩm";
			this.numericUpDown4.Location = new System.Drawing.Point(93, 43);
			this.numericUpDown4.Maximum = new decimal(new int[4] { 100000, 0, 0, 0 });
			this.numericUpDown4.Name = "numericUpDown4";
			this.numericUpDown4.Size = new System.Drawing.Size(45, 20);
			this.numericUpDown4.TabIndex = 6;
			this.numericUpDown4.ValueChanged += new System.EventHandler(numericUpDown4_ValueChanged);
			this.label35.AutoSize = true;
			this.label35.Location = new System.Drawing.Point(6, 47);
			this.label35.Name = "label35";
			this.label35.Size = new System.Drawing.Size(70, 13);
			this.label35.TabIndex = 5;
			this.label35.Text = "Tổng Số Chế";
			this.label33.AutoSize = true;
			this.label33.Location = new System.Drawing.Point(144, 21);
			this.label33.Name = "label33";
			this.label33.Size = new System.Drawing.Size(61, 13);
			this.label33.TabIndex = 4;
			this.label33.Text = "/1 Lần Chế";
			this.numsonguyenlieu.Location = new System.Drawing.Point(93, 18);
			this.numsonguyenlieu.Maximum = new decimal(new int[4] { 10000, 0, 0, 0 });
			this.numsonguyenlieu.Name = "numsonguyenlieu";
			this.numsonguyenlieu.Size = new System.Drawing.Size(45, 20);
			this.numsonguyenlieu.TabIndex = 3;
			this.numsonguyenlieu.ValueChanged += new System.EventHandler(numsonguyenlieu_ValueChanged);
			this.label32.AutoSize = true;
			this.label32.Location = new System.Drawing.Point(6, 22);
			this.label32.Name = "label32";
			this.label32.Size = new System.Drawing.Size(83, 13);
			this.label32.TabIndex = 2;
			this.label32.Text = "Số Nguyên Liệu";
			this.checkBox12.AutoSize = true;
			this.checkBox12.Location = new System.Drawing.Point(3, 1);
			this.checkBox12.Name = "checkBox12";
			this.checkBox12.Size = new System.Drawing.Size(131, 17);
			this.checkBox12.TabIndex = 1;
			this.checkBox12.Text = "Tự Nhận Nguyên Liệu";
			this.toolTip1.SetToolTip(this.checkBox12, "Tích vào đây auto sẽ tự nhân nguyên liệu miễn phí\r\nnếu thiếu nguyên liệu");
			this.checkBox12.UseVisualStyleBackColor = true;
			this.checkBox12.CheckedChanged += new System.EventHandler(checkBox12_CheckedChanged);
			this.tabautologin.Controls.Add(this.button7);
			this.tabautologin.Controls.Add(this.ListViewLogin);
			this.tabautologin.Controls.Add(this.label57);
			this.tabautologin.Controls.Add(this.ComMayChu);
			this.tabautologin.Controls.Add(this.label56);
			this.tabautologin.Controls.Add(this.label55);
			this.tabautologin.Controls.Add(this.txtmk);
			this.tabautologin.Controls.Add(this.txttk);
			this.tabautologin.Location = new System.Drawing.Point(4, 29);
			this.tabautologin.Name = "tabautologin";
			this.tabautologin.Padding = new System.Windows.Forms.Padding(3);
			this.tabautologin.Size = new System.Drawing.Size(374, 398);
			this.tabautologin.TabIndex = 6;
			this.tabautologin.Text = "AutoLogin";
			this.tabautologin.UseVisualStyleBackColor = true;
			this.tabautologin.Click += new System.EventHandler(tabautologin_Click);
			this.button7.BackColor = System.Drawing.Color.Gainsboro;
			this.button7.Location = new System.Drawing.Point(239, 38);
			this.button7.Name = "button7";
			this.button7.Size = new System.Drawing.Size(102, 23);
			this.button7.TabIndex = 16;
			this.button7.Text = "Thêm Tài Khoản";
			this.toolTip1.SetToolTip(this.button7, "Nhập tên tài khoản\r\nNhập mật khẩu\r\nChọn máy chủ \r\n==> Nhấn Thêm Tài Khoản để sử dụng chức năng tự động đăng nhập\r\n");
			this.button7.UseVisualStyleBackColor = false;
			this.button7.Click += new System.EventHandler(button7_Click);
			this.ListViewLogin.Columns.AddRange(new System.Windows.Forms.ColumnHeader[5] { this.tennhanvat, this.maychu, this.trangthai, this.nhanvat, this.phai });
			this.ListViewLogin.FullRowSelect = true;
			this.ListViewLogin.GridLines = true;
			this.ListViewLogin.HideSelection = false;
			this.ListViewLogin.Location = new System.Drawing.Point(-4, 80);
			this.ListViewLogin.Name = "ListViewLogin";
			this.ListViewLogin.Size = new System.Drawing.Size(379, 198);
			this.ListViewLogin.TabIndex = 6;
			this.toolTip1.SetToolTip(this.ListViewLogin, resources.GetString("ListViewLogin.ToolTip"));
			this.ListViewLogin.UseCompatibleStateImageBehavior = false;
			this.ListViewLogin.View = System.Windows.Forms.View.Details;
			this.ListViewLogin.DragDrop += new System.Windows.Forms.DragEventHandler(ListViewLogin_DragDrop);
			this.ListViewLogin.DoubleClick += new System.EventHandler(ListViewLogin_DoubleClick);
			this.ListViewLogin.KeyDown += new System.Windows.Forms.KeyEventHandler(ListViewLogin_KeyDown);
			this.tennhanvat.Text = "Tên Nhân Vật";
			this.tennhanvat.Width = 100;
			this.maychu.Text = "Máy Chủ";
			this.trangthai.Text = "Trạng Thái";
			this.trangthai.Width = 80;
			this.nhanvat.Text = "Nhân Vật";
			this.nhanvat.Width = 70;
			this.phai.Text = "Phái";
			this.label57.AutoSize = true;
			this.label57.Location = new System.Drawing.Point(195, 13);
			this.label57.Name = "label57";
			this.label57.Size = new System.Drawing.Size(38, 13);
			this.label57.TabIndex = 5;
			this.label57.Text = "Server";
			this.ComMayChu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComMayChu.FormattingEnabled = true;
			this.ComMayChu.Location = new System.Drawing.Point(239, 10);
			this.ComMayChu.Name = "ComMayChu";
			this.ComMayChu.Size = new System.Drawing.Size(104, 21);
			this.ComMayChu.TabIndex = 4;
			this.toolTip1.SetToolTip(this.ComMayChu, "Nhập tên tài khoản\r\nNhập mật khẩu\r\nChọn máy chủ \r\n==> Nhấn Thêm Tài Khoản để sử dụng chức năng tự động đăng nhập\r\n");
			this.label56.AutoSize = true;
			this.label56.Location = new System.Drawing.Point(37, 41);
			this.label56.Name = "label56";
			this.label56.Size = new System.Drawing.Size(23, 13);
			this.label56.TabIndex = 3;
			this.label56.Text = "MK";
			this.label55.AutoSize = true;
			this.label55.Location = new System.Drawing.Point(15, 18);
			this.label55.Name = "label55";
			this.label55.Size = new System.Drawing.Size(45, 13);
			this.label55.TabIndex = 2;
			this.label55.Text = "Tên ĐN";
			this.txtmk.Location = new System.Drawing.Point(71, 41);
			this.txtmk.Name = "txtmk";
			this.txtmk.Size = new System.Drawing.Size(88, 20);
			this.txtmk.TabIndex = 1;
			this.toolTip1.SetToolTip(this.txtmk, "Nhập tên tài khoản\r\nNhập mật khẩu\r\nChọn máy chủ \r\n==> Nhấn Thêm Tài Khoản để sử dụng chức năng tự động đăng nhập\r\n");
			this.txtmk.UseSystemPasswordChar = true;
			this.txttk.Location = new System.Drawing.Point(71, 11);
			this.txttk.Name = "txttk";
			this.txttk.Size = new System.Drawing.Size(88, 20);
			this.txttk.TabIndex = 0;
			this.toolTip1.SetToolTip(this.txttk, "Nhập tên tài khoản\r\nNhập mật khẩu\r\nChọn máy chủ \r\n==> Nhấn Thêm Tài Khoản để sử dụng chức năng tự động đăng nhập");
			this.timeMonitor.Interval = 2000;
			this.timeMonitor.Tick += new System.EventHandler(timeMonitor_Tick);
			this.notifyIcon1.Icon = (System.Drawing.Icon)resources.GetObject("notifyIcon1.Icon");
			this.notifyIcon1.Text = AppBranding.WindowTitle;
			this.notifyIcon1.Visible = true;
			this.notifyIcon1.DoubleClick += new System.EventHandler(notifyIcon1_DoubleClick);
			this.tmrLogin.Enabled = true;
			this.tmrLogin.Interval = 1000;
			this.tmrLogin.Tick += new System.EventHandler(tmrLogin_Tick);
			this.AccountLogin.DoWork += new System.ComponentModel.DoWorkEventHandler(AccountLogin_DoWork);
			this.tmrRefresh.Enabled = true;
			this.tmrRefresh.Interval = 2000;
			this.tmrRefresh.Tick += new System.EventHandler(tmrRefresh_Tick);
			this.CheDoF5.Enabled = true;
			this.CheDoF5.Interval = 1000;
			this.CheDoF5.Tick += new System.EventHandler(CheDoF5_Tick);
			this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.itemresetauto, this.ẩnGameToolStripMenuItem, this.hiệnGameToolStripMenuItem });
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(134, 70);
			this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(contextMenuStrip1_Opening);
			this.itemresetauto.Name = "itemresetauto";
			this.itemresetauto.Size = new System.Drawing.Size(133, 22);
			this.itemresetauto.Text = "Reset Auto";
			this.itemresetauto.Click += new System.EventHandler(itemresetauto_Click);
			this.ẩnGameToolStripMenuItem.Name = "ẩnGameToolStripMenuItem";
			this.ẩnGameToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
			this.ẩnGameToolStripMenuItem.Text = "Ẩn Game";
			this.ẩnGameToolStripMenuItem.Click += new System.EventHandler(ẩnGameToolStripMenuItem_Click);
			this.hiệnGameToolStripMenuItem.Name = "hiệnGameToolStripMenuItem";
			this.hiệnGameToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
			this.hiệnGameToolStripMenuItem.Text = "Hiện Game";
			this.hiệnGameToolStripMenuItem.Click += new System.EventHandler(hiệnGameToolStripMenuItem_Click);
			this.ChacterReport.Enabled = true;
			this.ChacterReport.Interval = 10000;
			this.ChacterReport.Tick += new System.EventHandler(ChacterReport_Tick);
			this.ServerConnect.DoWork += new System.ComponentModel.DoWorkEventHandler(ServerConnect_DoWork);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(382, 599);
			base.Controls.Add(this.panel1);
			base.Controls.Add(this.paneltop);
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			this.MaximumSize = new System.Drawing.Size(398, 638);
			this.MinimumSize = new System.Drawing.Size(398, 638);
			base.Name = "FrmMain";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = AppBranding.WindowTitle;
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(FrmMain_FormClosing);
			base.Load += new System.EventHandler(FrmMain_Load);
			base.Resize += new System.EventHandler(FrmMain_Resize);
			this.paneltop.ResumeLayout(false);
			this.paneltop.PerformLayout();
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.pictureBox5).EndInit();
			((System.ComponentModel.ISupportInitialize)this.pictureBox6).EndInit();
			((System.ComponentModel.ISupportInitialize)this.pictureBox3).EndInit();
			((System.ComponentModel.ISupportInitialize)this.buttheo).EndInit();
			((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
			((System.ComponentModel.ISupportInitialize)this.pictureBox2).EndInit();
			((System.ComponentModel.ISupportInitialize)this.unpickall).EndInit();
			((System.ComponentModel.ISupportInitialize)this.butpickall).EndInit();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.panellistview.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.TablControl.ResumeLayout(false);
			this.tabtophop.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.numcanhbaohp).EndInit();
			((System.ComponentModel.ISupportInitialize)this.nudNM).EndInit();
			((System.ComponentModel.ISupportInitialize)this.numhuyettemp).EndInit();
			((System.ComponentModel.ISupportInitialize)this.numbercongsinhhp).EndInit();
			((System.ComponentModel.ISupportInitialize)this.nudMP).EndInit();
			((System.ComponentModel.ISupportInitialize)this.nudHP).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.numberdanhquanh).EndInit();
			((System.ComponentModel.ISupportInitialize)this.butboqua).EndInit();
			this.tabkynang.ResumeLayout(false);
			this.TabKyNangControl.ResumeLayout(false);
			this.tabdanhquai.ResumeLayout(false);
			this.tabdanhquai.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.butthemskilldanhquai).EndInit();
			this.tabbufffhotro.ResumeLayout(false);
			this.tabbufffhotro.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.pictureBox10).EndInit();
			this.tabvatpham.ResumeLayout(false);
			this.tabvatpham.PerformLayout();
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.butitemtuanhoan).EndInit();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.numrangerpickitem).EndInit();
			((System.ComponentModel.ISupportInitialize)this.butbanvatpham).EndInit();
			((System.ComponentModel.ISupportInitialize)this.butdanhsachhuy).EndInit();
			this.tabPage2.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			this.groupBox6.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.numericUpDown3).EndInit();
			this.tabtienich.ResumeLayout(false);
			this.groupBox9.ResumeLayout(false);
			this.groupBox9.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.pictureBox12).EndInit();
			((System.ComponentModel.ISupportInitialize)this.numbankinhtheosau).EndInit();
			this.groupBox8.ResumeLayout(false);
			this.groupBox8.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.pictureBox11).EndInit();
			this.groupBox7.ResumeLayout(false);
			this.groupBox7.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.numuplevel).EndInit();
			((System.ComponentModel.ISupportInitialize)this.butdanhsachdongy).EndInit();
			this.tabchedo.ResumeLayout(false);
			this.tabchedo.PerformLayout();
			this.groupBox14.ResumeLayout(false);
			this.groupBox14.PerformLayout();
			this.groupBox11.ResumeLayout(false);
			this.groupBox11.PerformLayout();
			this.groupBox10.ResumeLayout(false);
			this.groupBox10.PerformLayout();
			this.groupBox13.ResumeLayout(false);
			this.groupBox13.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.numericUpDown6).EndInit();
			((System.ComponentModel.ISupportInitialize)this.numericUpDown5).EndInit();
			((System.ComponentModel.ISupportInitialize)this.numsosao).EndInit();
			this.groupBox12.ResumeLayout(false);
			this.groupBox12.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.numericUpDown4).EndInit();
			((System.ComponentModel.ISupportInitialize)this.numsonguyenlieu).EndInit();
			this.tabautologin.ResumeLayout(false);
			this.tabautologin.PerformLayout();
			this.contextMenuStrip1.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
