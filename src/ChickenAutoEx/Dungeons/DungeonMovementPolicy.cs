namespace TinhKiemAuto
{
    // Legacy Game.Status labels 2 as moving and 7 as attacking. Riding/following are separate observations.
    internal static class DungeonMovementPolicy
    {
        internal static bool CanObserve(int state) { return state == 0 || state == 2 || state == 7; }
        internal static bool IsMoving(int state) { return state == 2; }
        internal static bool IsAttacking(int state) { return state == 7; }
        internal static bool CanTalk(int state) { return state == 0; }
    }
}
