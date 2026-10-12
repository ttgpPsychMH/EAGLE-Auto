using System.Collections.Concurrent;
using System.Text.Json;
using EagleMultiClientMonitor;
using static MultiClientSimulation.LaunchModel;

namespace MultiClientSimulation;

internal static class Program
{
    private sealed record Check(string Name, string Kind, bool Passed, string? Failure);
    private static readonly List<Check> checks = new();
    private static void Assert(bool condition, string message = "Assertion failed")
    { if (!condition) throw new InvalidOperationException(message); }
    private static void Test(string name, string kind, Action action)
    {
        try { action(); checks.Add(new Check(name, kind, true, null)); }
        catch (Exception ex) { checks.Add(new Check(name, kind, false, ex.GetType().Name + ": " + ex.Message)); }
    }
    private static Identity Fake(int id) => new(id, id * 100L);
    public static int Main(string[] args)
    {
        if (args.Length != 2) { Console.Error.WriteLine("Usage: MultiClientSimulation <repository> <summary.json>"); return 2; }
        string repo = Path.GetFullPath(args[0]);
        try
        {
            dynamic probe = LegacyProbe.Build(repo);
            dynamic state = LegacyProbe.State(probe);
            Test("legacy_monitor_patches_any_game_dialog_without_text_check", "unsafe_legacy_reproduced", () => {
                state.Reset(); probe.MonitorBranch(); Assert((int)state.Attempts == 2 && (int)state.CloseRequests == 1);
            });
            Test("legacy_monitor_closes_after_failed_writes", "unsafe_legacy_reproduced", () => {
                state.Reset(); state.WriteSucceedsValue = false; probe.MonitorBranch();
                Assert((int)state.Attempts == 2 && (int)state.CloseRequests == 1);
            });
            Test("legacy_init_closes_after_failed_writes", "unsafe_legacy_reproduced", () => {
                state.Reset(); state.WriteSucceedsValue = false; probe.InitBranch();
                Assert((int)state.Attempts == 2 && (int)state.CloseRequests == 1);
            });
            Test("legacy_retries_patch_on_repeated_dialog_observation", "unsafe_legacy_reproduced", () => {
                state.Reset(); probe.MonitorBranch(); probe.MonitorBranch(); Assert((int)state.Attempts == 4);
            });
            Test("legacy_post_failure_not_reported", "unsafe_legacy_reproduced", () => {
                state.Reset(); state.PostSucceedsValue = false; probe.MonitorBranch(); Assert((int)state.CloseRequests == 1);
            });
            foreach (int failedWrite in new[] { 1, 2 })
            {
                int failure = failedWrite;
                Test($"legacy_partial_write_{failure}_failure_still_closes_dialog", "unsafe_legacy_reproduced", () => {
                    state.Reset(); state.FailAtValue = failure; probe.MonitorBranch();
                    Assert((int)state.Attempts == 2 && (int)state.CloseRequests == 1);
                });
            }
            Test("legacy_no_dialog_no_write", "legacy_control", () => {
                state.Reset(); state.Dialog = false; probe.MonitorBranch(); probe.InitBranch();
                Assert((int)state.Attempts == 0 && (int)state.CloseRequests == 0);
            });
            Test("legacy_guard_allows_launch_when_three_already_exist", "unsafe_legacy_reproduced", () => {
                Assert((bool)probe.Allows(2) && (bool)probe.Allows(3) && !(bool)probe.Allows(4));
            });
            Test("legacy_same_stale_count_accepts_multiple_requests", "unsafe_legacy_reproduced", () => {
                int accepted = Enumerable.Range(0, 6).Count(_ => (bool)probe.Allows(3)); Assert(accepted == 6);
            });
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Legacy harness setup failed: " + ex.GetType().Name + ": " + ex.Message);
            return 2;
        }

        foreach (int capacity in new[] { 2, 3, 4, 6 })
        {
            int limit = capacity;
            Test($"capacity_{limit}_includes_starting_and_running", "proposed_model", () => {
                var m = new LaunchModel(limit);
                var tickets = Enumerable.Range(0, limit).Select(i => m.Reserve("job" + i, 0, 100, true, true)!).ToArray();
                Assert(tickets.All(t => t != null) && m.Occupied == limit);
                Assert(m.Reserve("overflow", 0, 100, true, true) == null);
                for (int i = 0; i < limit; i++) Assert(m.Started(tickets[i], Fake(100 + i)));
                Assert(m.Occupied == limit && m.Reserve("overflow", 1, 100, true, true) == null);
            });
            Test($"capacity_{limit}_concurrent_reservations", "proposed_model", () => {
                var m = new LaunchModel(limit); var accepted = new ConcurrentBag<Ticket>();
                Parallel.For(0, 128, i => { var t = m.Reserve("parallel" + i, 0, 100, true, true); if (t != null) accepted.Add(t); });
                Assert(accepted.Count == limit && m.Occupied == limit);
            });
            Test($"capacity_{limit}_existing_clients_use_slots", "proposed_model", () => {
                var m = new LaunchModel(limit); for (int i = 0; i < limit - 1; i++) Assert(m.Attach(Fake(100 + i)));
                Assert(m.Reserve("last", 0, 100, true, true) != null);
                Assert(m.Reserve("overflow", 0, 100, true, true) == null);
            });
        }
        Test("six_slots_concurrency_stress_200_rounds", "proposed_model", () => {
            for (int round = 0; round < 200; round++) {
                var m = new LaunchModel(6); var jobs = new ConcurrentBag<Ticket>();
                Parallel.For(0, 64, i => { var t = m.Reserve("stress" + i, 0, 100, true, true); if (t != null) jobs.Add(t); });
                Assert(jobs.Count == 6 && m.Occupied == 6);
                Parallel.ForEach(jobs, t => Assert(m.Started(t, Fake((int)t.Id))));
                Assert(m.Occupied == 6);
            }
        });
        Test("duplicate_request_uses_one_slot", "proposed_model", () => {
            var m = new LaunchModel(6); var jobs = new ConcurrentBag<Ticket>();
            Parallel.For(0, 64, _ => { var t = m.Reserve("same", 0, 100, true, true); if (t != null) jobs.Add(t); });
            Assert(jobs.Count == 1 && m.Occupied == 1);
            Assert(m.Started(jobs.Single(), Fake(100)) && m.Reserve("same", 1, 100, true, true) == null);
        });
        Test("external_permission_denied_even_with_free_capacity", "proposed_model", () => {
            var m = new LaunchModel(6); Assert(m.Reserve("denied", 0, 100, false, true) == null && m.Occupied == 0);
        });
        Test("resource_pressure_denies_launch", "proposed_model", () => {
            var m = new LaunchModel(6); Assert(m.Reserve("pressure", 0, 100, true, false) == null && m.Occupied == 0);
        });
        Test("slow_start_holds_slot_until_ack", "proposed_model", () => {
            var m = new LaunchModel(1); var t = m.Reserve("slow", 0, 100, true, true)!;
            m.Poll(99); Assert(m.UnconfirmedTimeouts == 0 && m.Reserve("second", 99, 100, true, true) == null);
            Assert(m.Started(t, Fake(100)) && m.Occupied == 1 && m.Ready == 0);
            Assert(m.WindowReady(Fake(100)) && m.Ready == 1);
        });
        Test("timeout_is_not_proof_of_absence", "proposed_model", () => {
            var m = new LaunchModel(1); var t = m.Reserve("timeout", 0, 100, true, true)!;
            m.Poll(100); Assert(m.UnconfirmedTimeouts == 1 && m.Occupied == 1);
            Assert(m.Reserve("oversubscribe", 101, 100, true, true) == null);
            Assert(m.Started(t, Fake(100)) && m.UnconfirmedTimeouts == 0 && m.Occupied == 1);
        });
        Test("confirmed_failed_launch_releases_slot", "proposed_model", () => {
            var m = new LaunchModel(1); var t = m.Reserve("failed", 0, 100, true, true)!;
            Assert(m.ConfirmNoProcess(t) && m.Occupied == 0 && m.Reserve("retry", 1, 100, true, true) != null);
        });
        Test("unconfirmed_timeout_can_be_reconciled_absent", "proposed_model", () => {
            var m = new LaunchModel(1); var t = m.Reserve("failed", 0, 100, true, true)!;
            m.Poll(100); Assert(m.ConfirmNoProcess(t) && m.Occupied == 0 && m.UnconfirmedTimeouts == 0);
        });
        Test("discovery_before_ack_merges_same_identity", "proposed_model", () => {
            var m = new LaunchModel(2); var t = m.Reserve("starting", 0, 100, true, true)!;
            Assert(m.Attach(Fake(100)) && m.Occupied == 2); Assert(m.Started(t, Fake(100)) && m.Occupied == 1);
        });
        Test("double_ack_does_not_change_count", "proposed_model", () => {
            var m = new LaunchModel(2); var t = m.Reserve("once", 0, 100, true, true)!;
            Assert(m.Started(t, Fake(100)) && !m.Started(t, Fake(100)) && m.Occupied == 1);
        });
        Test("two_tickets_cannot_own_same_process", "proposed_model", () => {
            var m = new LaunchModel(2); var a = m.Reserve("a", 0, 100, true, true)!; var b = m.Reserve("b", 0, 100, true, true)!;
            Assert(m.Started(a, Fake(100)) && !m.Started(b, Fake(100)) && m.Occupied == 2);
        });
        Test("exit_frees_ready_process_slot", "proposed_model", () => {
            var m = new LaunchModel(1); m.Attach(Fake(100)); m.WindowReady(Fake(100));
            Assert(m.Exited(Fake(100)) && m.Occupied == 0 && m.Ready == 0 && !m.Exited(Fake(100)));
            Assert(m.Reserve("replacement", 0, 100, true, true) != null);
        });
        Test("pid_reuse_old_exit_does_not_remove_new_process", "proposed_model", () => {
            var m = new LaunchModel(2); m.Attach(Fake(100)); m.Exited(Fake(100));
            var replacement = new Identity(100, 20000); m.Attach(replacement);
            Assert(!m.Exited(Fake(100)) && m.Occupied == 1);
        });
        Test("preexisting_over_capacity_still_observed_no_more_launches", "proposed_model", () => {
            var m = new LaunchModel(2); for (int i = 0; i < 6; i++) m.Attach(Fake(100 + i));
            Assert(m.Occupied == 6 && m.Reserve("seventh", 0, 100, true, true) == null);
        });
        Test("unknown_ready_event_cannot_create_client", "proposed_model", () => {
            var m = new LaunchModel(6); Assert(!m.WindowReady(Fake(100)) && m.Occupied == 0);
        });
        Test("late_ack_after_confirmed_absence_is_rejected", "proposed_model", () => {
            var m = new LaunchModel(1); var t = m.Reserve("absent", 0, 100, true, true)!; m.ConfirmNoProcess(t);
            Assert(!m.Started(t, Fake(100)) && m.Occupied == 0);
            // Unexpected actual processes must still be attached by the independent observer.
            Assert(m.Attach(Fake(100)) && m.Occupied == 1);
        });
        Test("screenshot_warning_matches_only_owned_game", "dialog_candidate", () => {
            Assert(DialogObservation.IsKnownWarning(DialogObservation.Title, DialogObservation.Message, true));
            Assert(!DialogObservation.IsKnownWarning(DialogObservation.Title, DialogObservation.Message, false));
        });
        Test("warning_accepts_comma_width_and_line_breaks", "dialog_candidate", () => {
            Assert(DialogObservation.IsKnownWarning(DialogObservation.Title,
                "因为您运行的客户端过多，\r\n导致资源不足，无法启动！", true));
        });
        Test("other_game_error_dialog_is_rejected", "dialog_candidate", () => {
            Assert(!DialogObservation.IsKnownWarning(DialogObservation.Title, "无法连接服务器!", true));
            Assert(!DialogObservation.IsKnownWarning(DialogObservation.Title, "内存不足!", true));
            Assert(!DialogObservation.IsKnownWarning(DialogObservation.Title, "", true));
        });
        Test("different_title_is_rejected", "dialog_candidate", () => {
            Assert(!DialogObservation.IsKnownWarning("Other application", DialogObservation.Message, true));
        });
        Test("prefix_suffix_or_missing_warning_words_rejected", "dialog_candidate", () => {
            Assert(!DialogObservation.IsKnownWarning(DialogObservation.Title, "error " + DialogObservation.Message, true));
            Assert(!DialogObservation.IsKnownWarning(DialogObservation.Title, DialogObservation.Message + " fatal", true));
            Assert(!DialogObservation.IsKnownWarning(DialogObservation.Title, "客户端过多", true));
        });
        Test("cpu_percent_machine_normalized", "measurement_math", () => {
            Assert(Metrics.CpuPercent(4000, 1, 8) == 50 && Metrics.CpuPercent(0, 1, 8) == 0);
        });
        Test("cpu_invalid_intervals_rejected", "measurement_math", () => {
            foreach (double interval in new[] { 0.0, -1.0, double.NaN, double.PositiveInfinity }) {
                try { Metrics.CpuPercent(10, interval, 8); throw new Exception("Invalid interval accepted"); }
                catch (ArgumentOutOfRangeException) { }
            }
        });
        Test("csv_quoting_and_numbers_are_culture_invariant", "measurement_math", () => {
            var before = System.Globalization.CultureInfo.CurrentCulture;
            try { System.Globalization.CultureInfo.CurrentCulture = new("vi-VN");
                Assert(Metrics.Number(1.25) == "1.25" && Metrics.Csv("a\"b") == "\"a\"\"b\"");
            } finally { System.Globalization.CultureInfo.CurrentCulture = before; }
        });
        var summary = new {
            generated_utc = DateTimeOffset.UtcNow.ToString("O"),
            scope = "Selected legacy C# branches against owned fake APIs; proposed offline launch model; no game executable/DLL loaded.",
            passed = checks.Count(c => c.Passed), failed = checks.Count(c => !c.Passed),
            unsafe_legacy_behaviors_reproduced = checks.Count(c => c.Passed && c.Kind == "unsafe_legacy_reproduced"),
            capacities = new[] { 2, 3, 4, 6 }, concurrency_stress_rounds = 200,
            runtime_integration = false, windows_game_tested = false, checks
        };
        string output = Path.GetFullPath(args[1]); Directory.CreateDirectory(Path.GetDirectoryName(output)!);
        File.WriteAllText(output, JsonSerializer.Serialize(summary, new JsonSerializerOptions { WriteIndented = true }) + "\n");
        foreach (var check in checks.Where(c => !c.Passed)) Console.Error.WriteLine(check.Name + ": " + check.Failure);
        Console.WriteLine($"Passed {summary.passed}; failed {summary.failed}; unsafe legacy reproductions {summary.unsafe_legacy_behaviors_reproduced}.");
        return summary.failed == 0 ? 0 : 1;
    }
}
