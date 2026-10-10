using System;

namespace TinhKiemAuto
{
    public partial class Game
    {
        public void AOE()
        {
            if (TickCount % 150 != 0 || !Global.UseSkillPet || !IsAuto || TLBB == null || !TLBB.Online
                || TLBB.PlayerState == 2 || TLBB.PlayerState == 7 || ON_SCENE_TRANSING || IsChangeMap) return;
            if (string.IsNullOrEmpty(TLBB.SkillPetType)) return;
            int skill = SkillPetId(TLBB.SkillPetType);
            if (skill == -1) return;
            GameObject target = null;
            float distance = float.MaxValue;
            foreach (GameObject candidate in Objects.NearMonter20m)
            {
                if (candidate == null || !(candidate.HP > 0f) || float.IsInfinity(candidate.HP) || float.IsNaN(candidate.X) || float.IsNaN(candidate.Y)
                    || float.IsInfinity(candidate.X) || float.IsInfinity(candidate.Y)) continue;
                float value = TINHKIEM.GetDistance(CharX, CharY, candidate.X, candidate.Y);
                if (value < 20f && value < distance) { target = candidate; distance = value; }
            }
            if (target != null) UseSkillPet(skill, target.X, target.Y);
        }
    }
}
