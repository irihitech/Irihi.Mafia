using Irihi.Mafia.Common;
using Irihi.Mafia.Controls;

namespace Irihi.Mafia.UnitTest;

public class HyperlinkButtonTest
{
    [Fact]
    public void Default_Size_Is_Medium_And_Mirrors_Class()
    {
        var link = new HyperlinkButton();

        Assert.Equal(ControlSize.Medium, link.Size);
        Assert.Contains("Medium", link.Classes);
        Assert.DoesNotContain("Small", link.Classes);
        Assert.DoesNotContain("Large", link.Classes);
    }

    [Fact]
    public void Size_Change_Mirrors_Classes_And_Removes_Old_Ones()
    {
        var link = new HyperlinkButton { Size = ControlSize.Large };

        Assert.Contains("Large", link.Classes);
        Assert.DoesNotContain("Medium", link.Classes);

        link.Size = ControlSize.Small;

        Assert.Contains("Small", link.Classes);
        Assert.DoesNotContain("Medium", link.Classes);
        Assert.DoesNotContain("Large", link.Classes);
    }
}
