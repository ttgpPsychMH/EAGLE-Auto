namespace TinhKiemAuto.Controllers
{
	public class SkillModel
	{
		public int id { get; set; }

		public int classID { get; set; }

		public string name { get; set; }

		public string icon { get; set; }

		public bool isNeedWeapon { get; set; }

		public bool isTargetMustBeAlive { get; set; }

		public bool isPassive { get; set; }

		public bool isPetActiveSkill { get; set; }

		public string skillType { get; set; }

		public int useRange { get; set; }

		public string skillTargetType { get; set; }

		public bool isAutoCast { get; set; }

		public Impact impact { get; set; }
	}
}
