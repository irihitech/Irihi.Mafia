using Irihi.Mafia.Common;
using Irihi.Mafia.Controls;

namespace Irihi.Mafia.UnitTest;

public class LinkTest
{
    [Fact]
    public void Default_Size_Is_Medium_And_Mirrors_Pseudo_Class()
    {
        var link = new Link();

        Assert.Equal(ControlSize.Medium, link.Size);
        Assert.Contains(":medium", link.Classes);
        Assert.DoesNotContain(":small", link.Classes);
        Assert.DoesNotContain(":large", link.Classes);
    }

    [Fact]
    public void Size_Change_Mirrors_Pseudo_Classes_And_Removes_Old_Ones()
    {
        var link = new Link { Size = ControlSize.Large };

        Assert.Contains(":large", link.Classes);
        Assert.DoesNotContain(":medium", link.Classes);

        link.Size = ControlSize.Small;

        Assert.Contains(":small", link.Classes);
        Assert.DoesNotContain(":medium", link.Classes);
        Assert.DoesNotContain(":large", link.Classes);
    }
}
