using System.Linq;
using TinhKiemAuto.Dashboard;
using Xunit;
namespace ChickenAutoEx.Startup.Tests
{
    public sealed class CompactNavigationTests
    {
        [Theory][InlineData(0,0)][InlineData(1,1)][InlineData(2,4)][InlineData(3,7)]
        public void FourTabsMapToOriginalIndicesWithoutReordering(int front,int legacy)
        {Assert.Equal(legacy,CompactNavigationPolicy.LegacyIndex(front));Assert.Equal(front,CompactNavigationPolicy.FrontIndex(legacy));}
        [Theory][InlineData(2)][InlineData(3)][InlineData(5)][InlineData(6)]
        public void AdvancedSettingsStayReachableWithoutSelectingAnUnrelatedMainTab(int legacy)
        {Assert.Contains(legacy,CompactNavigationPolicy.AdvancedIndexes);Assert.Equal(-1,CompactNavigationPolicy.FrontIndex(legacy));}
        [Fact] public void AllEightExistingPagesRemainReachable()
        {
            var all=Enumerable.Range(0,4).Select(CompactNavigationPolicy.LegacyIndex).Concat(CompactNavigationPolicy.AdvancedIndexes).OrderBy(i=>i);
            Assert.Equal(Enumerable.Range(0,8),all);Assert.Equal(4,CompactNavigationPolicy.Captions.Length);
        }
    }
}
