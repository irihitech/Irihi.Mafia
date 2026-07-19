using Irihi.Mafia.Common;
using Irihi.Mafia.Controls.Primitives;

namespace Irihi.Mafia.UnitTest;

public class PopupTest
{
    [Fact]
    public void Default_IsOpen_Is_False()
    {
        var popup = new Popup();
        Assert.False(popup.IsOpen);
    }

    [Fact]
    public void Default_IsModal_Is_True()
    {
        var popup = new Popup();
        Assert.True(popup.IsModal);
    }

    [Fact]
    public void Default_Placement_Is_Bottom()
    {
        var popup = new Popup();
        Assert.Equal(PopupPlacement.Bottom, popup.Placement);
    }

    [Fact]
    public void Default_IsLightDismissEnabled_Is_True()
    {
        var popup = new Popup();
        Assert.True(popup.IsLightDismissEnabled);
    }

    [Fact]
    public void Default_MaskBrush_Is_Null()
    {
        var popup = new Popup();
        Assert.Null(popup.MaskBrush);
    }

    [Fact]
    public void Default_Child_Is_Null()
    {
        var popup = new Popup();
        Assert.Null(popup.Child);
    }

    [Fact]
    public void Setting_IsOpen_True_Without_VisualRoot_Does_Not_Throw()
    {
        var popup = new Popup();
        popup.IsOpen = true;
        // No OverlayLayer available, so it silently does nothing
        Assert.True(popup.IsOpen);
    }
}
