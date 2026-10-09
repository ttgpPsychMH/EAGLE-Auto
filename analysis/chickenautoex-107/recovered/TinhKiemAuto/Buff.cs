using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TinhKiemAuto.Models;

namespace TinhKiemAuto
{
	public class Buff : Form
	{
		private Game game;

		public List<BuffPramenter> DanhSachBuff = new List<BuffPramenter>();

		private IContainer components;

		private ListView ListViewNhanBuff;

		private ColumnHeader columnHeader1;

		private ListView listViewName;

		private ColumnHeader buttenquai;

		private Button butthemdanhsach;

		private Button button1;

		private Button button2;

		private Label label4;

		private ToolTip toolTip1;

		public Buff(Game game)
		{
			this.game = game;
			InitializeComponent();
		}

		public void LoadDanhSach()
		{
			DanhSachBuff.Clear();
			listViewName.Items.Clear();
			GameObject[] array = game.Objects.AllPlayer.ToArray();
			foreach (GameObject gameObject in array)
			{
				if (!game.Objects.Pk.Contains(gameObject) && gameObject.IsPlayer && gameObject.Name != "#G")
				{
					BuffPramenter buffPramenter = new BuffPramenter();
					buffPramenter.TenNguoiChoi = gameObject.Name;
					buffPramenter.IDnguoichoi = gameObject.TrueId.Replace("FFFFFFFF", "");
					DanhSachBuff.Add(buffPramenter);
					listViewName.Items.Add(gameObject.Name);
				}
			}
		}

		public void LoadDuocBufff()
		{
			try
			{
				foreach (BuffPramenter item in Setting.BuffValue)
				{
					ListViewNhanBuff.Items.Add(item.TenNguoiChoi);
				}
			}
			catch
			{
			}
		}

		private void Buff_Load(object sender, EventArgs e)
		{
			LoadDanhSach();
			LoadDuocBufff();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			LoadDanhSach();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Setting.SaveBuff();
			CanhBao.Msg("Thiết Lập Thành Công", "Lưu thiết lập buff NM thành công!", CanhBao.Kieu.OK);
			Close();
		}

		public void AddToSetting(string CharName)
		{
			BuffPramenter buffPramenter = DanhSachBuff.Find((BuffPramenter x) => x.TenNguoiChoi == CharName);
			if (buffPramenter != null)
			{
				Setting.BuffValue.Add(buffPramenter);
			}
		}

		public void AddName()
		{
			foreach (ListViewItem selectedItem in listViewName.SelectedItems)
			{
				if (selectedItem.Text.Trim() != "" && ListViewNhanBuff.FindItemWithText(selectedItem.Text.Trim()) == null)
				{
					ListViewNhanBuff.Items.Add(selectedItem.Text);
					AddToSetting(selectedItem.Text);
				}
			}
		}

		private void butthemdanhsach_Click(object sender, EventArgs e)
		{
			AddName();
		}

		public void Remove(string CharName)
		{
			int num = 0;
			BuffPramenter[] array = Setting.BuffValue.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].TenNguoiChoi == CharName)
				{
					Setting.BuffValue.RemoveAt(num);
				}
				num++;
			}
		}

		private void ListViewNhanBuff_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Delete)
			{
				return;
			}
			foreach (ListViewItem selectedItem in ListViewNhanBuff.SelectedItems)
			{
				if (selectedItem.Text.Trim() != "")
				{
					selectedItem.Remove();
					Remove(selectedItem.Text.Trim());
				}
			}
		}

		private void listViewName_DoubleClick(object sender, EventArgs e)
		{
			AddName();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TinhKiemAuto.Buff));
			this.ListViewNhanBuff = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.listViewName = new System.Windows.Forms.ListView();
			this.buttenquai = new System.Windows.Forms.ColumnHeader();
			this.butthemdanhsach = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			base.SuspendLayout();
			this.ListViewNhanBuff.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1] { this.columnHeader1 });
			this.ListViewNhanBuff.GridLines = true;
			this.ListViewNhanBuff.HideSelection = false;
			this.ListViewNhanBuff.Location = new System.Drawing.Point(349, 41);
			this.ListViewNhanBuff.Name = "ListViewNhanBuff";
			this.ListViewNhanBuff.Size = new System.Drawing.Size(205, 276);
			this.ListViewNhanBuff.TabIndex = 6;
			this.toolTip1.SetToolTip(this.ListViewNhanBuff, resources.GetString("ListViewNhanBuff.ToolTip"));
			this.ListViewNhanBuff.UseCompatibleStateImageBehavior = false;
			this.ListViewNhanBuff.View = System.Windows.Forms.View.Details;
			this.ListViewNhanBuff.KeyDown += new System.Windows.Forms.KeyEventHandler(ListViewNhanBuff_KeyDown);
			this.columnHeader1.Text = "Danh Sách Được Đồng Ý Tổ Đội";
			this.columnHeader1.Width = 200;
			this.listViewName.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1] { this.buttenquai });
			this.listViewName.GridLines = true;
			this.listViewName.HideSelection = false;
			this.listViewName.Location = new System.Drawing.Point(6, 41);
			this.listViewName.Name = "listViewName";
			this.listViewName.Size = new System.Drawing.Size(203, 276);
			this.listViewName.TabIndex = 5;
			this.toolTip1.SetToolTip(this.listViewName, resources.GetString("listViewName.ToolTip"));
			this.listViewName.UseCompatibleStateImageBehavior = false;
			this.listViewName.View = System.Windows.Forms.View.Details;
			this.listViewName.DoubleClick += new System.EventHandler(listViewName_DoubleClick);
			this.buttenquai.Text = "Danh Sách Người Chơi Xung Quanh";
			this.buttenquai.Width = 190;
			this.butthemdanhsach.Location = new System.Drawing.Point(245, 106);
			this.butthemdanhsach.Name = "butthemdanhsach";
			this.butthemdanhsach.Size = new System.Drawing.Size(75, 23);
			this.butthemdanhsach.TabIndex = 7;
			this.butthemdanhsach.Text = "---->";
			this.toolTip1.SetToolTip(this.butthemdanhsach, resources.GetString("butthemdanhsach.ToolTip"));
			this.butthemdanhsach.UseVisualStyleBackColor = true;
			this.butthemdanhsach.Click += new System.EventHandler(butthemdanhsach_Click);
			this.button1.Location = new System.Drawing.Point(55, 334);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 8;
			this.button1.Text = "Làm Mới";
			this.toolTip1.SetToolTip(this.button1, resources.GetString("button1.ToolTip"));
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(button1_Click);
			this.button2.Location = new System.Drawing.Point(408, 334);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 9;
			this.button2.Text = "Lưu Lại";
			this.toolTip1.SetToolTip(this.button2, resources.GetString("button2.ToolTip"));
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(button2_Click);
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.label4.ForeColor = System.Drawing.Color.Red;
			this.label4.Location = new System.Drawing.Point(12, 9);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(396, 15);
			this.label4.TabIndex = 31;
			this.label4.Text = "- Người chơi sẽ ở trong danh sách sẽ tự động được chấp nhận vào tổ đội";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(566, 379);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.button2);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.butthemdanhsach);
			base.Controls.Add(this.ListViewNhanBuff);
			base.Controls.Add(this.listViewName);
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(582, 418);
			base.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(582, 418);
			base.Name = "Buff";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Thiết Lập Tự Đồng Ý Tổ Đội";
			base.Load += new System.EventHandler(Buff_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
