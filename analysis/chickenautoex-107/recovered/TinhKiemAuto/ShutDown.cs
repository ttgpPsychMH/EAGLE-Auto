using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace TinhKiemAuto
{
	public class ShutDown : Form
	{
		private int exitTime = 60;

		private IContainer components;

		private Label label1;

		private Button button1;

		private Timer tmrCountDown;

		public ShutDown()
		{
			InitializeComponent();
			base.FormBorderStyle = FormBorderStyle.None;
			base.Disposed += ShutDown_Disposed;
			Show();
		}

		private void ShutDown_Disposed(object sender, EventArgs e)
		{
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void tmrCountDown_Tick(object sender, EventArgs e)
		{
			exitTime--;
			TimeSpan timeSpan = TimeSpan.FromSeconds(exitTime);
			label1.Text = "Tắt máy sau " + $"{timeSpan.Minutes:0}:{timeSpan.Seconds:00}";
			if (exitTime == 0)
			{
				Process.Start("shutdown", "-s -t 0");
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
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.tmrCountDown = new System.Windows.Forms.Timer(this.components);
			base.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label1.ForeColor = System.Drawing.SystemColors.Window;
			this.label1.Location = new System.Drawing.Point(63, 58);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(99, 25);
			this.label1.TabIndex = 0;
			this.label1.Text = "Tắt Sau :";
			this.button1.Location = new System.Drawing.Point(144, 103);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 35);
			this.button1.TabIndex = 1;
			this.button1.Text = "Hủy";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(button1_Click);
			this.tmrCountDown.Enabled = true;
			this.tmrCountDown.Interval = 1000;
			this.tmrCountDown.Tick += new System.EventHandler(tmrCountDown_Tick);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.IndianRed;
			base.ClientSize = new System.Drawing.Size(370, 160);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.label1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "ShutDown";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ShutDown";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
