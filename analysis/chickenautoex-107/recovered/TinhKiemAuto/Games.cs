using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TinhKiemAuto
{
	internal class Games : Form
	{
		public const int WS_BORDER = 8388608;

		public const int WS_DLGFRAME = 4194304;

		public const int WS_CAPTION = 12582912;

		public const int WS_SYSMENU = 524288;

		public const int WS_THICKFRAME = 262144;

		public const int WS_MINIMIZE = 536870912;

		public const int WS_MAXIMIZEBOX = 65536;

		public const int GWL_STYLE = -16;

		public const int GWL_EXSTYLE = -20;

		public const int WS_EX_DLGMODALFRAME = 1;

		public const int SWP_NOMOVE = 2;

		public const int SWP_NOSIZE = 1;

		public const int SWP_FRAMECHANGED = 32;

		public const uint MF_BYPOSITION = 1024u;

		public const uint MF_REMOVE = 4096u;

		public const int WS_MINIMIZEBOX = 131072;

		public static Games instance;

		private bool IsResize;

		private const int WmPaint = 15;

		private IContainer components;

		private TabControl tab;

		private Timer timer1;

		private Timer timer2;

		public static Games Instance
		{
			get
			{
				if (instance == null || instance.IsDisposed)
				{
					instance = new Games();
				}
				return instance;
			}
		}

		[DllImport("user32.dll")]
		public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		[DllImport("USER32.DLL")]
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		public Games()
		{
			InitializeComponent();
			instance = this;
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			foreach (KeyValuePair<int, Game> item in FrmMain.dicGame)
			{
				if (item.Value.Parrent == IntPtr.Zero)
				{
					if (item.Value.TLBB.Online)
					{
						BindGame(item.Value);
					}
				}
				else
				{
					item.Value.Parrent = tab.SelectedTab.Handle;
				}
			}
			foreach (TabPage tabPage in tab.TabPages)
			{
				if (tabPage.Tag != null && ((Game)tabPage.Tag).Parrent == IntPtr.Zero)
				{
					tab.TabPages.Remove(tabPage);
				}
			}
		}

		private void BindGame(Game game)
		{
			int num = (game.style = GetWindowLong(game.Handle, -16));
			num &= -12582913;
			num &= -536870913;
			num &= -65537;
			num &= -131073;
			TabPage tabPage = new TabPage(game.TLBB.Name);
			tabPage.Tag = game;
			tab.TabPages.Add(tabPage);
			Win.Active(game.Handle);
			SetParent(game.Handle, tabPage.Handle);
			SetWindowLong(game.Handle, -16, num);
			MoveWindow(game.Handle, 0, 0, tabPage.Width, tabPage.Height, bRepaint: true);
			game.LuaDoString("PushEvent('VIEW_RESOLUTION_CHANGED')");
			game.Parrent = tabPage.Handle;
		}

		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr SetFocus(IntPtr hWnd);

		private void Games_FormClosing(object sender, FormClosingEventArgs e)
		{
			foreach (KeyValuePair<int, Game> item in FrmMain.dicGame)
			{
				if (item.Value.Parrent != IntPtr.Zero)
				{
					SetParent(item.Value.Handle, IntPtr.Zero);
					item.Value.Parrent = IntPtr.Zero;
					SetWindowLong(item.Value.Handle, -16, item.Value.style);
				}
			}
		}

		private void tab_MouseClick(object sender, MouseEventArgs e)
		{
		}

		private void Games_ResizeEnd(object sender, EventArgs e)
		{
			IsResize = true;
		}

		private void Games_Resize(object sender, EventArgs e)
		{
			IsResize = true;
		}

		private void Games_SizeChanged(object sender, EventArgs e)
		{
		}

		private void Games_MouseUp(object sender, MouseEventArgs e)
		{
		}

		private void Games_MouseEnter(object sender, EventArgs e)
		{
		}

		private void Games_Validated(object sender, EventArgs e)
		{
		}

		private void ResizeGame()
		{
			if (IsResize && tab.SelectedTab != null && tab.SelectedTab.Tag != null)
			{
				IsResize = false;
				Game game = tab.SelectedTab.Tag as Game;
				Win.GetWindowRect(game.Handle, out var lpRect);
				if (lpRect.Left + lpRect.Right != 0 || lpRect.Left == -32000 || TINHKIEM.NumDiff(lpRect.Right - lpRect.Left + 1, tab.SelectedTab.Width) > 10)
				{
					MoveWindow(game.Handle, 0, 0, tab.SelectedTab.Width, tab.SelectedTab.Height, bRepaint: false);
					ForcePaint(game.Handle);
				}
			}
		}

		private void timer2_Tick(object sender, EventArgs e)
		{
			ResizeGame();
		}

		[DllImport("User32.dll")]
		public static extern long SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

		public static void ForcePaint(IntPtr handle)
		{
			SendMessage(handle, 15u, IntPtr.Zero, IntPtr.Zero);
		}

		private void Games_Load(object sender, EventArgs e)
		{
		}

		private void Games_Load_1(object sender, EventArgs e)
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TinhKiemAuto.Games));
			this.tab = new System.Windows.Forms.TabControl();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.timer2 = new System.Windows.Forms.Timer(this.components);
			base.SuspendLayout();
			this.tab.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tab.Location = new System.Drawing.Point(0, 0);
			this.tab.Name = "tab";
			this.tab.SelectedIndex = 0;
			this.tab.Size = new System.Drawing.Size(984, 761);
			this.tab.TabIndex = 0;
			this.tab.MouseClick += new System.Windows.Forms.MouseEventHandler(tab_MouseClick);
			this.timer1.Enabled = true;
			this.timer1.Interval = 300;
			this.timer1.Tick += new System.EventHandler(timer1_Tick);
			this.timer2.Enabled = true;
			this.timer2.Interval = 1000;
			this.timer2.Tick += new System.EventHandler(timer2_Tick);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(984, 761);
			base.Controls.Add(this.tab);
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.Name = "Games";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Games - Hiển thị Game thành tab";
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Games_FormClosing);
			base.Load += new System.EventHandler(Games_Load_1);
			base.ResizeEnd += new System.EventHandler(Games_ResizeEnd);
			base.SizeChanged += new System.EventHandler(Games_SizeChanged);
			base.MouseEnter += new System.EventHandler(Games_MouseEnter);
			base.MouseUp += new System.Windows.Forms.MouseEventHandler(Games_MouseUp);
			base.Resize += new System.EventHandler(Games_Resize);
			base.Validated += new System.EventHandler(Games_Validated);
			base.ResumeLayout(false);
		}
	}
}
