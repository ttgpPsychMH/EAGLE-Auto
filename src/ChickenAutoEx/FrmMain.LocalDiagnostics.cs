using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using TinhKiemAuto.Diagnostics;

namespace TinhKiemAuto
{
    public partial class FrmMain
    {
        private DiagnosticsHost localDiagnosticsHost;
        private sealed class DiagnosticAlias { internal int Value; }
        private readonly ConditionalWeakTable<Game, DiagnosticAlias> diagnosticAliases = new ConditionalWeakTable<Game, DiagnosticAlias>();
        private int nextDiagnosticAlias;
        private void InitializeLocalDiagnostics()
        {
            localDiagnosticsHost = new DiagnosticsHost(this, tùyChọnToolStripMenuItem, CaptureDiagnosticWindows);
        }
        private DiagnosticWindow[] CaptureDiagnosticWindows()
        {
            var result = new List<DiagnosticWindow>();
            foreach (ListViewItem item in ListViewNhanVat.Items)
            {
                var game = item.Tag as Game;
                if (game == null || result.Count >= 32) continue;
                DiagnosticAlias alias;
                if (!diagnosticAliases.TryGetValue(game, out alias))
                {
                    if (nextDiagnosticAlias >= 9999) continue;
                    diagnosticAliases.Add(game, alias = new DiagnosticAlias { Value = ++nextDiagnosticAlias });
                }
                result.Add(new DiagnosticWindow { Alias = alias.Value, Pid = game.ProcessId,
                    Flags = game.DiagnosticFlags(), Phases = game.DiagnosticPhases(), AcTac = game.DiagnosticAcTac(), TeamPatrol = game.DiagnosticTeamPatrol() });
            }
            return result.ToArray();
        }
    }
}
