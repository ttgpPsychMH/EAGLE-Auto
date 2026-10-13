using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace ChickenAutoEx.Startup.Tests
{
    public sealed class TeamPatrolSimulationTests
    {
        [Theory][InlineData(0)][InlineData(1)][InlineData(2)][InlineData(3)]
        public void CommandsWaitForObservedMountAndFollowBeforeMoving(int kind)
        {
            dynamic g = TeamHarness.Create(kind), m = g.AddMember();
            Assert.False((bool)g.Prepare(false,false));
            Assert.Equal(new[]{"Mount"}, Commands((object)g)); Assert.Equal(new[]{"Mount"}, Commands((object)m));
            g.Now=1000;g.Prepare(false,false);Assert.Single(Commands((object)g));
            g.IsRide=m.IsRide=true;
            Assert.False((bool)g.Prepare(false,false));Assert.Contains("FollowAll",Commands((object)g));
            g.Now=3000;g.Prepare(false,false);Assert.Single(Commands((object)g),c=>c=="FollowAll");
            m.TLBB.IsFollow=true;
            Assert.True((bool)g.Prepare(false,false));Assert.DoesNotContain("Move",Commands((object)m));
            Assert.Equal(3,(int)g.Observation[1]);Assert.Equal(1,(int)g.Observation[5]);
        }
        [Theory][InlineData(0)][InlineData(1)][InlineData(2)][InlineData(3)]
        public void MovingAttackingIdleCyclePreservesTaskAndOrdersCombatPreparation(int kind)
        {
            dynamic g=TeamHarness.Create(kind),m=g.AddMember();g.IsRide=m.IsRide=true;m.TLBB.IsFollow=true;
            g.TLBB.PlayerState=m.TLBB.PlayerState=2;
            Assert.True((bool)g.Prepare(false,false));Assert.True((bool)g.Active);
            g.Prepare(true,false);Assert.False((bool)g.CombatReady);Assert.Equal(new[]{"Unfollow"},Commands((object)m));
            Assert.DoesNotContain("Dismount",Commands((object)m));
            m.TLBB.IsFollow=false;g.Now=1000;g.Prepare(true,false);
            Assert.Contains("Dismount",Commands((object)m));Assert.DoesNotContain("Post:105",Commands((object)m));
            g.IsRide=m.IsRide=false;g.Now=2000;g.Prepare(true,false);
            Assert.Equal(new[]{"Unfollow","Dismount","Post:50","Post:51","Post:105"},Commands((object)m));
            g.TLBB.PlayerState=m.TLBB.PlayerState=7;g.Now=3000;g.Prepare(true,false);Assert.Equal(7,(int)g.Observation[1]);Assert.True((bool)g.CombatReady);Assert.True((bool)m.CombatReady);
            Assert.True((bool)g.Active);g.ClearAt=3000;g.Now=4000;g.TLBB.PlayerState=m.TLBB.PlayerState=0;
            g.Prepare(false,false);Assert.DoesNotContain("Mount",Commands((object)g));
            g.Now=5001;g.Prepare(false,false);Assert.Contains("Mount",Commands((object)g));
        }
        [Theory][InlineData(0)][InlineData(1)][InlineData(2)][InlineData(3)]
        public void MissingFollowAckUsesThrottledSameMapFallback(int kind)
        {
            dynamic g=TeamHarness.Create(kind),m=g.AddMember();g.IsRide=m.IsRide=true;
            Assert.False((bool)g.Prepare(false,false));g.Now=5001;
            Assert.True((bool)g.Prepare(false,false));Assert.Contains("Move",Commands((object)m));
            g.Now=5500;g.Prepare(false,false);Assert.Single(Commands((object)m),c=>c=="Move");
            g.Now=8000;g.Prepare(false,false);Assert.Equal(2,Commands((object)m).Count(c=>c=="Move"));
            Assert.Single(Commands((object)g),c=>c=="FollowAll");
        }
        [Theory][InlineData(0)][InlineData(1)][InlineData(2)][InlineData(3)]
        public void UnconfirmedNativeMountStopsBoundedlyAndReleasesMembers(int kind)
        {
            dynamic g=TeamHarness.Create(kind),m=g.AddMember();g.Prepare(false,false);Assert.True((bool)m.Controlled);
            g.Now=10001;g.Prepare(false,false);Assert.False((bool)g.Active);Assert.False((bool)m.Controlled);
            int count=Commands((object)m).Count;g.Now=30000;g.Prepare(false,false);Assert.Equal(count,Commands((object)m).Count);
        }
        [Theory][InlineData(0)][InlineData(1)][InlineData(2)][InlineData(3)]
        public void AutoOffAndOtherMapMembersCannotReceiveOrTriggerTeamBroadcast(int kind)
        {
            dynamic g=TeamHarness.Create(kind),m=g.AddMember();g.IsRide=true;m.IsAuto=false;
            g.Prepare(false,false);Assert.Empty(Commands((object)m));Assert.DoesNotContain("FollowAll",Commands((object)g));
            m.IsAuto=true;m.TLBB.MapId=2;g.Prepare(false,false);Assert.Empty(Commands((object)m));
            Assert.DoesNotContain("FollowAll",Commands((object)g));
        }
        [Theory][InlineData(0)][InlineData(1)][InlineData(2)][InlineData(3)]
        public void FollowLossRequestsAgainOnlyAfterObservedSuccess(int kind)
        {
            dynamic g=TeamHarness.Create(kind),m=g.AddMember();g.IsRide=m.IsRide=true;m.TLBB.IsFollow=true;
            g.Prepare(false,false);m.TLBB.IsFollow=false;g.Now=100;g.Prepare(false,false);
            Assert.Single(Commands((object)g),c=>c=="FollowAll");
            m.TLBB.IsFollow=true;g.Now=1000;g.Prepare(false,false);
            m.TLBB.IsFollow=false;g.Now=4000;g.Prepare(false,false);Assert.Equal(2,Commands((object)g).Count(c=>c=="FollowAll"));
        }
        [Theory][InlineData(0)][InlineData(1)][InlineData(2)][InlineData(3)]
        public void CancelDuringStopCoordinatesPreventsRemainingNativeWrites(int kind)
        {
            dynamic g=TeamHarness.Create(kind),m=g.AddMember();g.TLBB.PlayerState=2;g.CancelOnPost=true;
            g.Prepare(true,false);Assert.Equal(new[]{"Post:50"},Commands((object)g));Assert.Empty(Commands((object)m));
        }
        [Theory][InlineData(0)][InlineData(1)][InlineData(2)][InlineData(3)]
        public void NoHorseAndDisabledCombatRemainExplicitPreferences(int kind)
        {
            dynamic g=TeamHarness.Create(kind);g.TLBB.HaveRide=false;g.IsAttack=false;
            Assert.True((bool)g.Prepare(false,false));Assert.DoesNotContain("Mount",Commands((object)g));
            g.Prepare(true,false);Assert.Equal((int)TinhKiemAuto.Diagnostics.DiagnosticReason.CombatDisabled,(int)g.Observation[8]);
            Assert.False((bool)g.IsAttack);
        }
        private static List<string> Commands(object g)=>(List<string>)g.GetType().GetField("Commands").GetValue(g);
    }
    internal static class TeamHarness
    {
        private static readonly Lazy<Assembly> Compiled=new Lazy<Assembly>(Compile);
        internal static dynamic Create(int kind)
        { dynamic g=Activator.CreateInstance(Compiled.Value.GetType("TinhKiemAuto.Game"));g.Configure(kind);return g; }
        private static Assembly Compile()
        {
            var trees=new List<SyntaxTree>{CSharpSyntaxTree.ParseText(Fixture)};DungeonHarnessSources.Add(trees);
            var refs=((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator)
                .Append(typeof(TinhKiemAuto.Diagnostics.DiagnosticReason).Assembly.Location).Distinct().Select(p=>MetadataReference.CreateFromFile(p));
            var c=CSharpCompilation.Create("TeamPatrolSimulation",trees,refs,new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using var bytes=new MemoryStream();var result=c.Emit(bytes);
            Assert.True(result.Success,string.Join("\n",result.Diagnostics.Where(d=>d.Severity==DiagnosticSeverity.Error)));return Assembly.Load(bytes.ToArray());
        }
        private const string Fixture="""
using System;using System.Collections.Generic;namespace TinhKiemAuto {
 public class TLBB {public string Id="leader",KeyId="leader";public int PlayerState,MapId=1;public bool Online=true,IsFollow,HaveRide=true,IsQuestOpen;}
 public static class TINHKIEM {public static float GetDistance(float x,float y,float a,float b)=>(float)Math.Sqrt((x-a)*(x-a)+(y-b)*(y-b));}
 public partial class Game {
  public TLBB TLBB=new TLBB();public bool IsAuto=true,IsRide,IsAttack=true,IsKyCuoc,IsAcBa,IsThuyLao,CancelOnPost;public int MapAcTac;
  public float CharX=50,CharY=50;public long Now,ClearAt=-10000;private DungeonTask kind;private Game owner;
  public List<Game> Party=new List<Game>();public List<string> Commands=new List<string>();
  public void Configure(int value){kind=(DungeonTask)value;switch(value){case 0:IsKyCuoc=true;break;case 1:MapAcTac=6;break;case 2:IsAcBa=true;break;case 3:IsThuyLao=true;break;}}
  public Game AddMember(){var m=new Game{owner=this,CharX=0,CharY=0};m.TLBB.Id="member";Party.Add(m);return m;}
  private bool Eligible(Game g)=>g!=null&&g.IsAuto&&g.TLBB!=null&&g.TLBB.Online&&DungeonMovementPolicy.CanObserve(g.TLBB.PlayerState);
  public bool Prepare(bool combat,bool loot){var people=new List<Game>{this};people.AddRange(Party);return PrepareDungeonPatrol(kind,people,Eligible,Now,ClearAt,2000,combat,loot,Stop);}
  public bool CombatReady=>DungeonCombatReady(); public bool Controlled=>DungeonControlsCurrentActor();public int[] Observation=>dungeonPatrolDiagnostic;public bool Active=>DungeonTaskActive(kind);
  private void Stop(string reason){IsKyCuoc=IsAcBa=IsThuyLao=false;MapAcTac=0;ResetDungeonPatrol(kind);}
  public void StopFollow(){Commands.Add("Unfollow");}public void UpRide(){Commands.Add("Mount");}public void DownRide(){Commands.Add("Dismount");}
  public void AskTeamFollow(){Commands.Add("FollowAll");}public void Move(float x,float y){Commands.Add("Move");}
  public void PostMessage(int value,int op){Commands.Add("Post:"+op);if(CancelOnPost)Stop("cancel");}
 }
}
""";
    }
}
