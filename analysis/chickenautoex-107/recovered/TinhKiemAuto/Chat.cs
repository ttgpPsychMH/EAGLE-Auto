using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TinhKiemAuto
{
	public class Chat : Form
	{
		private IContainer components;

		private CheckBox checknear;

		private CheckBox checkbig_world;

		private CheckBox checkmenpai;

		private CheckBox checkteam;

		private CheckBox checkguild;

		private CheckBox checkguild_league;

		private Button button1;

		public Chat()
		{
			InitializeComponent();
		}

		private void checknear_CheckedChanged(object sender, EventArgs e)
		{
			FrmMain.CurGame.ChatGan = checknear.Checked;
		}

		private void checkbig_world_CheckedChanged(object sender, EventArgs e)
		{
			FrmMain.CurGame.ChatTheGioi = checkbig_world.Checked;
		}

		private void checkmenpai_CheckedChanged(object sender, EventArgs e)
		{
			FrmMain.CurGame.ChatMonPhai = checkmenpai.Checked;
		}

		private void checkteam_CheckedChanged(object sender, EventArgs e)
		{
			FrmMain.CurGame.ChatDoi = checkteam.Checked;
		}

		private void checkguild_CheckedChanged(object sender, EventArgs e)
		{
			FrmMain.CurGame.ChatBangPhai = checkguild.Checked;
		}

		private void checkguild_league_CheckedChanged(object sender, EventArgs e)
		{
			FrmMain.CurGame.ChatDongMinh = checkguild_league.Checked;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Chat_Load(object sender, EventArgs e)
		{
			checknear.Checked = FrmMain.CurGame.ChatGan;
			checkbig_world.Checked = FrmMain.CurGame.ChatTheGioi;
			checkmenpai.Checked = FrmMain.CurGame.ChatMonPhai;
			checkteam.Checked = FrmMain.CurGame.ChatDoi;
			checkguild.Checked = FrmMain.CurGame.ChatBangPhai;
			checkguild_league.Checked = FrmMain.CurGame.ChatDongMinh;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TinhKiemAuto.Chat));
			this.checknear = new System.Windows.Forms.CheckBox();
			this.checkbig_world = new System.Windows.Forms.CheckBox();
			this.checkmenpai = new System.Windows.Forms.CheckBox();
			this.checkteam = new System.Windows.Forms.CheckBox();
			this.checkguild = new System.Windows.Forms.CheckBox();
			this.checkguild_league = new System.Windows.Forms.CheckBox();
			this.button1 = new System.Windows.Forms.Button();
			base.SuspendLayout();
			this.checknear.AutoSize = true;
			this.checknear.Location = new System.Drawing.Point(56, 28);
			this.checknear.Name = "checknear";
			this.checknear.Size = new System.Drawing.Size(71, 17);
			this.checknear.TabIndex = 0;
			this.checknear.Text = "Chat Gần";
			this.checknear.UseVisualStyleBackColor = true;
			this.checknear.CheckedChanged += new System.EventHandler(checknear_CheckedChanged);
			this.checkbig_world.AutoSize = true;
			this.checkbig_world.Location = new System.Drawing.Point(56, 71);
			this.checkbig_world.Name = "checkbig_world";
			this.checkbig_world.Size = new System.Drawing.Size(66, 17);
			this.checkbig_world.TabIndex = 1;
			this.checkbig_world.Text = "Thế Giới";
			this.checkbig_world.UseVisualStyleBackColor = true;
			this.checkbig_world.CheckedChanged += new System.EventHandler(checkbig_world_CheckedChanged);
			this.checkmenpai.AutoSize = true;
			this.checkmenpai.Location = new System.Drawing.Point(56, 114);
			this.checkmenpai.Name = "checkmenpai";
			this.checkmenpai.Size = new System.Drawing.Size(96, 17);
			this.checkmenpai.TabIndex = 2;
			this.checkmenpai.Text = "Chat Môn Phái";
			this.checkmenpai.UseVisualStyleBackColor = true;
			this.checkmenpai.CheckedChanged += new System.EventHandler(checkmenpai_CheckedChanged);
			this.checkteam.AutoSize = true;
			this.checkteam.Location = new System.Drawing.Point(56, 157);
			this.checkteam.Name = "checkteam";
			this.checkteam.Size = new System.Drawing.Size(67, 17);
			this.checkteam.TabIndex = 3;
			this.checkteam.Text = "Chat Đội";
			this.checkteam.UseVisualStyleBackColor = true;
			this.checkteam.CheckedChanged += new System.EventHandler(checkteam_CheckedChanged);
			this.checkguild.AutoSize = true;
			this.checkguild.Location = new System.Drawing.Point(56, 200);
			this.checkguild.Name = "checkguild";
			this.checkguild.Size = new System.Drawing.Size(76, 17);
			this.checkguild.TabIndex = 4;
			this.checkguild.Text = "Chat Bang";
			this.checkguild.UseVisualStyleBackColor = true;
			this.checkguild.CheckedChanged += new System.EventHandler(checkguild_CheckedChanged);
			this.checkguild_league.AutoSize = true;
			this.checkguild_league.Location = new System.Drawing.Point(56, 243);
			this.checkguild_league.Name = "checkguild_league";
			this.checkguild_league.Size = new System.Drawing.Size(78, 17);
			this.checkguild_league.TabIndex = 5;
			this.checkguild_league.Text = "Đồng Minh";
			this.checkguild_league.UseVisualStyleBackColor = true;
			this.checkguild_league.CheckedChanged += new System.EventHandler(checkguild_league_CheckedChanged);
			this.button1.Location = new System.Drawing.Point(56, 283);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 6;
			this.button1.Text = "OK";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(button1_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(190, 332);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.checkguild_league);
			base.Controls.Add(this.checkguild);
			base.Controls.Add(this.checkteam);
			base.Controls.Add(this.checkmenpai);
			base.Controls.Add(this.checkbig_world);
			base.Controls.Add(this.checknear);
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "Chat";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Cài Đặt Chat";
			base.Load += new System.EventHandler(Chat_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
