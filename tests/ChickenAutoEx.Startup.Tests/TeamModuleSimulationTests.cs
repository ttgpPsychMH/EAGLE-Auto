using System;
using System.Collections.Generic;
using Xunit;

namespace ChickenAutoEx.Startup.Tests
{
    [Collection("Dungeon simulations")]
    public sealed class TeamModuleSimulationTests
    {
        [Theory, Trait("DungeonPartyBaseline", "yes")]
        [InlineData(0)][InlineData(1)][InlineData(2)][InlineData(3)]
        public void MovingLeaderDoesNotCancelTheDungeon(int kind)
        {
            dynamic g=Create(kind);g.TLBB.PlayerState=2;Tick(g,kind);
            Assert.True(Active(g,kind));
        }
        [Theory][InlineData(0)][InlineData(1)][InlineData(2)][InlineData(3)]
        public void AttackingLeaderIsObservedWithoutTravelCommand(int kind)
        {
            dynamic g=Create(kind);g.TLBB.PlayerState=7;Tick(g,kind);
            Assert.True(Active(g,kind));
            Assert.DoesNotContain((List<string>)g.Commands,c=>c.StartsWith("Move")||c.StartsWith("Goto"));
        }
        internal static dynamic Create(int kind)
        {
            dynamic g;
            switch(kind){case 0:g=KyCuocHarness.Create();g.TLBB.MapId=61;break;
                case 1:g=AcTacHarness.Create();g.TLBB.MapId=170;break;
                case 2:g=AcBaHarness.Create();g.Choose(1);g.TLBB.MapId=173;break;
                default:g=ThuyLaoHarness.Create();g.TLBB.MapId=66;break;}
            return g;
        }
        internal static void Tick(dynamic g,int kind){if(kind==3)g.DatDoiThuyLao();else g.Step();}
        internal static bool Active(dynamic g,int kind){switch(kind){case 0:return g.IsKyCuoc;case 1:return g.MapAcTac!=0;case 2:return g.IsAcBa;default:return g.IsThuyLao;}}
    }
    [CollectionDefinition("Dungeon simulations", DisableParallelization=true)]
    public sealed class DungeonSimulationCollection { }
}
