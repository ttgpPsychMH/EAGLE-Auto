using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TinhKiemAuto.Hooks;
using Xunit;

namespace ChickenAutoEx.Startup.Tests
{
    public sealed class HookDiagnosticsTests
    {
        private static HookDiagnostics Ready()
        {
            var h = new HookDiagnostics(); h.WindowHookResult(true); h.ReceiverResult(true, 0); return h;
        }
        [Fact] public void QueueAcceptanceDoesNotClaimNativeAck()
        {
            var h=Ready(); Assert.True(h.ReadResult(true,true,10,0)); Assert.True(h.ReaderQueueResult(true,0));
            Assert.Equal(ReaderObservation.RequestQueuedNoAck,h.Reader); Assert.False(h.CanRequestReader);
            Assert.Contains("no ACK",h.Report(4242)); Assert.DoesNotContain("Installed",h.Report(4242));
        }
        [Theory][InlineData(false,true)][InlineData(true,false)]
        public void BothWindowHookAndReceiverQueueAreRequired(bool window,bool receiver)
        {
            var h=new HookDiagnostics();h.WindowHookResult(window);h.ReceiverResult(receiver,5);
            Assert.False(h.ReadResult(true,true,10,0));Assert.False(h.ReaderQueueResult(true,0));
            Assert.Equal(ReaderObservation.NotRequested,h.Reader);
        }
        [Theory][InlineData(false,true,10)][InlineData(true,false,10)][InlineData(true,true,0)][InlineData(true,true,9)]
        public void InvalidRecvReadDoesNotBecomeQueued(bool address,bool success,int read)
        {var h=Ready();Assert.False(h.ReadResult(address,success,read,299));Assert.Equal(ReaderObservation.NotRequested,h.Reader);}
        [Fact] public void RejectedQueueCanBeRetriedWithoutClaimingInstallation()
        {var h=Ready();Assert.False(h.ReaderQueueResult(false,5));Assert.True(h.CanRequestReader);Assert.Contains("ReaderQueueRejected",h.Report(4242));}
        [Fact] public void MatchingCopyDataIsOnlyTransportEvidence()
        {
            var h=Ready();h.ReaderQueueResult(true,0);h.ObserveCopyData(0,20,20);
            Assert.Equal(ReaderObservation.DataObserved,h.Reader);Assert.Contains("Observed payload bytes: 16",h.Report(4242));
        }
        [Theory][InlineData(1,20,20)][InlineData(0,19,20)][InlineData(0,15,15)][InlineData(0,1000,1000)]
        public void InvalidShapeCannotConfirmReader(int tag,int sender,int copied)
        {var h=Ready();h.ReaderQueueResult(true,0);h.ObserveCopyData(tag,sender,copied);Assert.Equal(ReaderObservation.RequestQueuedNoAck,h.Reader);}
        [Fact] public void UnsolicitedCopyDataCannotConfirmReader()
        {var h=Ready();h.ObserveCopyData(0,20,20);Assert.Equal(ReaderObservation.NotRequested,h.Reader);Assert.Contains("CopyDataUnexpected",h.Report(4242));}
        [Theory][InlineData(false,0)][InlineData(false,10)][InlineData(true,9)]
        public void FailedRestoreBlocksAnotherRequest(bool ok,int count)
        {var h=Ready();h.ReaderQueueResult(true,0);Assert.False(h.RestoreResult(ok,count,5));Assert.Equal(ReaderObservation.RestoreFailed,h.Reader);Assert.False(h.CanRequestReader);}
        [Fact] public void SuccessfulWriteIsStillNotNativeCleanupAck()
        {var h=Ready();h.ReaderQueueResult(true,0);Assert.True(h.RestoreResult(true,10,0));Assert.Equal(ReaderObservation.RestoreWrittenNoAck,h.Reader);}
        [Fact] public void WindowReinitializationDoesNotEraseOutstandingReader()
        {var h=Ready();h.ReaderQueueResult(true,0);h.WindowHookResult(true);h.ReceiverResult(true,0);Assert.False(h.CanRequestReader);}
        [Fact] public void RepeatedFailuresKeepExactCountersAndBoundedHistory()
        {
            var h=Ready();for(int i=0;i<10000;i++)h.ReaderQueueResult(false,5);
            for(int i=0;i<100;i++)h.ExceptionResult(i);
            var report=h.Report(4242);Assert.Contains("ReaderQueueRejected count=10000",report);
            Assert.Equal(HookDiagnostics.HistoryLimit,report.Split('\n').Count(s=>s.Contains("T")&&s.Contains(" value=")));
        }
        [Fact] public void ConcurrentObservationsKeepExactCounters()
        {var h=Ready();h.ReaderQueueResult(true,0);Parallel.For(0,1000,_=>h.ObserveCopyData(0,20,20));Assert.Contains("CopyDataObserved count=1000",h.Report(4242));Assert.Contains("Observed payload bytes: 16000",h.Report(4242));}

        [Fact] public void ActualManagedHookPathQueuesExistingAbiOnlyOnce()
        {
            dynamic g=HookHarness.Create();g.Initialize();g.Receiver();g.Step();g.Step();
            Assert.True((bool)g.IsHooked);Assert.Equal(1,(int)g.ReaderPosts);Assert.Contains("RequestQueuedNoAck",(string)g.HookDiagnosticReport);
            Assert.Equal(-5,(int)g.ReceiverCommand);Assert.Equal(-10,(int)g.ReaderCommand);Assert.Equal((int)g.ProcessId,(int)g.ReaderValue);
        }
        [Fact] public void SetHookFalseIsDiagnosedAndReaderNotRequested()
        {dynamic g=HookHarness.Create();g.WindowOk=false;g.Initialize();g.Receiver();g.Step();Assert.False((bool)g.IsHooked);Assert.Equal(0,(int)g.ReadCalls);Assert.Equal(0,(int)g.ReaderPosts);Assert.Contains("WindowHookRejected",(string)g.HookDiagnosticReport);}
        [Fact] public void ReceiverPostFalseIsDiagnosedAndReaderNotRequested()
        {dynamic g=HookHarness.Create();g.Initialize();g.PostOk=false;g.Receiver();g.PostOk=true;g.Step();Assert.False((bool)g.IsHooked);Assert.Equal(0,(int)g.ReadCalls);Assert.Contains("ReceiverQueueRejected",(string)g.HookDiagnosticReport);}
        [Fact] public void ReaderPostFalseDoesNotSetLegacyFlag()
        {dynamic g=HookHarness.Create();g.Initialize();g.Receiver();g.PostOk=false;g.Step();Assert.False((bool)g.IsHooked);Assert.Contains("ReaderQueueRejected",(string)g.HookDiagnosticReport);}
        [Theory][InlineData(0,true,10)][InlineData(123,false,10)][InlineData(123,true,9)]
        public void ProductionPreflightReportsMissingAndPartialRead(int address,bool success,int bytes)
        {dynamic g=HookHarness.Create();g.Initialize();g.Receiver();g.AddressResult=address;g.ReadOk=success;g.BytesRead=bytes;g.Step();Assert.False((bool)g.IsHooked);Assert.Equal(0,(int)g.ReaderPosts);}
        [Theory][InlineData(0)][InlineData(123)][InlineData(-1)][InlineData(65536)]
        public void InvalidRegisteredMessageNeverPosts(int message)
        {dynamic g=HookHarness.Create();g.Initialize();g.MessageId(message);g.Receiver();g.Step();Assert.Equal(0,(int)g.ReceiverPosts);Assert.Equal(0,(int)g.ReaderPosts);Assert.False((bool)g.IsHooked);}
        [Fact] public void ResolverExceptionIsRedactedAndKeepsExistingAcBaFallback()
        {
            dynamic g=HookHarness.Create();g.Initialize();g.Receiver();g.ThrowResolver=true;g.Step();
            Assert.Equal(1,(int)g.AcBaStops);Assert.Contains("ExceptionObserved",(string)g.HookDiagnosticReport);
            Assert.DoesNotContain("SECRET",(string)g.HookDiagnosticReport);Assert.False((bool)g.IsHooked);
        }
        [Fact] public void NativeSetHookExceptionIsRecordedAndRethrown()
        {dynamic g=HookHarness.Create();g.ThrowInstall=true;Assert.Throws<InvalidOperationException>(()=>{g.Initialize();});Assert.Contains("ExceptionObserved",(string)g.HookDiagnosticReport);}
        [Fact] public void CleanupFailureKeepsLegacyGuardAndCanBeRetried()
        {
            dynamic g=HookHarness.Create();g.Initialize();g.Receiver();g.Step();g.WriteOk=false;g.UnHookRecv();g.Step();
            Assert.True((bool)g.IsHooked);Assert.Equal(1,(int)g.ReaderPosts);Assert.Contains("RestoreFailed",(string)g.HookDiagnosticReport);
            g.WriteOk=true;g.UnHookRecv();Assert.False((bool)g.IsHooked);Assert.Contains("RestoreWrittenNoAck",(string)g.HookDiagnosticReport);
        }
        [Fact] public void PartialRestoreDoesNotClearLegacyGuard()
        {dynamic g=HookHarness.Create();g.Initialize();g.Receiver();g.Step();g.BytesWritten=9;g.UnHookRecv();Assert.True((bool)g.IsHooked);}
        [Fact] public void ActualReceiveObserverUpdatesDiagnosticWithoutPayload()
        {dynamic g=HookHarness.Create();g.Initialize();g.Receiver();g.Step();g.ObserveHookCopyData(0L,20L,20);Assert.Contains("Reader observation: DataObserved",(string)g.HookDiagnosticReport);}
        [Fact] public void OriginalDllAbiDeclarationsRemainUntouchedAndWinApisCaptureErrors()
        {
            var original=HookHarness.Class("LegacyGame.cs","Game");
            Assert.Contains(original.Members.OfType<MethodDeclarationSyntax>(),m=>m.Identifier.Text=="SetHook"&&m.ReturnType.ToString()=="IntPtr");
            var partial=HookHarness.Class("Game.HookDiagnostics.cs","Game");
            foreach(var method in partial.Members.OfType<MethodDeclarationSyntax>().Where(m=>m.Modifiers.Any(SyntaxKind.ExternKeyword)))
                Assert.Contains("SetLastError = true",method.ToFullString());
            Assert.DoesNotContain("GetLastWin32Error",partial.Members.OfType<MethodDeclarationSyntax>().Single(m=>m.Identifier.Text=="InstallWindowHookWithDiagnostics").ToFullString());
        }
    }

    internal static class HookHarness
    {
        private static readonly Lazy<Assembly> Compiled=new Lazy<Assembly>(Compile);
        internal static dynamic Create()
        { dynamic game=Activator.CreateInstance(Compiled.Value.GetType("TinhKiemAuto.Game"));game.MessageId(0xC123);return game; }
        internal static string Read(string path)=>File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"RegressionSources",path));
        internal static ClassDeclarationSyntax Class(string path,string name)=>CSharpSyntaxTree.ParseText(Read(path)).GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Single(c=>c.Identifier.Text==name);
        private static Assembly Compile()
        {
            var partial=Class("Game.HookDiagnostics.cs","Game");
            var natives=partial.Members.OfType<MethodDeclarationSyntax>().Where(m=>m.Modifiers.Any(SyntaxKind.ExternKeyword)).ToArray();
            partial=partial.ReplaceNodes(natives,(old,_)=>old.WithAttributeLists(default).WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword)))
                .WithSemicolonToken(default).WithBody(SyntaxFactory.ParseStatement(old.Identifier.Text switch {
                    "HookPostMessage"=>"{if(command.ToInt32()==-5){ReceiverPosts++;ReceiverCommand=command.ToInt32();}else {ReaderPosts++;ReaderCommand=command.ToInt32();ReaderValue=value.ToInt32();}return PostOk;}",
                    "HookReadMemory"=>"{ReadCalls++;read=new IntPtr(BytesRead);return ReadOk;}",
                    "HookWriteMemory"=>"{WriteCalls++;written=new IntPtr(BytesWritten);return WriteOk;}",_=>throw new Exception("Unexpected API")
                }) as BlockSyntax).NormalizeWhitespace());
            var reviewed=Class("HookReviewedGame.cs","Game");
            var init=reviewed.Members.OfType<MethodDeclarationSyntax>().Single(m=>m.Identifier.Text=="Init").DescendantNodes().OfType<ExpressionStatementSyntax>().Single(s=>s.ToString()=="InstallWindowHookWithDiagnostics();");
            var receiver=reviewed.Members.OfType<MethodDeclarationSyntax>().Single(m=>m.Identifier.Text=="SetDll").DescendantNodes().OfType<ExpressionStatementSyntax>().Single(s=>s.ToString().StartsWith("QueueHookReceiver("));
            var block=reviewed.Members.OfType<MethodDeclarationSyntax>().Single(m=>m.Identifier.Text=="Auto").DescendantNodes().OfType<IfStatementSyntax>().Single(i=>i.Condition.ToString().StartsWith("!IsHooked &&"));
            var restore=reviewed.Members.OfType<MethodDeclarationSyntax>().Single(m=>m.Identifier.Text=="UnHookRecv");
            var text="using System;using System.Runtime.InteropServices;using TinhKiemAuto.Hooks;namespace TinhKiemAuto {"+Fixture+partial.ToFullString()+
                "public partial class Game { public void Initialize(){"+init+"}public void Receiver(){"+receiver+"}public void Step(){"+block+"}"+restore+"} }";
            var refs=((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator).Select(p=>MetadataReference.CreateFromFile(p)).Append(MetadataReference.CreateFromFile(typeof(HookDiagnostics).Assembly.Location));
            var compilation=CSharpCompilation.Create("HookSubsetSimulation",new[]{CSharpSyntaxTree.ParseText(text)},refs,new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using var bytes=new MemoryStream();var result=compilation.Emit(bytes);Assert.True(result.Success,string.Join("\n",result.Diagnostics.Where(d=>d.Severity==DiagnosticSeverity.Error)));return Assembly.Load(bytes.ToArray());
        }
        private const string Fixture=@"
            public static class Global{public static int HookMessage=0xC123;}
            public static class Option{public static bool AlarmChat;}
            public class FakeForm{public IntPtr Handle=new IntPtr(84);}
            public static class FrmMain{public static bool AlarmAcBa;public static FakeForm Instance=new FakeForm();}
            public sealed class Memory{public IntPtr Id=new IntPtr(99);}
            public sealed class FakeTlbb{public bool IsLeader=true;}
            public partial class Game{
                public bool IsHooked,AlarmChat,RequiresAcBaNotices=true,WindowOk=true,PostOk=true,ReadOk=true,WriteOk=true,ThrowResolver,ThrowInstall;
                public int ReadCalls,WriteCalls,ReaderPosts,ReceiverPosts,ReceiverCommand,ReaderCommand,ReaderValue,AcBaStops;
                public int BytesRead=10,BytesWritten=10,RecvAddress,AddressResult=123;public byte[] bufferRecv=new byte[10];
                public int ProcessId=System.Environment.ProcessId;public IntPtr Handle=new IntPtr(42);public Memory Memory=new Memory();public FakeTlbb TLBB=new FakeTlbb();
                private IntPtr SetHook(IntPtr handle){if(ThrowInstall)throw new InvalidOperationException(""SECRET account/password/token"");return WindowOk?new IntPtr(1):IntPtr.Zero;}
                private IntPtr GetRemoteProcAddress(System.Diagnostics.Process p,string dll,string symbol){if(ThrowResolver)throw new InvalidOperationException(""SECRET packet/chat/credential"");return new IntPtr(AddressResult);}
                private void StopAcBa(string text){AcBaStops++;}
                public void MessageId(int value){Global.HookMessage=value;}
            }";
    }
}
