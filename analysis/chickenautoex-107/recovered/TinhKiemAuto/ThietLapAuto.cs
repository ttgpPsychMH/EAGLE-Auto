using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TinhKiemAuto.Models;

namespace TinhKiemAuto
{
	public class ThietLapAuto : Form
	{
		private IContainer components;

		private Label label1;

		private ComboBox combotrilieu;

		private ComboBox combando;

		private Label label2;

		private CheckBox checkBaoPK;

		private CheckBox checkExitIfPK;

		private Button button1;

		public ThietLapAuto()
		{
			InitializeComponent();
		}

		private void ThietLapAuto_Load(object sender, EventArgs e)
		{
			checkBaoPK.Checked = Global.AlarmPk;
			checkExitIfPK.Checked = Global.ExitPk;
			combando.SelectedIndex = GetIndex(Option.MapBanDoIndex);
			combotrilieu.SelectedIndex = GetIndex(Option.MaptriLieuIndex);
		}

		private void checkBaoPK_CheckedChanged(object sender, EventArgs e)
		{
			Global.AlarmPk = checkBaoPK.Checked;
		}

		private void checktuvePk_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void checkExitIfPK_CheckedChanged(object sender, EventArgs e)
		{
			Global.ExitPk = checkExitIfPK.Checked;
		}

		public int GetIndex(int MapINDEX)
		{
			int result = 0;
			switch (MapINDEX)
			{
			case 0:
				result = 0;
				break;
			case 1:
				result = 1;
				break;
			case 2:
				result = 2;
				break;
			case 246:
				result = 3;
				break;
			}
			return result;
		}

		public int GetMapIndex(int selectIndex)
		{
			int result = 0;
			switch (selectIndex)
			{
			case 0:
				result = 0;
				break;
			case 1:
				result = 1;
				break;
			case 2:
				result = 2;
				break;
			case 3:
				result = 246;
				break;
			}
			return result;
		}

		private void combotrilieu_SelectedIndexChanged(object sender, EventArgs e)
		{
			Option.MaptriLieuIndex = GetMapIndex(combotrilieu.SelectedIndex);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void combando_SelectedIndexChanged(object sender, EventArgs e)
		{
			Option.MapBanDoIndex = GetMapIndex(combando.SelectedIndex);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TinhKiemAuto.ThietLapAuto));
			this.label1 = new System.Windows.Forms.Label();
			this.combotrilieu = new System.Windows.Forms.ComboBox();
			this.combando = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.checkBaoPK = new System.Windows.Forms.CheckBox();
			this.checkExitIfPK = new System.Windows.Forms.CheckBox();
			this.button1 = new System.Windows.Forms.Button();
			base.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(51, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(110, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Thiết lập điểm trị liệu :";
			this.combotrilieu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.combotrilieu.FormattingEnabled = true;
			this.combotrilieu.Items.AddRange(new object[4] { "Lạc Dương", "Tô Châu", "Đại Lý", "Lâu Lan" });
			this.combotrilieu.Location = new System.Drawing.Point(171, 28);
			this.combotrilieu.Name = "combotrilieu";
			this.combotrilieu.Size = new System.Drawing.Size(162, 21);
			this.combotrilieu.TabIndex = 1;
			this.combotrilieu.SelectedIndexChanged += new System.EventHandler(combotrilieu_SelectedIndexChanged);
			this.combando.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.combando.FormattingEnabled = true;
			this.combando.Items.AddRange(new object[4] { "Lạc Dương", "Tô Châu", "Đại Lý", "Lâu Lan" });
			this.combando.Location = new System.Drawing.Point(171, 64);
			this.combando.Name = "combando";
			this.combando.Size = new System.Drawing.Size(162, 21);
			this.combando.TabIndex = 3;
			this.combando.SelectedIndexChanged += new System.EventHandler(combando_SelectedIndexChanged);
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(52, 70);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(111, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Thiết lập điểm bán đồ";
			this.checkBaoPK.AutoSize = true;
			this.checkBaoPK.Location = new System.Drawing.Point(54, 115);
			this.checkBaoPK.Name = "checkBaoPK";
			this.checkBaoPK.Size = new System.Drawing.Size(126, 17);
			this.checkBaoPK.TabIndex = 4;
			this.checkBaoPK.Text = "Báo Động Nếu Bị PK";
			this.checkBaoPK.UseVisualStyleBackColor = true;
			this.checkBaoPK.CheckedChanged += new System.EventHandler(checkBaoPK_CheckedChanged);
			this.checkExitIfPK.AutoSize = true;
			this.checkExitIfPK.Location = new System.Drawing.Point(54, 153);
			this.checkExitIfPK.Name = "checkExitIfPK";
			this.checkExitIfPK.Size = new System.Drawing.Size(106, 17);
			this.checkExitIfPK.TabIndex = 6;
			this.checkExitIfPK.Text = "Thoát Nếu Bị PK";
			this.checkExitIfPK.UseVisualStyleBackColor = true;
			this.checkExitIfPK.CheckedChanged += new System.EventHandler(checkExitIfPK_CheckedChanged);
			this.button1.Location = new System.Drawing.Point(152, 228);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 7;
			this.button1.Text = "Lưu";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(button1_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(421, 274);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.checkExitIfPK);
			base.Controls.Add(this.checkBaoPK);
			base.Controls.Add(this.combando);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.combotrilieu);
			base.Controls.Add(this.label1);
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ThietLapAuto";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ThietLapAuto";
			base.Load += new System.EventHandler(ThietLapAuto_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
