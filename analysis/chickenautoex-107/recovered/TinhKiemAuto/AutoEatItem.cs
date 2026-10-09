using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;
using TinhKiemAuto.Models;

namespace TinhKiemAuto
{
	public class AutoEatItem : Form
	{
		private IContainer components;

		private ListView listViewName;

		private ColumnHeader buttenquai;

		private ListView listViewDuocAn;

		private ColumnHeader columnHeader1;

		private Label label5;

		private Label label4;

		private Button button1;

		private Label label3;

		private Label label23;

		private NumericUpDown numphut;

		private Button button2;

		public AutoEatItem()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			List<AutoEat> list = new List<AutoEat>();
			foreach (ListViewItem item2 in listViewDuocAn.Items)
			{
				AutoEat item = (AutoEat)item2.Tag;
				list.Add(item);
			}
			Setting.AutoEat = JsonConvert.SerializeObject(list);
			FrmMain.CurGame.TudongAn = list;
			Close();
		}

		public void LoadData()
		{
			if (Setting.AutoEat.Length <= 0)
			{
				return;
			}
			foreach (AutoEat item in JsonConvert.DeserializeObject<List<AutoEat>>(Setting.AutoEat))
			{
				ListViewItem listViewItem = new ListViewItem
				{
					UseItemStyleForSubItems = false
				};
				listViewItem.Text = item.VatPhamName + " | " + item.TimeEach;
				listViewItem.Tag = item;
				listViewDuocAn.Items.Add(listViewItem);
			}
		}

		private void AutoEatItem_Load(object sender, EventArgs e)
		{
			HashSet<string> hashSet = new HashSet<string>();
			new HashSet<string>();
			foreach (KeyValuePair<int, Game> item in FrmMain.dicGame)
			{
				foreach (PacketItem item2 in PacketItem.EnumDrop(item.Value))
				{
					if (!(item2.Name.Trim() == "") && !(item2.TypeName.Trim() == "") && !item2.Name.Contains("_XML_") && !hashSet.Contains(item2.Name))
					{
						hashSet.Add(item2.Name);
						listViewName.Items.Add(item2.Name);
					}
				}
			}
			LoadData();
		}

		public void AddName()
		{
			foreach (ListViewItem selectedItem in listViewName.SelectedItems)
			{
				if (selectedItem.Text.Trim() != "")
				{
					ListViewItem listViewItem2 = new ListViewItem
					{
						UseItemStyleForSubItems = false
					};
					listViewItem2.Text = selectedItem.Text.Trim() + "  |  " + (int)numphut.Value;
					AutoEat autoEat = new AutoEat();
					autoEat.StartEat = DateTime.Now;
					autoEat.ID = new Random().Next(1, 99999);
					autoEat.VatPhamName = selectedItem.Text.Trim();
					autoEat.TimeEach = (int)numphut.Value;
					listViewItem2.Tag = autoEat;
					listViewDuocAn.Items.Add(listViewItem2);
				}
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			AddName();
		}

		private void listViewDuocAn_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Delete || listViewDuocAn.SelectedItems.Count == 0)
			{
				return;
			}
			foreach (ListViewItem selectedItem in listViewDuocAn.SelectedItems)
			{
				try
				{
					selectedItem.Remove();
				}
				catch
				{
				}
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TinhKiemAuto.AutoEatItem));
			this.listViewName = new System.Windows.Forms.ListView();
			this.buttenquai = new System.Windows.Forms.ColumnHeader();
			this.listViewDuocAn = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label23 = new System.Windows.Forms.Label();
			this.numphut = new System.Windows.Forms.NumericUpDown();
			this.button2 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)this.numphut).BeginInit();
			base.SuspendLayout();
			this.listViewName.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1] { this.buttenquai });
			this.listViewName.GridLines = true;
			this.listViewName.HideSelection = false;
			this.listViewName.Location = new System.Drawing.Point(12, 40);
			this.listViewName.Name = "listViewName";
			this.listViewName.Size = new System.Drawing.Size(203, 276);
			this.listViewName.TabIndex = 3;
			this.listViewName.UseCompatibleStateImageBehavior = false;
			this.listViewName.View = System.Windows.Forms.View.Details;
			this.buttenquai.Text = "Danh Sách Vật Phẩm Trong Túi";
			this.buttenquai.Width = 180;
			this.listViewDuocAn.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1] { this.columnHeader1 });
			this.listViewDuocAn.GridLines = true;
			this.listViewDuocAn.HideSelection = false;
			this.listViewDuocAn.Location = new System.Drawing.Point(355, 40);
			this.listViewDuocAn.Name = "listViewDuocAn";
			this.listViewDuocAn.Size = new System.Drawing.Size(205, 276);
			this.listViewDuocAn.TabIndex = 4;
			this.listViewDuocAn.UseCompatibleStateImageBehavior = false;
			this.listViewDuocAn.View = System.Windows.Forms.View.Details;
			this.listViewDuocAn.KeyDown += new System.Windows.Forms.KeyEventHandler(listViewDuocAn_KeyDown);
			this.columnHeader1.Text = "Danh Sách Sử Dụng Tự Động";
			this.columnHeader1.Width = 200;
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25f, System.Drawing.FontStyle.Bold);
			this.label5.ForeColor = System.Drawing.Color.Red;
			this.label5.Location = new System.Drawing.Point(12, 339);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(411, 13);
			this.label5.TabIndex = 71;
			this.label5.Text = "- Auto sẽ tự ăn tuần hoàn theo thời gian chỉ định nếu trong túi vẫn còn vật phẩm";
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25f, System.Drawing.FontStyle.Bold);
			this.label4.ForeColor = System.Drawing.Color.Red;
			this.label4.Location = new System.Drawing.Point(12, 363);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(243, 13);
			this.label4.TabIndex = 70;
			this.label4.Text = "- Nhấn Delete để xóa vật phẩm khỏi danh sách";
			this.button1.Location = new System.Drawing.Point(476, 371);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 72;
			this.button1.Text = "Lưu Lại";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(button1_Click);
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.label3.ForeColor = System.Drawing.Color.Black;
			this.label3.Location = new System.Drawing.Point(217, 112);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(41, 13);
			this.label3.TabIndex = 74;
			this.label3.Text = "Ăn Sau";
			this.label23.AutoSize = true;
			this.label23.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.label23.ForeColor = System.Drawing.Color.Black;
			this.label23.Location = new System.Drawing.Point(320, 112);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(29, 13);
			this.label23.TabIndex = 73;
			this.label23.Text = "Phút";
			this.numphut.Location = new System.Drawing.Point(264, 107);
			this.numphut.Maximum = new decimal(new int[4] { 200, 0, 0, 0 });
			this.numphut.Name = "numphut";
			this.numphut.Size = new System.Drawing.Size(43, 20);
			this.numphut.TabIndex = 75;
			this.button2.Location = new System.Drawing.Point(249, 150);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 76;
			this.button2.Text = "--->>";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(button2_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(572, 405);
			base.Controls.Add(this.button2);
			base.Controls.Add(this.numphut);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label23);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.label5);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.listViewDuocAn);
			base.Controls.Add(this.listViewName);
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(588, 444);
			base.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(588, 444);
			base.Name = "AutoEatItem";
			this.Text = "AutoEatItem";
			base.Load += new System.EventHandler(AutoEatItem_Load);
			((System.ComponentModel.ISupportInitialize)this.numphut).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
