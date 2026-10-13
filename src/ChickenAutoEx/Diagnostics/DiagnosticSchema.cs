using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace TinhKiemAuto.Diagnostics
{
    public enum DiagnosticFeature { General, Startup, Interface, GameLoop, Combat, Skills, Crafting, Pet, Loot, Recovery,
        Travel, AutoLogin, TrungAc, ThuyLao, KyCuoc, AcTac, AcBa, Hook }
    public enum DiagnosticEvent { SessionStarted, FeatureState, Warning, ExceptionObserved, UnhandledException,
        ProcessSample, InterfaceSample, HookCounter, SamplesDropped, QueueSamplesDropped, ErrorSamplesDropped,
        TaskState, TaskReady, TaskInside, TaskCombatHold, TaskProgress }
    public enum DiagnosticErrorKind { None, NullReference, InvalidOperation, Argument, Index, IO, Access,
        Timeout, Win32, Format, Other }
    public enum DiagnosticReason { None, OtherWarning, WaitingTimeout, MissingTarget, InvalidState, TeamChanged,
        AutoDisabled, SessionCleared, NotInitialized, NotOnline, MissingIdentity, LeaderUnconfirmed,
        CharacterUnavailable, SceneTransition, OtherTask, ReadFailure, SnapshotUnavailable,
        EntrancePending, CombatHold, LootHold, NoProgress, BossPending, ExitPending, CompletedMapReturn,
        DialogPending, OptionMissing }

    public static class DiagnosticSchema
    {
        public const string Header = "utc,version,event,feature,error_kind,hresult,window,cpu_percent,working_set_mib,private_bytes_mib,handles,ui_delay_ms,game_count,state,phase,count,reason";
        public static string Version(string value)
        {
            if (value == null || !Regex.IsMatch(value, @"\A[0-9]{1,3}(\.[0-9]{1,3}){1,3}\z")) throw new ArgumentException("Invalid product version.");
            return value;
        }
        public static DiagnosticErrorKind ErrorKind(Exception error)
        {
            if (error is NullReferenceException) return DiagnosticErrorKind.NullReference;
            if (error is IndexOutOfRangeException) return DiagnosticErrorKind.Index;
            if (error is ArgumentException) return DiagnosticErrorKind.Argument;
            if (error is InvalidOperationException) return DiagnosticErrorKind.InvalidOperation;
            if (error is UnauthorizedAccessException) return DiagnosticErrorKind.Access;
            if (error is TimeoutException) return DiagnosticErrorKind.Timeout;
            if (error is System.ComponentModel.Win32Exception) return DiagnosticErrorKind.Win32;
            if (error is IOException) return DiagnosticErrorKind.IO;
            if (error is FormatException) return DiagnosticErrorKind.Format;
            return DiagnosticErrorKind.Other;
        }
        public static DiagnosticFeature Feature(string ownedMethodOrTitle)
        {
            string text = ownedMethodOrTitle ?? "";
            if (Has(text, "TrungAc", "Trừng Ác")) return DiagnosticFeature.TrungAc;
            if (Has(text, "ThuyLao", "Thủy Lao")) return DiagnosticFeature.ThuyLao;
            if (Has(text, "KyCuoc", "Kỳ Cuộc")) return DiagnosticFeature.KyCuoc;
            if (Has(text, "AcTac", "Ác Tặc")) return DiagnosticFeature.AcTac;
            if (Has(text, "AcBa", "Ác Bá")) return DiagnosticFeature.AcBa;
            if (Has(text, "CheDo", "Craft", "Chế Đồ", "chedo", "cheLoai")) return DiagnosticFeature.Crafting;
            if (Has(text, "Skill", "skill", "Kỹ Năng")) return DiagnosticFeature.Skills;
            if (Has(text, "Attack", "Đánh Quái")) return DiagnosticFeature.Combat;
            if (Has(text, "Pet", "pet")) return DiagnosticFeature.Pet;
            if (Has(text, "Pick", "Loot", "Nhặt", "SellItem", "DropItem")) return DiagnosticFeature.Loot;
            if (Has(text, "Buff", "Regen", "HoiSinh", "Trị Liệu", "Trilieu")) return DiagnosticFeature.Recovery;
            if (Has(text, "Login", "Đăng Nhập", "Mở Game")) return DiagnosticFeature.AutoLogin;
            if (Has(text, "Hook", "Recv", "SetDll")) return DiagnosticFeature.Hook;
            if (Has(text, "Update", "Startup", "LoadConfig")) return DiagnosticFeature.Startup;
            if (Has(text, "Move", "GoTo", "Travel", "Di Chuyển")) return DiagnosticFeature.Travel;
            return DiagnosticFeature.General;
        }
        public static DiagnosticReason Reason(string warning)
        {
            // Only map existing in-memory text to a finite code. Never return or retain it.
            if (Has(warning, "thời gian chờ", "timeout", "Timeout")) return DiagnosticReason.WaitingTimeout;
            if (Has(warning, "không có mục tiêu", "thiếu mục tiêu", "thiếu NPC", "NPC vào cửa")) return DiagnosticReason.MissingTarget;
            if (Has(warning, "đội đã thay đổi", "đội thay đổi", "nhân vật đã thay đổi", "rời đội")) return DiagnosticReason.TeamChanged;
            if (Has(warning, "không hợp lệ", "không có tuyến", "thiếu tuyến")) return DiagnosticReason.InvalidState;
            return DiagnosticReason.OtherWarning;
        }
        private static bool Has(string text, params string[] keys)
        {
            if (text == null) return false;
            foreach (string key in keys) if (text.IndexOf(key, StringComparison.Ordinal) >= 0) return true;
            return false;
        }
        public static string Row(DateTime utc, string version, DiagnosticEvent code, DiagnosticFeature feature,
            DiagnosticErrorKind error = DiagnosticErrorKind.None, int hresult = 0, int window = 0,
            double? cpu = null, long? working = null, long? privateBytes = null, int? handles = null,
            long? uiDelay = null, int? gameCount = null, bool? state = null, int? phase = null,
            long? count = null, DiagnosticReason reason = DiagnosticReason.None)
        {
            if (!Enum.IsDefined(typeof(DiagnosticEvent), code) || !Enum.IsDefined(typeof(DiagnosticFeature), feature)
                || !Enum.IsDefined(typeof(DiagnosticErrorKind), error) || !Enum.IsDefined(typeof(DiagnosticReason), reason)
                || window < 0 || window > 9999 || (cpu.HasValue && (double.IsNaN(cpu.Value) || double.IsInfinity(cpu.Value) || cpu < 0))
                || working < 0 || privateBytes < 0 || handles < 0 || uiDelay < 0 || gameCount < 0 || phase < 0 || phase > 255 || count < 0)
                throw new ArgumentException("Invalid diagnostic values.");
            string[] cells = { utc.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture), Version(version), code.ToString(), feature.ToString(),
                error.ToString(), "0x" + hresult.ToString("X8", CultureInfo.InvariantCulture), window == 0 ? "" : "window_" + window.ToString("D4", CultureInfo.InvariantCulture),
                Number(cpu), MiB(working), MiB(privateBytes), Number(handles), Number(uiDelay), Number(gameCount), state.HasValue ? (state.Value ? "On" : "Off") : "",
                Number(phase), Number(count), reason.ToString() };
            // Every text field is a fixed enum, timestamp, validated version or generated alias.
            return string.Join(",", cells);
        }
        private static string Number<T>(T? value) where T : struct, IFormattable
        { return value.HasValue ? value.Value.ToString(null, CultureInfo.InvariantCulture) : ""; }
        private static string MiB(long? value) { return value.HasValue ? (value.Value / 1048576.0).ToString("F2", CultureInfo.InvariantCulture) : ""; }
    }
}
