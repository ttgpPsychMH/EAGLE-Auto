using System;
using System.Diagnostics;
using System.Reflection;

namespace TinhKiemAuto.Diagnostics
{
    public static class LocalDiagnostics
    {
        private static volatile DiagnosticRecorder recorder;
        private static Assembly ownedAssembly;
        private static readonly Stopwatch clock = Stopwatch.StartNew();
        [ThreadStatic] private static bool observing;
        public static bool Enabled { get { var r = recorder; return r != null && r.Enabled; } }
        public static bool Failed { get { var r = recorder; return r != null && r.Failed; } }
        public static void Attach(DiagnosticRecorder value, Assembly assembly)
        { ownedAssembly = assembly; recorder = value; }
        public static void Detach()
        { var old = recorder; recorder = null; if (old != null) old.Dispose(); }
        public static void Warning(string title, string body, int severity)
        {
            var r = recorder;
            if (r == null || !r.Enabled || severity < 2 || severity > 3 || !r.TakeExceptionBudget(clock.ElapsedMilliseconds)) return;
            try {
                var feature = title != null && title.Length <= 160 ? DiagnosticSchema.Feature(title) : DiagnosticFeature.General;
                var reason = body != null && body.Length <= 2048 ? DiagnosticSchema.Reason(body) : DiagnosticReason.OtherWarning;
                r.Record(DiagnosticSchema.Row(DateTime.UtcNow, r.Version, DiagnosticEvent.Warning, feature, reason: reason));
            } catch { }
        }
        public static void ObserveException(Exception error, bool unhandled = false)
        {
            var r = recorder;
            if (observing || error == null || r == null || !r.Enabled) return;
            observing = true;
            try
            {
                DiagnosticFeature feature; bool belongs;
                Classify(error, ownedAssembly, out feature, out belongs);
                if (!unhandled && !belongs) return;
                // Excluded/framework errors must not consume the budget for actual application errors.
                if (!unhandled && !r.TakeExceptionBudget(clock.ElapsedMilliseconds)) return;
                string row = DiagnosticSchema.Row(DateTime.UtcNow, r.Version,
                    unhandled ? DiagnosticEvent.UnhandledException : DiagnosticEvent.ExceptionObserved,
                    feature, DiagnosticSchema.ErrorKind(error), error.HResult);
                if (unhandled) r.FlushUnhandled(row); else r.Record(row);
            }
            catch { }
            finally { observing = false; }
        }
        public static void Classify(Exception error, Assembly assembly, out DiagnosticFeature feature, out bool belongs)
        {
            feature = DiagnosticFeature.General; belongs = false;
            if (error == null || assembly == null) return;
            var trace = new StackTrace(error, false);
            for (int i = 0; i < Math.Min(trace.FrameCount, 24); i++)
            {
                MethodBase method = trace.GetFrame(i).GetMethod(); Type type = method == null ? null : method.DeclaringType;
                if (type == null || type.Assembly != assembly) continue;
                string ns = type.Namespace ?? "";
                if (ns.StartsWith("TinhKiemAuto.Diagnostics", StringComparison.Ordinal)) return;
                if (ns != "TinhKiemAuto" && !ns.StartsWith("TinhKiemAuto.", StringComparison.Ordinal)) continue;
                // Do not collect authentication/licensing/config crypto contexts.
                for (Type context = type; context != null; context = context.DeclaringType)
                    if (context.Name == "Debug" || context.Name == "LoadFile" || context.Name == "Hasher") { belongs = false; return; }
                belongs = true;
                var found = DiagnosticSchema.Feature(method.Name + " " + type.Name);
                if ((feature == DiagnosticFeature.General || feature == DiagnosticFeature.Interface || feature == DiagnosticFeature.GameLoop)
                    && found != DiagnosticFeature.General) feature = found;
                if (feature == DiagnosticFeature.General && type.Name == "FrmMain") feature = DiagnosticFeature.Interface;
                if (feature == DiagnosticFeature.General && type.Name == "Game") feature = DiagnosticFeature.GameLoop;
            }
        }
    }
}
