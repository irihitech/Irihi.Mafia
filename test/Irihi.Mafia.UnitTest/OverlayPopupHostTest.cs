using Irihi.Mafia.Common;
using Irihi.Mafia.Controls.Primitives;
using MafiaOverlayHost = Irihi.Mafia.Controls.Primitives.OverlayPopupHost;

namespace Irihi.Mafia.UnitTest;

public class OverlayPopupHostTest
{
    [Fact]
    public void Default_Placement_Is_Bottom()
    {
        var host = new MafiaOverlayHost();
        Assert.Equal(PopupPlacement.Bottom, host.Placement);
    }

    [Fact]
    public void Default_IsModal_Is_False()
    {
        var host = new MafiaOverlayHost();
        Assert.False(host.IsModal);
    }

    [Fact]
    public void Default_MaskBrush_Is_Null()
    {
        var host = new MafiaOverlayHost();
        Assert.Null(host.MaskBrush);
    }

    [Fact]
    public void Placement_Change_Updates_PseudoClasses()
    {
        var host = new MafiaOverlayHost();

        host.Placement = PopupPlacement.Center;
        Assert.Contains(MafiaOverlayHost.PC_Center, host.Classes);
        Assert.DoesNotContain(MafiaOverlayHost.PC_Bottom, host.Classes);

        host.Placement = PopupPlacement.Top;
        Assert.Contains(MafiaOverlayHost.PC_Top, host.Classes);
        Assert.DoesNotContain(MafiaOverlayHost.PC_Center, host.Classes);

        host.Placement = PopupPlacement.Left;
        Assert.Contains(MafiaOverlayHost.PC_Left, host.Classes);
        Assert.DoesNotContain(MafiaOverlayHost.PC_Top, host.Classes);

        host.Placement = PopupPlacement.Right;
        Assert.Contains(MafiaOverlayHost.PC_Right, host.Classes);
        Assert.DoesNotContain(MafiaOverlayHost.PC_Left, host.Classes);

        host.Placement = PopupPlacement.FullScreen;
        Assert.Contains(MafiaOverlayHost.PC_FullScreen, host.Classes);
        Assert.DoesNotContain(MafiaOverlayHost.PC_Right, host.Classes);

        host.Placement = PopupPlacement.Bottom;
        Assert.Contains(MafiaOverlayHost.PC_Bottom, host.Classes);
        Assert.DoesNotContain(MafiaOverlayHost.PC_FullScreen, host.Classes);
    }
}
