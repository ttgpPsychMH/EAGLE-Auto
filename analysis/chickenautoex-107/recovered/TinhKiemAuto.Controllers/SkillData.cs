using System.Collections.Generic;
using Newtonsoft.Json;
using TinhKiemAuto.Models;

namespace TinhKiemAuto.Controllers
{
	public static class SkillData
	{
		public static Dictionary<int, SkillModel> skillList = new Dictionary<int, SkillModel>();

		public static SkillModel GetSkillByID(int id)
		{
			SkillModel value = new SkillModel();
			if (skillList.TryGetValue(id, out value))
			{
				return value;
			}
			return null;
		}

		public static void LoadSkillData()
		{
			skillList = JsonConvert.DeserializeObject<Dictionary<int, SkillModel>>(LoadFile.LoadFileWithDecrypt(Global.DataPath + "\\SkillList.dat"));
		}
	}
}
