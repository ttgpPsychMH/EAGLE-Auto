using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TinhKiemAuto.Models;

namespace TinhKiemAuto
{
	public class SellItem : Form
	{
		private IContainer components;

		private Button button1;

		private Label label4;

		private Label label8;

		private Button buthemkieu;

		private Button buthem;

		private ListView listViewTypeEx;

		private ColumnHeader loaivatphamhuy;

		private ListView listViewType;

		private ColumnHeader loaivatpham;

		private ListView ListViewNameEx;

		private ColumnHeader tenvatphamboqua;

		private ListView listViewName;

		private ColumnHeader buttenvatpham;

		private Label label9;

		public SellItem()
		{
			InitializeComponent();
		}

		private void SellItem_Load(object sender, EventArgs e)
		{
			HashSet<string> hashSet = new HashSet<string>();
			HashSet<string> hashSet2 = new HashSet<string>();
			foreach (KeyValuePair<int, Game> item in FrmMain.dicGame)
			{
				foreach (PacketItem item2 in PacketItem.EnumDrop(item.Value))
				{
					if (!(item2.Name.Trim() == "") && !(item2.TypeName.Trim() == ""))
					{
						if (!hashSet.Contains(item2.Name))
						{
							hashSet.Add(item2.Name);
							listViewName.Items.Add(item2.Name);
						}
						if (!hashSet2.Contains(item2.TypeName))
						{
							hashSet2.Add(item2.TypeName);
							listViewType.Items.Add(item2.TypeName);
						}
					}
				}
			}
			loadData();
		}

		private void AddName()
		{
			foreach (ListViewItem selectedItem in listViewName.SelectedItems)
			{
				if (selectedItem.Text.Trim() != "" && ListViewNameEx.FindItemWithText(selectedItem.Text.Trim()) == null)
				{
					ListViewNameEx.Items.Add(selectedItem.Text.Trim());
				}
			}
		}

		private void AddType()
		{
			foreach (ListViewItem selectedItem in listViewType.SelectedItems)
			{
				if (selectedItem.Text.Trim() != "" && listViewTypeEx.FindItemWithText(selectedItem.Text.Trim()) == null)
				{
					listViewTypeEx.Items.Add(selectedItem.Text.Trim());
				}
			}
		}

		public void Save()
		{
			string text = "";
			string text2 = "";
			foreach (ListViewItem item in ListViewNameEx.Items)
			{
				if (item.Text.Trim() != "")
				{
					text = text + item.Text.Trim() + "\n";
				}
			}
			foreach (ListViewItem item2 in listViewTypeEx.Items)
			{
				if (item2.Text.Trim() != "")
				{
					text2 = text2 + item2.Text.Trim() + "\n";
				}
			}
			LoadFile.WriteFileWithEncrypt(text, Global.DataPath + "\\SellName.dat");
			LoadFile.WriteFileWithEncrypt(text2, Global.DataPath + "\\SellType.dat");
		}

		public void loadData()
		{
			string sellName = Setting.SellName;
			string sellType = Setting.SellType;
			string[] array = sellName.Split('\n');
			foreach (string text in array)
			{
				if (text.Length > 0)
				{
					ListViewNameEx.Items.Add(text);
				}
			}
			array = sellType.Split('\n');
			foreach (string text2 in array)
			{
				if (text2.Length > 0)
				{
					listViewTypeEx.Items.Add(text2);
				}
			}
		}

		private void listViewName_DoubleClick(object sender, EventArgs e)
		{
			AddName();
		}

		private void buthem_Click(object sender, EventArgs e)
		{
			AddName();
		}

		private void ListViewNameEx_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Delete || ListViewNameEx.SelectedItems.Count == 0)
			{
				return;
			}
			foreach (ListViewItem selectedItem in ListViewNameEx.SelectedItems)
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

		private void listViewTypeEx_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Delete || listViewTypeEx.SelectedItems.Count == 0)
			{
				return;
			}
			foreach (ListViewItem selectedItem in listViewTypeEx.SelectedItems)
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

		private void button1_Click(object sender, EventArgs e)
		{
			Save();
			Close();
		}

		private void listViewType_DoubleClick(object sender, EventArgs e)
		{
			AddType();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TinhKiemAuto.SellItem));
			this.button1 = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.buthemkieu = new System.Windows.Forms.Button();
			this.buthem = new System.Windows.Forms.Button();
			this.listViewTypeEx = new System.Windows.Forms.ListView();
			this.loaivatphamhuy = new System.Windows.Forms.ColumnHeader();
			this.listViewType = new System.Windows.Forms.ListView();
			this.loaivatpham = new System.Windows.Forms.ColumnHeader();
			this.ListViewNameEx = new System.Windows.Forms.ListView();
			this.tenvatphamboqua = new System.Windows.Forms.ColumnHeader();
			this.listViewName = new System.Windows.Forms.ListView();
			this.buttenvatpham = new System.Windows.Forms.ColumnHeader();
			this.label9 = new System.Windows.Forms.Label();
			base.SuspendLayout();
			this.button1.Location = new System.Drawing.Point(497, 606);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 29);
			this.button1.TabIndex = 80;
			this.button1.Text = "Lưu Lại";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(button1_Click);
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25f, System.Drawing.FontStyle.Bold);
			this.label4.ForeColor = System.Drawing.Color.Red;
			this.label4.Location = new System.Drawing.Point(20, 604);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(267, 13);
			this.label4.TabIndex = 79;
			this.label4.Text = "- Nhấn Delete Trên Phím Để Xóa Đồ Khỏi Danh Sách";
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25f, System.Drawing.FontStyle.Bold);
			this.label8.ForeColor = System.Drawing.Color.Red;
			this.label8.Location = new System.Drawing.Point(20, 624);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(441, 13);
			this.label8.TabIndex = 78;
			this.label8.Text = "- Cẩn trọng khi sử dụng tính năng Hủy Đồ + Bán Đồ theo loại vì rất nhiều đồ cùng loại";
			this.buthemkieu.Location = new System.Drawing.Point(255, 430);
			this.buthemkieu.Name = "buthemkieu";
			this.buthemkieu.Size = new System.Drawing.Size(75, 29);
			this.buthemkieu.TabIndex = 77;
			this.buthemkieu.Text = "---->>>";
			this.buthemkieu.UseVisualStyleBackColor = true;
			this.buthem.Location = new System.Drawing.Point(255, 139);
			this.buthem.Name = "buthem";
			this.buthem.Size = new System.Drawing.Size(75, 32);
			this.buthem.TabIndex = 76;
			this.buthem.Text = "---->>>";
			this.buthem.UseVisualStyleBackColor = true;
			this.buthem.Click += new System.EventHandler(buthem_Click);
			this.listViewTypeEx.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1] { this.loaivatphamhuy });
			this.listViewTypeEx.GridLines = true;
			this.listViewTypeEx.HideSelection = false;
			this.listViewTypeEx.Location = new System.Drawing.Point(342, 311);
			this.listViewTypeEx.Name = "listViewTypeEx";
			this.listViewTypeEx.Size = new System.Drawing.Size(205, 276);
			this.listViewTypeEx.TabIndex = 75;
			this.listViewTypeEx.UseCompatibleStateImageBehavior = false;
			this.listViewTypeEx.View = System.Windows.Forms.View.Details;
			this.listViewTypeEx.KeyDown += new System.Windows.Forms.KeyEventHandler(listViewTypeEx_KeyDown);
			this.loaivatphamhuy.Text = "Loại Vật Phẩm Sẽ Bán";
			this.loaivatphamhuy.Width = 200;
			this.listViewType.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1] { this.loaivatpham });
			this.listViewType.GridLines = true;
			this.listViewType.HideSelection = false;
			this.listViewType.Location = new System.Drawing.Point(44, 311);
			this.listViewType.Name = "listViewType";
			this.listViewType.Size = new System.Drawing.Size(205, 276);
			this.listViewType.TabIndex = 74;
			this.listViewType.UseCompatibleStateImageBehavior = false;
			this.listViewType.View = System.Windows.Forms.View.Details;
			this.listViewType.DoubleClick += new System.EventHandler(listViewType_DoubleClick);
			this.loaivatpham.Text = "Các Loại Vật Phẩm Tìm Thấy Trong Túi";
			this.loaivatpham.Width = 200;
			this.ListViewNameEx.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1] { this.tenvatphamboqua });
			this.ListViewNameEx.GridLines = true;
			this.ListViewNameEx.HideSelection = false;
			this.ListViewNameEx.Location = new System.Drawing.Point(342, 29);
			this.ListViewNameEx.Name = "ListViewNameEx";
			this.ListViewNameEx.Size = new System.Drawing.Size(205, 276);
			this.ListViewNameEx.TabIndex = 73;
			this.ListViewNameEx.UseCompatibleStateImageBehavior = false;
			this.ListViewNameEx.View = System.Windows.Forms.View.Details;
			this.ListViewNameEx.KeyDown += new System.Windows.Forms.KeyEventHandler(ListViewNameEx_KeyDown);
			this.tenvatphamboqua.Text = "Danh Sách Vật Phẩm Sẽ Bán";
			this.tenvatphamboqua.Width = 200;
			this.listViewName.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1] { this.buttenvatpham });
			this.listViewName.GridLines = true;
			this.listViewName.HideSelection = false;
			this.listViewName.Location = new System.Drawing.Point(44, 29);
			this.listViewName.Name = "listViewName";
			this.listViewName.Size = new System.Drawing.Size(205, 276);
			this.listViewName.TabIndex = 72;
			this.listViewName.UseCompatibleStateImageBehavior = false;
			this.listViewName.View = System.Windows.Forms.View.Details;
			this.listViewName.DoubleClick += new System.EventHandler(listViewName_DoubleClick);
			this.buttenvatpham.Text = "Toàn Bộ Vật Phẩm Trong Túi";
			this.buttenvatpham.Width = 200;
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25f, System.Drawing.FontStyle.Bold);
			this.label9.ForeColor = System.Drawing.Color.Red;
			this.label9.Location = new System.Drawing.Point(20, 642);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(501, 13);
			this.label9.TabIndex = 81;
			this.label9.Text = "- Auto sẽ không bán + hủy đồ trang bị có trang bị Điêu Văn hoặc Tinh Thông hoặc đã Khảm NGỌC\r\n";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(593, 664);
			base.Controls.Add(this.label9);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.label8);
			base.Controls.Add(this.buthemkieu);
			base.Controls.Add(this.buthem);
			base.Controls.Add(this.listViewTypeEx);
			base.Controls.Add(this.listViewType);
			base.Controls.Add(this.ListViewNameEx);
			base.Controls.Add(this.listViewName);
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.Name = "SellItem";
			this.Text = "Danh Sách Vật Phẩm Sẽ Bán";
			base.Load += new System.EventHandler(SellItem_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
