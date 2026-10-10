using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TinhKiemAuto
{
	public class ChangeLogs : Form
	{
		private IContainer components;

		private RichTextBox richTextBox1;

		public ChangeLogs()
		{
			InitializeComponent();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TinhKiemAuto.ChangeLogs));
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			base.SuspendLayout();
			this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.richTextBox1.Location = new System.Drawing.Point(0, 0);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.Size = new System.Drawing.Size(625, 490);
			this.richTextBox1.TabIndex = 0;
			this.richTextBox1.Text = AppBranding.ChangelogText;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(625, 490);
			base.Controls.Add(this.richTextBox1);
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.Name = "ChangeLogs";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = AppBranding.UpdateTitle;
			base.ResumeLayout(false);
		}
	}
}
