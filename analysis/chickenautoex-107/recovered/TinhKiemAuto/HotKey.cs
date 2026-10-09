using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TinhKiemAuto
{
	public class HotKey : Form
	{
		private IContainer components;

		private Label label20;

		private Label label9;

		private Label label8;

		private Label label7;

		private Label label6;

		private Label label5;

		private Label label4;

		private Label label3;

		private Label label2;

		private Label label1;

		private Label label19;

		private Label label18;

		private Label label17;

		private Label label15;

		private Label label14;

		private Label label13;

		private Label label12;

		private Label label11;

		private Label label10;

		private Button button1;

		private Button button2;

		public HotKey()
		{
			InitializeComponent();
		}

		private void HotKey_Load(object sender, EventArgs e)
		{
		}

		private void button1_Click(object sender, EventArgs e)
		{
			FrmMain.Instance.UnSetHoKey();
			MessageBox.Show("Đã tắt HotKey");
		}

		private void button2_Click(object sender, EventArgs e)
		{
			FrmMain.Instance.SetHotKey();
			MessageBox.Show("Đã bật Hotkey");
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TinhKiemAuto.HotKey));
			this.label20 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.label18 = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			base.SuspendLayout();
			this.label20.AutoSize = true;
			this.label20.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label20.ForeColor = System.Drawing.Color.Black;
			this.label20.Location = new System.Drawing.Point(48, 392);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(177, 17);
			this.label20.TabIndex = 31;
			this.label20.Text = "Ngừng Tất Cả Auto : Alt + F3";
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label9.ForeColor = System.Drawing.Color.Black;
			this.label9.Location = new System.Drawing.Point(51, 358);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(160, 17);
			this.label9.TabIndex = 30;
			this.label9.Text = "Mở Thêm Game : Ctrl + O";
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label8.ForeColor = System.Drawing.Color.Black;
			this.label8.Location = new System.Drawing.Point(48, 324);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(283, 17);
			this.label8.TabIndex = 29;
			this.label8.Text = "Triệu Tập Chạy Về Nhân Vật Chỉ Định  : Alt + F2";
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label7.ForeColor = System.Drawing.Color.Black;
			this.label7.Location = new System.Drawing.Point(48, 290);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(269, 17);
			this.label7.TabIndex = 28;
			this.label7.Text = "Triệu Tập Thành Viên Chạy Tới Key  : Alt + F1";
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label6.ForeColor = System.Drawing.Color.Black;
			this.label6.Location = new System.Drawing.Point(48, 256);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(149, 17);
			this.label6.TabIndex = 27;
			this.label6.Text = "Bật Tắt Trị Liệu : Ctrl + R";
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label5.ForeColor = System.Drawing.Color.Black;
			this.label5.Location = new System.Drawing.Point(48, 222);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(169, 17);
			this.label5.TabIndex = 26;
			this.label5.Text = "Ẩn Hiện Game  : Ctrl + END";
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label4.ForeColor = System.Drawing.Color.Black;
			this.label4.Location = new System.Drawing.Point(48, 188);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(168, 17);
			this.label4.TabIndex = 25;
			this.label4.Text = "Bặt Tắt AutoTrain  : Ctrl + G";
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label3.ForeColor = System.Drawing.Color.Black;
			this.label3.Location = new System.Drawing.Point(48, 154);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(163, 17);
			this.label3.TabIndex = 24;
			this.label3.Text = "Bật Tắt Theo Key : Ctrl + D";
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label2.ForeColor = System.Drawing.Color.Black;
			this.label2.Location = new System.Drawing.Point(48, 120);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(124, 17);
			this.label2.TabIndex = 23;
			this.label2.Text = "Bật Tắt Auto : Pause";
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label1.ForeColor = System.Drawing.Color.Black;
			this.label1.Location = new System.Drawing.Point(48, 86);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(203, 17);
			this.label1.TabIndex = 22;
			this.label1.Text = "Bật Tắt Nhặt Đồ :  Ctrl + Shift + C";
			this.label19.AutoSize = true;
			this.label19.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label19.ForeColor = System.Drawing.Color.Black;
			this.label19.Location = new System.Drawing.Point(381, 358);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(117, 17);
			this.label19.TabIndex = 40;
			this.label19.Text = "Hái Dược  : Ctrl + I";
			this.label18.AutoSize = true;
			this.label18.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label18.ForeColor = System.Drawing.Color.Black;
			this.label18.Location = new System.Drawing.Point(381, 324);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(123, 17);
			this.label18.TabIndex = 39;
			this.label18.Text = "Mời  Đội  : Ctrl + M";
			this.label17.AutoSize = true;
			this.label17.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label17.ForeColor = System.Drawing.Color.Black;
			this.label17.Location = new System.Drawing.Point(381, 290);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(166, 17);
			this.label17.TabIndex = 38;
			this.label17.Text = "Mở Tàng Bảo Đồ  : Ctrl + T";
			this.label15.AutoSize = true;
			this.label15.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label15.ForeColor = System.Drawing.Color.Black;
			this.label15.Location = new System.Drawing.Point(381, 256);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(239, 17);
			this.label15.TabIndex = 37;
			this.label15.Text = "Thoát Nhân Vật Hiện Tại  : Ctrl + Delete";
			this.label14.AutoSize = true;
			this.label14.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label14.ForeColor = System.Drawing.Color.Black;
			this.label14.Location = new System.Drawing.Point(381, 222);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(153, 17);
			this.label14.TabIndex = 36;
			this.label14.Text = "Quay Về Đại Lý : Ctrl + Q";
			this.label13.AutoSize = true;
			this.label13.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label13.ForeColor = System.Drawing.Color.Black;
			this.label13.Location = new System.Drawing.Point(381, 188);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(200, 17);
			this.label13.TabIndex = 35;
			this.label13.Text = "Bật/Tắt Cất Đồ Vào Kho : Ctrl + L";
			this.label12.AutoSize = true;
			this.label12.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label12.ForeColor = System.Drawing.Color.Black;
			this.label12.Location = new System.Drawing.Point(381, 154);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(166, 17);
			this.label12.TabIndex = 34;
			this.label12.Text = "Lên/Xuống Ngựa : Ctrl + N";
			this.label11.AutoSize = true;
			this.label11.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label11.ForeColor = System.Drawing.Color.Black;
			this.label11.Location = new System.Drawing.Point(381, 120);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(250, 17);
			this.label11.TabIndex = 33;
			this.label11.Text = "Bật/Tắt Hủy đồ Theo Danh Sách : Ctrl + H";
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label10.ForeColor = System.Drawing.Color.Black;
			this.label10.Location = new System.Drawing.Point(381, 86);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(247, 17);
			this.label10.TabIndex = 32;
			this.label10.Text = "Bật/Tắt Bán đồ Theo Danh Sách : Ctrl + B";
			this.button1.Location = new System.Drawing.Point(185, 433);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(99, 23);
			this.button1.TabIndex = 41;
			this.button1.Text = "Tắt Phím Tắt";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(button1_Click);
			this.button2.Location = new System.Drawing.Point(357, 433);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(118, 23);
			this.button2.TabIndex = 42;
			this.button2.Text = "Bật Phím Tắt";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(button2_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(667, 497);
			base.Controls.Add(this.button2);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.label19);
			base.Controls.Add(this.label18);
			base.Controls.Add(this.label17);
			base.Controls.Add(this.label15);
			base.Controls.Add(this.label14);
			base.Controls.Add(this.label13);
			base.Controls.Add(this.label12);
			base.Controls.Add(this.label11);
			base.Controls.Add(this.label10);
			base.Controls.Add(this.label20);
			base.Controls.Add(this.label9);
			base.Controls.Add(this.label8);
			base.Controls.Add(this.label7);
			base.Controls.Add(this.label6);
			base.Controls.Add(this.label5);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(683, 536);
			base.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(683, 536);
			base.Name = "HotKey";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Phím Tắt";
			base.Load += new System.EventHandler(HotKey_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
