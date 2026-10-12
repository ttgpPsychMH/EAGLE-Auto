using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace EagleMultiClientMonitor
{
    // Source-owned, read-only Windows sampler. No game launcher, injection, patch or credential access.
    internal static class Program
    {
        private sealed class Target
        {
            public int Pid;
            public string Role;
            public long? StartTicks;
            public double? CpuMilliseconds;
            public double LastSampleSeconds;
        }
        private static volatile bool cancelled;
        private static int Main(string[] args)
        {
            if (args.Length == 0 || args.Contains("--help"))
            {
                Console.WriteLine("EAGLE-MultiClient-Monitor --games 123,456 [--auto 789] --out sample.csv [--seconds 180] [--interval 5]");
                Console.WriteLine("Windows/.NET Framework 4.8; explicit PIDs only; CSV must not already exist. Ctrl+C stops sampling.");
                return 0;
            }
            try
            {
                if (Environment.OSVersion.Platform != PlatformID.Win32NT) throw new PlatformNotSupportedException();
                var options = new Dictionary<string, string>();
                if (args.Length % 2 != 0) throw new ArgumentException();
                for (int i = 0; i < args.Length; i += 2)
                {
                    if (!new[] { "--games", "--auto", "--out", "--seconds", "--interval" }.Contains(args[i])
                        || options.ContainsKey(args[i])) throw new ArgumentException();
                    options.Add(args[i], args[i + 1]);
                }
                if (!options.ContainsKey("--games") || !options.ContainsKey("--out")) throw new ArgumentException();
                int seconds = options.ContainsKey("--seconds") ? ParseInt(options["--seconds"], 15, 3600) : 180;
                int interval = options.ContainsKey("--interval") ? ParseInt(options["--interval"], 1, 60) : 5;
                var targets = Pids(options["--games"]).Select(pid => new Target { Pid = pid, Role = "game" }).ToList();
                if (options.ContainsKey("--auto")) targets.AddRange(Pids(options["--auto"]).Select(pid => new Target { Pid = pid, Role = "auto" }));
                if (targets.Count > 64 || targets.Select(t => t.Pid).Distinct().Count() != targets.Count) throw new ArgumentException();
                int processors = Environment.ProcessorCount;
                Console.CancelKeyPress += (sender, e) => { e.Cancel = true; cancelled = true; };
                var watch = Stopwatch.StartNew();
                int successes = 0;
                using (var file = new FileStream(options["--out"], FileMode.CreateNew, FileAccess.Write, FileShare.Read))
                using (var writer = new StreamWriter(file, new UTF8Encoding(true)))
                {
                    writer.WriteLine("utc,role,pid,status,cpu_machine_percent,working_set_mib,private_bytes_mib,handles,gdi_objects,user_objects,window_status,wm_null_ms,system_available_mib,system_total_mib,logical_processors");
                    do
                    {
                        var memory = new MemoryStatus { Length = (uint)Marshal.SizeOf(typeof(MemoryStatus)) };
                        bool memoryKnown = GlobalMemoryStatusEx(ref memory);
                        foreach (var target in targets)
                        {
                            string[] row = new string[15];
                            row[0] = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
                            row[1] = target.Role; row[2] = target.Pid.ToString(CultureInfo.InvariantCulture);
                            row[14] = processors.ToString(CultureInfo.InvariantCulture);
                            if (memoryKnown) { row[12] = MiB(memory.AvailablePhysical); row[13] = MiB(memory.TotalPhysical); }
                            try
                            {
                                using (var process = Process.GetProcessById(target.Pid))
                                {
                                    process.Refresh();
                                    long start = process.StartTime.ToUniversalTime().Ticks;
                                    if (target.StartTicks.HasValue && start != target.StartTicks.Value)
                                    {
                                        row[3] = "pid_reused_not_sampled";
                                    }
                                    else
                                    {
                                        target.StartTicks = start;
                                        double cpu = process.TotalProcessorTime.TotalMilliseconds;
                                        double now = watch.Elapsed.TotalSeconds;
                                        if (target.CpuMilliseconds.HasValue && now > target.LastSampleSeconds && cpu >= target.CpuMilliseconds.Value)
                                            row[4] = Metrics.Number(Metrics.CpuPercent(cpu - target.CpuMilliseconds.Value,
                                                now - target.LastSampleSeconds, processors));
                                        target.CpuMilliseconds = cpu; target.LastSampleSeconds = now;
                                        row[5] = MiB((ulong)process.WorkingSet64); row[6] = MiB((ulong)process.PrivateMemorySize64);
                                        row[7] = process.HandleCount.ToString(CultureInfo.InvariantCulture);
                                        // Request query access only; Process.Handle can request broader access on net48.
                                        IntPtr query = OpenProcess(0x0400, false, target.Pid);
                                        if (query != IntPtr.Zero)
                                        {
                                            try {
                                                row[8] = GetGuiResources(query, 0).ToString(CultureInfo.InvariantCulture);
                                                row[9] = GetGuiResources(query, 1).ToString(CultureInfo.InvariantCulture);
                                            } finally { CloseHandle(query); }
                                        }
                                        IntPtr window = process.MainWindowHandle;
                                        if (window == IntPtr.Zero) row[10] = "no_main_window";
                                        else
                                        {
                                            var latency = Stopwatch.StartNew(); UIntPtr result;
                                            // WM_NULL does not invoke gameplay. Bounded probe of the UI message loop.
                                            IntPtr delivered = SendMessageTimeout(window, 0, UIntPtr.Zero, IntPtr.Zero, 2, 200, out result);
                                            latency.Stop(); row[10] = delivered == IntPtr.Zero ? "timeout_or_denied" : "responsive";
                                            row[11] = Metrics.Number(latency.Elapsed.TotalMilliseconds);
                                        }
                                        row[3] = "sampled"; successes++;
                                    }
                                }
                            }
                            catch (ArgumentException) { row[3] = "missing_or_exited"; }
                            catch (InvalidOperationException) { row[3] = "exited_or_unavailable"; }
                            catch (System.ComponentModel.Win32Exception) { row[3] = "access_denied_or_query_failed"; }
                            writer.WriteLine(string.Join(",", row.Select(Metrics.Csv)));
                        }
                        writer.Flush();
                        double next = Math.Min(seconds, watch.Elapsed.TotalSeconds + interval);
                        while (!cancelled && watch.Elapsed.TotalSeconds < next) Thread.Sleep(100);
                    } while (!cancelled && watch.Elapsed.TotalSeconds < seconds);
                }
                Console.WriteLine(successes == 0 ? "No process samples succeeded; check PIDs/permissions." : "Sampling complete. No game process was started or modified.");
                return successes == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                // Do not print process paths, command lines, credentials or full exception data.
                Console.Error.WriteLine("Sampling failed: " + ex.GetType().Name + ". Use --help; choose a new output filename.");
                return 2;
            }
        }
        private static int ParseInt(string text, int minimum, int maximum)
        {
            int value;
            if (!int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out value) || value < minimum || value > maximum)
                throw new ArgumentException();
            return value;
        }
        private static IEnumerable<int> Pids(string text)
        {
            return text.Split(',').Select(s => ParseInt(s, 1, int.MaxValue)).ToArray();
        }
        private static string MiB(ulong bytes) { return Metrics.Number(bytes / 1048576.0); }
        [StructLayout(LayoutKind.Sequential)]
        private struct MemoryStatus
        {
            public uint Length, Load;
            public ulong TotalPhysical, AvailablePhysical, TotalPageFile, AvailablePageFile, TotalVirtual, AvailableVirtual, AvailableExtendedVirtual;
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx(ref MemoryStatus memory);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint access, [MarshalAs(UnmanagedType.Bool)] bool inherit, int pid);
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr handle);
        [DllImport("user32.dll")]
        private static extern uint GetGuiResources(IntPtr process, uint flags);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr SendMessageTimeout(IntPtr hwnd, uint message, UIntPtr wParam, IntPtr lParam,
            uint flags, uint timeout, out UIntPtr result);
    }
}
