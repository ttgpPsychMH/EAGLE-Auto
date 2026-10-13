using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ChickenAutoEx.Startup.Tests
{
    internal static class DungeonHarnessSources
    {
        internal static void Add(List<SyntaxTree> trees, bool core = true)
        {
            string folder = Path.Combine(AppContext.BaseDirectory, "RegressionSources");
            trees.Add(CSharpSyntaxTree.ParseText(File.ReadAllText(Path.Combine(folder, "DungeonMovementPolicy.cs"))));
            if (!core) return;
            var root = CSharpSyntaxTree.ParseText(File.ReadAllText(Path.Combine(folder, "Game.DungeonPatrol.cs"))).GetRoot();
            var snapshot = root.DescendantNodes().OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.Text == "DungeonWindowSnapshot");
            // Fake the UI registry dependency only. All state transitions and native-command guards stay actual source.
            root = root.ReplaceNode(snapshot, snapshot.WithBody(SyntaxFactory.Block(
                SyntaxFactory.ParseStatement("return Party.ToArray();"))));
            trees.Add(CSharpSyntaxTree.ParseText(root.ToFullString()));
        }
    }
}
