using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TinhKiemAuto
{
	public class BoQua : Form
	{
		private HashSet<string> DropName = new HashSet<string>();

		private IContainer components;

		private Label label1;

		private Label label2;

		private ListView listViewName;

		private ListView listviewboqua;

		private Button butthemdanhsach;

		private Button butlammoi;

		private Button button1;

		private ColumnHeader buttenquai;

		private ColumnHeader tenquai;

		private ToolTip toolTip1;

		public BoQua()
		{
			InitializeComponent();
		}

		private void BoQua_Load(object sender, EventArgs e)
		{
			string[] array = Setting.BoQua.Split('\n');
			foreach (string text in array)
			{
				if (text.Length > 0)
				{
					listviewboqua.Items.Add(text);
				}
			}
			foreach (KeyValuePair<int, Game> item in FrmMain.dicGame)
			{
				foreach (GameObject item2 in item.Value.Objects.NearMonter20m)
				{
					if (!(item2.Name.Trim() == "") && !DropName.Contains(item2.Name))
					{
						DropName.Add(item2.Name);
						listViewName.Items.Add(item2.Name);
					}
				}
			}
		}

		private void butlammoi_Click(object sender, EventArgs e)
		{
			listViewName.Items.Clear();
			DropName.Clear();
			foreach (KeyValuePair<int, Game> item in FrmMain.dicGame)
			{
				foreach (GameObject item2 in item.Value.Objects.NearMonter20m)
				{
					if (!(item2.Name.Trim() == "") && !DropName.Contains(item2.Name))
					{
						DropName.Add(item2.Name);
						listViewName.Items.Add(item2.Name);
					}
				}
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			string text = "";
			foreach (ListViewItem item in listviewboqua.Items)
			{
				if (item.Text.Trim() != "")
				{
					text = text + item.Text.Trim() + "\n";
				}
			}
			Setting.BoQua = text;
			CanhBao.Msg("Thiết Lập Thành Công", "Lưu thiết lập bỏ qua quái thành công!", CanhBao.Kieu.OK);
			Close();
		}

		private void menuDelete_Click(object sender, EventArgs e)
		{
			if (listviewboqua.SelectedItems.Count == 0)
			{
				return;
			}
			foreach (ListViewItem selectedItem in listviewboqua.SelectedItems)
			{
				if (selectedItem.Text.Trim() != "")
				{
					selectedItem.Remove();
				}
			}
		}

		private void listviewboqua_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				menuDelete_Click(null, null);
			}
		}

		private void AddName()
		{
			foreach (ListViewItem selectedItem in listViewName.SelectedItems)
			{
				if (selectedItem.Text.Trim() != "" && listviewboqua.FindItemWithText(selectedItem.Text.Trim()) == null)
				{
					listviewboqua.Items.Add(selectedItem.Text);
				}
			}
		}

		private void butthemdanhsach_Click(object sender, EventArgs e)
		{
			AddName();
		}

		private void listViewName_DoubleClick(object sender, EventArgs e)
		{
			AddName();
		}

		private void listViewName_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		private void listviewboqua_SelectedIndexChanged(object sender, EventArgs e)
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TinhKiemAuto.BoQua));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.listViewName = new System.Windows.Forms.ListView();
			this.buttenquai = new System.Windows.Forms.ColumnHeader();
			this.listviewboqua = new System.Windows.Forms.ListView();
			this.tenquai = new System.Windows.Forms.ColumnHeader();
			this.butthemdanhsach = new System.Windows.Forms.Button();
			this.butlammoi = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			base.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(23, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(149, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Danh Sách Quái Xung Quanh";
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(270, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(183, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Danh Sách Quái Đang Được Bỏ Qua";
			this.listViewName.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1] { this.buttenquai });
			this.listViewName.GridLines = true;
			this.listViewName.HideSelection = false;
			this.listViewName.Location = new System.Drawing.Point(12, 32);
			this.listViewName.Name = "listViewName";
			this.listViewName.Size = new System.Drawing.Size(172, 276);
			this.listViewName.TabIndex = 2;
			this.toolTip1.SetToolTip(this.listViewName, "-Nhấn DoubleClick vào giao diện Tên Quái Xung Quanh để\r\nthêm dánh sách bỏ qua.\r\n-Chọn quái bên Tên Quái Bỏ Qua và nhấn Delete để xóa quái \r\nkhỏi danh sách bỏ qua\r\n");
			this.listViewName.UseCompatibleStateImageBehavior = false;
			this.listViewName.View = System.Windows.Forms.View.Details;
			this.listViewName.SelectedIndexChanged += new System.EventHandler(listViewName_SelectedIndexChanged);
			this.listViewName.DoubleClick += new System.EventHandler(listViewName_DoubleClick);
			this.buttenquai.Text = "Tên Quái Xung Quanh";
			this.buttenquai.Width = 160;
			this.listviewboqua.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1] { this.tenquai });
			this.listviewboqua.GridLines = true;
			this.listviewboqua.HideSelection = false;
			this.listviewboqua.Location = new System.Drawing.Point(273, 32);
			this.listviewboqua.Name = "listviewboqua";
			this.listviewboqua.Size = new System.Drawing.Size(172, 276);
			this.listviewboqua.TabIndex = 3;
			this.toolTip1.SetToolTip(this.listviewboqua, "-Nhấn DoubleClick vào giao diện Tên Quái Xung Quanh để\r\nthêm dánh sách bỏ qua.\r\n-Chọn quái bên Tên Quái Bỏ Qua và nhấn Delete để xóa quái \r\nkhỏi danh sách bỏ qua");
			this.listviewboqua.UseCompatibleStateImageBehavior = false;
			this.listviewboqua.View = System.Windows.Forms.View.Details;
			this.listviewboqua.SelectedIndexChanged += new System.EventHandler(listviewboqua_SelectedIndexChanged);
			this.listviewboqua.KeyDown += new System.Windows.Forms.KeyEventHandler(listviewboqua_KeyDown);
			this.tenquai.Text = "Tên Quái Bỏ Qua";
			this.tenquai.Width = 165;
			this.butthemdanhsach.Location = new System.Drawing.Point(190, 114);
			this.butthemdanhsach.Name = "butthemdanhsach";
			this.butthemdanhsach.Size = new System.Drawing.Size(75, 23);
			this.butthemdanhsach.TabIndex = 4;
			this.butthemdanhsach.Text = "---->";
			this.butthemdanhsach.UseVisualStyleBackColor = true;
			this.butthemdanhsach.Click += new System.EventHandler(butthemdanhsach_Click);
			this.butlammoi.Location = new System.Drawing.Point(49, 311);
			this.butlammoi.Name = "butlammoi";
			this.butlammoi.Size = new System.Drawing.Size(75, 23);
			this.butlammoi.TabIndex = 5;
			this.butlammoi.Text = "Làm Mới";
			this.toolTip1.SetToolTip(this.butlammoi, "Nhấn vào đây để làm mới danh sách quái xung quanh");
			this.butlammoi.UseVisualStyleBackColor = true;
			this.butlammoi.Click += new System.EventHandler(butlammoi_Click);
			this.button1.Location = new System.Drawing.Point(319, 311);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 6;
			this.button1.Text = "Lưu Lại";
			this.toolTip1.SetToolTip(this.button1, "Nhấn vào đây để lưu lại danh sách quái được bỏ qua");
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(button1_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(465, 339);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.butlammoi);
			base.Controls.Add(this.butthemdanhsach);
			base.Controls.Add(this.listviewboqua);
			base.Controls.Add(this.listViewName);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(481, 378);
			base.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(481, 378);
			base.Name = "BoQua";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Bỏ Qua Quái";
			base.Load += new System.EventHandler(BoQua_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
