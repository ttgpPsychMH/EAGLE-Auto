using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Zen.Barcode;

namespace TinhKiemAuto
{
	public class ThongQR : Form
	{
		private IContainer components;

		private PictureBox pictureBox1;

		private Label txtphantcung;

		public ThongQR()
		{
			InitializeComponent();
		}

		private void ThongQR_Load(object sender, EventArgs e)
		{
			txtphantcung.Text = "Hardwave ID : " + Class95.String_0;
			CodeQrBarcodeDraw codeQr = BarcodeDrawFactory.CodeQr;
			pictureBox1.Image = codeQr.Draw(Class95.String_0, 100);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TinhKiemAuto.ThongQR));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.txtphantcung = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.pictureBox1.Location = new System.Drawing.Point(177, 33);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(221, 208);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.txtphantcung.AutoSize = true;
			this.txtphantcung.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.txtphantcung.Location = new System.Drawing.Point(32, 304);
			this.txtphantcung.Name = "txtphantcung";
			this.txtphantcung.Size = new System.Drawing.Size(96, 20);
			this.txtphantcung.TabIndex = 1;
			this.txtphantcung.Text = "Phân Cứng :";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(550, 497);
			base.Controls.Add(this.txtphantcung);
			base.Controls.Add(this.pictureBox1);
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.Name = "ThongQR";
			this.Text = "ThongQR";
			base.Load += new System.EventHandler(ThongQR_Load);
			((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
