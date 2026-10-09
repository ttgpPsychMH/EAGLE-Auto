using System;
using System.ComponentModel;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using TinhKiemAuto.Models;

namespace TinhKiemAuto
{
	public class CanhBao : Form
	{
		public delegate void LogBack(string tieude, string noidung, Kieu type);

		public enum Kieu
		{
			OK,
			Info,
			Warning,
			Eror
		}

		private static CanhBao _instance;

		private int TopValue;

		private int invital;

		private IContainer components;

		private Label txttieude;

		private PictureBox picicon;

		private Label txtnoidung;

		private PictureBox butclose;

		private ImageList imageList1;

		private Timer timeout;

		private new Timer Show;

		private new Timer Close;

		public static CanhBao Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new CanhBao("", "", Kieu.OK);
				}
				return _instance;
			}
		}

		public static void AddLog(string tieude, string noidung, Kieu type)
		{
			if (Instance.InvokeRequired)
			{
				Instance.Invoke(new LogBack(AddLog), tieude, noidung, type);
			}
			else
			{
				new CanhBao(tieude, noidung, type).Show();
			}
		}

		public CanhBao(string tieude, string noidung, Kieu type)
		{
			InitializeComponent();
			_instance = this;
			txttieude.Text = tieude;
			txtnoidung.Text = noidung;
			switch (type)
			{
			case Kieu.OK:
				BackColor = Color.LightSeaGreen;
				picicon.Image = imageList1.Images[0];
				break;
			case Kieu.Info:
				BackColor = Color.DimGray;
				picicon.Image = imageList1.Images[1];
				break;
			case Kieu.Warning:
				BackColor = Color.DarkOrange;
				picicon.Image = imageList1.Images[2];
				break;
			case Kieu.Eror:
				BackColor = Color.Crimson;
				picicon.Image = imageList1.Images[3];
				try
				{
					using (SoundPlayer soundPlayer = new SoundPlayer("c:\\Windows\\Media\\notify.wav"))
					{
						soundPlayer.Play();
						break;
					}
				}
				catch
				{
					break;
				}
			}
		}

		public CanhBao()
		{
			InitializeComponent();
		}

		public static void Msg(string tieude, string noidung, Kieu type)
		{
			TienIch.CountMSG++;
			new CanhBao(tieude, noidung, type).Show();
		}

		private void CanhBao_Load(object sender, EventArgs e)
		{
			TopValue = base.Height * TienIch.CountMSG;
			base.Top = 0;
			base.Left = Screen.PrimaryScreen.Bounds.Width - base.Width;
			Show.Start();
		}

		private void timeout_Tick(object sender, EventArgs e)
		{
			if (TienIch.CountMSG > 0)
			{
				TienIch.CountMSG--;
			}
			Close.Start();
		}

		private void Show_Tick(object sender, EventArgs e)
		{
			if (base.Top < TopValue)
			{
				base.Top += invital;
				invital += 2;
			}
			else
			{
				Show.Stop();
			}
		}

		private void Close_Tick(object sender, EventArgs e)
		{
			if (base.Opacity > 0.0)
			{
				base.Opacity -= 0.1;
			}
			else
			{
				Close();
			}
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TinhKiemAuto.CanhBao));
			this.txttieude = new System.Windows.Forms.Label();
			this.picicon = new System.Windows.Forms.PictureBox();
			this.txtnoidung = new System.Windows.Forms.Label();
			this.butclose = new System.Windows.Forms.PictureBox();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.timeout = new System.Windows.Forms.Timer(this.components);
			this.Show = new System.Windows.Forms.Timer(this.components);
			this.Close = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)this.picicon).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.butclose).BeginInit();
			base.SuspendLayout();
			this.txttieude.AutoSize = true;
			this.txttieude.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.txttieude.ForeColor = System.Drawing.Color.White;
			this.txttieude.Location = new System.Drawing.Point(85, 9);
			this.txttieude.Name = "txttieude";
			this.txttieude.Size = new System.Drawing.Size(69, 17);
			this.txttieude.TabIndex = 2;
			this.txttieude.Text = "THIÊN HÀ";
			this.picicon.Location = new System.Drawing.Point(23, 33);
			this.picicon.Name = "picicon";
			this.picicon.Size = new System.Drawing.Size(51, 51);
			this.picicon.TabIndex = 3;
			this.picicon.TabStop = false;
			this.txtnoidung.AutoSize = true;
			this.txtnoidung.ForeColor = System.Drawing.Color.White;
			this.txtnoidung.Location = new System.Drawing.Point(85, 51);
			this.txtnoidung.Name = "txtnoidung";
			this.txtnoidung.Size = new System.Drawing.Size(71, 13);
			this.txtnoidung.TabIndex = 4;
			this.txtnoidung.Text = "Đã Chế Xong";
			this.butclose.Cursor = System.Windows.Forms.Cursors.Hand;
			this.butclose.Location = new System.Drawing.Point(262, -3);
			this.butclose.Name = "butclose";
			this.butclose.Size = new System.Drawing.Size(20, 19);
			this.butclose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.butclose.TabIndex = 5;
			this.butclose.TabStop = false;
			this.imageList1.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageList1.ImageStream");
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "Ok_50px.png");
			this.imageList1.Images.SetKeyName(1, "Info_48px.png");
			this.imageList1.Images.SetKeyName(2, "Warning Shield_52px.png");
			this.imageList1.Images.SetKeyName(3, "Delete_52px.png");
			this.timeout.Enabled = true;
			this.timeout.Interval = 5000;
			this.timeout.Tick += new System.EventHandler(timeout_Tick);
			this.Show.Interval = 10;
			this.Show.Tick += new System.EventHandler(Show_Tick);
			this.Close.Tick += new System.EventHandler(Close_Tick);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			base.ClientSize = new System.Drawing.Size(284, 111);
			base.Controls.Add(this.butclose);
			base.Controls.Add(this.txtnoidung);
			base.Controls.Add(this.picicon);
			base.Controls.Add(this.txttieude);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "CanhBao";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			this.Text = "CanhBao";
			base.Load += new System.EventHandler(CanhBao_Load);
			((System.ComponentModel.ISupportInitialize)this.picicon).EndInit();
			((System.ComponentModel.ISupportInitialize)this.butclose).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
