using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TinhKiemAuto.CostumeControlner
{
	public class AlarmVaoPhai : UserControl
	{
		private Game game;

		private IContainer components;

		private Label lblName;

		private Label lblAlarm;

		private ComboBox cboMenpai;

		private Label label1;

		private Button btnOk;

		private Label lblClose;

		private CheckBox chkCoBan;

		public AlarmVaoPhai(Game game)
		{
			this.game = game;
			InitializeComponent();
			lblName.Text = game.TLBB.Name;
		}

		private void AlarmVaoPhai_Load(object sender, EventArgs e)
		{
			chkCoBan.Checked = game.IsNhiemVuCoBan;
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			int num = cboMenpai.SelectedIndex + 1;
			if (num == 10)
			{
				num = 32;
			}
			if (num == 11)
			{
				num = 37;
			}
			if (num == 12)
			{
				num = 0;
			}
			if (num != 0)
			{
				game.IsSetMenPai = true;
				game.SetMenPai = (TINHKIEM.Menpai)num;
			}
			Dispose();
		}

		private void lblAlarm_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				game.Active();
			}
		}

		private void lblName_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				game.Active();
			}
		}

		private void lblClose_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				base.Parent.Controls.Remove(this);
			}
		}

		private void chkCoBan_CheckedChanged(object sender, EventArgs e)
		{
			if (Global.IsVIP == 0)
			{
				chkCoBan.Checked = false;
			}
			game.IsNhiemVuCoBan = chkCoBan.Checked;
		}

		private void lblName_Click(object sender, EventArgs e)
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
			this.lblName = new System.Windows.Forms.Label();
			this.lblAlarm = new System.Windows.Forms.Label();
			this.cboMenpai = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnOk = new System.Windows.Forms.Button();
			this.lblClose = new System.Windows.Forms.Label();
			this.chkCoBan = new System.Windows.Forms.CheckBox();
			base.SuspendLayout();
			this.lblName.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.lblName.ForeColor = System.Drawing.Color.DarkGreen;
			this.lblName.Location = new System.Drawing.Point(0, 0);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(320, 23);
			this.lblName.TabIndex = 0;
			this.lblName.Text = "tinhkiem.us";
			this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblName.Click += new System.EventHandler(lblName_Click);
			this.lblName.MouseClick += new System.Windows.Forms.MouseEventHandler(lblName_MouseClick);
			this.lblAlarm.Cursor = System.Windows.Forms.Cursors.Hand;
			this.lblAlarm.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblAlarm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.lblAlarm.ForeColor = System.Drawing.Color.Red;
			this.lblAlarm.Location = new System.Drawing.Point(0, 23);
			this.lblAlarm.Name = "lblAlarm";
			this.lblAlarm.Size = new System.Drawing.Size(320, 48);
			this.lblAlarm.TabIndex = 1;
			this.lblAlarm.Text = "Tự động vào phái.\r\nChọn phái bạn muốn vào.\r\nAuto sẽ tự gia nhập phái cho bạn\r\n";
			this.lblAlarm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblAlarm.Click += new System.EventHandler(lblAlarm_Click);
			this.lblAlarm.MouseClick += new System.Windows.Forms.MouseEventHandler(lblAlarm_MouseClick);
			this.cboMenpai.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboMenpai.FormattingEnabled = true;
			this.cboMenpai.Items.AddRange(new object[11]
			{
				"Thiếu Lâm", "Minh Giáo", "Cái Bang", "Võ Đang (Đánh Xa)", "Nga My (Đánh Xa)", "Tinh Túc (Đánh Xa)", "Thiên Long (Đánh Xa)", "Thiên Sơn", "Tiêu Dao (Đánh Xa)", "Mộ Dung",
				"Để Tôi Tự Vào"
			});
			this.cboMenpai.Location = new System.Drawing.Point(59, 77);
			this.cboMenpai.Name = "cboMenpai";
			this.cboMenpai.Size = new System.Drawing.Size(170, 21);
			this.cboMenpai.TabIndex = 2;
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(5, 81);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(50, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Vào Phái";
			this.btnOk.Location = new System.Drawing.Point(235, 76);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 4;
			this.btnOk.Text = "Đồng Ý";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(btnOk_Click);
			this.lblClose.BackColor = System.Drawing.Color.Silver;
			this.lblClose.Cursor = System.Windows.Forms.Cursors.Hand;
			this.lblClose.Location = new System.Drawing.Point(300, 0);
			this.lblClose.Name = "lblClose";
			this.lblClose.Size = new System.Drawing.Size(20, 23);
			this.lblClose.TabIndex = 5;
			this.lblClose.Text = "X";
			this.lblClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblClose.MouseClick += new System.Windows.Forms.MouseEventHandler(lblClose_MouseClick);
			this.chkCoBan.AutoSize = true;
			this.chkCoBan.Location = new System.Drawing.Point(8, 104);
			this.chkCoBan.Name = "chkCoBan";
			this.chkCoBan.Size = new System.Drawing.Size(240, 17);
			this.chkCoBan.TabIndex = 6;
			this.chkCoBan.Text = "Làm nhiệm vụ cơ bản để lên cấp và lấy vàng";
			this.chkCoBan.UseVisualStyleBackColor = true;
			this.chkCoBan.CheckedChanged += new System.EventHandler(chkCoBan_CheckedChanged);
			this.BackColor = System.Drawing.Color.White;
			base.Controls.Add(this.chkCoBan);
			base.Controls.Add(this.lblClose);
			base.Controls.Add(this.btnOk);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.cboMenpai);
			base.Controls.Add(this.lblAlarm);
			base.Controls.Add(this.lblName);
			base.Name = "AlarmVaoPhai";
			base.Size = new System.Drawing.Size(320, 130);
			base.Load += new System.EventHandler(AlarmVaoPhai_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void lblAlarm_Click(object sender, EventArgs e)
		{
		}
	}
}
