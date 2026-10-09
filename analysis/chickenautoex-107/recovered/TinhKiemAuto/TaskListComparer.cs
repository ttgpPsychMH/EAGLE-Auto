using System.Collections;
using System.Windows.Forms;

namespace TinhKiemAuto
{
	internal class TaskListComparer : IComparer
	{
		public int Compare(object a, object b)
		{
			try
			{
				ListViewItem listViewItem = a as ListViewItem;
				ListViewItem listViewItem2 = b as ListViewItem;
				int packetId = ((Skill)listViewItem.Tag).PacketId;
				int packetId2 = ((Skill)listViewItem2.Tag).PacketId;
				if (listViewItem.Checked && !listViewItem2.Checked)
				{
					return -1;
				}
				if (!listViewItem.Checked && listViewItem2.Checked)
				{
					return 1;
				}
				if (packetId > packetId2)
				{
					return 1;
				}
				if (packetId < packetId2)
				{
					return -1;
				}
				return 0;
			}
			catch
			{
				return 0;
			}
		}
	}
}
