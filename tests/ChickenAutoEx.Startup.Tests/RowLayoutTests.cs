using System;
using System.Linq;
using TinhKiemAuto.Appearance;
using Xunit;
namespace ChickenAutoEx.Startup.Tests
{
    public sealed class RowLayoutTests
    {
        [Theory][InlineData(1)][InlineData(1.25)][InlineData(1.5)][InlineData(2)]
        public void RecoveryCaptionAndInputColumnsNeverOverlapAtScaledSizes(double scale)
        {
            Func<int,int> s=v=>(int)Math.Ceiling(v*scale);
            var cells=new[]{
                new RowCell{Row=0,Column=0,Width=s(126),Height=s(24)},new RowCell{Row=0,Column=1,Width=s(16),Height=s(24)},
                new RowCell{Row=0,Column=2,Width=s(60),Height=s(28)},new RowCell{Row=0,Column=3,Width=s(110),Height=s(24)},
                new RowCell{Row=0,Column=4,Width=s(16),Height=s(24)},new RowCell{Row=0,Column=5,Width=s(60),Height=s(28)},
                new RowCell{Row=1,Column=0,Width=s(120),Height=s(24)},new RowCell{Row=1,Column=1,Width=s(16),Height=s(24)},
                new RowCell{Row=1,Column=2,Width=s(60),Height=s(28)},new RowCell{Row=1,Column=3,Width=s(175),Height=s(24)}};
            var plan=RowLayoutPolicy.Arrange(cells,6,2,s(12),s(28),s(8));
            foreach(var row in cells.GroupBy(c=>c.Row))
            {
                var sorted=row.OrderBy(c=>c.Column).ToArray();
                for(int i=1;i<sorted.Length;i++)Assert.True(plan.X[sorted[i].Column]>=plan.X[sorted[i-1].Column]+sorted[i-1].Width+s(8));
                foreach(var cell in sorted)Assert.True(plan.Width>=plan.X[cell.Column]+cell.Width+s(12));
            }
            Assert.True(plan.Y[1]>=plan.Y[0]+plan.RowHeights[0]+s(8));
            Assert.True(plan.Height>=plan.Y[1]+s(28)+s(12));
        }
        [Fact] public void StandaloneLongCaptionCanSpanUnusedInputColumns()
        {
            var cells=new[]{new RowCell{Row=0,Column=0,Width=100,Height=24},new RowCell{Row=0,Column=1,Width=60,Height=28},new RowCell{Row=1,Column=0,Width=180,Height=24}};
            var plan=RowLayoutPolicy.Arrange(cells,2,2,12,28,8);
            Assert.Equal(120,plan.X[1]); Assert.Equal(204,plan.Width);
        }
        [Fact] public void EqualColumnCaptionsRemainAlignedAcrossDifferentRows()
        {
            var cells=new[]{new RowCell{Row=0,Column=0,Width=50,Height=24},new RowCell{Row=0,Column=1,Width=60,Height=28},new RowCell{Row=1,Column=0,Width=126,Height=24},new RowCell{Row=1,Column=1,Width=60,Height=28}};
            var plan=RowLayoutPolicy.Arrange(cells,2,2,12,28,8);Assert.Equal(146,plan.X[1]);
        }
    }
}
