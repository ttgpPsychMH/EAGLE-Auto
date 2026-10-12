using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MultiClientSimulation;

internal static class LegacyProbe
{
    public static dynamic Build(string repo)
    {
        var form = CSharpSyntaxTree.ParseText(File.ReadAllText(Path.Combine(repo, "src/ChickenAutoEx/FrmMain.cs"))).GetRoot();
        var game = CSharpSyntaxTree.ParseText(File.ReadAllText(Path.Combine(repo, "analysis/chickenautoex-107/recovered/TinhKiemAuto/Game.cs"))).GetRoot();
        var memory = CSharpSyntaxTree.ParseText(File.ReadAllText(Path.Combine(repo, "analysis/chickenautoex-107/recovered/TinhKiemAuto/Memory.cs"))).GetRoot();
        var monitor = form.DescendantNodes().OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.Text == "Monitor");
        var monitorBranch = monitor.DescendantNodes().OfType<IfStatementSyntax>()
            .Single(n => n.Condition.ToString() == "Win.GetHandle(current3.Id, \"#32770\") != IntPtr.Zero");
        var init = game.DescendantNodes().OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.Text == "Init");
        var initBranch = init.DescendantNodes().OfType<IfStatementSyntax>()
            .Single(n => n.Condition.ToString() == "handle != IntPtr.Zero");
        var write = memory.DescendantNodes().OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.Text == "Write"
            && m.ParameterList.Parameters.Count == 3 && m.ParameterList.Parameters[1].Type!.ToString() == "uint");
        var cap = form.DescendantNodes().OfType<IfStatementSyntax>()
            .Where(n => n.Condition.ToString() == "TienIch.GameCount() > 3").ToArray();
        if (cap.Length != 4) throw new InvalidOperationException("Legacy guard count changed: re-review required.");
        var source = "using System; namespace OwnedLegacyProbe { public class Probe {"
            + "public Memory Memory; public Address Address=new Address();"
            + "public void MonitorBranch(){var current3=new FakeProcess();" + monitorBranch + "}"
            + "public void InitBranch(){Memory=new Memory(100);var handle=Win.GetHandle(100,\"#32770\");" + initBranch + "}"
            + "public bool Allows(int count){TienIch.Count=count;return !(" + cap[0].Condition + ");}"
            + "} public class Memory {private IntPtr id; public Memory(int pid){} " + write
            + "private static bool WriteProcessMemory(IntPtr id,int address,byte[] bytes,int length,int ignored)"
            + "{State.WriteAttempts++;State.Writes.Add(address);return State.WriteSucceeds && State.WriteAttempts!=State.FailAt;} } }";
        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator)
            .Select(p => MetadataReference.CreateFromFile(p));
        var compilation = CSharpCompilation.Create("OwnedMultiClientLegacyProbe", new[] {
            CSharpSyntaxTree.ParseText(source), CSharpSyntaxTree.ParseText(Fakes)
        }, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        using var stream = new MemoryStream();
        var result = compilation.Emit(stream);
        if (!result.Success) throw new InvalidOperationException(string.Join("\n", result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)));
        return Activator.CreateInstance(Assembly.Load(stream.ToArray()).GetType("OwnedLegacyProbe.Probe")!)!;
    }
    public static dynamic State(dynamic probe) => Activator.CreateInstance(((object)probe).GetType().Assembly.GetType("OwnedLegacyProbe.State")!)!;
    private const string Fakes = """
using System;
using System.Collections.Generic;
namespace OwnedLegacyProbe {
 public class State {
  private static bool dialog=true,writes=true,post=true;private static int writeAttempts,posts,delay,failAt;
  private static List<int> addresses=new List<int>();
  public bool Dialog {get=>dialog;set=>dialog=value;} public bool WriteSucceedsValue {get=>writes;set=>writes=value;}
  public bool PostSucceedsValue {get=>post;set=>post=value;} public int Attempts=>writeAttempts;public int CloseRequests=>posts;public int Delay=>delay;
  public int FailAtValue {get=>failAt;set=>failAt=value;}public static int FailAt=>failAt;
  public static bool HasDialog=>dialog;public static bool WriteSucceeds=>writes;public static bool PostSucceeds=>post;
  public static int WriteAttempts {get=>writeAttempts;set=>writeAttempts=value;}public static List<int> Writes=>addresses;
  public static void Post(){posts++;}public static void Sleep(int ms){delay+=ms;}
  public void Reset(){dialog=true;writes=true;post=true;writeAttempts=0;posts=0;delay=0;failAt=0;addresses.Clear();}
 }
 public class FakeProcess {public int Id=100;}
 public class Address {public int MultiAcc=4096;public static Address GetInstance(string md,string offsets)=>new Address();}
 public static class Offset {public static string MD51="synthetic-build",OffList="synthetic-offsets";}
 public static class TienIch {public static int Count;public static int GameCount()=>Count;}
 public static class Thread {public static void Sleep(int ms)=>State.Sleep(ms);}
 public static class Win {public static IntPtr GetHandle(int pid,string name)=>State.HasDialog?new IntPtr(1):IntPtr.Zero;
  public static bool PostMessage(IntPtr hwnd,int message,int w,int l){if(message!=16)throw new Exception("Unexpected message");State.Post();return State.PostSucceeds;}}
}
""";
}
