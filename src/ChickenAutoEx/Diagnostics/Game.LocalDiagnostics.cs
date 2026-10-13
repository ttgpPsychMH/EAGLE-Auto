using TinhKiemAuto.Diagnostics;

namespace TinhKiemAuto
{
    public partial class Game
    {
        // Read cached fields only: no TLBB memory getter, no command, no engine lock or state mutation.
        internal bool[] DiagnosticFlags()
        {
            return new[] { IsAuto, IsAttack, Skills != null && Skills.Count > 0, IsCheDo, IsPet,
                IsPickItem, IsHP || IsMP || IsNM, IsTrungAc, thuyLaoEnabled, kyCuocEnabled, mapAcTac != 0, isAcBa, IsHooked };
        }
        internal int[] DiagnosticPhases()
        { return new[] { (int)trungAcPhase, 0, (int)kyCuocPhase, (int)acTacPhase, (int)acBaPhase, (int)hookDiagnostics.Reader }; }
        internal int[] DiagnosticAcTac() { return acTacDiagnostic; }
        internal int[] DiagnosticTeamPatrol() { return dungeonPatrolDiagnostic; }
    }
}
